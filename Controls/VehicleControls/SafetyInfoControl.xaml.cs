using CarDepot.VehicleStore;
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

namespace CarDepot.Controls.VehicleControls
{
    /// <summary>
    /// Interaction logic for SafetyInfoControl.xaml
    /// </summary>
    public partial class SafetyInfoControl : UserControl, IPropertyPanel
    {
        VehicleAdminObject _vehicle = null;
        public SafetyInfoControl()
        {
            InitializeComponent();
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void LoadPanel(IAdminObject item)
        {
            _vehicle = null;
            _vehicle = item as VehicleAdminObject;

            ClearFormState();

            LoadAllChildren(SafetyInfoGrid, item);

            LoadComboBoxes();
        }

        private void ClearFormState()
        {
            cmbGasTank.Items.Clear();
            cmbInspectedBy.Items.Clear();
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

                GroupBox isGroupBox = child as GroupBox;
                if (isGroupBox != null)
                {
                    LoadAllGroupChildren(isGroupBox, item);
                }

                Panel isPanel = child as Panel;
                if (isPanel != null)
                    LoadAllChildren(isPanel, item);

            }
        }

        private void LoadAllGroupChildren(GroupBox group, IAdminObject item)
        {
            foreach (Control ctrl in ((Grid)group.Content).Children)
            {
                if (ctrl is GroupBox)
                {
                    GroupBox _innerGB = ctrl as GroupBox;
                    foreach (Control gbc in ((Grid)_innerGB.Content).Children)
                    {
                        IPropertyPanel propPanel = gbc as IPropertyPanel;
                        if (propPanel != null)
                        {
                            propPanel.LoadPanel(item);
                        }
                    }
                }
            }
        }

        private void LoadComboBoxes()
        {
            //Initialize Fuel Levels
            List<string> gasLevels = new List<string>();
            gasLevels.Add("Empty");
            gasLevels.Add("1/4");
            gasLevels.Add("1/2");
            gasLevels.Add("3/4");
            gasLevels.Add("Full");


            int foundIndex = -1;

            string fuelLevel = _vehicle.GetValue(PropertyId.SafetyGasTankLevel);
            Label emptyRow = new Label { Content = "" };
            cmbGasTank.Items.Add(emptyRow);
            foreach (string level in gasLevels)
            {
                Label lblLevel = new Label { Content = level };
                cmbGasTank.Items.Add(lblLevel);

                if (fuelLevel != null && fuelLevel == lblLevel.Content.ToString())
                {
                    foundIndex = cmbGasTank.Items.Count - 1;
                }
            }
            if (foundIndex != -1)
                cmbGasTank.SelectedIndex = foundIndex;


            string inspectedBy = _vehicle.GetValue(PropertyId.SafetyInspectedBy);
            foundIndex = -1;

            //Initialize cmbCompletedBy ComboBox
            emptyRow = new Label { Content = "" };
            cmbInspectedBy.Items.Add(emptyRow);
            foreach (UserAdminObject user in CacheManager.UserCache)
            {
                Label lblRow = new Label { Content = user.Name };
                cmbInspectedBy.Items.Add(lblRow);
                if (inspectedBy != null && inspectedBy == lblRow.Content.ToString())
                {
                    foundIndex = cmbInspectedBy.Items.Count - 1;
                }
            }

            if (foundIndex != -1)
                cmbInspectedBy.SelectedIndex = foundIndex;
        }

        private void btnPrintSafetyInspectionReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintSafetyInspectionForm printCurrentCar = new PrintSafetyInspectionForm(_vehicle, sender, e);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Printing Safety Inspection Report Error: " + ex.Message);
            }
        }
    }
}
