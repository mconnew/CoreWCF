using System;
using CoreWCF.Channels;
using Microsoft.Extensions.Options;

namespace CoreWCF.Configuration
{
    public class UnixDomainSocketOptionsSetup : IConfigureOptions<Channels.UnixDomainSocketOptions>
    {
        private readonly IServiceProvider _services;

        public UnixDomainSocketOptionsSetup(IServiceProvider services)
        {
            _services = services;
        }

        public void Configure(Channels.UnixDomainSocketOptions options)
        {
            options.ApplicationServices = _services;
        }
    }
}

