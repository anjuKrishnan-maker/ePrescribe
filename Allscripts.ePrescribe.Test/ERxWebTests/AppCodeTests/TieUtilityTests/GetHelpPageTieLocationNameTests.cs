using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TieUtilityTests
{
    [TestClass]
    public class GetHelpPageTieLocationNameTests
    {
        public ITieUtility TieUtility => new TieUtility();

        [TestMethod]
        public void should_return_Home_Page_if_pageName_is_default()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("default");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Home_Page, result);
        }

        [TestMethod]
        public void should_return_GettingStarted_Page_if_pageName_is_gettingstarted()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("gettingstarted");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.GettingStarted_Page, result);
        }

        [TestMethod]
        public void should_return_ToolTips_Page_if_pageName_is_tooltips()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("tooltips");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.ToolTips_Page, result);
        }

        [TestMethod]
        public void should_return_Mobile_Page_if_pageName_is_mobile()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("mobile");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Mobile_Page, result);
        }

        [TestMethod]
        public void should_return_faq_Page_if_pageName_is_faq()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("faq");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.faq_Page, result);
        }

        [TestMethod]
        public void should_return_Tutorial_Page_if_pageName_is_tutorial()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("tutorial");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Tutorial_Page, result);
        }

        [TestMethod]
        public void should_return_Discuss_Page_if_pageName_is_discuss()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("discuss");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Discuss_Page, result);
        }

        [TestMethod]
        public void should_return_Training_Page_if_pageName_is_training()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("training");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Training_Page, result);
        }

        [TestMethod]
        public void should_return_AddOn_Page_if_pageName_is_add_on()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("add-on");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.AddOn_Page, result);
        }

        [TestMethod]
        public void should_return_Import_Page_if_pageName_is_import()
        {
            //act
            var result = TieUtility.GetHelpPageTieLocationName("import");

            //assert
            Assert.AreEqual(Constants.TIELocationPage.Import_Page, result);
        }
    }
}
