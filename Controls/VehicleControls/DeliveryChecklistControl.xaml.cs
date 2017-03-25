using CarDepot.VehicleStore;
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

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for DeliveryChecklistControl.xaml
    /// </summary>
    public partial class DeliveryChecklistControl : UserControl, IPropertyPanel
    {
        VehicleAdminObject _vehicle = null;
        public DeliveryChecklistControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            _vehicle = null;
            _vehicle = item as VehicleAdminObject;

            LoadAllChildren(DeliveryInfoGrid, item);
            lblSoldBy.Content = _vehicle.GetValue(PropertyId.SaleSoldBy);

            ApplyPermissions();
        }

        private void ApplyPermissions()
        {
            if (CacheManager.ActiveUser.Category != null &&
                (CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Mechanic) || CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Administrator)))
            {
                cbMechanical.IsEnabled = true;
            }
            else
            {
                cbMechanical.IsEnabled = false;
            }

            if (CacheManager.ActiveUser.Category != null && 
                (CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Detail) || CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Administrator)))
            {
                cbDetailing.IsEnabled = true;
            }
            else
            {
                cbDetailing.IsEnabled = false;
            }

            if (CacheManager.ActiveUser.Category != null && 
                (CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Documentation) || CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Administrator)))
            {
                cbMinistry.IsEnabled = true;
            }
            else
            {
                cbMinistry.IsEnabled = false;
            }

            if (CacheManager.ActiveUser.Category != null && 
                (CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Sales) ||
                CacheManager.ActiveUser.Category.Contains(UserAdminObject.UserCategory.Administrator)))
            {
                cbDelivered.IsEnabled = true;
            }
            else
            {
                cbDelivered.IsEnabled = false;
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

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        private void cbMechanical_Checked(object sender, RoutedEventArgs e)
        {
            MechanicalCheckRectangle.Fill = Brushes.LightGreen;   
        }

        private void cbMechanical_Unchecked(object sender, RoutedEventArgs e)
        {
            MechanicalCheckRectangle.Fill = Brushes.Red;
        }

        private void cbDetailing_Checked(object sender, RoutedEventArgs e)
        {
            DetailingCheckRectangle.Fill = Brushes.LightGreen;
        }

        private void cbDetailing_Unchecked(object sender, RoutedEventArgs e)
        {
            DetailingCheckRectangle.Fill = Brushes.Red;
        }

        private void cbMinistry_Unchecked(object sender, RoutedEventArgs e)
        {
            MinistryCheckRectangle.Fill = Brushes.Red;
        }

        private void cbMinistry_Checked(object sender, RoutedEventArgs e)
        {
            MinistryCheckRectangle.Fill = Brushes.LightGreen;
        }

        private void cbDelivered_Checked(object sender, RoutedEventArgs e)
        {
            DeliveredRectangle.Fill = Brushes.LightGreen;
        }

        private void cbDelivered_Unchecked(object sender, RoutedEventArgs e)
        {
            DeliveredRectangle.Fill = Brushes.Red;
        }
    }
}
