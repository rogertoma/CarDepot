using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            LoadAllChildren(SaleInfoGrid, item);
            LoadAllChildren(WarrantyGrid, item);
            LoadAllChildren(TradeInGrid, item);

            addtionalContentControl.ListChanged += addtionalContentControl_ListChanged;

            LoadComboBoxes();
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
            searchParam.Add(CustomerCacheSearchKey.Id, TxtCustomerId.Text);
            CustomerCache cache = new CustomerCache(searchParam);

            if (_vehicle != null)
            {
                _vehicle.SetValue(PropertyId.SaleCustomerId, TxtCustomerId.Text);
            }
            if (cache.Count > 0)
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

            double hst = netDifference*Settings.HST;
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
            System.Windows.Controls.PrintDialog dialog = new System.Windows.Controls.PrintDialog();
            dialog.PageRangeSelection = PageRangeSelection.AllPages;
            dialog.UserPageRangeEnabled = true;
            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = dialog.ShowDialog();
            if (print == true)
            {
                
                PrintInvoice printCurrentCar = new PrintInvoice(_vehicle, dialog, sender, e);
                
            }
        }

    }
}
