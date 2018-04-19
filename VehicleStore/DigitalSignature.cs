using System;
using System.Collections.Generic;

using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using DocuSign.eSign.Client;
using System.IO;
using Newtonsoft.Json;
using CarDepot.Controls.GeneralControls;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Windows.Forms;
using System.Threading;

namespace CarDepot.VehicleStore
{
    class DigitalSignature
    {
        WebControlWindow webControl = null;
        public delegate void DigitalSignatureDocumentsDownloadedEventHandler(List<string> filePaths);
        public event DigitalSignatureDocumentsDownloadedEventHandler DigitalSignatureDocumentsDownloaded;

        private VehicleAdminObject currVehicle;
        string accountId = "11706908";
        //string accountId = "3752282";

        // Credentials
        string Username = "info@rogersmotors.ca";
        string Password = "aJesus11";
        string IntegratorKey = "85d92d54-e3ef-49fc-8b0e-48c02c89472b";

        // set demo (aka test) environment (for production change to www.docusign.net/restapi) for test https://demo.docusign.net/restapi for live - https://na3.docusign.net/restapi
        string basePath = "https://na3.docusign.net/restapi";

        string lastSignedEnvelopeId = "";
        PrintInvoice invoice = null;

        public DigitalSignature(VehicleAdminObject vehicle, Object sender, EventArgs e)
        {
            currVehicle = vehicle;

            invoice = new PrintInvoice(vehicle, true, sender, e);
        }

        public List<string> DownloadDocuments()
        {
            List<string> filePaths = new List<string>();
            EnvelopesApi envelopesApi = new EnvelopesApi();
            EnvelopeDocumentsResult docsList = envelopesApi.ListDocuments(accountId, lastSignedEnvelopeId);

            int docCount = docsList.EnvelopeDocuments.Count;
            string filePath = null;
            FileStream fs = null;

            // loop through the envelope's documents and download each doc
            for (int i = 0; i < docCount; i++)
            {
                // GetDocument() API call returns a MemoryStream
                MemoryStream docStream = (MemoryStream)envelopesApi.GetDocument(accountId, lastSignedEnvelopeId, docsList.EnvelopeDocuments[i].DocumentId);
                // let's save the document to local file system
                if (i == 0)
                    filePath = Path.GetTempPath() + "Digitally Signed Bill of Sale.pdf";
                else
                    filePath = Path.GetTempPath() + "Digital Signature (" + i + " of " + (docCount - 1) + ") .pdf";

                fs = new FileStream(filePath, FileMode.Create);
                docStream.Seek(0, SeekOrigin.Begin);
                docStream.CopyTo(fs);
                fs.Close();

                filePaths.Add(filePath);
            }

            return filePaths;
        }


