using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.MedicationAspxTests
{
    [TestClass]
    public class SetSearchOptionsTests
    {
        [TestMethod]
        public void should_not_remove_items_from_search_if_workflow_is_not_changeRx()
        {
            //arrange
            var wkFlow = Constants.PrescriptionTaskType.DEFAULT;

            var rblSearch = new RadioButtonList();
            rblSearch.Items.Add(new ListItem("Patient", "P"));
            rblSearch.Items.Add(new ListItem("My", "M"));

            var div = new HtmlGenericControl();

            //act
            MedicationAspx.SetSearchOptions(wkFlow, rblSearch, div);

            //assert
            Assert.AreEqual(2, rblSearch.Items.Count);
        }

        [TestMethod]
        public void should_remove_items_from_search_if_workflow_is_changeRx()
        {
            //arrange
            var wkFlow = Constants.PrescriptionTaskType.RXCHG;

            var rblSearch = new RadioButtonList();
            rblSearch.Items.Add(new ListItem("Patient", "P"));
            rblSearch.Items.Add(new ListItem("My", "M"));

            var div = new HtmlGenericControl();

            //act
            MedicationAspx.SetSearchOptions(wkFlow, rblSearch, div);

            //assert
            Assert.AreEqual(0, rblSearch.Items.Count);
        }

        [TestMethod]
        public void should_not_change_visibility_on_div_if_workflow_is_not_changeRx()
        {
            //arrange
            var wkFlow = Constants.PrescriptionTaskType.DEFAULT;

            var rblSearch = new RadioButtonList();
            rblSearch.Items.Add(new ListItem("Patient", "P"));
            rblSearch.Items.Add(new ListItem("My", "M"));

            var div = new HtmlGenericControl {Visible = true};

            //act
            MedicationAspx.SetSearchOptions(wkFlow, rblSearch, div);

            //assert
            Assert.AreEqual(true, div.Visible);
        }

        [TestMethod]
        public void should_set_visibility_on_div_to_false_if_workflow_is_changeRx()
        {
            //arrange
            var wkFlow = Constants.PrescriptionTaskType.RXCHG;

            var rblSearch = new RadioButtonList();
            rblSearch.Items.Add(new ListItem("Patient", "P"));
            rblSearch.Items.Add(new ListItem("My", "M"));

            var div = new HtmlGenericControl { Visible = true };

            //act
            MedicationAspx.SetSearchOptions(wkFlow, rblSearch, div);

            //assert
            Assert.IsFalse(div.Visible);
        }
    }
}
