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
using CarDepot.VehicleStore;

namespace CarDepot.Controls
{
    /// <summary>
    /// Interaction logic for VehicleListView.xaml
    /// </summary>
    public partial class VehicleListView : UserControl
    {
        private VehicleTileControl mouseTile = null;
        private VehicleCache cache = null;
        private List<VehicleInfoWindow> openedWindows = new List<VehicleInfoWindow>();

        public VehicleListView()
        {
            InitializeComponent();
        }

        public void SetContent(VehicleCache vehicleCache)
        {
            cache = vehicleCache;
            InitializeList();
        }

        private void InitializeList()
        {
            cache.ItemUpdate += cache_ItemUpdate;
            foreach (VehicleAdminObject vehicle in cache)
            {
                ListViewItem item = new ListViewItem();
                item.Content = vehicle;
                item.MouseDoubleClick += item_MouseDoubleClick;
                item.KeyDown += item_KeyDown;
                item.MouseEnter += item_MouseEnter;
                item.MouseMove += item_MouseMove;
                item.MouseLeave += item_MouseLeave;
                LstSearchResults.Items.Add(item);
            }
        }

        void item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mouseTile != null)
                mouseTile.Hide();
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
                    mouseTile.Hide();
                    mouseTile = new VehicleTileControl(vehicle);
                }
            }

            mouseTile.Left = System.Windows.Forms.Cursor.Position.X;
            mouseTile.Top = System.Windows.Forms.Cursor.Position.Y + 20;
            mouseTile.Show();
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
                    mouseTile.Hide();
                    mouseTile = new VehicleTileControl(vehicle);
                }
            }

            mouseTile.Left = System.Windows.Forms.Cursor.Position.X;
            mouseTile.Top = System.Windows.Forms.Cursor.Position.Y;
            mouseTile.Show();

        }

        private void cache_ItemUpdate(object sender, AdminItemCache.UpdateEventArgs e)
        {
            if (e.Type == AdminItemCache.UpdateType.ModifyItem)
            {
                ListViewItem listViewItem = (from ListViewItem item in LstSearchResults.Items
                                             let listContent = item.Content as VehicleAdminObject
                                             where listContent.ObjectId == e.ItemId
                                             select item).FirstOrDefault();

                VehicleAdminObject cacheContent =
                    cache.FirstOrDefault(vehicleAdminObject => vehicleAdminObject.ObjectId == e.ItemId);

                if (listViewItem != null && cacheContent != null)
                {
                    listViewItem.Content = null;
                    listViewItem.Content = cacheContent;
                }
            }
        }

        private void LoadVehicleInfoWindow(VehicleAdminObject vehicle)
        {
            if (vehicle == null)
            {
                throw new NullReferenceException("Load vehicle info window requires non null vehicle");
            }
            else
            {
                VehicleInfoWindow window = new VehicleInfoWindow(vehicle);
                window.Show();
                openedWindows.Add(window);
            }
        }

        void item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoadVehicleInfoWindow(((ListViewItem)sender).Content as VehicleAdminObject);
        }

        void item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadVehicleInfoWindow(((ListViewItem)sender).Content as VehicleAdminObject);
        }

        private GridViewColumnHeader lastHeaderClicked = null;
        ListSortDirection lastDirection = ListSortDirection.Ascending;

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction = ListSortDirection.Ascending;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {

                    if (headerClicked == lastHeaderClicked)
                    {
                        direction = lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    }
                }

                string header = headerClicked.Column.Header as string;


                PropertyId id = (PropertyId)Enum.Parse(typeof(PropertyId), header);
                cache.SortCache(id, direction);

                LstSearchResults.Items.Clear();
                foreach (VehicleAdminObject vehicle in cache)
                {
                    ListViewItem item = new ListViewItem();
                    item.Content = vehicle;
                    item.MouseDoubleClick += item_MouseDoubleClick;
                    item.KeyDown += item_KeyDown;

                    LstSearchResults.Items.Add(item);
                }

                //if (direction == ListSortDirection.Ascending)
                //{
                //    headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                //}
                //else
                //{
                //    headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                //}

                if (lastHeaderClicked != null && lastHeaderClicked != headerClicked)
                {
                    lastHeaderClicked.Column.HeaderTemplate = null;
                }

                lastHeaderClicked = headerClicked;
                lastDirection = direction;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (VehicleInfoWindow vehicleInfoWindow in openedWindows)
            {
                VehicleInfoWindow window = vehicleInfoWindow;
                window.Close();
                window = null;
            }
        }
    }
}
