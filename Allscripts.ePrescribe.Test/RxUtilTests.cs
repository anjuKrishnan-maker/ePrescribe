using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb;
using Allscripts.ePrescribe.Data.Model;
namespace Allscripts.ePrescribe.Test
{
    [TestClass]
    public class RxUtilTests
    {
        [TestMethod]
        public void should_return_PBM_reported_when_drug_history_type_is_PY()
        {
            var newModel = new ReviewHistoryDataModel()
            {
                RxID = new Guid(),
                Pharmacy = Convert.ToString(false),
                Comments = "PBM Reported Rx History",
                RxSource = "PBMX",
                Type = "N",
                TransmissionMethod = "H",
                ControlledSubstanceCode = "4",
                Updated = Convert.ToDateTime("7/25/2019"),
                TransmissionStatus = "0",
                DrugHistoryType = Constants.DrugHistoryType.PY
            };         
            int siteId = 1;
            Guid licenseId = new Guid();            
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_SERVER_1;
            string expectedRXDescription = "PBM Reported Rx History";
            var actualRXDescription = RxUtils.GetNewPrescriptionStatusDescription(newModel, siteId, licenseId, dbID);
            Assert.AreEqual(expectedRXDescription, actualRXDescription);
        }

        [TestMethod]
        public void should_return_PBM_reported_when_drug_history_type_is_None()
        {
            var newModel = new ReviewHistoryDataModel()
            {
                RxID = new Guid(),
                Pharmacy = Convert.ToString(false),
                Comments = "PBM Reported Rx History",
                RxSource = "PBMX",
                Type = "N",
                TransmissionMethod = "H",
                ControlledSubstanceCode = "4",
                Updated = Convert.ToDateTime("7/25/2019"),
                TransmissionStatus = "0",
                DrugHistoryType = Constants.DrugHistoryType.None
            };
            int siteId = 1;
            Guid licenseId = new Guid();
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_SERVER_1;
            string expectedRXDescription = "PBM Reported Rx History";
            var actualRXDescription = RxUtils.GetNewPrescriptionStatusDescription(newModel, siteId, licenseId, dbID);
            Assert.AreEqual(expectedRXDescription, actualRXDescription);
        }


        [TestMethod]
        public void should_return_Pharmacy_reported_when_drug_history_type_is_P2()
        {
            var newModel = new ReviewHistoryDataModel()
            {
                RxID = new Guid(),
                Pharmacy = Convert.ToString(false),
                Comments = "Pharmacy Reported Rx History",
                RxSource = "PBMX",
                Type = "N",
                TransmissionMethod = "H",
                ControlledSubstanceCode = "4",
                Updated = Convert.ToDateTime("7/25/2019"),
                TransmissionStatus = "0",
                DrugHistoryType = Constants.DrugHistoryType.P2
            };
            int siteId = 1;
            Guid licenseId = new Guid();
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_SERVER_1;
            string expectedRXDescription = "Pharmacy Reported Rx History";
            var actualRXDescription = RxUtils.GetNewPrescriptionStatusDescription(newModel, siteId, licenseId, dbID);
            Assert.AreEqual(expectedRXDescription, actualRXDescription);
        }
    }
}
