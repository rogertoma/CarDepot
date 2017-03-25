using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using CarDepot.Resources;
using CarDepot.VehicleStore;

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for VehicleInfoControl.xaml
    /// </summary>
    public partial class BasicVehicleInfoControl : UserControl, IPropertyPanel
    {
        private IAdminObject _item = null;
        private PropertyId[] basicIds = { PropertyId.Bodystyle, PropertyId.Engine, PropertyId.Fueltype, 
                                            PropertyId.DriveTrain, PropertyId.Transmission, PropertyId.ExtColor, PropertyId.IntColor, 
                                            PropertyId.Mileage, PropertyId.StockNumber, PropertyId.ModelCode, 
                                            PropertyId.VinNumber, PropertyId.Comments, };
        public BasicVehicleInfoControl()
        {
            InitializeComponent();
            ApplyActiveUserPermissions();
        }

        public void LoadPanel(IAdminObject item)
        {
            _item = item;
            LoadBasicIds(item);
            LoadAllChildren(BasicVehicleInfo, item);

            string isOffProperty = _item.GetValue(PropertyId.IsOffProperty);
            if (!string.IsNullOrEmpty(isOffProperty) && isOffProperty.ToLower() == "true")
            {
                txtYear.FontColor = System.Windows.Media.Brushes.Pink;
                txtMake.FontColor = System.Windows.Media.Brushes.Pink;
                txtModel.FontColor = System.Windows.Media.Brushes.Pink;
                txtTrim.FontColor = System.Windows.Media.Brushes.Pink;
            }
        }

        public void ApplyUiMode()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            if (CacheManager.ActiveUser.Permissions.Contains(UserAdminObject.PermissionTypes.ShowCheckedOutBy))
            {
                AdminLabelTextbox checkedOutByLabelItem = new AdminLabelTextbox();
                checkedOutByLabelItem.PropertyId = PropertyId.CheckOutBy;
                checkedOutByLabelItem.SetMinGapSize = UISettings.ADMINLABELTEXTBOX_GAPSIZE;
                checkedOutByLabelItem.IsEditable = false;

                RowDefinition checkedOutByRowDefinition = new RowDefinition();
                checkedOutByRowDefinition.Height = GridLength.Auto;
                BasicIdsGrid.RowDefinitions.Add(checkedOutByRowDefinition);

                Grid.SetRow(checkedOutByLabelItem, basicIds.Count());

                bool checkedOutByAlreadyExists =
                    BasicIdsGrid.Children.Cast<AdminLabelTextbox>()
                        .Any(textbox => textbox.PropertyId == PropertyId.CheckOutBy);

                if (!checkedOutByAlreadyExists)
                    BasicIdsGrid.Children.Add(checkedOutByLabelItem);
            }

            //AdminCheckBox checkbox = new AdminCheckBox();
            //checkbox.PropertyId = PropertyId.IsOnProperty;
            //checkbox.IsChecked = false;

            //RowDefinition isOnProperty = new RowDefinition();
            //isOnProperty.Height = GridLength.Auto;
            //BasicIdsGrid.RowDefinitions.Add(isOnProperty);
            //Grid.SetRow(checkbox, basicIds.Count());
            //BasicIdsGrid.Children.Add(checkbox);

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

                bool alreadyExists = BasicIdsGrid.Children.Cast<AdminLabelTextbox>().Any(textbox => textbox.PropertyId == propertyId);

                if (!alreadyExists)
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

        public List<AdminLabelTextbox> GetAdminLabelTextbox()
        {
            return GetAdminLabelTextbox(BasicVehicleInfo);
        }

        public List<AdminLabelTextbox> GetAdminLabelTextbox(Grid myGrid)
        {
            List<AdminLabelTextbox> adminLabelTextboxes = new List<AdminLabelTextbox>();

            foreach (var child in myGrid.Children)
            {
                Grid isGrid = child as Grid;
                if (isGrid != null)
                {
                    adminLabelTextboxes.AddRange(GetAdminLabelTextbox(isGrid));
                }

                AdminLabelTextbox isAdminLabelTextBox = child as AdminLabelTextbox;
                if (isAdminLabelTextBox != null)
                {
                    adminLabelTextboxes.Add(isAdminLabelTextBox);
                }
            }

            return adminLabelTextboxes;
        }

        private void btnPrintBrochure_Click(object sender, RoutedEventArgs e)
        {
            VehicleAdminObject vehicle = _item as VehicleAdminObject;
            if (vehicle != null)
            {
                PrintCar printCurrentCar = new PrintCar(vehicle, sender, e);
            }
        }
    }
}