        public void SignDocument(CustomerAdminObject customer)
        {
            try
            {
                string pdfPath = invoice.PrintToPDFFilePath;
                if (string.IsNullOrEmpty(pdfPath))
                {
                    return;
                }

                int recipientId = 0;
                UserAdminObject managerUserAdminObject = null;
                UserAdminObject soldByAdminObject = null;
                string soldByName = currVehicle.GetValue(PropertyId.SaleSoldBy);
                foreach (UserAdminObject user in CacheManager.UserCache)
                {
                    if (user.Name.Trim() == soldByName.Trim())
                    {
                        soldByAdminObject = user;
                        break;
                    }
                }
                // instantiate a new api client and set desired environment
                ApiClient apiClient = new ApiClient(basePath);

                // set client in global config so we don't have to pass it to each API object.
                Configuration.Default.ApiClient = apiClient;

                // create JSON formatted auth header containing Username, Password, and Integrator Key
                string authHeader = "{\"Username\":\"" + Username + "\", \"Password\":\"" + Password + "\", \"IntegratorKey\":\"" + IntegratorKey + "\"}";
                Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", authHeader);

                // the authentication api uses the apiClient (and X-DocuSign-Authentication header) that are set in Configuration object
                AuthenticationApi authApi = new AuthenticationApi();
                LoginInformation loginInfo = authApi.Login();

                Console.WriteLine("LoginInformation: {0}", loginInfo.ToJson());

                // specify the document we want signed
                string SignTest1File = pdfPath;

                // read a file from disk to use as a document.
                int numOfTries = 0;
                int maxNumOfTries = 5;
                byte[] fileBytes = null;
                while (numOfTries < maxNumOfTries)
                {
                    try
                    {
                        fileBytes = File.ReadAllBytes(SignTest1File);
                        break;
                    }
                    catch (Exception ex)
                    {
                        numOfTries++;
                        Thread.Sleep(1000);
                    }
                }

                if (fileBytes == null)
                {
                    MessageBox.Show("Unable to read pdf file", "Error");
                    return;
                }

                EnvelopeDefinition envDef = new EnvelopeDefinition();
                envDef.EmailSubject = "[DocuSign C# SDK] - Please sign this doc";

                // Add a document to the envelope
                Document doc = new Document();
                doc.DocumentBase64 = System.Convert.ToBase64String(fileBytes);
                doc.Name = "BillofSale.pdf";
                doc.DocumentId = "1";

                envDef.Documents = new List<Document>();
                envDef.Documents.Add(doc);

                // Add a recipient to sign the documeent
                Signer signer = new Signer();
                signer.Email = customer.Email;
                signer.Name = customer.FirstName + " " + customer.LastName;
                signer.RecipientId = (++recipientId).ToString();

                //Authentication required has been disabled through docusign support, if i need it in the future i need to call back

                // must set |clientUserId| to embed the recipient
                signer.ClientUserId = customer.ObjectId;

                // Create a |SignHere| tab somewhere on the document for the recipient to sign
                signer.Tabs = new Tabs();
                signer.Tabs.SignHereTabs = new List<SignHere>();
                signer.Tabs.InitialHereTabs = new List<InitialHere>();
                SignHere customerSignHere = new SignHere();
                customerSignHere.DocumentId = "1";
                customerSignHere.PageNumber = "1";
                customerSignHere.RecipientId = recipientId.ToString();
                customerSignHere.XPosition = "425";
                customerSignHere.YPosition = "670";
                signer.Tabs.SignHereTabs.Add(customerSignHere);

                InitialHere initialHere = new InitialHere();
                initialHere.DocumentId = "1";
                initialHere.PageNumber = "1";
                initialHere.RecipientId = "1";
                initialHere.XPosition = "180";
                initialHere.YPosition = "480";
                signer.Tabs.InitialHereTabs.Add(initialHere);

                Signer salesmanSigner = new Signer();
                salesmanSigner.Email = soldByAdminObject.Email;
                salesmanSigner.Name = soldByAdminObject.Name;
                salesmanSigner.RecipientId = (++recipientId).ToString();

                //Authentication required has been disabled through docusign support, if i need it in the future i need to call back

                // must set |clientUserId| to embed the recipient
                salesmanSigner.ClientUserId = soldByAdminObject.Id;

                // Create a |SignHere| tab somewhere on the document for the recipient to sign
                salesmanSigner.Tabs = new Tabs();
                salesmanSigner.Tabs.SignHereTabs = new List<SignHere>();
                SignHere salesmanSignHere = new SignHere();
                salesmanSignHere.DocumentId = "1";
                salesmanSignHere.PageNumber = "1";
                salesmanSignHere.RecipientId = recipientId.ToString();
                salesmanSignHere.XPosition = "15";
                salesmanSignHere.YPosition = "618";
                salesmanSigner.Tabs.SignHereTabs.Add(salesmanSignHere);

                bool samePerson = currVehicle.GetValue(PropertyId.SaleSoldBy) == currVehicle.GetValue(PropertyId.SaleManager);

                SignHere managerSignHere = new SignHere();
                managerSignHere.DocumentId = "1";
                managerSignHere.PageNumber = "1";
                if (samePerson)
                    managerSignHere.RecipientId = recipientId.ToString();
                else
                    managerSignHere.RecipientId = (++recipientId).ToString();

                managerSignHere.XPosition = "100";
                managerSignHere.YPosition = "700";

                Signer saleManagerSigner = null;
                if (!samePerson)
                {
                    saleManagerSigner = new Signer();
                    saleManagerSigner.Tabs = new Tabs();
                    saleManagerSigner.Tabs.SignHereTabs = new List<SignHere>();
                    string saleManagerName = currVehicle.GetValue(PropertyId.SaleManager);
                    foreach (UserAdminObject user in CacheManager.UserCache)
                    {
                        if (user.Name.Trim() == saleManagerName.Trim())
                        {
                            managerUserAdminObject = user;
                            break;
                        }
                    }

                    if (managerUserAdminObject == null)
                    {
                        MessageBox.Show("Unable to find a manager for the sale", "Error");
                        return;
                    }

                    saleManagerSigner.Email = managerUserAdminObject.Email;
                    saleManagerSigner.Name = managerUserAdminObject.Name;
                    saleManagerSigner.RecipientId = (recipientId).ToString();
                    saleManagerSigner.ClientUserId = managerUserAdminObject.Id;
                }

                if (samePerson)
                {
                    salesmanSigner.Tabs.SignHereTabs.Add(managerSignHere);
                }
                else
                {
                    saleManagerSigner.Tabs.SignHereTabs.Add(managerSignHere);
                }

                envDef.Recipients = new Recipients();
                envDef.Recipients.Signers = new List<Signer>();
                envDef.Recipients.Signers.Add(signer);
                envDef.Recipients.Signers.Add(salesmanSigner);
                if (!samePerson)
                {
                    envDef.Recipients.Signers.Add(saleManagerSigner);
                }

                // set envelope status to "sent" to immediately send the signature request
                envDef.Status = "sent";

                // use the EnvelopesApi to crate and send the signature request
                EnvelopesApi envelopesApi = new EnvelopesApi();
                EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

                Console.WriteLine("EnvelopeSummary:\n{0}", JsonConvert.SerializeObject(envelopeSummary));

                RecipientViewRequest customerViewOptions = new RecipientViewRequest()
                {
                    ReturnUrl = "https://www.docusign.net/restapi/v2/login_information",
                    ClientUserId = customer.ObjectId,  // must match clientUserId set in step #2!
                    AuthenticationMethod = "email",
                    UserName = customer.FirstName + " " + customer.LastName,
                    Email = customer.Email
                };

                RecipientViewRequest salesmanViewOptions = new RecipientViewRequest()
                {
                    ReturnUrl = "https://www.docusign.net/restapi/v2/login_information",
                    ClientUserId = soldByAdminObject.Id,  // must match clientUserId set in step #2!
                    AuthenticationMethod = "email",
                    UserName = soldByAdminObject.Name,
                    Email = soldByAdminObject.Email
                };

                RecipientViewRequest salesManagerViewOptions = null;
                if (!samePerson)
                {
                    salesManagerViewOptions = new RecipientViewRequest()
                    {
                        ReturnUrl = "https://www.docusign.net/restapi/v2/login_information",
                        ClientUserId = managerUserAdminObject.Id,  // must match clientUserId set in step #2!
                        AuthenticationMethod = "email",
                        UserName = managerUserAdminObject.Name,
                        Email = managerUserAdminObject.Email
                    };
                }

                lastSignedEnvelopeId = envelopeSummary.EnvelopeId;

                // create the recipient view (aka signing URL)
                ViewUrl saleManagerRecipientView = null;
                ViewUrl customerRecipientView = envelopesApi.CreateRecipientView(accountId, lastSignedEnvelopeId, customerViewOptions);
                ViewUrl salesmanRecipientView = envelopesApi.CreateRecipientView(accountId, lastSignedEnvelopeId, salesmanViewOptions);
                if (salesManagerViewOptions != null)
                {
                    saleManagerRecipientView = envelopesApi.CreateRecipientView(accountId, lastSignedEnvelopeId, salesManagerViewOptions);
                }

                // print the JSON response
                Console.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(customerRecipientView));
                Console.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(salesmanRecipientView));
                if (saleManagerRecipientView != null)
                    Console.WriteLine("ViewUrl:\n{0}", JsonConvert.SerializeObject(saleManagerRecipientView));
                // Start the embedded signing session
                //System.Diagnostics.Process.Start(recipientView.Url);

