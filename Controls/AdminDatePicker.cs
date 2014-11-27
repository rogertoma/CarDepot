using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Controls
{
    class AdminDatePicker : DatePicker, IPropertyPanel
    {
        private IAdminObject _item = null;
        
        public PropertyId PropertyId { set; get; }

        public AdminDatePicker()
        {
            SelectedDateChanged += AdminDatePicker_SelectedDateChanged;
            LostFocus += AdminDatePicker_LostFocus;
        }

        void AdminDatePicker_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            string selectedDate = "";
            if (SelectedDate != null)
                selectedDate = ((DateTime) SelectedDate).ToString("d");

            _item.SetValue(PropertyId, selectedDate);
        }

        void AdminDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void LoadPanel(IAdminObject item)
        {
            _item = item;

            //if (item == null || item.GetValue(PropertyId) == null)
            //{
            //    SelectedDate = DateTime.Today;
            //}
            //else
            //{
            string date = item.GetValue(PropertyId);
            if (!string.IsNullOrEmpty(date))
                SelectedDate = DateTime.Parse(date);
            //}
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }
    }
}
