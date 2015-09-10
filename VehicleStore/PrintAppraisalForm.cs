using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    class PrintAppraisalForm
    {
        private VehicleAdminObject currVehicle;
        float backgroundXPos = 0;
        float backgroundYPos = -10;
        private int numOfPages = 3;
        private int currentPage = 1;

        public PrintAppraisalForm(VehicleAdminObject vehicle, Object sender, EventArgs e)
        {
            currVehicle = vehicle;

            PrintDocument invoice = new PrintDocument();
            try
            {
                invoice.PrintPage += new PrintPageEventHandler(this.printAppraisalForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintAppraisal: " + ex.Message);
            }

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = invoice;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                invoice.Print();
            }
        }

        private void printAppraisalForm(Object sender, PrintPageEventArgs e)
        {
            printPageOne(e);
            //if (currentPage == 1)
            //{
            //    printPageOne(e);
            //    currentPage++;
            //    e.HasMorePages = true;
            //}
            //else
            //{
            //    //printPageTwo(e);
            //}
        }

        private void printPageOne(PrintPageEventArgs e)
        {
            #region background

            Image background = Image.FromFile(Settings.Resouces + @"\Appraisal.png");
            e.Graphics.DrawImage(background, 10, 0, 800, 1070);

            #endregion

            printAppraisalInfo(e);
            printCustomerInformation(e);
            printVehicleInformation(e);
            printOdometerReadings(e);
        }

        private void printAppraisalInfo(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString(CacheManager.ActiveUser.Name, font, Brushes.Black, backgroundXPos + 130,
                    backgroundYPos + 130);

                string sDate = currVehicle.GetValue(PropertyId.SaleDate);
                if (!string.IsNullOrEmpty(sDate))
                {
                    e.Graphics.DrawString(sDate, font, Brushes.Black, backgroundXPos + 120, backgroundYPos + 100);
                }
            }
        }

        private void printOdometerReadings(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {

                string mileage = currVehicle.GetValue(PropertyId.SaleTradeInMileage);
                if (!string.IsNullOrEmpty(mileage))
                {
                    //Top Box
                    mileage = mileage.Replace(",", string.Empty);
                    int xPos = 180;
                    int spacing = 6 - mileage.Length;
                    xPos = 191 + (21*spacing);

                    foreach (char mileageChar in mileage)
                    {
                        e.Graphics.DrawString(mileageChar.ToString(), font, Brushes.Black, backgroundXPos + xPos,
                            backgroundYPos + 445);
                        xPos += 21;
                    }

                    // Bottom left box
                    spacing = 6 - mileage.Length;
                    xPos = 35 + (28 * spacing);

                    foreach (char mileageChar in mileage)
                    {
                        e.Graphics.DrawString(mileageChar.ToString(), font, Brushes.Black, backgroundXPos + xPos,
                            backgroundYPos + 1012);
                        xPos += 28;
                    }

                    // Bottom right box
                    spacing = 6 - mileage.Length;
                    xPos = 285 + (28 * spacing);

                    foreach (char mileageChar in mileage)
                    {
                        e.Graphics.DrawString(mileageChar.ToString(), font, Brushes.Black, backgroundXPos + xPos,
                            backgroundYPos + 1012);
                        xPos += 28;
                    }
                }

            }
        }

        private void printVehicleInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                //VIN
                string vin = currVehicle.GetValue(PropertyId.SaleTradeInVIN);
                if (string.IsNullOrEmpty(vin))
                    return;

                int xPos = 25;
                foreach (char vinChar in vin)
                {
                    e.Graphics.DrawString(vinChar.ToString(), font, Brushes.Black, backgroundXPos + xPos,
                        backgroundYPos + 275);
                    xPos += 18;
                }

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInYear), font, Brushes.Black, backgroundXPos + 330, backgroundYPos + 275);
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInMake), font, Brushes.Black, backgroundXPos + 400, backgroundYPos + 275);
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInModel), font, Brushes.Black, backgroundXPos + 490, backgroundYPos + 275);
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInColour), font, Brushes.Black, backgroundXPos + 660, backgroundYPos + 275);
            }
        }

        private void printCustomerInformation(PrintPageEventArgs e)
        {
            CustomerCache cache = null;
            Dictionary<CustomerCacheSearchKey, string> searchParam = new Dictionary<CustomerCacheSearchKey, string>();
            if (!string.IsNullOrEmpty(currVehicle.GetValue(PropertyId.SaleCustomerId)))
            {
                searchParam.Add(CustomerCacheSearchKey.Id, currVehicle.GetValue(PropertyId.SaleCustomerId));
                cache = new CustomerCache(searchParam);
            }

            if (cache == null || cache.Count <= 0)
                return;

            using (Font font = new Font("Calibri (Body)", 10))
            {
                CustomerAdminObject customer = cache[0];

                e.Graphics.DrawString(customer.LastName, font, Brushes.Black, backgroundXPos + 40, backgroundYPos + 165);
                e.Graphics.DrawString(customer.FirstName, font, Brushes.Black, backgroundXPos + 435, backgroundYPos + 165);

                string address = "";

                if (string.IsNullOrEmpty(customer.GetValue(PropertyId.HomeUnitNumber)))
                {
                    address = string.Format("{0} {1}",
                    customer.GetValue(PropertyId.HomeStreetNumber),
                    customer.GetValue(PropertyId.HomeStreet));
                }
                else
                {
                    address = string.Format("{0} - {1} {2}",
                        customer.GetValue(PropertyId.HomeUnitNumber),
                        customer.GetValue(PropertyId.HomeStreetNumber),
                        customer.GetValue(PropertyId.HomeStreet));
                }

                e.Graphics.DrawString(address, font, Brushes.Black, backgroundXPos + 40, backgroundYPos + 197);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomeCity), font, Brushes.Black, backgroundXPos + 410, backgroundYPos + 197);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomeProvince), font, Brushes.Black, backgroundXPos + 570, backgroundYPos + 197);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomePostalCode), font, Brushes.Black, backgroundXPos + 700, backgroundYPos + 197);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomePhone), font, Brushes.Black, backgroundXPos + 40, backgroundYPos + 230);

                e.Graphics.DrawString(customer.GetValue(PropertyId.BusinessPhone), font, Brushes.Black, backgroundXPos + 225, backgroundYPos + 230);

                e.Graphics.DrawString(customer.GetValue(PropertyId.MobilePhone), font, Brushes.Black, backgroundXPos + 410, backgroundYPos + 230);
            }

        }
    }
}
