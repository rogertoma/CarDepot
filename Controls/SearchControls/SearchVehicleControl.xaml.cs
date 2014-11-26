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
using Microsoft.Win32;

namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for SearchVehicleControl.xaml
    /// </summary>
    public partial class SearchVehicleControl : UserControl, IPropertyPanel
    {
        
        private VehicleCache cache = null;
        public SearchVehicleControl()
        {
            InitializeComponent();
            ApplyActiveUserPermissions();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LstSearchResults.Clear();

            Dictionary<VehicleCacheSearchKey, string> searchParam = new Dictionary<VehicleCacheSearchKey, string>();
            searchParam.Add(VehicleCacheSearchKey.FromDate, dpFrom.SelectedDate.ToString());
            searchParam.Add(VehicleCacheSearchKey.ToDate, dpTo.SelectedDate.ToString());
            if (cbSold.IsChecked == true)
            {
                searchParam.Add(VehicleCacheSearchKey.IsSold, null);
            }
            
            if (cbAvailable.IsChecked == true)
            {
                searchParam.Add(VehicleCacheSearchKey.IsAvailable, null);
            }

            cache = new VehicleCache(Settings.VehiclePath, searchParam);
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
            else
            {
                SaveFileDialog saveDialog;
                saveDialog = new SaveFileDialog();
                
                saveDialog.FileName = "Document";
                saveDialog.DefaultExt = ".xls";                 // Default file extension
                saveDialog.Filter = "Excel File (.xls)|*.xls";  // Filter files by extension 
                // Show save file dialog box
                Nullable<bool> result = saveDialog.ShowDialog();
                // Process save file dialog box results 
                if (result == true)
                {
                    // Save document 
                    string filename = saveDialog.FileName;
                    ExportVehicleInfo exp = new ExportVehicleInfo(vehicleList, filename);
                    // Auto-open
                    System.Diagnostics.Process.Start(filename);
                }
                else
                {
                    return;
                }
            }
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
            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.GenerateReport))
            {
                BtnGenerateReport.Visibility = Visibility.Visible;
            }
            else
            {
                BtnGenerateReport.Visibility = Visibility.Hidden;
            }
        }
    }
}
