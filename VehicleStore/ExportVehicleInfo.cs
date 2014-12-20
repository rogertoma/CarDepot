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
        private VehicleCache _vehicleCache = null;
        #region private class datastructures

        private SortedDictionary<string, List<VehicleAdminObject>> soldVehicles = new SortedDictionary<string,List<VehicleAdminObject>>();
        private SortedDictionary<string, List<VehicleAdminObject>> purchasedVehicles = new SortedDictionary<string, List<VehicleAdminObject>>();
        #endregion
        
        public ExportVehicleInfo(VehicleCache vehicles, string fileName)
        {
            _vehicleCache = vehicles;
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

        private void createPurchasedSheet(Workbook wb, string monthAndYearPurchased, List<VehicleAdminObject> vehicles, bool isForAccounting = false)
        {
            double purchasePriceTotal = 0,
                buyerFeeTotal = 0,
                otherCostTotal = 0,
                tasksTotal = 0,
                warrantyCostTotal = 0,
                totalCostTotal = 0,
                hstTotal = 0,
                totalTotal = 0;

            #region Setup Headers

            if (string.IsNullOrEmpty(monthAndYearPurchased))
            {
                if (isForAccounting)
                    wb.Worksheets.Add(new Worksheet("Acc All Purchased Vehicles"));
                else
                    wb.Worksheets.Add(new Worksheet("All Purchased Vehicles"));
            }
            else
            {
                if (isForAccounting)
                    wb.Worksheets.Add(new Worksheet("Acc Purchased " + monthAndYearPurchased));    
                else
                    wb.Worksheets.Add(new Worksheet("Purchased " + monthAndYearPurchased));    
            }
            
            Worksheet currentSheet = wb.Worksheets.Last();
            int row = 0;
            int column = 0;

            currentSheet.Cells[row, column++] = new Cell("Item Number");
            currentSheet.Cells[row, column++] = new Cell("Purchased Date");
            currentSheet.Cells[row, column++] = new Cell("Vendor");
            currentSheet.Cells[row, column++] = new Cell("Vendor Description");
            currentSheet.Cells[row, column++] = new Cell("Year");
            currentSheet.Cells[row, column++] = new Cell("Make");
            currentSheet.Cells[row, column++] = new Cell("Model");
            currentSheet.Cells[row, column++] = new Cell("VIN");
            currentSheet.Cells[row, column++] = new Cell("Purchased Price");
            currentSheet.Cells[row, column++] = new Cell("Buyer Fee");
            if (!isForAccounting)
                currentSheet.Cells[row, column++] = new Cell("Tasks Cost");
            currentSheet.Cells[row, column++] = new Cell("Warranty Cost");
            currentSheet.Cells[row, column++] = new Cell("Other Cost");
            currentSheet.Cells[row, column++] = new Cell("Total Cost");
            currentSheet.Cells[row, column++] = new Cell("Total HST");
            currentSheet.Cells[row, column] = new Cell("Total");

            #endregion

            #region Populate Vehicle Data
            row = 1;
            foreach (VehicleAdminObject vehicle in vehicles)
            {
                column = 0;
                currentSheet.Cells[row, column++] = new Cell(row, "#");

                // Purchased Date
                string pDate = vehicle.GetValue(PropertyId.PurchaseDate);
                DateTime purchaseDate = DateTime.Now;
                if (DateTime.TryParse(pDate, out purchaseDate))
                    currentSheet.Cells[row, column] = new Cell(purchaseDate, CellFormat.Date);
                else
                    currentSheet.Cells[row, column] = new Cell(pDate);
                column++;

                //Vendor
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.Vendor));

                //Vendor Description
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.VendorDescription));

                //Year
                string sYear = vehicle.GetValue(PropertyId.Year);
                double year = 0;
                if (Utilities.StringToDouble(sYear, out year))
                    currentSheet.Cells[row, column] = new Cell(year, "####");
                else
                    currentSheet.Cells[row, column] = new Cell(sYear);
                column++;

                //Make
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.Make));

                //Model
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.Model));

                //Vin
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.VinNumber));

                //Purchase Price
                string sPurchasePrice = vehicle.GetValue(PropertyId.PurchasePrice);
                double purchasePrice = 0;
                if (Utilities.StringToDouble(sPurchasePrice, out purchasePrice))
                    currentSheet.Cells[row, column] = new Cell(purchasePrice, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sPurchasePrice);
                purchasePriceTotal += purchasePrice;
                column++;

                //Buyer Fee
                string sBuyerFee = vehicle.GetValue(PropertyId.PurchaseBuyerFee);
                double buyerFee = 0;
                if (Utilities.StringToDouble(sBuyerFee, out buyerFee))
                    currentSheet.Cells[row, column] = new Cell(buyerFee, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sBuyerFee);
                buyerFeeTotal += buyerFee;
                column++;

                //Tasks Cost
                double tasksCost = 0;
                if (!isForAccounting)
                {
                    tasksCost = CalculateTasksCost(vehicle);
                    currentSheet.Cells[row, column++] = new Cell(tasksCost, "#,##0.00");
                    tasksTotal += tasksCost;
                }

                //Warranty Cost
                string sWarrantyCost = vehicle.GetValue(PropertyId.PurchaseWarrantyCost);
                double warrantyCost = 0;
                if (Utilities.StringToDouble(sWarrantyCost, out warrantyCost))
                    currentSheet.Cells[row, column] = new Cell(warrantyCost, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sWarrantyCost);
                warrantyCostTotal += warrantyCost;
                column++;

                //Other Cost
                string sOtherCost = vehicle.GetValue(PropertyId.PurchaseOtherCosts);
                double otherCost = 0;
                if (Utilities.StringToDouble(sOtherCost, out otherCost))
                    currentSheet.Cells[row, column] = new Cell(otherCost, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sOtherCost);
                otherCostTotal += otherCost;
                column++;

                //Total Cost
                double totalCost = purchasePrice + buyerFee + tasksCost + warrantyCost + otherCost;
                currentSheet.Cells[row, column++] = new Cell(totalCost, "#,##0.00");
                totalCostTotal += totalCost;

                //Total Hst
                double totalHst = totalCost*Settings.HST;
                currentSheet.Cells[row, column++] = new Cell(totalHst, "#,##0.00");
                hstTotal += totalHst;

                //Total
                double total = totalCost + totalHst;
                currentSheet.Cells[row, column++] = new Cell(total, "#,##0.00");
                totalTotal += total;

                row++;
            }

            #endregion

            #region TotalRow

            currentSheet.Cells[row, 0] = new Cell("Total");
            column = 8;
            currentSheet.Cells[row, column++] = new Cell(purchasePriceTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(buyerFeeTotal, "#,##0.00");
            if (!isForAccounting)
                currentSheet.Cells[row, column++] = new Cell(tasksTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(warrantyCostTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(otherCostTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(totalCostTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(hstTotal, "#,##0.00");
            currentSheet.Cells[row, column] = new Cell(totalTotal, "#,##0.00");

            #endregion

        }
        private double CalculateTasksCost(VehicleAdminObject vehicleAdminObject)
        {
            double totalCost = 0;
            foreach (VehicleTask vehicleTask in vehicleAdminObject.VehicleTasks)
            {
                double cost = 0;
                if (Utilities.StringToDouble(vehicleTask.Cost, out cost))
                {
                    totalCost += cost;
                }
            }

            return totalCost;
        }

        private void CreateSoldSheet (Workbook wb, string monthAndYearSold, List<VehicleAdminObject> vehicles, bool isForAccounting = false)
        {
            double purchasePriceTotal = 0,
                buyerFeeTotal = 0,
                otherCostTotal = 0,
                tasksTotal = 0,
                totalCostTotal = 0,
                hstTotal = 0,
                totalTotal = 0;

            #region Setup Headers

            if (string.IsNullOrEmpty(monthAndYearSold))
            {
                if (isForAccounting)
                    wb.Worksheets.Add(new Worksheet("Acc All Sold Cars"));
                else
                    wb.Worksheets.Add(new Worksheet("All All Sold Cars"));   
            }
            else
            {
                if (isForAccounting)
                    wb.Worksheets.Add(new Worksheet("Acc Sold " + monthAndYearSold));
                else
                    wb.Worksheets.Add(new Worksheet("Sold " + monthAndYearSold));    
            }
            
            Worksheet currentSheet = wb.Worksheets.Last();
            int row = 0;
            int column = 0;

            currentSheet.Cells[row, column++] = new Cell("Item Number");
            currentSheet.Cells[row, column++] = new Cell("Sale Date");
            currentSheet.Cells[row, column++] = new Cell("Year");
            currentSheet.Cells[row, column++] = new Cell("Make");
            currentSheet.Cells[row, column++] = new Cell("Model");
            currentSheet.Cells[row, column++] = new Cell("VIN");
            currentSheet.Cells[row, column++] = new Cell("Sale Price");
            currentSheet.Cells[row, column++] = new Cell("Warranty Price");
            currentSheet.Cells[row, column++] = new Cell("Finance Fees");
            currentSheet.Cells[row, column++] = new Cell("Trade In");
            currentSheet.Cells[row, column++] = new Cell("Sale Net");
            currentSheet.Cells[row, column++] = new Cell("Sale HST");
            currentSheet.Cells[row, column++] = new Cell("Ministry Licensing");
            currentSheet.Cells[row, column++] = new Cell("Total Sale Amount");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Dealer Reserve");
            currentSheet.Cells[row, column++] = new Cell("HST on Dealer Reserve");
            currentSheet.Cells[row, column++] = new Cell("Net Dealer Reserve");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Total Income");
            currentSheet.Cells[row, column++] = new Cell("Total HST");
            currentSheet.Cells[row, column++] = new Cell("Net Income");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Purchase Price inc Fees");
            currentSheet.Cells[row, column++] = new Cell("Sale Total");
            currentSheet.Cells[row, column++] = new Cell("Profit");

            #endregion

            #region Populate Vehicle Data
            row = 1;
            foreach (VehicleAdminObject vehicle in _vehicleCache)
            {
                column = 0;
                currentSheet.Cells[row, column++] = new Cell(row, "#");

                // Purchased Date
                string sDate = vehicle.GetValue(PropertyId.SaleDate);
                if (string.IsNullOrEmpty(sDate))
                    continue;
                DateTime saleDate = DateTime.Now;
                if (DateTime.TryParse(sDate, out saleDate))
                    currentSheet.Cells[row, column] = new Cell(saleDate, CellFormat.Date);
                else
                    currentSheet.Cells[row, column] = new Cell(sDate);
                column++;

                //Year
                string sYear = vehicle.GetValue(PropertyId.Year);
                double year = 0;
                if (Utilities.StringToDouble(sYear, out year))
                    currentSheet.Cells[row, column] = new Cell(year, "####");
                else
                    currentSheet.Cells[row, column] = new Cell(sYear);
                column++;

                //Make
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.Make));

                //Model
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.Model));

                //Vin
                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.VinNumber));

                //Sale Price
                string sSalePrice = vehicle.GetValue(PropertyId.SalePrice);
                double salePrice = 0;
                if (Utilities.StringToDouble(sSalePrice, out salePrice))
                    currentSheet.Cells[row, column] = new Cell(salePrice, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sSalePrice);
                column++;

                //Warranty Price
                string sWarrantyPrice = vehicle.GetValue(PropertyId.SaleWarrantyCost);
                double warrantyPrice = 0;
                if (Utilities.StringToDouble(sWarrantyPrice, out warrantyPrice))
                    currentSheet.Cells[row, column] = new Cell(warrantyPrice, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sWarrantyPrice);
                column++;


                //Finance Fees
                string sFinanceFees = vehicle.GetValue(PropertyId.SaleFinanceCost);
                double financeFees = 0;
                if (Utilities.StringToDouble(sFinanceFees, out financeFees))
                    currentSheet.Cells[row, column] = new Cell(financeFees, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sFinanceFees);
                column++;

                //Trade In
                string sTradeIn = vehicle.GetValue(PropertyId.SaleTradeInCost);
                double tradeIn = 0;
                if (Utilities.StringToDouble(sTradeIn, out tradeIn))
                    currentSheet.Cells[row, column] = new Cell(tradeIn, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sTradeIn);
                column++;

                //Sale net
                double saleNet = 0;
                saleNet = salePrice + warrantyPrice + financeFees - tradeIn;
                currentSheet.Cells[row, column++] = new Cell(saleNet, "#,##0.00");

                //Sale HST
                double saleHST = 0;
                saleHST = saleNet*Settings.HST;
                currentSheet.Cells[row, column++] = new Cell(saleHST, "#,##0.00");

                //Ministry Licensing 
                string sLicenseFee = vehicle.GetValue(PropertyId.SaleLicenseFee);
                double licenseFee = 0;
                if (Utilities.StringToDouble(sLicenseFee, out licenseFee))
                    currentSheet.Cells[row, column] = new Cell(licenseFee, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sLicenseFee);
                column++;

                //Total Amount
                double totalSaleAmount = 0;
                totalSaleAmount = saleNet + saleHST + licenseFee;
                currentSheet.Cells[row, column++] = new Cell(totalSaleAmount, "#,##0.00");

                //Empty Column
                column++;

                //Dealer Reserve
                string sDealerReserve = vehicle.GetValue(PropertyId.SaleDealerReserve);
                double dealerReserve = 0;
                if (Utilities.StringToDouble(sDealerReserve, out dealerReserve))
                    currentSheet.Cells[row, column] = new Cell(dealerReserve, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sDealerReserve);
                column++;


                //Dealer Reserve HST
                double dealerReserveHST = 0;
                dealerReserveHST = dealerReserve * Settings.HST;
                currentSheet.Cells[row, column++] = new Cell(dealerReserveHST, "#,##0.00");

                //Net Dealer Reserve
                double netDealerReserve = 0;
                netDealerReserve = dealerReserve + dealerReserveHST;
                currentSheet.Cells[row, column++] = new Cell(netDealerReserve, "#,##0.00");

                // Empty Column
                column++;

                //Total Income
                double totalIncome = 0;
                totalIncome = totalSaleAmount + dealerReserve - licenseFee;
                currentSheet.Cells[row, column++] = new Cell(totalIncome, "#,##0.00");

                //total HST
                double totalHST = 0;
                totalHST = saleHST + dealerReserveHST;
                currentSheet.Cells[row, column++] = new Cell(totalHST, "#,##0.00");

                //net Income
                double netIncome = 0;
                netIncome = totalIncome - totalHST;
                currentSheet.Cells[row, column++] = new Cell(netIncome, "#,##0.00");

                //Empty column
                column++;

                // Purchase Price inc Fees
                double purchasePriceIncludingFees = 0;
                double purchasePrice = 0;
                double buyerFee = 0;
                double tasksCost = 0;
                double otherCost = 0;
                double warrantyCost = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchasePrice), out purchasePrice);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseBuyerFee), out buyerFee);
                if (!isForAccounting)
                    tasksCost = CalculateTasksCost(vehicle);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseOtherCosts), out otherCost);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseWarrantyCost), out warrantyCost);
                purchasePriceIncludingFees = purchasePrice + buyerFee + tasksCost + warrantyCost + otherCost;
                currentSheet.Cells[row, column++] = new Cell(purchasePriceIncludingFees, "#,##0.00");

                //Sale Price
                currentSheet.Cells[row, column++] = new Cell(saleNet + tradeIn, "#,##0.00");
                
                string sLienPayout = vehicle.GetValue(PropertyId.SalePayoutLienOnTradeIn);
                double lienPayout = 0;
                Utilities.StringToDouble(sLienPayout, out lienPayout);

                // Profit
                double profit = 0;
                profit = saleNet - purchasePriceIncludingFees + tradeIn - lienPayout;
                currentSheet.Cells[row, column++] = new Cell(profit, "#,##0.00");

                row++;
            }

            #endregion
        }

        private void createExcelFile(Workbook wb, string currFileName)
        {
            createPurchasedSheet(wb, null, _vehicleCache, true);
            createPurchasedSheet(wb, null, _vehicleCache);

            CreateSoldSheet(wb, null, _vehicleCache, true);
            CreateSoldSheet(wb, null, _vehicleCache);

            foreach (string monthYear in purchasedVehicles.Keys)
            {
                createPurchasedSheet(wb, monthYear, purchasedVehicles[monthYear], true);
                createPurchasedSheet(wb, monthYear, purchasedVehicles[monthYear]);
            }
            foreach (string monthYear in soldVehicles.Keys)
            {
                CreateSoldSheet(wb, monthYear, soldVehicles[monthYear], true);
                CreateSoldSheet(wb, monthYear, soldVehicles[monthYear]);
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
            }
        }
    }
}