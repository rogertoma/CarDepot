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
    /// Interaction logic for ImageThumbNailControl.xaml
    /// </summary>
    public partial class ImageThumbNailControl : UserControl
    {
        public delegate void DeleteImageEventHandler(string deleteImagePath);
        public event DeleteImageEventHandler DeleteImage;

        public delegate void ImageClickedEventHanlder(string imagePath);
        public event ImageClickedEventHanlder ImageClicked;

        string imagePath = null;

        public ImageThumbNailControl()
        {
            InitializeComponent();
        }

        public void SetImage(string imagePath)
        {
            this.imagePath = imagePath;
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(imagePath);
            img.EndInit();
            
            imgThumbImage.Source = img;
        }

        private void lblClose_MouseEnter(object sender, MouseEventArgs e)
        {
            lblClose.Foreground = Brushes.Red;
        }

        private void lblClose_MouseLeave(object sender, MouseEventArgs e)
        {
            lblClose.Foreground = Brushes.Black;
        }

        private void lblClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DeleteImage != null)
            {
                MessageBoxResult result = MessageBox.Show(Strings.CONTROL_IMAGETHUMBNAILCONROL_CONFIRMDELETE, Strings.WARNING, MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteImage(imagePath);
                }
            }
        }

        private void imgThumbImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ImageClicked != null)
            {
                ImageClicked(imagePath);
            }
        }
    }
}
