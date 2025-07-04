using System.IO;
using JSTest;
using JSTest.ScriptLibraries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test
{
    public static class JsTestUtil
    {

        public static TestScript InitTestScript(string scriptFileName)
        {
            var script = new TestScript();
            var path = Path.GetFullPath(".");
            script.AppendFile(string.Format(@"{0}", scriptFileName));
            script.AppendBlock(new JsAssertLibrary());

            return script;
        }

        public static TestScript InitTestScript(string scriptFileName , string stubFileName)
        {
            var script = new TestScript();
            var path = Path.GetFullPath(".");
            script.AppendFile(string.Format(@"{0}", scriptFileName));
            
            script.AppendFile(string.Format(@"{0}", stubFileName));
            script.AppendBlock(new JsAssertLibrary());

            return script;
        }

    }
}