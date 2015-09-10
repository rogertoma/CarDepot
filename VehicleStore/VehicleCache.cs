using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    public enum VehicleCacheSearchKey
    {
        IsAvailable,
        IsSold,
        WasAvailable,
        WasPurchased,
        IncludeSoldNotDelivered,
        FromDate,
        ToDate,
        VinNumber,
        Year,
        Make,
        Model,
        CustomerId,
    }

    public enum VehicleCacheTaskSearchKey
    {
        AssignedTo,
        Category
    }

    public class VehicleCache : List<VehicleAdminObject>, IAdminItemCache
    {
        //private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public VehicleCache(string vehiclePath)
        {
            try
            {
                string[] vehicles = Directory.GetDirectories(vehiclePath);
                foreach (var vehicle in vehicles)
                {
                    foreach (string file in Directory.GetFiles(vehicle, Strings.FILTER_ALL_XML))
                    {
                        LoadVehicle(file);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: VehicleCache(string vehiclePath)\n" + ex.StackTrace);
            }
        }

        public VehicleCache(string vehiclePath, Dictionary<VehicleCacheSearchKey, string> searchParam, bool forceRefresh = false)//:this(vehiclePath)
        {
            //useCache = true;

            if (CacheManager.AllVehicleCache == null || forceRefresh)
            {
                VehicleCacheSearchDrive(vehiclePath, searchParam);
            }
            else
            {
                VehicleCacheSearchCache(searchParam);
            }
        }

        
        public void VehicleCacheSearchCache(Dictionary<VehicleCacheSearchKey, string> searchParam)
        {
            foreach (VehicleAdminObject vehicleAdminObject in CacheManager.AllVehicleCache)
            {
                if (vehicleAdminObject.GetValue(PropertyId.IsDeleted) == true.ToString())
                {
                    continue;
                }
                if (string.IsNullOrEmpty(vehicleAdminObject.GetValue(PropertyId.Year)) ||
                    string.IsNullOrEmpty(vehicleAdminObject.GetValue(PropertyId.Make)) ||
                    string.IsNullOrEmpty(vehicleAdminObject.GetValue(PropertyId.Model)))
                {
                    continue;
                }

                if (searchParam == null || searchParam.Count == 0 || VehicleMeetsSearchParam(vehicleAdminObject, searchParam))
                {
                    vehicleAdminObject.Cache = this;
                    this.Add(vehicleAdminObject);
                }

            }

            if (searchParam.ContainsKey(VehicleCacheSearchKey.IncludeSoldNotDelivered))
            {
                foreach (VehicleAdminObject vehicleAdminObject in CacheManager.AllVehicleCache)
                {
                    foreach (VehicleTask vehicleTask in vehicleAdminObject.VehicleTasks)
                    {
                        if (vehicleTask.Id.Equals(Strings.CARDELIVEREDTASK) &&
                            !vehicleTask.Status.Equals(VehicleTask.StatusTypes.Completed.ToString()))
                        {
                            if (!this.Contains(vehicleAdminObject))
                            {
                                this.Add(vehicleAdminObject);
                            }
                        }
                    }
                }

            }
        }

        public void VehicleCacheSearchDrive(string vehiclePath, Dictionary<VehicleCacheSearchKey, string> searchParam)
        {
            try
            {
                string[] vehicles = Directory.GetDirectories(vehiclePath);
                foreach (var vehicle in vehicles)
                {
                    foreach (string file in Directory.GetFiles(vehicle, Strings.FILTER_ALL_XML))
                    {
                        VehicleAdminObject vehicleAdminObject = new VehicleAdminObject(file);
                        if (vehicleAdminObject.GetValue(PropertyId.IsDeleted) == true.ToString())
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(vehicleAdminObject.GetValue(PropertyId.Year)) ||
                            string.IsNullOrEmpty(vehicleAdminObject.GetValue(PropertyId.Make)) ||
                            string.IsNullOrEmpty(vehicleAdminObject.GetValue(PropertyId.Model)))
                        {
                            continue;
                        }

                        if (searchParam == null || VehicleMeetsSearchParam(vehicleAdminObject, searchParam))
                        {
                            vehicleAdminObject.Cache = this;
                            this.Add(vehicleAdminObject);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ERROR: VehicleCacheSearchDrive(string vehiclePath, Dictionary<VehicleCacheSearchKey, string> searchParam)\n" +
                    ex.StackTrace);
            }
        }

        public VehicleCache(string vehiclePath, Dictionary<VehicleCacheTaskSearchKey, string> searchParam)
        {
            while (CacheManager.AllVehicleCache == null)
            {
                System.Threading.Thread.Sleep(1000);
            }

            VehicleCacheSearchCache(searchParam);

        }

        private void VehicleCacheSearchCache(Dictionary<VehicleCacheTaskSearchKey, string> searchParam)
        {
            foreach (VehicleAdminObject temp in CacheManager.AllVehicleCache)
            {
                if (!string.IsNullOrEmpty(temp.IsDeleted) && bool.Parse(temp.IsDeleted))
                {
                    continue;
                }

                if (searchParam.ContainsKey(VehicleCacheTaskSearchKey.AssignedTo) &&
                    temp.VehicleTasks.Any(vehicleTask => (vehicleTask.AssignedTo == searchParam[VehicleCacheTaskSearchKey.AssignedTo] && vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())))
                {
                    temp.Cache = this;
                    this.Add(temp);
                }
                else if (searchParam.ContainsKey(VehicleCacheTaskSearchKey.Category) &&
                    temp.VehicleTasks.Any(vehicleTask => (vehicleTask.Category == searchParam[VehicleCacheTaskSearchKey.Category] && vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())))
                {
                    temp.Cache = this;
                    this.Add(temp);
                }
            } 
        }

        /*
        private void VehicleCacheSearchDrive(string vehiclePath,
            Dictionary<VehicleCacheTaskSearchKey, string> searchParam)
        {
            string[] vehicles = Directory.GetDirectories(vehiclePath);
            foreach (var vehicle in vehicles)
            {
                foreach (string file in Directory.GetFiles(vehicle, Strings.FILTER_ALL_XML))
                {
                    VehicleAdminObject temp = new VehicleAdminObject(file);
                    if (!string.IsNullOrEmpty(temp.IsDeleted) && bool.Parse(temp.IsDeleted))
                    {
                        continue;
                    }

                    if (searchParam.ContainsKey(VehicleCacheTaskSearchKey.AssignedTo) &&
                        temp.VehicleTasks.Any(vehicleTask => (vehicleTask.AssignedTo == searchParam[VehicleCacheTaskSearchKey.AssignedTo] && vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())))
                    {
                        temp.Cache = this;
                        this.Add(temp);
                    }
                    else if (searchParam.ContainsKey(VehicleCacheTaskSearchKey.Category) &&
                        temp.VehicleTasks.Any(vehicleTask => (vehicleTask.Category == searchParam[VehicleCacheTaskSearchKey.Category] && vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())))
                    {
                        temp.Cache = this;
                        this.Add(temp);
                    }
                }
            }
        }
        */

        private bool VehicleMeetsSearchParam(VehicleAdminObject vehicle, Dictionary<VehicleCacheSearchKey, string> searchParam)
        {
            DateTime fromDate, toDate, date;
            if (searchParam.ContainsKey(VehicleCacheSearchKey.FromDate) && !String.IsNullOrEmpty(searchParam[VehicleCacheSearchKey.FromDate]))
            {
                fromDate = DateTime.Parse(searchParam[VehicleCacheSearchKey.FromDate]);
            }
            else
            {
                fromDate = new DateTime(1000, 01, 01);
            }

            if (searchParam.ContainsKey(VehicleCacheSearchKey.ToDate) && !String.IsNullOrEmpty(searchParam[VehicleCacheSearchKey.ToDate]))
            {
                toDate = DateTime.Parse(searchParam[VehicleCacheSearchKey.ToDate]);
            }
            else
            {
                toDate = new DateTime(5000, 01, 01);
            }


            foreach (VehicleCacheSearchKey searchKey in searchParam.Keys)
            {
                DateTime saleDate = DateTime.Now;
                DateTime purchaseDate = DateTime.Now;

                string sDate = vehicle.GetValue(PropertyId.SaleDate);
                if (!string.IsNullOrEmpty(sDate))
                {
                    saleDate = DateTime.Parse(sDate, CultureInfo.InvariantCulture);
                }
                string pDate = vehicle.GetValue(PropertyId.PurchaseDate);
                if (!string.IsNullOrEmpty(pDate))
                {
                    purchaseDate = DateTime.Parse(pDate, CultureInfo.InvariantCulture);
                }

                switch (searchKey)
                {
                    case VehicleCacheSearchKey.CustomerId:
                        string vehicleCustomerID = vehicle.GetValue(PropertyId.SaleCustomerId);
                        if (!string.IsNullOrEmpty(vehicleCustomerID) &&
                            vehicleCustomerID.ToLower().Trim().Equals(searchParam[VehicleCacheSearchKey.CustomerId].ToLower().Trim()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }


                        break;
                    case VehicleCacheSearchKey.IsAvailable:
                        if (!String.IsNullOrEmpty(sDate) && !searchParam.ContainsKey(VehicleCacheSearchKey.IsSold))
                        {
                            return false;
                        }

                        if (String.IsNullOrEmpty(pDate))
                        {
                            return false;
                        }

                        if (purchaseDate < fromDate || purchaseDate > toDate)
                        {
                            return false;
                        }

                        break;

                    case VehicleCacheSearchKey.WasAvailable:
                        if (string.IsNullOrEmpty(pDate))
                        {
                            return false;
                        }
                        
                        if (purchaseDate > toDate)
                        {
                            return false;
                        }

                        if (!string.IsNullOrEmpty(sDate))
                        {
                            if (saleDate < toDate)
                            {
                                return false;
                            }
                        }

                        break;

                    case VehicleCacheSearchKey.WasPurchased:
                        if (string.IsNullOrEmpty(pDate))
                        {
                            return false;
                        }

                        if (purchaseDate < fromDate)
                        {
                            return false;
                        }

                        if (purchaseDate > toDate)
                        {
                            return false;
                        }

                        break;

                    case VehicleCacheSearchKey.IsSold:
                        if (String.IsNullOrEmpty(sDate))
                        {
                            return false;
                        }

                        if (saleDate < fromDate || saleDate > toDate)
                        {
                            return false;
                        }
                        break;

                    case VehicleCacheSearchKey.VinNumber:
                        if (!string.IsNullOrEmpty(vehicle.VinNumber) && 
                            !vehicle.VinNumber.ToLower().Contains(searchParam[VehicleCacheSearchKey.VinNumber].ToLower()))
                        {
                            return false;
                        }
                        if (string.IsNullOrEmpty(vehicle.VinNumber))
                        {
                            return false;
                        }
                        break;

                    case VehicleCacheSearchKey.Year:
                        if (!string.IsNullOrEmpty(vehicle.Year) &&
                            !vehicle.Year.ToLower().Contains(searchParam[VehicleCacheSearchKey.Year].ToLower()))
                        {
                            return false;
                        }
                        break;

                    case VehicleCacheSearchKey.Make:
                        if (!string.IsNullOrEmpty(vehicle.Make) &&
                            !vehicle.Make.ToLower().Contains(searchParam[VehicleCacheSearchKey.Make].ToLower()))
                        {
                            return false;
                        }
                        break;

                    case VehicleCacheSearchKey.Model:
                        if (!string.IsNullOrEmpty(vehicle.Model) &&
                            !vehicle.Model.ToLower().Contains(searchParam[VehicleCacheSearchKey.Model].ToLower()))
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        private void LoadVehicle(string file)
        {
            VehicleAdminObject vehicle = new VehicleAdminObject(file);
            vehicle.Cache = this;
            this.Add(vehicle);
        }

        public event EventHandler<AdminItemCache.RefreshEventArgs> RefreshStarted;
        public event EventHandler<AdminItemCache.RefreshEventArgs> RefreshEnded;
        public event EventHandler<AdminItemCache.UpdateEventArgs> ItemUpdate;

        public void AddItem(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string objectId)
        {
            var vehicles = from vehicle in this where vehicle.ObjectId == objectId select vehicle;

            if (vehicles.FirstOrDefault() == null)
                return false;

            return true;
        }

        public void ExitReadLock()
        {
            throw new NotImplementedException();
        }

        public bool InitialRefreshPerformed
        {
            get { throw new NotImplementedException(); }
        }

        public void ModifyItem(IAdminObject item)
        {

            if (CacheManager.AllVehicleCache != null)
            {
                CacheManager.AllVehicleCache.RemoveItem(item.ObjectId);
                VehicleAdminObject vehicle = new VehicleAdminObject(item.ObjectId);
                vehicle.Cache = CacheManager.AllVehicleCache;
                CacheManager.AllVehicleCache.Add(vehicle);
            }

            if (ItemUpdate != null)
            {
                ItemUpdate.Invoke(this, new AdminItemCache.UpdateEventArgs(item.ObjectId, AdminItemCache.UpdateType.ModifyItem));
            }

        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public IAdminObject RefreshItem(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string objectId)
        {

            foreach (VehicleAdminObject vehicleAdminObject in this)
            {
                if (vehicleAdminObject.ObjectId == objectId)
                {
                    Remove(vehicleAdminObject);
                    break;
                }
            }
        }

        public IAdminObject this[string objectId]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<T> ToList<T>() where T : IAdminObject
        {
            throw new NotImplementedException();
        }

        public bool TryEnterReadLock()
        {
            throw new NotImplementedException();
        }

        public void SortCache(PropertyId category, ListSortDirection direction)
        {
            List<VehicleAdminObject> sortedList = new List<VehicleAdminObject>();

            int outInteger = 0;

            if (int.TryParse(this[0].GetValue(category), out outInteger))
            {
                sortedList.AddRange(direction == ListSortDirection.Ascending
                    ? this.OrderBy(vehicleAdminObject => int.Parse(vehicleAdminObject.GetValue(category)))
                    : this.OrderByDescending(vehicleAdminObject => int.Parse(vehicleAdminObject.GetValue(category))));
            }
            else
            {
                sortedList.AddRange(direction == ListSortDirection.Ascending
                    ? this.OrderBy(vehicleAdminObject => vehicleAdminObject.GetValue(category))
                    : this.OrderByDescending(vehicleAdminObject => vehicleAdminObject.GetValue(category)));
            }


            this.Clear();
            this.AddRange(sortedList);
        }
    }
}
