// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using ServiceContract;

namespace Services
{
    public class EnumService : IEnumService
    {
        public void Accept(TestEnum accept) => throw new NotImplementedException();
        public TestEnum Request() => throw new NotImplementedException();
    }
}
