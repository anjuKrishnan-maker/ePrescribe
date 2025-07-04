using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.SettingsApiControllerTests
{
    [TestClass]
    public class MergePatientsLinkTests
    {
        [TestMethod]
        public void should_add_link()
        {
            //act
            var link = SettingsApiController.GenerateMergePatientsLink(EnabledDisabled.Enabled);

            //assert
            Assert.AreEqual("Merge Patients", link.Label);
            Assert.AreEqual(Constants.PageNames.MERGE_PATIENTS, link.ActionUrl);
        }

        [TestMethod]
        public void should_not_add_link()
        {
            //act
            var link = SettingsApiController.GenerateMergePatientsLink(EnabledDisabled.Disabled);

            //assert
            Assert.IsNull(link);
        }
    }
}
