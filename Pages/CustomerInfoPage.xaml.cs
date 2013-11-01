using System;
using System.Collections.Generic;
using System.IO;
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
using CarDepot.Controls;
using CarDepot.Resources;

namespace CarDepot.Pages
{
    /// <summary>
    /// Interaction logic for CustomerInfoPage.xaml
    /// </summary>
    public partial class CustomerInfoPage : Window, IPropertyPanel
    {
        private CustomerAdminObject _customer;
        public CustomerInfoPage()
            : this(null)
        {
            //InitializeComponent();
        }

        public CustomerInfoPage(CustomerAdminObject customer)
        {
            InitializeComponent();
            _customer = customer ?? CreateNewDefaultVehicleObject();

            //propertyPanels.Add(BasicVehicleControlPropertyPanel);
            //propertyPanels.Add(ManageVehicleTasksControlPropertyPanel);

            LoadPanel(_customer);
        }

        public void LoadPanel(IAdminObject item)
        {
            CustomerInfoControl.LoadPanel(item);
        }

        private CustomerAdminObject CreateNewDefaultVehicleObject()
        {
            int lastId = GetNextFolderId();

            DirectoryInfo newDirectory = Directory.CreateDirectory(Settings.CustomerPath + "\\" + (lastId + 1));
            FileStream newfile = File.Create(newDirectory.FullName + "\\" + Settings.CustomerInfoFileName);
            string fileName = newfile.Name;
            newfile.Close();

            File.WriteAllText(fileName, Settings.CustomerInfoDefaultFileText);

            return new CustomerAdminObject(fileName);

            //TODO: add this vehicle to some cache.
        }

        private int GetNextFolderId()
        {
            int largestId = 0;
            List<string> directories = Directory.GetDirectories(Settings.CustomerPath).ToList();

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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool successfulSave = _customer.Save(this);
            if (!successfulSave)
            {
                MessageBox.Show(Strings.PAGES_VEHICLEINFOPAGE_ONCLOSING_UNABLETOSAVE,
                                Strings.PAGES_VEHICLEINFOPAGE_ERROR, MessageBoxButton.OK);
            }
        }
    }
}
