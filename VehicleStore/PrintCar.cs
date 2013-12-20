using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Collections.Generic;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    class PrintCar : System.Drawing.Printing.PrintDocument
    {
        private VehicleAdminObject currVehicle;
        
        public PrintCar(VehicleAdminObject vehicle)
            : base()
        {
            currVehicle = vehicle;
        }

        protected override void OnPrintPage(System.Drawing.Printing.PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            using (Font font = new Font("Arial", 12))
            {
                // Margin settings
                float x = e.page.Left;
                float y = e.MarginBounds.Top;

                // Determine the height of a line (based on the font used).
                float lineHeight = font.GetHeight(e.Graphics);
                foreach (PropertyId id in currVehicle.BasicInfo.Keys)
                {
                    e.Graphics.DrawString(currVehicle.BasicInfo[id], font, Brushes.Black, x, y);
                    
                    // Move down the equivalent spacing of one line.
                    y += lineHeight;
                }
                y += lineHeight;
                
                // Draw an image.
                e.Graphics.DrawImage(Image.FromFile(currVehicle.GetMultiValue(PropertyId.Images)[0][1]), x, y)
            }
            //int printHeight;
            //int printWidth;
            //int leftMargin;
            //int rightMargin;
            //Int32 lines;
            //Int32 chars;

            //printHeight = base.DefaultPageSettings.PaperSize.Height - base.DefaultPageSettings.Margins.Top - base.DefaultPageSettings.Margins.Bottom;
            //printWidth = base.DefaultPageSettings.PaperSize.Width - base.DefaultPageSettings.Margins.Left - base.DefaultPageSettings.Margins.Right;
            //leftMargin = base.DefaultPageSettings.Margins.Left;  //X
            //rightMargin = base.DefaultPageSettings.Margins.Top;  //Y
            
            //if (base.DefaultPageSettings.Landscape)
            //{
            //    int temp = printHeight;
            //    printHeight = printWidth;
            //    printWidth = temp;
            //}
        }
    }
}
