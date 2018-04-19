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
using ExcelLibrary.BinaryFileFormat;

namespace CarDepot.VehicleStore
{
    public enum VehicleImportStatus
    {
        PASS,
        FAIL
    }

    internal class VehicleUrlImport
    {
        VehicleAdminObject _vehicle = null;
        private Dictionary<PropertyId, Object> dataMap = new Dictionary<PropertyId, Object>();
        private List<string[]> imagePaths;
        VehicleImportStatus result = VehicleImportStatus.FAIL;
        
        public VehicleImportStatus ImportStatus
        {
            get { return result; }
        }
        public VehicleUrlImport(VehicleAdminObject vehicle,string url)
        {
            loadURL(vehicle, url);
        }

        private void loadURL(VehicleAdminObject vehicle, string url)
        {
            _vehicle = vehicle;
            Regex validURL = new Regex(@"\A(https?|ftp|file)://.+\z");
            if (validURL.IsMatch(url))
            {
                imagePaths = new List<string[]>();

                string urlAddress = url;
                string page = "";

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11");
                    page = client.DownloadString(url);
                }

                /*
                Regex hostPattern = new Regex(@"[a-z][a-z0-9+\-.]*://([a-z0-9\-._~%]+|\[[a-z0-9\-._~%!$&'()*+,;=:]+\])", RegexOptions.IgnoreCase);
                Match host = hostPattern.Match(url);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader inStream = new StreamReader(res.GetResponseStream());

                page = inStream.ReadToEnd();
                inStream.Close();
                res.Close();
                */

                //Exterior Color
                string exteriorColor = "Exterior Colour\n</span>\n<span class=\"separator\">:</span>\n<span class=\"value\">";
                int startIndex = page.IndexOf(exteriorColor, System.StringComparison.Ordinal);
                int length = page.IndexOf("</", startIndex + exteriorColor.Length, System.StringComparison.Ordinal) - startIndex - exteriorColor.Length;
                string foundExteriorColor = page.Substring(startIndex + exteriorColor.Length, length).Trim();
                dataMap.Add(PropertyId.ExtColor, foundExteriorColor.Trim());

                string bodyStyle = "bodyStyle: '";
                startIndex = page.IndexOf(bodyStyle, System.StringComparison.Ordinal);
                length = page.IndexOf("'", startIndex + bodyStyle.Length, System.StringComparison.Ordinal) - startIndex - bodyStyle.Length;
                string foundBodyStyle = page.Substring(startIndex + bodyStyle.Length, length);
                dataMap.Add(PropertyId.Bodystyle, foundBodyStyle.Trim());

                string Year = "year: '";
                startIndex = page.IndexOf(Year, System.StringComparison.Ordinal);
                length = page.IndexOf("'", startIndex + Year.Length, System.StringComparison.Ordinal) - startIndex - Year.Length;
                string foundYear = page.Substring(startIndex + Year.Length, length);
                dataMap.Add(PropertyId.Year, foundYear.Trim());

                string Make = "make: '";
                startIndex = page.IndexOf(Make, System.StringComparison.Ordinal);
                length = page.IndexOf("'", startIndex + Make.Length, System.StringComparison.Ordinal) - startIndex - Make.Length;
                string foundMake = page.Substring(startIndex + Make.Length, length);
                dataMap.Add(PropertyId.Make, foundMake.Trim());

                string Model = "model: '";
                startIndex = page.IndexOf(Model, System.StringComparison.Ordinal);
                length = page.IndexOf("'", startIndex + Model.Length, System.StringComparison.Ordinal) - startIndex - Model.Length;
                string foundModel = page.Substring(startIndex + Model.Length, length);
                dataMap.Add(PropertyId.Model, foundModel.Trim());

                string Transmission = "\"transmission\": \"";
                startIndex = page.IndexOf(Transmission, System.StringComparison.Ordinal);
                length = page.IndexOf("\"", startIndex + Transmission.Length, System.StringComparison.Ordinal) - startIndex - Transmission.Length;
                string foundTransmission = page.Substring(startIndex + Transmission.Length, length);
                dataMap.Add(PropertyId.Transmission, foundTransmission.Trim());

                string Engine = "\"engine\": \"";
                startIndex = page.IndexOf(Engine, System.StringComparison.Ordinal);
                length = page.IndexOf("\"", startIndex + Engine.Length, System.StringComparison.Ordinal) - startIndex - Engine.Length;
                string foundEngine = page.Substring(startIndex + Engine.Length, length);
                foundEngine = foundEngine.Replace("\\x2D", "-");
                foundEngine = foundEngine.Replace("\\x20", " ");
                dataMap.Add(PropertyId.Engine, foundEngine.Trim());

                string StockNumber = "stockNumber: '";
                startIndex = page.IndexOf(StockNumber, System.StringComparison.Ordinal);
                length = page.IndexOf("'", startIndex + StockNumber.Length, System.StringComparison.Ordinal) - startIndex - StockNumber.Length;
                string foundStockNumber = page.Substring(startIndex + StockNumber.Length, length);
                dataMap.Add(PropertyId.StockNumber, foundStockNumber.Trim());

                string Mileage = "\"odometer\": \"";
                startIndex = page.IndexOf(Mileage, System.StringComparison.Ordinal);
                length = page.IndexOf("\"", startIndex + Mileage.Length, System.StringComparison.Ordinal) - startIndex - Mileage.Length;
                string foundMileage = page.Substring(startIndex + Mileage.Length, length);
                dataMap.Add(PropertyId.Mileage, foundMileage.Trim());


                string Price = "<strong class=\"h1 price\" >";
                startIndex = page.LastIndexOf(Price, System.StringComparison.Ordinal);
                length = page.IndexOf("<", startIndex + Price.Length, System.StringComparison.Ordinal) - startIndex - Price.Length;
                string foundPrice = page.Substring(startIndex + Price.Length, length);
                dataMap.Add(PropertyId.ListPrice, foundPrice.Trim());

                string Trim = "\"trim\": \"";
                startIndex = page.IndexOf(Trim, System.StringComparison.Ordinal);
                length = page.IndexOf("\"", startIndex + Trim.Length, System.StringComparison.Ordinal) - startIndex - Trim.Length;
                string foundTrim = page.Substring(startIndex + Trim.Length, length);
                foundTrim = foundTrim.Replace("\\x2D", "-");
                foundTrim = foundTrim.Replace("\\x20", " ");
                if (foundTrim.StartsWith("-"))
                {
                    foundTrim = foundTrim.Substring(1);
                }
                dataMap.Add(PropertyId.Trim, foundTrim.Trim());

                // Load Images
                string linkHeader = "<a href=\"";
                startIndex = 0;
                int foundIndex = page.IndexOf(linkHeader, startIndex, System.StringComparison.Ordinal);
                while (foundIndex != -1)
                {
                    startIndex = foundIndex;
                    int stringLength = page.IndexOf("\"", startIndex + linkHeader.Length, System.StringComparison.Ordinal) - foundIndex -
                                       linkHeader.Length;

                    string foundLink = page.Substring(foundIndex + linkHeader.Length, stringLength);
                    if (foundLink.Contains(".jpg"))
                    {
                        if (foundLink.StartsWith("//"))
                        {
                            string imageExension = ".jpg";
                            foundLink = foundLink.Substring(2);
                            foundLink = "http://" + foundLink.Substring(0, foundLink.IndexOf(imageExension) + imageExension.Length);
                        }

                        foundLink = foundLink.Substring(0, foundLink.IndexOf(".jpg") + 4);

                        WebClient downloadClient = new WebClient();
                        string tempOutputFile = Resources.Settings.TempFolder + "\\Image" + DateTime.Now.ToFileTimeUtc().ToString() + ".jpg";
                        downloadClient.DownloadFile(foundLink, tempOutputFile);
                        string outputFile = Settings.MoveToItemImageFolder(vehicle, tempOutputFile);
                        imagePaths.Add(new string[] { PropertyId.VehicleImage.ToString(), outputFile });
                        File.Delete(tempOutputFile);
                    }

                    foundIndex = page.IndexOf(linkHeader, startIndex + 1, System.StringComparison.Ordinal);
                }

                result = VehicleImportStatus.PASS;
            }
        }

        public void ApplyVehicleValues() 
        {
            foreach (var vehicleData in dataMap)
            {
                if (!string.IsNullOrEmpty(vehicleData.Value.ToString()))
                {
                    if (vehicleData.Key == PropertyId.Mileage)
                    {
                        if (!string.IsNullOrEmpty(_vehicle.GetValue(PropertyId.Mileage)))
                            continue;
                    }

                    _vehicle.SetValue(vehicleData.Key, vehicleData.Value);
                }
            }

            _vehicle.Images = imagePaths;
            _vehicle.SetMultiValue(PropertyId.Images, imagePaths);
        }
    }
}
