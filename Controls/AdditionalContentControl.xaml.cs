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
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace CarDepot.Controls
{
    /// <summary>
    /// Interaction logic for AdditionalContentControl.xaml
    /// </summary>
    public partial class AdditionalContentControl : UserControl, IPropertyPanel
    {
        public delegate void ListChangedEventHandler(object sender, EventArgs e);
        public event ListChangedEventHandler ListChanged;

        private IAdminObject _item = null;
        List<string[]> files = new List<string[]>();

        public PropertyId PropertyId { set; get; }

        public AdditionalContentControl()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
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
            
            foreach (var item in FileList.Items)
            {
                files.Add(new string[] { PropertyId.File.ToString(), item.ToString() });
            }

            _item.SetValue(PropertyId, files);
        }

        private void MoveToAdditionalFilesFolder(string origionalFilePath)
        {
            string filePath = new FileInfo(_item.ObjectId).Directory.FullName + Settings.AdditionalFilesFolder + "\\";
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
            _item = item;
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
        }
    }
}
