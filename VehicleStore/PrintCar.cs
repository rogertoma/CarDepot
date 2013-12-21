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
            // Margin settings of paper
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float xPos = leftMargin;
            float yPos = topMargin;

            #region Print the header of the brochure
            using (Font font = new Font("Ariel", 20))
            {
                // Determine the height of a line (based on the font used).
                float lineHeight = font.GetHeight(e.Graphics);
                // Print the Year
                e.Graphics.DrawString(currVehicle.Year, font, Brushes.Black, xPos, yPos);
                // Move cursor over and print the Make
                xPos += (e.Graphics.MeasureString(currVehicle.Year, font).Width);
                e.Graphics.DrawString(currVehicle.Make, font, Brushes.Black, xPos, yPos);
                // Move cursor over and print the Model
                xPos += (e.Graphics.MeasureString(currVehicle.Make, font).Width);
                e.Graphics.DrawString(currVehicle.Model, font, Brushes.Black, xPos, yPos);
                // Move cursor over and print the List Price
                xPos += (e.Graphics.MeasureString(currVehicle.Model,font).Width);
                e.Graphics.DrawString(currVehicle.ListPrice, font, Brushes.Black, xPos, yPos);
                // Reset the x position to the left Margin
                xPos = leftMargin;
                // Move the cursor down the height of one line
                yPos += lineHeight;
            }
            #endregion
            
            #region Print the image
            Image carImage = Image.FromFile(currVehicle.GetMultiValue(PropertyId.Images)[0][1]);
            // Scale raw image to fit within margins
            float printImageWidth = e.MarginBounds.Right - leftMargin;
            float printImageHeight = carImage.Height / 2;
            // Draw an image.
            e.Graphics.DrawImage(carImage, xPos, yPos, printImageWidth, printImageHeight);
            yPos += printImageHeight;
            #endregion
            
            #region Print the rest of the car information
            using (Font font = new Font("Arial", 12))
            {
                float lineHeight = font.GetHeight(e.Graphics);
                yPos += lineHeight;

                foreach (PropertyId id in propertiesToPrint)
                {
                    string infoToPrint = currVehicle.GetValue(id);
                    if (infoToPrint != null && !infoToPrint.Equals(string.Empty))
                    {
                        e.Graphics.DrawString(infoToPrint, font, Brushes.Black, xPos, yPos);
                        yPos += lineHeight;
                    }
                }
            }
            #endregion
        }
    }
}
