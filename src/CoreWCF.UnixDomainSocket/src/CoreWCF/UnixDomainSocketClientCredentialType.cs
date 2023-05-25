using System;
namespace CoreWCF
{
    public enum UnixDomainSocketClientCredentialType
    {
        None,
        Default ,
        Certificate ,
        Windows ,
        IdentityOnly,
    }

    internal static class UnixDomainSocketClientCredentialTypeHelper
    {
        internal static bool IsDefined(UnixDomainSocketClientCredentialType value)
        {
            return (value == UnixDomainSocketClientCredentialType.None ||
                value == UnixDomainSocketClientCredentialType.Certificate ||
                value == UnixDomainSocketClientCredentialType.Default ||
                value == UnixDomainSocketClientCredentialType.Windows ||
                value == UnixDomainSocketClientCredentialType.IdentityOnly 
                );
        }
    }
}

