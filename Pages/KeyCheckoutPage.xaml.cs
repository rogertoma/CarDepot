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
    /// Interaction logic for KeyCheckoutPage.xaml
    /// </summary>
    public partial class KeyCheckoutPage : UserControl
    {
        public KeyCheckoutPage()
        {
            InitializeComponent();
        }

        public string PageTitle
        {
            get { return Strings.PAGE_KEYCHECKOUT_TITLE; }
        }
    }
}
