// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DataContractDictionary = System.Collections.Generic.Dictionary<System.Xml.XmlQualifiedName, object>;
using System.Runtime.Serialization;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CoreWCF.Runtime.Serialization
{
    internal class DataContractSetEx
    {
        private static Type s_dataContractSetType = typeof(DataContractSerializer).Assembly.GetType("System.Runtime.Serialization.DataContractSet");

        internal DataContractSetEx()
        {
            Wrapped = FormatterServices.GetUninitializedObject(s_dataContractSetType);
        }

        internal DataContractSetEx(DataContractSetEx dataContractSet)
        {
            Wrapped = Activator.CreateInstance(s_dataContractSetType,
                                               BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                                               null,
                                               new object[] { dataContractSet.Wrapped },
                                               null);
        }

        public object Wrapped { get; }

        internal void Add(Type type)
        {
            var addTypeMethodInfo = s_dataContractSetType.GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            addTypeMethodInfo.Invoke(Wrapped, new object[] { type });
        }

        internal static void EnsureTypeNotGeneric(Type type)
        {
            if (type.ContainsGenericParameters)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(SR.Format(SR.GenericTypeNotExportable, type)));
        }

        //#endregion
    }
}
