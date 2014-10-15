using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for ImageViewerControl.xaml
    /// </summary>
    public partial class ImageViewerControl : UserControl, IPropertyPanel
    {

        private delegate void UpdateVehicleImageDelegate(string filePath);

        private IAdminObject _item = null;
        public PropertyId PropertyId { set; get; }
        private List<string[]> _images;
        private Timer imageTimer = null;
        private int imageLoadIndex = 0;

        public ImageViewerControl()
        {
            InitializeComponent();

            imageTimer = new Timer();
            imageTimer.Interval = 3000;
            imageTimer.Elapsed += imageTimer_Elapsed;
        }


        public void LoadPanel(IAdminObject item)
        {
            _item = item;

            ThumbNailWrapPanel.Children.Clear();

            if (item == null)
            {
                LoadDefaultImage();
                return;
            }

            _images = item.GetMultiValue(PropertyId);

            if (_images == null || _images.Count == 0)
            {
                LoadDefaultImage();
                return;
            }

            try
            {
                LoadVehicleImageLarge(_images[imageLoadIndex][Settings.MultiValueValueIndex]);
                LoadVehicleThumbNails();
            }
            catch {}
            imageTimer.Start();
        }

        private bool IsImage(string filePath)
        {
            try
            {
                System.Drawing.Image.FromFile(filePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        void imageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            imageTimer.Stop();
            imageLoadIndex++;
            if (imageLoadIndex >= _images.Count)
            {
                imageLoadIndex = 0;
            }

            Dispatcher.Invoke(new UpdateVehicleImageDelegate(LoadVehicleImageLarge), _images[imageLoadIndex][Settings.MultiValueValueIndex]);
        }

        private void LoadVehicleImageLarge(string path)
        {
            if (!File.Exists(path))
                LoadDefaultImage();

            try
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(path);
                img.EndInit();
                VehicleImageLarge.Source = img;
                imageTimer.Start();
            }
            catch (Exception)
            {
                LoadDefaultImage();
            }
        }

        private void LoadVehicleThumbNails()
        {
            if (_images == null || _images.Count == 0)
                return;

            foreach (string[] image in _images)
            {
                if (!File.Exists(image[Settings.MultiValueValueIndex]))
                    continue;

                ImageThumbNailControl thumb = new ImageThumbNailControl();
                thumb.SetImage(image[Settings.MultiValueValueIndex]);
                thumb.Margin = new Thickness(5);
                thumb.DeleteImage += thumb_DeleteImage;
                thumb.ImageClicked += thumb_ImageClicked;

                ThumbNailWrapPanel.Children.Add(thumb);
            }
        }

        void thumb_ImageClicked(string imagePath)
        {
            LoadVehicleImageLarge(imagePath);
            imageTimer.Stop();
        }

        void thumb_DeleteImage(string deleteImagePath)
        {
            foreach (string[] image in _images)
            {
                string imagePath = image[Settings.MultiValueValueIndex];

                if (imagePath.Equals(deleteImagePath))
                {
                    _images.Remove(image);
                    break;
                }
            }

            _item.SetValue(PropertyId.Images, _images);
            LoadPanel(_item);
        }

        private void LoadDefaultImage()
        {
            LoadVehicleImageLarge(Settings.DefaultVehicleImagePath);
            imageTimer.Stop();
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileDroped in files)
            {
                if (!IsImage(fileDroped))
                    continue;

                string newPath = Settings.MoveToItemImageFolder(_item,fileDroped);
                _images.Add(new string[] { PropertyId.VehicleImage.ToString(), newPath });

                //TODO: should be outside for loop
                _item.SetValue(PropertyId, _images);
            }

            LoadPanel(_item);
        }

    }
}
