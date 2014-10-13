using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarDepot.Pages
{
    interface IPropertyPage
    {
        string PageTitle { get; }
        bool IsCloseable { get; }
    }
}
