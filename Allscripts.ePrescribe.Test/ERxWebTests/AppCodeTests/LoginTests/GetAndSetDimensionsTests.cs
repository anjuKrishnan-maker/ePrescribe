using System;
using eRxWeb;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.LoginTests
{
    [TestClass]
    public class GetAndSetDimensionsTests
    {
        [TestMethod]
        public void should_set_height_to_1000_if_height_is_null()
        {
            //arrange
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions(null, "23", mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEHEIGHT"], 1000);
        }

        [TestMethod]
        public void should_set_height_to_1000_if_height_is_non_convertible()
        {
            //arrange  
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions("Ethan Is Cool!", "23", mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEHEIGHT"], 1000);
        }

        [TestMethod]
        public void should_set_height_to_1000_if_height_is_less_than_505()
        {
            //arrange  
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions("504", "23", mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEHEIGHT"], 1000);
        }

        [TestMethod]
        public void should_set_height_to_input_if_height_is_more_than_504()
        {
            //arrange  
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions("505", "23", mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEHEIGHT"], 505);
        }

        [TestMethod]
        public void should_set_width_to_900_if_width_is_null()
        {
            //arrange
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions("23", null, mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEWIDTH"], 900);
        }

        [TestMethod]
        public void should_set_width_to_900_if_width_is_non_convertible()
        {
            //arrange  
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions("23", "Ethan Is Still Cool", mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEWIDTH"], 900);
        }

        [TestMethod]
        public void should_set_width_to_input_param()
        {
            //arrange  
            var mockState = MockRepository.GenerateStub<IStateContainer>();

            //act
            Login.GetAndSetDimensions("504", "23", mockState);

            //assert
            Assert.AreEqual((int)mockState["PAGEWIDTH"], 23);
        }
    }
}
