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
using System.Windows.Shapes;

namespace CarDepot.Pages
{
    /// <summary>
    /// Interaction logic for LogonPage.xaml
    /// </summary>
    public partial class LogonPage : Window
    {
        public LogonPage()
        {
            InitializeComponent();
        }

        private void LogonPageControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (LogonControl.Visibility == Visibility.Hidden)
                this.Close();
        }
    }
}
