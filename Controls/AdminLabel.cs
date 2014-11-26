using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CarDepot.VehicleStore;

namespace CarDepot.Controls
{
    class AdminLabel: Label, IPropertyPanel
    {
        public PropertyId PropertyId { set; get; }

        public AdminLabel()
        {
            
        }

        public virtual void LoadPanel(IAdminObject item)
        {
            if (item == null)
            {
                Content = String.Empty;
            }
            else
            {
                Content = item.GetValue(PropertyId);
            }
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            throw new NotImplementedException();
        }
    }
}
