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

namespace CarDepot.Controls.GeneralControls
{
    /// <summary>
    /// Interaction logic for WebControlWindow.xaml
    /// </summary>
    public partial class WebControlWindow : Window
    {
        public delegate void CompletedURLReachedEventHandler();
        public event CompletedURLReachedEventHandler CompletedURLReached;

        string reachedURL = "";

        public WebControlWindow(string url, string completedURL)
        {
            InitializeComponent();

            webBrowser.Address = url;
            reachedURL = completedURL;
        }

        private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (CompletedURLReached == null)
                return;

            if (webBrowser.Address.ToLower().Contains(reachedURL))
            {
                CompletedURLReached();
            }
        }

        private void webBrowser_StatusMessage(object sender, CefSharp.StatusMessageEventArgs e)
        {
            /*
            if (CompletedURLReached == null)
                return;
            this.Dispatcher.Invoke(() =>
            {
                if (webBrowser.Address.ToLower().Contains(reachedURL))
                {
                    CompletedURLReached();
                }
            });
            */

            
        }

        private void webBrowser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (CompletedURLReached == null)
                return;

                if (webBrowser.Address.ToLower().Contains(reachedURL))
                {
                    CompletedURLReached();
                }
        }
    }
}
