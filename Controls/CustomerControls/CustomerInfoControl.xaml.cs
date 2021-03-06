﻿using System;
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
using CarDepot.VehicleStore;

namespace CarDepot.Controls.CustomerControls
{
    /// <summary>
    /// Interaction logic for CustomerInfoControl.xaml
    /// 
    /// </summary>
    public partial class CustomerInfoControl : UserControl, IPropertyPanel
    {
        private bool _isEditable = true;
        CustomerAdminObject _customer = null;
        public CustomerInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPanel(IAdminObject item)
        {
            var customer = item as CustomerAdminObject;
            if (customer == null)
            {
                return;
            }

            _customer = customer;

            LoadAllChildren(ContactCardGrid, item);

            addtionalContentControl.ListChanged += addtionalContentControl_ListChanged;

            Dictionary<VehicleCacheSearchKey, string> searchParam = new Dictionary<VehicleCacheSearchKey, string>();
            searchParam.Add(VehicleCacheSearchKey.CustomerId, item.GetValue(PropertyId.Id));

            VehicleCache cache = new VehicleCache(Settings.VehiclePath, searchParam);
            LstCustomerVehicles.SetContent(cache);
        }

        public void ApplyUiMode()
        {
            throw new NotImplementedException();
        }

        public void ApplyActiveUserPermissions()
        {
            throw new NotImplementedException();
        }

        void addtionalContentControl_ListChanged(List<string[]> files)
        {
            if (_customer != null)
            {
                _customer.AssociatedFiles = files;
            }
        }

        public bool isEditable
        {
            set
            {
                _isEditable = value;
                updateUIToReflectIsEditable();
            }
            get
            {
                return _isEditable;
            }
        }

        private void updateUIToReflectIsEditable()
        {
            foreach (var child in ContactCardGrid.Children)
            {
                AdminLabelTextbox textBox = child as AdminLabelTextbox;
                if (textBox != null)
                {
                    textBox.IsEditable = _isEditable;
                }
            }

            addtionalContentControl.IsEditable = _isEditable;

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

                Panel isPanel = child as Panel;
                if (isPanel != null)
                    LoadAllChildren(isPanel, item);
            }
        }
    }
}
