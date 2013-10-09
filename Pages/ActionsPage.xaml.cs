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

namespace CarDepot.Controls.GeneralControls
{
    /// <summary>
    /// Interaction logic for ActionsControl.xaml
    /// </summary>
    public partial class ActionsControl : UserControl, IPropertyPage
    {
        private TabControl mainTabControl = null;

        public ActionsControl()
        {
            InitializeComponent();
        }

        public string PageTitle
        {
            get { return Strings.CONTROL_ACTIONSCONTROL_TAB_TITLE; }
        }

        public void SetTabControlContext(TabControl control)
        {
            mainTabControl = control;
        }

        public bool IsCloseable
        {
            get { return false; }
        }

        private void BtnCreateNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            VehicleInfoWindow page = new VehicleInfoWindow();
            page.Show();
        }

        private void BtnSearchVehicles_Click(object sender, RoutedEventArgs e)
        {
            VehicleSearchPage page = new VehicleSearchPage();

            if (mainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            mainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }
    }
}
