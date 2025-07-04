using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.Common.ExtensionMethodTests
{
    [TestClass]
    public class ToEnumObject
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
            //arrange
            object obj = 12;

            //act
            obj.ToEnum<int>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_object_is_null()
        {
            //arrange
            object obj = null;

            //act
            obj.ToEnum<int>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_string_is_not_defined_in_enum()
        {
            //arrange
            object obj = "Mouse";

            //act
            obj.ToEnum<ToEnumStringTests.TestEnum>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_int_is_not_defined_in_enum()
        {
            //arrange
            object obj = 2;

            //act
            obj.ToEnum<ToEnumStringTests.TestEnum>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void should_throw_exception_if_object_is_not_string_or_int()
        {
            //arrange
            object obj = new List<string>();

            //act
            obj.ToEnum<ToEnumStringTests.TestEnum>();
        }

        [TestMethod]
        public void should_return_enum_value_type()
        {
            //arrange
            object obj = 3;

            //act
            var result = obj.ToEnum<ToEnumStringTests.TestEnum>();

            //assert
            Assert.AreEqual(ToEnumStringTests.TestEnum.DOG, result);
        }

        [TestMethod]
        public void should_return_enum_value_type2()
        {
            //arrange
            object obj = "CAT";

            //act
            var result = obj.ToEnum<ToEnumStringTests.TestEnum>();

            //assert
            Assert.AreEqual(ToEnumStringTests.TestEnum.CAT, result);
        }
    }
}
