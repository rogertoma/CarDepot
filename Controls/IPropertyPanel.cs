using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarDepot.VehicleStore;

namespace CarDepot.Controls
{
    public interface IPropertyPanel
    {
        void LoadPanel( IAdminObject item );

        //void ApplyActiveUserPermissions();

        void ApplyUiMode();
    }
}
