using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace Allscripts.ePrescribe.Test.JavaScript
{
    [TestClass]
    [DeploymentItem("editUser.js")]
    public class EditUserTest
    {
        [TestMethod]
        public void should_enable_child_checkbox_when_parent_isChecked()
        {
           var script = JsTestUtil.InitTestScript("editUser.js");
           script.RunTest(@"assert.isTrue(JSON.stringify(updateCheckBoxCheckStatus({checked:true},{checked:false, disabled:true})) == JSON.stringify({checked:true, disabled:false}));");
        }

        [TestMethod]
        public void should_disable_child_checkbox_when_parent_isChecked()
        {
            var script = JsTestUtil.InitTestScript("editUser.js");
            script.RunTest(@"assert.isTrue(JSON.stringify(updateCheckBoxCheckStatus({checked:false},{checked:false, disabled:true})) == JSON.stringify({checked:false, disabled:true}));");
        }
    }
}
