using System;
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
    public class VehicleAdminObject : AdminObject
    {
        private Dictionary<PropertyId, string> _basicInfo = new Dictionary<PropertyId, string>();
        private List<string[]> _images = new List<string[]>();
        private List<string[]> _purchaseAssociatedFiles = new List<string[]>();
        private List<string[]> _saleAssociatedFiles = new List<string[]>();


        private ObservableCollection<VehicleTask> _vehicleTasks = new ObservableCollection<VehicleTask>();


        public override IAdminItemCache Cache { get; set; }

        #region ExposedPropertiesForListResults

        public string VehicleId
        {
            get { return Id; }
        }

        public string Year
        {
            get { return GetValue(PropertyId.Year); }
            set { ApplyValue(PropertyId.Year, value); }
        }
        public string Make
        {
            get { return GetValue(PropertyId.Make); }
            set { ApplyValue(PropertyId.Make, value); }
        }
        public string Model
        {
            get { return GetValue(PropertyId.Model); }
            set { ApplyValue(PropertyId.Model, value); }
        }
        public string ListPrice
        {
            get { return GetValue(PropertyId.ListPrice); }
            set { ApplyValue(PropertyId.ListPrice, value); }
        }
        public string DriveTrain
        {
            get { return GetValue(PropertyId.DriveTrain); }
            set { ApplyValue(PropertyId.DriveTrain, value); }
        }
        public string Transmission
        {
            get { return GetValue(PropertyId.Transmission); }
            set { ApplyValue(PropertyId.Transmission, value); }
        }
        public string Mileage
        {
            get { return GetValue(PropertyId.Mileage); }
            set { ApplyValue(PropertyId.Mileage, value); }
        }
        public string Engine
        {
            get { return GetValue(PropertyId.Engine); }
            set { ApplyValue(PropertyId.Engine, value); }
        }
        public string ExtColor
        {
            get { return GetValue(PropertyId.ExtColor); }
            set { ApplyValue(PropertyId.ExtColor, value); }
        }
        public string StockNumber
        {
            get { return GetValue(PropertyId.StockNumber); }
            set { ApplyValue(PropertyId.StockNumber, value); }
        }
        public string VinNumber
        {
            get { return GetValue(PropertyId.VinNumber); }
            set { ApplyValue(PropertyId.VinNumber, value); }
        }
        public string SaleDate
        {
            get { return GetValue(PropertyId.SaleDate); }
            set { ApplyValue(PropertyId.SaleDate, value); }
        }
        public string PurchaseDate
        {
            get { return GetValue(PropertyId.PurchaseDate); }
            set { ApplyValue(PropertyId.PurchaseDate, value); }
        }
        public string Profit
        {
            get { return GetValue(PropertyId.Profit); }
            set { ApplyValue(PropertyId.Profit, value); }
        }
        //public string MinistryLicense
        //{
        //    get { return GetValue(PropertyId.MinistryLicense); }
        //    set { ApplyValue(PropertyId.MinistryLicense, value); }
        //}
        //public string SaleTotal
        //{
        //    get { return GetValue(PropertyId.SaleTotal); }
        //    set { ApplyValue(PropertyId.SaleTotal, value); }
        //}
        //public string CustomerDeposit
        //{
        //    get { return GetValue(PropertyId.CustomerDeposit); }
        //    set { ApplyValue(PropertyId.CustomerDeposit, value); }
        //}
        //public string CustomerPayments
        //{ 
        //    get { return GetValue(PropertyId.CustomerPayments); } 
        //    set { ApplyValue(PropertyId.CustomerPayments, value); }
        //}
        //public string DealerReserve
        //{
        //    get { return GetValue(PropertyId.DealerReserve); }
        //    set { ApplyValue(PropertyId.DealerReserve, value); }
        //}
        //public string FinancialFeeHst
        //{
        //    get { return GetValue(PropertyId.FinancialFeeHst); }
        //    set { ApplyValue(PropertyId.FinancialFeeHst, value); }
        //}
        //public string NetDealerReserve
        //{
        //    get { return GetValue(PropertyId.NetDealerReserve); }
        //    set { ApplyValue(PropertyId.NetDealerReserve, value); }
        //}
        //public string TotalIncomeLessMinistryLicense
        //{
        //    get { return GetValue(PropertyId.TotalIncomeLessMinistryLicense); }
        //    set { ApplyValue(PropertyId.TotalIncomeLessMinistryLicense, value); }
        //}
        //public string TotalHst
        //{
        //    get { return GetValue(PropertyId.TotalHst); }
        //    set { ApplyValue(PropertyId.TotalHst, value); }
        //}
        //public string NetIncomeLessMinistryLicense
        //{
        //    get { return GetValue(PropertyId.NetIncomeLessMinistryLicense); }
        //    set { ApplyValue(PropertyId.NetIncomeLessMinistryLicense, value); }
        //}
        #endregion
        public VehicleAdminObject() 
            : base() 
        {

        }

        public VehicleAdminObject(string objectId)
            : base(objectId)
        {

        }

        public List<string[]> Images
        {
            set { _images = value; }
            get { return _images; }
        }

        public List<string[]> PurchaseAssociatedFiles
        {
            set { _purchaseAssociatedFiles = value; }
            get { return _purchaseAssociatedFiles; }
        }

        public List<string[]> SaleAssociatedFiles
        {
            set { _saleAssociatedFiles = value; }
            get { return _saleAssociatedFiles; }
        }

        public ObservableCollection<VehicleTask> VehicleTasks
        {
            set { _vehicleTasks = value; }
            get { return _vehicleTasks; }
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
                case PropertyId.PurchaseAssociatedFiles:
                    _purchaseAssociatedFiles.Clear();
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

                        _purchaseAssociatedFiles.Add(multiValueItem);
                    }
                    break;
                case PropertyId.SaleAssociatedFiles:
                    _saleAssociatedFiles.Clear();
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

                        _saleAssociatedFiles.Add(multiValueItem);
                    }
                    break;
                case PropertyId.Tasks:
                    VehicleTasks.Clear();
                    foreach (XElement descendant in element.Descendants())
                    {
                        VehicleTask task = CreateTask(descendant);
                        if (!string.IsNullOrEmpty(task.Id))
                            VehicleTasks.Add(task);
                    }
                    break;

                default:
                    Images.Clear();
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

                        _images.Add(multiValueItem);
                    }
                    break;
            }
        }

        private VehicleTask CreateTask(XElement element)
        {
            VehicleTask task = new VehicleTask();
            foreach (XElement descendant in element.Descendants())
            {
                PropertyId id;
                try
                {
                    id = (PropertyId)Enum.Parse(typeof(PropertyId), descendant.Name.LocalName);
                }
                catch (Exception)
                {
                    continue;
                }

                task.ApplyValue(id, descendant.Value);
            }

            return task;
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
                    return VehicleId;
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
                case PropertyId.Images:
                    return Images;
                case PropertyId.PurchaseAssociatedFiles:
                    return PurchaseAssociatedFiles;
                case PropertyId.SaleAssociatedFiles:
                    return SaleAssociatedFiles;
                default:
                    return null;
            }
        }

        public override bool Equals(IAdminObject item)
        {
            VehicleAdminObject vehicle = item as VehicleAdminObject;
            if (vehicle == null)
                return false;

            //Tasks, Images, Basic Info

            if (VehicleTasks.Any(vehicleTask => !vehicle.VehicleTasks.Contains(vehicleTask)))
            {
                return false;
            }

            // images
            bool same = ListsAreEqual(_images, vehicle.Images.ToList());
            if (!same)
            {
                return false;
            }

            // PurchaseAssociatedFiles
            same = ListsAreEqual(_purchaseAssociatedFiles, vehicle.PurchaseAssociatedFiles.ToList());
            if (!same)
            {
                return false;
            }

            // SaleAssociatedFiles
            same = ListsAreEqual(_saleAssociatedFiles, vehicle.SaleAssociatedFiles.ToList());
            if (!same)
            {
                return false;
            }

            // basic info
            foreach (var info in _basicInfo)
            {
                if (!vehicle.BasicInfo.ContainsKey(info.Key))
                    return false;

                if (!vehicle.BasicInfo[info.Key].Equals(info.Value))
                    return false;
            }

            return true;

            #region PreLinq
            //            VehicleAdminObject vehicle = item as VehicleAdminObject;
            //if (vehicle == null)
            //    return false;

            ////Tasks, Images, Basic Info

            //foreach (var vehicleTask in VehicleTasks)
            //{
            //    if (!vehicle.VehicleTasks.Contains(vehicleTask))
            //        return false;
            //}

            //foreach (var image in _images)
            //{
            //    bool subListFound = false; 
            //    foreach (var otherImg in vehicle.Images)
            //    {
            //        if (otherImg[0] == image[0])
            //        {
            //            subListFound = true;
            //            break;
            //        }
            //    }

            //    if (subListFound)
            //    {
            //        continue;
            //    }

            //    return false;
            //}

            //foreach (var info in _basicInfo)
            //{
            //    if (!vehicle.BasicInfo.ContainsKey(info.Key))
            //        return false;

            //    if (!vehicle.BasicInfo[info.Key].Equals(info.Value))
            //        return false;
            //}

            //return true;
#endregion
        }

        private bool ListsAreEqual(List<string[]> list1, List<string[]> list2)
        {
            foreach (string[] list1Item in list1)
            {
                bool found = false;
                foreach (string[] list2Item in list2)
                {
                    if (list2Item[Settings.MultiValueValueIndex] == list1Item[Settings.MultiValueValueIndex])
                    {
                        found = true;
                        list2.Remove(list1Item);
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            if (list2.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}
