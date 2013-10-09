using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CarDepot.Resources;

namespace CarDepot
{
    /// <summary>
    /// Base class for all of the Vehicle store "objects" we work with in the Ux.
    /// </summary>
    public abstract class AdminObject : IAdminObject
    {
        private string objectId;
        private XDocument xdoc;

        public AdminObject()
            : base()
        {
        }

        public AdminObject(string objectId)
        {
            this.objectId = objectId;

            if (String.IsNullOrEmpty(objectId))
            {
                return;
            }

            xdoc = XDocument.Load(objectId);
            UpdateObjectWithDataFromFile();
        }

        public string FileVersion
        {
            set;
            get;
        }

        public abstract IAdminItemCache Cache { set; get; }

        /// <summary>
        /// The image to display next to the user
        /// </summary>
        /// <returns>Index of the image</returns>
        protected virtual int GetListViewImageIndex()
        {
            return -1;
        }

        public void UpdateObjectWithDataFromFile()
        {
            XDocument doc = XDocument.Load(objectId);
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

                if (PropertyIdSettings.IsMultiValue(id))
                {
                    ApplyMultiValue(id, element);
                }
                else
                {
                    ApplyValue(id, element.Value);
                }
            }
        }

        #region IAdminObject Members

        /// <summary>
        /// Remove the AdminObject from the owning cache, as well as from the appropriate store.
        /// </summary>
        public virtual void Delete(object sender)
        {
            Cache.RemoveItem(objectId);
        }

        public virtual bool Equals(IAdminObject item)
        {
            if (item == null)
            {
                return false;
            }

            if (item.ObjectId.Equals(objectId))
            {
                return true;
            }

            return false;
        }

        public T GetValue<T>(PropertyId id)
        {
            throw new NotImplementedException();
        }

        public virtual string GetValue(PropertyId id)
        {
            return "value returned";
        }

        public virtual void ApplyValue(PropertyId id, string value)
        {

        }

        public virtual void ApplyMultiValue(PropertyId id, XElement element)
        {

        }

        public virtual List<string[]> GetMultiValue(PropertyId id)
        {
            throw new NotImplementedException();
        }

        public virtual void Initialize()
        {
        }

        public string ObjectId
        {
            get { return objectId; }
        }

        private bool DoesFileVersionAllowSave()
        {
            XDocument latestDoc = XDocument.Load(objectId);
            var elements = from node in latestDoc.Descendants() where node.Name.LocalName == PropertyId.FileVersion.ToString() select node;
            XElement element = elements.FirstOrDefault() as XElement;

            if (element == null)
                return true;

            if (FileVersion.Trim() == element.Value.Trim())
            {
                FileVersion = (int.Parse(FileVersion) + 1).ToString(CultureInfo.InvariantCulture);
                elements = from node in xdoc.Descendants() where node.Name.LocalName == PropertyId.FileVersion.ToString() select node;
                element = elements.FirstOrDefault() as XElement;
                if (element != null)
                {
                    element.Value = FileVersion;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Save (add or update) the item in the appropriate store & cache.
        /// </summary>
        public virtual bool Save(object sender)
        {
            if (!DoesFileVersionAllowSave())
                return false;

            xdoc.Save(objectId);

            System.Diagnostics.Debug.Print("Saving item to policy store");
            System.Diagnostics.Debug.Print("this.Name = {0}", GetValue(PropertyId.Name));

            bool isNew = !Cache.ContainsKey(objectId);
            UpdateData();

            if (isNew)
            {
                System.Diagnostics.Debug.Print("Adding item to cache");
                Cache.AddItem(this);
            }
            else
            {
                System.Diagnostics.Debug.Print("Modifying item in cache");
                Cache.ModifyItem(this);
            }

            return true;
        }

        public void SetValue(PropertyId id, object value)
        {
            var elements = from node in xdoc.Descendants() where node.Name.LocalName == id.ToString() select node;
            XElement element = elements.FirstOrDefault() as XElement;

            if (element != null)
            {
                element.Remove();
            }

            if (PropertyIdSettings.IsMultiValue(id))
            {
                SetMultiValue(id, value);
                return;
            }

            XElement newElement = new XElement(id.ToString());
            newElement.Value = value.ToString();
            xdoc.Root.Add(newElement);

            this.ApplyValue(id, value.ToString());
        }

        public void SetMultiValue(PropertyId id, object value)
        {
            XElement newElement = null;
            switch (id)
            {
                case PropertyId.Tasks:
                    ObservableCollection<VehicleTask> vehicleTasks = (ObservableCollection<VehicleTask>)value;
                    if (vehicleTasks == null)
                        return;

                    newElement = new XElement(id.ToString());
                    foreach (var vehicleTask in vehicleTasks)
                    {
                        XElement task = new XElement(PropertyId.Task.ToString());

                        object[] attributes = vehicleTask.GetType().GetCustomAttributes(typeof(PropertyIdAttribute), true);
                        for (int index = 0; index < attributes.Length; index++)
                        {
                            PropertyIdAttribute attribute = (PropertyIdAttribute)attributes[index];
                            task.Add(new XElement(attribute.PropertyId.ToString(), vehicleTask.GetPropertyIdValue(attribute.PropertyId)));
                        }

                        newElement.Add(task);
                    }

                    xdoc.Root.Add(newElement);
                    break;
                default:
                    List<string[]> strings = (List<string[]>)value;
                    if (value == null)
                        return;

                    newElement = new XElement(id.ToString());

                    foreach (string[] s in strings)
                    {
                        string updated = s[Settings.MultiValueValueIndex].Replace(new FileInfo(objectId).DirectoryName, "");
                        newElement.Add(new XElement(s[Settings.MultiValueKeyIndex], updated));
                    }

                    xdoc.Root.Add(newElement);
                    break;
            }
        }

        /// <summary>
        /// Override in a derived class to perform any operations needed before an object is saved to the store.
        /// </summary>
        public virtual void UpdateData()
        {
        }

        public XDocument XDocument 
        { 
            get { return xdoc; }
        }

        #endregion

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
