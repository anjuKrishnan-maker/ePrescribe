using System;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ScriptPadTests
{
    /// <summary>
    /// Summary description for IsSpecMedDestTests
    /// </summary>
    [TestClass]
    public class IsSpecMedDestTests
    {

        [TestMethod]
        public void should_return_true_for_PatientAccessServices_as_selected_value()
        {
            //Arrange
            var dataList = new List<string> { "Patient Access Services" };
            var ddl = new System.Web.UI.WebControls.DropDownList();
            ddl.DataSource = dataList;
            ddl.DataBind();

            //Act
            var result = new ScriptPad().IsSpecMedDest(ddl);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_for_MOB_as_selected_value()
        {
            //Arrange
            var dataList = new List<string> { "MOB" };
            var ddl = new System.Web.UI.WebControls.DropDownList();
            ddl.DataSource = dataList;
            ddl.DataBind();



            //Act
            var result = new ScriptPad().IsSpecMedDest(ddl);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_control_is_not_dropdownlist()
        {
            //Arrange
            var cntrl = new TextBox();

            //Act
            var result = new ScriptPad().IsMailOrderDest(cntrl);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
