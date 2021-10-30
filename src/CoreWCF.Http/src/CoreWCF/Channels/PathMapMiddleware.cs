// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoreWCF.Channels
{
    internal class PathMapMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PathMapOptions _options;

        public PathMapMiddleware(RequestDelegate next, PathMapOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Branch == null)
            {
                throw new ArgumentException("Branch not set on options.", nameof(options));
            }

            _next = next;
            _options = options;
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task that represents the execution of this middleware.</returns>
        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            PathString matchPath = context.Request.Path;
            if (_options.MatchFromRoot)
            {
                matchPath = context.Request.PathBase.Add(context.Request.Path);
            }

            if (context.Request.Path.StartsWithSegments(_options.PathMatch, out var matchedPath, out var remainingPath))
            {
                return InvokeCore(context, matchedPath, remainingPath);
            }
            return _next(context);
        }

        private async Task InvokeCore(HttpContext context, PathString matchedPath, PathString remainingPath)
        {
            var path = context.Request.Path;
            var pathBase = context.Request.PathBase;

            // Update the path
            if (_options.MatchFromRoot)
            {
                context.Request.PathBase = matchedPath;
            }
            else
            {
                context.Request.PathBase = pathBase.Add(matchedPath);
            }

            context.Request.Path = remainingPath;

            try
            {
                await _options.Branch(context);
            }
            finally
            {
                context.Request.PathBase = pathBase;
                context.Request.Path = path;
            }
        }
    }
}
