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
            cache.ItemUpdate += cache_ItemUpdate;
            foreach (VehicleAdminObject vehicle in cache)
            {
                ListViewItem item = new ListViewItem();
                item.Content = vehicle;
                item.MouseDoubleClick += item_MouseDoubleClick;
                item.KeyDown += item_KeyDown;

                LstSearchResults.Items.Add(item);
            }
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

    }
}
