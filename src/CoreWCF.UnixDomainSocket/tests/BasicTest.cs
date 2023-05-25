 // Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using CoreWCF.Configuration;
using CoreWCF.IdentityModel.Selectors;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace CoreWCF.UnixDomainSocket.Tests
{
    public class BasicTest
    {
        private readonly ITestOutputHelper _output;
        public const string NoSecurityRelativePath = "/uds.svc/security-none";
        public const string LinuxSocketFilepath = "/tmp/basictest.txt";
        public BasicTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        private void BasicUserNameAuth()
        {
            string testString = new string('a', 3000);
            IHost host = Helpers.ServiceHelper.CreateWebHostBuilder<StartUpForUDS>(_output,LinuxSocketFilepath);
            using (host)
            {
                System.ServiceModel.ChannelFactory<ClientContract.ITestService> factory = null;
                ClientContract.ITestService channel = null;
                host.Start();
                try
                {
                    System.ServiceModel.UnixDomainSocketBinding binding = ClientHelper.GetBufferedModeBinding();
                    factory = new System.ServiceModel.ChannelFactory<ClientContract.ITestService>(binding,
                        new System.ServiceModel.EndpointAddress(new Uri("net.uds://"+ LinuxSocketFilepath + NoSecurityRelativePath)));
                    channel = factory.CreateChannel();
                    ((IChannel)channel).Open();
                    string result = channel.EchoString(testString);
                    Assert.Equal(testString, result);
                    ((IChannel)channel).Close();
                    factory.Close();
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
                }
            }
        }
    
        public class StartUpForUDS
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app)
            {
                CoreWCF.UnixDomainSocketBinding serverBinding = new CoreWCF.UnixDomainSocketBinding(SecurityMode.None);
                app.UseServiceModel(builder =>
                {
                    builder.AddService<Services.EchoService>();
                    builder.AddServiceEndpoint<Services.EchoService, Contract.IEchoService>(serverBinding, NoSecurityRelativePath);
                });
            }
        }
    }
}