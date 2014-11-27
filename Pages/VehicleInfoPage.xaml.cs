using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;
using CarDepot.Controls;
using CarDepot.Controls.VehicleControls;
using CarDepot.Pages;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using System.Drawing;
using System.Drawing.Printing;
using MessageBox = System.Windows.MessageBox;

namespace CarDepot
{
    /// <summary>
    /// Interaction logic for VehicleInfoWindow.xaml
    /// </summary>
    public partial class VehicleInfoWindow : UserControl, IPropertyPanel
    {
        public ClosableTab _parentTabControl; 
        private VehicleAdminObject _vehicle;
        //bool isDeleted = false;
        List<IPropertyPanel> propertyPanels = new List<IPropertyPanel>();
        private TabItem removedtTabItemItem = null;

        public enum VehicleInfoWindowTabs
        {
            Default,
            Tasks,
        }

        public VehicleInfoWindow() : this (null, VehicleInfoWindowTabs.Default)
        {
            //InitializeComponent();
        }

        public void SetParentTabControl(ClosableTab parentTabControl)
        {
            _parentTabControl = parentTabControl;
            _parentTabControl.TabClosing += _parentTabControl_TabClosing;
        }

        private void InitializePage()
        {

        }

        private int GetNextFolderId()
        {
            int largestId = 0;
            List<string> directories = Directory.GetDirectories(Settings.VehiclePath).ToList();

            foreach (var directory in directories)
            {
                DirectoryInfo dir = new DirectoryInfo(directory);
                int id = 0;
                if (int.TryParse(dir.Name, out id))
                {
                    if (id > largestId)
                        largestId = id;
                }
            }

            return largestId;
        }

        private VehicleAdminObject CreateNewDefaultVehicleObject()
        {
            int lastId = GetNextFolderId();

            DirectoryInfo newDirectory = Directory.CreateDirectory(Settings.VehiclePath + "\\" + (lastId + 1));
            FileStream newfile = File.Create(newDirectory.FullName + "\\" + Settings.VehicleInfoFileName);
            string fileName = newfile.Name;
            newfile.Close();

            File.WriteAllText(fileName, Settings.VehicleInfoDefaultFileText);

            VehicleAdminObject vehicle = new VehicleAdminObject(fileName);
            vehicle.SetValue(PropertyId.PurchaseDate, ((DateTime)DateTime.Now).ToString("d"));
            vehicle.Save(null);

            return vehicle;

            //TODO: add this vehicle to some cache.
        }

        public VehicleInfoWindow(VehicleAdminObject vehicle, VehicleInfoWindowTabs startTab)
        {
            InitializeComponent();
            _vehicle = vehicle ?? CreateNewDefaultVehicleObject();

            propertyPanels.Add(BasicVehicleControlPropertyPanel);
            propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);
            propertyPanels.Add(PurchaseInfoControlPropertyPanel);
            propertyPanels.Add(SaleInfoControlPropertyPanel);

            ApplyUiMode();
            ApplyActiveUserPermissions();

            LoadPanel(_vehicle);
            CacheManager.ActiveUser.OpenedPages.Add(this);

            if (startTab == VehicleInfoWindowTabs.Tasks)
            {
                foreach (var tabItem in VehicleInfoTabControl.Items)
                {
                    if (tabItem.ToString().ToLower().Contains("tasks"))
                    {
                        ((TabItem) tabItem).IsSelected = true;
                        break;
                    }
                }
            }
        }

        /*
        public VehicleInfoWindow(VehicleAdminObject vehicle): this(vehicle, VehicleInfoWindowTabs.Default)
        {
           
        }
        */

        public void LoadPanel(IAdminObject item)
        {
            foreach (IPropertyPanel propertyPanel in propertyPanels)
            {
                propertyPanel.LoadPanel(item);
            }
        }

        public void ApplyUiMode()
        {
            if (CacheManager.ActiveUser.UiMode == CacheManager.UIMode.Customer)
            {
                foreach (var tabItem in VehicleInfoTabControl.Items)
                {
                    if (tabItem.ToString().ToLower().Contains("purchase"))
                    {
                        removedtTabItemItem = (TabItem) tabItem;
                        VehicleInfoTabControl.Items.Remove(tabItem);
                        break;
                    }
                }
            }
            else if (CacheManager.ActiveUser.UiMode == CacheManager.UIMode.Full)
            {
                if (removedtTabItemItem != null)
                {
                    VehicleInfoTabControl.Items.Insert(1, removedtTabItemItem);
                }
            }

            ApplyActiveUserPermissions();
        }

        public void ApplyActiveUserPermissions()
        {
            if (!CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.PurchaseInformation))
            {
                foreach (var tabItem in VehicleInfoTabControl.Items)
                {
                    if (tabItem.ToString().ToLower().Contains("purchase"))
                    {
                        VehicleInfoTabControl.Items.Remove(tabItem);
                        break;
                    }
                }
            }

