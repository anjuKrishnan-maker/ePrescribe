using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetMasterPageTieLocationNameTests
    {
        public ITieUtility TieUtility => new TieUtility();

        [TestMethod]
        public void should_return_Login_Page_if_pagename_is_login()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("login");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Login_Page, result);
        }

        [TestMethod]
        public void should_return_SearchPatient_Page_if_pagename_is_searchpatient()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("searchpatient");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.SearchPatient_Page, result);
        }

        [TestMethod]
        public void should_return_SearchPatient_Page_if_pagename_is_default()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("default");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.SearchPatient_Page, result);
        }

        [TestMethod]
        public void should_return_Settings_Page_if_pagename_is_settings()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("settings");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Settings_Page, result);
        }

        [TestMethod]
        public void should_return_tools_Page_if_pagename_is_manageaccount()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("manageaccount");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.ManageAccount_Page, result);
        }

        [TestMethod]
        public void should_return_MessageQueueTx_Page_if_pagename_is_messagequeuetx()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("messagequeuetx");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.MessageQueueTx_Page, result);
        }

        [TestMethod]
        public void should_return_MessageQueueTx_Page_if_pagename_is_messagequeuetxfromreports()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("messagequeuetx?from=reports");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.MessageQueueTx_Page, result);
        }

        [TestMethod]
        public void should_return_MyProfile_Page_if_pagename_is_myprofile()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("myprofile");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.MyProfile_Page, result);
        }

        [TestMethod]
        public void should_return_Reports_Page_if_pagename_is_reports()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("reports");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Reports_Page, result);
        }

        [TestMethod]
        public void should_return_SelectDx_Page_if_pagename_is_diagnosis()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("diagnosis");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.SelectDx_Page, result);
        }

        [TestMethod]
        public void should_return_SelectAccountAndSite_Page_if_pagename_is_selectaccountandsite()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("selectaccountandsite");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.SelectAccountAndSite_Page, result);
        }

        [TestMethod]
        public void should_return_AddPatient_Page_if_pagename_is_addpatient()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("addpatient");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.AddPatient_Page, result);
        }

        [TestMethod]
        public void should_return_Library_Page_if_pagename_is_library()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("library");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Library_Page, result);
        }

        [TestMethod]
        public void should_return_Exception_Page_if_pagename_is_exception()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("exception");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Exception_Page, result);
        }

        [TestMethod]
        public void should_return_stringEmpty_if_pagename_is_empty()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("");

            //assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void should_return_NoAnalytics_if_pagename_is_approverefilltask()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("approverefilltask");

            //assert
            Assert.AreEqual("NoAnalytics", result);
        }

        [TestMethod]
        public void should_return_NoAnalytics_if_pagename_is_pharmacy()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("pharmacy");

            //assert
            Assert.AreEqual("NoAnalytics", result);
        }

        [TestMethod]
        public void should_return_NoAnalytics_if_pagename_is_multipleviewcss()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("multipleviewcss");

            //assert
            Assert.AreEqual("NoAnalytics", result);
        }

        [TestMethod]
        public void should_return_NoAnalytics_if_pagename_is_taskprocessor()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("taskprocessor");

            //assert
            Assert.AreEqual("NoAnalytics", result);
        }

        [TestMethod]
        public void should_return_NoAnalytics_if_pagename_is_taskscript()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("taskscript");

            //assert
            Assert.AreEqual("NoAnalytics", result);
        }

        [TestMethod]
        public void should_return_stringEmpty_if_pagename_is_not_listed()
        {
            //act
            var result = TieUtility.GetMasterPageTieLocationName("NewFancyPage");

            //assert
            Assert.AreEqual(string.Empty, result);
        }
    }
}
