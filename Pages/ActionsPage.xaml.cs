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
using CarDepot.Pages;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Controls.GeneralControls
{
    /// <summary>
    /// Interaction logic for ActionsControl.xaml
    /// </summary>
    public partial class ActionsControl : UserControl, IPropertyPage, IPropertyPanel
    {

        public ActionsControl()
        {
            InitializeComponent();
            ApplyActiveUserPermissions();
        }

        public string PageTitle
        {
            get { return Strings.CONTROL_ACTIONSCONTROL_TAB_TITLE; }
        }

        public bool IsCloseable
        {
            get { return false; }
        }

        private void BtnCreateNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            VehicleInfoWindow page = new VehicleInfoWindow();

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.VehicleTabColor;
            page.SetParentTabControl(tabItem);
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = "New Vehicle";
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);

            tabItem.Focus();
        }

        private void BtnSearchVehicles_Click(object sender, RoutedEventArgs e)
        {
            VehicleSearchPage page = new VehicleSearchPage();

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchVehicleColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void BtnCreateNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerInfoPage page = new CustomerInfoPage();

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.CustomerTabColor;
            page.SetParentTabControl(tabItem);
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = "New Customer";
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();

            //CustomerInfoPage customerInfo = new CustomerInfoPage();
            //customerInfo.Show();
        }

        private void BtnSearchTasks_Click(object sender, RoutedEventArgs e)
        {

            TasksSearchPage page = new TasksSearchPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchTasksColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void BtnSearchCustomers_Click(object sender, RoutedEventArgs e)
        {
            CustomerSearchPage page = new CustomerSearchPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchCustomerColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        public void LoadPanel(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.CreateNewCustomer))
            {
                BtnCreateNewCustomer.Visibility = Visibility.Visible;
            }
            else
            {
                BtnCreateNewCustomer.Visibility = Visibility.Hidden;
            }

            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.CreateNewVehicle))
            {
                BtnCreateNewVehicle.Visibility = Visibility.Visible;
            }
            else
            {
                BtnCreateNewVehicle.Visibility = Visibility.Hidden;
            }

        }

        private void BtnKeyCheckout_Click(object sender, RoutedEventArgs e)
        {
            KeyCheckoutPage page = new KeyCheckoutPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchTasksColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }
    }
}
