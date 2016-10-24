using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarDepot.Controls
{
    class AdminCheckBox : CheckBox, IPropertyPanel
    {
        private IAdminObject _item = null;
        public PropertyId PropertyId { set; get; }

        public AdminCheckBox()
        {
            Checked += AdminCheckBox_Checked;
            Unchecked += AdminCheckBox_Unchecked;
        }

        private void AdminCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _item.SetValue(PropertyId, "false");
        }

        private void AdminCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_item == null)
                return;

            _item.SetValue(PropertyId, "true");
            
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void LoadPanel(IAdminObject item)
        {
            _item = item;

            if (item == null)
            {
                IsChecked = false;
            }
            else
            {
                string value = item.GetValue(PropertyId);
                if (value != null && value.Trim().ToLower() == "true")
                    IsChecked = true;
                else
                    IsChecked = false;
            }
        }
    }
}
