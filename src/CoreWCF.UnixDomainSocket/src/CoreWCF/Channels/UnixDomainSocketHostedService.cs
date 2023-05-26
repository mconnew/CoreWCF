﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreWCF.Channels;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWCF.Channels
{
    internal class UnixDomainSocketHostedService : IHostedService
    {
        private static bool s_isNetFramework => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

        private readonly IServiceBuilder _serviceBuilder;
        private readonly ILogger<UnixDomainSocketHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private KestrelServer _kestrel;
        private CancellationTokenRegistration _applicationStartedRegistration;

        public UnixDomainSocketHostedService(IServiceBuilder serviceBuilder, ILogger<UnixDomainSocketHostedService> logger, IServiceProvider serviceProvider)
        {
            // We request the IServer in the constructor to trigger the constructor of the implementation. If the implementation
            // is KestrelServer it will run NetTcpFramingOptionsSetup.Configure, which we use to detect the server is Kestrel.
            // We can't just examine the type as we wrap it so we can throw any startup exceptions using WrappingIServer.
            // The IServer will have already been started on asp.net core 2.1, but later versions start it after IHostedService's
            _serviceBuilder = serviceBuilder;
            _logger = logger;
            _serviceProvider = serviceProvider;
            IApplicationLifetime appLifetime = _serviceProvider.GetRequiredService<IApplicationLifetime>();
            // asp.net core 2.1 executes the ApplicationStarted registered callback before it starts hosted services
            if (!s_isNetFramework)
            {
                _applicationStartedRegistration = appLifetime.ApplicationStarted.Register(UpdateServerAddressesFeature);
            }
            Init();
        }

        private void Init()
        {
            // Check if Kestrel is already being used and if it is, it was already configured so there's nothing to do.
            var unixDomainSocketFramingOptionsSetup = _serviceProvider.GetServices<IConfigureOptions<KestrelServerOptions>>().SingleOrDefault(options => options is UnixDomainSocketFramingOptionsSetup) as UnixDomainSocketFramingOptionsSetup;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var transportFactory = _serviceProvider.GetRequiredService<SocketTransportFactory>();
            var unixDomainSocketFramingOptionsSetup = _serviceProvider.GetServices<IConfigureOptions<KestrelServerOptions>>().SingleOrDefault(options => options is UnixDomainSocketFramingOptionsSetup) as UnixDomainSocketFramingOptionsSetup;
            unixDomainSocketFramingOptionsSetup.AttachUDS = true;
            _kestrel = ActivatorUtilities.CreateInstance(_serviceProvider, typeof(KestrelServer), transportFactory) as KestrelServer;
            Debug.Assert(unixDomainSocketFramingOptionsSetup.AttachUDS);

            await _kestrel.StartAsync<int>(null, cancellationToken);

            /* IS this needed ?
            // As we don't register the ApplicationStarted callback when running on .NET Framework, and we know that the IServer
            // has already started before us, we can fixup the server addresses now.
            if (s_isNetFramework)
            {
                UpdateServerAddressesFeature();
                // Need to update base addresses as ServiceBuilder was opened by wrapping WrappingIServer
                var framingOptionsSetup = _serviceProvider.GetRequiredService<NetTcpFramingOptionsSetup>();
                framingOptionsSetup.UpdateServiceBuilderBaseAddresses();
            }
            */
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (!s_isNetFramework)
            {
                _applicationStartedRegistration.Dispose();
            }

            return _kestrel.StopAsync(cancellationToken);
        }

        private void UpdateServerAddressesFeature()
        {
            // This method needs to be called from appLifetime.ApplicationStarted otherwise an IServer might try to listen on
            // the address that Kestrel is listening on. It's not needed if Kestrel is the IServer implementation for the WebHost
            var kestrelServerAddresses = _kestrel.Features.Get<IServerAddressesFeature>();
            var serverAddressesFeature = _serviceProvider.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
            foreach (var address in kestrelServerAddresses.Addresses)
            {
                serverAddressesFeature.Addresses.Add(address);
            }
        }
    }
}
