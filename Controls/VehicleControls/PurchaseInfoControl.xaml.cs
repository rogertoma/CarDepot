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

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for PurchaseInfoControl.xaml
    /// </summary>
    public partial class PurchaseInfoControl : UserControl, IPropertyPanel
    {
        public PurchaseInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            LoadAllChildren(PurchaseInfoGrid, item);
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

        private void CalculateHst_TextChanged(object sender, TextChangedEventArgs e)
        {
            double purchasePrice = 0;
            double buyerFee = 0;
            double otherCost = 0;

            Utilities.StringToDouble(TxtPurchasePrice.Text, out purchasePrice);
            Utilities.StringToDouble(TxtBuyerFee.Text, out buyerFee);
            Utilities.StringToDouble(TxtOtherCosts.Text, out otherCost);

            double hst = (purchasePrice + buyerFee + otherCost) * Settings.HST;

            TxtPurchaseHst.Text = "$" + hst.ToString("F");
        }

        private void TxtPurchaseHst_TextChanged(object sender, TextChangedEventArgs e)
        {
            double purchasePrice = 0;
            double buyerFee = 0;
            double otherCost = 0;
            double hst = 0;

            Utilities.StringToDouble(TxtPurchasePrice.Text, out purchasePrice);
            Utilities.StringToDouble(TxtBuyerFee.Text, out buyerFee);
            Utilities.StringToDouble(TxtOtherCosts.Text, out otherCost);
            Utilities.StringToDouble(TxtPurchaseHst.Text, out hst);

            double totalCost = purchasePrice + buyerFee + otherCost + hst;
            TxtPurchaseTotal.Text = "$" + totalCost.ToString("F");

        }

    }
}
