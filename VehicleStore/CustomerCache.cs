using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CarDepot.Resources;


namespace CarDepot.VehicleStore
{
    public enum CustomerCacheSearchKey
    {
        Id,
    }

    class CustomerCache : List<CustomerAdminObject>, IAdminItemCache
    {
        public UserAdminObject SystemAdminAccount = null;
        List<string> _properties = new List<string>();

        public CustomerCache()
        {
            Initialize();

            string[] customers = Directory.GetDirectories(Settings.CustomerPath);
            foreach (var customer in customers)
            {
                foreach (string file in Directory.GetFiles(customer, Strings.FILTER_ALL_XML))
                {
                    LoadCustomer(file);
                }
            }
        }

        public CustomerCache(Dictionary<CustomerCacheSearchKey, string> searchParam)
        {
            Initialize();

            string[] customers = Directory.GetDirectories(Settings.CustomerPath);
            foreach (var customer in customers)
            {
                foreach (string file in Directory.GetFiles(customer, Strings.FILTER_ALL_XML))
                {
                    LoadCustomerIfMeetsSearch(file, searchParam);
                }
            }

        }

        private void LoadCustomerIfMeetsSearch(string filePath, Dictionary<CustomerCacheSearchKey, string> searchParam)
        {
            CustomerAdminObject customer = new CustomerAdminObject(filePath);
            customer.Cache = this;

            foreach (var param in searchParam)
            {
                PropertyId id = (PropertyId)Enum.Parse(typeof(PropertyId), param.Key.ToString());
                if (customer.GetValue(id) == param.Value)
                    this.Add(customer);
            }

        }

        private void Initialize()
        {
            object[] attributes = GetType().GetCustomAttributes(typeof(PropertyIdAttribute), true);

            for (int index = 0; index < attributes.Length; index++)
            {
                PropertyIdAttribute attribute = (PropertyIdAttribute)attributes[index];
                _properties.Add(attribute.PropertyId.ToString());
            }
        }

        private void LoadCustomer(string file)
        {
            CustomerAdminObject customer = new CustomerAdminObject(file);
            customer.Cache = this;
            this.Add(customer);
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
            throw new NotImplementedException();
        }

        public void ExitReadLock()
        {
            throw new NotImplementedException();
        }

        public bool InitialRefreshPerformed { get; private set; }
        public void ModifyItem(IAdminObject item)
        {
            throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public List<T> ToList<T>() where T : IAdminObject
        {
            throw new NotImplementedException();
        }

        public bool TryEnterReadLock()
        {
            throw new NotImplementedException();
        }
    }
}

