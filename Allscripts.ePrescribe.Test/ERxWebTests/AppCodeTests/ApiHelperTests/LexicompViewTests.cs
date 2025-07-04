using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApiHelperTests
{
    [TestClass]
    public class LexicompViewTests
    {
        [TestMethod]
        public void should_return_disabled_if_EnableLexicomp_is_false()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "N";

            dt.Rows.Add(dr);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                }
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Disabled, viewStatus);
        }

        [TestMethod]
        public void should_return_disabled_if_EnableLexicomp_is_true_and_deluxe_status_is_not_true()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            dt.Rows.Add(dr);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.CompulsoryBasic
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Disabled, viewStatus);
        }

        [TestMethod]
        public void should_return_disabled_if_EnableLexicomp_is_true_and_deluxe_status_is_on_and_ShowLexicompDefault_is_false_EnableLexicompDefault_is_false()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            dt.Rows.Add(dr);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.CompulsoryBasic
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Disabled, viewStatus);
        }

        [TestMethod]
        public void should_return_disabled_if_EnableLexicomp_is_true_and_deluxe_status_is_on_and_ShowLexicompDefault_is_true_EnableLexicompDefault_is_false()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            dt.Rows.Add(dr);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.CompulsoryBasic
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShowLexicompDefault)).Return(true);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Disabled, viewStatus);
        }

        [TestMethod]
        public void should_return_enabled_if_EnableLexicomp_is_true_and_deluxe_status_is_on_and_ShowLexicompDefault_is_true_EnableLexicompDefault_is_true()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            var dr1 = dt.NewRow();
            dr1["Name"] = "EnableLexicompDefault";
            dr1["Value"] = "Y";

            dt.Rows.Add(dr);
            dt.Rows.Add(dr1);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.On
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShowLexicompDefault)).Return(true);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Enabled, viewStatus);
        }

        [TestMethod]
        public void should_return_enabled_if_EnableLexicomp_is_true_and_deluxe_status_is_AlwaysOn_and_ShowLexicompDefault_is_true_EnableLexicompDefault_is_true()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            var dr1 = dt.NewRow();
            dr1["Name"] = "EnableLexicompDefault";
            dr1["Value"] = "Y";

            dt.Rows.Add(dr);
            dt.Rows.Add(dr1);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.AlwaysOn
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShowLexicompDefault)).Return(true);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Enabled, viewStatus);
        }

        [TestMethod]
        public void should_return_enabled_if_EnableLexicomp_is_true_and_deluxe_status_is_Cancelled_and_ShowLexicompDefault_is_true_EnableLexicompDefault_is_true()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            var dr1 = dt.NewRow();
            dr1["Name"] = "EnableLexicompDefault";
            dr1["Value"] = "Y";

            dt.Rows.Add(dr);
            dt.Rows.Add(dr1);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShowLexicompDefault)).Return(true);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Enabled, viewStatus);
        }

        [TestMethod]
        public void should_return_enabled_if_EnableLexicomp_is_true_and_deluxe_status_is_Cancelled_and_ShowLexicompDefault_is_false_SHOW_LEXICOMP_is_true()
        {
            //arrange
            var ecDs = new DataSet();
            var dt = new DataTable();
            dt.Columns.AddRange("Name", "Value");

            var dr = dt.NewRow();
            dr["Name"] = "EnableLexicomp";
            dr["Value"] = "Y";

            var dr1 = dt.NewRow();
            dr1["Name"] = "EnableLexicompDefault";
            dr1["Value"] = "Y";

            dt.Rows.Add(dr);
            dt.Rows.Add(dr1);
            ecDs.Tables.Add(dt);

            var sessionLicense = new ApplicationLicense
            {
                EnterpriseClient = new EnterpriseClient
                {
                    DsEnterpriseClient = ecDs
                },
                LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled
            };

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _[Constants.SessionVariables.SessionLicense]).Return(sessionLicense);
            sessionMock.Stub(_ => _.Cast<ApplicationLicense>(Constants.SessionVariables.SessionLicense, null)).Return(sessionLicense);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.SessionVariables.ShowLexicompDefault)).Return(false);
            sessionMock.Stub(_ => _.GetBooleanOrFalse(Constants.LicensePreferences.SHOW_LEXICOMP)).Return(true);

            //act
            var viewStatus = ApiHelper.LexicompView(sessionMock);

            //assert
            Assert.AreEqual(EnabledDisabled.Enabled, viewStatus);
        }
    }
}
