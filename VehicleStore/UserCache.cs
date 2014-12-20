using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using CarDepot.Pages;
using CarDepot.Resources;


namespace CarDepot.VehicleStore
{
    [PropertyId(PropertyId.Name)]
    [PropertyId(PropertyId.Password)]
    [PropertyId(PropertyId.Picture)]
    [PropertyId(PropertyId.MainTabPages)]
    public class UserCache : List<UserAdminObject>, IAdminItemCache
    {
        public UserAdminObject SystemAdminAccount = null;
        List<string> _properties = new List<string>();

        public UserCache()
        {
            Initialize();

            if (!Directory.Exists(Settings.UserAccountsPath))
            {
                try
                {
                    Directory.CreateDirectory(Settings.UserAccountsPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to create Directory" + Settings.UserAccountsPath);
                    return;
                }   
            }

            string[] users = Directory.GetDirectories(Settings.UserAccountsPath);
            foreach (var user in users)
            {
                foreach (string file in Directory.GetFiles(user, Strings.FILTER_ALL_XML))
                {
                    LoadUser(file);
                }
            }

            SystemAdminAccount = new UserAdminObject("");
            SystemAdminAccount.Name = Strings.SYSTEMADMINACCOUNTNAME;
            SystemAdminAccount.Password = "";
            SystemAdminAccount.MainTabPages.Add("CarDepot.Controls.GeneralControls.ActionsControl");
            //SystemAdminAccount
            this.Add(SystemAdminAccount);
        }

        private void Initialize()
        {
            object[] attributes = GetType().GetCustomAttributes(typeof(PropertyIdAttribute), true);

            for (int index = 0; index < attributes.Length; index++)
            {
                PropertyIdAttribute attribute = (PropertyIdAttribute)attributes[index];
                _properties.Add(attribute.PropertyId.ToString());
            }
        }

        private void ApplyPermissions(UserAdminObject user, string permissions)
        {
            string[] perms = permissions.Split(';');
            foreach (string perm in perms)
            {
                if (!string.IsNullOrEmpty(perm))
                {
                    var type =
                        (UserAdminObject.PermissionTypes) Enum.Parse(typeof (UserAdminObject.PermissionTypes), perm);
                    user.Permissions.Add(type);
                }
            }
        }

        private void LoadUser(string file)
        {
            UserAdminObject user = new UserAdminObject(file);
            XDocument doc = XDocument.Load(file);
            var elements = doc.Descendants();
            foreach (var element in elements)
            {
                PropertyId id;
                try
                {
                    id = (PropertyId)Enum.Parse(typeof(PropertyId), element.Name.LocalName);
                }
                catch (Exception)
                {
                    continue;
                }

                switch (id)
                {
                    case PropertyId.Name:
                        user.Name = element.Value;
                        break;
                    case PropertyId.Password:
                        user.Password = element.Value;
                        break;
                    case PropertyId.MainTabPages:
                        user.MainTabPages.Add(element.Value);
                        break;
                    case PropertyId.Permissions:
                        ApplyPermissions(user, element.Value);
                        break;
                    case PropertyId.RegistrationNumber:
                        user.RegistrationNumer = element.Value;
                        break;
                    case PropertyId.Picture:
                        if (element.Value.StartsWith("\\"))
                        {
                            user.PicturePath = new FileInfo(file).Directory.FullName + element.Value;
                        }
                        else
                        {
                            user.PicturePath = element.Value;
                        }
                        break;
                }
            }
            this.Add(user);
        }

        public event EventHandler<AdminItemCache.RefreshEventArgs> RefreshStarted;
        public event EventHandler<AdminItemCache.RefreshEventArgs> RefreshEnded;
        public event EventHandler<AdminItemCache.UpdateEventArgs> ItemUpdate;
        public void AddItem(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string objectId)
        {
            throw new NotImplementedException();
        }

        public void ExitReadLock()
        {
            throw new NotImplementedException();
        }

        public bool InitialRefreshPerformed { get; private set; }
        public void ModifyItem(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public IAdminObject RefreshItem(IAdminObject item)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string objectId)
        {
            throw new NotImplementedException();
        }

        public IAdminObject this[string objectId]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public List<T> ToList<T>() where T : IAdminObject
        {
            throw new NotImplementedException();
        }

        public bool TryEnterReadLock()
        {
            throw new NotImplementedException();
        }
    }
}

