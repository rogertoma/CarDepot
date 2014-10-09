using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CarDepot.VehicleStore;

namespace CarDepot.Resources
{
    static class Settings
    {
        public static string UserAccountsPath = @"C:\Data\Users";
        public static string VehiclePath = @"C:\Data\Vehicles";
        public static string CustomerPath = @"C:\Data\Customers";
        public static string Resouces = @"C:\Data\Resources";
        public static string CustomerInfoFileName = @"info.xml";        
        public static string CustomerInfoDefaultFileText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Customer>\n</Customer>";
        public static string VehicleInfoFileName = @"info.xml";        
        public static string VehicleInfoDefaultFileText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Vehicle>\n</Vehicle>";
        public static string VehicleSoldPath = @"C:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\Users";
        public static string VehicleImageFolder = @"\Images";
        public static string AdditionalFilesFolder = @"\Files";
        public static string TempFolder = @"C:\Data\Temp";

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
            if (vehicle == null)
            {
                throw new NullReferenceException("Load vehicle info window requires non null vehicle");
            }
            else
            {
                VehicleInfoWindow window = new VehicleInfoWindow(vehicle);
                window.Show();
                CacheManager.ActiveUser.AddPage(window);
            }
        }
    }
}
