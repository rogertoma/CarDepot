using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.Controls;
using CarDepot.Controls.VehicleControls;
using CarDepot.Pages;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;


namespace CarDepot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserAdminObject _user;
        private int ControlSwitchCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            _user = CacheManager.UserCache[0];
            LoadUserTabs();
        }

        public MainWindow(UserAdminObject user)
        {
            InitializeComponent();
            _user = user;
            LoadUserTabs();
        }

        private void LoadUserTabs()
        {
            foreach (string tab in _user.MainTabPages)
            {
                Type elementType = Type.GetType(tab);
                object page = Activator.CreateInstance(elementType);

                var propertyPage = page as IPropertyPage;

                PagesTabControl.Items.Add(GetPageTabItem(propertyPage));
                CacheManager.MainTabControl = PagesTabControl;
            }
        }

        private TabItem GetPageTabItem(IPropertyPage page)
        {
            if (page == null)
                return null;

            if (page.IsCloseable)
            {
                ClosableTab theTabItem = new ClosableTab();
                theTabItem.Height = LookAndFeel.TabItemHeight;
                theTabItem.Title = ((IPropertyPage)page).PageTitle;
                theTabItem.Content = page;
                return theTabItem;
            }
            else
            {
                TabItem tabItem = new TabItem();
                tabItem.Height = LookAndFeel.TabItemHeight;
                tabItem.Header = ((IPropertyPage)page).PageTitle;
                tabItem.Foreground = Brushes.Red;
                tabItem.Content = page;
                return tabItem;
            }
        }

        //private void LoadTabs()
        //{
        //    TabItem tabItem = new TabItem ();
        //    tabItem.Header = "ROGER";
        //    Type elementType = Type.GetType("CarDepot.Controls.LogonPageControl");
        //    object list = Activator.CreateInstance(elementType);
        //    tabItem.Content = (LogonPageControl)list;
        //    PagesTabControl.Items.Add(tabItem);

        //    TabItem tabItem2 = new TabItem();
        //    tabItem2.Header = "ActiveVehiclePage";
        //    Type elementType2 = Type.GetType("CarDepot.Pages.ActiveVehiclePage");
        //    object list2 = Activator.CreateInstance(elementType2);
        //    tabItem2.Content = (ActiveVehiclePage)list2;
        //    PagesTabControl.Items.Add(tabItem2);
        //}

        private void CarDepot_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LogonPage logon = new LogonPage();
            logon.Show();
        }

        private void CarDepot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                ControlSwitchCount++;
            }
            else if (e.Key == Key.F5)
            {
                CacheManager.AllVehicleCache = new VehicleCache(Settings.VehiclePath, new Dictionary<VehicleCacheSearchKey, string>(), true);
            }
            else
            {
                ControlSwitchCount = 0;
            }

            if (ControlSwitchCount == 3)
            {
                ControlSwitchCount = 0;
                if (CacheManager.ActiveUser.UiMode == CacheManager.UIMode.Customer)
                {
                    foreach (TabItem tabItem in CacheManager.MainTabControl.Items)
                    {
                        tabItem.Foreground = Brushes.Red;
                    }

                    CacheManager.ActiveUser.UiMode = CacheManager.UIMode.Full;
                }
                else
                {
                    foreach (TabItem tabItem in CacheManager.MainTabControl.Items)
                    {
                        tabItem.Foreground = Brushes.Black;
                    }

                    CacheManager.ActiveUser.UiMode = CacheManager.UIMode.Customer;
                }

                foreach (var propertyPanel in CacheManager.ActiveUser.OpenedPages)
                {
                    propertyPanel.ApplyUiMode();
                }
            }
        }
    }
}
