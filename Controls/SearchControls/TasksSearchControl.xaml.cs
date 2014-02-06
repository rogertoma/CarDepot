using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for TasksSearchPage.xaml
    /// </summary>
    public partial class TasksSearchControl: UserControl
    {
        private VehicleCache cache = null;
        private string assignedTo = null;

        public TasksSearchControl()
        {
            InitializeComponent();

            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbAssignedTo.Items.Add(lblRow);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<VehicleCacheTaskSearchKey, string> searchParam = new Dictionary<VehicleCacheTaskSearchKey, string>();

            assignedTo = cmbAssignedTo.Text;

            if (string.IsNullOrEmpty(assignedTo))
            {
                cmbAssignedTo.Background = Brushes.OrangeRed;
                return;
            }
            else
            {
                cmbAssignedTo.Background = Brushes.Gray;
            }
            searchParam.Add(VehicleCacheTaskSearchKey.AssignedTo, assignedTo);
            cache = new VehicleCache(Settings.VehiclePath, searchParam);

            UpdateUI();
        }
        
        private void UpdateUI()
        {
            lstVehicles.LstSearchResults.Items.Clear();
            lstTasks.Items.Clear();

            //lstVehicles.Cache = cache;

            lstVehicles.SetContent(cache);

            foreach (VehicleAdminObject vehicle in cache)
            {
                foreach (var vehicleTask in vehicle.VehicleTasks)
                {
                    if (vehicleTask.AssignedTo == assignedTo)
                    {
                        ListViewItem taskItem = new ListViewItem();
                        taskItem.Content = vehicleTask;
                        lstTasks.Items.Add(taskItem);
                    }
                }
            }
            
        }

        private void lstTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem taskItem = lstTasks.SelectedItem as ListViewItem;
            if (taskItem == null)
                return;

            VehicleTask task = taskItem.Content as VehicleTask;
            if (task == null)
                return;

            foreach (ListViewItem listViewItem in lstVehicles.LstSearchResults.Items)
            {
                VehicleAdminObject vehicle = listViewItem.Content as VehicleAdminObject;
                if (vehicle == null)
                    continue;

                if (vehicle.Id == task.TaskVehicleId)
                {
                    UpdateListHighlights(vehicle);
                }                
            }
        }

        private void lstVehicles_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ListViewItem vehicleItem = lstVehicles.LstSearchResults.SelectedItem as ListViewItem;
            if (vehicleItem == null)
                return;

            VehicleAdminObject vehicle = vehicleItem.Content as VehicleAdminObject;
            if (vehicle == null)
                return;

            UpdateListHighlights(vehicle);
        }

        private void lstTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListViewItem taskItem = lstTasks.SelectedItem as ListViewItem;
            if (taskItem == null)
                return;

            VehicleTask task = taskItem.Content as VehicleTask;
            if (task == null)
                return;

            foreach (ListViewItem listViewItem in lstVehicles.LstSearchResults.Items)
            {
                VehicleAdminObject vehicle = listViewItem.Content as VehicleAdminObject;
                if (vehicle == null)
                    continue;

                if (vehicle.Id == task.TaskVehicleId)
                {
                    Utilities.LoadVehicleInfoWindow(vehicle);
                }
            }
        }

        private void UpdateListHighlights(VehicleAdminObject vehicle)
        {
            if (vehicle == null)
                return;

            foreach (ListViewItem listViewItem in lstTasks.Items)
            {
                VehicleTask task = listViewItem.Content as VehicleTask;
                if (task == null)
                    continue;

                if (vehicle.Id == task.TaskVehicleId)
                {
                    listViewItem.Background = Brushes.DeepSkyBlue;
                }
                else
                {
                    listViewItem.Background = Brushes.White;
                }
            }

            foreach (ListViewItem listViewItem in lstVehicles.LstSearchResults.Items)
            {
                VehicleAdminObject vehicleItem = listViewItem.Content as VehicleAdminObject;
                if (vehicleItem == null)
                    continue;

                if (vehicleItem.Id == vehicle.Id)
                {
                    listViewItem.Background = Brushes.DeepSkyBlue;
                }
                else
                {
                    listViewItem.Background = Brushes.White;    
                }
                
            }
        }


    }
}
