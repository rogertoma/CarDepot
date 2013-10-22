using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CarDepot.Resources
{
    static class Settings
    {
        public static string UserAccountsPath = @"C:\Data\Users";
        public static string VehiclePath = @"C:\Data\Vehicles";
        public static string VehicleInfoFileName = @"info.xml";
        public static string VehicleInfoDefaultFileText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Vehicle>\n</Vehicle>";
        public static string VehicleSoldPath = @"C:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\Users";
        public static string VehicleImageFolder = @"\Images";

        public static int MultiValueKeyIndex = 0;
        public static int MultiValueValueIndex = 1;
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
}
