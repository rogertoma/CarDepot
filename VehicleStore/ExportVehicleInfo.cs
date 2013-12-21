using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using CarDepot.Resources;
using ExcelLibrary.CompoundDocumentFormat;
using ExcelLibrary.SpreadSheet;

namespace CarDepot.VehicleStore
{
    class ExportVehicleInfo
    {
        #region private class datastructures
        private List<PropertyId> vehicleSaleProperties = new List<PropertyId>() {             
            PropertyId.SaleDate,
            PropertyId.Year,
            PropertyId.Make,
            PropertyId.Model,
            PropertyId.VinNumber,
            PropertyId.SalePrice,
            PropertyId.SaleHst,
            PropertyId.SaleFees,
            PropertyId.SaleTotalDue,
            PropertyId.SaleCustomerPayment,
            PropertyId.PurchaseTotal,
            PropertyId.SalePrice,
            PropertyId.Profit
        };
        private List<PropertyId> vehiclePurchaseProperties = new List<PropertyId>() {             
            PropertyId.PurchaseDate,
            PropertyId.Vendor,
            PropertyId.Year,
            PropertyId.Make,
            PropertyId.Model,
            PropertyId.VinNumber,
            PropertyId.PurchasePrice,
            PropertyId.PurchaseHst,
            PropertyId.PurchaseTotal
        };
        private Dictionary<string, List<VehicleAdminObject>> soldVehicles = new Dictionary<string, List<VehicleAdminObject>>();
        private Dictionary<string, List<VehicleAdminObject>> purchasedVehicles = new Dictionary<string, List<VehicleAdminObject>>();
        #endregion
        
        public ExportVehicleInfo(VehicleCache vehicles)
        {       
            bucketSortVehicles(vehicles);
            string file = Resources.Settings.TempFolder + DateTime.Now.ToFileTimeUtc().ToString() + ".xls";
            Workbook wb = new Workbook();
            createExcelFile(wb, file);
        }

        private bool isSold(VehicleAdminObject v)
        {
            string s = v.GetValue(PropertyId.SaleDate);
            return (s == null || s == string.Empty) ? false : true;
        }

        private bool isPurchased(VehicleAdminObject v)
        {
            string p = v.GetValue(PropertyId.PurchaseDate);
            return (p == null || p == string.Empty) ? false : true;
        }

        private void updatePurchasedVehicleList(VehicleAdminObject currVehicle, string monthAndYearPurchased)
        {
            if (purchasedVehicles.ContainsKey(monthAndYearPurchased))
            {
                purchasedVehicles[monthAndYearPurchased].Add(currVehicle);
            }
            else
            {
                List<VehicleAdminObject> newList = new List<VehicleAdminObject> { currVehicle };
                purchasedVehicles.Add(monthAndYearPurchased, newList);
            }
            return;
        }

        private void updateSoldVehicleList(VehicleAdminObject currVehicle, string monthAndYearSold)
        {
            if (soldVehicles.ContainsKey(monthAndYearSold))
            {
                soldVehicles[monthAndYearSold].Add(currVehicle);
            }
            else
            {
                List<VehicleAdminObject> newList = new List<VehicleAdminObject> { currVehicle };
                soldVehicles.Add(monthAndYearSold, newList);
            }
            return;
        }

        private string createDateKey(VehicleAdminObject currVehicle, PropertyId p)
        {
            string monthAndYearKey = string.Empty;
            string dateString = currVehicle.GetValue(p);
            DateTime d = DateTime.Parse(dateString);
            monthAndYearKey = d.Month + "-" + d.Year;
            return monthAndYearKey;
        }

        private void bucketSortVehicles(VehicleCache vehicles)
        { 
            foreach (var currVehicle in vehicles)
            {
                if (isSold(currVehicle))
                {
                    updateSoldVehicleList(currVehicle, createDateKey(currVehicle, PropertyId.SaleDate));
                }
                if(isPurchased(currVehicle))
                {
                    updatePurchasedVehicleList(currVehicle, createDateKey(currVehicle, PropertyId.PurchaseDate));
                }
            }
            //TODO: Sort Data structure; tough since the keys are strings
            return;
        }

