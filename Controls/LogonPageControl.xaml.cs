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
using CarDepot.VehicleStore;

namespace CarDepot.Controls
{
    /// <summary>
    /// Interaction logic for LogonPageControl.xaml
    /// </summary>
    public partial class LogonPageControl : UserControl
    {
        public LogonPageControl()
        {
            InitializeComponent();
            LoadUserIcons();
        }

        private void LoadUserIcons()
        {
            //int insertPosX = 0;
            //int insertPosY = 0;
            foreach (var user in CacheManager.UserCache)
            {
                LogonTileControl iconControl = new LogonTileControl(user);
                iconControl.OnUserCredentialsEntered += iconControl_OnUserCredentialsEntered;
                UsersPanel.Children.Add(iconControl);
            }
        }

        void iconControl_OnUserCredentialsEntered(bool validCredentials, UserAdminObject user)
        {
            if (validCredentials)
            {
                user.OpenedPages.Clear();
                CacheManager.ActiveUser = user;

                MainWindow carDepot = new MainWindow(user);
                carDepot.Closed += carDepot_Closed;
                carDepot.Show();
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show("FAIL FOR: " + user.Name);
            }
        }

        void carDepot_Closed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
