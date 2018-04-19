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
using CarDepot.Pages;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using System.IO;
using Microsoft.Office.Interop.Excel;
using static CarDepot.Controls.VehicleControls.PurchaseInfoControl;

namespace CarDepot.Controls.GeneralControls
{
    /// <summary>
    /// Interaction logic for ActionsControl.xaml
    /// </summary>
    public partial class ActionsControl : UserControl, IPropertyPage, IPropertyPanel
    {
        private System.Timers.Timer switchToLoadAllVehicles = null;

        public ActionsControl()
        {
            InitializeComponent();
            ApplyActiveUserPermissions();
            txtIDToLoad.Text = CacheManager.LatestVehicleIdToLoad.ToString();

            switchToLoadAllVehicles = new System.Timers.Timer();
            switchToLoadAllVehicles.Interval = 900000; //15 minutes 5 minutes before a new full refresh should happen
            switchToLoadAllVehicles.Elapsed += SwitchToLoadAllVehicles_Elapsed;
            switchToLoadAllVehicles.Start();
        }

        private void SwitchToLoadAllVehicles_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtIDToLoad.Text = "0";

                switchToLoadAllVehicles.Stop();
            });
        }

        public void UpdateLatestIDToLoad(string value)
        {
            txtIDToLoad.Text = value;
        }


        public string PageTitle
        {
            get { return Strings.CONTROL_ACTIONSCONTROL_TAB_TITLE; }
        }

        public bool IsCloseable
        {
            get { return false; }
        }

        private void BtnCreateNewVehicle_Click(object sender, RoutedEventArgs e)
        {
            VehicleInfoWindow page = new VehicleInfoWindow();

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.VehicleTabColor;
            page.SetParentTabControl(tabItem);
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = "New Vehicle";
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);

            tabItem.Focus();
        }

        private void BtnSearchVehicles_Click(object sender, RoutedEventArgs e)
        {
            VehicleSearchPage page = new VehicleSearchPage();

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchVehicleColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void BtnCreateNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerInfoPage page = new CustomerInfoPage();

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.CustomerTabColor;
            page.SetParentTabControl(tabItem);
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = "New Customer";
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();

            //CustomerInfoPage customerInfo = new CustomerInfoPage();
            //customerInfo.Show();
        }

        private void BtnSearchTasks_Click(object sender, RoutedEventArgs e)
        {

            TasksSearchPage page = new TasksSearchPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchTasksColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void BtnSearchCustomers_Click(object sender, RoutedEventArgs e)
        {
            CustomerSearchPage page = new CustomerSearchPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchCustomerColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        public void LoadPanel(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.CreateNewCustomer))
            {
                BtnCreateNewCustomer.Visibility = Visibility.Visible;
            }
            else
            {
                BtnCreateNewCustomer.Visibility = Visibility.Hidden;
            }

            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.CreateNewVehicle))
            {
                BtnCreateNewVehicle.Visibility = Visibility.Visible;
                BtnBatchManheimImport.Visibility = Visibility.Visible;
                BtnBatchAdesaImport.Visibility = Visibility.Visible;
            }
            else
            {
                BtnCreateNewVehicle.Visibility = Visibility.Hidden;
                BtnBatchManheimImport.Visibility = Visibility.Hidden;
                BtnBatchAdesaImport.Visibility = Visibility.Hidden;
            }

        }

        private void BtnKeyCheckout_Click(object sender, RoutedEventArgs e)
        {
            KeyCheckoutPage page = new KeyCheckoutPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchTasksColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void BtnSearchSold_Click(object sender, RoutedEventArgs e)
        {
            DeliveryVehicleSearchPage page = new DeliveryVehicleSearchPage();
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;

            if (CacheManager.MainTabControl == null)
            {
                throw new NotImplementedException("MainTabControl == null");
            }

            ClosableTab tabItem = new ClosableTab();
            tabItem.BackGroundColor = LookAndFeel.SearchSoldVehiclesColor;
            tabItem.Height = LookAndFeel.TabItemHeight;
            tabItem.Title = page.PageTitle;
            tabItem.Content = page;
            CacheManager.MainTabControl.Items.Add(tabItem);
            tabItem.Focus();
        }

        private void txtIDToLoad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtIDToLoad.Text != "-2")
                int.TryParse(txtIDToLoad.Text, out CacheManager.LatestVehicleIdToLoad);
                
        }

        private void BtnBatchManheimImport_Click(object sender, RoutedEventArgs e)
        {
            string result = Microsoft.VisualBasic.Interaction.InputBox("Please specify the manheim csv file path", "Path");
            if (result == "")
            {
                return;
            }

            if (!File.Exists(result))
            {
                MessageBox.Show("File not found", "Error");
                return;
            }

            using (var reader = new StreamReader(result))
            {
                const int VIN = 0;
                const int Year = 1;
                const int Make = 2;
                const int Model = 3;
                const int Trim = 4;
                const int Exterior = 5;
                const int Interior = 6;
                const int odometer = 7;
                const int transmission = 9;
                const int engine = 10;
                const int purchaseDate = 13;
                const int purchasedFrom = 19;
                const int totalPaid = 27;
                const int buyerFee = 50;
                const int otherCost = 51;
                const int tax = 52;


                List<string> vins = new List<string>();

                var line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    line = line.ToUpper();
                    int temp;
                    bool cFound = false;
                    for (int i = 17; i < line.Length; i++)
                    {
                        if (line[i] == 'C' && line[i + 1] == '$')
                        {
                            cFound = true;
                        }
                        else if (cFound && line[i] == ',' && int.TryParse(line[i + 1].ToString(), out temp))
                        {
                            //line = line.Substring(0, i) + line.Substring(i + 1, line.Length);
                            line = line.Remove(i, 1);
                        }
                        else if (cFound && line[i] == ',' && !int.TryParse(line[i + 1].ToString(), out temp))
                        {
                            cFound = false;
                        }
                        else if (cFound == true && line[i] == '"')
                        {
                            cFound = false;
                        }
                    }

                    line = line.Replace(", INC", " INC");

                    var values = line.Split(',');

                    double totalPaidAmount = 0;
                    VehicleAdminObject vehicle = null;
                    double dBuyerFee = 0;
                    double dOtherCost = 0;
                    double dTotalTax = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (vehicle == null && i != 0)
                        {
                            continue;
                        }
                        switch (i)
                        {
                            case VIN:
                                if (!vins.Contains(values[i]))
                                {
                                    vehicle = Utilities.CreateNewDefaultVehicleObject();
                                    vehicle.SetValue(PropertyId.VinNumber, values[i]);
                                    vehicle.SetValue(PropertyId.Vendor, WellKnownVendors.ManheimToronto.ToString());
                                    vins.Add(vehicle.VinNumber);
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            case Year:
                                vehicle.SetValue(PropertyId.Year, values[i]);
                                break;
                            case Make:
                                vehicle.SetValue(PropertyId.Make, values[i]);
                                break;
                            case Model:
                                vehicle.SetValue(PropertyId.Model, values[i]);
                                break;
                            case Trim:
                                vehicle.SetValue(PropertyId.Trim, values[i]);
                                break;
                            case Exterior:
                                vehicle.SetValue(PropertyId.ExtColor, values[i]);
                                break;
                            case Interior:
                                vehicle.SetValue(PropertyId.IntColor, values[i]);
                                break;
                            case odometer:
                                vehicle.SetValue(PropertyId.Mileage, values[i]);
                                break;
                            case transmission:
                                vehicle.SetValue(PropertyId.Transmission, values[i]);
                                break;
                            case engine:
                                vehicle.SetValue(PropertyId.Engine, values[i]);
                                break;
                            case purchaseDate:
                                //DateTime pDate = DateTime.Parse(values[i]);
                                //vehicle.SetValue(PropertyId.PurchaseDate, pDate.ToString("d"));
                                break;
                            case purchasedFrom:
                                vehicle.SetValue(PropertyId.VendorDescription, values[i]);
                                break;
                            case totalPaid:
                                string stotalPaid = values[i];
                                Utilities.StringToDouble(stotalPaid, out totalPaidAmount);
                                break;
                            case buyerFee:
                                Utilities.StringToDouble(values[i], out dBuyerFee);
                                vehicle.SetValue(PropertyId.PurchaseBuyerFee, dBuyerFee);
                                break;
                            case otherCost:
                                Utilities.StringToDouble(values[i], out dOtherCost);
                                vehicle.SetValue(PropertyId.PurchaseOtherCosts, dOtherCost);
                                break;
                            case tax:
                                string sTotalTax = values[i];
                                Utilities.StringToDouble(sTotalTax, out dTotalTax);
                                double purchasePrice = totalPaidAmount - dBuyerFee - dOtherCost - dTotalTax;
                                vehicle.SetValue(PropertyId.PurchasePrice, purchasePrice);
                                break;
                        }
                    }

                    if (vehicle != null)
                    {
                        vehicle.Save(this);
                    }
                }
                
            }
        }

        private void BtnBatchAdesaImport_Click(object sender, RoutedEventArgs e)
        {

            const int VIN = 1;
            const int YearMakeMode = 3;
            const int Mileage = 4;
            const int ExtColor = 5;
            const int Vendor = 6;
            const int PurchaseDate = 7;
            const int PurchasePrice = 18;
            const int TotalPrice = 17;

            string result = Microsoft.VisualBasic.Interaction.InputBox("Please specify the manheim csv file path", "Path");
            if (result == "")
            {
                return;
            }

            if (!File.Exists(result))
            {
                MessageBox.Show("File not found", "Error");
                return;
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(result);
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            bool vinFound = false;
            for (int i = 1; i <= rowCount; i++)
            {
                VehicleAdminObject vehicle = null;
                try
                {
                    string value = xlRange.Cells[i, 1].Value2.ToString().ToUpper().Trim();
                    if (!vinFound && value.Equals("VIN"))
                    {
                        vinFound = true;
                        continue;
                    }
                    else if (vinFound)
                    {
                        for (int j = 1; j <= colCount; j++)
                        {
                            try
                            {
                                string foundObject = xlRange.Cells[i, j].Value2.ToString().ToUpper().Trim();
                                switch (j)
                                {
                                    case VIN:
                                        vehicle = Utilities.CreateNewDefaultVehicleObject();
                                        vehicle.SetValue(PropertyId.VinNumber, foundObject);
                                        vehicle.SetValue(PropertyId.Vendor, WellKnownVendors.Adesa.ToString());
                                        break;
                                    case YearMakeMode:
                                        var values = foundObject.Split('/');
                                        if (values.Length < 3)
                                            continue;
                                        vehicle.SetValue(PropertyId.Year, values[0]);
                                        vehicle.SetValue(PropertyId.Make, values[1]);
                                        vehicle.SetValue(PropertyId.Model, values[2]);
                                        break;
                                    case Mileage:
                                        foundObject = foundObject.ToUpper().Replace("KM", "").Trim();
                                        vehicle.SetValue(PropertyId.Mileage, foundObject);
                                        break;
                                    case ExtColor:
                                        vehicle.SetValue(PropertyId.ExtColor, foundObject);
                                        break;
                                    case Vendor:
                                        vehicle.SetValue(PropertyId.VendorDescription, foundObject);
                                        break;
                                    case PurchaseDate:
                                        DateTime pDate = DateTime.Parse(foundObject);
                                        vehicle.SetValue(PropertyId.PurchaseDate, pDate.ToString("d"));
                                        break;
                                    case PurchasePrice:
                                        vehicle.SetValue(PropertyId.PurchasePrice, foundObject);
                                        break;

                                }
                            }catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
                
                if (vehicle != null)
                    vehicle.Save(this);
            }


        }
    }
}
