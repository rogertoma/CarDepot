using System;
using System.Windows;
using System.Windows.Controls;
using CarDepot.Resources;

namespace CarDepot.Controls
{
    class AdminTextBox: TextBox, IPropertyPanel
    {
        private bool _isEditable = true;
        public PropertyId PropertyId { set; get; }
        private bool _hasFocus = false;
        private IAdminObject _item = null;

        public AdminTextBox()
        {
            Background = System.Windows.Media.Brushes.Transparent;
            BorderThickness = new Thickness(0);
            MouseEnter += AdminTextBox_MouseEnter;
            MouseLeave += AdminTextBox_MouseLeave;
            GotFocus += AdminTextBox_GotFocus;
            //LostFocus += AdminTextBox_LostFocus;
            TextChanged += AdminTextBox_TextChanged;
        }

        void AdminTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_item == null)
                return;

            _item.SetValue(PropertyId, Text);

            if (Text == string.Empty || Text == "")
                this.MinWidth = UISettings.ADMINTEXTBOX_MINSIZE_WHEN_NO_TEXT;
            else
            {
                this.MinWidth = 0;
            }

            Background = System.Windows.Media.Brushes.Transparent;
            BorderThickness = new Thickness(0);

            _hasFocus = false;
        }

        public void LoadPanel(IAdminObject item)
        {
            _item = item;

            if (item == null)
            {
                Text = String.Empty;
            }
            else
            {
                Text = item.GetValue(PropertyId);
                if (Text == string.Empty || Text == "")
                    this.MinWidth = UISettings.ADMINTEXTBOX_MINSIZE_WHEN_NO_TEXT;
            }
        }

        //void AdminTextBox_LostFocus(object sender, RoutedEventArgs e)
        //{

        //}

        void AdminTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Background = System.Windows.Media.Brushes.WhiteSmoke;
            BorderThickness = new Thickness(1);
            _hasFocus = true;
        }

        void AdminTextBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_hasFocus)
                return;
            BorderThickness = new Thickness(0);
        }

        void AdminTextBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_hasFocus)
                return;
            BorderThickness = new Thickness(1);
        }

        public bool IsEditable
        {
            set { _isEditable = value; }
            get { return _isEditable; }
        }
    }
}
