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
using CarDepot.Resources;

namespace CarDepot.Controls
{
    /// <summary>
    /// Interaction logic for VehicleTileControl.xaml
    /// </summary>
    public partial class VehicleTileControl : UserControl
    {
        private VehicleAdminObject _vehicle;
        private Brush _defaultBrush = null;
        
        public VehicleTileControl(VehicleAdminObject vehicle)
        {
            InitializeComponent();
            _vehicle = vehicle;
            _defaultBrush = this.BorderBrush;
            Initialize();
            this.ToolTip = String.Format("{0} {1} {2} : {3}", _vehicle.Year, _vehicle.Make, _vehicle.Model,
                                                      _vehicle.Trim);
        }

        public void Initialize()
        {
            VehicleTitle.Content = String.Format("{0} {1} {2} : {3}", _vehicle.Year, _vehicle.Make, _vehicle.Model,
                                                      _vehicle.Trim);            

            foreach (IPropertyPanel propertyLabel in LayoutGrid.Children.OfType<IPropertyPanel>())
            {
                propertyLabel.LoadPanel(_vehicle);
            }

            if (_vehicle.Images.Count > 0)
            {
                BitmapImage icon = new BitmapImage();
                icon.BeginInit();
                icon.UriSource = new Uri(_vehicle.Images[Settings.MultiValueKeyIndex][Settings.MultiValueValueIndex]);
                icon.EndInit();
                VehicleImage.Source = icon;
            }
        }

        public VehicleAdminObject Vehicle
        {
            get { return _vehicle; }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.BorderBrush = new LinearGradientBrush(DefaultColors.LINEAR_GRADIANT_BRUSH_START, DefaultColors.LINEAR_GRADIANT_BRUSH_END, new Point(0.5, 0), new Point(0.5, 1));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.BorderBrush = _defaultBrush;
        }
    }
}
