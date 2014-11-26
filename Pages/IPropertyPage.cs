using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CarDepot.VehicleStore;

namespace CarDepot.Pages
{
    public interface IPropertyPage
    {
        string PageTitle { get; }
        bool IsCloseable { get; }
    }
}
