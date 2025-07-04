using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test
{
    [TestClass]
    public class PageNameTest
    {

        [TestMethod]
        public void should_return_correct_page_for_valid_target_url()
        {
            //arrange
            string targetPage = "SelectPatient";

            //act
            string result = Constants.PageNames.GetPageOrNotFoundPage(targetPage);

            //assert
            string expected = "SelectPatient";
            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        public void should_return_page_not_found_page_for_invalid_target_url()
        {
            //arrange
            string targetPage = "InvalidTest";

            //act
            string result = Constants.PageNames.GetPageOrNotFoundPage(targetPage);

            //assert
            string expected = "PagetNotFound.aspx";
            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_return_exception_page_for_target_url_null()
        {
            //arrange
            string targetPage = null;

            //act
            string result = Constants.PageNames.UrlForRedirection(targetPage);
        }

        [TestMethod]
        public void should_return_correct_page_for_valid_target_url_with_query_string_data()
        {
            //arrange
            string targetPage = "AddPatient.aspx?Msg=Successfull";

            //act
            string result = Constants.PageNames.UrlForRedirection(targetPage);

            //assert
            string expected = "AddPatient.aspx?Msg=Successfull";
            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        public void should_return_empty_string_for_invalid_page()
        {
            //arrange
            string targetPage = "hackedPage.aspx?Msg=Failed";

            //act
            string result = Constants.PageNames.UrlForRedirection(targetPage);

            //assert
            string expected = string.Empty;
            Assert.AreEqual(expected, result);

        }


        [TestMethod]
        public void should_return_correct_page_for_valid_target_https_url_with_query_string_data()
        {
            //arrange
            string targetPage = "https://eprescribe.allscripts.com/AddPatient.aspx?ReturnUrl=https://eprescribe.allscripts.com/EditPatient.aspx?msg=none";

            //act
            string result = Constants.PageNames.UrlForRedirection(targetPage);

            //assert
            string expected = "https://eprescribe.allscripts.com/AddPatient.aspx?ReturnUrl=https://eprescribe.allscripts.com/EditPatient.aspx?msg=none";
            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        public void should_return_empty_string_for_invalid_https_url()
        {
            //arrange
            string targetPage = "https://eprescribe.allscripts.com/hackedPage.aspx?Msg=Failed";

            //act
            string result = Constants.PageNames.UrlForRedirection(targetPage);

            //assert
            string expected = string.Empty;
            Assert.AreEqual(expected, result);

        }

    }
}
