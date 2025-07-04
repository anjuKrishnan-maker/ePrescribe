using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class ToEnumIntTests
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
            12.ToEnum<int>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_int_is_not_defined_in_enum()
        {
            //act
            2.ToEnum<ToEnumStringTests.TestEnum>();
        }

        [TestMethod]
        public void should_return_enum_value_type()
        {
            //act
            var result = 3.ToEnum<ToEnumStringTests.TestEnum>();

            //assert
            Assert.AreEqual(ToEnumStringTests.TestEnum.DOG, result);
        }
    }
}
