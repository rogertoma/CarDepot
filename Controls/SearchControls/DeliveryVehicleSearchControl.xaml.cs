using CarDepot.Pages;
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
using CarDepot.Resources;

namespace CarDepot.Controls.SearchControls
{
    /// <summary>
    /// Interaction logic for DeliveryVehicleSearchControl.xaml
    /// </summary>
    public partial class DeliveryVehicleSearchControl : UserControl
    {

        SortedDictionary<DateTime, VehicleCache> toBeDeliveredMap = null;

        public DeliveryVehicleSearchControl()
        {
            InitializeComponent();

            SearchVehicles();

            AddDates();

        }

        private void SearchVehicles()
        {
            int total = 0;
            int mechanicCompleted, DetailsCompleted, ministryCompleted = 0;

            toBeDeliveredMap = new SortedDictionary<DateTime, VehicleCache>();
            while (CacheManager.AllVehicleCache == null)
            {
                System.Threading.Thread.Sleep(4000);
            }

            foreach (VehicleAdminObject temp in CacheManager.AllVehicleCache)
            {
                DateTime saleDate;
                DateTime featureEnabledDate = new DateTime(2016, 10, 01);
                DateTime.TryParse(temp.GetValue(PropertyId.SaleDate), out saleDate);

                if (saleDate == null || saleDate < featureEnabledDate)
                {
                    continue;
                }

                bool isDelivered = false;
                bool.TryParse(temp.GetValue(PropertyId.DeliveryCheckListDelivered), out isDelivered);
                if (isDelivered)
                {
                    continue;
                }

                DateTime deliveryDate;
                DateTime.TryParse(temp.GetValue(PropertyId.SaleDeliveryDate), out deliveryDate);
                if (deliveryDate == null)
                {
                    string message = "Vehicle: " + temp.Year + " " + temp.Make + " " + temp.Model + ", with ID: " + temp.Id + " does not have a delivery date set.  Please notify Roger";
                    MessageBox.Show(message, Strings.WARNING, MessageBoxButton.OK);
                }

                if (!toBeDeliveredMap.ContainsKey(deliveryDate))
                {
                    total++;
                    toBeDeliveredMap.Add(deliveryDate, new VehicleCache());
                }

                toBeDeliveredMap[deliveryDate].Add(temp);
            }

        }

        private void AddDates()
        {
            int total = 0;
            int mechanicCompleted = 0;
            int detailCompleted = 0;
            int ministryCompleted = 0;

            int i = 0;
            foreach (var kvp in toBeDeliveredMap)
            {
                foreach (VehicleAdminObject vehicle in kvp.Value)
                {
                    total++; 

                    if (vehicle.DeliveryCheckListMechanical.Trim().ToLower() == "true")
                        mechanicCompleted++;
                    if (vehicle.DeliveryCheckListDetailing.Trim().ToLower() == "true")
                        detailCompleted++;
                    if (vehicle.DeliveryCheckListMinistry.Trim().ToLower() == "true")
                        ministryCompleted++;
                }

                DeliveryDateVehicleControl dateItem = new DeliveryDateVehicleControl(kvp.Key.DayOfWeek + ", "+ kvp.Key.ToLongDateString(), kvp.Value);
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                SoldVehicleWithDateGrid.RowDefinitions.Add(rowDefinition);

                Grid.SetRow(dateItem, i);
                SoldVehicleWithDateGrid.Children.Add(dateItem);
                i++;
            }

            lblTotalResult.Content = total;
            lblMechanicResult.Content = mechanicCompleted;
            lblDetailResult.Content = detailCompleted;
            lblMinistryResult.Content = ministryCompleted;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            while (SoldVehicleWithDateGrid.Children.Count > 0)
            {
                SoldVehicleWithDateGrid.Children.RemoveAt(0);
            }

            SearchVehicles();

            AddDates();
        }
    }
}
