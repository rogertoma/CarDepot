using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using CarDepot.Controls;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot
{
    /// <summary>
    /// Interaction logic for VehicleInfoWindow.xaml
    /// </summary>
    public partial class VehicleInfoWindow : Window, IPropertyPanel
    {
        private VehicleAdminObject _vehicle;
        List<IPropertyPanel> propertyPanels = new List<IPropertyPanel>();

        public VehicleInfoWindow()
        {
            InitializeComponent();
            _vehicle = CacheManager.ActiveVehicleCache[0];

            propertyPanels.Add(BasicVehicleControlPropertyPanel);
            propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);
            LoadPanel(_vehicle);
        }

        public VehicleInfoWindow(VehicleAdminObject vehicle)
        {
            InitializeComponent();
            _vehicle = vehicle;

            propertyPanels.Add(BasicVehicleControlPropertyPanel);
            propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);

            ((VehicleCache)_vehicle.Cache).UpdateObjectWithDataFromFile(_vehicle);

            LoadPanel(_vehicle);
        }

        public void LoadPanel(IAdminObject item)
        {
            foreach (IPropertyPanel propertyPanel in propertyPanels)
            {
                propertyPanel.LoadPanel(item);
            }
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            
        }

        private void VehicleInfoWindow_OnClosing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING,
                Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING_TITLE, MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                bool successfulSave = _vehicle.Save(this);
                if (!successfulSave)
                {
                    MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                    Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
                    e.Cancel = true;
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}
