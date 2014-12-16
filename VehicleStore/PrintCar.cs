using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Collections.Generic;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    class PrintCar
    {
        private VehicleAdminObject currVehicle;
        private List<PropertyId> propertiesToPrint = new List<PropertyId>() {
            PropertyId.Year,
            PropertyId.Make, 
            PropertyId.Model,
            PropertyId.Mileage,
            PropertyId.ListPrice,
            PropertyId.ExtColor,
            PropertyId.DriveTrain,
            PropertyId.Transmission,
            PropertyId.Engine,
            PropertyId.Bodystyle,
            PropertyId.VinNumber
        };

        public PrintCar(VehicleAdminObject vehicle, Object sender, EventArgs e)
        {
            currVehicle = vehicle;
            PrintDocument brochure = new PrintDocument();
            
            try
            {
                brochure.PrintPage += new PrintPageEventHandler(this.printBrochure);
                brochure.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void printBrochure(Object sender, PrintPageEventArgs e)
        {
            #region background
            float backgroundXPos = 50;
            float backgroundYPos = 50;
            Image background = Image.FromFile(Settings.Resouces + @"\poster.png");
            e.Graphics.DrawImage(background, backgroundXPos, backgroundXPos);

            #endregion

            #region printing car info

            int carInfoXPos = 175;

            using (Font font = new Font("Calibri (Body)", 20))
            {
                e.Graphics.DrawString(currVehicle.Year, font, Brushes.Black, backgroundXPos + carInfoXPos, backgroundYPos + 160);
                e.Graphics.DrawString(currVehicle.Make, font, Brushes.Black, backgroundXPos + carInfoXPos, backgroundYPos + 200);
                e.Graphics.DrawString(currVehicle.Model, font, Brushes.Black, backgroundXPos + carInfoXPos, backgroundYPos + 240);
                e.Graphics.DrawString(currVehicle.Transmission, font, Brushes.Black, backgroundXPos + carInfoXPos, backgroundYPos + 282);
                e.Graphics.DrawString(currVehicle.Mileage, font, Brushes.Black, backgroundXPos + carInfoXPos, backgroundYPos + 323);
            }

            #endregion

            #region Price

            using (Font font = new Font("Calibri (Body)", 36, FontStyle.Bold))
            {
                string listPrice = currVehicle.ListPrice;
                if (!listPrice.Contains("$"))
                {
                    listPrice = @"$" + listPrice;
                }
                e.Graphics.DrawString(listPrice, font, Brushes.Black, backgroundXPos + 505, backgroundYPos + 150);
            }

            #endregion

        }
    }
}