            if (!CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.SaleInformation))
            {
                foreach (var tabItem in VehicleInfoTabControl.Items)
                {
                    if (tabItem.ToString().ToLower().Contains("sale"))
                    {
                        VehicleInfoTabControl.Items.Remove(tabItem);
                        break;
                    }
                }
            }

            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.DeleteVehicle))
            {
                BtnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                BtnDelete.Visibility = Visibility.Hidden;
            }
        }

        private bool VerifyMinimumRequirements()
        {
            string property;
            property = _vehicle.GetValue(PropertyId.Year);
            if (property == null || string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
                return false;

            property = _vehicle.GetValue(PropertyId.Make);
            if (property == null || string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
                return false;

            property = _vehicle.GetValue(PropertyId.Model);
            if (property == null || string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
                return false;

            return true;
        }
        
        /// <summary>
        /// Return bool determines of cancellation of closing should occur
        /// True means cancel close
        /// false means continue with close
        /// </summary>
        /// <returns></returns>
        bool _parentTabControl_TabClosing()
        {
            MessageBoxResult result;
            if (_vehicle.GetValue(PropertyId.IsDeleted) == true.ToString()) 
            {
                return false;
            }

            bool requirementsMet = VerifyMinimumRequirements();
            if (!requirementsMet)
            {
                result = MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_MISSINGINFO_CONFIRMDELETE,
                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return true;
                }
                else
                {
                    DeleteVehicle();
                }
                return false;
            }

            VehicleAdminObject origionalVehicle = new VehicleAdminObject(_vehicle.ObjectId);

            bool same = _vehicle.Equals(origionalVehicle);

            if (same)
                return false;

            if (_vehicle.FileVersion == "0")
            {
                result = MessageBoxResult.Yes;
            }
            else
            {
                result = MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING,
                    Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING_TITLE, MessageBoxButton.YesNoCancel);
            }

            if (result == MessageBoxResult.Yes)
            {
                bool successfulSave = _vehicle.Save(this);
                if (!successfulSave)
                {
                    MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                    Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
                    return true;
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return true;
            }

            if (_vehicle != null && _vehicle.Cache != null)
            {
                _vehicle.Cache.ModifyItem(_vehicle);
            }

            return false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool successfulSave = _vehicle.Save(this);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            VehicleCache cache = _vehicle.Cache as VehicleCache;
            if (cache != null)
            {
                cache.Remove(_vehicle);
                //_vehicle = new VehicleAdminObject(_vehicle.ObjectId);
                cache.Add(_vehicle);
                _vehicle.Cache = cache;
            }
            LoadPanel(_vehicle);
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        { 
            string result = Microsoft.VisualBasic.Interaction.InputBox(Strings.VEHICLEINFOPAGE_IMPORTURL_PROMPT, Strings.VEHICLEINFOPAGE_IMPORTURL_TITLE, Strings.VEHICLEINFOPAGE_IMPORTURL_DATA, -1, -1);
            if (result == "")
            {
                return;
            }
            VehicleUrlImport urlImport = new VehicleUrlImport(_vehicle, result);
            if (urlImport.ImportStatus == VehicleImportStatus.PASS)
            {
                urlImport.ApplyVehicleValues();
            }
            else
            {
                MessageBox.Show(Strings.VEHICLEINFOPAGE_INVALID_URL, Strings.VEHICLEINFOPAGE_IMPORTURL_TITLE, MessageBoxButton.OK);
            }

            btnRefresh_Click(null, null);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_CONFIRMDELETE,
                Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING_TITLE, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
                return;

            _vehicle.SetValue(PropertyId.IsDeleted, true);
            bool successfulSave = _vehicle.Save(this);
            if (successfulSave)
            {
                _parentTabControl.button_close_Click(null, null);
            }
            else
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_UNABLETODELETE,
                    Strings.ERROR, MessageBoxButton.OK);
            }

            //this.Close();
        }

        private void DeleteVehicle()
        {
            _vehicle.SetValue(PropertyId.IsDeleted, true);
            bool successfulSave = _vehicle.Save(this);
            if (successfulSave)
            {
                try
                {
                    _parentTabControl.button_close_Click(null, null);
                }
                catch { }
            }
            else
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_UNABLETODELETE,
                    Strings.ERROR, MessageBoxButton.OK);
            }

            //this.Close();
            //_vehicle.SetValue(PropertyId.IsDeleted, true);
            //_vehicle.Save();
            //string id = _vehicle.ObjectId;
            //DirectoryInfo directory = new DirectoryInfo(_vehicle.ObjectId);
            //string name = directory.Parent.FullName;
            //Directory.Delete(name, true);
            //isDeleted = true;
        }

        private void BasicVehicleControlPropertyPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();
            dialog.PageRangeSelection = PageRangeSelection.AllPages;
            dialog.UserPageRangeEnabled = true;
            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = dialog.ShowDialog();
            if (print == true)
            {
                PrintCar printCurrentCar = new PrintCar(_vehicle, sender, e);
            }
            return;
        }
    }
}
