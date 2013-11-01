using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarDepot.Controls.CustomerControls
{
    /// <summary>
    /// Interaction logic for CustomerInfoControl.xaml
    /// </summary>
    public partial class CustomerInfoControl : UserControl, IPropertyPanel
    {
        public CustomerInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            LoadAllChildren(ContactCardGrid, item);
        }

        public void LoadAllChildren(Panel panel, IAdminObject item)
        {
            foreach (var child in panel.Children)
            {
                IPropertyPanel propPanel = child as IPropertyPanel;
                if (propPanel != null)
                {
                    propPanel.LoadPanel(item);
                }

                Panel isPanel = child as Panel;
                if (isPanel != null)
                    LoadAllChildren(isPanel, item);
            }
        }
    }
}
