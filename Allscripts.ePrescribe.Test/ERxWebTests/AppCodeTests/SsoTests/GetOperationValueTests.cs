using System;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class GetOperationValueTests
    {
        [TestMethod]
        public void should_return_operation_user_guid_if_opertion_is_default_value()
        {
            //arrange
            var opertion = default(string);

            //act
            var result = Sso.GetOperationValue(opertion);

            //assert
            Assert.AreEqual(Constants.SsoOperations.OPERATION_USERGUID, result);
        }

        [TestMethod]
        public void should_return_int32_version_of_string_if_it_does_not_contain_x()
        {
            //arrange
            var opertion = "231";

            //act
            var result = Sso.GetOperationValue(opertion);

            //assert
            Assert.AreEqual(Convert.ToInt32(opertion), result);
        }

        [TestMethod]
        public void should_return_int32_base_16_version_of_string_if_it_contains_x()
        {
            //arrange
            var opertion = "0x143";

            //act
            var result = Sso.GetOperationValue(opertion);

            //assert
            Assert.AreEqual(Convert.ToInt32(opertion, 16), result);
        }
    }
}
