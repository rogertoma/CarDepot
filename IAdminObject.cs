//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CarDepot
{
    public interface IAdminObject : ICloneable
    {
        bool Equals(IAdminObject item);
        //T GetValue<T>(PropertyId id);
        string GetValue(PropertyId id);
        List<String[]> GetMultiValue(PropertyId id);
        void Initialize();
        string ObjectId { get; }
        void SetValue(PropertyId id, object value);
        void ApplyValue(PropertyId id, string value);
        void ApplyMultiValue(PropertyId id, XElement element);
        void UpdateData();
        XDocument XDocument { get; }

        bool Save(object sender);
        void Delete(object sender);
    }
}
