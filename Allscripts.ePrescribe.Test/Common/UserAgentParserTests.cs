using System;
using Allscripts.ePrescribe.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common
{
    [TestClass]
    public class UserAgentParserTests
    {

        [TestMethod]
        public void should_return_default_if_bogus_input_is_passed()
        {
            // arrange

            var bogus = "slfjsdfowefijs fdslfje sldif-582-4-5u";
            var defaultValue = "Unknown";

            // act

            var result = UserAgentParser.GetPlatformFromUserAgent(defaultValue, bogus);

            // assert

            Assert.AreEqual("Unknown", result);


        }

        [TestMethod]
        public void should_return_default_on_null()
        {
            // arrange

            var defaultValue = "Unknown";

            // act

            var result = UserAgentParser.GetPlatformFromUserAgent(defaultValue, null);

            // assert

            Assert.AreEqual("Unknown", result);

        }


                [TestMethod]
        public void should_return_valid_platform_for_vista()
        {
            // arrange

            var defaultValue = "Unknown";

                    var agentString = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; InfoPath.1; SV1; .NET CLR 3.8.36217; WOW64; en-US)";

            // act

            var result = UserAgentParser.GetPlatformFromUserAgent(defaultValue, agentString);

            // assert

            Assert.AreEqual("WinVista", result);

        }
        [TestMethod]
        public void should_return_default_on_empty()
        {
            // arrange

            var defaultValue = "Unknown";

            // act

            var result = UserAgentParser.GetPlatformFromUserAgent(defaultValue, "");

            // assert

            Assert.AreEqual("Unknown", result);

        }


         [TestMethod]
        public void should_return_valid_platform_for_win7()

        {
            // arrange
            var uaString = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; chromeframe/13.0.782.215)";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Win7", result);

        }
        
        [TestMethod]
        public void should_return_valid_platform_for_win8()

        {
            // arrange
            var uaString = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0) ";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Win8", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_win81()

        {
            // arrange
            var uaString = "Mozilla/5.0 (Windows NT 6.3; Win64, x64; Trident/7.0; Touch; rv:11.0) like Gecko";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Win8.1", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_win10()
        {
            // arrange
            var uaString = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) ";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Win10", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_win_phone()
        {
            // arrange
            var uaString = "Mozilla/5.0 (Mobile; Windows Phone 8.1; Android 4.0; ARM; Trident/7.0; Touch; rv:11.0; IEMobile/11.0; NOKIA; Lumia 520) like iPhone OS 7_0_3 Mac OS X AppleWebKit/537 (KHTML, like Gecko) Mobile Safari/537";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("WinPhone", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_mac()
        {
            // arrange
            var uaString = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.130 Safari/537.36 ";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Mac", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_iphone()
        {
            // arrange
            var uaString = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_4 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) CriOS/44.0.2403.67 Mobile/12H143 Safari/600.1.";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("iPhone", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_ipad()
        {
            // arrange
            var uaString = "Mozilla/5.0(iPad; U; CPU iPhone OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Version/4.0.4 Mobile/7B314 Safari/531.21.10";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("iPad", result);

        }

        [TestMethod]
        public void should_return_valid_platform_for_android1()
        {
            // arrange
            var uaString = "Mozilla/5.0 (Linux; U; Android 4.0.3; ko-kr; LG-L160L Build/IML74K) AppleWebkit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Android", result);

        }


        [TestMethod]
        public void should_return_valid_platform_for_android2()
        {
            // arrange
            var uaString = "Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0";

            // act
            var result = UserAgentParser.GetPlatformFromUserAgent("blah", uaString);

            // assert
            Assert.AreEqual("Android", result);

        }

        [TestMethod]
        public void should_return_layout_engine_ie7trhough10()
        {
            var uaString = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MS-RTC LM 8; InfoPath.3; .NET4.0C; .NET4.0E) chromeframe/8.0.552.224";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("trident4.0", result);
        }
 
        [TestMethod]
        public void should_return_layout_engine_ie10inEdgeMode()
        {
            var uaString =
                "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; .NET4.0E; .NET4.0C; Tablet PC 2.0; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; InfoPath.3; rv:11.0) like Gecko";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("trident7.0", result);
        }

         [TestMethod]
        public void should_return_layout_engine_for_chrome()
        {
            var uaString =
                "Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; Googlebot/2.1; +http://www.google.com/bot.html) Safari/537.36";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("applewebkit537.36", result);
        }

        [TestMethod]
        public void should_return_layout_engine_for_opera()
        {
            var uaString = "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; fr) Presto/2.9.168 Version/11.52";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("presto2.9.168", result);
        }

        [TestMethod]
        public void should_return_layout_engine_for_firefox()
        {
            var uaString = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("gecko20100101", result);
        }

        [TestMethod]
        public void should_return_empty_if_bogus_input_provided()
        {

            var uaString = "dsfdlkj dlfjdsls fdslfjf dslfjsl";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void should_return_unknown_if_malformed_input_provided()
        {

            var uaString = "Opera/9.80 (Macintosh; Intel Mac OS X 10.6.8; U; fr)  applewebkit";
            var result = UserAgentParser.GetLayoutEngineFromUserAgent(uaString);

            Assert.AreEqual("UNKNOWN", result);
        }
    }
}
