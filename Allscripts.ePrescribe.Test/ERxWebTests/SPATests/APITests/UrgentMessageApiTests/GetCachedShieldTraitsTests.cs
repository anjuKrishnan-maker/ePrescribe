using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.Controllers;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Permission = eRxWeb.ePrescribeSvc.Permission;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;

namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.UrgentMessageApiTests
{
    [TestClass]
    public class GetCachedShieldTraitsTests
    {
        [TestMethod]
        public void should_load_nothing_if_nothing_is_there()
        {
            // arrange

            var mock = MockRepository.GenerateStub<IStateContainer>();
            
            // act

            var traits = UrgentMessageApiController.GetShieldTraitsFromSession(mock);

            // assert

            Assert.IsNull(traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.CanEnroll));
            Assert.IsNull(traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.CanApprove));
            Assert.IsNull(traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.CanPrescribe));
            Assert.AreEqual(ShieldTraitValue.NO, traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.IsIdentityCompromised)?.TraitValueEnum);
        }

        [TestMethod]
        public void should_load_canEnroll()
        {
            // arrange

            var mock = MockRepository.GenerateStub<IStateContainer>();
            var permissions = new []
            {
                new Permission() {Value = UserPermissions.EpcsCanEnroll}
            };
            mock.Stub(m => m.Cast(Constants.SessionVariables.UserAppPermissions, new Permission[0])).Return(permissions);
            
            // act

            var traits = UrgentMessageApiController.GetShieldTraitsFromSession(mock);

            // assert

            Assert.AreEqual(ShieldTraitValue.YES, traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.CanEnroll)?.TraitValueEnum);
        }

        [TestMethod]
        public void should_load_canApprove()
        {
            // arrange

            var mock = MockRepository.GenerateStub<IStateContainer>();
            var permissions = new []
            {
                new Permission() {Value = UserPermissions.EpcsCanApprove}
            };
            mock.Stub(m => m.Cast(Constants.SessionVariables.UserAppPermissions, new Permission[0])).Return(permissions);
            
            // act

            var traits = UrgentMessageApiController.GetShieldTraitsFromSession(mock);

            // assert

            Assert.AreEqual(ShieldTraitValue.YES, traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.CanApprove)?.TraitValueEnum);
        }

        [TestMethod]
        public void should_load_canPrescribe()
        {
            // arrange

            var mock = MockRepository.GenerateStub<IStateContainer>();
            var permissions = new []
            {
                new Permission() {Value = UserPermissions.EpcsCanPrescribe}
            };
            mock.Stub(m => m.Cast(Constants.SessionVariables.UserAppPermissions, new Permission[0])).Return(permissions);
            
            // act

            var traits = UrgentMessageApiController.GetShieldTraitsFromSession(mock);

            // assert

            Assert.AreEqual(ShieldTraitValue.YES, traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.CanPrescribe)?.TraitValueEnum);
        }

        [TestMethod]
        public void should_load_CompromisedIdentity_if_set_to_true()
        {
            // arrange

            var mock = MockRepository.GenerateStub<IStateContainer>();
            var properties = new []
            {
                new Property() { PropertyName = Constants.UserPropertyNames.COMPROMISED_IDENTITY, PropertyStatus = bool.TrueString}
            };
            mock[Constants.SessionVariables.UserAppProperties] = properties;
            
            // act

            var traits = UrgentMessageApiController.GetShieldTraitsFromSession(mock);

            // assert

            Assert.AreEqual(ShieldTraitValue.YES, traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.IsIdentityCompromised)?.TraitValueEnum);
        }

        [TestMethod]
        public void should_load_CompromisedIdentity_if_set_to_false()
        {
            // arrange

            var mock = MockRepository.GenerateStub<IStateContainer>();
            var properties = new []
            {
                new Property() { PropertyName = Constants.UserPropertyNames.COMPROMISED_IDENTITY, PropertyStatus = bool.FalseString}
            };
            mock[Constants.SessionVariables.UserAppProperties] = properties;
            
            // act

            var traits = UrgentMessageApiController.GetShieldTraitsFromSession(mock);

            // assert

            Assert.AreEqual(ShieldTraitValue.NO, traits.FirstOrDefault(t => t.TraitName == ShieldTraitName.IsIdentityCompromised)?.TraitValueEnum);
        }
    }
}