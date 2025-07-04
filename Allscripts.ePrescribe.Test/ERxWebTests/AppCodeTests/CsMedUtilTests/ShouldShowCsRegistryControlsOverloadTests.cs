using System;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.CsMedUtilTests
{
    [TestClass]
    public class ShouldShowCsRegistryControlsOverloadTests
    {
        [TestMethod]
        public void should_return_false_if_hasCsMeds_is_false()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(true);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("Url");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(false, true, pageStateMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_hasCsMeds_is_true_and_ISCSREGISTRYCHECKREQ_is_false_and_STATEREGISTRYURL_is_empty_and_showCsRegistryEnterpriseSetting_is_true()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(false);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(true, true, pageStateMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_hasCsMeds_is_true_and_ISCSREGISTRYCHECKREQ_is_false_and_STATEREGISTRYURL_is_not_empty_and_showCsRegistryEnterpriseSetting_is_false()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(false);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("Url");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(true, false, pageStateMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_hasCsMeds_is_true_and_ISCSREGISTRYCHECKREQ_is_true_and_STATEREGISTRYURL_is_empty_and_showCsRegistryEnterpriseSetting_is_false()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(true);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(true, false, pageStateMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_if_hasCsMeds_is_true_and_ISCSREGISTRYCHECKREQ_is_false_and_STATEREGISTRYURL_is_not_empty_and_showCsRegistryEnterpriseSetting_is_true()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(false);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("Url");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(true, true, pageStateMock);

            //assert
            Assert.IsTrue(result);
        }
    }
}
