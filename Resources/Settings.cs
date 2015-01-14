using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CarDepot.VehicleStore;
using CarDepot.Controls;

namespace CarDepot.Resources
{
    static class Settings
    {
        public static string UserAccountsPath = @"x:\Data\Users";
        public static string VehiclePath = @"x:\Data\Vehicles";
        public static string CustomerPath = @"x:\Data\Customers";
        public static string Resouces = @"x:\Data\Resources";
        public static string TempFolder = @"x:\Data\Temp";
        public static string DefaultVehicleImagePath = @"x:\Data\Resources\DefaultVehicleImage.jpg";

        public static string CustomerInfoFileName = @"info.xml";        
        public static string CustomerInfoDefaultFileText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Customer>\n</Customer>";
        public static string VehicleInfoFileName = @"info.xml";        
        public static string VehicleInfoDefaultFileText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Vehicle>\n</Vehicle>";
        //public static string VehicleSoldPath = @"x:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\Users";
        public static string VehicleImageFolder = @"\Images";
        public static string AdditionalFilesFolder = @"\Files";

        public static int MultiValueKeyIndex = 0;
        public static int MultiValueValueIndex = 1;

        public static double HST = 0.13;

        public static string MoveToItemImageFolder(IAdminObject item, string origionalFilePath)
        {
            string filePath = new FileInfo(item.ObjectId).Directory.FullName + Settings.VehicleImageFolder + "\\";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            string[] allImages = Directory.GetFiles(filePath);

            int fileNumber = 0;
            foreach (string image in allImages)
            {
                int num;
                FileInfo file = new FileInfo(image);
                int.TryParse(file.Name.Replace(file.Extension, ""), out num);

                if (num > fileNumber)
                    fileNumber = num;
            }
            fileNumber++;

            filePath = filePath + fileNumber + new FileInfo(origionalFilePath).Extension;

            File.Copy(origionalFilePath, filePath);

            return filePath;
        }
    }

    static class LookAndFeel
    {
        public static int TabItemHeight = 40;
        public static Brush VehicleTabColor = Brushes.Cyan;
        public static Brush SearchVehicleColor = Brushes.LightGreen;
        public static Brush CustomerTabColor = Brushes.LightBlue;
        public static Brush SearchCustomerColor = Brushes.LightGreen;
        public static Brush SearchTasksColor = Brushes.LightGreen;
    }

    static class DefaultColors
    {
        //this.BorderBrush = new LinearGradientBrush(Colors.LimeGreen, Colors.White, new Point(0.5, 0), new Point(0.5, 1));
        public static Color LINEAR_GRADIANT_BRUSH_START = Colors.LimeGreen;
        public static Color LINEAR_GRADIANT_BRUSH_END = Colors.White;
    }

    static class UISettings
    {
        public static double ADMINLABELTEXTBOX_GAPSIZE = 130;
        public static double ADMINTEXTBOX_MINSIZE_WHEN_NO_TEXT = 70;
    }

    static class Utilities
    {
        public static bool StringToDouble(string input, out double result)
        {

            if (input == null)
            {
                input = string.Empty;
            }
            input = input.Replace("$", "");
            input = input.Replace(",", "");
            return double.TryParse(input, out result);
        }

        public static void LoadVehicleInfoWindow(VehicleAdminObject vehicle)
        {
            LoadVehicleInfoWindow(vehicle, VehicleInfoWindow.VehicleInfoWindowTabs.Default);
        }

        public static void LoadVehicleInfoWindow(VehicleAdminObject vehicle, VehicleInfoWindow.VehicleInfoWindowTabs startTab)
        {
            if (vehicle == null)
            {
                throw new NullReferenceException("Load vehicle info window requires non null vehicle");
            }
            else
            {

                //VehicleSearchPage page = new VehicleSearchPage();

                //if (mainTabControl == null)
                //{
                //    throw new NotImplementedException("MainTabControl == null");
                //}

                //ClosableTab tabItem = new ClosableTab();
                //tabItem.Height = LookAndFeel.TabItemHeight;
                //tabItem.Title = page.PageTitle;
                //tabItem.Content = page;
                //mainTabControl.Items.Add(tabItem);
                //tabItem.Focus();

                IAdminItemCache tempCache = vehicle.Cache;
                vehicle = new VehicleAdminObject(vehicle.ObjectId);
                vehicle.Cache = tempCache;

                VehicleInfoWindow window = new VehicleInfoWindow(vehicle, startTab);
                if (CacheManager.MainTabControl == null)
                {
                    throw new NotImplementedException("MainTabControl == null");
                }

                ClosableTab tabItem = new ClosableTab();
                tabItem.BackGroundColor = LookAndFeel.VehicleTabColor;
                window.SetParentTabControl(tabItem);
                tabItem.Height = LookAndFeel.TabItemHeight;
                tabItem.Title = vehicle.Year + ": " + vehicle.Make + " - " + vehicle.Model;
                tabItem.Content = window;
                CacheManager.MainTabControl.Items.Add(tabItem);
                tabItem.Focus();

                //window.Show();
                //CacheManager.ActiveUser.AddPage(window);
            }
        }

    }
}
