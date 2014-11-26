using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarDepot.VehicleStore
{
    public class CacheManager
    {        
        private static UserCache _userCache = null;
        private static ActiveVehicleCache _activeVehicleCache = null;
        private static UserAdminObject _activeUser = null;
        private static TabControl _mainTabControl = null;

        public enum UIMode
        {
            Customer,
            Full,
        }

        static CacheManager()
        {
            Thread loadCache = new Thread(new ThreadStart(LoadCache));
            loadCache.SetApartmentState(ApartmentState.STA);
            loadCache.Start();
        }

        private static void LoadCache()
        {
            _userCache = new UserCache();
            //_activeVehicleCache = new ActiveVehicleCache();
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
    }
}
