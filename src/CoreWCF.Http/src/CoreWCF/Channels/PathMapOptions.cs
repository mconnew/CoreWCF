// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace CoreWCF.Channels
{
    internal class PathMapOptions
    {
        public RequestDelegate Branch { get; set; }
        public PathString PathMatch { get; set; }
        public bool MatchFromRoot { get; set; }
    }
}
