using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.JavaScript
{
    [TestClass]
    [DeploymentItem("ePA_Questions.js")]
    public class ePA_QuestionsIsNumericTest
    {
        [TestMethod]
        public void should_correctly_detect_is_numeric()
        {
            var script = JsTestUtil.InitTestScript("ePA_Questions.js");
            script.RunTest(@"assert.isTrue(IsNumeric('2343'));");
        }

        [TestMethod]
        public void should_correctly_detect_is_not_numeric()
        {
            var script = JsTestUtil.InitTestScript("ePA_Questions.js");
            script.RunTest(@"assert.isFalse(IsNumeric('wwesf'));");
        }



        [TestMethod]
        public void should_treat_ammpersand_as_not_numeric()
        {
            var script = JsTestUtil.InitTestScript("ePA_Questions.js");
            script.RunTest(@"assert.isFalse(IsNumeric('23&43'));");
        }

        [TestMethod]
        public void should_detect_letter_at_the_end_of_the_input()
        {
            var script = JsTestUtil.InitTestScript("ePA_Questions.js");
            script.RunTest(@"assert.isFalse(IsNumeric('2343a'));");
        }
        
    }
}