        private double calcPurchaseTotal(VehicleAdminObject currVehicle)
        {
            double purchaseHst;
            double purchasePrice;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.PurchasePrice), out purchasePrice);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.PurchaseHst), out purchaseHst);
            return purchasePrice + purchaseHst;
        }

        private double calcSaleTotal(VehicleAdminObject currVehicle)
        {
            double saleFees;
            double salePrice;
            double saleHst;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleFees), out saleFees);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePrice), out salePrice);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleHst), out saleHst);
            return saleFees + salePrice + saleHst;
        }

        private double calcProfit(VehicleAdminObject currVehicle)
        {
            double salePrice;
            double purchaseTotal;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePrice), out salePrice);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.PurchaseTotal), out purchaseTotal);
            return salePrice - purchaseTotal;
        }
        
        private bool isValidValue(VehicleAdminObject currVehicle, PropertyId p)
        {
            return (currVehicle.GetValue(p) != null && !currVehicle.GetValue(p).Equals(string.Empty));
        }
        
        private void applyValueToCell(VehicleAdminObject currVehicle, PropertyId p, Worksheet currSheet, int currRow, int currColumn)
        {
            if (isValidValue(currVehicle, p))
            {
                double value;
                CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(p), out value);
                currSheet.Cells[currRow, currColumn] = new Cell(value, "#,##0.00");
            }
            else
            {
                currSheet.Cells[currRow, currColumn] = new Cell(0, "0.00");
                currVehicle.ApplyValue(p, "0.00");
            }
            return;
        }

        private void populateSoldSheet(Worksheet currSheet, VehicleAdminObject currVehicle, int currRow) 
        {
            int column = 0;
            foreach (PropertyId p in vehicleSaleProperties)
            {
                switch (p)
                {
                    case PropertyId.ListPrice:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SalePrice:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleHst:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleCustomerPayment:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleFees:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleTotalDue:
                        double saleTotal = calcSaleTotal(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(saleTotal, "#,##0.00");
                        currVehicle.ApplyValue(PropertyId.SaleTotalDue,saleTotal.ToString());
                        break;
                    case PropertyId.Profit:
                        double profit = calcProfit(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(profit, "#,##0.00");
                        currVehicle.ApplyValue(PropertyId.Profit, profit.ToString());
                        break;
                    case PropertyId.PurchaseTotal:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    default:
                        currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
                        break;
                }
                column++;
            }
            currSheet.Cells.ColumnWidth[(ushort)column] = 3500;
        }
        
        private void populatePurchasedSheet(Worksheet currSheet, VehicleAdminObject currVehicle, int currRow) 
        {
            int column = 0;
            foreach (PropertyId p in vehiclePurchaseProperties)
            {
                switch (p)
                {
                    case PropertyId.ListPrice:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchasePrice:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchaseHst:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchaseTotal:
                        double purchaseTotal = calcPurchaseTotal(currVehicle);
                        currVehicle.ApplyValue(PropertyId.PurchaseTotal, purchaseTotal.ToString());
                        currSheet.Cells[currRow, column] = new Cell(purchaseTotal, "#,##0.00");
                        break;
                    default:
                        currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
                        break;
                }
                column++;
            }
            currSheet.Cells.ColumnWidth[(ushort)column] = 3500;
        }

        private void createSoldSheet(Workbook wb, string monthAndYearSold)
        {
            wb.Worksheets.Add(new Worksheet("Sold " + monthAndYearSold));
            Worksheet currentSheet = wb.Worksheets.Last();
            int row = 0; 
            for(int column = 0; column < vehicleSaleProperties.Count; column++)
            {
                currentSheet.Cells[row, column] = new Cell(vehicleSaleProperties[column].ToString());
            }
            row = 1;
            foreach(VehicleAdminObject v in soldVehicles[monthAndYearSold])
            {
                populateSoldSheet(currentSheet, v, row);
                row++;
            }
        }

        private void createPurchasedSheet(Workbook wb, string monthAndYearPurchased)
        {
            wb.Worksheets.Add(new Worksheet("Purchased " + monthAndYearPurchased));
            Worksheet currentSheet = wb.Worksheets.Last();
            int row = 0;
            for (int column = 0; column < vehiclePurchaseProperties.Count; column++)
            {
                currentSheet.Cells[row, column] = new Cell(vehiclePurchaseProperties[column].ToString());
            }
            row = 1;
            foreach (VehicleAdminObject v in purchasedVehicles[monthAndYearPurchased])
            {
                populatePurchasedSheet(currentSheet, v, row);
                row++;
            }
        }

        private void createExcelFile(Workbook wb, string file)
        {
            foreach (string monthYear in purchasedVehicles.Keys)
            {
                createPurchasedSheet(wb, monthYear);
            }
            foreach (string monthYear in soldVehicles.Keys)
            {
                createSoldSheet(wb, monthYear);
            }

            int lastSheetNum = wb.Worksheets.Count + 1;
            wb.Worksheets.Add(new Worksheet("Sheet " + lastSheetNum.ToString()));
            Worksheet ws = wb.Worksheets[wb.Worksheets.Count - 1];
            for (int i = 0; i <= 100; i++)
            {
                ws.Cells[i, 0] = new Cell(string.Empty);
            }
            wb.Save(file);
        }
    }
}

