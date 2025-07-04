using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class ToGuidTests
    {
        [TestMethod]
        [ExpectedException(typeof (InvalidCastException))]
        public void should_throw_invalid_cast_exception_if_value_is_null()
        {
            //arrange
            object value = null;

            //act
            var result = value.ToGuid();

            //assert
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_invalid_cast_exception_if_value_is_not_guid()
        {
            //arrange
            object value = "1";

            //act
            var result = value.ToGuid();

            //assert
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_invalid_cast_exception_if_value_is_int()
        {
            //arrange
            object value = 1;

            //act
            var result = value.ToGuid();

            //assert
        }

        [TestMethod]
        public void should_return_guid()
        {
            //arrange
            var guid = Guid.NewGuid();
            object value = guid.ToString();

            //act
            var result = value.ToGuid();

            //assert
            Assert.AreEqual(guid, result);
        }
    }
}
