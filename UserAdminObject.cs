﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarDepot.Controls;
using CarDepot.Pages;
using CarDepot.VehicleStore;

namespace CarDepot
{
    public class UserAdminObject: AdminObject
    {
        List<string> _mainTabPages = new List<string>(); 
        public string Name { get; set; }
        public string Password { get; set; }
        public string PicturePath { get; set; }
        List<IPropertyPanel> openedPages = new List<IPropertyPanel>(); 

        public UserAdminObject(string objectId)
            : base(objectId)
        {
            
        }

        public List<string> MainTabPages
        {
            set { _mainTabPages = value; }
            get { return _mainTabPages; }
        }

        public void AddPage(IPropertyPanel page)
        {
            openedPages.Add(page);
        }

        public override IAdminItemCache Cache
        {
            set { ;  }
            get { return CacheManager.UserCache; }
        }
    }
}
