using System;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class BuildApproveRadioButtonTcTests
    {
        [TestMethod]
        public void should_create_radio_button_that_is_enabled_if_isApprove_is_true()
        {
            //act
            var result = ChangeRxTask.BuildApproveRadioButtonTc(2, new PharmacyTaskRowControlValues(), true,
                "932");
            
            //assert
            Assert.IsTrue(((RadioButton)result.Controls[0]).Enabled);
        }

        [TestMethod]
        public void should_create_radio_button_that_is_disabled_if_isApprove_is_false()
        {
            //act
            var result = ChangeRxTask.BuildApproveRadioButtonTc(2, new PharmacyTaskRowControlValues(), false,
                "932");

            //assert
            Assert.IsFalse(((RadioButton)result.Controls[0]).Enabled);
        }

        [TestMethod]
        public void should_create_radio_button()
        {
            //act
            var result = ChangeRxTask.BuildApproveRadioButtonTc(2, new PharmacyTaskRowControlValues(), false,
                "932");
            
            //assert
            Assert.AreEqual("<td class=\"valgn\" style=\"border: none !important;\"><span class=\"aspNetDisabled\"><input id=\"\" type=\"radio\" name=\"932select\" value=\"requestedApprove2\" disabled=\"disabled\" onclick=\"enableProcessBtn(); ShowHideControls(, , , , );\" /><label for=\"\">Approve</label></span></td>", UiHelper.RenderControl(result));
        }
    }
}
