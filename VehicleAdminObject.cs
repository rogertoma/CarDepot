using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using CarDepot.VehicleStore;

namespace CarDepot
{
    public class VehicleAdminObject : AdminObject
    {
        private Dictionary<PropertyId, string> _basicInfo = new Dictionary<PropertyId, string>();
        private List<string[]> _images = new List<string[]>();
        private List<VehicleTask> _vehicleTasks2 = new List<VehicleTask>();
        private ObservableCollection<VehicleTask> _vehicleTasks = new ObservableCollection<VehicleTask>();

        public string FileVersion { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Trim { get; set; }
        public string Comments { get; set; }


        public string ListPrice { get; set; }
        public string SalePrice { get; set; }

        public override IAdminItemCache Cache { get; set; }

        public VehicleAdminObject(string objectId)
            : base(objectId)
        {

        }

        public List<string[]> Images
        {
            set { _images = value; }
            get { return _images; }
        }

        public ObservableCollection<VehicleTask> VehicleTasks
        {
            set { _vehicleTasks = value; }
            get { return _vehicleTasks; }
        }

        public Dictionary<PropertyId, string> BasicInfo
        {
            set { _basicInfo = value; }
            get { return _basicInfo; }
        }

        public override void ApplyMultiValue(PropertyId id, string[] value)
        {
            switch (id)
            {
                case PropertyId.Images:
                    _images.Add(value);
                    break;
            }
        }

        public override void ApplyValue(PropertyId id, string value)
        {
            switch (id)
            {
                case PropertyId.FileVersion:
                    SetFileVersion(value);
                    break;
                case PropertyId.Year:
                    Year = value;
                    break;
                case PropertyId.Make:
                    Make = value;
                    break;
                case PropertyId.Model:
                    Model = value;
                    break;
                case PropertyId.Trim:
                    Trim = value;
                    break;
                case PropertyId.Comments:
                    Comments = value;
                    break;
                case PropertyId.ListPrice:
                    ListPrice = value;
                    break;
                case PropertyId.Bodystyle:
                case PropertyId.Engine:
                case PropertyId.Fueltype:
                case PropertyId.Transmission:
                case PropertyId.ExtColor:
                case PropertyId.IntColor:
                case PropertyId.Mileage:
                case PropertyId.StockNumber:
                case PropertyId.ModelCode:
                case PropertyId.VinNumber:
                    if (BasicInfo.ContainsKey(id))
                    {
                        BasicInfo[id] = value;
                    }
                    else
                    {
                        BasicInfo.Add(id, value);
                    }
                    break;
            }
        }

        public override string GetValue(PropertyId id)
        {
            //FieldInfo fi = this.GetType().GetField(id.ToString());
            //string value = (string) fi.GetValue(null);
            //return value;

            switch (id)
            {
                case PropertyId.Year:
                    return Year;
                case PropertyId.Make:
                    return Make;
                case PropertyId.Model:
                    return Model;
                case PropertyId.Trim:
                    return Trim;
                case PropertyId.Comments:
                    return Comments;
                case PropertyId.ListPrice:
                    return ListPrice;
                default:
                    if (_basicInfo.ContainsKey(id))
                    {
                        return _basicInfo[id];
                    }
                    else
                    {
                        return null;
                    }
            }
        }

        public override List<String[]> GetMultiValue(PropertyId id)
        {
            switch (id)
            {
                case PropertyId.Images:
                    return Images;
                default:
                    return null;
            }
        }
    }
}
