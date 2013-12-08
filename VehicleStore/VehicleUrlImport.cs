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
                Regex hostPattern = new Regex(@"[a-z][a-z0-9+\-.]*://([a-z0-9\-._~%]+|\[[a-z0-9\-._~%!$&'()*+,;=:]+\])", RegexOptions.IgnoreCase);
                Match host = hostPattern.Match(url);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader inStream = new StreamReader(res.GetResponseStream());

                string page = inStream.ReadToEnd();
                inStream.Close();
                res.Close();

                string brochurePattern = "(/ebrochure.htm.*)(?=[\"])";
                Regex rxImage = new Regex(@"(?<=<a\s?href=.{1})(.*\.jpg)");
                Regex enginePattern = new Regex(@"(?<=<dt><span>Engine</span></dt><dd><span>)(.*)(?=</span></dd>)");
                Regex bodyPattern = new Regex(@"(?<=<dt><span>Bodystyle</span></dt><dd><span>)(.*)(?=</span></dd>)");
                Regex fuelPattern = new Regex(@"(?<=<dt><span>Fuel Type</span></dt><dd><span>)(.*)(?=</span></dd>)");
                Regex colorPattern = new Regex(@"(?<=<dt><span>Ext. Colour</span></dt><dd><span>)(.*)(?=</span></dd>)");
                Regex transmissionPattern = new Regex(@"(?<=<dt><span>Transmission</span></dt><dd><span>)(.*)(?=</span></dd>)");
                Regex interiorColorPattern = new Regex(@"(?<=<dt><span>Int. Colour</span></dt><dd><span>)(.*)(?=</span></dd>)");
                string mileagePattern = "(?<=<span>Kilometres</span></dt><dd class=\"mileageValue\"><span>)(.*)(?=</span></dd>)";
                Regex stockNumberPattern = new Regex(@"(?<=<dt><span>Stock Number</span></dt><dd><span>)(.*)(?=</span></dd>)");

                Match brochureLink = Regex.Match(page, brochurePattern);
                MatchCollection matches = rxImage.Matches(page);
                Match engine = enginePattern.Match(page);
                Match body = bodyPattern.Match(page);
                Match fuel = fuelPattern.Match(page);
                Match color = colorPattern.Match(page);
                Match transmission = transmissionPattern.Match(page);
                Match interiorColor = interiorColorPattern.Match(page);
                Match mileage = Regex.Match(page, mileagePattern);
                Match stockNumber = stockNumberPattern.Match(page);

                foreach (Match m in matches)
                {
                    Uri path = new Uri(m.ToString());
                    WebClient downloadClient = new WebClient();
                    string outputFile = Resources.Settings.TempFolder + "\\Image" + DateTime.Now.ToFileTimeUtc().ToString() + ".jpg";
                    downloadClient.DownloadFile(path, outputFile);
                    outputFile = Settings.MoveToItemImageFolder(vehicle, outputFile);
                    imagePaths.Add(new string[] { PropertyId.VehicleImage.ToString(), outputFile });
                }

                dataMap.Add(PropertyId.Fueltype, fuel.ToString().Trim());
                dataMap.Add(PropertyId.ExtColor, color.ToString().Trim());
                dataMap.Add(PropertyId.IntColor, interiorColor.ToString().Trim());
                dataMap.Add(PropertyId.Bodystyle, body.ToString().Trim());
                dataMap.Add(PropertyId.Transmission, transmission.ToString().Trim());
                dataMap.Add(PropertyId.Engine, engine.ToString().Trim());
                dataMap.Add(PropertyId.Mileage, mileage.ToString().Trim());
                dataMap.Add(PropertyId.Images, imagePaths);
                dataMap.Add(PropertyId.StockNumber, stockNumber.ToString().Trim());

                getBrochure(host + brochureLink.ToString());
                result = VehicleImportStatus.PASS;
                ExportVehicleInfo exp = new ExportVehicleInfo(dataMap);
            }
        }

        private void getBrochure(string brochureLink)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(brochureLink);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader inStream = new StreamReader(res.GetResponseStream());

            string brochure = inStream.ReadToEnd();
            inStream.Close();
            res.Close();

            string yearPattern = "(?<=<div id=\"ebVehicleTitle\">)(\\d{4})(?=.+</div>)";
            string makePattern = "(?<=<div id=\"ebVehicleTitle\">(\\d{4})\\s)(\\w+)(?=\\s.*</div>)";
            string modelPattern = "(?<=<div id=\"ebVehicleTitle\">(\\d{4})\\s(\\w+)\\s)(\\w+)(?=\\s.*</div>)";
            string trimPattern = "(?<=<div id=\"ebVehicleTitle\">(\\d{4}\\s(\\w+)\\s(\\w+)\\s))(.*)(?=</div>)";
            //Regex doorPattern = new Regex(@"(?<=<span>Doors:</span>)[^<]+(?=<br />)");
            Regex pricePattern = new Regex(@"(?<=Internet Price:)[^<]+(?=<br/>)");

            Match year = Regex.Match(brochure, yearPattern);
            Match make = Regex.Match(brochure, makePattern);
            Match model = Regex.Match(brochure, modelPattern);
            Match trim = Regex.Match(brochure, trimPattern);
            //Match door = doorPattern.Match(brochure);
            Match listPrice = pricePattern.Match(brochure);

            dataMap.Add(PropertyId.Year, year.ToString().Trim());
            dataMap.Add(PropertyId.Make, make.ToString().Trim());
            dataMap.Add(PropertyId.Model, model.ToString().Trim());
            dataMap.Add(PropertyId.Trim, trim.ToString().Trim());
            dataMap.Add(PropertyId.ListPrice, listPrice.ToString().Trim());
        }

        public void ApplyVehicleValues() 
        {
            foreach (var vehicleData in dataMap)
            {
                _vehicle.SetValue(vehicleData.Key, vehicleData.Value);
            }
            _vehicle.Images = imagePaths;
        }
    }
}