//vehicleSaleProperties.Add(PropertyId.CustomerDeposit);
//vehicleSaleProperties.Add(PropertyId.DealerReserve);
//vehicleSaleProperties.Add(PropertyId.FinancialFeeHst);
//vehicleSaleProperties.Add(PropertyId.NetDealerReserve);
//vehicleSaleProperties.Add(PropertyId.TotalIncomeLessMinistryLicense);
//vehicleSaleProperties.Add(PropertyId.TotalHst);
//vehicleSaleProperties.Add(PropertyId.NetIncomeLessMinistryLicense);
                //if (p.Equals(PropertyId.ListPrice) || p.Equals(PropertyId.PurchasePrice) || p.Equals(PropertyId.PurchaseHst))
                //{
                //    if (currVehicle.GetValue(p) != null && !currVehicle.GetValue(p).Equals(string.Empty))
                //    {
                //        double value;
                //        CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(p), out value);
                //        currSheet.Cells[currRow, column] = new Cell(value, "#,##0.00");
                //    }
                //    else
                //    {
                //        currSheet.Cells[currRow, column] = new Cell(0, "0.00");
                //        currVehicle.ApplyValue(p, "0.00");
                //    }
                //}
                //else if (p.Equals(PropertyId.PurchaseTotal))
                //{
                //    double purchaseTotal = calcPurchaseTotal(currVehicle);
                //    currVehicle.ApplyValue(PropertyId.PurchaseTotal, purchaseTotal.ToString());
                //    currSheet.Cells[currRow, column] = new Cell(purchaseTotal, "#,##0.00");
                //}
                //else
                //{
                //    currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
                //}
                //currSheet.Cells.ColumnWidth[(ushort)column] = 2500;
                //column++;
