using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDepot
{
    public static class PropertyIdSettings
    {
        public static bool IsMultiValue (PropertyId propertyId)
        {
            switch (propertyId)
            {
                case PropertyId.Images:
                case PropertyId.Tasks:
                    return true;
                default:
                    return false;
            }
        }
    }


    public enum PropertyId
    {
        //Global
        FileVersion,
        Id,

        //User PropertyId's
        Name,
        Password,
        Picture,
        MainTabPages,

        //Customer PropertId's
        FirstName,
        LastName,
        JobTitle,
        BusinessPhone,
        HomePhone,
        Fax,
        MobilePhone,
        HomeAddress,
        BusinessAddress,
        CustomerAssociatedVehicles,
        CustomerAssociatedFiles,

        //Car PropertyId's
        Year,
        Make,
        Model,
        Trim,
        Comments,
        VehicleImage,
        Images,
        Bodystyle,
        DriveTrain,
        Engine,
        Fueltype,
        Transmission,
        ExtColor,
        IntColor,
        Mileage,
        StockNumber,
        ModelCode,
        VinNumber,
        ListPrice,
        SalePrice,

        //Task PropertyId's
        Task,
        Tasks,
        TaskName,
        TaskStatus,
        TaskComments,
        TaskCreatedBy,
        TaskChangedBy,
        TaskCost,
        TaskCompletedBy,
        TaskAssignedTo,
        TaskCategory,
        TaskCreatedDate,
        TaskDueDate,
        TaskMinutes
    }
}
