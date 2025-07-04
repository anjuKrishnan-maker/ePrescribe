using System;
using System.Collections.Generic;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;
using Allscripts.Impact.Interfaces;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using eRxWeb.ServerModel;
using eRxWeb.Controllers;
using eRxWeb.State;
using System.Collections;
using eRxWeb.Controller;
using eRxWeb.ServerModel.Request;
using static Allscripts.Impact.IgnoreReason;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Common;
namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.PatientMedHistoryAPITests
{
    [TestClass]
    public class PatientMedHistoryAPITests
    {
        [TestMethod]
        public void should_get_patient_med_history_data()
        {
            //Arrange
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());

            ds.Tables[0].Columns.Add("RxDate", typeof(string));
            ds.Tables[0].Columns.Add("Dx", typeof(string));
            ds.Tables[0].Columns.Add("RxSourceDescription", typeof(string));
            ds.Tables[0].Columns.Add("Prescription", typeof(string));
            ds.Tables[0].Columns.Add("RxID", typeof(string));
            ds.Tables[0].Columns.Add("status", typeof(string));
            ds.Tables[0].Columns.Add("Pharmacy", typeof(string));
            ds.Tables[0].Columns.Add("comments", typeof(string));
            ds.Tables[0].Columns.Add("Updated", typeof(string));
            ds.Tables[0].Columns.Add("RxSource", typeof(string));
            ds.Tables[0].Columns.Add("Type", typeof(string));
            ds.Tables[0].Columns.Add("TransmissionMethod", typeof(string));
            ds.Tables[0].Columns.Add("ControlledSubstanceCode", typeof(string));
            ds.Tables[0].Columns.Add("TransmissionStatus", typeof(string));
            ds.Tables[0].Columns.Add("DrugHistoryType", typeof(string));

            DataRow dr = ds.Tables[0].NewRow();
            dr["RxDate"] = "01/01/2012";
            dr["Dx"] = "Some DX";
            dr["RxSourceDescription"] = "Some Description";
            dr["Prescription"] = "Synthroid Med";
            dr["RxID"] = Guid.NewGuid().ToString();
            dr["status"] = "2";
            dr["Pharmacy"] = "Test Pharmacy";
            dr["comments"] = "Test Comments";
            dr["Updated"] = "01/01/2012";
            dr["RxSource"] = "From Test";
            dr["Type"] = "2";
            dr["TransmissionMethod"] = "Online";
            dr["ControlledSubstanceCode"] = "2";
            dr["TransmissionStatus"] = "Success";
            dr["DrugHistoryType"] = Constants.DrugHistoryType.PY;
            ds.Tables[0].Rows.Add(dr);

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var prescription = MockRepository.GenerateStub<IPrescription>();

           var patientID=  sessionMock.Stub(p => p.GetStringOrEmpty("PatientID")).Return(String.Empty);
            var licenceID = sessionMock.Stub(p => p.GetStringOrEmpty("LicenseID")).Return(String.Empty);

            prescription.Stub(p => p.ChGetPatReviewHistory(sessionMock.GetStringOrEmpty("PatientID"), sessionMock.GetStringOrEmpty("LicenseID"), ApiHelper.GetDBID(sessionMock))).Return(ds);

            //Act
            PatientMedHistoryAPIController pm = new PatientMedHistoryAPIController(prescription,sessionMock);
            ApiResponse resp = pm.GetPatientMedicationHistoryData();

            var result = resp.Payload as List<PatientMedHistoryModel>;
            //Assert
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void should_get_PBM_medication_history_in_the_list()
        {
            //Arrange
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());

            ds.Tables[0].Columns.Add("RxDate", typeof(string));
            ds.Tables[0].Columns.Add("Dx", typeof(string));
            ds.Tables[0].Columns.Add("RxSourceDescription", typeof(string));
            ds.Tables[0].Columns.Add("Prescription", typeof(string));
            ds.Tables[0].Columns.Add("RxID", typeof(string));
            ds.Tables[0].Columns.Add("status", typeof(string));
            ds.Tables[0].Columns.Add("Pharmacy", typeof(string));
            ds.Tables[0].Columns.Add("comments", typeof(string));
            ds.Tables[0].Columns.Add("Updated", typeof(string));
            ds.Tables[0].Columns.Add("RxSource", typeof(string));
            ds.Tables[0].Columns.Add("Type", typeof(string));
            ds.Tables[0].Columns.Add("TransmissionMethod", typeof(string));
            ds.Tables[0].Columns.Add("ControlledSubstanceCode", typeof(string));
            ds.Tables[0].Columns.Add("TransmissionStatus", typeof(string));
            ds.Tables[0].Columns.Add("DrugHistoryType", typeof(string));

            DataRow dr = ds.Tables[0].NewRow();
            dr["RxDate"] = "01/01/2012";
            dr["Dx"] = "Some DX";
            dr["RxSourceDescription"] = "Some Description";
            dr["Prescription"] = "Synthroid Med";
            dr["RxID"] = Guid.NewGuid().ToString();
            dr["status"] = "0";
            dr["Pharmacy"] = "Test Pharmacy";
            dr["comments"] = "Test Comments";
            dr["Updated"] = "01/01/2012";
            dr["RxSource"] = "PBMX";
            dr["Type"] = "N";
            dr["TransmissionMethod"] = "H";
            dr["ControlledSubstanceCode"] = "4";
            dr["TransmissionStatus"] = "0";
            dr["DrugHistoryType"] = Constants.DrugHistoryType.PY;
            ds.Tables[0].Rows.Add(dr);

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var prescription = MockRepository.GenerateStub<IPrescription>();

            var patientID = sessionMock.Stub(p => p.GetStringOrEmpty("PatientID")).Return(String.Empty);
            var licenceID = sessionMock.Stub(p => p.GetStringOrEmpty("LicenseID")).Return(String.Empty);

            prescription.Stub(p => p.ChGetPatReviewHistory(sessionMock.GetStringOrEmpty("PatientID"), sessionMock.GetStringOrEmpty("LicenseID"), ApiHelper.GetDBID(sessionMock))).Return(ds);

            //Act
           
            List<PatientMedHistoryModel> patientHistoryList = new List<PatientMedHistoryModel>();
            patientHistoryList = PatientMedHistoryAPIController.GetPatientHistoryMedication(sessionMock, prescription);
            //Assert
            Assert.AreEqual(patientHistoryList[0].status, "PBM Reported Rx History");
        }

        [TestMethod]
        public void should_get_Pharmacy_medication_history_in_the_list()
        {
            //Arrange
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());

            ds.Tables[0].Columns.Add("RxDate", typeof(string));
            ds.Tables[0].Columns.Add("Dx", typeof(string));
            ds.Tables[0].Columns.Add("RxSourceDescription", typeof(string));
            ds.Tables[0].Columns.Add("Prescription", typeof(string));
            ds.Tables[0].Columns.Add("RxID", typeof(string));
            ds.Tables[0].Columns.Add("status", typeof(string));
            ds.Tables[0].Columns.Add("Pharmacy", typeof(string));
            ds.Tables[0].Columns.Add("comments", typeof(string));
            ds.Tables[0].Columns.Add("Updated", typeof(string));
            ds.Tables[0].Columns.Add("RxSource", typeof(string));
            ds.Tables[0].Columns.Add("Type", typeof(string));
            ds.Tables[0].Columns.Add("TransmissionMethod", typeof(string));
            ds.Tables[0].Columns.Add("ControlledSubstanceCode", typeof(string));
            ds.Tables[0].Columns.Add("TransmissionStatus", typeof(string));
            ds.Tables[0].Columns.Add("DrugHistoryType", typeof(string));

            DataRow dr = ds.Tables[0].NewRow();
            dr["RxDate"] = "01/01/2012";
            dr["Dx"] = "Some DX";
            dr["RxSourceDescription"] = "Some Description";
            dr["Prescription"] = "Synthroid Med";
            dr["RxID"] = Guid.NewGuid().ToString();
            dr["status"] = "0";
            dr["Pharmacy"] = "Test Pharmacy";
            dr["comments"] = "Test Comments";
            dr["Updated"] = "01/01/2012";
            dr["RxSource"] = "PBMX";
            dr["Type"] = "N";
            dr["TransmissionMethod"] = "H";
            dr["ControlledSubstanceCode"] = "4";
            dr["TransmissionStatus"] = "0";
            dr["DrugHistoryType"] = Constants.DrugHistoryType.P2;
            ds.Tables[0].Rows.Add(dr);

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var prescription = MockRepository.GenerateStub<IPrescription>();

            var patientID = sessionMock.Stub(p => p.GetStringOrEmpty("PatientID")).Return(String.Empty);
            var licenceID = sessionMock.Stub(p => p.GetStringOrEmpty("LicenseID")).Return(String.Empty);

            prescription.Stub(p => p.ChGetPatReviewHistory(sessionMock.GetStringOrEmpty("PatientID"), sessionMock.GetStringOrEmpty("LicenseID"), ApiHelper.GetDBID(sessionMock))).Return(ds);

            //Act

            List<PatientMedHistoryModel> patientHistoryList = new List<PatientMedHistoryModel>();
            patientHistoryList = PatientMedHistoryAPIController.GetPatientHistoryMedication(sessionMock, prescription);
            //Assert
            Assert.AreEqual(patientHistoryList[0].status, "Pharmacy Reported Rx History");
        }
    }
}
