// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CoreWCF;

namespace ServiceContract
{
    [ServiceContract(Namespace = Constants.NS, Name = "EnumService")]
    public interface IEnumService
    {
        [OperationContract]
        void Accept(TestEnum accept);

        [OperationContract]
        TestEnum Request();
    }

    public enum TestEnum
    {
        One = 1,
        Two = 2,
        Three = 3,
        Five = 5,
    }
}
