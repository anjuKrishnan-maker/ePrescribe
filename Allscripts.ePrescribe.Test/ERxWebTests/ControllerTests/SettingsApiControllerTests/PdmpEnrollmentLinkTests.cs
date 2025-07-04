using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.State;
using Rhino.Mocks;
using Allscripts.Impact;
using System.Data;
using Allscripts.ePrescribe.Data.PDMP;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class PdmpEnrollmentLinkTests
    {
        [TestMethod, TestCategory("PDMP")]
        public void should_give_enrollment_link_when_enterprise_ShowPDMP_and_license_state_pdmp_enabled_and_Enterprised_EPCS_license()
        {
            ApplicationLicense al = new ApplicationLicense();
            al.LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.AlwaysOn;
            EnterpriseClient ec = new EnterpriseClient(string.Empty);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowPDMP", "Y");
            ds.Tables.Add(dt);

            ec.DsEnterpriseClient = ds;
            al.EnterpriseClient = ec;

            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled)).Return(true);

            var pdmpDataMock = MockRepository.GenerateMock<IPdmpData>();
            pdmpDataMock.Stub(x => x.IsLicenseDefaultSiteStatePdmpEnabled("", DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT)).IgnoreArguments().Return(true);

            //act
            var link = SettingsApiController.GeneratePdmpEnrollmentLink(pageStateMock, pdmpDataMock);
            
            //assert
            Assert.AreEqual("PDMP Enrollment Form", link.Label);
            Assert.AreEqual(Constants.UIModals.PDMP_ENROLLMENT, link.ActionUrl);
        }

        [TestMethod, TestCategory("PDMP")]
        public void should_give_enrollment_link_when_enterprise_ShowPDMP_and_license_state_pdmp_enabled_and_purchased_EPCS_license()
        {
            ApplicationLicense al = new ApplicationLicense();
            al.LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.AlwaysOn;
            al.DeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcs;
            EnterpriseClient ec = new EnterpriseClient(string.Empty);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowPDMP", "Y");
            ds.Tables.Add(dt);

            ec.DsEnterpriseClient = ds;
            al.EnterpriseClient = ec;

            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled)).Return(false);

            var pdmpDataMock = MockRepository.GenerateMock<IPdmpData>();
            pdmpDataMock.Stub(x => x.IsLicenseDefaultSiteStatePdmpEnabled("", DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT)).IgnoreArguments().Return(true);

            //act
            var link = SettingsApiController.GeneratePdmpEnrollmentLink(pageStateMock, pdmpDataMock);

            //assert
            Assert.AreEqual("PDMP Enrollment Form", link.Label);
            Assert.AreEqual(Constants.UIModals.PDMP_ENROLLMENT, link.ActionUrl);
        }

        [TestMethod, TestCategory("PDMP")]
        public void should_give_teasar_link_when_enterprise_ShowPDMP_and_license_state_pdmp_enabled_and_no_EPCS_license()
        {
            ApplicationLicense al = new ApplicationLicense();
            al.LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On;
            al.DeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
            EnterpriseClient ec = new EnterpriseClient(string.Empty);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowPDMP", "Y");
            ds.Tables.Add(dt);

            ec.DsEnterpriseClient = ds;
            al.EnterpriseClient = ec;

            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled)).Return(false);

            var pdmpDataMock = MockRepository.GenerateMock<IPdmpData>();
            pdmpDataMock.Stub(x => x.IsLicenseDefaultSiteStatePdmpEnabled("", DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT)).IgnoreArguments().Return(true);

            //act
            var link = SettingsApiController.GeneratePdmpEnrollmentLink(pageStateMock, pdmpDataMock);

            //assert
            Assert.AreEqual("PDMP Enrollment Form", link.Label);
            Assert.AreEqual(Constants.UIModals.PDMP_EPCS_TEASER, link.ActionUrl);
        }

        [TestMethod, TestCategory("PDMP")]
        public void should_give_null_when_enterprise_no_ShowPDMP_and_license_state_pdmp_enabled()
        {
            ApplicationLicense al = new ApplicationLicense();
            al.LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On;
            al.DeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
            EnterpriseClient ec = new EnterpriseClient(string.Empty);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowPDMP", "N");
            ds.Tables.Add(dt);

            ec.DsEnterpriseClient = ds;
            al.EnterpriseClient = ec;

            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled)).Return(false);

            var pdmpDataMock = MockRepository.GenerateMock<IPdmpData>();
            pdmpDataMock.Stub(x => x.IsLicenseDefaultSiteStatePdmpEnabled("", DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT)).IgnoreArguments().Return(true);

            //act
            var link = SettingsApiController.GeneratePdmpEnrollmentLink(pageStateMock, pdmpDataMock);

            //assert
            Assert.AreEqual(null, link);
        }

        [TestMethod, TestCategory("PDMP")]
        public void should_give_null_when_enterprise_ShowPDMP_and_license_state_pdmp_not_enabled()
        {
            ApplicationLicense al = new ApplicationLicense();
            al.LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On;
            al.DeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
            EnterpriseClient ec = new EnterpriseClient(string.Empty);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowPDMP", "N");
            ds.Tables.Add(dt);

            ec.DsEnterpriseClient = ds;
            al.EnterpriseClient = ec;

            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled)).Return(false);

            var pdmpDataMock = MockRepository.GenerateMock<IPdmpData>();
            pdmpDataMock.Stub(x => x.IsLicenseDefaultSiteStatePdmpEnabled("", DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT)).IgnoreArguments().Return(false);

            //act
            var link = SettingsApiController.GeneratePdmpEnrollmentLink(pageStateMock, pdmpDataMock);

            //assert
            Assert.AreEqual(null, link);
        }

        [TestMethod, TestCategory("PDMP")]
        public void should_give_null_when_enterprise_no_ShowPDMP_and_license_state_pdmp_not_enabled()
        {
            ApplicationLicense al = new ApplicationLicense();
            al.LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On;
            al.DeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
            EnterpriseClient ec = new EnterpriseClient(string.Empty);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Value");

            dt.Rows.Add("ShowPDMP", "N");
            ds.Tables.Add(dt);

            ec.DsEnterpriseClient = ds;
            al.EnterpriseClient = ec;

            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense))).Return(al);
            pageStateMock.Stub(x => x.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled)).Return(false);

            var pdmpDataMock = MockRepository.GenerateMock<IPdmpData>();
            pdmpDataMock.Stub(x => x.IsLicenseDefaultSiteStatePdmpEnabled("", DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT)).IgnoreArguments().Return(false);

            //act
            var link = SettingsApiController.GeneratePdmpEnrollmentLink(pageStateMock, pdmpDataMock);

            //assert
            Assert.AreEqual(null, link);
        }
    }
}
