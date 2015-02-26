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
                case PropertyId.CustomerAssociatedFiles:
                case PropertyId.PurchaseAssociatedFiles:
                case PropertyId.CustomerAssociatedVehicles:
                case PropertyId.SaleAssociatedFiles:
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
        IsDeleted,

        //User PropertyId's
        Name,
        Password,
        Picture,
        MainTabPages,
        Permissions,
        RegistrationNumber,

        //Customer PropertId's
        FirstName,
        LastName,
        JobTitle,
        BusinessPhone,
        HomePhone,
        EMail,
        Fax,
        MobilePhone,
        HomeStreetNumber,
        HomeStreet,
        HomeUnitNumber,
        HomeCity,
        HomeProvince,
        HomePostalCode,
        DriversLicense,
        LicenseExpiry,
        InsuranceCompany,
        PolicyNumber,
        InsuranceExpiryDate,
        CustomerAssociatedVehicles,
        CustomerAssociatedFiles,
        File,

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
        SaleDate,
        SalePrice,
        SaleCustomerId,
        SaleHst,
        SaleWarrantyCost,
        SaleWarrantyProvider,
        SaleWarrantyMonths,
        SaleWarrantyKMs,
        SaleWarrantyDescription,
        SaleDeliveryDate,
        
        SaleTradeInYear,
        SaleTradeInMake,
        SaleTradeInModel,
        SaleTradeInColour,
        SaleTradeInVIN,
        SaleTradeInMileage,

        SaleLicenseFee,
        SalePayoutLienOnTradeIn,
        SaleBankAdminFee,
        SaleLienRegistrationFee,
        SaleFinanceCost,
        SaleTradeInCost,
        SaleDealerReserve,
        SaleAccessoryCost,
        SaleDepositType,
        SaleBrand,
        SaleSafetyCertificate,
        SaleComments,
        //SaleFees,               // aka ministry license
        SaleTotalDue,             // salefees + saleHST + salePrice
        SaleCustomerPayment,
        SaleAssociatedFiles,
        SaleTaxPercentage,

        PurchasePrice,
        PurchaseBuyerFee,
        PurchaseOtherCosts,
        PurchaseWarrantyCost,
        PurchaseHst,
        PurchaseTotal,
        PurchaseCheckNumber,
        PurchaseDate,
        PurchaseAssociatedFiles,
        PurchaseAccidentReported,
        PurchaseAccidentComment,
        Vendor,
        VendorDescription,
        LicensingCost,
        Profit,                 // salePrice - PurchaseTotal

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
        TaskMinutes,
        TaskVehicleId,

        //Excel specific
        NetDifference,
        NetDifferenceHST,
        CarHST,
        PaymentType,
        SaleDealerReserveHST,
        TotalHST,
        TotalIncome,
        NetIncome,
        TotalFee
    }
}
