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
    public enum VehicleImportStatus
    {
        PASS,
        FAIL
    }

    internal class VehicleUrlImport
    {
       
        
        //Excel.Application oXL;
        //Excel._Workbook oWB;
        //Excel._Worksheet oSheet;
        //Excel.Range oRng;

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
                //String link = "http://www.rogersmotors.ca/used/Toyota/2009-Toyota-Rav4-48db1cd20a0a010900163769627f5f05.htm";
                Regex hostPattern = new Regex(@"[a-z][a-z0-9+\-.]*://([a-z0-9\-._~%]+|\[[a-z0-9\-._~%!$&'()*+,;=:]+\])", RegexOptions.IgnoreCase);
                Match host = hostPattern.Match(url);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader inStream = new StreamReader(res.GetResponseStream());

                string page = inStream.ReadToEnd();
                inStream.Close();
                res.Close();
                string colorPattern = "(?<=<dt><span>Ext. Colour</span></dt><dd><span>)(.*)(?=</span></dd>)";
                Match color = Regex.Match(page, colorPattern);
                dataMap.Add(PropertyId.ExtColor, color.ToString().Trim());

                string brochurePattern = "(/ebrochure.htm.*)(?=[\"])";
                Match brochureLink = Regex.Match(page, brochurePattern);
                Regex rxImage = new Regex(@"(?<=<a\s?href=.{1})(.*\.jpg)");
                MatchCollection matches = rxImage.Matches(page);

                int count = 0; 
                foreach (Match m in matches)
                {
                    Uri path = new Uri(m.ToString());
                    WebClient downloadClient = new WebClient();
                    string outputFile = Resources.Settings.TempFolder + "\\Image" + DateTime.Now.ToFileTimeUtc().ToString() + ".jpg";

                    downloadClient.DownloadFile(path, outputFile);
                    outputFile = Settings.MoveToItemImageFolder(vehicle, outputFile);
                    imagePaths.Add(new string[] { PropertyId.VehicleImage.ToString(), outputFile });
                    //downloadClient.DownloadFile(path, "Image" + DateTime.Now.ToFileTimeUtc().ToString() + ".jpg");
                    count++;
                }
                getBrochure(host + brochureLink.ToString());
                result = VehicleImportStatus.PASS;
                string file = Resources.Settings.TempFolder + DateTime.Now.ToFileTimeUtc().ToString() + ".xls";
                Workbook wb = new Workbook();
                Worksheet ws = new Worksheet("Sheet 1");
                int j = 0;
                foreach (PropertyId p in dataMap.Keys)
                {
                    int i = 0;
                    while ( i <2 ) 
                    {
                        ws.Cells[i, j] = new Cell(p.ToString());
                        i++;
                        ws.Cells[i, j] = new Cell(dataMap[p]);
                    }
                    j++;
                }
                wb.Worksheets.Add(ws);
                wb.Save(file);
                Workbook book = Workbook.Load(file);
                Worksheet sheet = book.Worksheets[0];
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

            Regex stockNumberPattern = new Regex(@"(?<=<span>Stock Number:</span>)[^<]+(?=<br />)");
            Regex bodyPattern = new Regex(@"(?<=<span>Bodystyle:</span>)[^<]+(?=<br />)");
            Regex doorPattern = new Regex(@"(?<=<span>Doors:</span>)[^<]+(?=<br />)");
            Regex transmissionPattern = new Regex(@"(?<=<span>Transmission:</span>)[^<]+(?=<br />)");
            Regex enginePattern = new Regex(@"(?<=<span>Engine:</span>)[^<]+(?=<br />)");
            Regex mileagePattern = new Regex(@"(?<=<span>Kilometres:</span>)[^<]+(?=<br />)");
            Regex pricePattern = new Regex(@"(?<=Internet Price:)[^<]+(?=<br/>)");

            Match year = Regex.Match(brochure, yearPattern);
            Match make = Regex.Match(brochure, makePattern);
            Match model = Regex.Match(brochure, modelPattern);
            Match trim = Regex.Match(brochure, trimPattern);
            Match stockNumber = stockNumberPattern.Match(brochure);
            Match body = bodyPattern.Match(brochure);
            Match door = doorPattern.Match(brochure);
            Match transmission = transmissionPattern.Match(brochure);
            Match engine = enginePattern.Match(brochure);
            Match mileage = mileagePattern.Match(brochure);
            Match listPrice = pricePattern.Match(brochure);

            dataMap.Add(PropertyId.Year, year.ToString().Trim());
            dataMap.Add(PropertyId.Make, make.ToString().Trim());
            dataMap.Add(PropertyId.Model, model.ToString().Trim());
            dataMap.Add(PropertyId.Trim, trim.ToString().Trim());
            dataMap.Add(PropertyId.StockNumber, stockNumber.ToString().Trim());
            dataMap.Add(PropertyId.Bodystyle, body.ToString().Trim());
            dataMap.Add(PropertyId.Transmission, transmission.ToString().Trim());
            dataMap.Add(PropertyId.Engine, engine.ToString().Trim());
            dataMap.Add(PropertyId.Mileage, mileage.ToString().Trim());
            dataMap.Add(PropertyId.ListPrice, listPrice.ToString().Trim());
            dataMap.Add(PropertyId.Images, imagePaths);
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
