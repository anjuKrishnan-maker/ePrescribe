using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controller;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.Controls
{
   public class IdmeTeaserAdTests
    {
        [TestClass]
        public class IdmeVisibilityTest
        {
            private IStateContainer _session;
            private IEPSBroker _epcsBroker;

            [TestInitialize]
            public void init()
            {
                _session = MockRepository.GenerateStub<IStateContainer>();
                _epcsBroker = MockRepository.GenerateStub<IEPSBroker>();
            }

            [TestMethod]
            public void should_return_true_on_showEPCPrompt_true_EnrolledToZentry_CSPAppInstanceAvailable_NotEnrolledWithIdme()
            {
                //arrange
                _session.Stub(_ => _.Cast(Constants.SessionVariables.DisableIdmeTeaserAd,""));

                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPCSPrompt", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                _session.Stub(_ => _.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);
                
                _epcsBroker.Stub(_ => _.GetUserShieldCspStatusInfo(Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB, "", "", "", "")).IgnoreArguments().Return(new GetUserShieldCspStatusInfoResponse { IsTenantShieldCSPAppInstanceAvailable = true, IsUserEnrolledToZentryCsp =true,IsUserEnrolledWithIdMe=false });

                //act
                var result = SelectPatientApiController.GetIdmeTeaserAdVisibility(_session,_epcsBroker);

                //assert
                Assert.AreEqual(true,result);
            }

            [TestMethod]
            public void should_return_false_on_showEPCPrompt_false()
            {
                //arrange
                _session.Stub(_ => _.Cast(Constants.SessionVariables.DisableIdmeTeaserAd, ""));

                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPCSPrompt", "N");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                _session.Stub(_ => _.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);

                _epcsBroker.Stub(_ => _.GetUserShieldCspStatusInfo(Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB, "", "", "", "")).IgnoreArguments().Return(new GetUserShieldCspStatusInfoResponse { IsTenantShieldCSPAppInstanceAvailable = true, IsUserEnrolledToZentryCsp = true, IsUserEnrolledWithIdMe = false });

                //act
                var result = SelectPatientApiController.GetIdmeTeaserAdVisibility(_session, _epcsBroker);

                //assert
                Assert.AreEqual(false, result);
            }

            [TestMethod]
            public void should_return_false_on_IsUserEnrolledToZentryCsp_false()
            {
                //arrange
                _session.Stub(_ => _.Cast(Constants.SessionVariables.DisableIdmeTeaserAd, ""));

                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPCSPrompt", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                _session.Stub(_ => _.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);

                _epcsBroker.Stub(_ => _.GetUserShieldCspStatusInfo(Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB, "", "", "", "")).IgnoreArguments().Return(new GetUserShieldCspStatusInfoResponse { IsTenantShieldCSPAppInstanceAvailable = true, IsUserEnrolledToZentryCsp = false, IsUserEnrolledWithIdMe = false });

                //act
                var result = SelectPatientApiController.GetIdmeTeaserAdVisibility(_session, _epcsBroker);

                //assert
                Assert.AreEqual(false, result);
            }

            [TestMethod]
            public void should_return_false_on_IsTenantShieldCSPAppInstanceAvailable_false()
            {
                //arrange
                _session.Stub(_ => _.Cast(Constants.SessionVariables.DisableIdmeTeaserAd, ""));

                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPCSPrompt", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                _session.Stub(_ => _.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);

                _epcsBroker.Stub(_ => _.GetUserShieldCspStatusInfo(Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB, "", "", "", "")).IgnoreArguments().Return(new GetUserShieldCspStatusInfoResponse { IsTenantShieldCSPAppInstanceAvailable = false, IsUserEnrolledToZentryCsp = true, IsUserEnrolledWithIdMe = false });

                //act
                var result = SelectPatientApiController.GetIdmeTeaserAdVisibility(_session, _epcsBroker);

                //assert
                Assert.AreEqual(false, result);
            }

            [TestMethod]
            public void should_return_false_on_UserEnrolledWithIdme()
            {
                //arrange
                _session.Stub(_ => _.Cast(Constants.SessionVariables.DisableIdmeTeaserAd, ""));

                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPCSPrompt", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                _session.Stub(_ => _.Cast(Constants.SessionVariables.SessionLicense, default(ApplicationLicense))).Return(al);

                _epcsBroker.Stub(_ => _.GetUserShieldCspStatusInfo(Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.SHARED_DB, "", "", "", "")).IgnoreArguments().Return(new GetUserShieldCspStatusInfoResponse { IsTenantShieldCSPAppInstanceAvailable = true, IsUserEnrolledToZentryCsp = true, IsUserEnrolledWithIdMe = true });

                //act
                var result = SelectPatientApiController.GetIdmeTeaserAdVisibility(_session, _epcsBroker);

                //assert
                Assert.AreEqual(false, result);
            }
        }
    }
}
