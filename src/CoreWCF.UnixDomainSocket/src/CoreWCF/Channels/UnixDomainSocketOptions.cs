using System;
using System.Collections.Generic;
using CoreWCF.Configuration;

namespace CoreWCF.Channels
{
    public class UnixDomainSocketOptions
    {
        internal List<UnixDomainSocketListenOptions> CodeBackedListenOptions { get; } = new List<UnixDomainSocketListenOptions>();
        public IServiceProvider ApplicationServices { get; internal set; }

        public void Listen(string baseAddress) => Listen(new Uri(baseAddress));
        public void Listen(Uri baseAddress) => Listen(baseAddress, _ => { });

        public void Listen(string baseAddress, Action<UnixDomainSocketListenOptions> configure) => Listen(new Uri(baseAddress), configure);
        public void Listen(Uri baseAddress, Action<UnixDomainSocketListenOptions> configure)
        {
            if (baseAddress == null) throw new ArgumentNullException(nameof(baseAddress));
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var listenOptions = new UnixDomainSocketListenOptions(baseAddress);
            ApplyEndpointDefaults(listenOptions);
            configure(listenOptions);
            CodeBackedListenOptions.Add(listenOptions);
        }

        private void ApplyEndpointDefaults(UnixDomainSocketListenOptions listenOptions)
        {
            listenOptions.UnixDomainSocketServerOptions = this;
        }
    }
}

