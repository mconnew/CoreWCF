using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Versioning;
using System.Web.Services.Description;
using CoreWCF.Channels;
using CoreWCF.Channels.Framing;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreWCF.Configuration
{
    public static class UnixDomainSocketHostBuilderExtensions
    {

        public static IHostBuilder UseUnixDomainSocket(this IHostBuilder hostBuilder, Action<UnixDomainSocketOptions> options)
        {
            hostBuilder.ConfigureUnixDomainSocket(options);
            return hostBuilder;
        }

        internal static IHostBuilder ConfigureUnixDomainSocket(this IHostBuilder hostBuilder, Action<UnixDomainSocketOptions> options)
        {
            return hostBuilder.ConfigureServices((builderContext, services) => {
                services.AddNetFramingServices();
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, UnixDomainSocketHostedService>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<UnixDomainSocketOptions>, UnixDomainSocketOptionsSetup>());
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<KestrelServerOptions>, UnixDomainSocketFramingOptionsSetup>());
                services.TryAddSingleton<SocketTransportFactory>();
                services.Configure(options);
            });
        }
       
    }

}