//private double calcCustomerPayments(VehicleAdminObject currVehicle)
//{
//    double totalSaleAmount;
//    double customerDeposit;
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleTotal), out totalSaleAmount);
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.CustomerDeposit), out customerDeposit);
//    return totalSaleAmount - customerDeposit;
//}
//private double calcFinancialFeeHst(VehicleAdminObject currVehicle)
//{
//    double dealerReserve;
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.DealerReserve), out dealerReserve);
//    return dealerReserve / (1.13 * 0.13);
//}
//private double calcNetDealerReserve(VehicleAdminObject currVehicle)
//{
//    double financialFeeHst;
//    double dealerReserve;
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.DealerReserve), out dealerReserve);
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.FinancialFeeHst), out financialFeeHst);
//    return dealerReserve - financialFeeHst;
//}
//private double calctotalIncomeLessMinistryLicense(VehicleAdminObject currVehicle)
//{
//    double totalSaleAmount;
//    double dealerReserve;
//    double ministryLicense;
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.DealerReserve), out dealerReserve);
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleTotal), out totalSaleAmount);
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.MinistryLicense), out ministryLicense);
//    return totalSaleAmount + dealerReserve - ministryLicense;
//}
//private double calcTotalHst(VehicleAdminObject currVehicle)
//{
//    double financialFeeHst;
//    double purchaseHst;
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.FinancialFeeHst), out financialFeeHst);
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.PurchaseHst), out purchaseHst);
//    return financialFeeHst + purchaseHst;
//}
//private double calcnetIncomeLessMinistryLicense(VehicleAdminObject currVehicle)
//{
//    double totalHst;
//    double totalIncomeLessMinistryLicense;
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.TotalHst), out totalHst);
//    CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.TotalIncomeLessMinistryLicense), out totalIncomeLessMinistryLicense);
//    return totalIncomeLessMinistryLicense - totalHst;
//}
//if (p.Equals(PropertyId.ListPrice) || p.Equals(PropertyId.SalePrice) || p.Equals(PropertyId.SaleHst) 
//    || p.Equals(PropertyId.MinistryLicense) || p.Equals(PropertyId.CustomerDeposit) || p.Equals(PropertyId.DealerReserve))
//{
//    if (currVehicle.GetValue(p) != null && !currVehicle.GetValue(p).Equals(string.Empty))
//    {
//        double value;
//        CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(p), out value);
//        currSheet.Cells[currRow, column] = new Cell(value, "#,##0.00");
//    }
//    else
//    {
//        currSheet.Cells[currRow, column] = new Cell(0, "0.00");
//        currVehicle.ApplyValue(p, "0.00");
//    }
//}
//else if (p.Equals(PropertyId.SaleTotal))
//{
//    double totalSale = calcSaleTotal(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(totalSale, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.SaleTotal,totalSale.ToString());
//}
//else if (p.Equals(PropertyId.CustomerPayments))
//{
//    double customerPayments = calcCustomerPayments(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(customerPayments, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.CustomerPayments, customerPayments.ToString());
//}
//else if (p.Equals(PropertyId.FinancialFeeHst))
//{
//    double financialFeeHst = calcFinancialFeeHst(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(financialFeeHst, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.FinancialFeeHst, financialFeeHst.ToString());
//}
//else if (p.Equals(PropertyId.NetDealerReserve))
//{
//    double netDealerReserve = calcNetDealerReserve(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(netDealerReserve, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.NetDealerReserve, netDealerReserve.ToString());
//}
//else if (p.Equals(PropertyId.TotalIncomeLessMinistryLicense))
//{
//    double totalIncomeLessMinistryLicense = calctotalIncomeLessMinistryLicense(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(totalIncomeLessMinistryLicense, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.TotalIncomeLessMinistryLicense, totalIncomeLessMinistryLicense.ToString());
//}
//else if (p.Equals(PropertyId.TotalHst))
//{
//    double totalHst = calcTotalHst(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(totalHst, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.TotalHst, totalHst.ToString());
//}
//else if (p.Equals(PropertyId.NetIncomeLessMinistryLicense))
//{
//    double netIncomeLessMinistryLicense = calcnetIncomeLessMinistryLicense(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(netIncomeLessMinistryLicense, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.NetIncomeLessMinistryLicense, netIncomeLessMinistryLicense.ToString());
//}
//else if (p.Equals(PropertyId.Profit))
//{
//    double profit = calcProfit(currVehicle);
//    currSheet.Cells[currRow, column] = new Cell(profit, "#,##0.00");
//    currVehicle.ApplyValue(PropertyId.Profit, profit.ToString());                   
//}
//else
//{
//    currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
//}
//currSheet.Cells.ColumnWidth[(ushort)column] = 2500;
//column++;      
//}