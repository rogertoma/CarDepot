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

        //private const string NOTSTARTED = "Not Started";
        //private const string INPROGRESS = "In Progress";
        //private const string COMPLETED = "Completed";

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

        //public static StatusState StringToStatusState(string status)
        //{
        //    switch (status)
        //    {
        //        case NOTSTARTED:
        //            return StatusState.NotStarted;
        //        case COMPLETED:
        //            return StatusState.Completed;
        //        case INPROGRESS:
        //            return StatusState.InProgress;
        //        default:
        //            return StatusState.UnKnown;
        //    }
        //}

        //public static string StatusStateToString(StatusState status)
        //{
        //    switch (status)
        //    {
        //        case StatusState.NotStarted:
        //            return NOTSTARTED;
        //        case StatusState.Completed:
        //            return COMPLETED;
        //        case StatusState.InProgress:
        //            return INPROGRESS;
        //        default:
        //            return null;
        //    }
        //}

        public string Id { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string ChangedBy { get; set; }
        public string ClosedBy { get; set; }
        public string Comments { get; set; }
        public string Cost { get; set; }
        public string AssignedTo { get; set; }
        public string Category { get; set; }
        public string CreatedDate { get; set; }
        public string DueDate { get; set; }
        public string Minutes { get; set; }

        public void ApplyValue(PropertyId id, string value)
        {
            switch (id)
            {
                case PropertyId.TaskName:
                    Id = value;
                    break;
                case PropertyId.TaskStatus:
                    Status = value;
                    break;
                case PropertyId.TaskCreatedBy:
                    CreatedBy = value;
                    break;
                case PropertyId.TaskChangedBy:
                    ChangedBy = value;
                    break;
                case PropertyId.TaskCompletedBy:
                    ClosedBy = value;
                    break;
                case PropertyId.TaskComments:
                    Comments = value;
                    break;
                case PropertyId.TaskCost:
                    Cost = value;
                    break;
                case PropertyId.TaskAssignedTo:
                    AssignedTo = value;
                    break;
                case PropertyId.TaskCategory:
                    Category = value;
                    break;
                case PropertyId.TaskCreatedDate:
                    CreatedDate = value;
                    break;
                case PropertyId.TaskDueDate:
                    DueDate = value;
                    break;
                case PropertyId.TaskMinutes:
                    Minutes = value;
                    break;
            }
        }

        public string GetPropertyIdValue(PropertyId id)
        {
            switch (id)
            {
                case PropertyId.TaskName:
                    return Id;
                case PropertyId.TaskStatus:
                    return Status;
                case PropertyId.TaskCreatedBy:
                    return CreatedBy;
                case PropertyId.TaskChangedBy:
                    return ChangedBy;
                case PropertyId.TaskCompletedBy:
                    return ClosedBy;
                case PropertyId.TaskComments:
                    return Comments;
                case PropertyId.TaskCost:
                    return Cost;
                case PropertyId.TaskAssignedTo:
                    return AssignedTo;
                case PropertyId.TaskCategory:
                    return Category;
                case PropertyId.TaskCreatedDate:
                    return CreatedDate;
                case PropertyId.TaskDueDate:
                    return DueDate;
                case PropertyId.TaskMinutes:
                    return Minutes;
                default:
                    return null;
            }
        }
    }
}