                // Twilio - Send text message
                // Your Account SID from twilio.com/console
                var accountSid = "ACc2c50961715eca028be4befe2146be4c";
                // Your Auth Token from twilio.com/console
                var authToken = "5ae0ee6184c26793eac3a841033cdd94";

                TwilioClient.Init(accountSid, authToken);

                string body = soldByAdminObject.Name + " click: " + salesmanRecipientView.Url;
                var salesmanMessage = MessageResource.Create(
                                to: new PhoneNumber("+12898133783"),
                                from: new PhoneNumber("+12897994365"),
                                body: body);

                Console.WriteLine(salesmanMessage.Sid);

                body = customer.FirstName + " " + customer.LastName + " click: " + customerRecipientView.Url;
                var customerMessage = MessageResource.Create(
                                to: new PhoneNumber("+12898133783"),
                                from: new PhoneNumber("+12897994365"),
                                body: body);

                Console.WriteLine(salesmanMessage.Sid);

                if (saleManagerRecipientView != null)
                {
                    body = managerUserAdminObject.Name + " click: " + saleManagerRecipientView.Url;
                    var saleManagerMessage = MessageResource.Create(
                                    to: new PhoneNumber("+12898133783"),
                                    from: new PhoneNumber("+12897994365"),
                                    body: body);

                    Console.WriteLine(saleManagerMessage.Sid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("SignDocument Exception: " + ex.Message);
            }
            /*
            webControl = new WebControlWindow(recipientView.Url, "event=signing_complete");
            webControl.CompletedURLReached += WebControl_CompletedURLReached;
            webControl.ShowDialog();
            */
        }

        private void WebControl_CompletedURLReached()
        {
            webControl.Hide();
            List<string> digitalSignatureFiles;
            digitalSignatureFiles = DownloadDocuments();

            if (DigitalSignatureDocumentsDownloaded == null)
            {
                return; 
            }

            DigitalSignatureDocumentsDownloaded(digitalSignatureFiles);

            webControl.Close();
        }
    }
}
