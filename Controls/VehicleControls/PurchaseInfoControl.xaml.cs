using System;
using System.Collections.Generic;
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
using Microsoft.VisualBasic.CompilerServices;

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for PurchaseInfoControl.xaml
    /// </summary>
    public partial class PurchaseInfoControl : UserControl, IPropertyPanel
    {
        public enum WellKnownVendors
        {
            Adesa,
            Manheim,
            PrivatePurchase,
            TradeIn
        }

        VehicleAdminObject _vehicle = null;
        public PurchaseInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            _vehicle = item as VehicleAdminObject;

            LoadAllChildren(PurchaseInfoGrid, item);
            addtionalContentControl.ListChanged += addtionalContentControl_ListChanged;

            string foundVendor = _vehicle.GetValue(PropertyId.Vendor);
            WellKnownVendors vehicleVendor = WellKnownVendors.Adesa;
            bool foundVehicleVendor = false;
            if (!string.IsNullOrEmpty(foundVendor))
            {
                foundVehicleVendor = true;
                vehicleVendor = (WellKnownVendors)Enum.Parse(typeof(WellKnownVendors), foundVendor, true);
            }

            int foundIndex = -1;

            cmbVendor.Items.Add("");
            foreach (WellKnownVendors vendor in (WellKnownVendors[])Enum.GetValues(typeof(WellKnownVendors)))
            {
                cmbVendor.Items.Add(vendor.ToString());
                if (foundVehicleVendor && vendor == vehicleVendor)
                {
                    foundIndex = cmbVendor.Items.Count - 1;
                }
            }

            if (foundIndex != -1)
            {
                cmbVendor.SelectedIndex = foundIndex;
            }

            CalculateTasksCost();
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
                _vehicle.PurchaseAssociatedFiles = files;
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

        private void CalculateTasksCost()
        {
            double totalCost = 0;
            foreach (VehicleTask vehicleTask in _vehicle.VehicleTasks)
            {
                double cost = 0;
                if (Utilities.StringToDouble(vehicleTask.Cost, out cost))
                {
                    totalCost += cost;
                }
            }

            string totalCostString = "$" + totalCost.ToString("F");
            txtTasksCost.Text = totalCostString;
        }

        private void CalculateHst_TextChanged(object sender, TextChangedEventArgs e)
        {
            double purchasePrice = 0;
            double buyerFee = 0;
            double otherCost = 0;
            double tasksCost = 0;
            double warrantyCost = 0;

            Utilities.StringToDouble(TxtPurchasePrice.Text, out purchasePrice);
            Utilities.StringToDouble(TxtBuyerFee.Text, out buyerFee);
            Utilities.StringToDouble(TxtOtherCosts.Text, out otherCost);
            Utilities.StringToDouble(txtTasksCost.Text, out tasksCost);
            Utilities.StringToDouble(TxtWarrantyCosts.Text, out warrantyCost);

            lblSubtotal.Content = "Subtotal: " +
                                  (purchasePrice + buyerFee + otherCost + tasksCost + warrantyCost).ToString("F");

            double hst = (purchasePrice + buyerFee + otherCost + tasksCost + warrantyCost) * Settings.HST;

            TxtPurchaseHst.Text = "$" + hst.ToString("F");
        }

        private void TxtPurchaseHst_TextChanged(object sender, TextChangedEventArgs e)
        {
            double purchasePrice = 0;
            double buyerFee = 0;
            double otherCost = 0;
            double tasksCost = 0;
            double warrantyCost = 0;
            double hst = 0;

            Utilities.StringToDouble(TxtPurchasePrice.Text, out purchasePrice);
            Utilities.StringToDouble(TxtBuyerFee.Text, out buyerFee);
            Utilities.StringToDouble(TxtOtherCosts.Text, out otherCost);
            Utilities.StringToDouble(TxtPurchaseHst.Text, out hst);
            Utilities.StringToDouble(txtTasksCost.Text, out tasksCost);
            Utilities.StringToDouble(TxtWarrantyCosts.Text, out warrantyCost);

            double totalCost = purchasePrice + buyerFee + otherCost + tasksCost + warrantyCost + hst;
            TxtPurchaseTotal.Text = "$" + totalCost.ToString("F");

        }

    }
}
