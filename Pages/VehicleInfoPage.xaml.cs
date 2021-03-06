﻿using System;
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

  

        public VehicleInfoWindow(VehicleAdminObject vehicle, VehicleInfoWindowTabs startTab)
        {
            if (!File.Exists(Settings.DefaultVehicleImagePath))
            {
                MessageBox.Show("ERROR: Experiencing connectivity issues can't load vehicle");
                return;
            }

            InitializeComponent();
            _vehicle = vehicle ?? Utilities.CreateNewDefaultVehicleObject();

            propertyPanels.Add(BasicVehicleControlPropertyPanel);
            propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);
            propertyPanels.Add(PurchaseInfoControlPropertyPanel);
            propertyPanels.Add(SaleInfoControlPropertyPanel);
            propertyPanels.Add(SafetyInspectionControlPropertyPanel);
            propertyPanels.Add(DeliveryCheckListControlPropertyPanel);

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
            if (_vehicle.GetValue(PropertyId.VinNumber).ToString().Length != 17)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_VINLENGTHCOUNT, Strings.WARNING, MessageBoxButton.OK);
            }

            bool successfulSave = _vehicle.Save(this);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                LoadPanel(_vehicle);
                return;
            }
            VehicleCache cache = _vehicle.Cache as VehicleCache;
            if (cache != null)
            {
                cache.Remove(_vehicle);
                _vehicle = new VehicleAdminObject(_vehicle.ObjectId);
                cache.Add(_vehicle);
                _vehicle.Cache = cache;
            }

            if (CacheManager.AllVehicleCache != null)
            {
                CacheManager.AllVehicleCache.RemoveItem(_vehicle.ObjectId);
                VehicleAdminObject temp = new VehicleAdminObject(_vehicle.ObjectId);
                temp.Cache = CacheManager.AllVehicleCache;
                CacheManager.AllVehicleCache.Add(temp);
            }

            LoadPanel(_vehicle);
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = Microsoft.VisualBasic.Interaction.InputBox(Strings.VEHICLEINFOPAGE_IMPORTURL_PROMPT,
                    Strings.VEHICLEINFOPAGE_IMPORTURL_TITLE, Strings.VEHICLEINFOPAGE_IMPORTURL_DATA, -1, -1);
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
                    MessageBox.Show(Strings.VEHICLEINFOPAGE_INVALID_URL, Strings.VEHICLEINFOPAGE_IMPORTURL_TITLE,
                        MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("BtnImport: " + ex.Message);
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

    }
}
