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
        private List<PropertyId> vehicleSaleProperties = new List<PropertyId>();
        private List<PropertyId> vehiclePurchaseProperties = new List<PropertyId>();
        private Dictionary<string, List<VehicleAdminObject>> soldVehicles = new Dictionary<string, List<VehicleAdminObject>>();
        private Dictionary<string, List<VehicleAdminObject>> purchasedVehicles = new Dictionary<string, List<VehicleAdminObject>>();

        public ExportVehicleInfo(VehicleCache vehicles)
        {       
            addProperties();
            bucketSortVehicles(vehicles);
            string file = Resources.Settings.TempFolder + DateTime.Now.ToFileTimeUtc().ToString() + ".xls";
            Workbook wb = new Workbook();
            createExcelFile(wb, file);
        }
        
        private void addProperties()
        {
            vehicleSaleProperties.Add(PropertyId.SaleDate);
            vehicleSaleProperties.Add(PropertyId.Make);
            vehicleSaleProperties.Add(PropertyId.Model);
            vehicleSaleProperties.Add(PropertyId.Year);
            vehicleSaleProperties.Add(PropertyId.SaleCustomerId);
            vehicleSaleProperties.Add(PropertyId.ListPrice);
            vehicleSaleProperties.Add(PropertyId.SalePrice);
            vehicleSaleProperties.Add(PropertyId.SaleHst);
            vehicleSaleProperties.Add(PropertyId.Profit);
            vehicleSaleProperties.Add(PropertyId.Mileage);
            vehicleSaleProperties.Add(PropertyId.StockNumber);
            vehicleSaleProperties.Add(PropertyId.VinNumber);
            vehicleSaleProperties.Add(PropertyId.ModelCode);

            vehiclePurchaseProperties.Add(PropertyId.PurchaseDate);
            vehiclePurchaseProperties.Add(PropertyId.Make);
            vehiclePurchaseProperties.Add(PropertyId.Model);
            vehiclePurchaseProperties.Add(PropertyId.Year);
            vehiclePurchaseProperties.Add(PropertyId.PurchasePrice);
            vehiclePurchaseProperties.Add(PropertyId.PurchaseHst);
            vehiclePurchaseProperties.Add(PropertyId.PurchaseTotal);
            vehiclePurchaseProperties.Add(PropertyId.Mileage);
            vehiclePurchaseProperties.Add(PropertyId.StockNumber);
            vehiclePurchaseProperties.Add(PropertyId.VinNumber);
            vehiclePurchaseProperties.Add(PropertyId.ModelCode);
            return;
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

        private void bucketSortVehicles(VehicleCache vehicles)
        { 
            foreach (var currVehicle in vehicles)
            {
                string monthAndYearString = string.Empty;
                if (isSold(currVehicle))
                {
                    string s = currVehicle.GetValue(PropertyId.SaleDate);
                    DateTime d = DateTime.Parse(s);
                    monthAndYearString = d.Month + "-" + d.Year;
                    updateSoldVehicleList(currVehicle, monthAndYearString);
                }
                else if(isPurchased(currVehicle))
                {
                    string s = currVehicle.GetValue(PropertyId.PurchaseDate);
                    DateTime d = DateTime.Parse(s);
                    monthAndYearString = d.Month + "-" + d.Year;
                    updatePurchasedVehicleList(currVehicle, monthAndYearString);
                }
                else 
                {
                    continue;
                }
            }
            //TODO: Sort Data structure; tough since the keys are strings
        }

        private void populateSoldSheet(Worksheet currSheet, VehicleAdminObject currVehicle, string monthAndYearSold, int currRow) 
        {
            int column = 0;
            foreach (PropertyId p in vehicleSaleProperties)
            {
                if (p.Equals(PropertyId.ListPrice) || p.Equals(PropertyId.SalePrice) || p.Equals(PropertyId.SaleHst))
                {
                    if (currVehicle.GetValue(p) != null && !currVehicle.GetValue(p).Equals(string.Empty))
                    {
                        double value;
                        CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(p), out value);
                        currSheet.Cells[currRow, column] = new Cell(value, "#,##0.00");
                    }
                    else
                    {
                        currSheet.Cells[currRow, column] = new Cell(0, "0.00");
                    }
                }
                else
                {
                    currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
                }
                currSheet.Cells.ColumnWidth[(ushort)column] = 5000;
                column++;      
            }
        }
        
        private void populatePurchasedSheet(Worksheet currSheet, VehicleAdminObject currVehicle, string monthAndYearPurchased, int currRow) 
        {
            int column = 0;
            foreach (PropertyId p in vehiclePurchaseProperties)
            {
                if (p.Equals(PropertyId.ListPrice) || p.Equals(PropertyId.PurchasePrice) || p.Equals(PropertyId.PurchaseHst) || p.Equals(PropertyId.PurchaseTotal))
                {
                    if (currVehicle.GetValue(p) != null && !currVehicle.GetValue(p).Equals(string.Empty))
                    {
                        double value;
                        CarDepot.Resources.Utilities.StringToDouble(currVehicle.GetValue(p), out value);
                        currSheet.Cells[currRow, column] = new Cell(value, "#,##0.00");
                    }
                    else
                    {
                        currSheet.Cells[currRow, column] = new Cell(0, "0.00");
                    }
                }
                else
                {
                    currSheet.Cells[currRow, column] = new Cell(currVehicle.GetValue(p));
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
                populateSoldSheet(currentSheet, v, monthAndYearSold, row);
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
                populatePurchasedSheet(currentSheet, v, monthAndYearPurchased, row);
                row++;
            }
        }

        private void createExcelFile(Workbook wb, string file)
        {
            foreach (string monthYear in soldVehicles.Keys)
            {
                createSoldSheet(wb, monthYear);
            }
            foreach (string monthYear in purchasedVehicles.Keys)
            {
                createPurchasedSheet(wb, monthYear);
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