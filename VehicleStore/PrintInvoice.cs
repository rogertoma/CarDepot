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
using CarDepot.Controls.VehicleControls;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    class PrintInvoice
    {
        private VehicleAdminObject currVehicle;
        float backgroundXPos = 5;
        float backgroundYPos = 10;
        private int numOfPages = 3;
        private int currentPage = 1;

        public PrintInvoice(VehicleAdminObject vehicle, System.Windows.Controls.PrintDialog dialog, Object sender, EventArgs e)
        {
            currVehicle = vehicle;
            PrintDocument invoice = new PrintDocument();
            try
            {
                invoice.PrintPage += new PrintPageEventHandler(this.printInvoice);
                invoice.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintInvoice: " + ex.Message);
            }
        }

        private void printInvoice(Object sender, PrintPageEventArgs e)
        {
            if (currentPage == 1)
            {
                printPageOne(e);
                currentPage++;
                e.HasMorePages = true;
            }
            else
            {
                printPageTwo(e);
            }
        }



        private void printPageTwo(PrintPageEventArgs e)
        {
            Image background = Image.FromFile(Settings.Resouces + @"\invoice2.png");
            e.Graphics.DrawImage(background, backgroundXPos, backgroundXPos, 812, 1070);
        }

        private void printPageOne(PrintPageEventArgs e)
        {
            #region background

            Image background = Image.FromFile(Settings.Resouces + @"\invoice.png");
            e.Graphics.DrawImage(background, backgroundXPos, backgroundXPos, 800, 1070);

            #endregion

            printCustomerInformation(e);
            printVehicleInformation(e);
            printGeneralInformation(e);
            printWarrantyInformation(e);
            printTradeInformation(e);
            printPricingInformation(e);
            printUserInformation(e);
        }

        private void printUserInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString(CacheManager.ActiveUser.Name, font, Brushes.Black, backgroundXPos + 15,
                    backgroundYPos + 860);

                e.Graphics.DrawString(CacheManager.ActiveUser.RegistrationNumer, font, Brushes.Black, backgroundXPos + 225,
                    backgroundYPos + 860);

                e.Graphics.DrawString(CacheManager.ActiveUser.Name, font, Brushes.Black, backgroundXPos + 140,
                    backgroundYPos + 940);

                string sDate = currVehicle.GetValue(PropertyId.SaleDate);
                if (!string.IsNullOrEmpty(sDate))
                {
                    e.Graphics.DrawString(sDate, font, Brushes.Black, backgroundXPos + 20, backgroundYPos + 1005);
                }
            }
        }

        private void printPricingInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                //SalePrice 21
                double salePrice = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePrice), out salePrice))
                {
                    string stringSalePrice = salePrice.ToString("F");
                    e.Graphics.DrawString(stringSalePrice, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 285);
                }

                //Warranty
                double warranty = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleWarrantyCost), out warranty))
                {
                    string stringWarranty = warranty.ToString("F");
                    e.Graphics.DrawString(stringWarranty, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 306);
                }

                // Finance Fee
                double financeFee = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleFinanceCost), out financeFee))
                {
                    using (Font infoFont = new Font("Calibri", 10))
                    {
                        e.Graphics.DrawString("FINANCE", infoFont, Brushes.Black, backgroundXPos + 577, backgroundYPos + 327);
                    }

                    string stringFinanceFee = financeFee.ToString("F");
                    e.Graphics.DrawString(stringFinanceFee, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 328);
                }

                // Accessories Fee
                double accessories = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleAccessoryCost), out accessories))
                {
                    using (Font infoFont = new Font("Calibri", 10))
                    {
                        e.Graphics.DrawString("ACCESSORIES", infoFont, Brushes.Black, backgroundXPos + 577, backgroundYPos + 351);
                    }

                    string stringAccessories = accessories.ToString("F");
                    e.Graphics.DrawString(stringAccessories, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 352);
                }

                // Subtotal
                double subTotal = accessories + financeFee + warranty + salePrice;
                string stringSubTotal = subTotal.ToString("F");
                e.Graphics.DrawString(stringSubTotal, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 466);

                // Trade In Allowance
                double tradeInAllowance = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleTradeInCost), out tradeInAllowance))
                {
                    string stringTradeInAllowance = tradeInAllowance.ToString("F");
                    e.Graphics.DrawString(stringTradeInAllowance, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 490);
                }

                // Net difference
                double netDifference = subTotal - tradeInAllowance;
                if (netDifference != subTotal)
                {
                    string stringNetDifference = netDifference.ToString("F");
                    e.Graphics.DrawString(stringNetDifference, font, Brushes.Black, backgroundXPos + 715,
                        backgroundYPos + 512);
                }

                //Hst
                double hst = netDifference * Settings.HST;
                string stringHST = hst.ToString("F");
                e.Graphics.DrawString(stringHST, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 535);

                //Licensing Fee
                double licensingFee = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleLicenseFee), out licensingFee))
                {
                    string stringLicenseFee = licensingFee.ToString("F");
                    e.Graphics.DrawString(stringLicenseFee, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 577);
                }

                //Payout Lien on Trade
                double payOutLien = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SalePayoutLienOnTradeIn), out payOutLien))
                {
                    string stringPayOutLien = payOutLien.ToString("F");
                    e.Graphics.DrawString(stringPayOutLien, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 626);
                }

                // Subtotal 2
                double subTotal2 = netDifference + hst + licensingFee + payOutLien;
                string stringSubTotal2 = subTotal2.ToString("F");
                e.Graphics.DrawString(stringSubTotal2, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 671);

                //Deposit
                double deposit = 0;
                string depositString = "DEPOSIT ";
                using (Font infoFont = new Font("Calibri", 7))
                {
                    string paymentType = currVehicle.GetValue(PropertyId.SaleDepositType);
                    if (!string.IsNullOrEmpty(paymentType))
                    {
                        depositString += " (" + paymentType + ")";
                    }

                    e.Graphics.DrawString(depositString.ToUpper(), infoFont, Brushes.Black, backgroundXPos + 577, backgroundYPos + 695);
                }

                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleCustomerPayment), out deposit))
                {
                    string stringDeposit = deposit.ToString("F");
                    e.Graphics.DrawString(stringDeposit, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 694);
                }

                //BankAdminFee
                double bankAdminFee = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleBankAdminFee), out bankAdminFee))
                {
                    using (Font infoFont = new Font("Calibri", 7))
                    {
                        e.Graphics.DrawString("BANK ADMIN", infoFont, Brushes.Black, backgroundXPos + 577, backgroundYPos + 787);
                    }

                    string stringBankAdminFee = bankAdminFee.ToString("F");
                    e.Graphics.DrawString(stringBankAdminFee, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 785);
                }

                //Lien Registration Fee
                double lienRegistrationFee = 0;
                if (Utilities.StringToDouble(currVehicle.GetValue(PropertyId.SaleLienRegistrationFee), out lienRegistrationFee))
                {
                    string stringLienRegistrationFee = lienRegistrationFee.ToString("F");
                    e.Graphics.DrawString(stringLienRegistrationFee, font, Brushes.Black, backgroundXPos + 715, backgroundYPos + 809);
                }

                //Total Due
                double totalDue = subTotal2 - deposit + bankAdminFee + lienRegistrationFee;
                string stringTotalDue = totalDue.ToString("F");
                using (Font infoFont = new Font("Calibri", 14))
                {
                    e.Graphics.DrawString(stringTotalDue, infoFont, Brushes.Black, backgroundXPos + 695,
                        backgroundYPos + 900);
                }
            }
        }

        private void printTradeInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 8))
            {
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInYear), font, Brushes.Black, backgroundXPos + 5, backgroundYPos + 385);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInMake), font, Brushes.Black, backgroundXPos + 38, backgroundYPos + 385);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInModel), font, Brushes.Black, backgroundXPos + 130, backgroundYPos + 385);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleTradeInColour), font, Brushes.Black, backgroundXPos + 263, backgroundYPos + 385);

                int xPos = 9;
                if (!string.IsNullOrEmpty(currVehicle.GetValue(PropertyId.SaleTradeInVIN)))
                {
                    foreach (char vinChar in currVehicle.GetValue(PropertyId.SaleTradeInVIN))
                    {
                        e.Graphics.DrawString(vinChar.ToString(), font, Brushes.Black, backgroundXPos + xPos, backgroundYPos + 410);
                        xPos += 17;
                    }
                }

                string mileage = currVehicle.GetValue(PropertyId.SaleTradeInMileage);
                if (!string.IsNullOrEmpty(mileage))
                {
                    mileage = mileage.Replace(",", string.Empty);
                    int spacing = 6 - mileage.Length;
                    xPos = 8 + (20*spacing);

                    foreach (char mileageChar in mileage)
                    {
                        e.Graphics.DrawString(mileageChar.ToString(), font, Brushes.Black, backgroundXPos + xPos,
                            backgroundYPos + 447);
                        xPos += 22;
                    }

                    e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 162, backgroundYPos + 444);
                }

                
            }
        }

        private void printWarrantyInformation(PrintPageEventArgs e)
        {
            string warrantyProvider = currVehicle.GetValue(PropertyId.SaleWarrantyProvider);
            if (string.IsNullOrEmpty(warrantyProvider))
            {
                using (Font font = new Font("Calibri (Body)", 10))
                {
                    e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 536, backgroundYPos + 430);
                }
                return;
            }

            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 503, backgroundYPos + 430);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleWarrantyCost), font, Brushes.Black, backgroundXPos + 510, backgroundYPos + 455);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleWarrantyProvider), font, Brushes.Black, backgroundXPos + 380, backgroundYPos + 475);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleWarrantyMonths), font, Brushes.Black, backgroundXPos + 310, backgroundYPos + 500);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleWarrantyKMs), font, Brushes.Black, backgroundXPos + 440, backgroundYPos + 500);

                int yPos = 545;
                foreach (string description in currVehicle.GetValue(PropertyId.SaleWarrantyDescription).Split(';'))
                {
                    e.Graphics.DrawString(description, font, Brushes.Black, backgroundXPos + 320, backgroundYPos + yPos);
                    yPos += 20;
                }
            }
        }

        private void printGeneralInformation(PrintPageEventArgs e)
        {
            // SaleDate
            string sDate = currVehicle.GetValue(PropertyId.SaleDate);
            DateTime saleDate = DateTime.Now;
            if (DateTime.TryParse(sDate, out saleDate))
            {
                using (Font infoFont = new Font("Calibri", 14))
                {
                    e.Graphics.DrawString(saleDate.Day.ToString(), infoFont, Brushes.Black, backgroundXPos + 655, backgroundYPos + 64);
                    e.Graphics.DrawString(saleDate.Month.ToString(), infoFont, Brushes.Black, backgroundXPos + 705, backgroundYPos + 64);
                    e.Graphics.DrawString(saleDate.Year.ToString(), infoFont, Brushes.Black, backgroundXPos + 750, backgroundYPos + 64);
                }
            }

            using (Font font = new Font("Calibri (Body)", 10))
            {
                //Dealer Guarantee
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 530, backgroundYPos + 281);

                int yPos = 585;
                string comments = currVehicle.GetValue(PropertyId.SaleComments);

                if (!string.IsNullOrEmpty(comments))
                {
                    foreach (string comment in comments.Split(';'))
                    {
                        e.Graphics.DrawString(comment, font, Brushes.Black, backgroundXPos + 10, backgroundYPos + yPos);
                        yPos += 23;
                    }
                }
            }


        }

        private void printCustomerInformation(PrintPageEventArgs e)
        {

            Dictionary<CustomerCacheSearchKey, string> searchParam = new Dictionary<CustomerCacheSearchKey, string>();
            searchParam.Add(CustomerCacheSearchKey.Id, currVehicle.GetValue(PropertyId.SaleCustomerId));
            CustomerCache cache = new CustomerCache(searchParam);

            if (cache.Count <= 0)
                return;
            
            using (Font font = new Font("Calibri (Body)", 10))
            {
                CustomerAdminObject customer = cache[0];

                e.Graphics.DrawString(customer.FirstName, font, Brushes.Black, backgroundXPos + 90, backgroundYPos + 115);
                e.Graphics.DrawString(customer.LastName, font, Brushes.Black, backgroundXPos + 300, backgroundYPos + 115);

                string address = string.Format("{0} {1}",
                    customer.GetValue(PropertyId.HomeStreetNumber),
                    customer.GetValue(PropertyId.HomeStreet));

                e.Graphics.DrawString(address, font, Brushes.Black, backgroundXPos + 30, backgroundYPos + 145);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomeCity), font, Brushes.Black, backgroundXPos + 30, backgroundYPos + 170);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomeProvince), font, Brushes.Black, backgroundXPos + 220, backgroundYPos + 170);

                e.Graphics.DrawString(customer.GetValue(PropertyId.HomePostalCode), font, Brushes.Black, backgroundXPos + 330, backgroundYPos + 170);

                //Print phone number
                if (string.IsNullOrEmpty(customer.GetValue(PropertyId.MobilePhone)))
                {
                    e.Graphics.DrawString(customer.GetValue(PropertyId.HomePhone), font, Brushes.Black, backgroundXPos + 30, backgroundYPos + 200);
                }
                else
                {
                    e.Graphics.DrawString(customer.GetValue(PropertyId.MobilePhone) + " (cell)", font, Brushes.Black, backgroundXPos + 30, backgroundYPos + 200);
                }

                e.Graphics.DrawString(customer.GetValue(PropertyId.DriversLicense), font, Brushes.Black, backgroundXPos + 65, backgroundYPos + 225);

                e.Graphics.DrawString(customer.GetValue(PropertyId.LicenseExpiry), font, Brushes.Black, backgroundXPos + 315, backgroundYPos + 225);

            }

        }

        private void printVehicleInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                //Year
                string year = currVehicle.Year;
                if (year.Length == 4)
                    year = year.Substring(2);
                e.Graphics.DrawString(year, font, Brushes.Black, backgroundXPos + 403, backgroundYPos + 115);

                //Make
                e.Graphics.DrawString(currVehicle.Make, font, Brushes.Black, backgroundXPos + 445, backgroundYPos + 115);

                //Model
                e.Graphics.DrawString(currVehicle.Model, font, Brushes.Black, backgroundXPos + 545, backgroundYPos + 115);

                //Colour
                e.Graphics.DrawString(currVehicle.ExtColor, font, Brushes.Black, backgroundXPos + 685, backgroundYPos + 115);

                //VIN
                string vin = currVehicle.GetValue(PropertyId.VinNumber);
                int xPos = 406;
                foreach (char vinChar in vin)
                {
                    e.Graphics.DrawString(vinChar.ToString(), font, Brushes.Black, backgroundXPos + xPos, backgroundYPos + 147);
                    xPos += 23;
                }

                string mileage = currVehicle.GetValue(PropertyId.Mileage);
                if (!string.IsNullOrEmpty(mileage))
                {
                    mileage = mileage.Replace(",", string.Empty);
                    xPos = 410;
                    int spacing = 6 - mileage.Length;
                    xPos = 405 + (23*spacing);

                    foreach (char mileageChar in mileage)
                    {
                        e.Graphics.DrawString(mileageChar.ToString(), font, Brushes.Black, backgroundXPos + xPos,
                            backgroundYPos + 185);
                        xPos += 23;
                    }
                }

                //IS Km's X
                e.Graphics.DrawString("x", font, Brushes.Black, backgroundXPos + 470, backgroundYPos + 160);

                // Delivery Date
                string stringDeliveryDate = currVehicle.GetValue(PropertyId.SaleDeliveryDate);
                DateTime deliveryDate = DateTime.Now;
                if (DateTime.TryParse(stringDeliveryDate, out deliveryDate))
                {
                    e.Graphics.DrawString(deliveryDate.DayOfWeek.ToString(), font, Brushes.Black,
                        backgroundXPos + 620, backgroundYPos + 210);
                }

                // Certified
                string safety = currVehicle.GetValue(PropertyId.SaleSafetyCertificate);
                if (!string.IsNullOrEmpty(safety))
                {
                    SaleInfoControl.Certified certified = (SaleInfoControl.Certified)Enum.Parse(typeof(SaleInfoControl.Certified), safety, true);
                    if (certified == SaleInfoControl.Certified.Yes)
                    {
                        e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 540, backgroundYPos + 232);
                    }
                    else
                    {
                        e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 540, backgroundYPos + 247);
                    }
                }

                //Brand
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SaleBrand), font, Brushes.Black, backgroundXPos + 675, backgroundYPos + 240);
            }
        }
    }
}
