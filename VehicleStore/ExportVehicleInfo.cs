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
            PropertyId.Vendor,
            PropertyId.VendorDescription,
            PropertyId.Year,
            PropertyId.Make,
            PropertyId.Model,
            PropertyId.VinNumber,
            PropertyId.PurchasePrice,
            PropertyId.PurchaseBuyerFee,
            PropertyId.PurchaseOtherCosts,
        };
        
        //private List<PropertyId> allCarProperties = new List<PropertyId>() {             
        //    PropertyId.VinNumber,
        //    PropertyId.Year,
        //    PropertyId.Make,
        //    PropertyId.Model,
        //    PropertyId.ExtColor,
        //    PropertyId.Mileage,
        //    PropertyId.PurchasePrice,
        //    PropertyId.PurchaseBuyerFee,
        //    PropertyId.PurchaseOtherCosts,
        //};

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
            double purchasePriceTotal = 0,
                buyerFeeTotal = 0,
                otherCostTotal = 0,
                tasksTotal = 0,
                warrantyCostTotal = 0,
                totalCostTotal = 0,
                hstTotal = 0,
                totalTotal = 0;

            #region Setup Headers
            wb.Worksheets.Add(new Worksheet("Purchased " + monthAndYearPurchased));
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
            currentSheet.Cells[row, column++] = new Cell("Tasks Cost");
            currentSheet.Cells[row, column++] = new Cell("Warranty Cost");
            currentSheet.Cells[row, column++] = new Cell("Other Cost");
            currentSheet.Cells[row, column++] = new Cell("Total Cost");
            currentSheet.Cells[row, column++] = new Cell("Total HST");
            currentSheet.Cells[row, column] = new Cell("Total");

            #endregion

            #region Populate Vehicle Data
            row = 1;
            foreach (VehicleAdminObject vehicle in purchasedVehicles[monthAndYearPurchased])
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
                double tasksCost = CalculateTasksCost(vehicle);
                currentSheet.Cells[row, column++] = new Cell(tasksCost, "#,##0.00");
                tasksTotal += tasksCost;

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

        private void AllPurchasedVehiclesSheet(Workbook wb)
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
            wb.Worksheets.Add(new Worksheet("All Purchased Vehicles"));
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
            currentSheet.Cells[row, column++] = new Cell("Tasks Cost");
            currentSheet.Cells[row, column++] = new Cell("Warranty Cost");
            currentSheet.Cells[row, column++] = new Cell("Other Cost");
            currentSheet.Cells[row, column++] = new Cell("Total Cost");
            currentSheet.Cells[row, column++] = new Cell("Total HST");
            currentSheet.Cells[row, column] = new Cell("Total");

            #endregion

            #region Populate Vehicle Data
            row = 1;
            foreach (VehicleAdminObject vehicle in _vehicleCache)
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
                double tasksCost = CalculateTasksCost(vehicle);
                currentSheet.Cells[row, column++] = new Cell(tasksCost, "#,##0.00");
                tasksTotal += tasksCost;

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
                double totalHst = totalCost * Settings.HST;
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
            currentSheet.Cells[row, column++] = new Cell(tasksTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(warrantyCostTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(otherCostTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(totalCostTotal, "#,##0.00");
            currentSheet.Cells[row, column++] = new Cell(hstTotal, "#,##0.00");
            currentSheet.Cells[row, column] = new Cell(totalTotal, "#,##0.00");

            #endregion
        }

        private void AllSoldVehiclesSheet(Workbook wb)
        {
            double purchasePriceTotal = 0,
                buyerFeeTotal = 0,
                otherCostTotal = 0,
                tasksTotal = 0,
                totalCostTotal = 0,
                hstTotal = 0,
                totalTotal = 0;

            #region Setup Headers
            wb.Worksheets.Add(new Worksheet("All Sold Cars"));
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
                tasksCost = CalculateTasksCost(vehicle);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseOtherCosts), out otherCost);
                Utilities.StringToDouble(vehicle.GetValue(PropertyId.PurchaseWarrantyCost), out warrantyCost);
                purchasePriceIncludingFees = purchasePrice + buyerFee + tasksCost + warrantyCost + otherCost;
                currentSheet.Cells[row, column++] = new Cell(purchasePriceIncludingFees, "#,##0.00");

                //Sale Price
                currentSheet.Cells[row, column++] = new Cell(saleNet, "#,##0.00");

                // Profit
                double profit = 0;
                profit = saleNet - purchasePriceIncludingFees;
                currentSheet.Cells[row, column++] = new Cell(profit, "#,##0.00");

                row++;
            }

            #endregion
        }

        private void createExcelFile(Workbook wb, string currFileName)
        {
            AllPurchasedVehiclesSheet(wb);
            AllSoldVehiclesSheet(wb);

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
            }
        }
    }
}