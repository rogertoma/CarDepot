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
    internal static class Settings
    {
        public static string UserAccountsPath = @"r:\Data\Users";
        public static string VehiclePath = @"r:\Data\Vehicles";
        public static string CustomerPath = @"r:\Data\Customers";
        public static string Resouces = @"r:\Data\Resources";
        public static string TempFolder = @"r:\Data\Temp";
        public static string DefaultVehicleImagePath = @"r:\Data\Resources\DefaultVehicleImage.jpg";

        public static string CustomerInfoFileName = @"info.xml";

        public static string CustomerInfoDefaultFileText =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Customer>\n</Customer>";

        public static string VehicleInfoFileName = @"info.xml";

        public static string VehicleInfoDefaultFileText =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Vehicle>\n</Vehicle>";

        //public static string VehicleSoldPath = @"r:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\Users";
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

    internal static class LookAndFeel
    {
        public static int TabItemHeight = 40;
        public static Brush VehicleTabColor = Brushes.Cyan;
        public static Brush SearchVehicleColor = Brushes.LightGreen;
        public static Brush CustomerTabColor = Brushes.LightBlue;
        public static Brush SearchCustomerColor = Brushes.LightGreen;
        public static Brush SearchTasksColor = Brushes.LightGreen;
        public static Brush SearchSoldVehiclesColor = Brushes.LightGreen;
    }

    internal static class DefaultColors
    {
        //this.BorderBrush = new LinearGradientBrush(Colors.LimeGreen, Colors.White, new Point(0.5, 0), new Point(0.5, 1));
        public static Color LINEAR_GRADIANT_BRUSH_START = Colors.LimeGreen;
        public static Color LINEAR_GRADIANT_BRUSH_END = Colors.White;
    }

    internal static class UISettings
    {
        public static double ADMINLABELTEXTBOX_GAPSIZE = 130;
        public static double ADMINTEXTBOX_MINSIZE_WHEN_NO_TEXT = 70;
    }

    internal static class Utilities
    {
        public static VehicleAdminObject CreateNewDefaultVehicleObject()
        {
            int lastId = GetNextFolderId();

            DirectoryInfo newDirectory = Directory.CreateDirectory(Settings.VehiclePath + "\\" + (lastId + 1));
            FileStream newfile = File.Create(newDirectory.FullName + "\\" + Settings.VehicleInfoFileName);
            string fileName = newfile.Name;
            newfile.Close();

            File.WriteAllText(fileName, Settings.VehicleInfoDefaultFileText);

            VehicleAdminObject vehicle = new VehicleAdminObject(fileName);
            vehicle.SetValue(PropertyId.PurchaseDate, ((DateTime)DateTime.Now).ToString("d"));

            VehicleTask processVehicleTask = new VehicleTask();
            processVehicleTask.Id = "Process Vehicle";
            processVehicleTask.TaskVehicleId = vehicle.Id;
            processVehicleTask.CreatedDate = DateTime.Today.Date.ToString("d");
            processVehicleTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            processVehicleTask.AssignedTo = "Mathew Toma";
            processVehicleTask.Category = VehicleTask.TaskCategoryTypes.Documentation.ToString();
            processVehicleTask.CreatedBy = CacheManager.ActiveUser.Name;
            vehicle.VehicleTasks.Add(processVehicleTask);

            VehicleTask emissionVehicleTask = new VehicleTask();
            emissionVehicleTask.Id = "Create folder & tag key";
            emissionVehicleTask.TaskVehicleId = vehicle.Id;
            emissionVehicleTask.CreatedDate = DateTime.Today.Date.ToString("d");
            emissionVehicleTask.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            emissionVehicleTask.AssignedTo = "Sahar Kuba";
            emissionVehicleTask.Category = VehicleTask.TaskCategoryTypes.Documentation.ToString();
            emissionVehicleTask.CreatedBy = CacheManager.ActiveUser.Name;
            vehicle.VehicleTasks.Add(emissionVehicleTask);

            VehicleTask newVehicleClean = new VehicleTask();
            newVehicleClean.Id = Strings.NEWCARCLEANINGTASK;
            newVehicleClean.TaskVehicleId = vehicle.Id;
            newVehicleClean.CreatedDate = DateTime.Today.Date.ToString("d");
            newVehicleClean.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            newVehicleClean.AssignedTo = "Mike Wilson";
            newVehicleClean.Category = VehicleTask.TaskCategoryTypes.Detail.ToString();
            newVehicleClean.CreatedBy = CacheManager.ActiveUser.Name;
            vehicle.VehicleTasks.Add(newVehicleClean);

            VehicleTask newVehicleOilChange = new VehicleTask();
            newVehicleOilChange.Id = Strings.NEWCAROILCHANGETASK;
            newVehicleOilChange.TaskVehicleId = vehicle.Id;
            newVehicleOilChange.CreatedDate = DateTime.Today.Date.ToString("d");
            newVehicleOilChange.Status = VehicleTask.StatusTypes.NotStarted.ToString();
            newVehicleOilChange.AssignedTo = "Kevin Kokoski";
            newVehicleOilChange.Category = VehicleTask.TaskCategoryTypes.Other.ToString();
            newVehicleOilChange.CreatedBy = CacheManager.ActiveUser.Name;
            vehicle.VehicleTasks.Add(newVehicleOilChange);

            vehicle.SetValue(PropertyId.Tasks, vehicle.VehicleTasks);
            vehicle.Save(null);

            vehicle.Cache = CacheManager.AllVehicleCache;
            CacheManager.AllVehicleCache.Add(vehicle);

            return vehicle;

            //TODO: add this vehicle to some cache.
        }

        private static int GetNextFolderId()
        {
            int largestId = 0;
            List<string> directories = Directory.GetDirectories(Settings.VehiclePath).ToList();

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

        public static bool StringToDouble(string input, out double result)
        {

            if (input == null)
            {
                input = string.Empty;
            }
            input = input.Replace("$", "");
            input = input.Replace(",", "");
            input = input.Replace("C", "");
            input = input.Replace("\"", "");

            if (string.IsNullOrEmpty(input))
            {
                input = "0";
            }

            return double.TryParse(input, out result);
        }

        public static void LoadVehicleInfoWindow(VehicleAdminObject vehicle)
        {
            LoadVehicleInfoWindow(vehicle, VehicleInfoWindow.VehicleInfoWindowTabs.Default);
        }

        public static void LoadVehicleInfoWindow(VehicleAdminObject vehicle,
            VehicleInfoWindow.VehicleInfoWindowTabs startTab)
        {
            if (!File.Exists(Settings.DefaultVehicleImagePath))
            {
                MessageBox.Show("ERROR: Experiencing connectivity issues can't load vehicle");
                return;
            }

            if (vehicle == null)
            {
                throw new NullReferenceException("Load vehicle info window requires non null vehicle");
            }
            else
            {

                IAdminItemCache tempCache = vehicle.Cache;
                //THIS FORCES VEHICLE TO REFRESH
                //vehicle = new VehicleAdminObject(vehicle.ObjectId);
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
                tabItem.Title = vehicle.Year + ": " + vehicle.Make + " " + vehicle.Model;
                tabItem.Content = window;
                CacheManager.MainTabControl.Items.Add(tabItem);
                tabItem.Focus();

                //window.Show();
                //CacheManager.ActiveUser.AddPage(window);
            }
        }
    }
}
