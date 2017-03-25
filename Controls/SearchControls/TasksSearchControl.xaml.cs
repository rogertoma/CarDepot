using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CarDepot.Pages;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using System;


namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for TasksSearchPage.xaml
    /// </summary>
    public partial class TasksSearchControl : UserControl, IPropertyPage
    {
        private VehicleCache cache = null;
        Dictionary<VehicleCacheTaskSearchKey, string> searchParam = new Dictionary<VehicleCacheTaskSearchKey, string>();

        public TasksSearchControl()
        {
            InitializeComponent();

            InitializeSearchFields();

            DefaultToActiveUser();
        }

        private void DefaultToActiveUser()
        {
            bool foundUser = false;
            for (int i = 0; i < cmbAssignedTo.Items.Count; i++)
            {
                Label lbl = cmbAssignedTo.Items[i] as Label;
                if (lbl == null)
                    continue;

                if (lbl.Content.Equals(CacheManager.ActiveUser.Name))
                {
                    cmbAssignedTo.SelectedIndex = i;
                }
            }

            btnSearch_Click(null, null);
        }

        private void InitializeSearchFields()
        {
            cmbAssignedTo.Items.Add(new Label { Content = "" });
            cmbCategory.Items.Add(new Label { Content = "" });

            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbAssignedTo.Items.Add(lblRow);
            }

            foreach (VehicleTask.TaskCategoryTypes type in Enum.GetValues(typeof(VehicleTask.TaskCategoryTypes)))
            {
                cmbCategory.Items.Add(type.ToString());
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            searchParam.Clear();
            
            string assignedTo = cmbAssignedTo.Text;
            if (!string.IsNullOrEmpty(assignedTo))
            {
                searchParam.Add(VehicleCacheTaskSearchKey.AssignedTo, assignedTo);
            }

            string category = cmbCategory.Text;
            if (!string.IsNullOrEmpty(category))
            {
                searchParam.Add(VehicleCacheTaskSearchKey.Category, category);
            }

            cache = new VehicleCache(Settings.VehiclePath, searchParam);

            UpdateUI();
            lblTotalCount.Content = cache.Count.ToString();
        }
        
        private void UpdateUI()
        {
            lstVehicles.LstSearchResults.Items.Clear();
            lstTasks.Items.Clear();

            //lstVehicles.Cache = cache;

            lstVehicles.SetContent(cache);

            foreach (VehicleAdminObject vehicle in cache)
            {
                foreach (var task in vehicle.VehicleTasks)
                {
                    if (searchParam.ContainsKey(VehicleCacheTaskSearchKey.AssignedTo) &&
                        task.AssignedTo == searchParam[VehicleCacheTaskSearchKey.AssignedTo] &&
                        !task.Status.ToLower().Equals(VehicleTask.StatusTypes.Completed.ToString().ToLower()))
                    {
                        ListViewItem taskItem = new ListViewItem();
                        taskItem.Content = task;
                        lstTasks.Items.Add(taskItem);
                    }
                    else if (searchParam.ContainsKey(VehicleCacheTaskSearchKey.Category) &&
                        task.Category == searchParam[VehicleCacheTaskSearchKey.Category] && 
                        !task.Status.ToLower().Equals(VehicleTask.StatusTypes.Completed.ToString().ToLower()))
                    {
                        ListViewItem taskItem = new ListViewItem();
                        taskItem.Content = task;
                        lstTasks.Items.Add(taskItem);
                    }
                }
            }

            //Priority0
            int insertPosition = 0;
            for (int i = 0; i < Enum.GetNames(typeof(VehicleTask.TaskPriority)).Length; i++)
            {
                for (int j = 0; j < lstTasks.Items.Count; j++)
                {
                    //object currentItem = (object)lstTasks.Items[j];
                    ListBoxItem item = lstTasks.Items.GetItemAt(j) as ListBoxItem;

                    VehicleTask task = item.Content as VehicleTask;
                    if (task.Priority != null &&
                        task.Priority.ToLower() == "priority" + i.ToString())
                    {
                        if (task.Priority.ToLower() == "priority0")
                        {
                            item.Foreground = Brushes.Red;
                            item.FontWeight = FontWeights.ExtraBold;
                        }
                        else if (task.Priority.ToLower() == "priority1")
                        {
                            item.FontWeight = FontWeights.Bold;
                        }

                        this.lstTasks.Items.RemoveAt(j);
                        this.lstTasks.Items.Insert(insertPosition, item);
                        insertPosition++;
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
                    Utilities.LoadVehicleInfoWindow(vehicle, VehicleInfoWindow.VehicleInfoWindowTabs.Tasks);
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
                    if (vehicleItem.IsOffProperty)
                        listViewItem.Background = Brushes.Pink;
                    else
                        listViewItem.Background = Brushes.White;    
                }
                
            }
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
                if (header.ToLower() == "task")
                {
                    header = "Id";
                }

                //PropertyId id = (PropertyId)Enum.Parse(typeof(PropertyId), header);
                List<String> rowList = new List<string>();
                List<VehicleTask> tasks = new List<VehicleTask>();

                foreach (ListViewItem listViewItem in lstTasks.Items)
                {
                    VehicleTask vehicleTask = (VehicleTask)listViewItem.Content;
                    tasks.Add(vehicleTask);
                    //rowList.Add();
                }

                List<VehicleTask> sortedTasks = new List<VehicleTask>();
                sortedTasks.AddRange(direction == ListSortDirection.Ascending
                        ? tasks.OrderBy(vehicleTask => vehicleTask.GetType().GetProperty(header).GetValue(vehicleTask, null))
                        : tasks.OrderByDescending(vehicleTask => vehicleTask.GetType().GetProperty(header).GetValue(vehicleTask, null)));
                /*
                List<VehicleTask> sortedtasks = new List<VehicleTask>();
                sortedtasks.AddRange(direction == ListSortDirection.Ascending
                        ? tasks.OrderBy(vehicleAdminObject => vehicleAdminObject.GetValue(category))
                        : this.OrderByDescending(vehicleAdminObject => vehicleAdminObject.GetValue(category)));
                */
                
                //cache.SortCache(id, direction);

                lstTasks.Items.Clear();
                foreach (VehicleTask task in sortedTasks)
                {
                    ListViewItem taskItem = new ListViewItem();
                    taskItem.Content = task;
                    lstTasks.Items.Add(taskItem);
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

        public string PageTitle
        {
            get { return Strings.CONTROL_TASKSCONTROL_TAB_TITLE; }
        }

        public bool IsCloseable
        {
            get { return false; }
        }
    }
}
