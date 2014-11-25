﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CarDepot.Resources;
using System.ComponentModel;

namespace CarDepot.VehicleStore
{
    public enum CustomerCacheSearchKey
    {
        Id,
        FirstName,
        LastName,
        PhoneNumber
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

            if (customer.GetValue(PropertyId.IsDeleted) == true.ToString())
            {
                return;
            }

            bool meetsAllRequirements = true;

            foreach (var param in searchParam)
            {
                if (String.IsNullOrEmpty(param.Value) || String.IsNullOrWhiteSpace(param.Value))
                {
                    continue;
                }

                if (param.Key == CustomerCacheSearchKey.PhoneNumber)
                {
                    string searchNumber = StripPhoneNumber(param.Value);
                    if (!(StripPhoneNumber(customer.GetValue(PropertyId.MobilePhone)) == searchNumber ||
                        StripPhoneNumber(customer.GetValue(PropertyId.BusinessPhone)) == searchNumber ||
                        StripPhoneNumber(customer.GetValue(PropertyId.HomePhone)) == searchNumber))
                    {
                        meetsAllRequirements = false;
                        break;
                    }
                }
                else
                {
                    PropertyId id = (PropertyId)Enum.Parse(typeof(PropertyId), param.Key.ToString());
                    if (customer != null && customer.GetValue(id) != null &&
                        customer.GetValue(id).Trim().ToLower() != param.Value.Trim().ToLower())
                    {
                        meetsAllRequirements = false;
                        break;
                    }
                }
            }

            if (meetsAllRequirements)
            {
                this.Add(customer);
            }
        }

        private string StripPhoneNumber(string number)
        {
            string result = "";

            if (number == null)
                return result;
   
            foreach (char character in number.ToCharArray())
            {
                int digit;
                bool parseSuccessful = int.TryParse(character.ToString(), out digit);
                if (parseSuccessful)
                {
                    result += digit;
                }
            }

            return result;
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
            var customers = from customer in this where customer.ObjectId == objectId select customer;

            if (customers.FirstOrDefault() == null)
                return false;

            return true;
        }

        public void SortCache(PropertyId category, ListSortDirection direction)
        {
            List<CustomerAdminObject> sortedList = new List<CustomerAdminObject>();

            sortedList.AddRange(direction == ListSortDirection.Ascending
                                    ? this.OrderBy(customerAdminObject => customerAdminObject.GetValue(category))
                                    : this.OrderByDescending(customerAdminObject => customerAdminObject.GetValue(category)));

            this.Clear();
            this.AddRange(sortedList);
        }

        public void ExitReadLock()
        {
            throw new NotImplementedException();
        }

        public bool InitialRefreshPerformed { get; private set; }
        public void ModifyItem(IAdminObject item)
        {
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

