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
        Dictionary<string, int> monthToWorksheet = new Dictionary<string, int>();

        private List<PropertyId> vehicleProperties = new List<PropertyId>();
        public ExportVehicleInfo(VehicleCache vehicles)
        {
            addProperties();

            Dictionary<string, List<VehicleAdminObject>> monthList = bucketSort(vehicles);
            createExcelFile(monthList);
        }
        private Dictionary<string, List<VehicleAdminObject>> bucketSort(VehicleCache vehicles)
        {
            Dictionary<string, List<VehicleAdminObject>> sortedVehicles = new Dictionary<string, List<VehicleAdminObject>>();

            foreach (var v in vehicles)
            {
                string s = v.GetValue(PropertyId.SaleDate);
                DateTime d = DateTime.Parse(s);
                string excelMonthString = d.Month + "-" + d.Year;
                if (sortedVehicles.ContainsKey(excelMonthString))
                {
                    sortedVehicles[excelMonthString].Add(v);
                }
                else
                {
                    List<VehicleAdminObject> newList = new List<VehicleAdminObject> { v };
                    sortedVehicles.Add(excelMonthString, newList);
                }
            }

            //if (vehicles != null)
            //{
            //    for (int i = 1; i <= 12; i++)
            //    {
            //        sortedVehicles.Add(i, new List<VehicleAdminObject>());
            //    }
            //    foreach (var v in vehicles)
            //    {
            //        string s = v.GetValue(PropertyId.SaleDate);
            //        DateTime d = DateTime.Parse(s);
            //        sortedVehicles[d.Month].Add(v);
            //    }
            //}
            return sortedVehicles;
        }
        private void addProperties()
        {
            vehicleProperties.Add(PropertyId.SaleDate);
            vehicleProperties.Add(PropertyId.Make);
            vehicleProperties.Add(PropertyId.Model);
            vehicleProperties.Add(PropertyId.Year);
            vehicleProperties.Add(PropertyId.Mileage);
            vehicleProperties.Add(PropertyId.ListPrice);
            vehicleProperties.Add(PropertyId.SalePrice);
            vehicleProperties.Add(PropertyId.SaleHst);
            vehicleProperties.Add(PropertyId.StockNumber);
            vehicleProperties.Add(PropertyId.VinNumber);
            vehicleProperties.Add(PropertyId.ModelCode);
        }
        private void createExcelFile(Dictionary<string, List<VehicleAdminObject>> vehicles)
        {
            string file = Resources.Settings.TempFolder + DateTime.Now.ToFileTimeUtc().ToString() + ".xls";
            Workbook wb = new Workbook();

            //wb.Worksheets.Add(new Worksheet(vehicles.GetValue(PropertyId.Trim)));
            //Worksheet ws = wb.Worksheets.First();
            foreach (string monthYear in vehicles.Keys)
            {

                    int row = 0;
                    int column = 0;

                    monthToWorksheet.Add(monthYear, wb.Worksheets.Count);
                    wb.Worksheets.Add(new Worksheet(monthYear));
                    Worksheet ws = wb.Worksheets[monthToWorksheet[monthYear]];

                    // to avoid corrupt excel file message. Create more cells till we have a total of 100.
                    for (int i = 0; i < 100; i++)
                    {
                        ws.Cells[i, 0] = new Cell("");
                    }

                    for (; column < vehicleProperties.Count; column++)
                    {
                        ws.Cells[row, column] = new Cell(vehicleProperties[column].ToString());
                    }

                    row = 1;
                    foreach (VehicleAdminObject v in vehicles[monthYear])
                    {
                        column = 0;

                        foreach (PropertyId p in vehicleProperties)
                        {
                            if (p.Equals(PropertyId.ListPrice))
                            {
                                if (v.GetValue(p) != null || (v.GetValue(p).CompareTo("") != 0))
                                {
                                    //decimal price = System.Convert.ToDecimal(currentVehicle.GetValue(p).Substring(1, currentVehicle.GetValue(p).Length - 1));
                                    double price;
                                    CarDepot.Resources.Utilities.StringToDouble(v.GetValue(p), out price);
                                    ws.Cells[row, column] = new Cell(price);
                                    //ws.Cells[++i, j] = new Cell(price, "#,##0.00");
                                }
                                else
                                {
                                    ws.Cells[row, column] = new Cell(0.0);
                                }
                            }
                            else
                            {
                                ws.Cells[row, column] = new Cell(v.GetValue(p));
                            }
                            ws.Cells.ColumnWidth[(ushort)column] = 5000;

                            column++;
                        }
                        row++;
                        
                    }
                     
                    wb.Save(file);
                
            }
        }
    }
}
