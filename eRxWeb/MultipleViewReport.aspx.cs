/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
** 06/08/2009  Dharani Reddem        Issue #2701-Mimic the display of patientcurrentMed report as in report tab instead pop up. 
 * 09/10/2009  Sonal                 Issue #2761- Provider Report (Detailed),Provider Report (basic),
 *                                   Provider Report (Patient on Behalf Of) and Pharmacy Refill Report 
 *                                   will not return a white screen on a provider with an apostrophe
 *                                   in their last name.
*******************************************************************************/
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
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;

namespace eRxWeb
{
public partial class MultipleViewReport : BasePage 
{
    public ReportAuditExtendedInfo ReportAuditInfo;

    private string reportID
    {
        get
        {
            string ret = string.Empty;
            if (Session["CURRENT_REPORT"] != null)
            {
                ret = Session["CURRENT_REPORT"].ToString();
            }

            return ret;
        }
        set
        {
            Session["CURRENT_REPORT"] = value;
        }
    }

    private string rptFrame = string.Empty;
    private int height = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ReportAuditInfo = new ReportAuditExtendedInfo
            {
                Action = AuditAction.REPORT_VIEWED,
                UserId = SessionUserID.ToGuidOr0x0(),
                LicenseId = SessionLicenseID.ToGuidOr0x0(),
                IpAddress = Request.UserIpAddress(),
                DbId = DBID
            };
        }

