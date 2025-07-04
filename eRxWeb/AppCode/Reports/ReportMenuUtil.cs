using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Reports
{
    public class ReportMenuUtil
    {
        public ReportMenuModel GenerateReportMenu(Constants.DeluxeFeatureStatus gCodeReportDeluxeFeatureStatus)
        {
            var menu = new ReportMenuModel();

            menu.ReportGroupList = new List<ReportGroup>();

            menu.ReportGroupList.Add(GetProviderReports(gCodeReportDeluxeFeatureStatus));
            menu.ReportGroupList.Add(GetPatientReports());
            menu.ReportGroupList.Add(GetPharmacyReports());
            menu.ReportGroupList.Add(GetMedicationReports());
            menu.ReportGroupList.Add(GetSettingsReports());
            menu.ReportGroupList.Add(GetAuditReports());

            return menu;
        }



        private ReportGroup GetPatientReports()
        {
            var patientReport = new ReportGroup("Patient Reports");
            patientReport.ReportLinkList = new List<ReportLink>();

            patientReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Patient Medication Report",
                Description = "Tracks patient’s medication activity (including prescriptions, patient reported meds, etc.)",
                Page = "PatientMedHistory.aspx"
            });

            patientReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Patient Entry Report",
                Description = "Tracks all patients that have been manually added",
                Page = "PatientAddReport.aspx"
            });

            return patientReport;
        }

        private ReportGroup GetPharmacyReports()
        {
            var pharmacyReport = new ReportGroup("Pharmacy Reports");
            pharmacyReport.ReportLinkList = new List<ReportLink>();

            pharmacyReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Pharmacy Report",
                Description = "Tracks all transactions with all pharmacies during a specified time period",
                Page = "PharmacySummary.aspx"
            });

            pharmacyReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Pharmacy Refill Report",
                Description = "This report will show you any refill requests coming in from all pharmacies for your patients, when you select either All Providers or a specific Provider",
                Page = "PharmRefillReport.aspx"
            });
            return pharmacyReport;
        }

        private ReportGroup GetProviderReports(Constants.DeluxeFeatureStatus gCodeReportDeluxeFeatureStatus)
        {
            var providerReport = new ReportGroup("Provider Reports");
            providerReport.ReportLinkList = new List<ReportLink>();

            providerReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Provider Report (Basic)",
                Description = "Tracks provider's prescription activity",
                Page = "PrescriptionDetail.aspx"
            });

            providerReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Provider Report (Detailed)",
                Description = "Tracks provider's prescription activity, along with details the personnel involved in each script (originator, authorizing provider, and sender)",
                Page = "PrescriptionDetailCAS.aspx"
            });

            providerReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Provider Report (Prescribe on Behalf of)",
                Description = "Similar to Detailed Provider report, with focus on providers who are authorized to write prescriptions on another provider's credentials",
                Page = "PrescriptionDetailPOB.aspx"

            });

            providerReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Provider DUR Report (Prescribe on Behalf of)",
                Description = "Reports DUR warning detail displayed to providers who are authorized to write prescriptions on another provider's credentials",
                Page = "PrescriptionDetailPOB.aspx?ReportID=SuperPOBDUR"
            });

            switch (ShowDeluxeAdForReport(gCodeReportDeluxeFeatureStatus))
            {
                case ReportDisplayStatus.DisplayDeluxeAd:
                    providerReport.ReportLinkList.Add(new ReportLink
                    {
                        Name = "Provider eRx Activity Report",
                        Description = "Tracks provider's prescription activity. Captures the percentage of electronic transmitted scripts",
                        Page = string.Empty,
                        ShowDeluxeAd = true
                    });
                    break;

                case ReportDisplayStatus.DisplayReport:
                    providerReport.ReportLinkList.Add(new ReportLink
                    {
                        Name = "Provider eRx Activity Report",
                        Description = "Tracks provider's prescription activity. Captures the percentage of electronic transmitted scripts",
                        Page = "ProviderErxActivityReport.aspx",
                        ShowDeluxeAd = false
                    });
                    break;

                    //Report not added to list when HideReport
            }

            return providerReport;
        }

        private ReportGroup GetMedicationReports()
        {
            var medicationReport = new ReportGroup("Medication Reports");
            medicationReport.ReportLinkList = new List<ReportLink>();

            medicationReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Medication Report",
                Description = "Tracks how often a drug has been prescribed, along with corresponding patient information",
                Page = "PatientMedReport.aspx"
            });

            medicationReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Medication and Med Allergy Reconciliation Report",
                Description = "Tracks all the Transfer of Care Or Medication and Med Allergy Reconciliation performed for patients",
                Page = "PatientMedRecRpt.aspx"
            });

            medicationReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Medication and Med Allergy Reconciliation Detail Report",
                Description = "Tracks all the Transfer of Care Or Medication and Med Allergy Reconciliation performed with patients' information",
                Page = "PatientMedRecDetailRpt.aspx"
            });

            return medicationReport;
        }

        private ReportGroup GetSettingsReports()
        {
            var settingsReport = new ReportGroup("Settings Reports");
            settingsReport.ReportLinkList = new List<ReportLink>();

            settingsReport.ReportLinkList.Add(new ReportLink
            {
                Name = "POB to Provider Association Report",
                Description = "This report will show the providers associated with a POB",
                Page = "POBToProviderReport.aspx"
            });

            settingsReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Provider to POB Association Report",
                Description = "This report will show you the POBs a Provider is associated with",
                Page = "ProviderToPOBReport.aspx"
            });

            settingsReport.ReportLinkList.Add(new ReportLink
            {
                Name = "EPCS Rights Assignment Report",
                Description = "This report shows current assignment of EPCS roles and responsibilities critical to the EPCS management process",
                Page = "EPCSRightsAssignment.aspx"
            });

            return settingsReport;
        }

        private ReportGroup GetAuditReports()
        {
            var auditReport = new ReportGroup("Audit Reports");
            auditReport.ReportLinkList = new List<ReportLink>();

            auditReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Message Queue",
                Description = "This report will show the status of all prescriptions sent to the pharmacy. It includes the transmission status of each prescription, "
                            + "including if there was an error in transmission to the pharmacy. NOTE: This error message will also show in red on the main patient screen",
                Page = "MessageQueueTx.aspx?From=Reports.aspx"
            });

            auditReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Registry Checked Report",
                Description = "This report shows details of controlled substance prescription registry checked at the state PDMP",
                Page = "RegistryCheckedReport.aspx"
            });

            auditReport.ReportLinkList.Add(new ReportLink
            {
                Name = "Audit Log",
                Description = "This report will track all user activity in relation to patients and users",
                Page = "ViewAuditLog.aspx"
            });

            return auditReport;
        }

        public ReportDisplayStatus ShowDeluxeAdForReport(Constants.DeluxeFeatureStatus deluxeFeatureStatus)
        {
            ReportDisplayStatus displayStatus = ReportDisplayStatus.DisplayReport;
            switch (deluxeFeatureStatus)
            {
                case Constants.DeluxeFeatureStatus.On:
                    displayStatus = ReportDisplayStatus.DisplayReport;
                    break;
                case Constants.DeluxeFeatureStatus.Disabled:
                case Constants.DeluxeFeatureStatus.Off:
                    displayStatus = ReportDisplayStatus.DisplayDeluxeAd;
                    break;
                case Constants.DeluxeFeatureStatus.Hide:
                    displayStatus = ReportDisplayStatus.HideReport;
                    break;
            }

            return displayStatus;
        }

        public enum ReportDisplayStatus
        {
            DisplayReport = 1,
            DisplayDeluxeAd = 2,
            HideReport = 3
        }
    }
}