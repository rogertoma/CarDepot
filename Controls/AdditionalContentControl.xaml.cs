using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarDepot.Resources;
using CarDepot.VehicleStore;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace CarDepot.Controls
{
    /// <summary>
    /// Interaction logic for AdditionalContentControl.xaml
    /// </summary>
    public partial class AdditionalContentControl : UserControl, IPropertyPanel
    {
        public delegate void ListChangedEventHandler(List<string[]> files);
        public event ListChangedEventHandler ListChanged;

        private IAdminObject _item = null;
        List<string[]> files = new List<string[]>();
        private bool _isEditable = true;

        public PropertyId PropertyId { set; get; }

        public AdditionalContentControl()
        {
            InitializeComponent();
        }

        public bool IsEditable
        {
            set
            {
                _isEditable = value;
                if (_isEditable)
                {
                    BtnBrowse.Visibility = System.Windows.Visibility.Visible;
                    btnDelete.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    BtnBrowse.Visibility = System.Windows.Visibility.Hidden;
                    btnDelete.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            get { return _isEditable; }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (_item == null)
            {
                return;
            }
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            //openFileDialog1.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            DialogResult userAction = openFileDialog1.ShowDialog();

            foreach (var fileName in openFileDialog1.FileNames)
            {
                MoveToAdditionalFilesFolder(fileName);
            }

            files.Clear();
            foreach (var item in FileList.Items)
            {
                files.Add(new string[] { PropertyId.File.ToString(), item.ToString() });
            }

            _item.SetValue(PropertyId, files);

            // If it's a customer we need to update the list here as well.
            if (ListChanged != null)
            {
                ListChanged(files);
            }
        }

        private void MoveToAdditionalFilesFolder(string origionalFilePath)
        {
            string filePath = new FileInfo(_item.ObjectId).Directory.FullName + "\\" + PropertyId + "\\";
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            FileInfo origionalFile = new FileInfo(origionalFilePath);
            filePath = filePath + origionalFile.Name;

            //string[] allFiles = Directory.GetFiles(filePath);

            if (File.Exists(filePath))
            {
                MessageBoxResult result = MessageBox.Show(Strings.CUSTOMERINFOPAGE_ADDFILERESOURCE_REPLACEFILE, Strings.CUSTOMERINFOPAGE_ADDFILERESOURCE_REPLACEFILE_TITLE, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    return;
            }

            File.Copy(origionalFilePath, filePath, true);
            FileList.Items.Add(origionalFile.Name);
            //return filePath;
        }

        public void LoadPanel(IAdminObject item)
        {
            files.Clear();
            FileList.Items.Clear();

            _item = item;
            List<string[]> items = _item.GetMultiValue(PropertyId);
            if (items == null)
            {
                return;
            }

            foreach (string[] file in items)
            {
                files.Add(file);
                FileList.Items.Add(file[Settings.MultiValueValueIndex]);
            }
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedItem == null) 
                return;

            string selectedFile = FileList.SelectedItem.ToString();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i][1].Contains(selectedFile))
                {
                    files.RemoveAt(i);
                    //File.Delete(new FileInfo(_item.ObjectId).Directory.FullName + Settings.AdditionalFilesFolder + "\\" + selectedFile);
                    FileList.Items.RemoveAt(FileList.SelectedIndex);
                    break;
                }
            }

            _item.SetValue(PropertyId, files);
            // If it's a customer we need to update the list here as well.
            CustomerAdminObject customer = _item as CustomerAdminObject;
            if (customer != null)
            {
                customer.AssociatedFiles = files;
            }
        }

        private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string selectedFile = FileList.SelectedItem.ToString();
            string file = new FileInfo(_item.ObjectId).Directory.FullName + "\\" + PropertyId + "\\" + selectedFile;
            try
            {
                System.Diagnostics.Process.Start(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FileList_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] dropedFiles = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

            foreach (var fileName in dropedFiles)
            {
                MoveToAdditionalFilesFolder(fileName);
            }

            files.Clear();
            foreach (var item in FileList.Items)
            {
                files.Add(new string[] { PropertyId.File.ToString(), item.ToString() });
            }

            _item.SetValue(PropertyId, files);

            // If it's a customer we need to update the list here as well.
            if (ListChanged != null)
            {
                ListChanged(files);
            }
        }

    }
}
