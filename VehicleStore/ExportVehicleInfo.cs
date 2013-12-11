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
        private List<PropertyId> vehicleProperties = new List<PropertyId>();
        public ExportVehicleInfo(VehicleCache vehicles)
        {
            addProperties();
            createExcelFile(vehicles[0]);
        }
        private void addProperties()
        {
            vehicleProperties.Add(PropertyId.Make);
            vehicleProperties.Add(PropertyId.Model);
            vehicleProperties.Add(PropertyId.Year);
            vehicleProperties.Add(PropertyId.Mileage);
            vehicleProperties.Add(PropertyId.ListPrice);
            vehicleProperties.Add(PropertyId.SalePrice);
            vehicleProperties.Add(PropertyId.Transmission);
            vehicleProperties.Add(PropertyId.DriveTrain);
            vehicleProperties.Add(PropertyId.Engine);
            vehicleProperties.Add(PropertyId.Fueltype);
            vehicleProperties.Add(PropertyId.Trim);
            vehicleProperties.Add(PropertyId.ExtColor);
            vehicleProperties.Add(PropertyId.IntColor);
            vehicleProperties.Add(PropertyId.Bodystyle);
            vehicleProperties.Add(PropertyId.StockNumber);
            vehicleProperties.Add(PropertyId.VinNumber);
            vehicleProperties.Add(PropertyId.ModelCode);
        }
        private void createExcelFile(VehicleAdminObject currentVehicle)
        {
            string file = Resources.Settings.TempFolder + DateTime.Now.ToFileTimeUtc().ToString() + ".xls";
            Workbook wb = new Workbook();
            wb.Worksheets.Add(new Worksheet("Sheet 1"));
            Worksheet ws = wb.Worksheets.First();
            
            int j = 0;
            int maxCol, maxRow = 0;
            foreach (PropertyId p in vehicleProperties)
            {
                    int i = -1;
                    do
                    {
                        ws.Cells[++i, j] = new Cell(p.ToString());
                        if (p.Equals(PropertyId.ListPrice))
                        {
                            if (currentVehicle.GetValue(p) != null)
                            {
                                decimal price = System.Convert.ToDecimal(currentVehicle.GetValue(p).Substring(1, currentVehicle.GetValue(p).Length - 1));
                                ws.Cells[++i, j] = new Cell(price, "#,##0.00");
                            }
                            else
                            {
                                ws.Cells[++i, j] = new Cell(0.0);
                            }
                        }
                        else
                        {
                            ws.Cells[++i, j] = new Cell(currentVehicle.GetValue(p));
                        }
                        //ws.Cells.ColumnWidth[(ushort)j] = ws.Cells.ColumnWidth.Default;
                        ws.Cells.ColumnWidth[(ushort)j] = 5000;
                    } while (i < 1);
                    maxRow = i;
                    j++;
            }
            maxCol = j;
            // to avoid corrupt excel file message. Create more cells till we have a total of 100.
            if (maxCol * maxRow < 100)
            {
                for (int i = maxCol * maxRow; i < 100; i++)
                {
                    ws.Cells[i, 0] = new Cell("");
                }
            }
            wb.Save(file);
            return;
        }
    }
}
