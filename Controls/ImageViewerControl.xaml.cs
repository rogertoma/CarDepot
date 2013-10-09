﻿using System;
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

            if (_images == null)
            {
                LoadDefaultImage();
                return;
            }

            if (_images.Count == 0)
                return;

            LoadVehicleImageLarge(_images[imageLoadIndex][Settings.MultiValueValueIndex]);
            LoadVehicleThumbNails();
            imageTimer.Start();
        }

        private string MoveToImageFolder(string origionalFilePath)
        {
            string filePath = new FileInfo(_item.ObjectId).Directory.FullName + Settings.VehicleImageFolder + "\\";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            string[] allImages = Directory.GetFiles(filePath);

            int fileNumber = 0;
            foreach (string image in allImages)
            {
                int num;
                FileInfo file = new FileInfo(image);
                int.TryParse(file.Name.Replace(file.Extension, ""), out num);

                if (num > fileNumber)
                    fileNumber = num;
            }
            fileNumber++;

            filePath = filePath + fileNumber + new FileInfo(origionalFilePath).Extension;

            File.Copy(origionalFilePath, filePath);

            return filePath;
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

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(image[Settings.MultiValueValueIndex]);
                img.EndInit();
                Image thumb = new Image();
                thumb.Source = img;
                thumb.Margin = new Thickness(5);
                thumb.MouseUp += thumb_MouseUp;
                thumb.Width = 50;
                thumb.Height = 50;
                thumb.Stretch = Stretch.Uniform;
                ThumbNailWrapPanel.Children.Add(thumb);
            }
        }

        void thumb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LoadVehicleImageLarge(((Image)sender).Source.ToString());
            imageTimer.Stop();
        }

        private void LoadDefaultImage()
        {
            //TODO: Load default image
            imageTimer.Stop();
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileDroped in files)
            {
                if (!IsImage(fileDroped))
                    continue;

                string newPath = MoveToImageFolder(fileDroped);
                _images.Add(new string[] { PropertyId.VehicleImage.ToString(), newPath });

                //TODO: should be outside for loop
                _item.SetValue(PropertyId, _images);
            }

            LoadPanel(_item);
        }

    }
}
