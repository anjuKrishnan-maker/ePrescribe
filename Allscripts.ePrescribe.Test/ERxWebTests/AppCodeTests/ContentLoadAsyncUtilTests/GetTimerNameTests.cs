using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ContentLoadAsyncUtilTests
{
    [TestClass]
    public class GetTimerNameTests
    {
        [TestMethod]
        public void return_timer_name()
        {
            //act
            var asyncUtil = new LoadAsyncUtil(new AsyncContext(null,  null, null, "ContentLoad"), null);
            var result = asyncUtil.GetTimerName("Method");

            //assert
            Assert.AreEqual("ContentLoad-Method", result);
        }
    }
}
