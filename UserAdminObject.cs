using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarDepot.Controls;
using CarDepot.Pages;
using CarDepot.VehicleStore;

namespace CarDepot
{

    public class UserAdminObject: AdminObject
    {
        List<string> _mainTabPages = new List<string>(); 
        public string Name { get; set; }
        public string Password { get; set; }
        public string RegistrationNumer { get; set; }
        public List<UserCategory> Category = new List<UserCategory>();
        public string PicturePath { get; set; }
        List<IPropertyPanel> _openedPages = new List<IPropertyPanel>();
        List<PermissionTypes> _permissions = new List<PermissionTypes>();
        public CacheManager.UIMode UiMode = CacheManager.UIMode.Full;

        public enum PermissionTypes
        {
            CreateNewVehicle,
            CreateNewCustomer,
            DeleteVehicle,
            DeleteCustomer,
            PurchaseInformation,
            SaleInformation,
            SoldButNotDeliveredCheckBox,
            WarPurchasedCheckBox,
            SoldCheckBox,
            GenerateReport,
            UpdateSaleTaxPercentage,
            ShowCheckedOutBy,
            IsManager
        }

        public enum UserCategory
        {
            Detail,
            Mechanic,
            Sales,
            Documentation,
            Administrator
        }

        public UserAdminObject(string objectId)
            : base(objectId)
        {
            
        }

        public List<string> MainTabPages
        {
            set { _mainTabPages = value; }
            get { return _mainTabPages; }
        }

        public List<PermissionTypes> Permissions
        {
            set { _permissions = value; }
            get { return _permissions;  }
        }

        public List<IPropertyPanel> OpenedPages
        {
            set { _openedPages = value; }
            get { return _openedPages; }
        }


        public override IAdminItemCache Cache
        {
            set { ;  }
            get { return CacheManager.UserCache; }
        }
    }
}
