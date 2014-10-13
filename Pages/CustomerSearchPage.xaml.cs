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
    /// Interaction logic for CustomerSearchPage.xaml
    /// </summary>
    public partial class CustomerSearchPage : UserControl
    {
        public CustomerSearchPage()
        {
            InitializeComponent();
        }

        public string PageTitle
        {
            get { return Strings.PAGE_CUSTOMERSEARCHPAGE_TITLE; }
        }
    }
}
