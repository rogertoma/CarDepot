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
    /// Interaction logic for AdminLabelTextbox.xaml
    /// </summary>
    public partial class AdminLabelTextbox : UserControl, IPropertyPanel
    {
        private PropertyId _propertyId;

        public AdminLabelTextbox()
        {
            InitializeComponent();
            PropertyValueTextBox.PropertyId = this.PropertyId;
        }

        public PropertyId PropertyId
        {
            set
            {
                _propertyId = value;
                PropertyValueTextBox.PropertyId = value;
            }
            get { return _propertyId; }
        }

        public new double FontSize
        {
            set
            {
                PropertyLabel.FontSize = value;
                PropertyValueTextBox.FontSize = value;
            }
            get { return PropertyLabel.FontSize; }
        }

        public double SetMinGapSize
        {
            set { PropertyLabel.MinWidth = value; }
            get { return PropertyLabel.MinWidth; }
        }

        public bool ApplyDefaultGApSize
        {
            get
            {
                if (PropertyLabel.MinWidth == UISettings.ADMINLABELTEXTBOX_GAPSIZE)
                    return true;
                else
                {
                    return false;
                }

            }
            set
            {
                if (value)
                    PropertyLabel.MinWidth = UISettings.ADMINLABELTEXTBOX_GAPSIZE;
                else
                {
                    PropertyLabel.MinWidth = 0;
                }
            }
        }

        public void LoadPanel(IAdminObject item)
        {
            if (item == null)
            {
                PropertyLabel.Content = String.Empty;
            }
            else
            {
                PropertyLabel.Content = PropertyId;
            }
            
            PropertyValueTextBox.LoadPanel(item);
        }

        public string TextBoxText
        {
            set { PropertyValueTextBox.Text = value; }
            get { return PropertyValueTextBox.Text; }
        }
    }
}
