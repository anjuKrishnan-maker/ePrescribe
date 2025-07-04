using System;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.CsMedUtilTests
{
    [TestClass]
    public class ShouldShowCsRegistryControlsTests
    {
        [TestMethod]
        public void should_return_true_when_ISCSREGISTRYCHECKREQ_is_true()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(true);

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(pageStateMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_when_STATEREGISTRYURL_is_not_empty_and_ShowCSRegistry_is_true_and_ISCSREGISTRYCHECKREQ_is_false()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(false);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("Url");
            pageStateMock.Stub(x => x.GetStringOrEmpty("ShowCSRegistry")).Return("True");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(pageStateMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_true_when_STATEREGISTRYURL_is_not_empty_and_ShowCSRegistry_is_true_and_ISCSREGISTRYCHECKREQ_is_true()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(true);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("Url");
            pageStateMock.Stub(x => x.GetStringOrEmpty("ShowCSRegistry")).Return("True");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(pageStateMock);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_when_STATEREGISTRYURL_is_empty_and_ShowCSRegistry_is_true_and_ISCSREGISTRYCHECKREQ_is_false()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(false);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("");
            pageStateMock.Stub(x => x.GetStringOrEmpty("ShowCSRegistry")).Return("True");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(pageStateMock);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_when_STATEREGISTRYURL_is_empty_and_ShowCSRegistry_is_false_and_ISCSREGISTRYCHECKREQ_is_false()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateStub<IStateContainer>();
            pageStateMock.Stub(x => x.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ")).Return(false);
            pageStateMock.Stub(x => x.GetStringOrEmpty("STATEREGISTRYURL")).Return("Url");
            pageStateMock.Stub(x => x.GetStringOrEmpty("ShowCSRegistry")).Return("False");

            //act
            var result = CsMedUtil.ShouldShowCsRegistryControls(pageStateMock);

            //assert
            Assert.IsFalse(result);
        }
    }
}
