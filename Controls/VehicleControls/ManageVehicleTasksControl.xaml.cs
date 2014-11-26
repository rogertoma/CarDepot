using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using ExcelLibrary.SpreadSheet;

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for ManageVehicleTasksControl.xaml
    /// </summary>
    public partial class ManageVehicleTasksControl : UserControl, IPropertyPanel
    {
        private VehicleAdminObject _vehicle = new VehicleAdminObject("");

        public ManageVehicleTasksControl()
        {
            InitializeComponent();
            UpdateButtonState();
            LoadUIControls();
        }



        #region events
        private void txtTaskId_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateButtonState();
        }
        #endregion

        public ObservableCollection<VehicleTask> VehicleTasksCollection
        {
            get { return _vehicle.VehicleTasks; }
        }

        public void LoadPanel(IAdminObject item)
        {
            _vehicle = item as VehicleAdminObject;
            if (_vehicle == null)
                return;

            VehicleListView.LoadPanel(_vehicle);
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            throw new NotImplementedException();
        }

        private void ClearTaskContent()
        {
            txtTaskId.Text = "";
            cmbStatus.Items.Clear();
            cmbAssignedTo.Items.Clear();
            cmbCategory.Items.Clear();
            cmbCreatedBy.Items.Clear();
            cmbCompletedBy.Items.Clear();
            txtCost.Text = "";
            txtComments.Text = "";
        }

        private void LoadUIControls()
        {
            ClearTaskContent();
            foreach (VehicleTask.StatusTypes status in Enum.GetValues(typeof(VehicleTask.StatusTypes)))
            {
                Label lblRow = new Label { Content = status };
                cmbStatus.Items.Add(lblRow);
            }

            //Initialize AssignedTo ComboBox
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbAssignedTo.Items.Add(lblRow);
            }

            //Initialize Category ComboBox
            foreach (VehicleTask.TaskCategoryTypes category in Enum.GetValues(typeof(VehicleTask.TaskCategoryTypes)))
            {
                Label lblRow = new Label { Content = category };
                cmbCategory.Items.Add(lblRow);
            }

            //Initialize cmbCreatedBy combobox
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbCreatedBy.Items.Add(lblRow);
            }

            //Initialize cmbCompletedBy ComboBox
            Label emptyRow = new Label { Content = "" };
            cmbCompletedBy.Items.Add(emptyRow);
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbCompletedBy.Items.Add(lblRow);
            }
        }

        private void VehicleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearTaskContent();

            VehicleTask task = null;
            AdminListView listView = sender as AdminListView;
            if (listView != null)
            {
                task = listView.SelectedItem as VehicleTask;
            }

            if (task == null)
                return;

            txtTaskId.Text = task.Id;
            //Initialize Status ComboBox
            int selectedIndex = -1;
            foreach (VehicleTask.StatusTypes status in Enum.GetValues(typeof(VehicleTask.StatusTypes)))
            {
                Label lblRow = new Label { Content = status };
                cmbStatus.Items.Add(lblRow);
                if (status.ToString() == task.Status)
                    selectedIndex = cmbStatus.Items.Count - 1;
            }
            cmbStatus.SelectedIndex = selectedIndex;

            //Initialize AssignedTo ComboBox
            selectedIndex = -1;
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbAssignedTo.Items.Add(lblRow);
                if (user.Name == task.AssignedTo)
                    selectedIndex = cmbAssignedTo.Items.Count - 1;
            }
            cmbAssignedTo.SelectedIndex = selectedIndex;

            //Initialize Category ComboBox
            selectedIndex = -1;
            foreach (VehicleTask.TaskCategoryTypes category in Enum.GetValues(typeof(VehicleTask.TaskCategoryTypes)))
            {
                Label lblRow = new Label { Content = category };
                cmbCategory.Items.Add(lblRow);
                if (category.ToString() == task.Category)
                    selectedIndex = cmbCategory.Items.Count - 1;
            }
            cmbCategory.SelectedIndex = selectedIndex;

            //Initialize cmbCreatedBy ComboBox
            selectedIndex = -1;
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbCreatedBy.Items.Add(lblRow);
                if (user.Name == task.CreatedBy)
                    selectedIndex = cmbCreatedBy.Items.Count - 1;
            }
            cmbCreatedBy.SelectedIndex = selectedIndex;

            //Initialize cmbCompletedBy ComboBox
            selectedIndex = -1;
            Label emptyRow = new Label { Content = "" };
            cmbCompletedBy.Items.Add(emptyRow);
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbCompletedBy.Items.Add(lblRow);
                if (user.Name == task.ClosedBy)
                    selectedIndex = cmbCompletedBy.Items.Count - 1;
            }
            cmbCompletedBy.SelectedIndex = selectedIndex;

            txtCost.Text = task.Cost;

            txtComments.Text = task.Comments;

            txtMinutes.Text = task.Minutes;

            if (task.CreatedDate != String.Empty)
                dpCreatedDate.SelectedDate = DateTime.Parse(task.CreatedDate);

            if (!string.IsNullOrEmpty(task.DueDate))
                dpDueDate.SelectedDate = DateTime.Parse(task.DueDate);

            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            VehicleTask task = VehicleListView.Items.Cast<VehicleTask>().FirstOrDefault(vehicleTask => vehicleTask.Id.Trim() == txtTaskId.Text.Trim());

            if (task == null || task.Id.Trim() != txtTaskId.Text.Trim())
            {
                btnCreate.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnCompleteTask.Visibility = Visibility.Collapsed;
            }
            else if (task.Id == txtTaskId.Text.Trim())
            {
                btnCreate.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;

                btnCompleteTask.Visibility = string.IsNullOrEmpty(task.ClosedBy) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateComboBoxWithSelection(ComboBox cmb, String selection)
        {
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                Label label = cmb.Items[i] as Label;
                if (label == null)
                    continue;

                if (label.Content.ToString().Trim() == selection.Trim())
                {
                    cmb.SelectedIndex = i;
                    break;
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            VehicleTask task = new VehicleTask();

            UpdateTaskBasedOnUi(task);

            _vehicle.VehicleTasks.Add(task);
            VehicleListView.Items.Add(task);
            VehicleListView.SelectedIndex = VehicleListView.Items.Count - 1;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            VehicleTask task = VehicleListView.Items.Cast<VehicleTask>().FirstOrDefault(vehicleTask => vehicleTask.Id.Trim() == txtTaskId.Text.Trim());
            if (task == null)
                return;

            UpdateTaskBasedOnUi(task);

            VehicleListView.Items.Refresh();
            VehicleListView.ForceXmlUpdate();
            VehicleListView.SelectedIndex = -1;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            VehicleTask task = VehicleListView.Items.Cast<VehicleTask>().FirstOrDefault(vehicleTask => vehicleTask.Id.Trim() == txtTaskId.Text.Trim());
            if (task == null)
                return;

            MessageBoxResult result = MessageBox.Show(Strings.CONTROL_MANAGEVEHICLETASKS_CONFIRMDELETE, Strings.CONTROL_MANAGEVEHICLETASKS_CONFIRMDELETETITLE, MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.Cancel)
                return;

            VehicleListView.Items.Remove(task);
            _vehicle.VehicleTasks.Remove(task);
            VehicleListView.ForceXmlUpdate();
        }

        private void btnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            VehicleTask task = VehicleListView.Items.Cast<VehicleTask>().FirstOrDefault(vehicleTask => vehicleTask.Id.Trim() == txtTaskId.Text.Trim());
            if (task == null)
                return;

            task.ClosedBy = CacheManager.ActiveUser.Name;
            task.Status = VehicleTask.StatusTypes.Completed.ToString();
            VehicleListView.Items.Refresh();
            VehicleListView.ForceXmlUpdate();
            VehicleListView.SelectedIndex = -1;
        }

        private void UpdateTaskBasedOnUi(VehicleTask task)
        {
            task.Id = txtTaskId.Text;
            task.TaskVehicleId = _vehicle.Id;

            if (cmbStatus.SelectedIndex == -1)
            {
                task.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                UpdateComboBoxWithSelection(cmbStatus, VehicleTask.StatusTypes.NotStarted.ToString());
            }
            else
            {
                task.Status = ((Label)cmbStatus.SelectedItem).Content.ToString();
            }

            if (cmbAssignedTo.SelectedIndex == -1)
            {
                task.AssignedTo = CacheManager.UserCache.SystemAdminAccount.Name;
                UpdateComboBoxWithSelection(cmbAssignedTo, CacheManager.UserCache.SystemAdminAccount.Name);
            }
            else
            {
                task.AssignedTo = ((Label)cmbAssignedTo.SelectedItem).Content.ToString();
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                task.Category = VehicleTask.TaskCategoryTypes.Other.ToString();
                UpdateComboBoxWithSelection(cmbCategory, VehicleTask.TaskCategoryTypes.Other.ToString());
            }
            else
            {
                task.Category = ((Label)cmbCategory.SelectedItem).Content.ToString();
            }

            if (cmbCreatedBy.SelectedIndex == -1)
            {
                task.CreatedBy = CacheManager.ActiveUser.Name;
                UpdateComboBoxWithSelection(cmbCreatedBy, CacheManager.ActiveUser.Name);
            }
            else
            {
                task.CreatedBy = ((Label)cmbCreatedBy.SelectedItem).Content.ToString();
            }

            if (cmbCompletedBy.SelectedIndex != -1)
            {
                task.ClosedBy = ((Label)cmbCompletedBy.SelectedItem).Content.ToString();
            }

            if (dpCreatedDate.SelectedDate == null)
            {
                task.CreatedDate = DateTime.Today.Date.ToString("d");
            }
            else
            {
                task.CreatedDate = ((DateTime)dpCreatedDate.SelectedDate).ToString("d");
            }

            if (dpDueDate.SelectedDate != null)
            {
                task.DueDate = ((DateTime)dpDueDate.SelectedDate).ToString("d");
            }

            task.Cost = txtCost.Text;
            task.Comments = txtComments.Text;

            task.Minutes = txtMinutes.Text;
        }

        private void btnGenerateDefaultTasks_Click(object sender, RoutedEventArgs e)
        {
            VehicleTask picturesTask = new VehicleTask();
            picturesTask.Id = "Take Pictures";
            picturesTask.TaskVehicleId = _vehicle.Id;
            picturesTask.CreatedDate = DateTime.Today.Date.ToString("d");
            picturesTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            picturesTask.AssignedTo = CacheManager.UserCache.SystemAdminAccount.Name;
            picturesTask.Category = VehicleTask.TaskCategoryTypes.Documentation.ToString();
            picturesTask.CreatedBy = CacheManager.ActiveUser.Name;
            _vehicle.VehicleTasks.Add(picturesTask);
            VehicleListView.Items.Add(picturesTask);

            VehicleTask advertisingTask = new VehicleTask();
            advertisingTask.Id = "Advertising";
            advertisingTask.TaskVehicleId = _vehicle.Id;
            advertisingTask.CreatedDate = DateTime.Today.Date.ToString("d");
            advertisingTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            advertisingTask.AssignedTo = CacheManager.UserCache.SystemAdminAccount.Name;
            advertisingTask.Category = VehicleTask.TaskCategoryTypes.Documentation.ToString();
            advertisingTask.CreatedBy = CacheManager.ActiveUser.Name;
            _vehicle.VehicleTasks.Add(advertisingTask);
            VehicleListView.Items.Add(advertisingTask);

            VehicleTask oilTask = new VehicleTask();
            oilTask.Id = "Oil Change";
            oilTask.TaskVehicleId = _vehicle.Id;
            oilTask.CreatedDate = DateTime.Today.Date.ToString("d");
            oilTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            oilTask.AssignedTo = CacheManager.UserCache.SystemAdminAccount.Name;
            oilTask.Category = VehicleTask.TaskCategoryTypes.Mechanic.ToString();
            oilTask.CreatedBy = CacheManager.ActiveUser.Name;
            _vehicle.VehicleTasks.Add(oilTask);
            VehicleListView.Items.Add(oilTask);

            VehicleTask cleaningTask = new VehicleTask();
            cleaningTask.Id = "Clean Car";
            cleaningTask.TaskVehicleId = _vehicle.Id;
            cleaningTask.CreatedDate = DateTime.Today.Date.ToString("d");
            cleaningTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            cleaningTask.AssignedTo = CacheManager.UserCache.SystemAdminAccount.Name;
            cleaningTask.Category = VehicleTask.TaskCategoryTypes.Detail.ToString();
            cleaningTask.CreatedBy = CacheManager.ActiveUser.Name;
            _vehicle.VehicleTasks.Add(cleaningTask);
            VehicleListView.Items.Add(cleaningTask);

            VehicleTask emissionsTask = new VehicleTask();
            emissionsTask.Id = "Emissions Test";
            emissionsTask.TaskVehicleId = _vehicle.Id;
            emissionsTask.CreatedDate = DateTime.Today.Date.ToString("d");
            emissionsTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            emissionsTask.AssignedTo = CacheManager.UserCache.SystemAdminAccount.Name;
            emissionsTask.Category = VehicleTask.TaskCategoryTypes.Mechanic.ToString();
            emissionsTask.CreatedBy = CacheManager.ActiveUser.Name;
            _vehicle.VehicleTasks.Add(emissionsTask);
            VehicleListView.Items.Add(emissionsTask);

            VehicleListView.SelectedIndex = VehicleListView.Items.Count - 1;
        }
    }
}
