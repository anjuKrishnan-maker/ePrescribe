using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Allscripts.Impact;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Text;

namespace eRxWeb
{
public partial class PrintReportDynamic : BasePage
{
    string reportID = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        rptViewer.DocumentMapCollapsed = true;

        if (Request.QueryString["ReportID"] != null)
            reportID = Request.QueryString["ReportID"].ToString().Trim();

        if (!Page.IsPostBack)
        {
            try
            {
                if (Session["PAGEHEIGHT"] != null)
                    rptViewer.Height = Convert.ToInt32(Session["PAGEHEIGHT"]) - 240;
                else
                    rptViewer.Height = 400;

                switch (reportID)
                {
                    case "PatientCurrentMeds":
                        PatientMedActive();
                        break;

                    default:
                        DefaultReport();
                        break;
                }
            }
            catch (Exception ex)
            {
                Response.Write("Error in Printing" + ex.Message.ToString());
            }
        }

    }

    private void PatientMedActive()
    {
        string PrintDate = string.Empty;
        string userName = string.Empty;
        if (Session["UserName"] != null)
        {
            userName = Session["UserName"].ToString();
        }

        string LicenseID = Session["LICENSEID"].ToString();
        DataSet ds = ApplicationLicense.SiteLoad(LicenseID, Convert.ToInt32(Session["SITEID"]), base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_PatientMedHistory", "ObjDSPatientMedActiveInfo"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_PatientMedHistory_Person", "ObjDSPatientPersonalInfo"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_PatientMedHistory_Allergy", "ObjDSPatientAllergyInfo"));       
        rptViewer.LocalReport.ReportPath = "Reports\\PatientCurrentMeds.rdlc";
        SetRptParamPatientMedActive(userName, siteName, PrintDate);
        rptViewer.ShowDocumentMapButton = false;
    }

    private string getSiteAddress(DataRow dr)
    {
        StringBuilder address = new StringBuilder();

        if (dr["Address1"] != DBNull.Value)
            address.Append(dr["Address1"].ToString());

        if (dr["Address2"] != DBNull.Value)
        {
            address.Append(", ");
            address.Append(dr["Address2"].ToString());
        }

        if (dr["City"] != DBNull.Value)
        {
            address.Append(dr["City"].ToString());
            address.Append(", ");
        }

        if (dr["State"] != DBNull.Value)
        {
            address.Append(dr["State"].ToString());
            address.Append(" ");
        }

        if (dr["ZipCode"] != DBNull.Value)
            address.Append(dr["ZipCode"].ToString());

        return address.ToString();
    }

    private void DefaultReport()
    {
        string userName = Session["UserName"].ToString();
        string startDate = string.Empty;
        string endDate = string.Empty;
        string user = string.Empty;
        string site = string.Empty;
        string patient = string.Empty;
        string provider = string.Empty;
        string pob = string.Empty;
        string medication = string.Empty;
        string schedules = string.Empty;
        string plan = string.Empty;
        string payor = string.Empty;
        string path = string.Empty;
        string id = string.Empty;
        string sortcd = string.Empty;
        string useReplica = "N";

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["UserID"] != null)
            user = Request.QueryString["UserID"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            provider = Request.QueryString["ProviderID"].ToString();

        if (Request.QueryString["POBID"] != null)
            pob = Request.QueryString["POBID"].ToString();

        if (Request.QueryString["PatientID"] != null)
            patient = Request.QueryString["PatientID"].ToString();

        if (Request.QueryString["PlanID"] != null)
            plan = Request.QueryString["PlanID"].ToString();

        if (Request.QueryString["PayorID"] != null)
            payor = Request.QueryString["PayorID"].ToString();

        if (Request.QueryString["DDI"] != null)
            medication = Request.QueryString["DDI"].ToString();

        if (Request.QueryString["Schedules"] != null)
            schedules = Request.QueryString["Schedules"].ToString();

        if (Request.QueryString["SiteID"] != null)
            site = Request.QueryString["SiteID"].ToString();

        if (Request.QueryString["SortCd"] != null)
            sortcd = Request.QueryString["SortCd"].ToString();

        if (Request.QueryString["UseReplica"] != null)
            useReplica = Request.QueryString["UseReplica"].ToString();

        if (Request.QueryString["ReportPath"] != null)
            path = Request.QueryString["ReportPath"].ToString();

        if (Request.QueryString["ReportID"] != null)
            id = Request.QueryString["ReportID"].ToString();

        int SiteID = 1;

        if (!string.IsNullOrEmpty(site))
        {
            SiteID = Convert.ToInt32(site);
        }

        DataSet ds = ApplicationLicense.SiteLoad(SessionLicenseID, SiteID, base.DBID);
        string siteName = string.Empty;
        string siteAddress = string.Empty;
        string medName = string.Empty;
        string PrintDate = DateTime.Now.ToString("U");

        if (!string.IsNullOrEmpty(medication))
        {
            medName = Allscripts.Impact.Medication.GetNameFromDDI(medication, base.DBID);
        }
        else
        {
            medName = "Empty";
        }

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
            siteAddress = getSiteAddress(ds.Tables[0].Rows[0]);
        }

        ObjDSDefaultReport.SelectParameters["LicenseID"].DefaultValue = SessionLicenseID;
        ObjDSDefaultReport.SelectParameters["SiteID"].DefaultValue = SiteID.ToString();
        ObjDSDefaultReport.SelectParameters["UserID"].DefaultValue = user;
        ObjDSDefaultReport.SelectParameters["ProviderID"].DefaultValue = provider;
        ObjDSDefaultReport.SelectParameters["POBID"].DefaultValue = pob;
        ObjDSDefaultReport.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjDSDefaultReport.SelectParameters["EndDate"].DefaultValue = endDate;
        ObjDSDefaultReport.SelectParameters["PatientID"].DefaultValue = patient;
        ObjDSDefaultReport.SelectParameters["PlanID"].DefaultValue = plan;
        ObjDSDefaultReport.SelectParameters["PayorID"].DefaultValue = payor;
        ObjDSDefaultReport.SelectParameters["DDI"].DefaultValue = medication;
        ObjDSDefaultReport.SelectParameters["Schedules"].DefaultValue = schedules;
        ObjDSDefaultReport.SelectParameters["ReportID"].DefaultValue = id;
        ObjDSDefaultReport.SelectParameters["SortCd"].DefaultValue = sortcd;
        ObjDSDefaultReport.SelectParameters["UseReplica"].DefaultValue = useReplica;
        ObjDSDefaultReport.SelectParameters["SessionUserID"].DefaultValue = SessionUserID;

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_" + id, "ObjDSDefaultReport"));

        if (id == "PatientMedHistory")
        {
            setPatientInformation(patient, SessionLicenseID, SessionUserID);
            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_PatientMedHistory_Person", "ObjDSPatientPersonalInfo"));
            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_PatientMedHistory_Allergy", "ObjDSPatientAllergyInfo"));            
        }

        rptViewer.LocalReport.ReportPath = "Reports\\" + path;

        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true);
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter drugNameParameter = new ReportParameter("DrugName", medName, true);
        ReportParameter siteAddressParam = new ReportParameter("SiteAddress", siteAddress, true);
        ReportParameter loginUserNameParam = new ReportParameter("LoginUserName", userName, true);
        rptViewer.LocalReport.SetParameters(new ReportParameter[] { userNameParam, siteNameParam, PrintDateParam, startDateParam, 
                                                                    endDateParam, drugNameParameter, siteAddressParam, loginUserNameParam });
        rptViewer.ShowDocumentMapButton = false;
    }

    private void SetRptParamPatientMedHistory(string startDate, string endDate, string userName, string siteName, string PrintDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true); //AKS 30 to display proper timezone formated date 
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, PrintDateParam });
    }
    private void SetRptParamPatientMedActive(string userName, string siteName, string PrintDate)
    {
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true); //AKS 30 to display proper timezone formated date 
        //this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { userNameParam, siteNameParam, PrintDateParam });
    }

    public void btnPrint_Click(object sender, EventArgs e)
    {
        switch (reportID)
        {

            case "PatientMed":
            case "PatientMedHistory":
            case "PrescriptionDetail":
            case "PrescriptionDetailCAS": //Added by AKS 14 April
            case "PrescriptionDetailPOB":
            case "ProviderMed":
            case "PharmacyRefillReport":
            case "PharmacyDetail":
                PrintLandscapeReports();  // Modified this code for AKS crashing of report in deployment
                break;
            default: PrintReports();
                break;

        }
    }
    protected void PrintReports()
    {

        Warning[] warnings;
        string[] streamids;
        string mimeType;
        string encoding;
        string extension;

        string deviceInfo = "<DeviceInfo>" +

              "<OutputFormat>PDF</OutputFormat>" +
              "<PageWidth>12.5in</PageWidth>" +             
              "<PageHeight>8.5in</PageHeight>" +
              "<MarginTop>0.3in</MarginTop>" +
              "<MarginLeft>1in</MarginLeft>" +
              "<MarginRight>1in</MarginRight>" +
              "<MarginBottom>0.1in</MarginBottom>" +
              "</DeviceInfo>";

        byte[] bytes = rptViewer.LocalReport.Render(
           "PDF", deviceInfo, out mimeType, out encoding,
            out extension,
           out streamids, out warnings);

        MemoryStream oStream = new MemoryStream(bytes);

        oStream.Read(bytes, 0, bytes.Length - 1);

        Response.Clear();

        Response.Buffer = true;

        Response.ContentType = "application/pdf";

        Response.BinaryWrite(oStream.ToArray());

        Response.End();

    }
    protected void PrintLandscapeReports()
    {

        Warning[] warnings;

        string reporttype = "PDF";

        string mimeType;

        string encoding;

        string extension;

        string[] streamids;
        //Modifed on Feb 13 to accomodate the no of records to be display and cut down on no of pages..

        string deviceInfo = "<DeviceInfo>" +

               "<OutputFormat>PDF</OutputFormat>" +
               "<PageWidth>12.5in</PageWidth>" +
               "<PageHeight>8.5in</PageHeight>" +
               "<MarginTop>.3in</MarginTop>" +
               "<MarginLeft>1in</MarginLeft>" +
               "<MarginRight>1in</MarginRight>" +
               "<MarginBottom>.1in</MarginBottom>" +
               "</DeviceInfo>";

        byte[] bytes = rptViewer.LocalReport.Render(

        reporttype, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

        MemoryStream oStream = new MemoryStream(bytes);

        oStream.Read(bytes, 0, bytes.Length - 1);

        Response.Clear();

        Response.Buffer = true;

        Response.ContentType = "application/pdf";

        Response.BinaryWrite(oStream.ToArray());

        Response.End();

    }

    private void setPatientInformation(string PatientID, string LicenseID, string UserID)
    {
        ObjDSPatientAllergyInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientAllergyInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientAllergyInfo.SelectParameters["UserID"].DefaultValue = UserID;

        ObjDSPatientPersonalInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientPersonalInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientPersonalInfo.SelectParameters["UserID"].DefaultValue = UserID;
    }
}


}