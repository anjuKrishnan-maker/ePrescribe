using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using ExpertPdf.HtmlToPdf;
using Allscripts.Impact;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using System.Net.Mail;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.AppCode;
using eRxWeb.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using RxUser = Allscripts.Impact.RxUser;

namespace eRxWeb
{
public partial class pdf : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["FC"] != null)
        {
            string FC_PRINT_URL = "FCPrint.aspx?FC_DDI=" + Request.QueryString["DDI"] + "&DBID=" + Request.QueryString["DBID"];
            ConvertToPdfAndWriteToResponse(FC_PRINT_URL);
        }
        else if (Request.QueryString["GeneralDoc"] != null)
        {
            ConvertToPdfAndWriteToResponse(Request.QueryString["GeneralDoc"]);
        }
        else if (Request.QueryString["RXIDS"] != null)
        {
            string rxids = Request.QueryString["RXIDS"].ToString().Trim();
            string pharmacyid = Request.QueryString["PHARMACYID"] != null ? Request.QueryString["PHARMACYID"].ToString().Trim() : "";
            if (!string.IsNullOrEmpty(rxids))
            {
                if (Request.QueryString["Type"] == "PatEd")
                {
                    printPatientEducation(rxids);
                }
                else if (Request.QueryString["Type"] == "PatReceipt")
                {
                    printPatientReceipt(rxids);
                }
            }
        }
        else             
        {
            Session["DBID"] = (ConnectionStringPointer)Enum.Parse(typeof(ConnectionStringPointer), Request.QueryString["DBID"]);
            bool execute = true;
            string prescriberID = string.Empty;
            string licenseID = string.Empty;
            int siteID = 0;
            
            
            prescriberID = Session["USERID"].ToString();
            siteID = Convert.ToInt32(Session["SITEID"]);
            licenseID = Session["LICENSEID"].ToString();
            

            string state = string.Empty;
            if (execute && !string.IsNullOrEmpty(prescriberID) && siteID != 0)
            {
                Session["LICENSEID"] = licenseID;
                Session["SITEID"] = Convert.ToInt32(siteID);
                DataSet dsSite = ApplicationLicense.SiteLoad(licenseID, Convert.ToInt32(siteID), base.DBID);
                state = dsSite.Tables[0].Rows[0]["State"].ToString();

                //first check to see if the state of the site is eligible for 4up.
                int statePrintFormats = SystemConfig.GetStatePrintFormats(state);
                ApplicationLicense license = new ApplicationLicense(licenseID, Convert.ToInt32(siteID), base.DBID);
                bool bFourUp = SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURUP, statePrintFormats) && license.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On;
                bool bFourRow = SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURROW, statePrintFormats);
                string printingPreference = SitePreference.GetPreference(licenseID, siteID, "PRINTINGPREFERENCE", base.DBID);
                switch (printingPreference)
                {
                    case "4":
                        printingPreference = bFourUp ? "4Up" : "1Up";
                        break;
                    case "R":
                        printingPreference = bFourRow ? "4Row" : "1Up";
                        break;
                }

                Session["PRINTINGPREFERENCE"] = printingPreference;

                if (Session["USERID"] == null)
                {
                    DataSet dsUser = RxUser.Load(prescriberID, base.DBID);
                    Session["UserType"] = dsUser.Tables["User"].Rows[0]["UserRole"].ToString();
                    Session["UserID"] = prescriberID;
                    dsUser = null;
                }
            }
            else
            {
                //licenseIDs don't match...get rid of the hacker!
                Session.Abandon();
            }

            var rxIds = new List<Guid>();

            var id = Convert.ToString(Request.QueryString["id"]);
            if (!string.IsNullOrWhiteSpace(id))
            {
                rxIds.Add(id.ToGuidOr0x0());
            }
            else if (Session["ProcessList"] != null)
            {
                var al = (ArrayList)Session["ProcessList"];
                rxIds.AddRange(al.ToList<string>().Select(_ => _.ToGuidOr0x0()));
                Session["ProcessList"] = null;
            }
            
            doit(rxIds, state);
        }

        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());
    }

    private void printPatientReceipt(string rxids)
    {
        char[] delimiterChars = { ';' }; string downloadURL = string.Empty;
        string[] RXIDS = rxids.Split(delimiterChars);
        ArrayList alPatientReceiptRXIDs = new ArrayList();
        foreach (object rxID in RXIDS)
        {
            alPatientReceiptRXIDs.Add(rxID);
        }
        Session["PatientReceiptList"] = alPatientReceiptRXIDs;
        Session["DBID"] = (ConnectionStringPointer)Enum.Parse(typeof(ConnectionStringPointer), Request.QueryString["DBID"]);
        string prescriberID = string.Empty; prescriberID = Request.QueryString["UserID"];
        DataSet dsUser = RxUser.Load(prescriberID, base.DBID);
        Session["UserType"] = dsUser.Tables["User"].Rows[0]["UserRole"].ToString();
        Session["UserID"] = prescriberID;
        Session["UserName"] = dsUser.Tables["User"].Rows[0]["FirstName"].ToString() + " " + dsUser.Tables["User"].Rows[0]["LastName"].ToString();
        dsUser = null;
        
        downloadURL = Request.QueryString["Doc"].ToString().Trim();
       
        ConvertToPdfAndWriteToResponse(downloadURL);
    }

    private void doit(List<Guid> rxIds, string state)
    {
        var printRxRequest = new PrintRxRequest
        {
            LicenseId = SessionLicenseID.ToGuidOr0x0(),
            DelegateProviderId = SessionDelegateProviderID.ToGuidOr0x0(),
            State = string.IsNullOrWhiteSpace(Request.QueryString["StateOverride"]) ? state : Request.QueryString["StateOverride"],
            PrintAction = PrintAction.OUTPUT_PDF,
            PrintMode = RetrievePrintMode(PageState.GetStringOrEmpty("PRINTINGPREFERENCE")),
            RxIdList = rxIds.ToArray(),
            SiteId = SessionSiteID,
            TimeZone = PageState.GetStringOrEmpty("TimeZone"),
            UserType = Session["UserType"].ToEnum<Constants.UserCategory>().ToString().ToEnum<UserCategory>(),
            DbID = (ePrescribeSvc.ConnectionStringPointer?) DBID
        };

        var pdfResponse = EPSBroker.PrintRxs(printRxRequest);
        
        WritePdfToResponse(pdfResponse.PdfBase64);
    }

    private PrintMode RetrievePrintMode(string printingPreference)
    {
        if(string.IsNullOrWhiteSpace(printingPreference)) return PrintMode.ONE_UP;

        switch (printingPreference.ToLower())
        {
                case "1up":
                    return PrintMode.ONE_UP;
                case "4up":
                    return PrintMode.FOUR_UP;
                case "4row":
                    return PrintMode.FOUR_ROW;
                default:
                    return PrintMode.ONE_UP;
        }
    }

    private void ConvertToPdfAndWriteToResponse(string downloadURL)
    {
        var sw = new StringWriter();

        downloadURL = downloadURL.Replace("%3f", "?");
        downloadURL = downloadURL.Replace("%26", "&");
        Server.Execute(downloadURL, sw);

        string htmlCodeToConvert = sw.GetStringBuilder().ToString();

        var response = EPSBroker.ConvertFromHtmlToPdf(htmlCodeToConvert, PdfDocumentSettingsType.GENERAL);

        WritePdfToResponse(response.PdfBase64);
    }

    private void WritePdfToResponse(string pdfBase64)
    {
        var bytes = Convert.FromBase64String(pdfBase64);
        Response.Clear();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-length", bytes.Length.ToString());
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }


    private void printPatientEducation(string rxids)
    {
        string htmlCodeToConvert = string.Empty;

        char[] delimiterChars = { ';' };
        string[] RXIDS = rxids.Split(delimiterChars);
        string licenseID = string.Empty;
        string userID = string.Empty;

        int numrxs = RXIDS.Length;
        for (int l = 0; l < numrxs; l++)
        {
            DataSet ds = Prescription.Load(RXIDS[l], base.DBID);
            if (ds.Tables[1].Rows.Count > 0)
            {
                string ddi = ds.Tables[1].Rows[0]["DDI"].ToString();

                DataSet dsEd = CHPatient.GetPatientEducation(ddi, licenseID, userID, base.DBID);
                if (dsEd.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsEd.Tables[0].Rows[0];
                    htmlCodeToConvert += dr["GenericMsg"].ToString().Replace("GENERIC NAME:", "<b>GENERIC NAME:</b>") + "<br />";
                    htmlCodeToConvert += dr["CommMsg"].ToString().Replace("COMMON USES:", "<b>COMMON USES:</b>") + "<br />";
                    htmlCodeToConvert += dr["HowMsg"].ToString().Replace("HOW TO USE THIS MEDICINE:", "<b>HOW TO USE THIS MEDICINE:</b>") + "<br />";
                    htmlCodeToConvert += dr["CautionsMsg"].ToString().Replace("CAUTIONS:", "<b>CAUTIONS:</b>") + "<br />";
                    htmlCodeToConvert += dr["SideEffectMsg"].ToString().Replace("POSSIBLE SIDE EFFECTS:", "<b>POSSIBLE SIDE EFFECTS:</b>") + "<br />";
                    htmlCodeToConvert += dr["BeforeMsg"].ToString().Replace("BEFORE USING THIS MEDICINE:", "<b>BEFORE USING THIS MEDICINE:</b>") + "<br />";
                    htmlCodeToConvert += dr["OverDoseMsg"].ToString().Replace("OVERDOSE:", "<b>OVERDOSE:</b>") + "<br />";
                    htmlCodeToConvert += dr["AdditionMsg"].ToString().Replace("ADDITIONAL INFORMATION:", "<b>ADDITIONAL INFORMATION:</b>") + "<br />";
                    htmlCodeToConvert += "<br /><br />";
                }
            }
        }

        var response = EPSBroker.ConvertFromHtmlToPdf(htmlCodeToConvert, PdfDocumentSettingsType.GENERAL);

        WritePdfToResponse(response.PdfBase64);
    }
}

}