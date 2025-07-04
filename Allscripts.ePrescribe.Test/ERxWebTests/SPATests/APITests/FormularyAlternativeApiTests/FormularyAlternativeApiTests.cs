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
using static Allscripts.Impact.IgnoreReason;
using Rx = Allscripts.Impact.Rx;
using eRxWeb.ServerModel.Request;
using Medication = Allscripts.Impact.Medication;

namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.FormularyAlternativeApiTests
{
    [TestClass]
    public class FormularyAlternativeApiTests
    {
        [TestMethod]
        public void should_get_empty_formulary_alternatives_from_session_if_the_RxList_is_null()
        {
            //Arrange
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub<IStateContainer>(p => p.Cast("RxList", default(ArrayList))).Return(new ArrayList());

            //Act
            FormularyAlternativesApiController falt = new FormularyAlternativesApiController(sessionMock, new Medication());
            var resp = falt.GetFormularyAlternativesFromSession();
            var res = resp.Payload as FormularyAlternative;

            //Asssert
            Assert.AreEqual(res.Copay, null);
            Assert.AreEqual(res.DrugName, null);
        }

        [TestMethod]
        public void should_get_valid_formulary_alternatives_from_session_if_the_RxList_is_not_null()
        {
            //Arrange
            Rx rx = new Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Synthroid";
            rx.Strength = "10Mg";
            rx.StrengthUOM = "ml";
            rx.DosageFormDescription = "some description";
            rx.IsOTC = "Y";
            rx.FormularyStatus = "5";

            ArrayList rxList = new ArrayList();
            rxList.Add(rx);
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub<IStateContainer>(p => p.Cast("RxList", default(ArrayList))).Return(rxList);


            //Act
            FormularyAlternativesApiController falt = new FormularyAlternativesApiController(sessionMock, new Medication());
            var resp = falt.GetFormularyAlternativesFromSession();
            var res = resp.Payload as FormularyAlternative;

            //Assert
            Assert.AreEqual(res.ImageUrl, "images/fs_otc.gif");
            Assert.AreEqual(res.DrugName, "Synthroid 10Mg ml some description");
        }

        [TestMethod]
        public void should_get_formulary_alternative_for_the_provided_med()
        {
            //Arrange
            MedicationSelectedRequest Med = new MedicationSelectedRequest();
            Med.taskScriptMessageId = "123456";
            Med.DDI = "78781";
            Med.FormularyStatus = 5;
            Med.MedName = "Synthroid";

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());
            ds.Tables.Add(new DataTable());

            ds.Tables[3].Columns.Add("Name", typeof(string));
            ds.Tables[3].Columns.Add("Copay", typeof(string));
            ds.Tables[3].Columns.Add("IsOTC", typeof(string));
            ds.Tables[3].Columns.Add("IsGeneric", typeof(string));
            ds.Tables[3].Columns.Add("FormularyStatus", typeof(string));
            ds.Tables[3].Columns.Add("LevelOfPreferedness", typeof(string));

            DataRow dr = ds.Tables[3].NewRow();
            dr["Name"] = "Synthroid" ;
            dr["Copay"] = "Covered" ;
            dr["IsOTC"] = "Y" ;
            dr["IsGeneric"] = "Y" ;
            dr["FormularyStatus"] = "5";
            dr["LevelOfPreferedness"] = "3";

            ds.Tables[3].Rows.Add(dr);

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var medication = MockRepository.GenerateStub<IMedication>();
            sessionMock.Stub(p => p.GetStringOrEmpty("FORMULARYID")).Return("True");
            medication.Stub(p => p.GetFormularyAlternatives(string.Empty,string.Empty,string.Empty,string.Empty,0,string.Empty,
                string.Empty,1,string.Empty,ConnectionStringPointer.AUDIT_ERXDB_SERVER_1)).Return(ds);

            //Act
            FormularyAlternativesApiController falt = new FormularyAlternativesApiController(sessionMock, medication);
            var resp = falt.GetFormularyAlternatives(Med);
            var res = resp.Payload as List<FormularyAlternative>;

            //Assert
            Assert.AreEqual(res.Count, 0);
        }


    }
}
