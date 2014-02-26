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
            PropertyId.CarHST,
            PropertyId.SaleHst,
            PropertyId.PurchasePrice,
            PropertyId.PurchaseBuyerFee,
            PropertyId.PurchaseTotal,
            PropertyId.SaleTradeInCost,
            PropertyId.SaleWarrantyCost,
            PropertyId.SaleLicenseFee,
            PropertyId.SaleLienRegistrationFee,
            PropertyId.SaleFinanceCost,
            PropertyId.SaleTotalDue,
            PropertyId.SaleCustomerPayment,
            PropertyId.NetDifference,
            PropertyId.NetDifferenceHST,
            PropertyId.TotalFee,
            PropertyId.PaymentType,
            PropertyId.SaleDealerReserve,
            PropertyId.SaleDealerReserveHST,
            PropertyId.TotalHST,
            PropertyId.TotalIncome,
            PropertyId.NetIncome,
            PropertyId.Profit
        };
        private List<PropertyId> vehiclePurchaseProperties = new List<PropertyId>() {             
            PropertyId.PurchaseDate,
            PropertyId.PurchaseBuyerFee,
            PropertyId.Vendor,
            PropertyId.Year,
            PropertyId.Make,
            PropertyId.Model,
            PropertyId.VinNumber,
            PropertyId.PurchasePrice,
            PropertyId.PurchaseHst,
            PropertyId.PurchaseTotal
        };
        private SortedDictionary<string, List<VehicleAdminObject>> soldVehicles = new SortedDictionary<string,List<VehicleAdminObject>>();
        private SortedDictionary<string, List<VehicleAdminObject>> purchasedVehicles = new SortedDictionary<string, List<VehicleAdminObject>>();
        #endregion
        
        public ExportVehicleInfo(VehicleCache vehicles, string fileName)
        {       
            bucketSortVehicles(vehicles);
            //string file = Resources.Settings.TempFolder + DateTime.Now.ToFileTimeUtc().ToString() + ".xls";
            string currFileName = fileName;
            Workbook wb = new Workbook();
            createExcelFile(wb, currFileName);
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
            double salePrice;
            double saleWarrantyCost;
            double saleFinanceFees;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleWarrantyCost), out saleWarrantyCost);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePrice), out salePrice);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleFinanceCost), out saleFinanceFees);
            return salePrice + saleWarrantyCost + saleFinanceFees;
        }

        private double calcProfit(VehicleAdminObject currVehicle)
        {
            double salePrice;
            double purchaseTotal;
            double buyerFee;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePrice), out salePrice);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.PurchaseTotal), out purchaseTotal);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.PurchaseBuyerFee), out buyerFee);
            return salePrice - purchaseTotal - buyerFee;
        }

        private double calcNetDifference(VehicleAdminObject currVehicle)
        {
            double subTotal;
            double tradeIn;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleTotalDue), out subTotal);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleTradeInCost), out tradeIn);
            return subTotal - tradeIn;
        }

        private double calcNetDifferenceHST(VehicleAdminObject currVehicle)
        {
            double netDiff;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.NetDifference), out netDiff);
            return netDiff * 0.13;
        }

        private double calcTotalFee(VehicleAdminObject currVehicle)
        {
            double netDiff;
            double netDiffHST;
            double licenseFee;
            double lienRegFee;
                CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.NetDifference), out netDiff);
                CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.NetDifferenceHST), out netDiffHST);
                CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleLicenseFee), out licenseFee);
                CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleLienRegistrationFee), out lienRegFee);
            return netDiff + netDiffHST + licenseFee + lienRegFee;
        }

        private double calcTotalHST(VehicleAdminObject currVehicle)
        {
            double carHST;
            double dealerReserveHST;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleDealerReserveHST), out dealerReserveHST);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.CarHST), out carHST);
            return carHST + dealerReserveHST;
        }

        private double calcCarHST(VehicleAdminObject currVehicle)
        {
            double salePrice;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePrice), out salePrice);
            return salePrice * 0.13;
        }

        private double calcTotalIncome(VehicleAdminObject currVehicle)
        {
            double netDifference;
            double totalHST;
            double dealerReserve;
            
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleDealerReserve), out dealerReserve);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.NetDifference), out netDifference);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.TotalHST), out totalHST);

            return netDifference + totalHST + dealerReserve;
        }

        private double calcNetIncome(VehicleAdminObject currVehicle)
        {
            double totalIncome;
            double totalHST;
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.TotalIncome), out totalIncome);
            CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(PropertyId.TotalHST), out totalHST);
            return totalIncome - totalHST;
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
                    case PropertyId.PurchasePrice:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchaseBuyerFee:
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
                    case PropertyId.PaymentType:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleLicenseFee:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleTradeInCost:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchaseTotal:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleWarrantyCost:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleLienRegistrationFee:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleFinanceCost:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleDealerReserve:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.SaleDealerReserveHST:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.CarHST:
                        double carHST = calcCarHST(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(carHST, "#,##0.00");
                        break;
                    case PropertyId.TotalHST:
                        double totalHST = calcTotalHST(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(totalHST, "#,##0.00");
                        break;
                    case PropertyId.NetDifference:
                        double NetDiff = calcNetDifference(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(NetDiff, "#,##0.00");
                        break;
                    case PropertyId.NetDifferenceHST:
                        double NetDiffHST = calcNetDifferenceHST(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(NetDiffHST, "#,##0.00");
                        break;
                    case PropertyId.TotalFee:
                        double totalFee = calcTotalFee(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(totalFee, "#,##0.00");
                        break;
                    case PropertyId.SaleTotalDue:
                        double saleTotal = calcSaleTotal(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(saleTotal, "#,##0.00");
                        break;
                    case PropertyId.TotalIncome:
                        double totalIncome = calcTotalIncome(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(totalIncome, "#,##0.00");
                        break;
                    case PropertyId.NetIncome:
                        double netIncome = calcNetIncome(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(netIncome, "#,##0.00");
                        break;
                    case PropertyId.Profit:
                        double profit = calcProfit(currVehicle);
                        currSheet.Cells[currRow, column] = new Cell(profit, "#,##0.00");
                        break;
                    default:
                        currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
                        break;
                }
                currSheet.Cells.ColumnWidth[(ushort)column] = 5000;
                column++;
            } 
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
                    case PropertyId.PurchaseBuyerFee:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchaseHst:
                        applyValueToCell(currVehicle, p, currSheet, currRow, column);
                        break;
                    case PropertyId.PurchaseTotal:
                        double purchaseTotal = calcPurchaseTotal(currVehicle);
                        //currVehicle.ApplyValue(PropertyId.PurchaseTotal, purchaseTotal.ToString());
                        currSheet.Cells[currRow, column] = new Cell(purchaseTotal, "#,##0.00");
                        break;
                    default:
                        currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
                        break;
                }
                currSheet.Cells.ColumnWidth[(ushort)column] = 5000;
                column++;
            }
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

        private void createExcelFile(Workbook wb, string currFileName)
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
            try
            {
                wb.Save(currFileName);
            }
            catch (Exception e)
            {
                MessageBox.Show("Looks like this file is already open! Try a different file name or close the open spreadsheet.");
                return;
            }
        }
    }
}