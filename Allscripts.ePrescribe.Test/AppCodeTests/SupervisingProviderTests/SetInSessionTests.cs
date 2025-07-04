using System;
using System.Collections.Generic;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.SupervisingProviderTests
{
    [TestClass]
    public class SetInSessionTests
    {
        [TestMethod]
        public void should_return_message_if_no_npi()
        {
            //arrange
            var dt = new DataTable("User");
            dt.Columns.Add("UPIN");

            var dr = dt.NewRow();
            dr["UPIN"] = "";

            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            var user = new RxUser(ds);

            var session = MockRepository.GenerateMock<IStateContainer>();

            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();

            //act
            var result = SupervisingProvider.SetInSession(user, session, deaUtil);

            //assert
            Assert.AreEqual("Please select a provider with a valid NPI.", result);
        }

        [TestMethod]
        public void should_set_dea_schedules()
        {
            //arrange
            var dt = new DataTable("User");
            dt.Columns.Add("UPIN");

            var dr = dt.NewRow();
            dr["UPIN"] = "8938929238";

            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            var user = new RxUser(ds);

            var session = MockRepository.GenerateMock<IStateContainer>();

            var deaNumbers = new List<string> {"1", "4"};
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();
            deaUtil.Stub(_ => _.ExtractAllowed(null)).IgnoreArguments().Return(deaNumbers);


            session.Expect(_ => _[Constants.SessionVariables.SupProviderDeaSchedules] = deaNumbers);

            //act
            SupervisingProvider.SetInSession(user, session, deaUtil);

            //assert
            session.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_set_min_dea()
        {
            //arrange
            var dt = new DataTable("User");
            dt.Columns.Add("UPIN");
            dt.Columns.Add("UserID");

            var dr = dt.NewRow();
            dr["UPIN"] = "8938929238";
            dr["UserID"] = "939202";

            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            var user = new RxUser(ds);

            var session = MockRepository.GenerateMock<IStateContainer>();

            var deaNumbers = new List<string> { "4", "1" };
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();
            deaUtil.Stub(_ => _.ExtractAllowed(null)).IgnoreArguments().Return(deaNumbers);
            deaUtil.Stub(_ => _.RetrieveMin(null)).IgnoreArguments().Return("1");


            session.Expect(_ => _[Constants.SessionVariables.SupProviderMinDea] = "1");

            //act
            SupervisingProvider.SetInSession(user, session, deaUtil);

            //assert
            session.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_set_sup_provider_id()
        {
            //arrange
            var dt = new DataTable("User");
            dt.Columns.Add("UPIN");
            dt.Columns.Add("UserID");

            var dr = dt.NewRow();
            dr["UPIN"] = "8938929238";
            dr["UserID"] = "939202";

            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            var user = new RxUser(ds);

            var session = MockRepository.GenerateMock<IStateContainer>();

            var deaNumbers = new List<string> { "4", "1" };
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();
            deaUtil.Stub(_ => _.ExtractAllowed(null)).IgnoreArguments().Return(deaNumbers);


            session.Expect(_ => _[Constants.SessionVariables.SupervisingProviderId] = "939202");

            //act
            SupervisingProvider.SetInSession(user, session, deaUtil);

            //assert
            session.VerifyAllExpectations();
        }
    }
}
