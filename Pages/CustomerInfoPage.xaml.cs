using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.Controls;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Pages
{
    /// <summary>
    /// Interaction logic for CustomerInfoPage.xaml
    /// </summary>
    public partial class CustomerInfoPage : UserControl, IPropertyPanel
    {
        private CustomerAdminObject _customer;
        private ClosableTab _parentTabControl; 
        public CustomerInfoPage()
            : this(null)
        {
            //InitializeComponent();
        }

        public CustomerInfoPage(CustomerAdminObject customer)
        {
            InitializeComponent();
            _customer = customer ?? CreateNewDefaultVehicleObject();

            //propertyPanels.Add(BasicVehicleControlPropertyPanel);
            //propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);

            LoadPanel(_customer);
        }

        public void LoadPanel(IAdminObject item)
        {
            CustomerInfoControl.LoadPanel(item);
        }

        public void SetParentTabControl(ClosableTab parentTabControl)
        {
            _parentTabControl = parentTabControl;
            _parentTabControl.TabClosing += _parentTabControl_TabClosing;
        }

        bool _parentTabControl_TabClosing()
        {
            MessageBoxResult result;
            if (_customer.GetValue(PropertyId.IsDeleted) == true.ToString())
            {
                return false;
            }

            bool requirementsMet = VerifyMinimumRequirements();
            if (!requirementsMet)
            {
                result = MessageBox.Show(Strings.PAGES_CUSTOMERINFOPAGE_MISSINGINFO_CONFIRMDELETE,
                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    return true;
                }
                else
                {
                    DeleteCustomer();
                }
                return false;
            }

            CustomerAdminObject origionalCustomer = new CustomerAdminObject(_customer.ObjectId);

            bool same = _customer.Equals(origionalCustomer);

            if (same)
                return false;

            if (_customer.FileVersion == "0")
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
                bool successfulSave = _customer.Save(this);
                if (!successfulSave)
                {
                    MessageBox.Show(Strings.PAGES_CUSTOMERINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                    Strings.ERROR, MessageBoxButton.OK);
                    return true;
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return true;
            }

            return false;
        }

        private bool VerifyMinimumRequirements()
        {
            string property;
            property = _customer.GetValue(PropertyId.FirstName);
            if (property == null || string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
                return false;

            property = _customer.GetValue(PropertyId.LastName);
            if (property == null || string.IsNullOrEmpty(property) || string.IsNullOrWhiteSpace(property))
                return false;

            return true;
        }

        private CustomerAdminObject CreateNewDefaultVehicleObject()
        {
            int lastId = GetNextFolderId();

            DirectoryInfo newDirectory = Directory.CreateDirectory(Settings.CustomerPath + "\\" + (lastId + 1));
            FileStream newfile = File.Create(newDirectory.FullName + "\\" + Settings.CustomerInfoFileName);
            string fileName = newfile.Name;
            newfile.Close();

            File.WriteAllText(fileName, Settings.CustomerInfoDefaultFileText);

            return new CustomerAdminObject(fileName);

            //TODO: add this vehicle to some cache.
        }

        private int GetNextFolderId()
        {
            int largestId = 0;
            List<string> directories = Directory.GetDirectories(Settings.CustomerPath).ToList();

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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool successfulSave = _customer.Save(this);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            CustomerCache cache = _customer.Cache as CustomerCache;
            if (cache != null)
            {
                cache.Remove(_customer);
                _customer = new CustomerAdminObject(_customer.ObjectId);
                cache.Add(_customer);
                _customer.Cache = cache;
            }

            LoadPanel(_customer);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(Strings.PAGES_CUSTOMERINFOPAGE_CONFIRMDELETE,
    Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING_TITLE, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.No)
                return;

            _customer.SetValue(PropertyId.IsDeleted, true);
            bool successfulSave = _customer.Save(this);
            if (successfulSave)
            {
                _parentTabControl.button_close_Click(null, null);
            }
            else
            {
                MessageBox.Show(Strings.PAGES_CUSTOMERINFOPAGE_UNABLETODELETE, Strings.ERROR, MessageBoxButton.OK);
            }

            //_vehicle.SetValue(PropertyId.IsDeleted, true);
            //bool successfulSave = _vehicle.Save(this);
            //if (successfulSave)
            //{
            //    _parentTabControl.button_close_Click(null, null);
            //}
            //else
            //{
            //    MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_UNABLETODELETE,
            //        Strings.ERROR, MessageBoxButton.OK);
            //}
        }

        private void DeleteCustomer()
        {
            _customer.SetValue(PropertyId.IsDeleted, true);
            bool successfulSave = _customer.Save(this);
            if (successfulSave)
            {
                try
                {
                    _parentTabControl.button_close_Click(null, null);
                }
                catch (Exception e) { }
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
    }
}
