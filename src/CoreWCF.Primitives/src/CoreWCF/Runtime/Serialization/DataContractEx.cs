// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace CoreWCF.Runtime.Serialization
{
    internal class DataContractEx
    {
        private object _wrappedDataContract;

        private DataContractEx(object dataContract) => _wrappedDataContract = dataContract;

        public Type UnderlyingType => s_getUnderlyingType(_wrappedDataContract);
        public XmlQualifiedName StableName => s_getStableName(_wrappedDataContract);
        public XmlDictionaryString TopLevelElementName => s_getTopLevelElementName(_wrappedDataContract);
        public XmlDictionaryString TopLevelElementNamespace => s_getTopLevelElementNamespace(_wrappedDataContract);
        public bool HasRoot => s_getHasRoot(_wrappedDataContract);
        public bool IsXmlDataContract => _wrappedDataContract.GetType() == s_xmlDataContractType;
        public bool XmlDataContractIsAnonymous => s_getXmlDataContractIsAnonymous(_wrappedDataContract);
        public XmlSchemaType XmlDataContractXsdType => s_getXmlDataContractXsdType(_wrappedDataContract);

        internal static Func<Type, DataContractEx> GetDataContract { private set; get; } = GetDataContractStub;

        private static Type s_dataContractType = typeof(DataContractSerializer).Assembly.GetType("System.Runtime.Serialization.DataContract");
        private static Type s_xmlDataContractType = typeof(DataContractSerializer).Assembly.GetType("System.Runtime.Serialization.XmlDataContract");
        private static Func<object, Type> s_getUnderlyingType = ReflectionHelper.GetPropertyDelegate<Type>(s_dataContractType, "UnderlyingType");
        private static Func<object, XmlQualifiedName> s_getStableName = ReflectionHelper.GetPropertyDelegate<XmlQualifiedName>(s_dataContractType, "StableName");
        private static Func<object, XmlDictionaryString> s_getTopLevelElementName = ReflectionHelper.GetPropertyDelegate<XmlDictionaryString>(s_dataContractType, "TopLevelElementName");
        private static Func<object, XmlDictionaryString> s_getTopLevelElementNamespace = ReflectionHelper.GetPropertyDelegate<XmlDictionaryString>(s_dataContractType, "TopLevelElementNamespace");
        private static Func<object, bool> s_getHasRoot = ReflectionHelper.GetPropertyDelegate<bool>(s_dataContractType, "HasRoot");
        private static Func<object, bool> s_getXmlDataContractIsAnonymous = ReflectionHelper.GetPropertyDelegate<bool>(s_xmlDataContractType, "IsAnonymous");
        private static Func<object, XmlSchemaType> s_getXmlDataContractXsdType = ReflectionHelper.GetPropertyDelegate<XmlSchemaType>(s_xmlDataContractType, "XsdType");

        private static DataContractEx GetDataContractStub(Type clrType)
        {
            var methodInfo = s_dataContractType.GetMethod("GetDataContract", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(Type) }, null);
            var getDataContractDelegate = ReflectionHelper.CreateStaticMethodCallLambda<Type, object>(methodInfo);
            Func<Type, DataContractEx> wrappingDelegate = (Type type) => new DataContractEx(getDataContractDelegate(type));
            GetDataContract = wrappingDelegate;
            return GetDataContract(clrType);
        }
    }
}
