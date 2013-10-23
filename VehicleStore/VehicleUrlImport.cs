using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDepot.VehicleStore
{
    internal class VehicleUrlImport
    {
        private Dictionary<PropertyId, string> dataMap = new Dictionary<PropertyId, string>();

        public VehicleUrlImport(string url)
        {
            dataMap.Add(PropertyId.Year, "2009");
            dataMap.Add(PropertyId.Make, "Honda");
            dataMap.Add(PropertyId.Model, "Civic");
        }

        public string GetDataFromPropertyId(PropertyId id)
        {
            if (dataMap.ContainsKey(id))
            {
                return dataMap[id];
            }

            return "";
        }
    }
}
