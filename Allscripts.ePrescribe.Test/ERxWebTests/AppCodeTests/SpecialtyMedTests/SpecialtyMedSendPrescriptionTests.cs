using System;
using System.Data;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.ePrescribeSvc;
using Allscripts.Impact.Interfaces;
using eRxWeb;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.State;
using Rhino.Mocks;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class SpecialtyMedSendPrescriptionTests
    {
        [TestMethod]
        public void should_return_success_when_valid_values_send_to_pharmacy()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.SiteId)).Return("1");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.STANDING)).Return("1");
            string token = GetShieldToken();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken)).Return(token);

            var ScriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            var PrescriptionMock = MockRepository.GenerateMock<IPrescription>();
            var rxResponse = new eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse();
            rxResponse.Success = false;

            var epsBrokerMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(eRxWeb.ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_SERVER_10)).Return(ConnectionStringPointer.ERXDB_SERVER_10);

            string rxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            string rxStatus = "";
            int refills = 1;
            int daysSupply = 1;
            string pharmacy = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            bool isLimitedOrMailOrderPharmacy = false;
            string ipAddress = "10.100.50.38";


            SpecialtyMedTaskProcessResult expectedspecialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult
            {
                RedirectString = string.Empty,
                MessageText = "Prescription sent to pharmacy",
                MessageVisible = true,
                MessageType = (int)Controls_Message.MessageType.SUCCESS
            };

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            SpecialtyMedTaskProcessResult SpecialtyMedTaskProcessResultActual = specialtyMed.SendPrescription(rxID, rxStatus, refills, daysSupply, pharmacy, isLimitedOrMailOrderPharmacy, pageStateMock, ipAddress,
                ScriptMessageMock, PrescriptionMock, epsBrokerMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.RedirectString, SpecialtyMedTaskProcessResultActual.RedirectString);
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.MessageText, SpecialtyMedTaskProcessResultActual.MessageText);
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.MessageType, SpecialtyMedTaskProcessResultActual.MessageType);
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.MessageVisible,
                SpecialtyMedTaskProcessResultActual.MessageVisible);
        }

        [TestMethod]
        public void should_return_failure_message_when_invalid_values_send_to_pharmacy()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("invalid");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("invalid");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.SiteId)).Return("1");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.STANDING)).Return("1");
            string token = GetShieldToken();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken)).Return(token);

            var ScriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            var PrescriptionMock = MockRepository.GenerateMock<IPrescription>();
            var rxResponse = new eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse();
            rxResponse.Success = false;

            var epsBrokerMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_SERVER_10)).Return(ConnectionStringPointer.ERXDB_SERVER_10);

            string rxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            string rxStatus = "";
            int refills = 1;
            int daysSupply = 1;
            string pharmacy = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            bool isLimitedOrMailOrderPharmacy = false;
            string ipAddress = "10.100.50.38";


            SpecialtyMedTaskProcessResult expectedspecialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult
            {
                RedirectString = string.Empty,
                MessageText = "Error Occurred",
                MessageVisible = true,
                MessageType = (int)Controls_Message.MessageType.SUCCESS
            };

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            SpecialtyMedTaskProcessResult SpecialtyMedTaskProcessResultActual = specialtyMed.SendPrescription(rxID, rxStatus, refills, daysSupply, pharmacy, isLimitedOrMailOrderPharmacy, pageStateMock, ipAddress,
                ScriptMessageMock, PrescriptionMock, epsBrokerMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.RedirectString, SpecialtyMedTaskProcessResultActual.RedirectString);
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.MessageText, SpecialtyMedTaskProcessResultActual.MessageText);
            Assert.AreEqual(expectedspecialtyMedTaskProcessResult.MessageVisible,
                SpecialtyMedTaskProcessResultActual.MessageVisible);
        }

        [TestMethod]
        public void should_call_SendThisMessage_when_serviceTaskID_valid()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.SiteId)).Return("1");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.STANDING)).Return("1");
            string token = GetShieldToken();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken)).Return(token);
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT)).Return(ConnectionStringPointer.ERXDB_DEFAULT);

            var ScriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            ScriptMessageMock.Stub(x => x.CreateScriptMessage(null, 1, Constants.MessageTypes.NEWRX, null, null, null, 1, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("anyString");

            var PrescriptionMock = MockRepository.GenerateMock<IPrescription>();
            var rxResponse = new eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse();

            var epsBrokerMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(eRxWeb.ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            string rxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            string rxStatus = "";
            int refills = 1;
            int daysSupply = 1;
            string pharmacy = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            bool isLimitedOrMailOrderPharmacy = false;
            string ipAddress = "10.100.50.38";

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            specialtyMed.SendPrescription(rxID, rxStatus, refills, daysSupply, pharmacy, isLimitedOrMailOrderPharmacy, pageStateMock, ipAddress,
                ScriptMessageMock, PrescriptionMock, epsBrokerMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            ScriptMessageMock.AssertWasCalled(x => x.SendThisMessage(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));

        }

        [TestMethod]
        public void should_not_call_SendThisMessage_when_serviceTaskID_invalid()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.SiteId)).Return("1");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.STANDING)).Return("1");
            string token = GetShieldToken();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken)).Return(token);
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT)).Return(ConnectionStringPointer.ERXDB_DEFAULT);

            var ScriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            ScriptMessageMock.Stub(x => x.CreateScriptMessage(null, 1, Constants.MessageTypes.NEWRX, null, null, null, 1, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(String.Empty);

            var PrescriptionMock = MockRepository.GenerateMock<IPrescription>();
            var rxResponse = new eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse();

            var epsBrokerMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(eRxWeb.ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            string rxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            string rxStatus = "";
            int refills = 1;
            int daysSupply = 1;
            string pharmacy = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            bool isLimitedOrMailOrderPharmacy = false;
            string ipAddress = "10.100.50.38";

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            specialtyMed.SendPrescription(rxID, rxStatus, refills, daysSupply, pharmacy, isLimitedOrMailOrderPharmacy, pageStateMock, ipAddress,
                ScriptMessageMock, PrescriptionMock, epsBrokerMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            ScriptMessageMock.AssertWasNotCalled(x => x.SendThisMessage(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_not_call_SendOutboundInfoScriptMessage_when_ScriptMessage_invalid()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.SiteId)).Return("1");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.STANDING)).Return("1");
            string token = GetShieldToken();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken)).Return(token);
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT)).Return(ConnectionStringPointer.ERXDB_DEFAULT);

            var ScriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            ScriptMessageMock.Stub(x => x.CreateScriptMessage(null, 1, Constants.MessageTypes.NEWRX, null, null, null, 1, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(String.Empty);
            ScriptMessageMock.Stub(x => x.CreateDUREVTScriptMessage(null, 1, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(String.Empty);
            

            var PrescriptionMock = MockRepository.GenerateMock<IPrescription>();
            var rxResponse = new eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse();

            var epsBrokerMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(eRxWeb.ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            string rxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            string rxStatus = "";
            int refills = 1;
            int daysSupply = 1;
            string pharmacy = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            bool isLimitedOrMailOrderPharmacy = false;
            string ipAddress = "10.100.50.38";

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            specialtyMed.SendPrescription(rxID, rxStatus, refills, daysSupply, pharmacy, isLimitedOrMailOrderPharmacy, pageStateMock, ipAddress,
                ScriptMessageMock, PrescriptionMock, epsBrokerMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            ScriptMessageMock.AssertWasNotCalled(x => x.SendOutboundInfoScriptMessage(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_call_SendOutboundInfoScriptMessage_when_ScriptMessage_valid()
        {
            //arrange
            var pageStateMock = MockRepository.GenerateMock<IStateContainer>();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.PatientId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.LicenseId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.UserId)).Return("16516E56-8287-4A83-AE28-2A44D2793F6A");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.SiteId)).Return("1");
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.STANDING)).Return("1");
            string token = GetShieldToken();
            pageStateMock.Stub(x => x.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken)).Return(token);
            pageStateMock.Stub(x => x.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT)).Return(ConnectionStringPointer.ERXDB_DEFAULT);

            var ScriptMessageMock = MockRepository.GenerateMock<IScriptMessage>();
            ScriptMessageMock.Stub(x => x.CreateScriptMessage(null, 1, Constants.MessageTypes.NEWRX, null, null, null, 1, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("anyString");
            ScriptMessageMock.Stub(x => x.CreateDUREVTScriptMessage(null, 1, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("anyString");


            var PrescriptionMock = MockRepository.GenerateMock<IPrescription>();
            var rxResponse = new eRxWeb.ePrescribeSvc.AuditLogPatientRxResponse();

            var epsBrokerMock = MockRepository.GenerateMock<eRxWeb.AppCode.Interfaces.IEPSBroker>();
            epsBrokerMock.Stub(x => x.AuditLogPatientRxInsert(eRxWeb.ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, null, null, null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(rxResponse);

            string rxID = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            string rxStatus = "";
            int refills = 1;
            int daysSupply = 1;
            string pharmacy = "16516E56-8287-4A83-AE28-2A44D2793F6A";
            bool isLimitedOrMailOrderPharmacy = false;
            string ipAddress = "10.100.50.38";

            //act
            SpecialtyMed specialtyMed = new SpecialtyMed();
            specialtyMed.SendPrescription(rxID, rxStatus, refills, daysSupply, pharmacy, isLimitedOrMailOrderPharmacy, pageStateMock, ipAddress,
                ScriptMessageMock, PrescriptionMock, epsBrokerMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            ScriptMessageMock.AssertWasCalled(x => x.SendOutboundInfoScriptMessage(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        private string GetShieldToken()
        {
            return
                "<saml:Assertion MajorVersion=\"1\" MinorVersion=\"1\" AssertionID=\"_7b99507a-15c4-46ea-8c97-22ae20ecab2f\" Issuer=\"https://rp.allscripts.com/shield/shieldauthority\" IssueInstant=\"2016-11-09T21:53:53.434Z\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\"><saml:Conditions NotBefore=\"2016-11-09T21:53:53.434Z\" NotOnOrAfter=\"2016-11-09T22:13:53.434Z\"><saml:AudienceRestrictionCondition><saml:Audience>http://rp.allscripts.com/ePrescribe/MainApp</saml:Audience></saml:AudienceRestrictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:holder-of-key</saml:ConfirmationMethod><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><trust:BinarySecret xmlns:trust=\"http://docs.oasis-open.org/ws-sx/ws-trust/200512\">AjNlRa50FA1S2U5m4Y0p3DjyDqer3XQ32w6DPygzoeU=</trust:BinarySecret></KeyInfo></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=\"name\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>CSprovider2</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"surname\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>Smullen</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"givenname\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>Michael</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"pwdexpiretime\" AttributeNamespace=\"http://schemas.allscripts.com/shield\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>131243993417882058</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"changepwduri\" AttributeNamespace=\"http://schemas.allscripts.com/shield\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>https://authenticate.cloudsrt.local/adfs/ls/ChangePassword.aspx</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"middlename\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue /></saml:Attribute><saml:Attribute AttributeName=\"emailaddress\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" a:OriginalIssuer=\"http://authenticate.cloudsrt.local/adfs/services/trust\" xmlns:a=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:AttributeValue>judy.keadle@allscripts.com</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tokenreferencelist\" AttributeNamespace=\"http://schema.allscripts.com/security/sts\"><saml:AttributeValue>77u/PEFycmF5T2ZUb2tlblJlZmVyZW5jZSB4bWxuczppPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYS1pbnN0YW5jZSIgeG1sbnM9Imh0dHA6Ly9zY2hlbWFzLmRhdGFjb250cmFjdC5vcmcvMjAwNC8wNy9NZHJ4LlNoaWVsZC5Db21tb24uU2VjdXJpdHkiPjxUb2tlblJlZmVyZW5jZT48QXV0aGVudGljYXRpb25JRD5fNzI1MjhlMDEtZDBmYS00NDEyLWI3ZTAtNjk5NmQ5ZGM4NzM5PC9BdXRoZW50aWNhdGlvbklEPjxBdXRoZW50aWNhdGlvblR5cGU+UGFzc3dvcmQ8L0F1dGhlbnRpY2F0aW9uVHlwZT48SXNzdWVyVXJpPmh0dHA6Ly9hdXRoZW50aWNhdGUuY2xvdWRzcnQubG9jYWwvYWRmcy9zZXJ2aWNlcy90cnVzdDwvSXNzdWVyVXJpPjwvVG9rZW5SZWZlcmVuY2U+PC9BcnJheU9mVG9rZW5SZWZlcmVuY2U+</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"role\" AttributeNamespace=\"http://schemas.microsoft.com/ws/2008/06/identity/claims\"><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Roles/Admin</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Roles/Provider</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/shield/TenantAdmin</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"permission\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ViewSettings</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ProcessTasks</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/EditPatients</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ManageDeluxeBilling</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/AddPatients</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/ePA</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/ePrescribe/MainApp/Permissions/Prescribe</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canenroll</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canapprove</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canprescribe</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canviewproviderreports</saml:AttributeValue><saml:AttributeValue>http://schema.allscripts.com/security/epcs/canviewsecurityreports</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"identityId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>MSmullen</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"applicationInstanceId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634A</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"environmentId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634I</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"environmentName\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>Judysite</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tenantId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tenantName\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>Judysite</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"profileId\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>b426ab57-be4a-4571-b0ca-35cd6c111944</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"dearegistrantstatus\" AttributeNamespace=\"http://schema.allscripts.com/security/epcs\"><saml:AttributeValue>Valid</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"level\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/idproofing\"><saml:AttributeValue>3</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"CompromisedIdentity\" AttributeNamespace=\"http://schema.allscripts.com/shield\"><saml:AttributeValue>False</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"tenantidproofingmodel\" AttributeNamespace=\"http://schema.allscripts.com/security/epcs\"><saml:AttributeValue>Individual</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"status\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/idproofing\"><saml:AttributeValue>Passed</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"application\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>http://rp.allscripts.com/ePrescribe/MainApp</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"scopetype\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ApplicationInstance</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"scopefilter\" AttributeNamespace=\"http://schemas.allscripts.com/shield\"><saml:AttributeValue>ERX-2451634A</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"name\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account\"><saml:AttributeValue>CSprovider2</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"objectid\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account\"><saml:AttributeValue>9c58869f-1ac9-40ad-8347-c160f6332d6b</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"primarysid\" AttributeNamespace=\"http://schemas.microsoft.com/ws/2008/06/identity/claims\"><saml:AttributeValue>S-1-5-21-1795162713-183561324-162468270-1885</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"typeofuser\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\"><saml:AttributeValue>StandardUser</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"first\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account/name\"><saml:AttributeValue>Michael</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"middle\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account/name\"><saml:AttributeValue /></saml:Attribute><saml:Attribute AttributeName=\"last\" AttributeNamespace=\"http://schema.allscripts.com/security/shield/account/name\"><saml:AttributeValue>Smullen</saml:AttributeValue></saml:Attribute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMethod=\"urn:oasis:names:tc:SAML:1.0:am:password\" AuthenticationInstant=\"2016-11-09T21:53:51.786Z\"><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:holder-of-key</saml:ConfirmationMethod><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><trust:BinarySecret xmlns:trust=\"http://docs.oasis-open.org/ws-sx/ws-trust/200512\">AjNlRa50FA1S2U5m4Y0p3DjyDqer3XQ32w6DPygzoeU=</trust:BinarySecret></KeyInfo></saml:SubjectConfirmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /><ds:SignatureMethod Algorithm=\"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256\" /><ds:Reference URI=\"#_7b99507a-15c4-46ea-8c97-22ae20ecab2f\"><ds:Transforms><ds:Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><ds:Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /></ds:Transforms><ds:DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha256\" /><ds:DigestValue>RwHl5EyvEL4UkQuPe3iKEkf02Es2SvRSrxYmkEPChg4=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>at0maA0kob5Z5ZRyfp1OAUei5f8oDwoJG0mFLZikpPiaRam8HVOXZCGlLbUshPFYiA/SXO+q/QuriBmjudwhuyxPr7hcA0SFaK/4yCH2HbZlX/yYaTJTQWrjdBR3V6y8bxjjVZgGDuJmSxmLnTSGOkraJpJBegNmJRtu0nzbYazkW8OagIp0mET4nBTB6gi89qr1vbx3heM9A6hTpfLFGUim+HISSy690FxsaRuLEdDmKIjuQ+zZ644DdJkemTid1VC5b9QjiTY0msVgXcxlY7Vokn193e/brzGHVVq0wECmqCLy37GpO/ifBLm6sFihZ0ORLFn8PCJBHycxZXmc1w==</ds:SignatureValue><KeyInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><X509Data><X509Certificate>MIIEqDCCA5CgAwIBAgITVQAAACdsq/t1I+jABAAAAAAAJzANBgkqhkiG9w0BAQowADBQMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxGDAWBgoJkiaJk/IsZAEZFghjbG91ZHNydDEdMBsGA1UEAxMUSXNzdWluZ0NBLUNsb3VkU1JUQ0EwHhcNMTUwNDMwMTg0NTAzWhcNMjAwNDMwMTg1NTAzWjBNMRUwEwYKCZImiZPyLGQBGRYFbG9jYWwxGDAWBgoJkiaJk/IsZAEZFghjbG91ZHNydDEaMBgGA1UEAxMRc2FzLXRva2VuLXNpZ25pbmcwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDEC4wmZ864hKO0RSE+jPDZDc+7Wv9t1tgUE4bkMP0yYkXdsairfVeamR5tgT6TeHwRVmmupYCi5tADNDS0qQm+GIDnKDMqOvi8eBiyWxUmw+fUUbpauG8UTyroj7KcGsHgG3vzsRDyeIRfwGIr+bc+jGnP4uvjgp+agBmKy4xhZ2jJbst1HDspR2A3OtqkLsNsmFCyYSA/q4ktw7HMxhKd6Xfef2j8Ca80ll17F6Vw0MgFvuETPOkYhs3Bd2E1Cz2FCZLN2cvK1g49lx6aSBkOmXGHOa3+PaqquK/5fS22Hixf2YvQkyAQS8u1wsW7zCneL2SBso+R6NO3uzYKrd0vAgMBAAGjggF8MIIBeDA9BgkrBgEEAYI3FQcEMDAuBiYrBgEEAYI3FQiF34BR/L4ng4mTNoL91HaCtYc8gQuExq0Bg5SqEQIBZAIBDDATBgNVHSUEDDAKBggrBgEFBQcDATAOBgNVHQ8BAf8EBAMCBaAwGwYJKwYBBAGCNxUKBA4wDDAKBggrBgEFBQcDATAdBgNVHQ4EFgQU55viaV0VglsKVo1h/v9tSuy1+sIwHwYDVR0jBBgwFoAUJbi7SO3ov31K5bR7tgB/CUNHiOUwRwYDVR0fBEAwPjA8oDqgOIY2aHR0cDovL3d3dy5jbG91ZHNydC5sb2NhbC9wa2kvSXNzdWluZ0NBLUNsb3VkU1JUQ0EuY3JsMGwGCCsGAQUFBwEBBGAwXjBcBggrBgEFBQcwAoZQaHR0cDovL3d3dy5jbG91ZHNydC5sb2NhbC9wa2kvQ0xPVURTUlRDQS5jbG91ZHNydC5sb2NhbF9Jc3N1aW5nQ0EtQ2xvdWRTUlRDQS5jcnQwDQYJKoZIhvcNAQEKMAADggEBAFwEY/J0FNEZfZQT2FXnFC7oSCDlOMxGmSKK0sU54MR2KzZFfXrItVLpNyamEc9RkzmLqzcntpORVRv4u6S8PNzeEyioeP2tV5cLd0d6Vpqs0oACCJaB0Uq/E6Im5u7SG+5mWxNo9PcFjs7YJRRAye7Oa2UlAXaUB1WqoMW26vmIvYvTQDKrYoqGCxLjF2+oZaqy3XG91+0EnrJm1nMdi2U/UDwdMmsOeJy3xb3NVykUDcwY1tLNe59lufcqjsvZyXVO7yKZv3iBFxk9gJ98wp17xq8bXQa5tm+x35Iy1uEgZHZH0u0ZIHqojZoaSvWkR7uzwNXcKjEJ/u8GY2XtV5w=</X509Certificate></X509Data></KeyInfo></ds:Signature></saml:Assertion>";
        }
    }
}
