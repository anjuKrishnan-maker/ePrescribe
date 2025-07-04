using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.JsonHelperTests
{
    [TestClass]
    public class ConvertUrlParametersTests
    {
        [TestMethod]
        public void should_return_empty_if_urlParams_are_null()
        {
            //arrange
            string urlParms = null;

            //act
            var results = JsonHelper.ConvertUrlParameters(urlParms);

            //assert
            Assert.AreEqual("", results);
        }

        [TestMethod]
        public void should_return_empty_if_urlParams_is_empty()
        {
            //arrange
            string urlParms = " ";

            //act
            var results = JsonHelper.ConvertUrlParameters(urlParms);

            //assert
            Assert.AreEqual("", results);
        }

        [TestMethod]
        public void should_return_json_string()
        {
            //arrange
            string urlParms = "workflow=task&drug=advil";

            //act
            var results = JsonHelper.ConvertUrlParameters(urlParms);

            //assert: 
            Assert.AreEqual("{%22workflow%22%3a+%22task%22%2c%22drug%22%3a+%22advil%22}", results);
        }
    }
}
