using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb;
using Allscripts.Impact;
using System.Data;
using Rhino.Mocks;
using eRxWeb.State;

namespace Allscripts.ePrescribe.Test
{
    public class SiteManagementTests
    {
        [TestClass]
        public class IsShowREpaTests
        {
            [TestMethod, TestCategory("Retrospective EPA")]
            public void should_return_true_when_Enterpriese_ShowEPA_Y_LicensePreference_ShowEPA_Y_Enterpriese_ShowREPA_Y()
            {
                // Arrange
                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPA", "Y");
                dt.Rows.Add("ShowRePA", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;
                
                var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
                pageStateMock.Stub(x => x["SessionLicense"]).Return(al);
                pageStateMock.Stub(x => x.GetBooleanOrFalse("SHOW_EPA")).Return(true);

                SiteManagement sm = new SiteManagement();
                sm.PageState = pageStateMock;

                //Act
                bool actualResult = sm.IsShowREpa();

                //Assert
                Assert.IsTrue(actualResult);
            }

            [TestMethod, TestCategory("Retrospective EPA")]
            public void should_return_false_when_Enterpriese_ShowEPA_Y_LicensePreference_ShowEPA_Y_Enterpriese_ShowREPA_N()
            {
                // Arrange
                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPA", "Y");
                dt.Rows.Add("ShowRePA", "N");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
                pageStateMock.Stub(x => x["SessionLicense"]).Return(al);
                pageStateMock.Stub(x => x.GetBooleanOrFalse("SHOW_EPA")).Return(true);

                SiteManagement sm = new SiteManagement();
                sm.PageState = pageStateMock;

                //Act
                bool actualResult = sm.IsShowREpa();

                //Assert
                Assert.IsFalse(actualResult);
            }

            [TestMethod, TestCategory("Retrospective EPA")]
            public void should_return_false_when_Enterpriese_ShowEPA_N_LicensePreference_ShowEPA_N_Enterpriese_ShowREPA_Y()
            {
                // Arrange
                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPA", "N");
                dt.Rows.Add("ShowRePA", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
                pageStateMock.Stub(x => x["SessionLicense"]).Return(al);
                pageStateMock.Stub(x => x.GetBooleanOrFalse("SHOW_EPA")).Return(false);

                SiteManagement sm = new SiteManagement();
                sm.PageState = pageStateMock;

                //Act
                bool actualResult = sm.IsShowREpa();

                //Assert
                Assert.IsFalse(actualResult);
            }

            [TestMethod, TestCategory("Retrospective EPA")]
            public void should_return_false_when_Enterpriese_ShowEPA_N_LicensePreference_ShowEPA_Y_Enterpriese_ShowREPA_Y()
            {
                // Arrange
                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPA", "N");
                dt.Rows.Add("ShowRePA", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
                pageStateMock.Stub(x => x["SessionLicense"]).Return(al);
                pageStateMock.Stub(x => x.GetBooleanOrFalse("SHOW_EPA")).Return(true);

                SiteManagement sm = new SiteManagement();
                sm.PageState = pageStateMock;

                //Act
                bool actualResult = sm.IsShowREpa();

                //Assert
                Assert.IsFalse(actualResult);
            }

            [TestMethod, TestCategory("Retrospective EPA")]
            public void should_return_false_when_Enterpriese_ShowEPA_Y_LicensePreference_ShowEPA_N_Enterpriese_ShowREPA_Y()
            {
                // Arrange
                ApplicationLicense al = new ApplicationLicense();
                EnterpriseClient ec = new EnterpriseClient(string.Empty);

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("Name");
                dt.Columns.Add("Value");

                dt.Rows.Add("ShowEPA", "Y");
                dt.Rows.Add("ShowRePA", "Y");
                ds.Tables.Add(dt);

                ec.DsEnterpriseClient = ds;
                al.EnterpriseClient = ec;

                 var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
                pageStateMock.Stub(x => x["SessionLicense"]).Return(al);
                pageStateMock.Stub(x => x.GetBooleanOrFalse("SHOW_EPA")).Return(false);

                SiteManagement sm = new SiteManagement();
                sm.PageState = pageStateMock;

                //Act
                bool actualResult = sm.IsShowREpa();

                //Assert
                Assert.IsFalse(actualResult);
            }
        }
    }
}
