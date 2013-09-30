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
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Pages
{
    /// <summary>
    /// Interaction logic for ActiveVehiclePage.xaml
    /// </summary>
    public partial class ActiveVehiclePage : UserControl, IPropertyPage
    {
        private delegate void AddVehicleTileDelegate(VehicleAdminObject vehicle);

        public ActiveVehiclePage()
        {
            InitializeComponent();

            CacheManager.ActiveVehicleCache.ItemUpdate +=ActiveVehicleCache_ItemUpdate;

            TitleLabel.Content = Strings.PAGES_ACTIVEVEHICLE_TITLE_LOADING;

            Thread loadUserThread = new Thread(new ThreadStart(LoadVehicleIcons));
            loadUserThread.SetApartmentState(ApartmentState.STA);
            loadUserThread.Start();
        }

        void ActiveVehicleCache_ItemUpdate(object sender, AdminItemCache.UpdateEventArgs e)
        {
            switch (e.Type)
            {
                case AdminItemCache.UpdateType.ModifyItem:
                    foreach (VehicleTileControl tile in ActiveVehiclesWrapPanel.Children)
                    {
                        if (tile.Vehicle.ObjectId == e.ItemId)
                        {
                            tile.Initialize();
                        }
                    }
                    break;
            }
        }

        private void LoadVehicleIcons()
        {
            foreach (var vehicle in CacheManager.ActiveVehicleCache)
            {
                Dispatcher.Invoke(new AddVehicleTileDelegate(AddVehicleTile), vehicle);
            }

            Dispatcher.Invoke(new Action(() => TitleLabel.Content = Strings.PAGES_ACTIVEVEHICLE_TITLE));
        }
        
        private void AddVehicleTile(VehicleAdminObject vehicle)
        {
            VehicleTileControl iconControl = new VehicleTileControl(vehicle);
            iconControl.MouseDoubleClick += iconControl_MouseDoubleClick;
            ActiveVehiclesWrapPanel.Children.Add(iconControl);
        }

        void iconControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VehicleTileControl iconVehicle = sender as VehicleTileControl;
            if (iconVehicle != null)
            {
                VehicleInfoWindow vehicleInfoWindow = new VehicleInfoWindow(iconVehicle.Vehicle);
                vehicleInfoWindow.Show();
                vehicleInfoWindow.Activate();
                vehicleInfoWindow.Topmost = true;
                vehicleInfoWindow.Focus();
            }
        }

        public string PageTitle
        {
            get { return Strings.PAGES_ACTIVEVEHICLE_TAB_TITLE; }
        }
    }
}
