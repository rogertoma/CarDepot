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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.Controls;
using CarDepot.Pages;
using CarDepot.Resources;

namespace CarDepot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserAdminObject _user;
        public MainWindow()
        {
            InitializeComponent();
            LoadTabs();
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
                TabItem tabItem = new TabItem();
                Type elementType = Type.GetType(tab);
                object page = Activator.CreateInstance(elementType);
                tabItem.Header = ((IPropertyPage)page).PageTitle;
                tabItem.Content = page;
                PagesTabControl.Items.Add(tabItem);
            }
        }

        private void LoadTabs()
        {
            TabItem tabItem = new TabItem ();
            tabItem.Header = "ROGER";
            Type elementType = Type.GetType("CarDepot.Controls.LogonPageControl");
            object list = Activator.CreateInstance(elementType);
            tabItem.Content = (LogonPageControl)list;
            PagesTabControl.Items.Add(tabItem);

            TabItem tabItem2 = new TabItem();
            tabItem2.Header = "ActiveVehiclePage";
            Type elementType2 = Type.GetType("CarDepot.Pages.ActiveVehiclePage");
            object list2 = Activator.CreateInstance(elementType2);
            tabItem2.Content = (ActiveVehiclePage)list2;
            PagesTabControl.Items.Add(tabItem2);
        }

        private void CarDepot_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LogonPage logon = new LogonPage();
            logon.Show();
        }
    }
}
