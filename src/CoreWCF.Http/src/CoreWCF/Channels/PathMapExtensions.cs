// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;

namespace CoreWCF.Channels
{
    internal static class PathMapExtensions
    {
        public static IApplicationBuilder PathMap(this IApplicationBuilder app, string pathMatch, Action<IApplicationBuilder> configuration)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!string.IsNullOrWhiteSpace(pathMatch) && pathMatch.EndsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException("The path must not end with a '/'", nameof(pathMatch));
            }

            // create branch
            var branchBuilder = app.New();
            configuration(branchBuilder);
            var branch = branchBuilder.Build();
            bool isAbsolutePath = pathMatch.StartsWith("/");

            var options = new PathMapOptions
            {
                Branch = branch,
                PathMatch = isAbsolutePath ? pathMatch : ("/" + pathMatch),
                MatchFromRoot = isAbsolutePath
            };

            return app.Use(next => new PathMapMiddleware(next, options).Invoke);
        }
    }
}
