/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 06/15/2009   Dharani Reddem           Added PatientActiveDiagnosis Method to print active diagnosis 
*                                       on Print Current Medication report.
* 09/12/2009   Anand Kumar Krishnan     Defect#2811 - StartDate and EndDate datetime is trimmed in to 
 *                                      date string to show on report in PharmacyUtilizationSummary method
* 08/17/2010   Dharani Reddem           Hotfix - artf597106 - Patient Entry report runs on replica now . 
*******************************************************************************/
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Reports;

// Made changes to display the Converted Date based on the timezone 30 April..
namespace eRxWeb
{
public partial class PrintReport : BasePage
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
                    // ePrescribe Reports - All
                    case "ProviderMed":
                        ProviderMedReport();
                        break;  /* NU */
                    case "PatientMedHistory":
                        PatientMedHistory();
                        break;
                    case "PatientAdd":
                        PatientAddReport();
                        break;  //Added new for patientaccess report..
                    case "PatientMed":
                        PatientMedReport();
                        break;
                    case "PrescriptionDetail":
                        PrescriptionDetail();
                        break;
                    case "PrescriptionDetailCAS":
                        PrescriptionDetailCAS();
                        break;
                    case "PrescriptionDetailPOB":
                        PrescriptionDetailPOB();
                        break;
                    //Modified by AKs on DEC 28 for avoiding the crahing of reports in deployment server
                    case "PharmacySummary":
                        PharmacyUtilizationSummary();
                        break;
                    //Modified by AKs on DEC 28 for avoiding the crahing of reports in deployment server
                    case "PharmacyDetail":
                        PharmacyUtilizationDetail();
                        break;
                    case "PharmacyRefillReport":
                        PharmacyRefillReport();
                        break;
                    case "PatientCurrentMeds":
                        PatientMedActive();
                        break;
                    case "PatientMedReconciliation":
                        PatientMedReconciliation();
                        break;
                    case "PatientMedReconciliationDetail":
                        PatientMedReconciliationDetail();
                        break;
                    case "SuperPOBDUR":
                        SuperPOBDUR();
                        break;
                    case "POBToProvider":
                        POBToProvider();
                        break;
                    case "ProviderToPOB":
                        ProviderToPOB();
                        break;
                    case "ProvidereRxActivityReport":
                        ProvidereRxActivityReport();
                        break;
                    case "EPCSRightsAssignment":
                        EPCSRightsAssignment();
                        break;
                    case "RegistryCheckedReport":
                        RegistryCheckedReport();
                        break;
                }
              
                //Remove the export to excel option from report viewer.
                ReportViewerHelper.SetExportFormatVisibility(rptViewer, ReportViewerHelper.ExportFormat.Excel, false);
            }
            catch (Exception ex) 
            {
                Response.Write("Error in Printing " + ex.Message.ToString());
            }
        }
    }

    // TODO: BEGIN Referral Reports
    private void ReferralsByStatusReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
    }

    private void ReferralsViaMechanismReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
    }

    private void ReferralWorkflowStatusesReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
    }
    // TODO: END Referral Reports
