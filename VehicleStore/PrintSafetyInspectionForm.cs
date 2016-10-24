using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;
using CarDepot.Resources;

namespace CarDepot.VehicleStore
{
    class PrintSafetyInspectionForm
    {
        private VehicleAdminObject currVehicle;
        float backgroundXPos = 0;
        float backgroundYPos = -10;
        private int numOfPages = 3;
        private int currentPage = 1;

        public PrintSafetyInspectionForm(VehicleAdminObject vehicle, Object sender, EventArgs e)
        {
            currVehicle = vehicle;

            PrintDocument safetyInspectionForm = new PrintDocument();
            try
            {
                safetyInspectionForm.PrintPage += new PrintPageEventHandler(this.printAppraisalForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PrintSafetyInspectionForm: " + ex.Message);
            }

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = safetyInspectionForm;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                safetyInspectionForm.Print();
            }
        }

        private void printAppraisalForm(Object sender, PrintPageEventArgs e)
        {
            printPageOne(e);
        }

        private void printPageOne(PrintPageEventArgs e)
        {
            #region background

            Image background = Image.FromFile(Settings.Resouces + @"\SafetyInspectionForm.png");
            e.Graphics.DrawImage(background, 10, 0, 800, 1070);

            #endregion
            PrintDealershipInformation(e);
            PrintVehicleInformation(e);
            printDateAndUnitMeasurement(e);
            printMechanicInformation(e);

            printFrontLeftSafetyInformation(e);
            printFrontRightSafetyInformation(e);
            printRearLeftSafetyInformation(e);
            printRearRightSafetyInformation(e);

            printFuelGage(e);
        }

        private void printFuelGage(PrintPageEventArgs e)
        {
            Font font = new Font("Calibri (Body)", 10, FontStyle.Bold);

            string sFuel = currVehicle.GetValue(PropertyId.SafetyGasTankLevel);
            if (string.IsNullOrEmpty(sFuel))
            {
                return;
            }
            sFuel = sFuel.Replace("System.Windows.Controls.Label:", String.Empty);
            sFuel = sFuel.Trim();

            if (sFuel.Trim().ToLower().Equals("empty"))
            {
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 542, backgroundYPos + 1018);  
            }

            if (sFuel.Trim().ToLower().Equals("1/4"))
            {
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 575, backgroundYPos + 1018);
            }