        if (!string.IsNullOrEmpty(Request.Params[btnAddNotes.UniqueID]))
        {
            btnAddNotes_Click(null, null);
        }
        else
        {
            //Fix the post back problem when the close button is clicked. 
            if (!string.IsNullOrEmpty(Request.Params[btnCancel.UniqueID]))
                btnCancel_Click(null, null);        

            height = ((PhysicianMasterPageBlank)Master).getTableHeight();

            if (!Page.IsPostBack)
            {
                Session["Report_Notes"] = null;
                try
                {
                    if (Request.QueryString["ReportID"] != null)
                        reportID = Request.QueryString["ReportID"].ToString().Trim();

                    makeIframe();

                    pnlFrame.Controls.Add(new LiteralControl(rptFrame));
                }

                catch (Exception ex)
                {
                    Response.Write("Error in Printing" + ex.ToString());
                }
            }
        }
    }    

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    private void PatientMedReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string medName = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["MedName"] != null)
            medName = Request.QueryString["MedName"].ToString().Trim();
        

        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
             Param("Start Date", startDate),
            Param("End Date", endDate),
            Param("Med Name", medName)
        };
        ReportAuditInfo.ReportName = "Medication Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

            var querystring = "StartDate=" + startDate + "&EndDate=" + endDate + "&MedName=" + Server.UrlEncode(medName);          
            rptFrame = GetIframeString(querystring);
            SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&MedName=" + Server.UrlEncode(medName) + "&ReportID=" + reportID); 
    }

    private void ProviderMedReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string providerName = string.Empty;
        //int providerID = 0;
        string providerID = String.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["ProviderName"] != null)
            providerName = Request.QueryString["ProviderName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            //Commented by AKS for Usercode deletion in DB
            // providerID = Convert.ToInt32(Request.QueryString["ProviderID"].ToString().Trim());
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

            
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
            Param("Start Date", startDate),
            Param("End Date", endDate),
            Param("Provider Name", providerName)
        };
        ReportAuditInfo.ReportName = "Provider Med Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var querystring = "StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;");        
        rptFrame = GetIframeString(querystring);
        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&ReportID=" + reportID); 
    }

    private void PatientMedHistory()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string patientID = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["PatientID"] != null)
            patientID = Request.QueryString["PatientID"].ToString().Trim();

        var patName = Convert.ToString(Request.QueryString["Name"]);
            
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
            Param("Start Date", startDate),
            Param("End Date", endDate),
            Param("Patient", patName)
        };
        ReportAuditInfo.ReportName = "Patient Medication Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

            var querystring = "StartDate=" + startDate + "&EndDate=" + endDate + "&PatientID=" + patientID;
        //rptFrame = "<iframe name='iframe1' id='iframe1' src='" + Constants.PageNames.PRINT_REPORT + "?StartDate=" + startDate + "&EndDate=" + endDate + "&PatientID=" + patientID + "&ReportID=" + reportID + "' align='top' class= 'printReportIframe' height='" + height + "' frameborder='0' title='Print Report' scrolling='no'>If you can see this, your browser does not support iframes! </iframe>";
            rptFrame = GetIframeString(querystring);
            SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&PatientID=" + patientID + "&ReportID=" + reportID); 
    }

    private void AuditLog()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        string userName = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Request.QueryString["UserName"] != null)
            userName = Request.QueryString["UserName"].ToString();

            var querystring = "StartDate=" + startDate + "&EndDate=" + endDate + "&UserName=" + userName;
            rptFrame = GetIframeString(querystring);
            SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&UserName=" + userName + "&ReportID=" + reportID); 
    }

    private void PatientAdd()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

       
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
            Param("Start Date", startDate),
            Param("End Date", endDate)
        };
        ReportAuditInfo.ReportName = "Patient Entry Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

            var queryString = "StartDate=" + startDate + "&EndDate=" + endDate;
            rptFrame = GetIframeString(queryString);
            SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ReportID=" + reportID); 
        //End  for  575 ID  AKS 
    }

    private void PrescriptionDetail()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        //AKS added for new report requirements 575 ID
        string providerName = string.Empty;
        string providerID = String.Empty;
        string medicationType = String.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        //Added for  575 ID  AKS 
        if (Request.QueryString["ProviderName"] != null)
            providerName = Request.QueryString["ProviderName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate),
           Param("Provider", providerName)
        };
        ReportAuditInfo.ReportName = "Provider Report (Basic)";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;");
        rptFrame = GetIframeString(queryString);
            //End  for  575 ID  AKS 

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&ReportID=" + reportID); 
    }

    //New Method for the Report same as prescriptiondetail method
    private void PrescriptionDetailCAS()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        //AKS added for new report requirements 575 ID
        string providerName = string.Empty;
        string providerID = String.Empty;
        string medicationType = String.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        //Added for  575 ID  AKS 
        if (Request.QueryString["ProviderName"] != null)
            providerName = Request.QueryString["ProviderName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate),
           Param("Provider", providerName)
        };
        ReportAuditInfo.ReportName = "Provider Report (Detailed)";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;");
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&ReportID=PrescriptionDetailOAS"); 
    }

    private void PrescriptionDetailPOB()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;
        //AKS added for new report requirements 575 ID
        string providerName = string.Empty;
        string providerID = String.Empty;
        string POBID = String.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        //Added for  575 ID  AKS 
        if (Request.QueryString["ProviderName"] != null)
            providerName = Request.QueryString["ProviderName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        if (Request.QueryString["POBID"] != null)
            POBID = Request.QueryString["POBID"].ToString().Trim();

        var pobName = Convert.ToString(Request.QueryString["POBName"]);

        ReportAuditInfo.ReportName = reportID.Contains("DUR") 
                    ? "Provider DUR Report (Prescribe on Behalf of)"
                    : "Provider Report (Prescribe on Behalf of)";
            
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate),
           Param("Provider", providerName),
           Param("POB Name", pobName)
        };

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&POBID=" + POBID;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&POBID=" + POBID + "&ReportID=" + reportID); 

    }

    private void POBToProvider()
    {
        string providerID = string.Empty;
        string pobID = string.Empty;
        string pobType = string.Empty;

        if (Request.QueryString["pobID"] != null)
            pobID = Request.QueryString["pobID"].ToString().Trim();

        if (Request.QueryString["pobType"] != null)
            pobType = Request.QueryString["pobType"].ToString().Trim();

        if (Request.QueryString["providerID"] != null)
            providerID = Request.QueryString["providerID"].ToString().Trim();

            var providerName = Convert.ToString(Request.QueryString["ProviderName"]);
            var pobName = Convert.ToString(Request.QueryString["POBName"]);
            var pobTypeName = Convert.ToString(Request.QueryString["POBTypeName"]);
            
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("POB Name", pobName),
           Param("POB Type", pobTypeName),
           Param("Provider Name", providerName)
        };
        ReportAuditInfo.ReportName = "POB to Provider Association Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "providerID=" + providerID + "&pobID=" + pobID + "&pobType=" + pobType;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?providerID=" + providerID + "&pobID=" + pobID + "&pobType=" + pobType + "&ReportID=" + reportID); 
    }

    private void ProviderToPOB()
    {
        string providerID = string.Empty;
        string pobID = string.Empty;
        string pobType = string.Empty;

        if (Request.QueryString["pobID"] != null)
            pobID = Request.QueryString["pobID"].ToString().Trim();

        if (Request.QueryString["pobType"] != null)
            pobType = Request.QueryString["pobType"].ToString().Trim();

        if (Request.QueryString["providerID"] != null)
            providerID = Request.QueryString["providerID"].ToString().Trim();

        var providerName = Convert.ToString(Request.QueryString["ProviderName"]);
        var pobName = Convert.ToString(Request.QueryString["POBName"]);
        var pobTypeName = Convert.ToString(Request.QueryString["POBTypeName"]);

       
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("POB Name", pobName),
           Param("POB Type", pobTypeName),
           Param("Provider Name", providerName)
        };
        ReportAuditInfo.ReportName = "Provider to POB Association Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "providerID=" + providerID + "&pobID=" + pobID + "&pobType=" + pobType;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?providerID=" + providerID + "&pobID=" + pobID + "&pobType=" + pobType + "&ReportID=" + reportID); 
    }

    //New Method for the Report same as prescriptiondetail method
    private void ProvidereRxActivityReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;        
        string providerName = string.Empty;
        string providerID = String.Empty;
        string includeCS = "A";

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        //Added for  575 ID  AKS 
        if (Request.QueryString["ProviderName"] != null)
            providerName = Request.QueryString["ProviderName"].ToString();

        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        if (Request.QueryString["Include"] != null)
            includeCS = Request.QueryString["Include"].ToString().Trim();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate),
           Param("Provider Name", providerName),
           Param("Med Type", includeCS == "A" ? "CS & Non CS" : "Non CS")
        };
        ReportAuditInfo.ReportName = "Provider Rx Activity Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

            var queryString = "StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&Include=" + includeCS.Replace("'", "&rsquo;");
            rptFrame = GetIframeString(queryString);
            SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ProviderID=" + providerID + "&ProviderName=" + providerName.Replace("'", "&rsquo;") + "&Include=" + includeCS.Replace("'", "&rsquo;") + "&ReportID=" + reportID);
    }

    //Added by Sandeep for Pharmacy Utilization Summary report
    private void PharmacyUtilizationSummary()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate)
        };
        ReportAuditInfo.ReportName = "Pharmacy Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ReportID=" + reportID); 
    }

    //Added by Sandeep for Pharmacy Utilization Detail report
    private void PharmacyUtilizationDetail()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate)
        };
        ReportAuditInfo.ReportName = "Pharmacy Detail Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ReportID=" + reportID); 
    }

    private void PharmacyRefillReport()
    {
        string providerID = string.Empty;

        providerID = Request.QueryString["ProviderID"];

        var providerName = Convert.ToString(Request.QueryString["ProviderName"]);
          
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Provider",providerName)
        };
        ReportAuditInfo.ReportName = "Pharmacy Refill Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "ProviderID=" + providerID;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?ProviderID=" + providerID + "&ReportID=" + reportID); 
    }

    private void RegistryCheckedReport()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;        

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["StartDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate)
        };
        ReportAuditInfo.ReportName = "Registry Checked Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());
        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate;
        rptFrame = GetIframeString(queryString);
        
        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ReportID=" + reportID); 
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        reportID = null;
        Session.Remove("PrintCurrentMed");
        Session.Remove("Report_Notes");
        if (Request.QueryString["To"] == null)
        {
                if(((PhysicianMasterPageBlank)Master).ReportsLinkURL == Constants.PageNames.REPORTS)
                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REPORTS);
                else
                    Response.Redirect(((PhysicianMasterPageBlank)Master).ReportsLinkURL);
        }
        else
        {
            if(Request.QueryString["To"] == "ReviewHistory")
                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.REVIEW_HISTORY);
            else
                Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()));
        }
    }

    private void PatientMedActive()
    {
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Patient", PageState.GetStringOrEmpty(Constants.SessionVariables.PatientName))
        };
        ReportAuditInfo.ReportName = "Patient Active Meds Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

        var queryString = "";
        rptFrame = GetIframeString(queryString);
        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?ReportID=" + reportID); 
    }

    protected void btnAddNotes_Click(object sender, EventArgs e)
    {
        Session["Report_Notes"] = txtNotes.Text;
        pnlFrame.Controls.Clear();
        height = ((PhysicianMasterPageBlank)Master).getTableHeight();
        makeIframe();
        pnlFrame.Controls.Add(new LiteralControl(rptFrame));
    }

    private void EPCSRightsAssignment()
    {
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Date", DateTime.UtcNow.ToShortDateString())
        };
        ReportAuditInfo.ReportName = "EPCS Rights Assignment Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());

            var queryString = "";
            rptFrame = GetIframeString(queryString);
            SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?ReportID=" + reportID);
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------
    // New ARN Reports

    private void PatientMedReconciliation()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate)
        };
        ReportAuditInfo.ReportName = "Med and Med Allergy Reconciliation Report";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());
        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ReportID=" + reportID); 
    }


    private void PatientMedReconciliationDetail()
    {
        string startDate = string.Empty;
        string endDate = string.Empty;

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();
        
        ReportAuditInfo.ReportParams = new List<KeyValuePair<string, string>> {
           Param("Start Date", startDate),
           Param("End Date", endDate)
        };
        ReportAuditInfo.ReportName = "MU Med List / Med Allergy List Patient Detail";

        Audit.AddAuditRecordAndSetExtendedInfoField(ReportAuditInfo, ref hdnAuditExtensionInfo, new AuditLog());
        var queryString = "StartDate=" + startDate + "&EndDate=" + endDate;
        rptFrame = GetIframeString(queryString);

        SetExportToExcelUrl(Constants.PageNames.EXPORT_REPORTS + "?StartDate=" + startDate + "&EndDate=" + endDate + "&ReportID=" + reportID);
    }
    //-----------------------------------------------------------------------------------------------------------------------------------

    private void SetExportToExcelUrl(string url)
    {
        hdnExportToExcel.Value = url;
    }

    private void makeIframe()
    {
        switch (reportID)
        {
            case "PatientMed":
                PatientMedReport();
                break;
            case "ProviderMed":
                ProviderMedReport();
                break;
            case "PatientMedHistory":
                PatientMedHistory();
                break;
            case "AuditLog":
                AuditLog();
                break;
            case "PrescriptionDetail":
                PrescriptionDetail();
                break;
            case "PrescriptionDetailCAS":
                PrescriptionDetailCAS();
                break;
            case "ProvidereRxActivityReport":
                ProvidereRxActivityReport();
                break;
            case "SuperPOBDUR":
            case "PrescriptionDetailPOB":
                PrescriptionDetailPOB();
                break;
            case "PharmacySummary":
                PharmacyUtilizationSummary();
                break;
            case "PharmacyDetail":
                PharmacyUtilizationDetail();
                break;
            case "PharmacyRefillReport":
                PharmacyRefillReport();
                break;
            case "PatientAdd":
                PatientAdd();
                break;
            case "PatientMedReconciliation":
                PatientMedReconciliation();
                break;
            case "PatientMedReconciliationDetail":
                PatientMedReconciliationDetail();
                break;
            case "PatientCurrentMeds":
                if (base.SessionLicense.EnterpriseClient.AllowReportNotes)
                {
                    panelNotesHeader.Visible = true;
                    panelNotes.Visible = true;
                    btnAddNotes.Visible = true;
                    txtNotes.Visible = true;
                    btnAddNotes.Attributes.Clear();
                    btnAddNotes.Attributes.Add("ReportID", reportID);
                }
                PatientMedActive();
                break;
            case "POBToProvider":
                POBToProvider();
                break;
            case "ProviderToPOB":
                ProviderToPOB();
                break;
            case "EPCSRightsAssignment":
                EPCSRightsAssignment();
                break;
            case "RegistryCheckedReport":
                RegistryCheckedReport();
                break;
        }
    }  
        
        private string GetIframeString(string queryParameter)
        {
            var isAmpersand = string.IsNullOrWhiteSpace(queryParameter) ? "" : "&";
            string iframeString = $"<iframe name='iframe1' id='iframe1' src='{ Constants.PageNames.PRINT_REPORT }?{queryParameter.ToHTMLEncode()}{isAmpersand}ReportID={reportID.ToHTMLEncode()}' align='top' class= 'printReportIframe' height='{ height }' frameborder='0' title='Print Report' scrolling='no'>If you can see this, your browser does not support iframes! </iframe>";
            return iframeString;
        }

        public KeyValuePair<string, string> Param(string paramName, string value)
        {
            return new KeyValuePair<string, string>(paramName, value);
        }
}

}