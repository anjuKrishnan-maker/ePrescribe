using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class ToEnumStringTests
    {
        public enum TestEnum
        {
            CAT = 1,
            DOG = 3
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_T_is_not_enum()
        {
            //act
            "me".ToEnum<int>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_string_is_not_defined_in_enum()
        {
            //act
            "me".ToEnum<TestEnum>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_string_is_not_defined_in_enum1()
        {
            //act
            "1".ToEnum<TestEnum>();
        }

        [TestMethod]
        public void should_return_enum_value_type()
        {
            //act
            var result = "CAT".ToEnum<TestEnum>();

            //assert
            Assert.AreEqual(TestEnum.CAT, result);
        }
    }
}
