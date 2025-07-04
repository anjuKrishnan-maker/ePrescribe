using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.Impact.Utilities;

namespace Allscripts.ePrescribe.Test
{
    public class ObjectTypeExtensionMethodsTests
    {
        [TestClass]
        public class RetrieveBooleanValueMethodTest
        {
            [TestMethod]
            public void should_return_false_if_DBNull()
            {
                // Arrange
                object o = DBNull.Value;

                //Act
                bool result = o.RetrieveBooleanValue();

                //Assert
                Assert.AreEqual(result, false);
            }

            [TestMethod]
            public void should_return_false_if_null()
            {
                // Arrange
                object o = null;

                //Act
                bool result = o.RetrieveBooleanValue();

                //Assert
                Assert.AreEqual(result, false);
            }

            [TestMethod]
            public void should_return_true_if_string_value_true()
            {
                // Arrange
                object o = "true";

                //Act
                bool result = o.RetrieveBooleanValue();

                //Assert
                Assert.AreEqual(result, true);
            }


            [TestMethod]
            public void should_return_false_if_string_value_false()
            {
                // Arrange
                object o = "false";

                //Act
                bool result = o.RetrieveBooleanValue();

                //Assert
                Assert.AreEqual(result, false);
            }

            [TestMethod]
            public void should_return_false_if_string_value_inValid()
            {
                // Arrange
                object o = "JunkString";

                //Act
                bool result = o.RetrieveBooleanValue();

                //Assert
                Assert.AreEqual(result, false);
            }
        }
    }
}
