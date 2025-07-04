using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb;
using eRxWeb.Controls;
using eRxWeb.Controls.Interfaces;
using eRxWeb.PageInterfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Telerik.Web.UI;

namespace Allscripts.ePrescribe.Test.ScriptPadTests
{
    [TestClass]
    public class CheckForSecondaryWorkflowTests
    {
        [TestMethod]
        public void should_not_show_epcs_digital_signing_overlay()
        {
            //arrange
            GridDataItemCollection scriptPadItems = new GridDataItemCollection();
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);
            mockScriptPad.Stub(x => x.IsSendingMedToMailOrder(scriptPadItems)).Return(false);
            

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Stub(x => x.ShouldShowEpcsSignAndSendScreen()).Return(true);

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("");
            mockPageState.Stub(x => x.GetInt("PATIENT_YEARS_OLD",0)).Return(21);
            mockPageState.Stub(x => x.Cast("PATIENTHEIGHT", new Height())).Return(new Height("185"));
            mockPageState.Stub(x => x.Cast("PATIENTWEIGHT", new Weight())).Return(new Weight("180"));
            
                


            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 1, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            var result = ScriptPad.ShouldShowPatMissingInfo(mockPageState);
            Assert.IsTrue(result);

            //assert
            mockScriptPad.AssertWasNotCalled(x => x.saveScripts());
        }

        [TestMethod]
        public void should_show_epcs_digital_signing_overlay()
        {
            //arrange
            GridDataItemCollection scriptPadItems = new GridDataItemCollection();
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Stub(x => x.ShouldShowEpcsSignAndSendScreen()).Return(true);

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("123");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("321");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTSTATE")).Return("321");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTZIP")).Return("321");
            mockPageState.Stub(x => x.GetInt("PATIENT_YEARS_OLD", 0)).Return(21);
            mockPageState.Stub(x => x.Cast("PATIENTHEIGHT", new Height())).Return(new Height("185"));
            mockPageState.Stub(x => x.Cast("PATIENTWEIGHT", new Weight())).Return(new Weight("180"));




            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 1, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockEpcsDigitalSigning.AssertWasCalled(x => x.ShouldShowEpcsSignAndSendScreen());
            mockScriptPad.AssertWasNotCalled(x => x.saveScripts());
        }

        [TestMethod]
        public void should_not_show_epcs_overlay_if_epcs_med_count_is_0()
        {
            //arrange
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Expect(x => x.ShouldShowEpcsSignAndSendScreen());

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("");


            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockEpcsDigitalSigning.AssertWasNotCalled(x => x.ShouldShowEpcsSignAndSendScreen());
        }

        [TestMethod]
        public void should_not_show_epcs_overlay_if_epcs_med_list_is_greater_than_0_but_epcsDigitalSigning_is_false()
        {
            //arrange
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(false);

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Expect(x => x.ShouldShowEpcsSignAndSendScreen());

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("");


            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 1, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockEpcsDigitalSigning.AssertWasNotCalled(x => x.ShouldShowEpcsSignAndSendScreen());
        }

        [TestMethod]
        public void should_show_patient_demographics_overlay()
        {
            //arrange
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);
            mockScriptPad.Stub(x => x.IsSendingMedToMailOrder(new GridDataItemCollection())).Return(true).IgnoreArguments();

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Expect(x => x.ShouldShowEpcsSignAndSendScreen());

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("");


            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockPatControl.AssertWasCalled(x => x.Show());
        }

        [TestMethod]
        public void should_not_show_patient_demographics_overlay_if_sending_to_mail_order_and_both_address1_and_city_are_present()
        {
            //arrange
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);
            mockScriptPad.Stub(x => x.IsSendingMedToMailOrder(new GridDataItemCollection())).Return(true);

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Expect(x => x.ShouldShowEpcsSignAndSendScreen());

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("testME");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("Neverland");
            mockPageState.Stub(x => x.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(20);
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Adress1");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockPatControl.AssertWasNotCalled(x => x.Show());
        }

        [TestMethod]
        public void should_show_patient_demographics_overlay_if_sending_to_mail_order_and_either_is_missing()
        {
            //arrange
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);
            mockScriptPad.Stub(x => x.IsSendingMedToMailOrder(new GridDataItemCollection())).Return(true).IgnoreArguments();

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Expect(x => x.ShouldShowEpcsSignAndSendScreen());

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("BlahBlah");


            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockPatControl.AssertWasCalled(x => x.Show());
        }

        [TestMethod]
        public void should_show_patient_demographics_overlay_if_sending_to_mail_order_and_city_is_missing()
        {
            //arrange
            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.isEPCSDigitalSigningRequired()).Return(true);
            mockScriptPad.Stub(x => x.IsSendingMedToMailOrder(new GridDataItemCollection())).Return(true).IgnoreArguments();

            var mockEpcsDigitalSigning = MockRepository.GenerateMock<IControls_EPCSDigitalSigning>();
            mockEpcsDigitalSigning.Expect(x => x.ShouldShowEpcsSignAndSendScreen());

            var mockPatControl = MockRepository.GenerateMock<IControlsPatMissingInfo>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTADDRESS1")).Return("DoobbaDee");
            mockPageState.Stub(x => x.GetStringOrEmpty("PATIENTCITY")).Return("");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Adress1");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");

            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, mockEpcsDigitalSigning, mockPatControl, mockPageState, null, null);


            //assert
            mockPatControl.AssertWasCalled(x => x.Show());
        }

        [TestMethod]
        public void should_show_specialty_med_welcome_message_if__provider_new_to_spec_med()
        {
            //arrange

            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.IsSendingSpecMedsTaskList(new GridDataItemCollection())).Return(true).IgnoreArguments();

            var mockSpecialtyMedsUserWelcome = MockRepository.GenerateMock<IControls_SpecialtyMedsUserWelcome>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetBooleanOrFalse("ShouldShowSpecMedsWelcomeMsg")).Return(true);
            mockPageState.Stub(x => x.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(20);
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Adress1");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, null, null, mockPageState, null, mockSpecialtyMedsUserWelcome);


            //assert
            mockSpecialtyMedsUserWelcome.AssertWasCalled(x => x.Show());
        }

        [TestMethod]
        public void should_not_show_specialty_med_welcome_message_if__provider_not_new_to_spec_med()
        {
            //arrange

            var mockScriptPad = MockRepository.GenerateMock<IScriptPad>();
            mockScriptPad.Stub(x => x.IsSendingSpecMedsTaskList(new GridDataItemCollection())).Return(true).IgnoreArguments();

            var mockSpecialtyMedsUserWelcome = MockRepository.GenerateMock<IControls_SpecialtyMedsUserWelcome>();

            var mockPageState = MockRepository.GenerateMock<IStateContainer>();
            mockPageState.Stub(x => x.GetBooleanOrFalse("ShouldShowSpecMedsWelcomeMsg")).Return(false);
            mockPageState.Stub(x => x.GetInt(Constants.SessionVariables.PatientYearsOld, 0)).Return(20);
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientAddress1)).Return("Adress1");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientCity)).Return("City");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientZip)).Return("88011");
            mockPageState.Stub(_ => _.GetStringOrEmpty(Constants.SessionVariables.PatientState)).Return("NM");
            //act
            new ScriptPad().CheckForSecondaryWorkflow(mockScriptPad, 0, null, null, mockPageState, null, mockSpecialtyMedsUserWelcome);


            //assert
            mockSpecialtyMedsUserWelcome.AssertWasNotCalled(x => x.Show());
        }

    }
}