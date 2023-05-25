// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CoreWCF.Channels
{
    public sealed class UnixDomainSocketConnectionPoolSettings : ConnectionPoolSettings
    {
        //string _groupName;
        //TimeSpan _leaseTimeout;

        internal UnixDomainSocketConnectionPoolSettings()
        {
            //_groupName = ConnectionOrientedTransportDefaults.ConnectionPoolGroupName;
            //_leaseTimeout = TcpTransportDefaults.ConnectionLeaseTimeout;
        }

        internal UnixDomainSocketConnectionPoolSettings(UnixDomainSocketConnectionPoolSettings tcp) : base(tcp)
        {
            //_groupName = tcp._groupName;
            //_leaseTimeout = tcp._leaseTimeout;
        }

        internal UnixDomainSocketConnectionPoolSettings Clone()
        {
            return new UnixDomainSocketConnectionPoolSettings(this);
        }
    }
}
