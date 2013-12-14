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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for SearchVehicleControl.xaml
    /// </summary>
    public partial class SearchVehicleControl : UserControl
    {
        
        private VehicleCache cache = null;
        public SearchVehicleControl()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            cache = new VehicleCache(Settings.VehiclePath, null);
            LstSearchResults.SetContent(cache);
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            VehicleCache vehicleList = LstSearchResults.Cache;
            if (vehicleList == null)
            {
                MessageBox.Show("The report is empty! Search first in order to generate a report.");
                return;
            }

            ExportVehicleInfo exp = new ExportVehicleInfo(vehicleList);
        }

    }
}
