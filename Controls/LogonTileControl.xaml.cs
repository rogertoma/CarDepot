using System;
using System.Collections.Generic;
using System.IO;
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

namespace CarDepot.Controls
{
    /// <summary>
    /// Interaction logic for LogonTileControl.xaml
    /// </summary>
    public partial class LogonTileControl : UserControl
    {
        public event UserCredentialsEntered OnUserCredentialsEntered;
        public delegate void UserCredentialsEntered(bool validCredentials, UserAdminObject user);

        private readonly UserAdminObject _user;

        public LogonTileControl(UserAdminObject user)
        {
            InitializeComponent();
            _user = user;
            if (user.PicturePath != null)
            {
                BitmapImage icon = new BitmapImage();
                icon.BeginInit();
                icon.UriSource = new Uri(user.PicturePath);
                icon.EndInit();
                UserPictureBox.Source = icon;
            }

            Initialize();
        }

        private void Initialize()
        {
            NameLabel.Content = _user.Name;
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            if (!InputPassword.Password.Equals(_user.Password))
            {
                InputPassword.Password = "";
            }

            if (OnUserCredentialsEntered != null)
            {
                OnUserCredentialsEntered(InputPassword.Password.Equals(_user.Password), _user);

            }
        }
    }
}
