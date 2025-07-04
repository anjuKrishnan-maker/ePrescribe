using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Allscripts.ePrescribe.Common;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class ObjectExtensionTests
    {
        [TestMethod]
        public void should_return_null_when_eVal_value_is_null()
        {
            //Arrange
            string parameterValue = null;
            //Act
            string result = parameterValue.ToEvalEncode();
            //Assert
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void should_return_empty_when_eVal_value_is_empty()
        {
            //Arrange
            string parameterValue ="";
            //Act
            string result = parameterValue.ToEvalEncode();
            //Assert
            Assert.AreEqual(result, parameterValue);
        }

        [TestMethod]
        public void should_return_num_when_eVal_value_is_num()
        {
            //Arrange
            string parameterValue = "00005";
            //Act
            string result = parameterValue.ToEvalEncode();
            //Assert
            Assert.AreEqual(result, parameterValue);
        }

        [TestMethod]
        public void should_return_string_when_eVal_value_is_string()
        {
            //Arrange
            string parameterValue = "aaaaa";
            //Act
            string result = parameterValue.ToEvalEncode();
            //Assert
            Assert.AreEqual(result, parameterValue);
        }

        [TestMethod]
        public void should_return_encodedValue_when_eVal_value_is_html()
        {
            //Arrange
            string parameterValue = "<li>Simplifies</li>< li > Provides formulary status </ li ></ ul > ";
            //Act
            string result = parameterValue.ToEvalEncode();
            //Assert
            Assert.AreEqual(result, "&lt;li&gt;Simplifies&lt;/li&gt;&lt; li &gt; Provides formulary status &lt;/ li &gt;&lt;/ ul &gt; ");
        }

        [TestMethod]
        public void should_return_GUIDValue_when_eVal_value_is_GUID()
        {
            //Arrange
            string parameterValue = "{ff00956a-d4fb-4c1f-88f5-4c2877b36e9e}";
            //Act
            string result = parameterValue.ToEvalEncode();
            //Assert
            Assert.AreEqual(result, parameterValue);
        }
    }
}
