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

namespace CarDepot.Pages
{
    /// <summary>
    /// Interaction logic for VehicleSearchPage.xaml
    /// </summary>
    public partial class VehicleSearchPage : UserControl, IPropertyPage
    {
        public VehicleSearchPage()
        {
            InitializeComponent();
        }

        public string PageTitle
        {
            get { return Strings.PAGE_VEHICLESEARCHPAGE_TITLE; }
        }

        public void SetTabControlContext(TabControl control)
        {
            throw new NotImplementedException();
        }

        public bool IsCloseable 
        {
            get { return true; }
        }
    }
}