//---------------------------------------------------------------------------------------------------------------------------------------------

    private void PatientMedActive()
    {
        string PrintDate = string.Empty;
        string userName = string.Empty;
        // RDR - Added site demographic information to display on header of current medication report.
        string address1 = string.Empty;
        string city = string.Empty;
        string state = string.Empty;
        string zipcode = string.Empty;
        string notes = string.Empty;

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        if (Session["Report_Notes"] != null)
            notes = Session["Report_Notes"].ToString();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();

        address1 = ds.Tables[0].Rows[0]["Address1"].ToString();
        city     = ds.Tables[0].Rows[0]["City"].ToString();
        state    = ds.Tables[0].Rows[0]["State"].ToString();
        zipcode  = ds.Tables[0].Rows[0]["ZipCode"].ToString();

        setPatientInformation(Session["PATIENTID"].ToString(), base.SessionLicenseID, Session["USERID"].ToString());

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientPersonalInfoSession_GetPatientByID", "ObjDSPatientPersonalInfo"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientAllergyInfo_GetAllergiesByPatientID", "ObjDSPatientAllergyInfo"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientMedActiveInfo_GetMedicationActiveByPatientID", "ObjDSPatientMedActiveInfo"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientDiagnosisInfo_GetActiveDiagnosisByPatientID", "ObjDSPatientDiagnosisInfo"));                        
        rptViewer.LocalReport.ReportPath = "Reports\\PatientCurrentMeds.rdlc";
        SetRptParamPatientMedActive(userName, siteName, PrintDate, address1, city, state, zipcode, notes);
        rptViewer.ShowDocumentMapButton = false;        
    }

    private void PatientMedReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string medName = string.Empty;
        string userName = string.Empty; //HA/Sandeep added this variable
        string siteName = string.Empty; //HA/Sandeep added this variable
        string PrintDate = string.Empty; //April 30 AKs

        if (!Page.IsPostBack)
        {
            if (Request.QueryString["StartDate"] != null)
                startDate = Request.QueryString["startDate"].ToString();

            if (Request.QueryString["EndDate"] != null)
                endDate = Request.QueryString["EndDate"].ToString();

            if (Request.QueryString["MedName"] != null)
                medName = Server.UrlDecode(Request.QueryString["MedName"].ToString().Trim());

            //HA/Sandeep added this statement to get the User Name to pass to the report
            if (Session["UserName"] != null)
                userName = Session["UserName"].ToString();
         
            //HA/Sandeep Added this logic to access the report
            rptViewer.LocalReport.DataSources.Clear();
            rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientMedName_GetPatientByMedicationName", "ObjDSPatientMedName"));
            DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
            siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
            PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
            rptViewer.LocalReport.ReportPath = "Reports\\PatientMed.rdlc";
            rptViewer.LocalReport.Refresh();
            SetRptParamPatientMed(startDate, endDate, medName, userName, siteName, PrintDate);
        }
    }

    private void PatientMedHistory()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string patientID = string.Empty;
        string userName = string.Empty;
        string PrintDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["PatientID"] != null)
            patientID = Request.QueryString["PatientID"].ToString().Trim();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
      
        setPatientInformationForPatMedReport(patientID, base.SessionLicenseID, base.SessionUserID);        

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientPersonalInfo_GetPatientByID", "ObjDSPatientPersonalInfo_PatMedReport"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientAllergyInfo_GetAllergiesByPatientID", "ObjDSPatientAllergyInfo_PatMedReport"));
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientMedInfo_GetMedicationByPatientID", "ObjDSPatientMedInfo"));
        rptViewer.LocalReport.ReportPath = "Reports\\PatientMedHistory.rdlc";

        SetRptParamPatientMedHistory(startDate, endDate, userName, siteName, PrintDate);
    }

    private void ProviderMedReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string providerName = string.Empty;
        string userName = string.Empty; //Added By Sandeep
        string siteName = string.Empty; //Added By Sandeep
        string providerID = string.Empty; 

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["ProviderName"] != null)
            providerName = Request.QueryString["ProviderName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        //Added By Sandeep
        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();
        
        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();    
        //JJ 96
        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("ProviderDrug_GetMedicationByProvider", "ObjDSProviderDrug"));
        rptViewer.LocalReport.ReportPath = "Reports\\ProviderDrug.rdlc";
        rptViewer.LocalReport.Refresh();
        SetRptParamProviderMed(startDate, endDate, providerName, userName, siteName); //Added by Sandeep
    }

    private void PatientAddReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = Session["UserName"].ToString();
        string PrintDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["UserName"] != null)
            userName = Request.QueryString["UserName"].ToString();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        string siteAddress = getSiteAddress(ds.Tables[0].Rows[0]);
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();

        ObjPatientAdd.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjPatientAdd.SelectParameters["EndDate"].DefaultValue = endDate;
        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientAddReport_GetPatientAddReport", "ObjPatientAdd"));
        rptViewer.LocalReport.ReportPath = "Reports\\PatientAdd.rdlc";
        SetReportParamPatientAdd(startDate, endDate, userName, siteName, siteAddress, PrintDate);
    }

    private void PatientMedReconciliation()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = Session["UserName"].ToString();
        string PrintDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["UserName"] != null)
            userName = Request.QueryString["UserName"].ToString();
        

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        string siteAddress = getSiteAddress(ds.Tables[0].Rows[0]);
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
        ObjectPatMedRec.SelectParameters["StartDate"].DefaultValue = startDate; 
        ObjectPatMedRec.SelectParameters["EndDate"].DefaultValue = endDate;
        ObjectPatMedRec.SelectParameters["LicenseID"].DefaultValue = base.SessionLicenseID;

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientMedRecReport_GetPatientMedRecReport", "ObjectPatMedRec"));
        rptViewer.LocalReport.ReportPath = "Reports\\PatientMedRecReport.rdlc";
        SetReportParamPatientMedRec(startDate, endDate, userName, siteName, siteAddress, PrintDate);
      
    }

    private void PatientMedReconciliationDetail()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = Session["UserName"].ToString();
        string PrintDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["UserName"] != null)
            userName = Request.QueryString["UserName"].ToString();

        ///1. Create the new data object and then change the impact.report method to make available this data.
        ///2. Change it here and uncomment this code.
        ///3. Create dataaccess method for this report and pass parameters.
        ///3. Code should be able to run correctly.

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        string siteAddress = getSiteAddress(ds.Tables[0].Rows[0]);
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
        ObjectPatMedRecDetail.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjectPatMedRecDetail.SelectParameters["EndDate"].DefaultValue = endDate;
        ObjectPatMedRecDetail.SelectParameters["LicenseID"].DefaultValue = base.SessionLicenseID;

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientMedRecDetailDataSet", "ObjectPatMedRecDetail"));
        rptViewer.LocalReport.ReportPath = "Reports\\PatientMedRecDetailReport.rdlc";
        SetReportParamPatientMedRec(startDate, endDate, userName, siteName, siteAddress, PrintDate);

    }

    private string getSiteAddress(DataRow dr)
    {
        StringBuilder address = new StringBuilder();

        if (dr["Address1"] != DBNull.Value)
        {
            address.Append(dr["Address1"].ToString());
            address.Append(", ");
        }

        if (dr["Address2"] != DBNull.Value)
        {            
            address.Append(dr["Address2"].ToString());
            address.Append(", ");
        }

        if (dr["City"] != DBNull.Value)
        {
            address.Append(dr["City"].ToString());
            address.Append(", ");
        }

        if (dr["State"] != DBNull.Value)
        {
            address.Append(dr["State"].ToString());            
        }

        if (dr["ZipCode"] != DBNull.Value)
        {
            address.Append(" ");
            address.Append(dr["ZipCode"].ToString());
        }

        return address.ToString();
    }
   
    private void PrescriptionDetail()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string providerID = string.Empty; //575 ID 
        string PrintDate = string.Empty;
        string reportTitle = string.Empty;
        

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        
        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        //Adding today 575 ...AKS
        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        reportTitle = "Provider Report (Basic)";

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID); 
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PrescriptionByProvider_CHGetPrescriptionByProvider", "ObjDSPrescriptionDetail"));
        rptViewer.LocalReport.ReportPath = "Reports\\PrescriptionDetail.rdlc";
        rptViewer.LocalReport.Refresh();        
        SetRptParamPrescriptionDetail(startDate, endDate, userName, siteName, reportTitle, PrintDate);        
    }

    //Added by AKS on APril 11 for the new Report that is generated...
    //New Method added for the new Report.
    private void PrescriptionDetailCAS()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string providerID = string.Empty;
        string PrintDate = string.Empty;
        string reportTitle = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        //Adding today 575 ...AKS
        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        reportTitle = "Provider Report (Detailed)";

		ObjDSPrescriptionDetailCAS.SelectParameters["ProviderID"].DefaultValue = providerID;
        ObjDSPrescriptionDetailCAS.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjDSPrescriptionDetailCAS.SelectParameters["EndDate"].DefaultValue = endDate;

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        //This is the dataset name that you get from clicking on properties in the rdlc file and you get it
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PrescriptionByProviderOAS_CHPrescriptionDetailOASReport", "ObjDSPrescriptionDetailCAS"));
        rptViewer.LocalReport.ReportPath = "Reports\\PrescriptionDetailCAS.rdlc";
        rptViewer.LocalReport.Refresh();
        SetRptParamPrescriptionDetailCAS(startDate, endDate, userName, siteName, reportTitle, PrintDate);
    }

    private void ProvidereRxActivityReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string providerID = string.Empty;
        string PrintDate = string.Empty;
        string reportTitle = string.Empty;
        string include = "A";

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        if (Request.QueryString["Include"] != null)
            include = Request.QueryString["Include"].ToString().Trim();

        reportTitle = "Provider eRx Activity Report";

        if (include.Equals("A", StringComparison.InvariantCultureIgnoreCase))
        {
            reportTitle += " for All Prescriptions (Including Controlled Substance)";
        }
        else
        {
            reportTitle += " for Non Controlled Substance Prescriptions";
        }

        ObjeRxProviderActivity.SelectParameters["ProviderID"].DefaultValue = providerID;
        ObjeRxProviderActivity.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjeRxProviderActivity.SelectParameters["EndDate"].DefaultValue = endDate;
        ObjeRxProviderActivity.SelectParameters["IncludeCS"].DefaultValue = include;

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        //This is the dataset name that you get from clicking on properties in the rdlc file and you get it
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("eRxProviderActivityReportDataset", "ObjeRxProviderActivity"));
        rptViewer.LocalReport.ReportPath = "Reports\\eRxProviderActivityReport.rdlc";
        rptViewer.LocalReport.Refresh();
        SetRptParameRxProviderActivityReport(startDate, endDate, userName, siteName, reportTitle, PrintDate, include);
    }
   
    private void SuperPOBDUR()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string providerID = string.Empty;
        string PrintDate = string.Empty;
        string POBID = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        if (Request.QueryString["POBID"] != null)
            POBID = Request.QueryString["POBID"].ToString().Trim();

        ObjDSPrescriptionDetailPOB.SelectParameters["ProviderID"].DefaultValue = providerID;
        ObjDSPrescriptionDetailPOB.SelectParameters["POBID"].DefaultValue = POBID;
        ObjDSPrescriptionDetailPOB.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjDSPrescriptionDetailPOB.SelectParameters["EndDate"].DefaultValue = endDate;

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        //This is the dataset name that you get from clicking on properties in the rdlc file and you get it
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DefaultReport_SuperPOBDUR", "ObjDSPrescriptionDetailPOB"));
        rptViewer.LocalReport.ReportPath = "Reports\\SuperPOBDUR.rdlc";
        rptViewer.LocalReport.Refresh();
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true);
        ReportParameter drugNameParam = new ReportParameter("DrugName", string.Empty, true);
        ReportParameter siteAddressParam = new ReportParameter("SiteAddress", string.Empty, true);
        ReportParameter loginUserParam = new ReportParameter("LoginUserName", userName, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, PrintDateParam, drugNameParam, siteAddressParam, loginUserParam });
    }

    private void PrescriptionDetailPOB()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string providerID = string.Empty;
        string PrintDate = string.Empty;
        string POBID = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        //Adding today 575 ...AKS
        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        if (Request.QueryString["POBID"] != null)
            POBID = Request.QueryString["POBID"].ToString().Trim();

        ObjDSPrescriptionDetailPOB.SelectParameters["ProviderID"].DefaultValue = providerID;
        ObjDSPrescriptionDetailPOB.SelectParameters["POBID"].DefaultValue = POBID;
        ObjDSPrescriptionDetailPOB.SelectParameters["StartDate"].DefaultValue = startDate;
        ObjDSPrescriptionDetailPOB.SelectParameters["EndDate"].DefaultValue = endDate;

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        //This is the dataset name that you get from clicking on properties in the rdlc file and you get it
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PrescriptionByProviderPOB_CHPrescriptionDetailPOBReport", "ObjDSPrescriptionDetailPOB"));
        rptViewer.LocalReport.ReportPath = "Reports\\PrescriptionDetailPOB.rdlc";
        rptViewer.LocalReport.Refresh();
        SetRptParamPrescriptionDetailPOB(startDate, endDate, userName, siteName, PrintDate);
    }

    private void POBToProvider()
    {
        string pobID = string.Empty;
        string pobType = string.Empty;
        string providerID = string.Empty;
        string PrintDate = string.Empty;

        if (Request.QueryString["pobID"] != null)
            pobID = Request.QueryString["pobID"].ToString().Trim();

        if (Request.QueryString["pobType"] != null)
            pobType = Request.QueryString["pobType"].ToString().Trim();

        if (Request.QueryString["providerID"] != null)
            providerID = Request.QueryString["providerID"].ToString().Trim();

        ObjDSPOBToProvider.SelectParameters["pobID"].DefaultValue = pobID;
        ObjDSPOBToProvider.SelectParameters["pobType"].DefaultValue = pobType;
        ObjDSPOBToProvider.SelectParameters["providerID"].DefaultValue = providerID;

        rptViewer.LocalReport.DataSources.Clear();

        //This is the dataset name that you get from clicking on properties in the rdlc file and you get it
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("POBToProvider_GetPOBToProviderAssociations", "ObjDSPOBToProvider"));
        rptViewer.LocalReport.ReportPath = "Reports\\POBToProvider.rdlc";
        rptViewer.LocalReport.Refresh();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();

        string userName = string.Empty;
        if (Session["UserName"] != null)
        {
            userName = Session["UserName"].ToString();
        }

        SetRptParamPOBToProvider(userName, siteName);
    }

    private void ProviderToPOB()
    {
        string pobID = string.Empty;
        string pobType = string.Empty;
        string providerID = string.Empty;

        if (Request.QueryString["pobID"] != null)
            pobID = Request.QueryString["pobID"].ToString().Trim();

        if (Request.QueryString["pobType"] != null)
            pobType = Request.QueryString["pobType"].ToString().Trim();

        if (Request.QueryString["providerID"] != null)
            providerID = Request.QueryString["providerID"].ToString().Trim();

        ObjDSProviderToPOB.SelectParameters["pobID"].DefaultValue = pobID;
        ObjDSProviderToPOB.SelectParameters["pobType"].DefaultValue = pobType;
        ObjDSProviderToPOB.SelectParameters["providerID"].DefaultValue = providerID;

        rptViewer.LocalReport.DataSources.Clear();

        //This is the dataset name that you get from clicking on properties in the rdlc file and you get it
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("ProviderToPOB_GetProviderToPOBAssociations", "ObjDSProviderToPOB"));
        rptViewer.LocalReport.ReportPath = "Reports\\ProviderToPOB.rdlc";
        rptViewer.LocalReport.Refresh();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();

        string userName = string.Empty;
        if (Session["UserName"] != null)
        {
            userName = Session["UserName"].ToString();
        }

        SetRptParamPOBToProvider(userName, siteName);
    }


    private void EPCSRightsAssignment()
    {
        string printDate = string.Empty;
        string timeZoneAbbreviation = string.Empty;

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        printDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
        timeZoneAbbreviation = ds.Tables[0].Rows[0]["TimeZoneAbbreviation"].ToString();
        
        string userName = string.Empty;
        if (Session["UserName"] != null)
        {
            userName = Session["UserName"].ToString();
        }

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("EPCSRightsAssignmentDS", "ObjEPCSRightsAssignment"));
        rptViewer.LocalReport.ReportPath = "Reports\\EPCSRightsAssignment.rdlc";
        rptViewer.LocalReport.Refresh();

        SetRptParamEPCSRightsAssignment(userName, siteName, printDate, timeZoneAbbreviation);
    }

    //Added by Sandeep for Pharmacy Utilization Summary report
    private void PharmacyUtilizationSummary()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string PrintDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = (DateTime.Parse(Request.QueryString["startDate"])).ToString("M/d/yyyy");

        if (Request.QueryString["EndDate"] != null)
            endDate = (DateTime.Parse(Request.QueryString["EndDate"])).ToString("M/d/yyyy"); 

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PharmacyUtilizationSummary_CHPharmacyUtilizationReport", "ObjDSPharmacyUtilizationSummary"));
        rptViewer.LocalReport.ReportPath = "Reports\\PharmacyUtilizationSummary.rdlc";
        rptViewer.LocalReport.Refresh();

        SetRptParamPharmacyUtilizationSummary(startDate, endDate, userName, siteName, PrintDate);
    }

    //Added by Sandeep for Pharmacy Utilization Detail report
    private void PharmacyUtilizationDetail()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;
        string PrintDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS
        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PharmacyUtilizationDetail_CHPharmacyUtilizationDetailReport", "ObjDSPharmacyUtilizationDetail"));
        rptViewer.LocalReport.ReportPath = "Reports\\PharmacyUtilizationDetail.rdlc";
        rptViewer.LocalReport.Refresh();
        SetRptParamPharmacyUtilizationDetail(startDate, endDate, userName, siteName, PrintDate);
    }

	private void PharmacyRefillReport()
	{
		string providerID = Request.QueryString["ProviderID"];
		string userName = (Session["UserName"] != null) ? Session["UserName"].ToString() : string.Empty;

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
		string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
		string PrintDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString(); // 30 APR AKS

		rptViewer.LocalReport.DataSources.Clear();
		rptViewer.LocalReport.DataSources.Add(new ReportDataSource("PharmacyRefillReport_CHPharmRefillReport", "ObjDSPharmacyRefillReport"));
		rptViewer.LocalReport.ReportPath = "Reports\\PharmacyRefillReport.rdlc";
		rptViewer.LocalReport.SetParameters(
			new ReportParameter[] {
			new ReportParameter("SiteName", siteName, true),
			new ReportParameter("UserName", userName, true),
			new ReportParameter("PrintDate", PrintDate, true) }
		);
		rptViewer.LocalReport.Refresh();
	}
    
    private void RegistryCheckedReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;        
        string userName = string.Empty;
        string printDate = string.Empty;

        if (!string.IsNullOrWhiteSpace(Request.QueryString["StartDate"]))
        {
            startDate = Request.QueryString["StartDate"].ToString();
        }
        if (!string.IsNullOrWhiteSpace(Request.QueryString["EndDate"]))
        {
            endDate = Request.QueryString["EndDate"].ToString();
        }        

        DataSet ds = ApplicationLicense.SiteLoad(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        string siteName = ds.Tables[0].Rows[0]["SiteName"].ToString();
        printDate = ds.Tables[0].Rows[0]["FormatedDate"].ToString();
        
        if (!string.IsNullOrWhiteSpace(PageState.GetStringOrEmpty("UserName")))
        {
            userName = PageState["UserName"].ToString();
        }

        rptViewer.LocalReport.DataSources.Clear();
        rptViewer.LocalReport.DataSources.Add(new ReportDataSource("RegistryCheckedDS", "ObjRegistryChecked"));
        rptViewer.LocalReport.ReportPath = "Reports\\RegistryChecked.rdlc";
        rptViewer.LocalReport.Refresh();

        SetRptParamRegistryCheckedReport(startDate, endDate, userName, siteName, printDate);
    }

    private void SetRptParamPatientMed(string startDate, string endDate, string medName, string userName, string siteName, string PrintDate) //Sandeep
    {
        ReportParameter startDateParameter = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParameter = new ReportParameter("ToDate", endDate, true);
        ReportParameter drugNameParameter = new ReportParameter("DrugName", medName, true);
        ReportParameter licenseIDParameter = new ReportParameter("UserName", userName, true);//HA/Sandeep added this statement to pass the user name to the report
        ReportParameter siteNameParameter = new ReportParameter("SiteName", siteName, true);//HA/Sandeep added this statement to pass the site name to the report
        ReportParameter PrintDateParameter = new ReportParameter("PrintDate", PrintDate, true);//Tod display date based on the timezone of the site
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParameter, endDateParameter, drugNameParameter, licenseIDParameter, siteNameParameter, PrintDateParameter });//Sandeep
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

    private void SetRptParamPatientMedActive(string userName, string siteName, string PrintDate, string address1, string city, string state, string zipcode, string notes)
    {
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true); //AKS 30 to display proper timezone formated date 
        ReportParameter address1Parm = new ReportParameter("Address1", address1, true);
        ReportParameter cityParm = new ReportParameter("City", city, true);
        ReportParameter stateParm = new ReportParameter("State", state, true);
        ReportParameter zipcodeParm = new ReportParameter("ZipCode", zipcode, true);
        ReportParameter notesParm = new ReportParameter("Notes", notes, true); 
        
        //this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { userNameParam, siteNameParam, PrintDateParam, address1Parm, cityParm, stateParm, zipcodeParm, notesParm });
    }

    private void SetRptParamProviderMed(string startDate, string endDate, string providerName, string userName, string siteName) //Added by sandeep
    {
        ReportParameter startDateParameter = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParameter = new ReportParameter("ToDate", endDate, true);
        ReportParameter ProviderNameParameter = new ReportParameter("ProviderName", providerName, true);
        ReportParameter UserNameParameter = new ReportParameter("UserName", userName, true); //Added by sandeep
        ReportParameter SiteNameParameter = new ReportParameter("SiteName", siteName, true); //Added by sandeep
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParameter, endDateParameter, ProviderNameParameter, UserNameParameter, SiteNameParameter }); //Added by sandeep
    }

    private void SetReportParamPatientAdd(string startDate, string endDate, string userName, string siteName, string siteAddress, string PrintDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true); 
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter siteAddressParam = new ReportParameter("SiteAddress", siteAddress, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, siteAddressParam, PrintDateParam });
    }

    private void SetReportParamPatientMedRec(string startDate, string endDate, string userName, string siteName, string siteAddress, string PrintDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter siteAddressParam = new ReportParameter("SiteAddress", siteAddress, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, siteAddressParam, PrintDateParam });
    }

    private void SetReportParamAuditLog(string startDate, string endDate, string userName, string siteName, string loginuserName)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true); //Sandeep
        ReportParameter loginUserNameParam = new ReportParameter("LoginUserName", loginuserName, true); //Sandeep
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, loginUserNameParam });
    }

    private void SetRptParamPrescriptionDetail(string startDate, string endDate, string userName, string siteName, string reportTitle, string printDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter reportTitleParam = new ReportParameter("ReportTitle", reportTitle, true);        
        ReportParameter printDateParam = new ReportParameter("PrintDate", printDate, true);               

        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, reportTitleParam, printDateParam });
    }

    private void SetRptParamPrescriptionDetailCAS(string startDate, string endDate, string userName, string siteName, string reportTitle, string printDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter reportTitleParam = new ReportParameter("ReportTitle", reportTitle, true);
        ReportParameter printDateParam = new ReportParameter("PrintDate", printDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, reportTitleParam, printDateParam });
    }

    private void SetRptParameRxProviderActivityReport(string startDate, string endDate, string userName, string siteName, string reportTitle, string PrintDate, string include)
    {
        ReportParameter startDateParam = new ReportParameter("StartDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("EndDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("Username", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter reportTitleParam = new ReportParameter("ReportTitle", reportTitle, true);
        ReportParameter printDateParam = new ReportParameter("PrintDate", PrintDate, true);
        ReportParameter includeCSParam = new ReportParameter("IncludeCS", include, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, reportTitleParam, printDateParam, includeCSParam });
    }

    private void SetRptParamPrescriptionDetailPOB(string startDate, string endDate, string userName, string siteName, string PrintDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, PrintDateParam });
    }

    private void SetRptParamPOBToProvider(string userName, string siteName)
    {
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { userNameParam, siteNameParam });
    }

    private void SetRptParamInactivatedParClass(string userName, string siteName, string printDate)
    {
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter printDateParam = new ReportParameter("PrintDate", printDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] {userNameParam, siteNameParam,printDateParam });

    }
    private void SetRptParamEPCSRightsAssignment(string userName, string siteName, string printDate, string timeZoneAbbreviation)
    {
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter printDateParam = new ReportParameter("PrintDate", printDate, true);
        ReportParameter timeZoneParam = new ReportParameter("TimeZone", timeZoneAbbreviation, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { userNameParam, siteNameParam, printDateParam, timeZoneParam });
    }
    private void SetRptParamProviderToPOB(string userName, string siteName)
    {
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { userNameParam, siteNameParam });
    }

    //Added by Sandeep for Pharmacy Utilization Summary Report
    private void SetRptParamPharmacyUtilizationSummary(string startDate, string endDate, string userName, string siteName, string PrintDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true); //30 April for PrintDate formated to timezone 
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, PrintDateParam });
    }

    //Added by Sandeep for Pharmacy Utilization Detail Report
    private void SetRptParamPharmacyUtilizationDetail(string startDate, string endDate, string userName, string siteName, string PrintDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", PrintDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, PrintDateParam });
    }
    
    private void SetRptParamRegistryCheckedReport(string startDate, string endDate, string userName, string siteName, string printDate)
    {
        ReportParameter startDateParam = new ReportParameter("FromDate", startDate, true);
        ReportParameter endDateParam = new ReportParameter("ToDate", endDate, true);
        ReportParameter userNameParam = new ReportParameter("UserName", userName, true);
        ReportParameter siteNameParam = new ReportParameter("SiteName", siteName, true);
        ReportParameter PrintDateParam = new ReportParameter("PrintDate", printDate, true);
        this.rptViewer.LocalReport.Refresh();
        this.rptViewer.LocalReport.SetParameters(new ReportParameter[] { startDateParam, endDateParam, userNameParam, siteNameParam, PrintDateParam });
    }

    public void btnPrint_Click(object sender, EventArgs e)
    {
        switch (reportID)
        {
            case "PatientMed":
            case "PatientMedHistory":
            case "PrescriptionDetail":
                    PrintCustomizedPortraitReports(); break;
            case "PrescriptionDetailCAS":
                    PrintCustomizedPortraitReports(); break;                    
            case "PrescriptionDetailPOB":
                    PrintCustomizedPortraitReports(); break;
            case "ProviderMed":
                    PrintLandscapeReports(); break;
                case "PharmacyRefillReport":
                    PrintCustomizedPortraitReports(); break;
                case "PharmacyDetail":
            case "ProvidereRxActivityReport": PrintLandscapeReports(); break; // Modified this code for AKS crashing of report in deployment  
            default: PrintReports(); break;
        }
    }

    protected void PrintReports()
    {

        Warning[] warnings;
        string[] streamids;
        string mimeType, encoding, extension;
        string deviceInfo = "<DeviceInfo>" +
                            "<OutputFormat>PDF</OutputFormat>" +
                            "<EmbedFonts>None</EmbedFonts>" +                           
                            "<PageWidth>10in</PageWidth>" +                            
                            "<PageHeight>11in</PageHeight>" +
                            "<MarginTop>0.0in</MarginTop>" +
                            "<MarginLeft>0.0in</MarginLeft>" +
                            "<MarginRight>0.0in</MarginRight>" +
                            "<MarginBottom>0.0in</MarginBottom>" +
                            "</DeviceInfo>";

        byte[] bytes = rptViewer.LocalReport.Render("PDF", deviceInfo, out mimeType, out encoding, out extension,
                                                    out streamids, out warnings);

        MemoryStream oStream = new MemoryStream(bytes);
        oStream.Read(bytes, 0, bytes.Length - 1);

        Response.Clear();
        Session["printPdf"] = oStream; redr();
    }

    protected void PrintLandscapeReports()
    {
        Warning[] warnings;
        string reporttype = "PDF", mimeType, encoding, extension;
        string[] streamids;

        //Modifed on Feb 13 to accomodate the no of records to be display and cut down on no of pages..
        string deviceInfo = "<DeviceInfo>" +
                            "<OutputFormat>PDF</OutputFormat>" +
                            "<EmbedFonts>None</EmbedFonts>" +                           
                            "<PageWidth>11in</PageWidth>" +
                            //"<PageHeight>14in</PageHeight>" +
                            "<PageHeight>14in</PageHeight>" +
                            "<MarginTop>.3in</MarginTop>" +
                            "<MarginLeft>1in</MarginLeft>" +
                            "<MarginRight>1in</MarginRight>" +
                            "<MarginBottom>.1in</MarginBottom>" +
                            "</DeviceInfo>";

        byte[] bytes = rptViewer.LocalReport.Render(reporttype, deviceInfo, out mimeType, out encoding, out extension, 
                                                    out streamids, out warnings);

        MemoryStream oStream = new MemoryStream(bytes);
        oStream.Read(bytes, 0, bytes.Length - 1);
        Response.Clear();
        Session["printPdf"] = oStream; redr();
    }
        protected void PrintCustomizedPortraitReports()
        {
            Warning[] warnings;
            string reporttype = "PDF", mimeType, encoding, extension;
            string[] streamids;

            //Modifed on Feb 13 to accomodate the no of records to be display and cut down on no of pages..
            string deviceInfo = "<DeviceInfo>" +
                                "<OutputFormat>PDF</OutputFormat>" +
                                "<EmbedFonts>None</EmbedFonts>" +
                                "<PageWidth>12in</PageWidth>" +                                
                                "<PageHeight>13in</PageHeight>" +
                                "<MarginTop>.3in</MarginTop>" +
                                "<MarginLeft>0in</MarginLeft>" +
                                "<MarginRight>0in</MarginRight>" +
                                "<MarginBottom>1in</MarginBottom>" +
                                "</DeviceInfo>";

            byte[] bytes = rptViewer.LocalReport.Render(reporttype, deviceInfo, out mimeType, out encoding, out extension,
                                                        out streamids, out warnings);

            MemoryStream oStream = new MemoryStream(bytes);
            oStream.Read(bytes, 0, bytes.Length - 1);
            Response.Clear();
            Session["printPdf"] = oStream; redr();
        }


        private void redr()
    {
        ClientScript.RegisterStartupScript(this.GetType(), null, $"printPdf('{Constants.PageNames.PRINT_PDF}');", true);
    }
    private void setPatientInformation(string PatientID, string LicenseID, string UserID)
    {
        ObjDSPatientAllergyInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientAllergyInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientAllergyInfo.SelectParameters["UserID"].DefaultValue = UserID;

        ObjDSPatientDiagnosisInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientDiagnosisInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientDiagnosisInfo.SelectParameters["UserID"].DefaultValue = UserID;

        ObjDSPatientMedActiveInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientMedActiveInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientMedActiveInfo.SelectParameters["UserID"].DefaultValue = UserID;

        ObjDSPatientPersonalInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientPersonalInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientPersonalInfo.SelectParameters["UserID"].DefaultValue = UserID;
    }
    private void setPatientInformationForPatMedReport(string PatientID, string LicenseID, string UserID)
    {
        ObjDSPatientAllergyInfo_PatMedReport.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientAllergyInfo_PatMedReport.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientAllergyInfo_PatMedReport.SelectParameters["UserID"].DefaultValue = UserID;
        ObjDSPatientAllergyInfo_PatMedReport.SelectParameters["DBID"].DefaultValue = base.DBID.ToString();

        ObjDSPatientMedActiveInfo.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientMedActiveInfo.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientMedActiveInfo.SelectParameters["UserID"].DefaultValue = UserID;

        ObjDSPatientPersonalInfo_PatMedReport.SelectParameters["PatientID"].DefaultValue = PatientID;
        ObjDSPatientPersonalInfo_PatMedReport.SelectParameters["LicenseID"].DefaultValue = LicenseID;
        ObjDSPatientPersonalInfo_PatMedReport.SelectParameters["UserID"].DefaultValue = UserID;
        ObjDSPatientPersonalInfo_PatMedReport.SelectParameters["DBID"].DefaultValue = base.DBID.ToString();

        }
}
}