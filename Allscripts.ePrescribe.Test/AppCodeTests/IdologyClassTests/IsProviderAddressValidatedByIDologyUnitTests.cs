using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb;
using Rhino.Mocks;
using eRxWeb.AppCode.Interfaces;
using System.Collections.Generic;

namespace Allscripts.ePrescribe.Test.AppCodeTests.IdologyClassTests
{
    [TestClass]
    public class IsProviderAddressValidatedByIDologyUnitTests
    {
        [TestMethod]
        public void should_return_true_if_resultCodeErrorsList_null()
        {
            //Arrange
            IDologyClass IDVerify;
            var appConfigMock = MockRepository.GenerateStub<IAppConfig>();
            appConfigMock.Stub(x => x.GetAppSettings("")).IgnoreArguments().Return("");


            var configKeysMock = MockRepository.GenerateStub<IConfigKeys>();
            configKeysMock.IDologyPwd = "";
            IDVerify = new IDologyClass(appConfigMock, configKeysMock);
            bool bExpected = true;

            //Assert
            bool bActual = IDVerify.IsProviderAddressValidatedByIDology(null, null);//means no address errors and no other miscellaneous errors

            //Act
            Assert.AreEqual(bExpected, bActual);

        }

        [TestMethod]
        public void should_return_false_if_resultCodeErrorsList_contains_a_valid_address_error_code()
        {
            //Arrange
            IDologyClass IDVerify;
            var appConfigMock = MockRepository.GenerateStub<IAppConfig>();
            appConfigMock.Stub(x => x.GetAppSettings("")).IgnoreArguments().Return("");

            List<string> list = new List<string> { "resultcode.address.does.not.match" };


            var configKeysMock = MockRepository.GenerateStub<IConfigKeys>();
            configKeysMock.IDologyPwd = "";
            IDVerify = new IDologyClass(appConfigMock, configKeysMock);
            bool bExpected = false;

            //Assert
            bool bActual = IDVerify.IsProviderAddressValidatedByIDology(list, null);

            //Act
            Assert.AreEqual(bExpected, bActual);
        }

        [TestMethod]
        public void should_return_true_if_resultCodeErrorsList_doesnot_contains_any_valid_address_error_codes()
        {
            //Arrange
            IDologyClass IDVerify;
            var appConfigMock = MockRepository.GenerateStub<IAppConfig>();
            appConfigMock.Stub(x => x.GetAppSettings("")).IgnoreArguments().Return("");

            List<string> list = new List<string> { "resultcode.of.age" };


            var configKeysMock = MockRepository.GenerateStub<IConfigKeys>();
            configKeysMock.IDologyPwd = "";
            IDVerify = new IDologyClass(appConfigMock, configKeysMock);
            bool bExpected = true;

            //Assert
            bool bActual = IDVerify.IsProviderAddressValidatedByIDology(list, null);

            //Act
            Assert.AreEqual(bExpected, bActual);
        }
    }
}
