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
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Controls.GeneralControls
{
    /// <summary>
    /// Interaction logic for KeyCheckoutControl.xaml
    /// </summary>
    public partial class KeyCheckoutControl : UserControl
    {
        VehicleAdminObject _foundVehicle = null;
        private VehicleCache _cache = null;

        public KeyCheckoutControl()
        {
            InitializeComponent();

            btnCheckIn.Visibility = Visibility.Collapsed;
            btnCheckOut.Visibility = Visibility.Collapsed;

            if (!CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.ShowCheckedOutBy))
            {
                btnRefresh.Visibility = Visibility.Collapsed;
                LstSearchResults.Visibility = Visibility.Collapsed;
            }
        }

        private void clearState()
        {
            _foundVehicle = null;
            lblVinSearchResults.Content = String.Empty;
            btnCheckIn.Visibility = Visibility.Collapsed;
            btnCheckOut.Visibility = Visibility.Collapsed;
        }

        private void txtVinSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVinSearch.Text.Length != 6)
            {
                clearState();
                return;
            }

            if (CacheManager.AllVehicleCache == null)
            {
                MessageBox.Show("CacheManager.AllVehicleCache not yet loaded please wait and try again");
                return;
            }

            foreach (VehicleAdminObject vehicleAdminObject in CacheManager.AllVehicleCache)
            {
                if (vehicleAdminObject.GetValue(PropertyId.VinNumber).ToLower().EndsWith(txtVinSearch.Text.ToLower()))
                {
                    _foundVehicle = vehicleAdminObject;
                }
            }

            if (_foundVehicle == null)
            {
                clearState();
                return;
            }

            lblVinSearchResults.Content = _foundVehicle.Year + " " + _foundVehicle.Make + " " + _foundVehicle.Model; 

            string checkOutUser = _foundVehicle.GetValue(PropertyId.CheckOutBy);
            if (string.IsNullOrEmpty(checkOutUser))
            {
                btnCheckOut.Visibility = Visibility.Visible;
            }
            else
            {
                btnCheckIn.Visibility = Visibility.Visible;
            }

        }

        private void btnCheckOut_Click(object sender, RoutedEventArgs e)
        {
            _foundVehicle.SetValue(PropertyId.CheckOutBy, CacheManager.ActiveUser.Name);
            bool successfulSave = _foundVehicle.Save(null);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
                return;
            }

            if (_foundVehicle != null && _foundVehicle.Cache != null)
            {
                _foundVehicle.Cache.ModifyItem(_foundVehicle);
            }

            txtVinSearch.Text = "";
            clearState();   
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (_foundVehicle.GetValue(PropertyId.CheckOutBy) != CacheManager.ActiveUser.Name && CacheManager.ActiveUser.Name != "Roger Toma" && CacheManager.ActiveUser.Name != "Filip Mitrofanov")
            {
                MessageBox.Show(Strings.CONTROL_KEYCHECKOUT_DIFFERENTUSERATTEMPTSCHECKIN, Strings.ERROR);
                return;
            }

            _foundVehicle.SetValue(PropertyId.CheckOutBy, "");
            bool successfulSave = _foundVehicle.Save(null);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
                return;
            }

            if (_foundVehicle != null && _foundVehicle.Cache != null)
            {
                _foundVehicle.Cache.ModifyItem(_foundVehicle);
            }

            txtVinSearch.Text = "";
            clearState();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LstSearchResults.Clear();

            Dictionary<VehicleCacheSearchKey, string> searchParam = new Dictionary<VehicleCacheSearchKey, string>();
            searchParam.Add(VehicleCacheSearchKey.IsCheckedOut, null);
            

            _cache = new VehicleCache(Settings.VehiclePath, searchParam);
            LstSearchResults.SetContent(_cache);
        }
    }
}
