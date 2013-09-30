using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDepot.Controls
{
    interface IPropertyPanel
    {
        void LoadPanel( IAdminObject item );
    }
}
