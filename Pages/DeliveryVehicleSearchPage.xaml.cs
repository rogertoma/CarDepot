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

namespace CarDepot.Pages
{
    /// <summary>
    /// Interaction logic for DeliveryVehicleSearchPage.xaml
    /// </summary>
    public partial class DeliveryVehicleSearchPage : UserControl, IPropertyPage
    {
        public DeliveryVehicleSearchPage()
        {
            InitializeComponent();
        }

        public bool IsCloseable
        {
            get
            {
                return true;
            }
        }

        public string PageTitle
        {
            get { return "Sold Vehicle Checklist"; }
        }
    }
}
