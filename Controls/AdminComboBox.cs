using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CarDepot.Controls.VehicleControls;

namespace CarDepot.Controls
{
    class AdminComboBox: ComboBox, IPropertyPanel
    {
        private IAdminObject _item = null;

        public PropertyId PropertyId { set; get; }

        public AdminComboBox()
        {
            SelectionChanged += AdminComboBox_SelectionChanged;
            LostFocus += AdminComboBox_LostFocus;
        }

        public void LoadPanel(IAdminObject item)
        {
            _item = item;
        }

        void AdminComboBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                string value;
                Label lblSelectedItem = SelectedItem as Label;

                if (lblSelectedItem != null)
                {
                    value = lblSelectedItem.Content.ToString(); 
                }
                else
                {
                    value = SelectedItem.ToString();
                }

                _item.SetValue(PropertyId, value);    
            }
        }

        void AdminComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            string value;
            Label lblSelectedItem = SelectedItem as Label;

            if (lblSelectedItem != null)
            {
                value = lblSelectedItem.Content.ToString();
            }
            else
            {
                value = SelectedItem.ToString();
            }

            _item.SetValue(PropertyId, value);
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }
    }
}

