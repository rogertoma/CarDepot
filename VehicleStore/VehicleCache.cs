using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    public enum VehicleCacheSearchKey
    {
        IsSold,
        IsAvailable,
        FromDate,
        ToDate
    }

    public class VehicleCache : List<VehicleAdminObject>, IAdminItemCache
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public VehicleCache(string vehiclePath)
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

        public VehicleCache(string vehiclePath, Dictionary<VehicleCacheSearchKey, string> searchParam):this(vehiclePath)
        {

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
            //_lock.EnterWriteLock();
            //try
            //{
            //    this[item.ObjectId] = item;
            //}
            //finally
            //{
            //    _lock.ExitWriteLock();
            //}

            if (ItemUpdate != null)
            {
                ItemUpdate.Invoke(this, new AdminItemCache.UpdateEventArgs(item.ObjectId, AdminItemCache.UpdateType.ModifyItem));
            }

            //var vehicles = from v in this where v.ObjectId == item.ObjectId select v;
            //VehicleAdminObject vehicle = vehicles.FirstOrDefault();
            //Remove(vehicle);
            //this.Add((VehicleAdminObject) item);
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
            throw new NotImplementedException();
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

            sortedList.AddRange(direction == ListSortDirection.Ascending
                                    ? this.OrderBy(vehicleAdminObject => vehicleAdminObject.GetValue(category))
                                    : this.OrderByDescending(vehicleAdminObject => vehicleAdminObject.GetValue(category)));

            this.Clear();
            this.AddRange(sortedList);
        }
    }
}
