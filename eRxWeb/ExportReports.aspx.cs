using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using Telerik.Web.UI;

namespace eRxWeb
{
public partial class ExportReports : BasePage
{
    string reportID = string.Empty;

    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {        
            if (Request.QueryString["ReportID"] != null)
                reportID = Request.QueryString["ReportID"].ToString().Trim();

            ExportToExcel(reportID);
        }

        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new ConfigurationManager());
    }  

    void ExportToExcel(string reportID)
    {
        string userName = string.Empty;
        string providerID = string.Empty;
        string pobID = string.Empty;
        string pobType = string.Empty;        
        string startDate = string.Empty;
        string endDate = string.Empty;
        string patientID = string.Empty;
        string planID = string.Empty;
        string payorID = string.Empty;
        string medicationName = string.Empty;
        string Schedules = string.Empty;       
        string sortCode = string.Empty;
        string SessionUserID = string.Empty;
        string UseReplica = "N";
        string reportTitle = string.Empty;
        string Include = string.Empty;
        string reportName = string.Empty;
        DataSet dsReports = new DataSet();
        DataTable dtReport = new DataTable();
        DataSet dsPatient = new DataSet();
        DataSet dsAllergies = new DataSet();
        DataSet dsDiagnosis = new DataSet();
        DataSet dsActiveMedication = new DataSet();

        if (Request.QueryString["StartDate"] != null)
            startDate = Request.QueryString["startDate"].ToString();

        if (Request.QueryString["EndDate"] != null)
            endDate = Request.QueryString["EndDate"].ToString();

        if (Session["UserName"] != null)
            userName = Session["UserName"].ToString();
        
        if (Request.QueryString["ProviderID"] != null)
            providerID = Request.QueryString["ProviderID"].ToString().Trim();

        if (Request.QueryString["MedName"] != null)
            medicationName = Request.QueryString["MedName"].ToString().Trim();

        if (Request.QueryString["PatientID"] != null)
            patientID = Request.QueryString["PatientID"].ToString().Trim();
        
        if (Request.QueryString["SortCd"] != null)
            sortCode = Request.QueryString["SortCd"].ToString();
        else
            sortCode = "D";       

        if (Request.QueryString["POBID"] != null)
            pobID = Request.QueryString["POBID"].ToString().Trim();

        if (Request.QueryString["pobID"] != null)
            pobID = Request.QueryString["pobID"].ToString().Trim();

        if (Request.QueryString["pobType"] != null)
            pobType = Request.QueryString["pobType"].ToString().Trim();

        if (Request.QueryString["Include"] != null)
            Include = Request.QueryString["Include"].ToString().Trim();
   
        reportTitle = "Provider Report (Basic)";
        switch (reportID)
        {
            case "PatientMed":
                if (string.Compare(UseReplica, "Y", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    dtReport = Medication.GetPatientMedList(startDate, endDate, medicationName, base.SessionLicenseID, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.REPLICA_DB,(int)base.DBID); 
                }
                else
                {
                    dtReport = Medication.GetPatientMedList(startDate, endDate, medicationName, base.SessionLicenseID, base.DBID,null);       
                }
                dsReports.Tables.Add(dtReport.Copy());
                reportName = "MedicationReport";
                break;            
            
            case "PatientMedHistory":
                dsPatient = Patient.GetPatientData(patientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                dsAllergies = Patient.GetPatientAllergy(patientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                 if (string.Compare(UseReplica, "Y", StringComparison.OrdinalIgnoreCase) == 0)
                 {
                     dsActiveMedication = Patient.GetPatientMedication(startDate, endDate, patientID, base.SessionLicenseID, base.SessionUserID, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.REPLICA_DB,(int)base.DBID); 
                 }
                 else
                 {
                     dsActiveMedication = Patient.GetPatientMedication(startDate, endDate, patientID, base.SessionLicenseID, base.SessionUserID, base.DBID,null);
                 }

                 foreach (DataTable dt in dsPatient.Tables)
                {
                    dt.TableName = "PatientData";
                    dsReports.Tables.Add(dt.Copy());                    
                }

                if (dsAllergies != null && dsAllergies.Tables.Count > 0)
                {

                    dsReports.Tables[0].Rows.Add(dsReports.Tables[0].NewRow());
                    DataRow dr = dsReports.Tables[0].NewRow();

                    dr[0] = "Active Allergies";

                    dsReports.Tables[0].Rows.Add(dr);


                    if (dsAllergies.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drAllergies in dsAllergies.Tables[0].Rows)
                        {
                            dr = dsReports.Tables[0].NewRow();
                            dr[0] = drAllergies[0].ToString();
                            dsReports.Tables[0].Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dsReports.Tables[0].NewRow();
                        dr[0] = "None Entered.";
                        dsReports.Tables[0].Rows.Add(dr);
                    }
                }            

                if (dsActiveMedication != null && dsActiveMedication.Tables.Count > 0)
                {
                    dsReports.Tables[0].Rows.Add(dsReports.Tables[0].NewRow());

                    DataRow dr = dsReports.Tables[0].NewRow();

                    dr[0] = "Active Medications";

                    dsReports.Tables[0].Rows.Add(dr);                 
                   
                    dr = dsReports.Tables[0].NewRow();
                    dr[0] = dsActiveMedication.Tables[0].Columns[0].ColumnName;
                    dr[1] = dsActiveMedication.Tables[0].Columns[1].ColumnName;
                    dr[2] = dsActiveMedication.Tables[0].Columns[2].ColumnName;
                    dr[3] = dsActiveMedication.Tables[0].Columns[3].ColumnName;
                    dr[4] = dsActiveMedication.Tables[0].Columns[4].ColumnName;
                    dr[5] = dsActiveMedication.Tables[0].Columns[5].ColumnName;
                    dr[6] = dsActiveMedication.Tables[0].Columns[6].ColumnName;

                    dsReports.Tables[0].Rows.Add(dr);

                    if (dsActiveMedication.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drActiveMedication in dsActiveMedication.Tables[0].Rows)
                        {
                            dr = dsReports.Tables[0].NewRow();

                            dr[0] = drActiveMedication[0];
                            dr[1] = drActiveMedication[1];
                            dr[2] = drActiveMedication[2];
                            dr[3] = drActiveMedication[3];
                            dr[4] = drActiveMedication[4];
                            dr[5] = drActiveMedication[5];
                            dr[6] = drActiveMedication[6];
                            dsReports.Tables[0].Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dsReports.Tables[0].NewRow();
                        dr[0] = "This patient currently has no medications.";
                        dsReports.Tables[0].Rows.Add(dr);
                    }

                }
                reportName = "PatientMedReport";
                break;
            case "PharmacyRefillReport":
                int? nodeId = null;
                if (string.Compare(UseReplica, "Y", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    nodeId = (int)base.DBID;
                    dsReports = Provider.GetPharmacyRefillReport(providerID, base.SessionLicenseID, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.REPLICA_DB, nodeId);
                }
                else
                {
                    dsReports = Provider.GetPharmacyRefillReport(providerID, base.SessionLicenseID, base.DBID, nodeId);
                }
                reportName = "PharmacyRefillReport";
                break;
            case "POBToProvider":
                dsReports = RxUser.GetPOBToProviderAssociations(pobID, pobType, providerID, base.SessionLicenseID, base.DBID);
                reportName = "POBtoProviderAssociationReport";
                break;
            case "ProviderToPOB":
                dsReports = RxUser.GetProviderToPOBAssociations(pobID, pobType, providerID, base.SessionLicenseID, base.DBID);
                reportName = "ProvidertoPOBAssociationReport";
                break;
            case "PatientCurrentMeds":

                dsPatient = Patient.GetPatientData(base.SessionPatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                dsAllergies = Patient.GetPatientAllergy(base.SessionPatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                dsDiagnosis = Patient.PatientActiveDiagnosis(base.SessionPatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                dsActiveMedication = Patient.GetPatientActiveMedication(base.SessionPatientID, base.SessionLicenseID, base.SessionUserID, base.DBID);

                foreach (DataTable dt in dsPatient.Tables)
                {
                    dt.TableName = "PatientData";
                    dsReports.Tables.Add(dt.Copy());                    
                }

                if (dsAllergies != null && dsAllergies.Tables.Count > 0)
                {

                    dsReports.Tables[0].Rows.Add(dsReports.Tables[0].NewRow());
                    DataRow dr = dsReports.Tables[0].NewRow();

                    dr[0] = "Active Allergies";

                    dsReports.Tables[0].Rows.Add(dr);


                    if (dsAllergies.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drAllergies in dsAllergies.Tables[0].Rows)
                        {
                            dr = dsReports.Tables[0].NewRow();
                            dr[0] = drAllergies[0].ToString();
                            dsReports.Tables[0].Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dsReports.Tables[0].NewRow();
                        dr[0] = "None Entered.";
                        dsReports.Tables[0].Rows.Add(dr);
                    }
                }

                if (dsDiagnosis != null && dsDiagnosis.Tables.Count > 0)
                {

                    dsReports.Tables[0].Rows.Add(dsReports.Tables[0].NewRow());
                    DataRow dr = dsReports.Tables[0].NewRow();

                    dr[0] = "Active Problems";

                    dsReports.Tables[0].Rows.Add(dr);

                    if (dsDiagnosis.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drDiagnosis in dsDiagnosis.Tables[0].Rows)
                        {
                            dr = dsReports.Tables[0].NewRow();
                            dr[0] = drDiagnosis[1].ToString() + " - " + drDiagnosis[0].ToString();
                            dsReports.Tables[0].Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dsReports.Tables[0].NewRow();
                        dr[0] = "None Entered.";
                        dsReports.Tables[0].Rows.Add(dr);
                    }
                }

                if (dsActiveMedication != null && dsActiveMedication.Tables.Count > 0)
                {
                    dsReports.Tables[0].Rows.Add(dsReports.Tables[0].NewRow());

                    DataRow dr = dsReports.Tables[0].NewRow();

                    dr[0] = "Active Medications";

                    dsReports.Tables[0].Rows.Add(dr);                 
                   
                    dr = dsReports.Tables[0].NewRow();
                    dr[0] = dsActiveMedication.Tables[0].Columns[0].ColumnName;
                    dr[1] = dsActiveMedication.Tables[0].Columns[1].ColumnName;
                    dr[2] = dsActiveMedication.Tables[0].Columns[2].ColumnName;
                    dr[3] = dsActiveMedication.Tables[0].Columns[3].ColumnName;
                    dr[4] = dsActiveMedication.Tables[0].Columns[4].ColumnName;
                    dr[5] = dsActiveMedication.Tables[0].Columns[5].ColumnName;                 
                    dsReports.Tables[0].Rows.Add(dr);

                    if (dsActiveMedication.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow drActiveMedication in dsActiveMedication.Tables[0].Rows)
                        {
                            dr = dsReports.Tables[0].NewRow();

                            dr[0] = drActiveMedication[0];
                            dr[1] = drActiveMedication[1];
                            dr[2] = drActiveMedication[2];
                            dr[3] = drActiveMedication[3];
                            dr[4] = drActiveMedication[4];
                            dr[5] = drActiveMedication[5];                            
                            dsReports.Tables[0].Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dsReports.Tables[0].NewRow();
                        dr[0] = "This patient currently has no active medications in eRx NOW.";
                        dsReports.Tables[0].Rows.Add(dr);
                    }

                }
                break;
            case "ProvidereRxActivityReport":
                dtReport = Allscripts.Impact.Reporting.GeteRxProviderActivity(startDate, endDate, providerID, base.SessionLicenseID, Include, reportID,
                    base.SessionUserID, "N", base.DBID);
                dsReports.Tables.Add(dtReport.Copy());
                reportName = "ProvidereRxActivityReport";
                break;
            case "EPCSRightsAssignment":
                dtReport = Allscripts.Impact.Reporting.GetEPCSRightsAssignment(base.SessionLicenseID, reportID, base.SessionUserID, "N", base.DBID);
                dtReport.Columns.Remove("UserGUID");
                dsReports.Tables.Add(dtReport.Copy());
                reportName = "EPCSRightsAssignment";
                break;
            case "RegistryCheckedReport":
                dtReport = Allscripts.Impact.Reporting.GetRegistryCheckedReport(startDate, endDate, base.SessionLicenseID, reportID,
                    base.SessionUserID, "N", base.DBID);
                dsReports.Tables.Add(dtReport.Copy());
                reportName = "RegistryCheckedReport";
                break;
            case "PrescriptionDetail": 
            case "PrescriptionDetailCAS": 
            case "PrescriptionDetailPOB":
            case "ProviderMed":           
            case "PharmacyDetail":
            case "PatientMedReconciliationDetail":

            default: if (reportID.Equals("PrescriptionDetail"))
                    {
                        reportName = "ProviderReport";
                    }
                    else if (reportID.Equals("PrescriptionDetailOAS"))
                    {
                        reportName = "ProviderReportDetail";
                    }
                    else if (reportID.Equals("PrescriptionDetailCAS"))
                    {
                        reportName = "ProviderReportDetail";
                    }
                    else if (reportID.Equals("PrescriptionDetailPOB"))
                    {
                        reportName = "ProviderReportPOB";
                    }
                    else if (reportID.Equals("SuperPOBDUR")) 
                    {
                        reportName = "ProviderDURReportPOB";
                    }
                    else if (reportID.Equals("PharmacySummary"))
                    {
                        reportName = "PharmacyReport";
                    }
                    else if (reportID.Equals("PatientAdd"))
                    {
                        reportName = "PatientEntryReport";
                    }
                    else if (reportID.Equals("PatientMedReconciliation"))
                    {
                        reportName = "MedicationAndMedAllergyReconciliationReport";
                    }
                    else if (reportID.Equals("PatientMedReconciliationDetail"))
                    {
                        reportName = "MedicationAndMedAllergyReconciliationDetailReport";
                    }
                    dtReport = Allscripts.Impact.Reporting.GetDefaultReportData(base.SessionLicenseID, base.SessionSiteID,
                   "", base.SessionUserID, providerID, pobID, startDate, endDate, patientID, medicationName, Schedules,
                   reportID, SessionUserID, UseReplica, base.DBID);
                   dsReports.Tables.Add(dtReport.Copy());
                   break;            
        }


        grdExportToExcel.DataSource = Remove(dsReports);
        grdExportToExcel.DataBind();


        grdExportToExcel.ExportSettings.OpenInNewWindow = true;
        grdExportToExcel.ExportSettings.ExportOnlyData = true;
        grdExportToExcel.ExportSettings.IgnorePaging = true;
        grdExportToExcel.ExportSettings.FileName = (reportName != string.Empty) ? reportName : reportID;
        grdExportToExcel.ExportSettings.Excel.FileExtension = "xls";
        grdExportToExcel.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
        grdExportToExcel.MasterTableView.ExportToExcel();
        
    }
    protected DataSet Remove(DataSet ds)
    {
        if (ds.Tables[0] != null)
        {
            switch (reportID)
            {
                case "PrescriptionDetail":
                case "PrescriptionDetailOAS":                    
                    ds.Tables[0].Columns.Remove("ProviderID");
                    break;
            }
                 
        }
        return ds;
    }
}


}