            if (sFuel.Trim().ToLower().Equals("1/2"))
            {
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 615, backgroundYPos + 1016);
            }

            if (sFuel.Trim().ToLower().Equals("3/4"))
            {
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 652, backgroundYPos + 1016);
            }

            if (sFuel.Trim().ToLower().Equals("full"))
            {
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 685, backgroundYPos + 1016);
            }
        }

        private void printRearRightSafetyInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightRotorThickness), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 622);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightPadThicknessMaterialThicknessInner), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 700);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightPadThicknessMaterialThicknessOuter), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 759);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightShoeLiningThickness), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 849);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightDrumDiameter), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 910);



                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightTireTreadDepth), font, Brushes.Black, backgroundXPos + 733, backgroundYPos + 664);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightTireInflationPressure), font, Brushes.Black, backgroundXPos + 733, backgroundYPos + 717);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightTireCorrectedAboveFivePsi), font, Brushes.Black, backgroundXPos + 733, backgroundYPos + 802);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearRightTireFinalReading), font, Brushes.Black, backgroundXPos + 733, backgroundYPos + 852);
            }
        }

        private void printRearLeftSafetyInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftRotorThickness), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 622);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftPadThicknessMaterialThicknessInner), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 700);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftPadThicknessMaterialThicknessOuter), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 759);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftShoeLiningThickness), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 849);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftDrumDiameter), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 910);



                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftTireTreadDepth), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 664);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftTireInflationPressure), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 717);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftTireCorrectedAboveFivePsi), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 802);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyRearLeftTireFinalReading), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 852);
            }
        }

        private void printFrontRightSafetyInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightRotorThickness), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 229);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightPadThicknessMaterialThicknessInner), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 307);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightPadThicknessMaterialThicknessOuter), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 366);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightShoeLiningThickness), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 456);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightDrumDiameter), font, Brushes.Black, backgroundXPos + 612, backgroundYPos + 517);



                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightTireTreadDepth), font, Brushes.Black, backgroundXPos + 728, backgroundYPos + 329);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightTireInflationPressure), font, Brushes.Black, backgroundXPos + 728, backgroundYPos + 382);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightTireCorrectedAboveFivePsi), font, Brushes.Black, backgroundXPos + 728, backgroundYPos + 467);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontRightTireFinalReading), font, Brushes.Black, backgroundXPos + 728, backgroundYPos + 517);
            }
        }

        private void printFrontLeftSafetyInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftRotorThickness), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 229);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftPadThicknessMaterialThicknessInner), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 307);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftPadThicknessMaterialThicknessOuter), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 366);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftShoeLiningThickness), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 456);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftDrumDiameter), font, Brushes.Black, backgroundXPos + 525, backgroundYPos + 517);



                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftTireTreadDepth), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 329);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftTireInflationPressure), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 382);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftTireCorrectedAboveFivePsi), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 467);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyFrontLeftTireFinalReading), font, Brushes.Black, backgroundXPos + 415, backgroundYPos + 517);
            }
        }

        private void printMechanicInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 14, FontStyle.Bold))
            {
                string mechanicName = "Refaat Toma";
                string certificateNumber = "S - 336732";
                e.Graphics.DrawString(mechanicName, font, Brushes.Black, backgroundXPos + 23, backgroundYPos + 368);

                e.Graphics.DrawString(certificateNumber, font, Brushes.Black, backgroundXPos + 103, backgroundYPos + 402);

                e.Graphics.DrawString(currVehicle.GetValue(PropertyId.SafetyCertificateNumbersIssued), font, Brushes.Black, backgroundXPos + 168, backgroundYPos + 440);
            }

            using (Font font = new Font("Calibri (Body)", 10, FontStyle.Bold))
            {
                if (!string.IsNullOrEmpty(currVehicle.GetValue(PropertyId.SafetyInspectionPass)) &&
                    currVehicle.GetValue(PropertyId.SafetyInspectionPass).Trim() == "true")
                {
                    e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 23, backgroundYPos + 445);
                }

                if (!string.IsNullOrEmpty(currVehicle.GetValue(PropertyId.SafetyInspectionFail)) &&
                    currVehicle.GetValue(PropertyId.SafetyInspectionFail).Trim() == "true")
                {
                    e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 90, backgroundYPos + 445);
                }
            }

        }

        private void PrintVehicleInformation(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 10))
            {
                string sYear = currVehicle.GetValue(PropertyId.Year);
                e.Graphics.DrawString(sYear, font, Brushes.Black, backgroundXPos + 20, backgroundYPos + 248);

                string sMake = currVehicle.GetValue(PropertyId.Make);
                e.Graphics.DrawString(sMake, font, Brushes.Black, backgroundXPos + 80, backgroundYPos + 248);

                string sModel = currVehicle.GetValue(PropertyId.Model);
                e.Graphics.DrawString(sModel, font, Brushes.Black, backgroundXPos + 240, backgroundYPos + 248);

                string vin = currVehicle.GetValue(PropertyId.VinNumber);
                if (!string.IsNullOrEmpty(vin))
                {
                    int xPos = 23;
                    foreach (char vinChar in vin)
                    {
                        e.Graphics.DrawString(vinChar.ToString(), font, Brushes.Black, backgroundXPos + xPos, backgroundYPos + 280);
                        xPos += 21;
                    }
                }

                string mileage = currVehicle.GetValue(PropertyId.Mileage);
                if (!string.IsNullOrEmpty(mileage))
                {
                    mileage = mileage.Replace(",", string.Empty);
                    int spacing = 6 - mileage.Length;
                    int xPos = 23 + (21 * spacing);

                    foreach (char mileageChar in mileage)
                    {
                        e.Graphics.DrawString(mileageChar.ToString(), font, Brushes.Black, backgroundXPos + xPos, backgroundYPos + 317);
                        xPos += 21;
                    }
                }
                e.Graphics.DrawString("X", font, Brushes.Black, backgroundXPos + 161, backgroundYPos + 315);
            }
        }

        private void PrintDealershipInformation(PrintPageEventArgs e)
        {
            Font fontSmaller = new Font("Calibri (Body)", 12, FontStyle.Bold);

            using (Font font = new Font("Calibri (Body)", 16, FontStyle.Bold))
            {
                e.Graphics.DrawString("Roger's Motors Ontario Inc", font, Brushes.Black, backgroundXPos + 20, backgroundYPos + 90);

                e.Graphics.DrawString("1035 Speers Road, Oakville ON L6L 2X5", fontSmaller, Brushes.Black, backgroundXPos + 20, backgroundYPos + 125);

                e.Graphics.DrawString("License Number: 2551607", fontSmaller, Brushes.Black, backgroundXPos + 20, backgroundYPos + 145);
            }
        }

        private void printDateAndUnitMeasurement(PrintPageEventArgs e)
        {
            using (Font font = new Font("Calibri (Body)", 24))
            {
                string sInspectionDate = currVehicle.GetValue(PropertyId.SafetyInspectionDate);
                if (!string.IsNullOrEmpty(sInspectionDate))
                {
                    DateTime inspectionDate = DateTime.Parse(sInspectionDate);
                    if (inspectionDate != null)
                    {
                        e.Graphics.DrawString(inspectionDate.Year.ToString(), font, Brushes.Black, backgroundXPos + 530, backgroundYPos + 40);

                        e.Graphics.DrawString(inspectionDate.Month.ToString(), font, Brushes.Black, backgroundXPos + 635, backgroundYPos + 40);

                        e.Graphics.DrawString(inspectionDate.Day.ToString(), font, Brushes.Black, backgroundXPos + 717, backgroundYPos + 40);
                    }
                }

                if (!string.IsNullOrEmpty(currVehicle.GetValue(PropertyId.SafetyUnitMeasurementMMS).Trim()) &&
                    currVehicle.GetValue(PropertyId.SafetyUnitMeasurementMMS) == "true")
                {
                    e.Graphics.DrawString("x", font, Brushes.Black, backgroundXPos + 607, backgroundYPos + 95);
                }

                if (!string.IsNullOrEmpty(currVehicle.GetValue(PropertyId.SafetyUnitMeasurementInches)) &&
                    currVehicle.GetValue(PropertyId.SafetyUnitMeasurementInches).Trim() == "true")
                {
                    e.Graphics.DrawString("x", font, Brushes.Black, backgroundXPos + 682, backgroundYPos + 95);
                }
            }
        }
        
    }
}
