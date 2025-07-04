using System;
using System.Collections.Generic;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.PrescriberOnRecordTests
{
    [TestClass]
    public class SetInSessionTests
    {
        private RxUser _user;

        [TestInitialize]
        public void Init()
        {
            var dt = new DataTable("User");
            dt.Columns.Add("UPIN");
            dt.Columns.Add("UserID");
            dt.Columns.Add("UserRole");
            dt.Columns.Add("LicenseID"); 
            dt.Columns.Add("FirstName");
            dt.Columns.Add("LastName");
            dt.Columns.Add("DEANumber");
            dt.Columns.Add("Email");

            var dr = dt.NewRow();
            dr["UPIN"] = "8329239";
            dr["UserID"] = "939202";
            dr["UserRole"] = Convert.ToInt32(RxUser.UserCategory.PROVIDER);
            dr["LicenseID"] = Guid.NewGuid().ToString();
            dr["DEANumber"] = "lksdf";
            dr["LastName"] = "lksdf";
            dr["FirstName"] = "lksdf";
            dr["Email"] = "lksdf@y.com";

            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            _user = new RxUser(ds);
        }

        [TestMethod]
        public void should_return_message_if_no_npi()
        {
            //arrange
            var dt = new DataTable("User");
            dt.Columns.Add("UPIN");
            dt.Columns.Add("UserID");

            var dr = dt.NewRow();
            dr["UPIN"] = "";
            dr["UserID"] = "939202";

            dt.Rows.Add(dr);
            var ds = new DataSet();
            ds.Tables.Add(dt);

            var user = new RxUser(ds);

            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();

            //act
            var result = PrescriberOnRecord.SetInSession(user, session, rxUser, spi, deaUtil);


            //assert
            Assert.AreEqual("Please select a provider with a valid NPI.", result);
        }

        [TestMethod]
        public void should_remove_supervising_provider_id()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _.Remove(Constants.SessionVariables.SupervisingProviderId));
        }

        [TestMethod]
        public void should_set_dea_allowed()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            
            var deaNumbers = new List<string> { "1", "4" };
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();
            deaUtil.Stub(_ => _.ExtractAllowed(null)).IgnoreArguments().Return(deaNumbers);


            session.Expect(_ => _[Constants.SessionVariables.DelegateDeaAllowed] = deaNumbers);

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_set_min_dea()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();

            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();
            deaUtil.Stub(_ => _.RetrieveMin(null)).IgnoreArguments().Return("1");


            session.Expect(_ => _[Constants.SessionVariables.DelegateMinDea] = "1");

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.VerifyAllExpectations();
        }

        [TestMethod]
        public void should_call_update_pob_usage()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();

            

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            rxUser.AssertWasCalled(_ => _.UpdatePobProviderUsage(Arg<Guid>.Is.Anything, 
                                                                Arg<int>.Is.Anything,
                                                                Arg<Guid>.Is.Anything, 
                                                                Arg<Guid>.Is.Anything, 
                                                                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_get_spi_for_current_user_if_pa_sup()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();

            session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.IsPaSupervised)).Return(true);

            var currentUserId = Guid.NewGuid();
            session.Stub(_ => _.GetGuidOr0x0(Constants.SessionVariables.UserId)).Return(currentUserId);

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            spi.AssertWasCalled(_ => _.RetrieveSpiForSession(Arg<Guid>.Is.Equal(currentUserId), Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.SHARED_DB)));
        }

        [TestMethod]
        public void should_get_spi_for_provider_if_not_pa_sup()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();

            session.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.IsPaSupervised)).Return(false);

            var currentUserId = Guid.NewGuid();
            session.Stub(_ => _.GetGuidOr0x0(Constants.SessionVariables.UserId)).Return(currentUserId);

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            spi.AssertWasCalled(_ => _.RetrieveSpiForSession(Arg<Guid>.Is.NotEqual(currentUserId), Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.SHARED_DB)));
        }

        [TestMethod]
        public void should_set_spi()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.SPI)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_delegate_npi()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();
            

            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.DelegateProviderNPI)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_delegate_prov_id()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.DelegateProviderId)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_ShouldPrintOfferAutomatically()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.ShouldPrintOfferAutomatically)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_ShouldShowTherapeuticAlternativeAutomatically()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.ShouldShowTherapeuticAlternativeAutomatically)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_ShouldShowPpt()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.ShouldShowPpt)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_ShouldShowRtbi()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.ShouldShowRtbi)] = Arg<string>.Is.Anything);
        }

        [TestMethod]
        public void should_set_PptProviderInfo()
        {
            //arrange
            var session = MockRepository.GenerateMock<IStateContainer>();
            var rxUser = MockRepository.GenerateMock<IRxUser>();
            var spi = MockRepository.GenerateMock<ISPI>();
            var deaUtil = MockRepository.GenerateMock<IDeaScheduleUtility>();


            //act
            PrescriberOnRecord.SetInSession(_user, session, rxUser, spi, deaUtil);


            //assert
            session.AssertWasCalled(_ => _[Arg<string>.Is.Equal(Constants.SessionVariables.CommonCompProviderInfo)] = Arg<string>.Is.Anything);
        }
    }
}
