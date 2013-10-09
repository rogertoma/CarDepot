using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDepot
{
    [PropertyId(PropertyId.TaskAssignedTo)]
    [PropertyId(PropertyId.TaskCategory)]
    [PropertyId(PropertyId.TaskChangedBy)]
    [PropertyId(PropertyId.TaskComments)]
    [PropertyId(PropertyId.TaskCompletedBy)]
    [PropertyId(PropertyId.TaskCost)]
    [PropertyId(PropertyId.TaskCreatedBy)]
    [PropertyId(PropertyId.TaskName)]
    [PropertyId(PropertyId.TaskStatus)]
    [PropertyId(PropertyId.TaskCreatedDate)]
    [PropertyId(PropertyId.TaskDueDate)]
    [PropertyId(PropertyId.TaskMinutes)]
    public class VehicleTask
    {
        private Dictionary<PropertyId, string> _basicInfo = new Dictionary<PropertyId, string>();
        //private const string NOTSTARTED = "Not Started";
        //private const string INPROGRESS = "In Progress";
        //private const string COMPLETED = "Completed";

        public Dictionary<PropertyId, string> BasicInfo
        {
            get { return _basicInfo; }
            set { _basicInfo = value; }
        }

        public enum StatusTypes
        {
            NotStarted,
            InProgress,
            Completed
        }

        public enum TaskCategoryTypes
        {
            BodyWork,
            Detail,
            Documentation,
            Finance,
            Mechanic,
            Other
        }

        #region SettingProperties

        public string Id
        {
            get { return GetPropertyIdValue(PropertyId.TaskName); } 
            set { ApplyValue(PropertyId.TaskName, value); }
        }
        public string Status
        {
            get { return GetPropertyIdValue(PropertyId.TaskStatus); }
            set { ApplyValue(PropertyId.TaskStatus, value); }
        }
        public string CreatedBy
        {
            get { return GetPropertyIdValue(PropertyId.TaskCreatedBy); }
            set { ApplyValue(PropertyId.TaskCreatedBy, value); }
        }
        public string ChangedBy
        {
            get { return GetPropertyIdValue(PropertyId.TaskChangedBy); }
            set { ApplyValue(PropertyId.TaskChangedBy, value); }
        }
        public string ClosedBy
        {
            get { return GetPropertyIdValue(PropertyId.TaskCompletedBy); }
            set { ApplyValue(PropertyId.TaskCompletedBy, value); }
        }
        public string Comments
        {
            get { return GetPropertyIdValue(PropertyId.TaskComments); }
            set { ApplyValue(PropertyId.TaskComments, value); }
        }
        public string Cost
        {
            get { return GetPropertyIdValue(PropertyId.TaskCost); }
            set { ApplyValue(PropertyId.TaskCost, value); }
        }
        public string AssignedTo
        {
            get { return GetPropertyIdValue(PropertyId.TaskAssignedTo); }
            set { ApplyValue(PropertyId.TaskAssignedTo, value); }
        }
        public string Category
        {
            get { return GetPropertyIdValue(PropertyId.TaskCategory); }
            set { ApplyValue(PropertyId.TaskCategory, value); }
        }
        public string CreatedDate
        {
            get { return GetPropertyIdValue(PropertyId.TaskCreatedDate); }
            set { ApplyValue(PropertyId.TaskCreatedDate, value); }
        }
        public string DueDate
        {
            get { return GetPropertyIdValue(PropertyId.TaskDueDate); }
            set { ApplyValue(PropertyId.TaskDueDate, value); }
        }
        public string Minutes
        {
            get { return GetPropertyIdValue(PropertyId.TaskMinutes); }
            set { ApplyValue(PropertyId.TaskMinutes, value); }
        }

        #endregion

        public void ApplyValue(PropertyId id, string value)
        {
            switch (id)
            {
                default:
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

        public string GetPropertyIdValue(PropertyId id)
        {
            switch (id)
            {
                default:
                    if (BasicInfo.ContainsKey(id))
                        return BasicInfo[id];

                    return null;
            }
        }

        public override bool Equals(object obj)
        {
            VehicleTask task = obj as VehicleTask;
            if (task == null)
                return false;

            Attribute[] attrs = Attribute.GetCustomAttributes(this.GetType());

            return attrs.Cast<PropertyIdAttribute>().All(attribute => this.GetPropertyIdValue(attribute.PropertyId) == task.GetPropertyIdValue(attribute.PropertyId));
        }
    }
}
