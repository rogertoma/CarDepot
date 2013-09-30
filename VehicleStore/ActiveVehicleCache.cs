using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    class ActiveVehicleCache: VehicleCache
    {
        public ActiveVehicleCache()
            : base(Settings.VehicleActivePath)
        {
            
        }
    }
}
