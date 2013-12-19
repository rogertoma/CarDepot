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

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for SaleInfoControl.xaml
    /// </summary>
    public partial class SaleInfoControl : UserControl, IPropertyPanel
    {
        public SaleInfoControl()
        {
            InitializeComponent();
            LblBalanceDue.Visibility = Visibility.Collapsed;
            LblBalanceDueTitle.Visibility = Visibility.Collapsed;
        }

        public void LoadPanel(IAdminObject item)
        {
            LoadAllChildren(SaleInfoGrid, item);
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

            if (cache.Count > 0)
            {
                customerInfoControl.LoadPanel(cache[0]);
            }
        }

        private void TxtSalePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            double result;
            Utilities.StringToDouble(TxtSalePrice.Text, out result);

            double hst = result * Settings.HST;
            TxtSaleHst.Text = "$" + hst.ToString("F");

        }

        private void TxtSaleHst_TextChanged(object sender, TextChangedEventArgs e)
        {
            double salePrice, hst, fees;
            Utilities.StringToDouble(TxtSalePrice.Text, out salePrice);
            Utilities.StringToDouble(TxtSaleHst.Text, out hst);
            Utilities.StringToDouble(TxtTotalFees.Text, out fees);

            double totalCost = salePrice + hst + fees;
            TxtTotalDue.Text = "$" + totalCost.ToString("F");
        }

        private void TxtTotalDue_TextChanged(object sender, TextChangedEventArgs e)
        {
            double totalDue = 0;
            Utilities.StringToDouble(TxtTotalDue.Text, out totalDue);

            if (totalDue != 0)
            {
                LblBalanceDue.Visibility = Visibility.Visible;
                LblBalanceDueTitle.Visibility = Visibility.Visible;

                double customerPayment = 0;
                Utilities.StringToDouble(TxtCustomerPayment.Text, out customerPayment);
                totalDue -= customerPayment;
                LblBalanceDue.Content = "$" + totalDue.ToString("F");
            
            }
            else
            {
                LblBalanceDue.Visibility = Visibility.Collapsed;
                LblBalanceDueTitle.Visibility = Visibility.Collapsed;
            }
        }
    }
}
