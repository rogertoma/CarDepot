using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CarDepot.Controls;
using CarDepot.Controls.VehicleControls;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using MessageBox = System.Windows.MessageBox;

namespace CarDepot
{
    /// <summary>
    /// Interaction logic for VehicleInfoWindow.xaml
    /// </summary>
    public partial class VehicleInfoWindow : Window, IPropertyPanel
    {
        private VehicleAdminObject _vehicle;
        List<IPropertyPanel> propertyPanels = new List<IPropertyPanel>();

        public VehicleInfoWindow() : this (null)
        {
            //InitializeComponent();
        }

        private void InitializePage()
        {

        }

        private int GetNextFolderId()
        {
            int largestId = 0;
            List<string> directories = Directory.GetDirectories(Settings.VehiclePath).ToList();

            foreach (var directory in directories)
            {
                DirectoryInfo dir = new DirectoryInfo(directory);
                int id = 0;
                if (int.TryParse(dir.Name, out id))
                {
                    if (id > largestId)
                        largestId = id;
                }
            }

            return largestId;
        }

        private VehicleAdminObject CreateNewDefaultVehicleObject()
        {
            int lastId = GetNextFolderId();

            DirectoryInfo newDirectory = Directory.CreateDirectory(Settings.VehiclePath + "\\" + (lastId + 1));
            FileStream newfile = File.Create(newDirectory.FullName + "\\" + Settings.VehicleInfoFileName);
            string fileName = newfile.Name;
            newfile.Close();

            File.WriteAllText(fileName, Settings.VehicleInfoDefaultFileText);

            return new VehicleAdminObject(fileName);

            //TODO: add this vehicle to some cache.
        }

        public VehicleInfoWindow(VehicleAdminObject vehicle)
        {
            InitializeComponent();
            _vehicle = vehicle ?? CreateNewDefaultVehicleObject();

            propertyPanels.Add(BasicVehicleControlPropertyPanel);
            propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);

            LoadPanel(_vehicle);
        }

        public void LoadPanel(IAdminObject item)
        {
            foreach (IPropertyPanel propertyPanel in propertyPanels)
            {
                propertyPanel.LoadPanel(item);
                //propertyPanel.UpdatePanel(item);
            }
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            
        }

        private void VehicleInfoWindow_OnClosing(object sender, CancelEventArgs e)
        {
            VehicleAdminObject origionalVehicle = new VehicleAdminObject(_vehicle.ObjectId);

            bool same = _vehicle.Equals(origionalVehicle);

            if (same)
                return;

            MessageBoxResult result = MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING,
                Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_WARNING_TITLE, MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                bool successfulSave = _vehicle.Save(this);
                if (!successfulSave)
                {
                    MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                    Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
                    e.Cancel = true;
                }
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool successfulSave = _vehicle.Save(this);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            VehicleCache cache = _vehicle.Cache as VehicleCache;
            cache.Remove(_vehicle);
            _vehicle = new VehicleAdminObject(_vehicle.ObjectId);
            cache.Add(_vehicle);
            LoadPanel(_vehicle);
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            string result = Microsoft.VisualBasic.Interaction.InputBox("Prompt here", "Title here", "Default data", -1, -1);
            VehicleUrlImport urlImport = new VehicleUrlImport(result);

            if (urlImport == null)
                return;

            BasicVehicleInfoControl infoControl = null;

            foreach (var propertyPanel in propertyPanels)
            {
                infoControl = propertyPanel as BasicVehicleInfoControl;
                if (infoControl != null)
                    break;
            }

            if (infoControl == null)
                return;

            List<AdminLabelTextbox> adminLabelTextboxes = infoControl.GetAdminLabelTextbox();

            foreach (var adminLabelTextbox in adminLabelTextboxes)
            {
                adminLabelTextbox.TextBoxText = urlImport.GetDataFromPropertyId(adminLabelTextbox.PropertyId);
            }
        }
    }
}
