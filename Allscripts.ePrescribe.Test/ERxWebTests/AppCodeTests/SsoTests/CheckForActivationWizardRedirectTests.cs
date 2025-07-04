using System;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Data.Interfaces;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using Partner = eRxWeb.Partner;
using ShieldUserStatus = Allscripts.ePrescribe.Objects.ShieldUserStatus;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class CheckForActivationWizardRedirectTests
    {
        [TestMethod]
        public void should_return_null_if_partner_doesnt_allow_shield_enrollment()
        {
            //arrange
            var partner = new Partner
            {
                AllowShieldEnrollment = false,
                AllowsUserNameAndPassword = true
            };
            var actWizard = MockRepository.GenerateMock<IActivationWizard>();
            var stateContainer = MockRepository.GenerateMock<IStateContainer>();

            //act
            var result = Sso.CheckForActivationWizardRedirect(partner, Guid.Empty, stateContainer, null, actWizard);

            //assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void should_return_null_if_partner_doesnt_allow_username_and_password()
        {
            //arrange
            var partner = new Partner
            {
                AllowShieldEnrollment = true,
                AllowsUserNameAndPassword = false
            };
            var actWizard = MockRepository.GenerateMock<IActivationWizard>();
            var stateContainer = MockRepository.GenerateMock<IStateContainer>();

            //act
            var result = Sso.CheckForActivationWizardRedirect(partner, Guid.Empty, stateContainer, null, actWizard);

            //assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void should_return_null_if_auth_user_shield_status_is_not_pending_activation()
        {
            //arrange
            var partner = new Partner
            {
                AllowShieldEnrollment = true,
                AllowsUserNameAndPassword = true
            };
            var actWizard = MockRepository.GenerateMock<IActivationWizard>();
            var stateContainer = MockRepository.GenerateMock<IStateContainer>();
            actWizard.Stub(_ => _.LoadCheckInfo(new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments()
                .Return(new UserActivationCheckInfo
                {
                    Status = ShieldUserStatus.ACTIVATION_COMPLETE
                });

            //act
            var result = Sso.CheckForActivationWizardRedirect(partner, Guid.Empty, stateContainer, null, actWizard);

            //assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void should_return_redirect_string_and_set_activation_code_if_allowshieldenrollment_allowusernameandpassword_and_pendingactivation_are_true()
        {
            //arrange
            var partner = new Partner
            {
                AllowShieldEnrollment = true,
                AllowsUserNameAndPassword = true
            };
            var actWizard = MockRepository.GenerateMock<IActivationWizard>();
            actWizard.Stub(_ => _.LoadCheckInfo(new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments()
                .Return(new UserActivationCheckInfo
                {
                    Status = ShieldUserStatus.PENDING_ACTIVATION
                });

            var activationCode = "1234ABds";
            var epsBrokerMock = MockRepository.GenerateMock<IEPSBroker>();
            epsBrokerMock.Stub(x => x.GetShieldInternalTenantID(null)).IgnoreArguments().Return(123);
            epsBrokerMock.Stub(x => x.GetNewActivationCode(0, null))
                .IgnoreArguments()
                .Return(new GetNewActivationCodeResponse {ActivationCode = activationCode});

            var pageState = MockRepository.GenerateStub<IStateContainer>();

            //act
            var result = Sso.CheckForActivationWizardRedirect(partner, Guid.Empty, pageState, epsBrokerMock, actWizard);

            //assert
            Assert.AreEqual("~/"+Constants.PageNames.ACTIVATE, result);
            Assert.AreEqual(activationCode, pageState["ActivationCodeFromSSO"]);
        }
    }
}
