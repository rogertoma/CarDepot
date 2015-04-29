using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using CarDepot.Resources;
using System;
using System.Runtime.InteropServices;

namespace CarDepot.VehicleStore
{
    public class CacheManager
    {        
        private static bool _updateingCache = false;
        private static UserCache _userCache = null;
        private static ActiveVehicleCache _activeVehicleCache = null;
        private static VehicleCache _allVehicleCache = null;
        private static CustomerCache _allCustomerCache = null;
        private static UserAdminObject _activeUser = null;
        private static TabControl _mainTabControl = null;

        public enum UIMode
        {
            Customer,
            Full,
        }

        static CacheManager()
        {
            System.Timers.Timer updateTasks = null;
            updateTasks = new System.Timers.Timer();
            updateTasks.Interval = 300000; // Every 5 Minutes
            updateTasks.Elapsed += updateTasks_Elapsed;
            updateTasks.Start();

            Thread loadCache = new Thread(new ThreadStart(LoadCache));
            loadCache.SetApartmentState(ApartmentState.STA);
            loadCache.Start();
        }

        private static void LoadCache()
        {
            _userCache = new UserCache();
            _allVehicleCache = new VehicleCache(Settings.VehiclePath, new Dictionary<VehicleCacheSearchKey, string>());
            _allCustomerCache = new CustomerCache();
        }

        public static VehicleCache ActiveVehicleCache
        {
            get
            {
                if (_activeVehicleCache == null)
                {
                    _activeVehicleCache = new ActiveVehicleCache();
                }
                return _activeVehicleCache;
            }
        }

        public static VehicleCache AllVehicleCache
        {
            get
            {
                return _allVehicleCache;
            }
            set { _allVehicleCache = value; }
        }

        public static CustomerCache AllCustomerCache
        {
            get
            {
                return _allCustomerCache;
            }
            set { _allCustomerCache = value; }
        }

        public static UserCache UserCache
        {
            get
            {
                if (_userCache == null)
                {
                    _userCache = new UserCache();
                }
                return _userCache;
            }
        }

        public static UserAdminObject ActiveUser
        {
            get
            {
                if (_activeUser == null)
                {
                    _activeUser = UserCache.SystemAdminAccount;
                }
                return _activeUser;
            }
            set { _activeUser = value; }
        }

        public static TabControl MainTabControl
        {
            get
            {
                return _mainTabControl;
            }
            set { _mainTabControl = value; }
        }

        public static void UpdateAllVehicleCache()
        {
            _allVehicleCache = new VehicleCache(Settings.VehiclePath, new Dictionary<VehicleCacheSearchKey, string>(), true);
            _updateingCache = false;
        }

        static void updateTasks_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_updateingCache)
            {
                _updateingCache = true;
                Thread updateAllVehicleCache = new Thread(new ThreadStart(UpdateAllVehicleCache));
                updateAllVehicleCache.Start();
            }
        }
    }
}
