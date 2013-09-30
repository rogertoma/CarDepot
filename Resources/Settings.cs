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
        public static string UserAccountsPath = @"C:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\Users";
        public static string VehicleActivePath = @"C:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\ActiveVehicles";
        public static string VehicleSoldPath = @"C:\Users\rogerto\Dropbox\Apps\wpf\CarDepot\CarDepot\bin\Debug\Data\Users";
        public static string VehicleImageFolder = @"\Images";

        public static int MultiValueKeyIndex = 0;
        public static int MultiValueValueIndex = 1;
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
