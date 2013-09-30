using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarDepot.Resources;

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for VehicleInfoControl.xaml
    /// </summary>
    public partial class BasicVehicleInfoControl : UserControl, IPropertyPanel
    {
        private PropertyId[] basicIds = {PropertyId.Bodystyle, PropertyId.Engine, PropertyId.Fueltype, 
                                            PropertyId.Transmission, PropertyId.ExtColor, PropertyId.IntColor, 
                                            PropertyId.Mileage, PropertyId.StockNumber, PropertyId.ModelCode, 
                                            PropertyId.VinNumber, PropertyId.Comments, };
        public BasicVehicleInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            LoadBasicIds(item);
            LoadAllChildren(BasicVehicleInfo, item);
        }

        private void LoadBasicIds(IAdminObject item)
        {
            for (int i = 0; i < basicIds.Count(); i++) // PropertyId propertyId in basicIds)
            {
                PropertyId propertyId = basicIds[i];
                AdminLabelTextbox labelItem = new AdminLabelTextbox();
                labelItem.PropertyId = propertyId;
                labelItem.SetMinGapSize = UISettings.ADMINLABELTEXTBOX_GAPSIZE;
                
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                BasicIdsGrid.RowDefinitions.Add(rowDefinition);
                
                Grid.SetRow(labelItem, i);
                BasicIdsGrid.Children.Add(labelItem);
            }
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

        public string PageTitle
        {
            get { return Strings.PAGES_VEHICLEINFOWINDOW_BASICINFO_TAB_TITLE; }
        }
    }
}
