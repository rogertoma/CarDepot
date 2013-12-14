﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using CarDepot.VehicleStore;
using CarDepot.Resources;

namespace CarDepot
{
    public class CustomerAdminObject : AdminObject
    {
        private Dictionary<PropertyId, string> _basicInfo = new Dictionary<PropertyId, string>();
        private List<string[]> _vehicleIds = new List<string[]>();

        public override IAdminItemCache Cache { get; set; }

        public CustomerAdminObject() 
            : base() 
        {

        }

        public CustomerAdminObject(string objectId)
            : base(objectId)
        {

        }

        public List<string[]> VehicleIds
        {
            set { _vehicleIds = value; }
            get { return _vehicleIds; }
        }

        public Dictionary<PropertyId, string> BasicInfo
        {
            set { _basicInfo = value; }
            get { return _basicInfo; }
        }

        public override void ApplyMultiValue(PropertyId id, XElement element)
        {
            switch (id)
            {
                default:
                    VehicleIds.Clear();
                    foreach (XElement descendant in element.Descendants())
                    {
                        string[] multiValueItem = new string[2];
                        multiValueItem[Settings.MultiValueKeyIndex] = descendant.Name.ToString();

                        if (descendant.Value.StartsWith("\\"))
                        {
                            var directoryInfo = new FileInfo(ObjectId).Directory;
                            if (directoryInfo != null)
                                multiValueItem[Settings.MultiValueValueIndex] =
                                    directoryInfo.FullName + descendant.Value;
                        }
                        else
                        {
                            multiValueItem[Settings.MultiValueValueIndex] = descendant.Value;
                        }

                        VehicleIds.Add(multiValueItem);
                    }
                    break;
            }
        }

        public override void ApplyValue(PropertyId id, string value)
        {
            switch (id)
            {
                case PropertyId.FileVersion:
                    base.FileVersion = value;
                    break;
                default:
                    if (BasicInfo.ContainsKey(id))
                    {
                        BasicInfo[id] = value;
                    }
                    else
                    {
                        BasicInfo.Add(id, value);
                    }
                    break;
            }
        }

        public override string GetValue(PropertyId id)
        {
            switch (id)
            {
                case PropertyId.Id:
                    return Id;
                default:
                    if (_basicInfo.ContainsKey(id))
                    {
                        return _basicInfo[id];
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        public override List<String[]> GetMultiValue(PropertyId id)
        {
            switch (id)
            {
                case PropertyId.CustomerAssociatedVehicles:
                    return VehicleIds;
                default:
                    return null;
            }
        }

        public override bool Equals(IAdminObject item)
        {
            CustomerAdminObject customer = item as CustomerAdminObject;
            if (customer == null)
                return false;

            //Tasks, Images, Basic Info
            if (_vehicleIds.Select(vehicleId => customer.VehicleIds.Any(otherImg => otherImg[0] == vehicleId[0])).Any(subListFound => !subListFound))
            {
                return false;
            }

            foreach (var info in _basicInfo)
            {
                if (!customer.BasicInfo.ContainsKey(info.Key))
                    return false;

                if (!customer.BasicInfo[info.Key].Equals(info.Value))
                    return false;
            }

            return true;
        }

    }
}
