using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Web.UI;
using System.Collections.Generic;

namespace Allscripts.ePrescribe.Test.ScriptPadTests
{
    [TestClass]
    public class IsMailOrderDestTests
    {
        [TestMethod]
        public void should_return_true_for_MOB_as_selected_value()
        {
            //Arrange
            var dataList = new List<string> {"MOB"};
            var ddl = new DropDownList();
            ddl.DataSource = dataList;
            ddl.DataBind();
            


            //Act
            var result = new ScriptPad().IsMailOrderDest(ddl);

            //Assert
            Assert.IsTrue(result);
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
        

        [TestMethod]
        public void should_return_true_if_selected_value_is_DocMob()
        {
            //Arrange
            var dataList = new List<string> { "DOCMOB" };
            var ddl = new DropDownList();
            ddl.DataSource = dataList;
            ddl.DataBind();



            //Act
            var result = new ScriptPad().IsMailOrderDest(ddl);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_no_mail_order_pharm_is_selected()
        {
            //Arrange
            var dataList = new List<string> { "NotMO" };
            var ddl = new DropDownList();
            ddl.DataSource = dataList;
            ddl.DataBind();



            //Act
            var result = new ScriptPad().IsMailOrderDest(ddl);

            //Assert
            Assert.IsFalse(result);
        }
    }

}
