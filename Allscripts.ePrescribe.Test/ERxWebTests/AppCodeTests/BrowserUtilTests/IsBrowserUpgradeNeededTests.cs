using System;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.AppCodeTests.BrowserUtilTests
{
    [TestClass]
    public class IsBrowserUpgradeNeededTests
    {
        private const double TridentMin = 5.0;
        private const double FireFoxMin = 39.0;
        private const double ChromeMin = 44.0;
        private const double SafariMin = 7.0;

        [TestMethod]
        public void should_return_true_if_layout_engine_contains_trident_and_is_less_than_min()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "trident"+ (TridentMin-1);

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_layout_engine_contains_trident_and_is_equal_to_min()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "trident" + TridentMin;

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_layout_engine_contains_trident_and_is_greater_than_min()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "trident" + (TridentMin + 1);

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_layout_engine_contains_trident_and_does_not_have_a_version()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "trident";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_layout_engine_is_gecko_version_is_not_equal_to_20100101_and_is_less_than_min()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "gecko" + (FireFoxMin - 1);

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_layout_engine_is_gecko_version_is_not_equal_to_20100101_and_is_greater_than_min()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "gecko" + (FireFoxMin + 1);

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_layout_engine_is_gecko_version_is_not_equal_to_20100101_and_is_equal_to_min()
        {
            //arrange
            var userAgent = "";
            var layoutEngine = "gecko" + FireFoxMin;

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_layout_engine_is_gecko_version_is_equal_to_20100101_and_is_less_than_min()
        {
            //arrange
            var userAgent = "firefox/"+ (FireFoxMin - 1);
            var layoutEngine = "gecko20100101";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_layout_engine_is_gecko_version_is_equal_to_20100101_and_is_greater_than_min()
        {
            //arrange
            var userAgent = "firefox/" + (FireFoxMin + 1);
            var layoutEngine = "gecko20100101";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_layout_engine_is_gecko_version_is_equal_to_20100101_and_is_equal_to_min()
        {
            //arrange
            var userAgent = "firefox/" + FireFoxMin;
            var layoutEngine = "gecko20100101";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_user_agent_is_chrome_and_is_less_than_min()
        {
            //arrange
            var userAgent = "chrome/" + (ChromeMin - 1) + ".32.45 ";
            var layoutEngine = "";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_user_agent_is_chrome_and_is_greater_than_min()
        {
            //arrange
            var userAgent = "chrome/" + (ChromeMin + 1) + ".32.45 ";
            var layoutEngine = "";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_user_agent_is_chrome_and_is_equal_to_min()
        {
            //arrange
            var userAgent = "chrome/" + ChromeMin + ".32.45 ";
            var layoutEngine = "";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_user_agent_is_version_and_is_less_than_min()
        {
            //arrange
            var userAgent = "version/" + (SafariMin - 1) + ".32.45 ";
            var layoutEngine = "";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_user_agent_is_version_and_is_greater_than_min()
        {
            //arrange
            var userAgent = "version/" + (SafariMin + 1) + ".32.45 ";
            var layoutEngine = "";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_user_agent_is_version_and_is_equal_to_min()
        {
            //arrange
            var userAgent = "version/" + SafariMin + ".32.45 ";
            var layoutEngine = "";

            //act
            var result = new BrowserUtil().IsBrowserUpgradeNeeded(userAgent, layoutEngine);

            //assert
            Assert.IsFalse(result);
        }
    }
}
