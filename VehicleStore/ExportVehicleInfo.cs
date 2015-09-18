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
using Microsoft.VisualBasic.CompilerServices;

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
            //bucketSortVehicles(vehicles);
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
            {
                currentSheet.Cells[row, column++] = new Cell("Tasks Cost");
                currentSheet.Cells[row, column++] = new Cell("Warranty Cost");
            }
            currentSheet.Cells[row, column++] = new Cell("Other Cost");
            currentSheet.Cells[row, column++] = new Cell("Total Cost");
            currentSheet.Cells[row, column++] = new Cell("Total HST");
            currentSheet.Cells[row, column++] = new Cell("Total");
            currentSheet.Cells[row, column] = new Cell("Cheque Number");

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
                double warrantyCost = 0;
                if (!isForAccounting)
                {
                    tasksCost = CalculateTasksCost(vehicle);
                    currentSheet.Cells[row, column++] = new Cell(tasksCost, "#,##0.00");
                    tasksTotal += tasksCost;

                    //Warranty Cost
                    string sWarrantyCost = vehicle.GetValue(PropertyId.PurchaseWarrantyCost);
                    if (Utilities.StringToDouble(sWarrantyCost, out warrantyCost))
                        currentSheet.Cells[row, column] = new Cell(warrantyCost, "#,##0.00");
                    else
                        currentSheet.Cells[row, column] = new Cell(sWarrantyCost);
                    warrantyCostTotal += warrantyCost;
                    column++;
                }

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

                currentSheet.Cells[row, column++] = new Cell(vehicle.GetValue(PropertyId.PurchaseCheckNumber));

                row++;
            }

            #endregion

            #region TotalRow

            currentSheet.Cells[row, 0] = new Cell("Total");
            column = 8;
            currentSheet.Cells[row, column++] = new Cell(purchasePriceTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(buyerFeeTotal, "#,##0.00");
            if (!isForAccounting)
            {
                currentSheet.Cells[row, column++] = new Cell(tasksTotal, "#,##0.00");
                currentSheet.Cells[row, column++] = new Cell(warrantyCostTotal, "#,##0.00");
            }
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


        private void CreateSoldSheet(Workbook wb, string monthAndYearSold, List<VehicleAdminObject> vehicles)
        {
            #region Setup Headers

            if (string.IsNullOrEmpty(monthAndYearSold))
            {
                wb.Worksheets.Add(new Worksheet("Acc All Sold Cars"));
            }
            else
            {
                wb.Worksheets.Add(new Worksheet("Acc Sold " + monthAndYearSold));
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
            currentSheet.Cells[row, column++] = new Cell("Warranty");
            currentSheet.Cells[row, column++] = new Cell("Finance Fee");
            currentSheet.Cells[row, column++] = new Cell("Accessories");
            currentSheet.Cells[row, column++] = new Cell("Sub total");
            currentSheet.Cells[row, column++] = new Cell("Trade In");
            currentSheet.Cells[row, column++] = new Cell("Net total minus trade in");
            currentSheet.Cells[row, column++] = new Cell("HST Percentage");
            currentSheet.Cells[row, column++] = new Cell("HST");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Net Licence fee");
            currentSheet.Cells[row, column++] = new Cell("Licence fee HST");
            currentSheet.Cells[row, column++] = new Cell("Gross Licence fee");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Net Dealer reserve");
            currentSheet.Cells[row, column++] = new Cell("Dealer Reserve HST");
            currentSheet.Cells[row, column++] = new Cell("Dealer Reserve Gross");

            column++;

            currentSheet.Cells[row, column++] = new Cell("HST on Sales report to HST");
            currentSheet.Cells[row, column++] = new Cell("Total net sales report to HST");
            currentSheet.Cells[row, column++] = new Cell("Go to bank directly Lien fee");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Included HST Total");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Deposit");
            currentSheet.Cells[row, column++] = new Cell("SubTotal2");
            currentSheet.Cells[row, column++] = new Cell("Total Balance");

            #endregion

            #region Populate Vehicle Data
            row = 1;
            foreach (VehicleAdminObject vehicle in _vehicleCache)
            {
                column = 0;
                currentSheet.Cells[row, column++] = new Cell(row, "#");

                // Sale Date
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
                double warranty = 0;
                if (Utilities.StringToDouble(sWarrantyPrice, out warranty))
                    currentSheet.Cells[row, column] = new Cell(warranty, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sWarrantyPrice);
                column++;

                //Finance Fees
                string sFinanceFee = vehicle.GetValue(PropertyId.SaleFinanceCost);
                double finance = 0;
                if (Utilities.StringToDouble(sFinanceFee, out finance))
                    currentSheet.Cells[row, column] = new Cell(finance, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sFinanceFee);
                column++;

                //Accessories
                string sAccessoryPrice = vehicle.GetValue(PropertyId.SaleAccessoryCost);
                double accessories = 0;
                if (Utilities.StringToDouble(sAccessoryPrice, out accessories))
                    currentSheet.Cells[row, column] = new Cell(accessories, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sAccessoryPrice);
                column++;

                //Subtotal
                double subTotal = salePrice + warranty + finance + accessories;
                currentSheet.Cells[row, column] = new Cell(subTotal, "#,##0.00");
                column++;

                //Trade In
                string sTradeIn = vehicle.GetValue(PropertyId.SaleTradeInCost);
                double tradeIn = 0;
                if (Utilities.StringToDouble(sTradeIn, out tradeIn))
                    currentSheet.Cells[row, column] = new Cell(tradeIn, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sTradeIn);
                column++;

                //Net total minus trade in
                double netTotalMinusTradeIn = subTotal - tradeIn;
                currentSheet.Cells[row, column] = new Cell(netTotalMinusTradeIn, "#,##0.00");
                column++;

                //Hst Percentage
                string sSaleHstPercentage = vehicle.GetValue(PropertyId.SaleTaxPercentage);
                double saleHstPercentage = 0;
                Utilities.StringToDouble(sSaleHstPercentage, out saleHstPercentage);
                if (Math.Abs(saleHstPercentage) <= 0)
                {
                    saleHstPercentage = Settings.HST;
                }
                currentSheet.Cells[row, column] = new Cell(saleHstPercentage, "#,##0.00");
                column++;

                //HST
                double hst = 0;
                hst = netTotalMinusTradeIn * saleHstPercentage;
                currentSheet.Cells[row, column] = new Cell(hst, "#,##0.00");
                column++;

                column++;

                //Net Licence Fee
                string sMinistryLicensing = vehicle.GetValue(PropertyId.SaleLicenseFee);
                double grossLicenceFee = 0;
                Utilities.StringToDouble(sMinistryLicensing, out grossLicenceFee);
                double netLicenceFee = grossLicenceFee / (1 + Settings.HST);
                currentSheet.Cells[row, column] = new Cell(netLicenceFee, "#,##0.00");
                column++;

                //Licence Fee HST
                double licenceFeeHst = netLicenceFee*Settings.HST;
                currentSheet.Cells[row, column] = new Cell(licenceFeeHst, "#,##0.00");
                column++;

                //Gross Licence Fee
                currentSheet.Cells[row, column] = new Cell(grossLicenceFee, "#,##0.00");
                column++;

                column++;

                //Net Dealer Reserve
                double dealerReserve = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.SaleDealerReserve), out dealerReserve);
                double netDealerReserve = 0;
                netDealerReserve = dealerReserve / (1 + Settings.HST);
                currentSheet.Cells[row, column] = new Cell(netDealerReserve, "#,##0.00");
                column++;

                //Dealer Reserve HST
                double dealerReserveHST = 0;
                dealerReserveHST = dealerReserve / (1 + Settings.HST) * Settings.HST;
                currentSheet.Cells[row, column] = new Cell(dealerReserveHST, "#,##0.00");
                column++;

                //Gross Dealer reserve
                currentSheet.Cells[row, column] = new Cell(dealerReserve, "#,##0.00");
                column++;

                // Empty Column
                column++;

                //HST on Sales report to HST
                double hstOnSalesReportToHst = hst + licenceFeeHst + dealerReserveHST;
                currentSheet.Cells[row, column] = new Cell(hstOnSalesReportToHst, "#,##0.00");
                column++;

                //Total net sales report to HST
                double totalNetSalesReportToHst = netTotalMinusTradeIn + netLicenceFee + netDealerReserve;
                currentSheet.Cells[row, column] = new Cell(totalNetSalesReportToHst, "#,##0.00");
                column++;

                //Go to bank directly lien fee
                double goToBankDirectlyLienFee = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.SaleLienRegistrationFee), out goToBankDirectlyLienFee);
                currentSheet.Cells[row, column] = new Cell(goToBankDirectlyLienFee, "#,##0.00");
                column++;

                column++;

                double includedHstTotal = hstOnSalesReportToHst + totalNetSalesReportToHst + goToBankDirectlyLienFee;
                currentSheet.Cells[row, column] = new Cell(includedHstTotal, "#,##0.00");
                column++;

                column++;

                // Deposit
                string sDeposit = vehicle.GetValue(PropertyId.SaleCustomerPayment);
                double deposit = 0;
                if (Utilities.StringToDouble(sDeposit, out deposit))
                    currentSheet.Cells[row, column] = new Cell(deposit, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sDeposit);
                column++;

                //Subtotal 2
                double subTotal2 = netTotalMinusTradeIn + hst + grossLicenceFee;
                currentSheet.Cells[row, column] = new Cell(subTotal2, "#,##0.00");
                column++;

                //Total Balannce
                double totalBalance = subTotal2 - deposit + goToBankDirectlyLienFee;
                currentSheet.Cells[row, column] = new Cell(totalBalance, "#,##0.00");
            }

            
            #endregion
        }

        private void CreateSoldSheetNotAccounting (Workbook wb, string monthAndYearSold, List<VehicleAdminObject> vehicles)
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
                wb.Worksheets.Add(new Worksheet("All Sold Cars"));   
            }
            else
            {
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
            currentSheet.Cells[row, column++] = new Cell("Finance Fees");
            currentSheet.Cells[row, column++] = new Cell("Trade In");
            currentSheet.Cells[row, column++] = new Cell("Warranty");
            currentSheet.Cells[row, column++] = new Cell("Accessories");
            currentSheet.Cells[row, column++] = new Cell("Sale Net");
            currentSheet.Cells[row, column++] = new Cell("Sale HST Percentage");
            currentSheet.Cells[row, column++] = new Cell("Sale HST");
            currentSheet.Cells[row, column++] = new Cell("Lien Fee");
            currentSheet.Cells[row, column++] = new Cell("Gross Ministry Licensing");
            currentSheet.Cells[row, column++] = new Cell("SubTotal");
            currentSheet.Cells[row, column++] = new Cell("Ministry Licensing HST");
            currentSheet.Cells[row, column++] = new Cell("Net Ministry Licensing");
            currentSheet.Cells[row, column++] = new Cell("Deposit");
            currentSheet.Cells[row, column++] = new Cell("Total Sale Amount");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Dealer Reserve Gross");
            currentSheet.Cells[row, column++] = new Cell("Dealer Reserve HST");
            currentSheet.Cells[row, column++] = new Cell("Net Dealer Reserve");

            column++;

            currentSheet.Cells[row, column++] = new Cell("Total Net");
            currentSheet.Cells[row, column++] = new Cell("Total HST");


            column++;
            currentSheet.Cells[row, column++] = new Cell("Net Purchase cost");
            currentSheet.Cells[row, column++] = new Cell("Net Income");
            currentSheet.Cells[row, column++] = new Cell("Net Profit");
            

            #endregion

            #region Populate Vehicle Data
            row = 1;
            foreach (VehicleAdminObject vehicle in _vehicleCache)
            {
                column = 0;
                currentSheet.Cells[row, column++] = new Cell(row, "#");

                // Sale Date
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

                //Finance Fees
                string sFinanceFee = vehicle.GetValue(PropertyId.SaleFinanceCost);
                double financeFee = 0;
                if (Utilities.StringToDouble(sFinanceFee, out financeFee))
                    currentSheet.Cells[row, column] = new Cell(financeFee, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sFinanceFee);
                column++;

                //Trade In
                string sTradeIn = vehicle.GetValue(PropertyId.SaleTradeInCost);
                double tradeIn = 0;
                if (Utilities.StringToDouble(sTradeIn, out tradeIn))
                    currentSheet.Cells[row, column] = new Cell(tradeIn, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sTradeIn);
                column++;

                //Warranty Price
                string sWarrantyPrice = vehicle.GetValue(PropertyId.SaleWarrantyCost);
                double warrantyPrice = 0;
                if (Utilities.StringToDouble(sWarrantyPrice, out warrantyPrice))
                    currentSheet.Cells[row, column] = new Cell(warrantyPrice, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sWarrantyPrice);
                column++;

                //Accessories
                string sAccessoryPrice = vehicle.GetValue(PropertyId.SaleAccessoryCost);
                double accessoryPrice = 0;
                if (Utilities.StringToDouble(sAccessoryPrice, out accessoryPrice))
                    currentSheet.Cells[row, column] = new Cell(accessoryPrice, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sAccessoryPrice);
                column++;

                //Sale net
                double saleNet = 0;
                saleNet = salePrice + financeFee + warrantyPrice + accessoryPrice - tradeIn;
                currentSheet.Cells[row, column++] = new Cell(saleNet, "#,##0.00");

                //Sale Hst Percentage
                string sSaleHstPercentage = vehicle.GetValue(PropertyId.SaleTaxPercentage);
                double saleHstPercentage = 0;
                Utilities.StringToDouble(sSaleHstPercentage, out saleHstPercentage);
                if (Math.Abs(saleHstPercentage) <= 0)
                {
                    saleHstPercentage = Settings.HST;
                }
                currentSheet.Cells[row, column] = new Cell(saleHstPercentage, "#,##0.00");
                column++;

                //Sale HST
                double saleHST = 0;
                saleHST = saleNet * saleHstPercentage;
                currentSheet.Cells[row, column++] = new Cell(saleHST, "#,##0.00");

                //Lien Fee
                string sLienFee = vehicle.GetValue(PropertyId.SaleLienRegistrationFee);
                double lienFee = 0;
                if (Utilities.StringToDouble(sLienFee, out lienFee))
                    currentSheet.Cells[row, column] = new Cell(lienFee, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sLienFee);
                column++;

                //Ministry Fees
                string sMinistryLicensing = vehicle.GetValue(PropertyId.SaleLicenseFee);
                double ministryLicensing = 0;
                Utilities.StringToDouble(sMinistryLicensing, out ministryLicensing);
                double lienRegistrationFee = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.SaleLienRegistrationFee), out lienRegistrationFee);
                double bankAdminFee = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.SaleBankAdminFee), out bankAdminFee);
                double totalMinistryCost = ministryLicensing + bankAdminFee;

                currentSheet.Cells[row, column++] = new Cell(totalMinistryCost, "#,##0.00");

                //Subtotal
                double subTotal = 0;
                subTotal = saleNet + saleHST + totalMinistryCost;
                currentSheet.Cells[row, column++] = new Cell(subTotal, "#,##0.00");

                //Ministry HST
                double ministryHST = 0;
                ministryHST = totalMinistryCost / (1 + Settings.HST) * Settings.HST;
                currentSheet.Cells[row, column++] = new Cell(ministryHST, "#,##0.00");

                //Ministry NET
                double ministryNET = 0;
                ministryNET = totalMinistryCost / (1 + Settings.HST);
                currentSheet.Cells[row, column++] = new Cell(ministryNET, "#,##0.00");

                // Deposit
                string sDeposit = vehicle.GetValue(PropertyId.SaleCustomerPayment);
                double deposit = 0;
                if (Utilities.StringToDouble(sDeposit, out deposit))
                    currentSheet.Cells[row, column] = new Cell(deposit, "#,##0.00");
                else
                    currentSheet.Cells[row, column] = new Cell(sDeposit);
                column++;

                //Total Sale Amount
                double totalSaleAmount = 0;
                totalSaleAmount = subTotal + lienFee - deposit;
                currentSheet.Cells[row, column++] = new Cell(totalSaleAmount, "#,##0.00");

                //Empty Column
                column++;

                //Dealer Reserve
                double dealerReserve = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.SaleDealerReserve), out dealerReserve);

                currentSheet.Cells[row, column++] = new Cell(dealerReserve, "#,##0.00");


                //Dealer Reserve HST
                double dealerReserveHST = 0;
                dealerReserveHST = dealerReserve / (1 + Settings.HST) * Settings.HST;
                currentSheet.Cells[row, column++] = new Cell(dealerReserveHST, "#,##0.00");

                //Net Dealer Reserve
                double netDealerReserve = 0;
                netDealerReserve = dealerReserve / (1 + Settings.HST);
                currentSheet.Cells[row, column++] = new Cell(netDealerReserve, "#,##0.00");

                // Empty Column
                column++;

                double totalNet = 0;
                totalNet = saleNet + ministryNET + netDealerReserve;
                currentSheet.Cells[row, column++] = new Cell(totalNet, "#,##0.00");

                double totalHST = 0;
                totalHST = saleHST + dealerReserveHST + ministryHST;
                currentSheet.Cells[row, column++] = new Cell(totalHST, "#,##0.00");


                column++;

                // Purchase Price inc Fees
                double purchasePriceIncludingFees = 0;
                double purchasePrice = 0;
                double buyerFee = 0;
                double tasksCost = 0;
                double otherCost = 0;
                double warrantyCost = 0;
                double ministryLicensingCost = 0;
                double lienPayoutAmount = 0;
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchasePrice), out purchasePrice);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseBuyerFee), out buyerFee);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.LicensingCost), out ministryLicensingCost);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.SalePayoutLienOnTradeIn), out lienPayoutAmount);
                tasksCost = CalculateTasksCost(vehicle);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseOtherCosts), out otherCost);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseWarrantyCost), out warrantyCost);
                purchasePriceIncludingFees = purchasePrice + buyerFee + tasksCost + warrantyCost + otherCost + ministryLicensingCost;
                currentSheet.Cells[row, column++] = new Cell(purchasePriceIncludingFees, "#,##0.00");

                    
                // Net Income
                double netIncome = 0;
                netIncome = saleNet + ministryLicensing + netDealerReserve + (tradeIn - lienPayoutAmount);
                currentSheet.Cells[row, column++] = new Cell(netIncome, "#,##0.00");

                // Net Profit
                double netProfit = netIncome - purchasePriceIncludingFees;
                currentSheet.Cells[row, column++] = new Cell(netProfit, "#,##0.00");
                

                row++;
            }

            #endregion
        }

        private void createExcelFile(Workbook wb, string currFileName)
        {
            createPurchasedSheet(wb, null, _vehicleCache, true);
            createPurchasedSheet(wb, null, _vehicleCache);

            CreateSoldSheet(wb, null, _vehicleCache);
            CreateSoldSheetNotAccounting(wb, null, _vehicleCache);

            /*
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
            */
            
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