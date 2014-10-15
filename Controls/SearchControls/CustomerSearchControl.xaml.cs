using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.VehicleStore;
using CarDepot.Resources;
using CarDepot.Pages;
using System.ComponentModel;

namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for CustomerSearchControl.xaml
    /// </summary>
    public partial class CustomerSearchControl : UserControl
    {
        private CustomerCache cache = null;

        public CustomerSearchControl()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<CustomerCacheSearchKey, string> searchParam = new Dictionary<CustomerCacheSearchKey, string>();

            searchParam.Add(CustomerCacheSearchKey.FirstName, txtFirstName.Text.Trim());
            searchParam.Add(CustomerCacheSearchKey.LastName, txtLastName.Text.Trim());
            searchParam.Add(CustomerCacheSearchKey.PhoneNumber, txtPhoneNumber.Text.Trim());

            cache = new CustomerCache(searchParam);
            cache.ItemUpdate += cache_ItemUpdate;

            UpdateUI();
        }

        private void UpdateUI()
        {
            lstCustomers.Items.Clear();

            foreach (CustomerAdminObject customer in cache)
            {
                ListViewItem item = new ListViewItem();
                item.Content = customer;
                lstCustomers.Items.Add(item);
            }

        }

        private void lstCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = lstCustomers.SelectedItem as ListViewItem;
            if (item == null)
                return;

            CustomerAdminObject customer = item.Content as CustomerAdminObject;
            if (customer == null)
                return;

            CustomerInfoPage customerInfo = new CustomerInfoPage(customer);

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.CustomerTabColor;
            customerInfo.SetParentTabControl(tabItem);
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = "Customer: " + customer.LastName + ", " + customer.FirstName;
            tabItem.Content = customerInfo;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();

            //CustomerInfoPage customerInfo = new CustomerInfoPage(customer);
            //customerInfo.Show();
        }

        private void cache_ItemUpdate(object sender, AdminItemCache.UpdateEventArgs e)
        {
            if (e.Type == AdminItemCache.UpdateType.ModifyItem)
            {
                ListViewItem listViewItem = (from ListViewItem item in lstCustomers.Items
                                             let listContent = item.Content as CustomerAdminObject
                                             where listContent.ObjectId == e.ItemId
                                             select item).FirstOrDefault();

                CustomerAdminObject cacheContent =
                    cache.FirstOrDefault(customerAdminObject => customerAdminObject.ObjectId == e.ItemId);

                if (cacheContent != null)
                {
                    CustomerCache tempCache = cacheContent.Cache as CustomerCache;
                    cacheContent = new CustomerAdminObject(cacheContent.ObjectId);
                    cacheContent.Cache = tempCache;

                    if (cacheContent.GetValue(PropertyId.IsDeleted) == true.ToString())
                    {
                        lstCustomers.Items.Remove(listViewItem);
                        return;
                    }
                }

                if (listViewItem != null)
                {
                    listViewItem.Content = null;
                    listViewItem.Content = cacheContent;
                }
            }
        }

        private GridViewColumnHeader lastHeaderClicked = null;
        ListSortDirection lastDirection = ListSortDirection.Ascending;

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction = ListSortDirection.Ascending;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {

                    if (headerClicked == lastHeaderClicked)
                    {
                        direction = lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    }
                }

                string header = headerClicked.Column.Header as string;


                PropertyId id = (PropertyId)Enum.Parse(typeof(PropertyId), header);
                cache.SortCache(id, direction);

                lstCustomers.Items.Clear();
                foreach (CustomerAdminObject customer in cache)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = customer;
                    //item.MouseDoubleClick += item_MouseDoubleClick;
                    //item.KeyDown += item_KeyDown;

                    lstCustomers.Items.Add(item);
                }

                if (lastHeaderClicked != null && lastHeaderClicked != headerClicked)
                {
                    lastHeaderClicked.Column.HeaderTemplate = null;
                }

                lastHeaderClicked = headerClicked;
                lastDirection = direction;
            }
        }
    }
}
