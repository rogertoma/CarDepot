using CarDepot.Resources;
using CarDepot.VehicleStore;
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

namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for DeliveryDateVehicleControl.xaml
    /// </summary>
    public partial class DeliveryDateVehicleControl : UserControl
    {
        private VehicleTileControl mouseTile = null;
        VehicleCache cache = null;
        int safetyCompleted, detailCompleted, ministryCompleted = 0;
        public DeliveryDateVehicleControl(): this(null, null)
        {
            
        }

        public DeliveryDateVehicleControl(string title, VehicleCache vehicles)
        {
            InitializeComponent();

            cache = vehicles;
            cache.ItemUpdate += cache_ItemUpdate;
            this.lblTitle.Content = title;

            LoadVehicles(vehicles);
        }

        private void LoadVehicles(List<VehicleAdminObject> vehicles)
        {
            foreach (VehicleAdminObject vehicle in vehicles)
            {
                if (vehicle.DeliveryCheckListMechanical.Trim().ToLower() == "true")
                    safetyCompleted++;
                if (vehicle.DeliveryCheckListDetailing.Trim().ToLower() == "true")
                    detailCompleted++;
                if (vehicle.DeliveryCheckListMinistry.Trim().ToLower() == "true")
                    ministryCompleted++;

                vehicle.Cache = cache;
                ListViewItem item = new ListViewItem();
                item.Content = vehicle;
                item.MouseDoubleClick += item_MouseDoubleClick;
                item.KeyDown += item_KeyDown;
                item.MouseEnter += item_MouseEnter;
                item.MouseMove += item_MouseMove;
                item.MouseLeave += item_MouseLeave;

                #region ColorItem

                bool bSafetyCompleted, bDetailCompleted, bMinistryCompleted;
                bool.TryParse(vehicle.GetValue(PropertyId.DeliveryCheckListMechanical), out bSafetyCompleted);
                bool.TryParse(vehicle.GetValue(PropertyId.DeliveryCheckListDetailing), out bDetailCompleted);
                bool.TryParse(vehicle.GetValue(PropertyId.DeliveryCheckListMinistry), out bMinistryCompleted);

                item.Background = Brushes.IndianRed;
                if (bSafetyCompleted)
                {
                    item.Background = Brushes.Yellow;
                }
                if (bSafetyCompleted && bDetailCompleted)
                {
                    item.Background = Brushes.Orange;
                }
                if (bSafetyCompleted && bDetailCompleted && bMinistryCompleted)
                {
                    item.Background = Brushes.LightGreen;
                }

                #endregion

                lstVehicleForDate.Items.Add(item);
            }

            lblTotalResult.Content = cache.Count;
            lblMechanicResult.Content = safetyCompleted;
            lblDetailResult.Content = detailCompleted;
            lblMinistryResult.Content = ministryCompleted;
        }


        private void cache_ItemUpdate(object sender, AdminItemCache.UpdateEventArgs e)
        {
            if (e.Type == AdminItemCache.UpdateType.ModifyItem)
            {
                ListViewItem listViewItem = (from ListViewItem item in lstVehicleForDate.Items
                                             let listContent = item.Content as VehicleAdminObject
                                             where listContent.ObjectId == e.ItemId
                                             select item).FirstOrDefault();

                VehicleAdminObject cacheContent =
                    cache.FirstOrDefault(vehicleAdminObject => vehicleAdminObject.ObjectId == e.ItemId);

                if (cacheContent != null)
                {
                    VehicleCache tempCache = cacheContent.Cache as VehicleCache;
                    cacheContent = new VehicleAdminObject(cacheContent.ObjectId);
                    cacheContent.Cache = tempCache;

                    if (cacheContent.GetValue(PropertyId.IsDeleted) == true.ToString())
                    {
                        lstVehicleForDate.Items.Remove(listViewItem);
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

        void item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Utilities.LoadVehicleInfoWindow(((ListViewItem)sender).Content as VehicleAdminObject);
        }

        void item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Utilities.LoadVehicleInfoWindow(((ListViewItem)sender).Content as VehicleAdminObject);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }

        void item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mouseTile != null)
                mouseTile.Close();
        }

        void item_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item == null)
                return;

            VehicleAdminObject vehicle = item.Content as VehicleAdminObject;
            if (vehicle == null)
                return;

            if (mouseTile == null)
            {
                mouseTile = new VehicleTileControl(vehicle);
            }
            else
            {
                if (!mouseTile.Vehicle.Equals(vehicle))
                {
                    mouseTile.Close();
                    mouseTile = new VehicleTileControl(vehicle);
                }
            }

            mouseTile.Left = System.Windows.Forms.Cursor.Position.X;
            mouseTile.Top = System.Windows.Forms.Cursor.Position.Y + 20;
            try
            {
                mouseTile.Show();
            }
            catch
            {
            }
        }

        private void item_MouseEnter(object sender, MouseEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item == null)
                return;

            VehicleAdminObject vehicle = item.Content as VehicleAdminObject;
            if (vehicle == null)
                return;

            if (mouseTile == null)
            {
                mouseTile = new VehicleTileControl(vehicle);
            }
            else
            {
                if (!mouseTile.Vehicle.Equals(vehicle))
                {
                    mouseTile.Close();
                    mouseTile = new VehicleTileControl(vehicle);
                }
            }

            mouseTile.Left = System.Windows.Forms.Cursor.Position.X;
            mouseTile.Top = System.Windows.Forms.Cursor.Position.Y;
            try
            {
                mouseTile.Show();
            }
            catch
            {
            }

        }

    }
}
