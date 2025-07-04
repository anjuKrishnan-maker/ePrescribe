using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static eRxWeb.AppCode.Reports.ReportMenuUtil;

namespace Allscripts.ePrescribe.Test.AppCodeTests.ReportTests.ReportMenuUtilTests
{
    [TestClass]
    public class GenerateReportMenuTests
    {
        [TestMethod]
        public void Should_have_six_report_groups()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedGroupCount = 6;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);
            
            //Assert
            Assert.AreEqual(result.ReportGroupList.Count, expectedGroupCount);
        }

        [TestMethod]
        public void Should_have_two_patient_reports()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedPatientReports = 2;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var patientReports = result.ReportGroupList.Find(a => a.Name == "Patient Reports");

            //Assert
            Assert.AreEqual(patientReports.ReportLinkList.Count, expectedPatientReports);
            Assert.AreEqual(patientReports.ReportLinkList[0].Name, "Patient Medication Report");
            Assert.AreEqual(patientReports.ReportLinkList[0].Page, "PatientMedHistory.aspx");
            Assert.AreEqual(patientReports.ReportLinkList[1].Name, "Patient Entry Report");
            Assert.AreEqual(patientReports.ReportLinkList[1].Page, "PatientAddReport.aspx");
        }

        [TestMethod]
        public void Should_have_two_pharmacy_reports()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedPharmacyReports = 2;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var pharmacyReports = result.ReportGroupList.Find(a => a.Name == "Pharmacy Reports");

            //Assert
            Assert.AreEqual(pharmacyReports.ReportLinkList.Count, expectedPharmacyReports);
            Assert.AreEqual(pharmacyReports.ReportLinkList[0].Name, "Pharmacy Report");
            Assert.AreEqual(pharmacyReports.ReportLinkList[0].Page, "PharmacySummary.aspx");
            Assert.AreEqual(pharmacyReports.ReportLinkList[1].Name, "Pharmacy Refill Report");
            Assert.AreEqual(pharmacyReports.ReportLinkList[1].Page, "PharmRefillReport.aspx");
        }

        [TestMethod]
        public void Should_have_five_provider_reports()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedProviderReports = 5;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var providerReports = result.ReportGroupList.Find(a => a.Name == "Provider Reports");

            //Assert
            Assert.AreEqual(providerReports.ReportLinkList.Count, expectedProviderReports);
            Assert.AreEqual(providerReports.ReportLinkList[0].Name, "Provider Report (Basic)");
            Assert.AreEqual(providerReports.ReportLinkList[0].Page, "PrescriptionDetail.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[1].Name, "Provider Report (Detailed)");
            Assert.AreEqual(providerReports.ReportLinkList[1].Page, "PrescriptionDetailCAS.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[2].Name, "Provider Report (Prescribe on Behalf of)");
            Assert.AreEqual(providerReports.ReportLinkList[2].Page, "PrescriptionDetailPOB.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[3].Name, "Provider DUR Report (Prescribe on Behalf of)");
            Assert.AreEqual(providerReports.ReportLinkList[3].Page, "PrescriptionDetailPOB.aspx?ReportID=SuperPOBDUR");
            Assert.AreEqual(providerReports.ReportLinkList[4].Name, "Provider eRx Activity Report");
            Assert.AreEqual(providerReports.ReportLinkList[4].Page, "ProviderErxActivityReport.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[4].ShowDeluxeAd, false);
        }

        [TestMethod]
        public void Should_have_four_provider_reports_when_gcdstatus_hide()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.Hide;
            int expectedProviderReports = 4;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var providerReports = result.ReportGroupList.Find(a => a.Name == "Provider Reports");

            //Assert
            Assert.AreEqual(providerReports.ReportLinkList.Count, expectedProviderReports);
            Assert.AreEqual(providerReports.ReportLinkList[0].Name, "Provider Report (Basic)");
            Assert.AreEqual(providerReports.ReportLinkList[0].Page, "PrescriptionDetail.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[1].Name, "Provider Report (Detailed)");
            Assert.AreEqual(providerReports.ReportLinkList[1].Page, "PrescriptionDetailCAS.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[2].Name, "Provider Report (Prescribe on Behalf of)");
            Assert.AreEqual(providerReports.ReportLinkList[2].Page, "PrescriptionDetailPOB.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[3].Name, "Provider DUR Report (Prescribe on Behalf of)");
            Assert.AreEqual(providerReports.ReportLinkList[3].Page, "PrescriptionDetailPOB.aspx?ReportID=SuperPOBDUR");
        }

        [TestMethod]
        public void Should_have_five_provider_reports_when_gcd_status_disabled()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.Disabled;
            int expectedProviderReports = 5;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var providerReports = result.ReportGroupList.Find(a => a.Name == "Provider Reports");

            //Assert
            Assert.AreEqual(providerReports.ReportLinkList.Count, expectedProviderReports);
            Assert.AreEqual(providerReports.ReportLinkList[0].Name, "Provider Report (Basic)");
            Assert.AreEqual(providerReports.ReportLinkList[0].Page, "PrescriptionDetail.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[1].Name, "Provider Report (Detailed)");
            Assert.AreEqual(providerReports.ReportLinkList[1].Page, "PrescriptionDetailCAS.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[2].Name, "Provider Report (Prescribe on Behalf of)");
            Assert.AreEqual(providerReports.ReportLinkList[2].Page, "PrescriptionDetailPOB.aspx");
            Assert.AreEqual(providerReports.ReportLinkList[3].Name, "Provider DUR Report (Prescribe on Behalf of)");
            Assert.AreEqual(providerReports.ReportLinkList[3].Page, "PrescriptionDetailPOB.aspx?ReportID=SuperPOBDUR");
            Assert.AreEqual(providerReports.ReportLinkList[4].Name, "Provider eRx Activity Report");
            Assert.AreEqual(providerReports.ReportLinkList[4].Page, string.Empty);
            Assert.AreEqual(providerReports.ReportLinkList[4].ShowDeluxeAd, true);
        }

        [TestMethod]
        public void Should_have_three_provider_reports()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedMedicationReports = 3;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var medicationReports = result.ReportGroupList.Find(a => a.Name == "Medication Reports");

            //Assert
            Assert.AreEqual(medicationReports.ReportLinkList.Count, expectedMedicationReports);
            Assert.AreEqual(medicationReports.ReportLinkList[0].Name, "Medication Report");
            Assert.AreEqual(medicationReports.ReportLinkList[0].Page, "PatientMedReport.aspx");
            Assert.AreEqual(medicationReports.ReportLinkList[1].Name, "Medication and Med Allergy Reconciliation Report");
            Assert.AreEqual(medicationReports.ReportLinkList[1].Page, "PatientMedRecRpt.aspx");
            Assert.AreEqual(medicationReports.ReportLinkList[2].Name, "Medication and Med Allergy Reconciliation Detail Report");
            Assert.AreEqual(medicationReports.ReportLinkList[2].Page, "PatientMedRecDetailRpt.aspx");
        }

        [TestMethod]
        public void Should_have_three_settings_reports()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedSettingsReports = 3;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var settingsReports = result.ReportGroupList.Find(a => a.Name == "Settings Reports");

            //Assert
            Assert.AreEqual(settingsReports.ReportLinkList.Count, expectedSettingsReports);
            Assert.AreEqual(settingsReports.ReportLinkList[0].Name, "POB to Provider Association Report");
            Assert.AreEqual(settingsReports.ReportLinkList[0].Page, "POBToProviderReport.aspx");
            Assert.AreEqual(settingsReports.ReportLinkList[1].Name, "Provider to POB Association Report");
            Assert.AreEqual(settingsReports.ReportLinkList[1].Page, "ProviderToPOBReport.aspx");
            Assert.AreEqual(settingsReports.ReportLinkList[2].Name, "EPCS Rights Assignment Report");
            Assert.AreEqual(settingsReports.ReportLinkList[2].Page, "EPCSRightsAssignment.aspx");
        }

        [TestMethod]
        public void Should_have_three_audit_reports()
        {
            //Arrange 
            Constants.DeluxeFeatureStatus gcdStatus = Constants.DeluxeFeatureStatus.On;
            int expectedAuditReports = 3;

            //Act
            var result = new ReportMenuUtil().GenerateReportMenu(gcdStatus);

            var auditReports = result.ReportGroupList.Find(a => a.Name == "Audit Reports");

            //Assert
            Assert.AreEqual(auditReports.ReportLinkList.Count, expectedAuditReports);
            Assert.AreEqual(auditReports.ReportLinkList[0].Name, "Message Queue");
            Assert.AreEqual(auditReports.ReportLinkList[0].Page, "MessageQueueTx.aspx?From=Reports.aspx");
            Assert.AreEqual(auditReports.ReportLinkList[1].Name, "Registry Checked Report");
            Assert.AreEqual(auditReports.ReportLinkList[1].Page, "RegistryCheckedReport.aspx");
            Assert.AreEqual(auditReports.ReportLinkList[2].Name, "Audit Log");
            Assert.AreEqual(auditReports.ReportLinkList[2].Page, "ViewAuditLog.aspx");
        }
    }
}
