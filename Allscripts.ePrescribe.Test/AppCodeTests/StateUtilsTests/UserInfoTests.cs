using Allscripts.ePrescribe.Common;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.StateUtilsTests
{
    [TestClass]
    public class UserInfoTests
    {
          [TestMethod]
        public void should_return_individual_idproofing_mode_if_set()
        {
            // arrange

           var sessionMock = MockRepository.GenerateStub<IStateContainer>();
           sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Individual)).Return(true);

            // act

            var result = eRxWeb.AppCode.StateUtils.UserInfo.GetIdProofingMode(sessionMock);

            // assert

            Assert.AreEqual(ShieldTenantIDProofingModel.Individual, result);
        }
    
       [TestMethod]
        public void should_return_individual_idproofing_mode_if_it_is_not_set()
        {
            // arrange

           var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            // act

            var result = eRxWeb.AppCode.StateUtils.UserInfo.GetIdProofingMode(sessionMock);

            // assert

            Assert.AreEqual(ShieldTenantIDProofingModel.Individual, result);
        }
    
       [TestMethod]
        public void should_return_institutional_idproofing_mode_if_set()
        {
            // arrange

           var sessionMock = MockRepository.GenerateStub<IStateContainer>();
           sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Institutional)).Return(true);

            // act

            var result = eRxWeb.AppCode.StateUtils.UserInfo.GetIdProofingMode(sessionMock);

            // assert

            Assert.AreEqual(ShieldTenantIDProofingModel.Institutional, result);
        }
    }
}
