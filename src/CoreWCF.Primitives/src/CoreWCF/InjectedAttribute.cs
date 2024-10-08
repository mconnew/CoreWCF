﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace CoreWCF
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class InjectedAttribute : Attribute
    {
        public string PropertyName { get; set; }
        public object ServiceKey { get; set; }
    }
}
