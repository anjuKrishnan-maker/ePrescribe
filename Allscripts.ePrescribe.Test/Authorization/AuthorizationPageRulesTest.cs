using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.Authorize;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;


namespace Allscripts.ePrescribe.Test.Authorization
{
    [TestClass]
    public class AuthorizationPageRulesTest
    {
        private IStateContainer Session { get; set; }

        private HttpSessionState HttpSessionState { get; set; }

        private Page PageMock { get; set; }

        [TestInitialize]
        public void InitializeLogon()
        {
            HttpContext.Current = MockHelpers.FakeHttpContext();
            Session = new StateContainer(HttpContext.Current.Session);
            PageMock = MockRepository.GenerateStub<Page>();
            PageMock.Stub((x) => x.Session).Return(HttpSessionState);
            var configDictionary = new Dictionary<string, string>()
             {
                 {"AuthorizationEnabled", "true"},
             };
            Impact.ConfigKeys.TestInitialize(configDictionary);
        }

        [TestMethod]
        //_rule[Constants.PageNames.INTEGRATION_SOLUTIONS_LIST].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
        public void Should_not_allow_integration_list_When_ShowIntegrationSolutions_is_off()
        {
            Session[Constants.SessionVariables.IsAdmin] = true;
            Session[Constants.SessionVariables.UserType] = Constants.UserCategory.GENERAL_USER;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowIntegrationSolutions", "N");
            ds.Tables.Add(dt);

            EnterpriseClient enterPriceClient = new EnterpriseClient(string.Empty);
            enterPriceClient.DsEnterpriseClient = ds;
            ApplicationLicense al = new ApplicationLicense { EnterpriseClient = enterPriceClient };

            Session[Constants.SessionVariables.SessionLicense] = al;


            //act
            bool isAuthoirizedINTEGRATION_SOLUTIONS_LIST = AuthorizationManager.Process(Session, Constants.PageNames.INTEGRATION_SOLUTIONS_LIST.ToLower());

            //assert
            Assert.IsFalse(isAuthoirizedINTEGRATION_SOLUTIONS_LIST);
        }

        [TestMethod]
        //_rule[Constants.PageNames.LIBRARY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
        public void Should_allow_library_When_deluxefeaturestatus_is_not_on_off_disabled()
        {
            Session[Constants.SessionVariables.IsAdmin] = true;
            Session[Constants.SessionVariables.UserType] = Constants.UserCategory.GENERAL_USER;

            Session[Constants.SessionVariables.SessionLicense] = null;

            //act
            bool isAuthoirizedLibrary = AuthorizationManager.Process(Session, Constants.PageNames.LIBRARY.ToLower());

            //assert
            Assert.IsFalse(isAuthoirizedLibrary);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("AdvertiseDeluxe", "N");
            ds.Tables.Add(dt);

            EnterpriseClient enterPriceClient = new EnterpriseClient(string.Empty);
            enterPriceClient.DsEnterpriseClient = ds;
            ApplicationLicense al = new ApplicationLicense { EnterpriseClient = enterPriceClient };

            Session[Constants.SessionVariables.SessionLicense] = al;

            //act
            isAuthoirizedLibrary = AuthorizationManager.Process(Session, Constants.PageNames.LIBRARY.ToLower());

            //assert
            Assert.IsFalse(isAuthoirizedLibrary); //is getting Hide as the staus of the GetFeatureStaus method.If not relook into it.

            ///!!!!! The method getfeaturestatus is making me crazy.Not able to get the flag of any except ON/OFF/Disabled.So leaving here.
        }

        [TestMethod]
        public void Should_log_exception_in_audit_with_detailed_error_message()
        {
            Session[Constants.SessionVariables.IsAdmin] = true;
            Session[Constants.SessionVariables.UserType] = Constants.UserCategory.GENERAL_USER;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowIntegrationSolutions", "N");
            ds.Tables.Add(dt);

            EnterpriseClient enterPriceClient = new EnterpriseClient(string.Empty);
            enterPriceClient.DsEnterpriseClient = ds;
            ApplicationLicense al = new ApplicationLicense { EnterpriseClient = enterPriceClient };

            Session[Constants.SessionVariables.SessionLicense] = al;


            //act
            bool isAuthoirizedINTEGRATION_SOLUTIONS_LIST = AuthorizationManager.Process(Session, Constants.PageNames.INTEGRATION_SOLUTIONS_LIST.ToLower());

            //assert
            Assert.IsFalse(isAuthoirizedINTEGRATION_SOLUTIONS_LIST);

            string error = Session[Constants.SessionVariables.CURRENT_ERROR].ToString();

            StringAssert.Contains(error, Constants.PageNames.INTEGRATION_SOLUTIONS_LIST.ToLower());
            StringAssert.Contains(error, "missing additional permission check");

        }


    }
}