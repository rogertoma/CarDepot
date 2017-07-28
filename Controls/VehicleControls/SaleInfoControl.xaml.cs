using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using CarDepot.Pages;
using Panel = System.Windows.Controls.Panel;
using PrintDialog = System.Windows.Controls.PrintDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for SaleInfoControl.xaml
    /// </summary>
    public partial class SaleInfoControl : UserControl, IPropertyPanel
    {
        public enum DepositTypes
        {
            Cash,
            Credit,
            Debit,
            Cheque
        }

        public enum BrandTypes
        {
            None,
            Rebuilt,
            Salvage
        }

        public enum Certified
        {
            Yes,
            No
        }

        VehicleAdminObject _vehicle = null;

        public SaleInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            _vehicle = null;
            TxtCustomerId.Text = "";
            _vehicle = item as VehicleAdminObject;

            ClearFormState();

            LoadAllChildren(SaleInfoGrid, item);
            LoadAllChildren(WarrantyGrid, item);
            LoadAllChildren(TradeInGrid, item);

            addtionalContentControl.ListChanged += addtionalContentControl_ListChanged;

            LoadComboBoxes();

            if (!CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.UpdateSaleTaxPercentage))
            {
                txtSalesTaxPercentage.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(txtSalesTaxPercentage.Text))
            {
                txtSalesTaxPercentage.Text = Settings.HST.ToString(CultureInfo.InvariantCulture);
            }

        }

        private void ClearFormState()
        {
            cmbSaleDepositeType.Items.Clear();
            cmbBrand.Items.Clear();
            cmbSafetyCertificate.Items.Clear();
            cmbSoldBy.Items.Clear();
            cmbSaleManager.Items.Clear();
        }

        private void LoadComboBoxes()
        {
            #region Deposit Type
            string foundDepositType = _vehicle.GetValue(PropertyId.SaleDepositType);
            DepositTypes depositType = DepositTypes.Cash;
            bool foundVehicleDepositType = false;
            if (!string.IsNullOrEmpty(foundDepositType))
            {
                foundVehicleDepositType = true;
                depositType = (DepositTypes)Enum.Parse(typeof(DepositTypes), foundDepositType, true);
            }

            int foundIndex = -1;

            cmbSaleDepositeType.Items.Add("");
            foreach (DepositTypes vendor in (DepositTypes[])Enum.GetValues(typeof(DepositTypes)))
            {
                cmbSaleDepositeType.Items.Add(vendor.ToString());
                if (foundVehicleDepositType && vendor == depositType)
                {
                    foundIndex = cmbSaleDepositeType.Items.Count - 1;
                }
            }

            if (foundIndex != -1)
            {
                cmbSaleDepositeType.SelectedIndex = foundIndex;
            }
            #endregion

            #region BrandType
            string foundBrandType = _vehicle.GetValue(PropertyId.SaleBrand);
            BrandTypes brandType = BrandTypes.None;
            bool foundVehicleBrandType = false;
            if (!string.IsNullOrEmpty(foundBrandType))
            {
                foundVehicleBrandType = true;
                brandType = (BrandTypes)Enum.Parse(typeof(BrandTypes), foundBrandType, true);
            }

            foundIndex = -1;

            cmbBrand.Items.Add("");
            foreach (BrandTypes vendor in (BrandTypes[])Enum.GetValues(typeof(BrandTypes)))
            {
                cmbBrand.Items.Add(vendor.ToString());
                if (foundVehicleBrandType && vendor == brandType)
                {
                    foundIndex = cmbBrand.Items.Count - 1;
                }
            }

            if (foundIndex != -1)
            {
                cmbBrand.SelectedIndex = foundIndex;
            }
            #endregion

            #region Certified
            string foundSafetyCertificateType = _vehicle.GetValue(PropertyId.SaleSafetyCertificate);
            Certified SafetyCertificateType = Certified.No;
            bool foundVehicleSafetyCertificateType = false;
            if (!string.IsNullOrEmpty(foundSafetyCertificateType))
            {
                foundVehicleSafetyCertificateType = true;
                SafetyCertificateType = (Certified)Enum.Parse(typeof(Certified), foundSafetyCertificateType, true);
            }

            foundIndex = -1;

            cmbSafetyCertificate.Items.Add("");
            foreach (Certified vendor in (Certified[])Enum.GetValues(typeof(Certified)))
            {
                cmbSafetyCertificate.Items.Add(vendor.ToString());
                if (foundVehicleSafetyCertificateType && vendor == SafetyCertificateType)
                {
                    foundIndex = cmbSafetyCertificate.Items.Count - 1;
                }
            }

            if (foundIndex != -1)
            {
                cmbSafetyCertificate.SelectedIndex = foundIndex;
            }
            #endregion

            #region SoldBy

            int selectedIndex = -1;
            string soldBy = _vehicle.GetValue(PropertyId.SaleSoldBy);

            System.Windows.Controls.Label lblEmptyRow = new System.Windows.Controls.Label { Content = string.Empty };
            cmbSoldBy.Items.Add(lblEmptyRow);

            if (CacheManager.ActiveUser.Name == "Roger Toma")
            {
                foreach (UserAdminObject user in CacheManager.UserCache)
                {
                    System.Windows.Controls.Label lblRow = new System.Windows.Controls.Label { Content = user.Name };
                    cmbSoldBy.Items.Add(lblRow);
                    if (user.Name == soldBy)
                        selectedIndex = cmbSoldBy.Items.Count - 1;
                }
            }
            else
            {
                System.Windows.Controls.Label lblRow = new System.Windows.Controls.Label { Content = CacheManager.ActiveUser.Name };
                cmbSoldBy.Items.Add(lblRow);
                if (CacheManager.ActiveUser.Name == soldBy)
                    selectedIndex = cmbSoldBy.Items.Count - 1;
            }
            
            if (selectedIndex == -1 && !string.IsNullOrEmpty(soldBy))
            {
                System.Windows.Controls.Label lblRow = new System.Windows.Controls.Label { Content = soldBy };
                cmbSoldBy.Items.Add(lblRow);
                selectedIndex = cmbSoldBy.Items.Count - 1;
            }

            cmbSoldBy.SelectedIndex = selectedIndex;

            if (!string.IsNullOrEmpty(soldBy) && CacheManager.ActiveUser.Name != "Roger Toma")
            {
                cmbSoldBy.IsEnabled = false;
            }
            if (CacheManager.ActiveUser.Name == soldBy)
            {
                cmbSoldBy.IsEnabled = true;
            }

            #endregion

            #region Manager

            selectedIndex = -1;
            string saleManager = _vehicle.GetValue(PropertyId.SaleManager);

            System.Windows.Controls.Label lblSaleManagerEmptyRow = new System.Windows.Controls.Label { Content = string.Empty };
            cmbSaleManager.Items.Add(lblSaleManagerEmptyRow);

            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                if (!user.Permissions.Contains(UserAdminObject.PermissionTypes.IsManager))
                    continue;

                System.Windows.Controls.Label lblRow = new System.Windows.Controls.Label { Content = user.Name };
                cmbSaleManager.Items.Add(lblRow);
                if (user.Name == saleManager)
                    selectedIndex = cmbSaleManager.Items.Count - 1;
            }

            if (selectedIndex != -1)
                cmbSaleManager.SelectedIndex = selectedIndex;

            if (selectedIndex != -1 && CacheManager.ActiveUser.Name != "Roger Toma")
            {
                cmbSaleManager.IsEnabled = false;
            }

            if (string.IsNullOrEmpty(soldBy))
            {
                cmbSaleManager.IsEnabled = true;
            }

            #endregion
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            throw new NotImplementedException();
        }

        void addtionalContentControl_ListChanged(List<string[]> files)
        {
            if (_vehicle != null)
            {
                _vehicle.SaleAssociatedFiles = files;
            }
        }

        public void LoadAllChildren(Panel panel, IAdminObject item)
        {
            foreach (var child in panel.Children)
            {
                IPropertyPanel propPanel = child as IPropertyPanel;
                if (propPanel != null)
                {
                    propPanel.LoadPanel(item);
                }

                Panel isPanel = child as Panel;
                if (isPanel != null)
                    LoadAllChildren(isPanel, item);

            }
        }

        private void TxtCustomerId_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dictionary<CustomerCacheSearchKey, string> searchParam = new Dictionary<CustomerCacheSearchKey, string>();
            CustomerCache cache = null;
            if (!string.IsNullOrEmpty(TxtCustomerId.Text))
            {
                searchParam.Add(CustomerCacheSearchKey.Id, TxtCustomerId.Text);
                cache = new CustomerCache(searchParam);
            }

            if (_vehicle != null)
            {
                _vehicle.SetValue(PropertyId.SaleCustomerId, TxtCustomerId.Text);
            }
            if (cache != null && cache.Count > 0)
            {
                CustomerAdminObject customer = cache[0];
                lblCustomerFound.Content = customer.FirstName + " " + customer.LastName;
            }
            else
            {
                lblCustomerFound.Content = "";
            }

        }

        private void CalculateTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            double salePrice = 0,
                warranty = 0,
                financeFee = 0,
                accessorys = 0,
                tradeIn = 0,
                licenseFee = 0,
                payoutLienOnTrade = 0,
                deposit = 0,
                bankAdminFee = 0,
                lienRegistrationFee = 0;

            Utilities.StringToDouble(TxtSalePrice.Text, out salePrice);
            // TODO: Calculate Warranty
            Utilities.StringToDouble(txtWarrantyCost.Text, out warranty);
            Utilities.StringToDouble(TxtFinanceCost.Text, out financeFee);
            Utilities.StringToDouble(TxtAccessoryCost.Text, out accessorys);

            double subTotal1 = salePrice + warranty + financeFee + accessorys;
            txtSubTotal.Text = "$" + subTotal1.ToString("F");

            Utilities.StringToDouble(TxtTradeInCost.Text, out tradeIn);
            double netDifference = subTotal1 - tradeIn;
            txtNetDifference.Text = "$" + netDifference.ToString("F");

            double hstPercentage = 0;
            if (!Utilities.StringToDouble(_vehicle.GetValue(PropertyId.SaleTaxPercentage), out hstPercentage))
            {
                hstPercentage = Settings.HST;
            }

            double hst = netDifference * hstPercentage;
            txtSalesTax.Text = "$" + hst.ToString("F");

            Utilities.StringToDouble(TxtLicensingFee.Text, out licenseFee);
            Utilities.StringToDouble(TxtPayoutLienOnTradeIn.Text, out payoutLienOnTrade);

            double subTotal2 = netDifference + hst + licenseFee + payoutLienOnTrade;
            txtSubTotal2.Text = "$" + subTotal2.ToString("F");

            Utilities.StringToDouble(TxtCustomerPayment.Text, out deposit);
            Utilities.StringToDouble(TxtBankAdminFee.Text, out bankAdminFee);
            Utilities.StringToDouble(TxtLienRegistrationFee.Text, out lienRegistrationFee);

            double totalDue = subTotal2 - deposit + bankAdminFee + lienRegistrationFee;
            txtTotalBalanceDue.Text = "$" + totalDue.ToString("F");
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("Did you remember to update the mileage?", Strings.WARNING,
                System.Windows.Forms.MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                double tradeInAmount = 0;

                if (string.IsNullOrEmpty(_vehicle.GetValue(PropertyId.SaleSoldBy)))
                {
                    System.Windows.Forms.MessageBox.Show("No sales person selected, can not finalize this sale.", Strings.ERROR);
                    return;
                }

                if (string.IsNullOrEmpty(_vehicle.GetValue(PropertyId.SaleManager)))
                {
                    System.Windows.Forms.MessageBox.Show("No manager selected, can not finalize this sale.", Strings.ERROR);
                    return;
                }

                if (Utilities.StringToDouble(TxtTradeInCost.Text, out tradeInAmount) && tradeInAmount != 0)
                {
                    if (string.IsNullOrEmpty(txtTradeInYear.Text))
                    {
                        System.Windows.Forms.MessageBox.Show("You did not specify the trade in Year", Strings.ERROR);
                        return;
                    }

                    if (string.IsNullOrEmpty(txtTradeInMake.Text))
                    {
                        System.Windows.Forms.MessageBox.Show("You did not specify the trade in Make", Strings.ERROR);
                        return;
                    }

                    if (string.IsNullOrEmpty(txtTradeInModel.Text))
                    {
                        System.Windows.Forms.MessageBox.Show("You did not specify the trade in Model", Strings.ERROR);
                        return;
                    }

                    if (string.IsNullOrEmpty(txtTradeInMileage.Text))
                    {
                        System.Windows.Forms.MessageBox.Show("You did not specify the trade in Mileage", Strings.ERROR);
                        return;
                    }

                    if (string.IsNullOrEmpty(txtTradeInVIN.Text))
                    {
                        System.Windows.Forms.MessageBox.Show("You did not specify the trade in VIN", Strings.ERROR);
                        return;
                    }
                }
            }

            if (result == DialogResult.Yes)
            {
                PrintInvoice printCurrentCar = new PrintInvoice(_vehicle, sender, e);

                //if (!string.IsNullOrEmpty(_vehicle.GetValue(PropertyId.SaleTradeInVIN)))
                //{
                //    PrintAppraisalForm printAppraisalForm = new PrintAppraisalForm(_vehicle, sender, e);
                //}
            }
        }

        private void PayoutLien_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal_TextChanged(sender, e);

            double payoutLien = 0;

            if (!Utilities.StringToDouble(_vehicle.GetValue(PropertyId.SalePayoutLienOnTradeIn), out payoutLien) || Math.Abs(payoutLien) == 0.0)
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.PAYOUTLIENTASK))
                    {
                        _vehicle.VehicleTasks.Remove(vehicleTask);
                        break;
                    }
                }
            }
            else
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.PAYOUTLIENTASK))
                    {
                        return;
                    }
                }

                VehicleTask payoutLienTask = new VehicleTask();
                payoutLienTask.Id = Strings.PAYOUTLIENTASK;
                payoutLienTask.TaskVehicleId = _vehicle.Id;
                payoutLienTask.CreatedDate = DateTime.Today.Date.ToString("d");
                payoutLienTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                payoutLienTask.AssignedTo = "Reyad Toma";
                payoutLienTask.Category = VehicleTask.TaskCategoryTypes.Finance.ToString();
                payoutLienTask.CreatedBy = CacheManager.ActiveUser.Name;
                payoutLienTask.Priority = VehicleTask.TaskPriority.Priority0.ToString();
                _vehicle.VehicleTasks.Add(payoutLienTask);    
            }

            _vehicle.SetValue(PropertyId.Tasks, _vehicle.VehicleTasks);
        }

        private void LienRegistrationFee_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal_TextChanged(sender, e);

            double leinRegistrationFee = 0;

            if (!Utilities.StringToDouble(TxtLienRegistrationFee.Text, out leinRegistrationFee) || Math.Abs(leinRegistrationFee) == 0.0)
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.VERIFYFINANCEDEPOSIT))
                    {
                        _vehicle.VehicleTasks.Remove(vehicleTask);
                        break;
                    }
                }
            }
            else
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.VERIFYFINANCEDEPOSIT))
                    {
                        return;
                    }
                }

                VehicleTask verifyFinanceDepositTask = new VehicleTask();
                verifyFinanceDepositTask.Id = Strings.VERIFYFINANCEDEPOSIT;
                verifyFinanceDepositTask.TaskVehicleId = _vehicle.Id;
                verifyFinanceDepositTask.CreatedDate = DateTime.Today.Date.ToString("d");
                verifyFinanceDepositTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                verifyFinanceDepositTask.AssignedTo = "Reyad Toma";
                verifyFinanceDepositTask.Category = VehicleTask.TaskCategoryTypes.Finance.ToString();
                verifyFinanceDepositTask.CreatedBy = CacheManager.ActiveUser.Name;
                _vehicle.VehicleTasks.Add(verifyFinanceDepositTask);
            }

            _vehicle.SetMultiValue(PropertyId.Tasks, _vehicle.VehicleTasks);


        }

        private void SaleDatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            DateTime saleDate = DateTime.Now;

            if (!DateTime.TryParse(_vehicle.GetValue(PropertyId.SaleDate), out saleDate) )
            {
                //foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                //{
                    //if (vehicleTask.Id.Equals(Strings.SOLDCARCLEANINGTASK))
                    //{
                    //    _vehicle.VehicleTasks.Remove(vehicleTask);
                    //    break;
                    //}
                //}

                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.DELETECARFROMADVERTISING))
                    {
                        _vehicle.VehicleTasks.Remove(vehicleTask);
                        break;
                    }
                }

                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.CARDELIVEREDTASK))
                    {
                        _vehicle.VehicleTasks.Remove(vehicleTask);
                        break;
                    }
                }

                /*
                Ministry Paper Work
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.SOLDCARMINISTRYTASK))
                    {
                        _vehicle.VehicleTasks.Remove(vehicleTask);
                        break;
                    }
                }
                */
                _vehicle.SetMultiValue(PropertyId.Tasks, _vehicle.VehicleTasks);
                return;
            }

            #region Delete Oil Cleaning
            foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
            {
                if (vehicleTask.Id.Equals(Strings.NEWCAROILCHANGETASK) && vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())
                {
                    _vehicle.VehicleTasks.Remove(vehicleTask);
                    break;
                }
            }
            #endregion

            #region Sold Car Ministry task
            /*
            bool soldCarMinistryTaskFound = false;
            foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
            {
                if (vehicleTask.Id.Equals(Strings.SOLDCARMINISTRYTASK))
                {
                    soldCarMinistryTaskFound = true;
                    break;
                }
            }
            
            // Only 
            DateTime June2016 = new DateTime(2016, 06, 01);

            if (!soldCarMinistryTaskFound && saleDate != null && saleDate > June2016)
            {
                VehicleTask vehicleSoldMinistryTask = new VehicleTask();
                vehicleSoldMinistryTask.Id = Strings.SOLDCARMINISTRYTASK;
                vehicleSoldMinistryTask.TaskVehicleId = _vehicle.Id;
                vehicleSoldMinistryTask.Priority = VehicleTask.TaskPriority.Priority0.ToString();
                vehicleSoldMinistryTask.CreatedDate = DateTime.Today.Date.ToString("d");
                vehicleSoldMinistryTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                vehicleSoldMinistryTask.AssignedTo = "Filip Mitrofanov";
                vehicleSoldMinistryTask.Category = VehicleTask.TaskCategoryTypes.Other.ToString();
                vehicleSoldMinistryTask.CreatedBy = CacheManager.ActiveUser.Name;
                _vehicle.VehicleTasks.Add(vehicleSoldMinistryTask);
            }
            */
            #endregion

            #region Car Delivery Task

            bool carDeliveryTaskFound = false;
            foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
            {
                if (vehicleTask.Id.Equals(Strings.CARDELIVEREDTASK))
                {
                    carDeliveryTaskFound = true;
                    break;
                }
            }

            if (!carDeliveryTaskFound)
            {
                VehicleTask vehicleDeliveredTask = new VehicleTask();
                vehicleDeliveredTask.Id = Strings.CARDELIVEREDTASK;
                vehicleDeliveredTask.TaskVehicleId = _vehicle.Id;
                vehicleDeliveredTask.CreatedDate = DateTime.Today.Date.ToString("d");
                vehicleDeliveredTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                vehicleDeliveredTask.AssignedTo = "Reyad Toma";
                vehicleDeliveredTask.Category = VehicleTask.TaskCategoryTypes.Other.ToString();
                vehicleDeliveredTask.CreatedBy = CacheManager.ActiveUser.Name;
                _vehicle.VehicleTasks.Add(vehicleDeliveredTask);
            }

            #endregion

            #region Delete car advertising task

            bool deleteCarFromAdvertisingTaskFound = false;
            foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
            {
                if (vehicleTask.Id.Equals(Strings.DELETECARFROMADVERTISING))
                {
                    deleteCarFromAdvertisingTaskFound = true;
                    break;
                }
            }

            if (!deleteCarFromAdvertisingTaskFound)
            {
                VehicleTask deleteAdvertisingTask = new VehicleTask();
                deleteAdvertisingTask.Id = Strings.DELETECARFROMADVERTISING;
                deleteAdvertisingTask.TaskVehicleId = _vehicle.Id;
                deleteAdvertisingTask.CreatedDate = DateTime.Today.Date.ToString("d");
                deleteAdvertisingTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                deleteAdvertisingTask.AssignedTo = "Mathew Toma";
                deleteAdvertisingTask.Category = VehicleTask.TaskCategoryTypes.Detail.ToString();
                deleteAdvertisingTask.CreatedBy = CacheManager.ActiveUser.Name;
                _vehicle.VehicleTasks.Add(deleteAdvertisingTask);
            }

            #endregion

            _vehicle.SetMultiValue(PropertyId.Tasks, _vehicle.VehicleTasks);
        }

        private void txtWarrantyCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal_TextChanged(sender, e);
            double warranty = 0;

            if (Utilities.StringToDouble(txtWarrantyCost.Text, out warranty) && !warranty.Equals(0))
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.PURCHASEWARRANTYTASK))
                    {
                        return;
                    }
                }

                VehicleTask purchaseWarrantyTask = new VehicleTask();
                purchaseWarrantyTask.Id = Strings.PURCHASEWARRANTYTASK;
                purchaseWarrantyTask.TaskVehicleId = _vehicle.Id;
                purchaseWarrantyTask.CreatedDate = DateTime.Today.Date.ToString("d");
                purchaseWarrantyTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                purchaseWarrantyTask.AssignedTo = "Mathew Toma";
                purchaseWarrantyTask.Category = VehicleTask.TaskCategoryTypes.Detail.ToString();
                purchaseWarrantyTask.CreatedBy = CacheManager.ActiveUser.Name;
                _vehicle.VehicleTasks.Add(purchaseWarrantyTask);
            }
            else
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.PURCHASEWARRANTYTASK))
                    {
                        if (vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())
                            _vehicle.VehicleTasks.Remove(vehicleTask);

                        break;
                    }
                }
            }

            _vehicle.SetMultiValue(PropertyId.Tasks, _vehicle.VehicleTasks);
        }

        private void lblCustomerFound_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Dictionary<CustomerCacheSearchKey, string> searchParam = new Dictionary<CustomerCacheSearchKey, string>();
            CustomerCache cache = null;
            CustomerAdminObject customer = null;
            if (!string.IsNullOrEmpty(TxtCustomerId.Text))
            {
                searchParam.Add(CustomerCacheSearchKey.Id, TxtCustomerId.Text);
                cache = new CustomerCache(searchParam);
            }

            if (cache != null && cache.Count > 0)
            {
                customer = cache[0];
            }
            else
            {
                return;
            }

            CustomerInfoPage customerInfo = new CustomerInfoPage(customer);

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.CustomerTabColor;
            customerInfo.SetParentTabControl(tabItem);
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = "Customer: " + customer.LastName + ", " + customer.FirstName;
            tabItem.Content = customerInfo;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void TxtTradeInCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal_TextChanged(sender, e);

            double tradeIn = 0;

            if (Utilities.StringToDouble(TxtTradeInCost.Text, out tradeIn) && !tradeIn.Equals(0))
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.CREATETRADEINFORMTASK))
                    {
                        return;
                    }
                }

                VehicleTask createTradeInTask = new VehicleTask();
                string taskText = string.Format(Strings.CREATETRADEINFORMTASK, txtTradeInModel.Text, txtTradeInVIN.Text);
                createTradeInTask.Id = taskText;
                createTradeInTask.TaskVehicleId = _vehicle.Id;
                createTradeInTask.CreatedDate = DateTime.Today.Date.ToString("d");
                createTradeInTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                createTradeInTask.AssignedTo = CacheManager.ActiveUser.Name.ToString(CultureInfo.InvariantCulture);
                createTradeInTask.Category = VehicleTask.TaskCategoryTypes.Documentation.ToString();
                createTradeInTask.CreatedBy = CacheManager.ActiveUser.Name;
                createTradeInTask.Priority = VehicleTask.TaskPriority.Priority1.ToString();
                _vehicle.VehicleTasks.Add(createTradeInTask);
            }
            else
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.CREATETRADEINFORMTASK))
                    {
                        if (vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())
                            _vehicle.VehicleTasks.Remove(vehicleTask);

                        break;
                    }
                }
            }

            _vehicle.SetMultiValue(PropertyId.Tasks, _vehicle.VehicleTasks);
        }

        private void cmbSafetyCertificate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double tradeIn = 0;

            if (cmbSafetyCertificate.SelectedItem == null)
                return;
            

            if (cmbSafetyCertificate.SelectedItem.ToString() == Certified.No.ToString())
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.ENSUREVEHICLEHASTRADEINFORM))
                    {
                        return;
                    }
                }

                VehicleTask createTradeInTask = new VehicleTask();
                string taskText = string.Format(Strings.ENSUREVEHICLEHASTRADEINFORM, _vehicle.Model, _vehicle.VinNumber);
                createTradeInTask.Id = taskText;
                createTradeInTask.TaskVehicleId = _vehicle.Id;
                createTradeInTask.CreatedDate = DateTime.Today.Date.ToString("d");
                createTradeInTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
                createTradeInTask.AssignedTo = CacheManager.ActiveUser.Name.ToString(CultureInfo.InvariantCulture);
                createTradeInTask.Category = VehicleTask.TaskCategoryTypes.Documentation.ToString();
                createTradeInTask.CreatedBy = CacheManager.ActiveUser.Name;
                _vehicle.VehicleTasks.Add(createTradeInTask);
            }
            else
            {
                foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
                {
                    if (vehicleTask.Id.Equals(Strings.ENSUREVEHICLEHASTRADEINFORM))
                    {
                        if (vehicleTask.Status != VehicleTask.StatusTypes.Completed.ToString())
                            _vehicle.VehicleTasks.Remove(vehicleTask);

                        break;
                    }
                }
            }

            _vehicle.SetMultiValue(PropertyId.Tasks, _vehicle.VehicleTasks);
        }


        private void btnPrintAppraisalForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintAppraisalForm printCurrentCar = new PrintAppraisalForm(_vehicle, sender, e);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Printing Appraisal Error: " + ex.Message);
            }
        }
    }
}
