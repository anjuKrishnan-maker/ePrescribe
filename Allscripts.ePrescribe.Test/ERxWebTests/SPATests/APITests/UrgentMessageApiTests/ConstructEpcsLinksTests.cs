using System;
using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact.Interfaces;
using eRxWeb.Controllers;
using eRxWeb.ePrescribeSvc;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Expectations;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;

namespace Allscripts.ePrescribe.Test.ERxWebTests.SPATests.APITests.UrgentMessageApiTests
{
    [TestClass]
    public class ConstructEpcsLinksTests
    {
        [TestMethod]
        public void should_handle_compromised_identity()
        {
            // arrange

            var model = new UrgentMessageModel();
            model.IsEpcsCanEnrollVisible = true;
            model.IsEpcsApproverVisible = true;

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo()
                    {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.YES}
            };

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsFalse(model.IsEpcsApproverVisible);
        }

        /*
         
            IDProofingModel CanEnroll CanPrescribe CanApprove ShowRegistrationLink 
            Individual      N         N            N          N
            Individual      Y         N            N          Y 
            Individual      Y         Y            N          N 
            Individual      Y         N            Y          N 

            Institutional   N         N            N          N           
            Institutional   Y         N            N          Y 
            Institutional   Y         Y            N          N 
            Institutional   Y         N            Y          Y 
       
         */

        [TestMethod]
        public void should_not_show_reg_link_for_individual_with_no_cans()
        {
            //Individual      N         N            N          N
            // arrange

            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Individual)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.NO}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsFalse(model.IsEpcsApproverVisible );

        }

       [TestMethod]
        public void should_show_reg_link_for_individual_with_canEnroll()
        {
            //Individual      Y         N            N          N
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Individual)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsTrue(model.IsEpcsCanEnrollVisible);
            Assert.AreEqual("Start/Resume EPCS Registration", model.EpcsCanEnrollText);
            Assert.IsFalse(model.IsEpcsApproverVisible );

        }

        [TestMethod]
        public void should_not_show_reg_link_for_individual_with_canEnroll_canPrescribe()
        {
            //Individual      Y         N            N          N
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Individual)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.YES}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);
            

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsFalse(model.IsEpcsApproverVisible );

        }

        [TestMethod]
        public void should_not_show_reg_link_for_individual_with_canEnroll_canApprove()
        {
            //Individual      Y         N            Y          N
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Individual)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.NO}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsTrue(model.IsEpcsApproverVisible );

        }


        [TestMethod]
        public void should_not_show_reg_link_for_institutional_with_no_cans()
        {

            //Institutional      N         N            N          N
            // arrange

            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Institutional)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.NO}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsFalse(model.IsEpcsApproverVisible );

        }
        [TestMethod]
        public void should_show_reg_link_for_institutional_with_canEnroll()
        {
            //Institutional      Y         N            N          N
            // arrange

            var model = new UrgentMessageModel();
            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Institutional)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsTrue(model.IsEpcsCanEnrollVisible);
            Assert.AreEqual("Start/Resume EPCS Registration", model.EpcsCanEnrollText);
            Assert.IsFalse(model.IsEpcsApproverVisible );
        }


        [TestMethod]
        public void should_not_show_reg_link_for_institutional_with_canEnroll_canPrescribe()
        {
            //Institutional      Y         N            N          N
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Institutional)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.YES}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsFalse(model.IsEpcsApproverVisible );
        }
        
        [TestMethod]
        public void should_show_reg_link_for_institutional_with_canEnroll_canApprove()
        {
            //Institutional      Y         N            Y         Y 
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Institutional)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.NO}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsTrue(model.IsEpcsCanEnrollVisible);
            Assert.AreEqual("Start/Resume EPCS Registration", model.EpcsCanEnrollText);
            Assert.IsTrue(model.IsEpcsApproverVisible );
        }

        [TestMethod]
        public void should_show_get_reg_link_when_visible()
        {
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Institutional)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsMock.Stub(m => m.GetIdProofingUrl(ShieldTenantIDProofingModel.Institutional, "blah")).IgnoreArguments() .Return("https://shield.com");

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanEnroll, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanApprove, TraitValueEnum = ShieldTraitValue.YES},
                new ShieldTraitInfo() {TraitName = ShieldTraitName.CanPrescribe, TraitValueEnum = ShieldTraitValue.NO}
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);

            // assert

            Assert.IsTrue(model.IsEpcsCanEnrollVisible);
            Assert.AreEqual("https://shield.com", model.EpcsCanEnrollUrl);
        }


        [TestMethod]
        public void should_not_get_reg_link_url_when_not_visible()
        {
            // arrange


            var model = new UrgentMessageModel();

            var sessionMock = MockRepository.GenerateStub<IStateContainer>();
            sessionMock.Stub(_ => _.EqualsToEnum(Constants.SessionVariables.TenantIdProofingModel, ShieldTenantIDProofingModel.Individual)).Return(true);

            var epsMock = MockRepository.GenerateStub<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsMock.Stub(m => m.GetIdProofingUrl(ShieldTenantIDProofingModel.Institutional, "blah")).IgnoreArguments() .Return("https://shouldnotbeshield.com");

            var traits = new List<ShieldTraitInfo>()
            {
                new ShieldTraitInfo() {TraitName = ShieldTraitName.IsIdentityCompromised, TraitValueEnum = ShieldTraitValue.NO},
            };

            // act

            UrgentMessageApiController.ConstructEpcsLinks(sessionMock, model, traits, epsMock);
            

            // assert

            Assert.IsFalse(model.IsEpcsCanEnrollVisible);
            Assert.IsNull(model.EpcsCanEnrollUrl);

        }

    }
}
