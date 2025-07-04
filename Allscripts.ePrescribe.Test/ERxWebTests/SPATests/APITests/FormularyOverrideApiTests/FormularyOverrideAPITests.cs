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

namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.FormularyOverrideApiTests
{
    [TestClass]
    public class FormularyOverrideAPITests
    {
        [TestMethod]
        public void should_get_the_list_of_override_reasons()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ReasonDescription");
            DataRow dr = dt.NewRow();
            dr["ReasonDescription"] = "Test Reason";
            dt.Rows.Add(dr);
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var reasonMock = MockRepository.GenerateStub<IIgnoreReason>();
            sessionMock.Stub<IStateContainer>(p => p.Cast("OverrideRxList", default(ArrayList))).Return(new ArrayList());
            reasonMock.Stub<IIgnoreReason>(p => p.GetIgnoreReasons(IgnoreCategory.FORMULARY_ALTS, ConnectionStringPointer.SHARED_DB)).Return(dt);

            FormularyOverrideApiController formOverride = new FormularyOverrideApiController(sessionMock, reasonMock);
            var resp = formOverride.GetFormularyOverideIgnoreReasons();
            var res = resp.Payload as FormularyOverideModel;
            var result = res.IgnoreReasons.IgnoreReasons[0];


            Assert.AreEqual(result , "Test Reason");
        }

        [TestMethod]
        public void should_get_list_of_medications_to_be_overridden()
        {
            Impact.Rx rx = new Impact.Rx();
            rx.RxID = Guid.NewGuid().ToString();
            rx.MedicationName = "Lipitor";
            rx.Strength = "650";
            rx.StrengthUOM = "MG";
            rx.DosageFormCode = "SUPP";
            rx.RouteOfAdminCode = "OR";

            ArrayList arr = new ArrayList();
            arr.Add(rx);

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            var reasonMock = MockRepository.GenerateStub<IIgnoreReason>();
            sessionMock.Stub<IStateContainer>(p => p.Cast("OverrideRxList", default(ArrayList))).Return(arr);
            reasonMock.Stub<IIgnoreReason>(p => p.GetIgnoreReasons(IgnoreCategory.FORMULARY_ALTS, ConnectionStringPointer.SHARED_DB)).Return(new DataTable());

            FormularyOverrideApiController formOverride = new FormularyOverrideApiController(sessionMock, reasonMock);
            var resp = formOverride.GetFormularyOverideIgnoreReasons();

            var res = resp.Payload as FormularyOverideModel;
            int medCount = res.Medication.Length;

            Assert.AreEqual(medCount, 1);

        }
        
    }
}
