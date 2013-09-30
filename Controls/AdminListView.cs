﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CarDepot.Controls
{
    class AdminListView: ListView, IPropertyPanel
    {
        private IAdminObject _item = null;
        public PropertyId PropertyId { set; get; }

        public AdminListView()
        {
            ((INotifyCollectionChanged)this.Items).CollectionChanged += AdminListView_CollectionChanged;
        }

        void AdminListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_item == null)
                return;

            _item.SetValue(PropertyId.Tasks, ((VehicleAdminObject)_item).VehicleTasks);
        }

        public void LoadPanel(IAdminObject item)
        {
            _item = item;
            VehicleAdminObject vehicle = item as VehicleAdminObject;
            if (vehicle == null)
                return;

            foreach (VehicleTask task in vehicle.VehicleTasks)
            {
                this.Items.Add(task);
            }
        }

        public void ForceXmlUpdate()
        {
            AdminListView_CollectionChanged(null, null);
        }
    }
}
