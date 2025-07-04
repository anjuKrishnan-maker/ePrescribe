using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Web;
using System.Xml;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Data.Interfaces;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using ComponentSpace.SAML;
using ComponentSpace.SAML.Assertions;
using ComponentSpace.SAML.Protocol;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using IEPSBroker = eRxWeb.AppCode.Interfaces.IEPSBroker;
using IPatient = Allscripts.Impact.Interfaces.IPatient;
using Patient = Allscripts.Impact.Patient;

namespace eRxWeb.AppCode
{
    public class Sso : BasePage, ISso
    {
        public const string NOT_BEFORE_MSG = "; NotBefore: ";
        public const string NOT_ON_OR_AFTER_MSG = "; NotOnOrAfter: ";

        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected internal void LoadPartners(HttpApplicationState applicationState, ConnectionStringPointer dbId)
        {
            try
            {
                Partners partners = new Partners();

                if (applicationState["Partners"] != null)
                {
                    applicationState.Remove("Partners");
                }

                applicationState["Partners"] = partners;
            }
            catch (Exception ex)
            {
                try
                {
                    //could not load partners
                    Audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(), "Could not load partners from Sso.aspx.cs " + ex.ToString(), String.Empty, String.Empty, String.Empty, dbId);
                }
                catch (Exception finalex)
                {
                    try
                    {
                        //we've pretty much caught everything we're gonna catch
                        using (
                            var writer = new StreamWriter(
                                AppDomain.CurrentDomain.BaseDirectory + "\\eRxNOWErrorLog.txt", true))
                        {
                            writer.WriteLine(DateTime.Now + " An error occurred while creating an error: " +
                                             finalex.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        // ignored no more options
                    }
                }
            }
        }

        public Partner VerifyPartnerAndSetValues(Partners partners, Assertion assertion, XmlElement xmlElement, IStateContainer pageState, string ipAddress, ConnectionStringPointer dbId)
        {

            logger.Debug("VerifyPartnerAndSetValues- Assertion Details: ssertion.Signature: {0}, assertion.Issue: {1}", assertion.Signature.ToLogString(), assertion.Issuer.ToLogString());
            var partner = FindPartnerByIssuer(partners, assertion.Issuer);

            SetPartnerSession(partner, pageState);
            
            VerifyPartnerCertificate(partner, xmlElement, ipAddress, pageState, new Audit(), new Sso(),  dbId);

            return partner;
        }

        internal static void VerifyPartnerCertificate(Partner partner, XmlElement xmlElement, string ipAddress, IStateContainer pageState, IAudit audit, ISso sso, ConnectionStringPointer dbId)
        {
            // Reconstruct the SAML response message.
            bool verified;
            if (partner.Certificate2 != null)
            {
                // Use the pre-loaded certificate to verify the signature.
                verified = sso.VerifyResponseSigniture(partner, xmlElement);
            }
            else
            {
                audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(),
                    "The X509 Certificate was not found at the location specified.", ipAddress, String.Empty, String.Empty, dbId);
                throw (new ArgumentException("The X509 Certificate was not found at the location specified."));
            }

            if (!verified)
            {
                audit.AddException(Guid.Empty.ToString(), Guid.Empty.ToString(),
                    "The SAML response signature failed to verify for partnerID " + pageState.GetStringOrEmpty("PartnerID"),
                    ipAddress, String.Empty, String.Empty, dbId);
                throw (new ArgumentException("The SAML response signature failed to verify."));
            }
        }

        string ISso.CheckForActivationWizardRedirect(Partner partner, Guid userId, IStateContainer pageState, IEPSBroker epsBroker)
        {
            return CheckForActivationWizardRedirect(partner, userId, pageState, epsBroker, new ActivationWizard());
        }

        /// <summary>
        /// Wrapped this in order to test methods that call this
        /// </summary>
        /// <param name="partner"></param>
        /// <param name="xmlElement"></param>
        /// <returns></returns>
        public bool VerifyResponseSigniture(Partner partner, XmlElement xmlElement)
        {
            return ResponseSignature.Verify(xmlElement, partner.Certificate2);
        }

        internal static void SetPartnerSession(Partner partner, IStateContainer pageState)
        {
            pageState["PartnerID"] = partner.ID;

            if (partner.Theme.Trim() != String.Empty)
            {
                pageState["Theme"] = partner.Theme;
            }

            if (partner.LogoutUrl != null)
            {
                pageState["PartnerLogoutUrl"] = partner.LogoutUrl;
            }
        }

        internal static Partner FindPartnerByIssuer(Partners partners, string issuer)
        {
            var partner = partners.Find(x => x.Issuer.ToUpper() == issuer.ToUpper());
            
            if (partner != null)
            {
                return partner;
            }

            throw (new ArgumentException("The Issuer '" + issuer + "' was not found to verify SAML response signature."));
        }

        protected internal static AuthenticationStatement GetAuthenticationStatement(Assertion assertion)
        {
            // Get the subject from the assertion's authentication statement
            IList authenticationStatements = assertion.GetAuthenticationStatements() as IList;

            if (authenticationStatements == null || authenticationStatements.Count == 0)
            {
                throw (new ArgumentException("There's no authentication statement in the SAML response."));
            }

            var authenticationStatement = (AuthenticationStatement)authenticationStatements[0];

            return authenticationStatement;
        }

        
        protected internal static bool IsNameIdentifierValuePopulated(NameIdentifier nameIdentifier)
        {
            return (!String.IsNullOrWhiteSpace(nameIdentifier.Value));
        }

        protected internal static bool IsKeynoteSSoUser(NameIdentifier nameIdentifier, ISystemConfig systemConfig)
        {
            return (IsNameIdentifierValuePopulated(nameIdentifier)
                && nameIdentifier.Value.Equals(systemConfig.GetAppSetting("KeynoteSSOUserGuid"), StringComparison.OrdinalIgnoreCase));
        }

        protected internal static bool IsConditionsInSsoPost(Assertion assertion)
        {
            return (assertion.Conditions != null);
        }

        protected internal static bool IsTimeValidOnSsoPost(Assertion assertion)
        {
            TimeSpan clockSkew = new TimeSpan(0, 10, 0); //10 minutes

            return (assertion.Conditions.IsWithinTimePeriod(clockSkew));
        }

        protected internal static long GetOperationValue(string operation)
        {
            if (operation != default(string))
            {
                if (operation.ToLower().Contains("x"))
                {
                    return Convert.ToInt32(operation, 16);
                }

                return Convert.ToInt32(operation);
            }
            return Constants.SsoOperations.OPERATION_USERGUID;
        }

        public Partner FindAndSetPartnerValuesFromGuid(Partners partners, Guid partnerGuid, IStateContainer pageState)
        {
            var partner = FindPartnerByGuid(partners, partnerGuid);

            SetPartnerSession(partner, pageState);

            return partner;
        }

        internal static Partner FindPartnerByGuid(Partners partners, Guid partnerGuid)
        {
            var partner = partners.Find(x => x.ID.ToUpper() == partnerGuid.ToString().ToUpper());

            if (partner != null)
            {
                return partner;
            }

            throw (new ArgumentException("The partner '" + partnerGuid + "' was not found."));
        }

        protected internal static bool IsEpaTaskMode(string ssoMode)
        {
            if (!String.IsNullOrEmpty(ssoMode))
            {
                if (ssoMode.Equals(Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        protected internal static Guid GetPatientGuidGetWithAttribute(SsoAttributes ssoAttributes, out bool isMultiplePatientsFound, IPatient patient, ConnectionStringPointer dbId)
        {
            isMultiplePatientsFound = false;

            if (ssoAttributes.PatientId != Guid.Empty)
            {
                return ssoAttributes.PatientId;
            }

            if (!string.IsNullOrWhiteSpace(ssoAttributes.PatientMrn))
            {
                var mrnPatientDs = patient.SearchByChartID(ssoAttributes.PatientMrn, ssoAttributes.LicenseId.ToString(), dbId);

                if (mrnPatientDs != null && mrnPatientDs.Tables.Count > 0 &&
                    mrnPatientDs.Tables[0].Rows.Count > 0)
                {
                    //we found one patientGUID for the MRN, so set the _patientGUID accordingly
                    if (mrnPatientDs.Tables[0].Rows.Count == 1)
                    {
                        if (!String.IsNullOrEmpty(mrnPatientDs.Tables[0].Rows[0]["PatientID"].ToString()))
                        {
                            return new Guid(mrnPatientDs.Tables[0].Rows[0]["PatientID"].ToString());
                        }
                    }
                    else if (mrnPatientDs.Tables[0].Rows.Count > 1)
                    //we found more than 1 patientGUID for the MRN, so redirect user to choose patient page
                    {
                        isMultiplePatientsFound = true;
                    }
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// If there is data to pass then return true if not return false
        /// </summary>
        /// <param name="patientDoc"></param>
        /// <param name="doc"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        protected  internal static string LoadPatientXmlDoc(string patientDoc, ref XmlDocument doc, IStateContainer session)
        {
            if (!String.IsNullOrEmpty(patientDoc))
            {
                patientDoc = patientDoc.Replace("&lt;", "<");
                patientDoc = patientDoc.Replace("&gt;", ">");
                patientDoc = patientDoc.Replace("&", "&amp;"); // To resolve the XML parsing error with special characters

                try
                {
                    doc.LoadXml(patientDoc);
                    return patientDoc;
                }
                catch (XmlException xmlEx)
                {
                    session["SSOError"] = "An error occurred when processing the patient record. Some data may not be in sync. Please contact support and notify them of this issue. Error: " + xmlEx.Message;
                    return null;
                }
            }

            return null;
        }

        internal static Assertion GetAssertion(Response samlResponse)
        {
            if (!samlResponse.Status.IsSuccess())
            {
                throw (new ArgumentException("The SAML response status failed, possible an invalid assertion."));
            }

            if (samlResponse.Assertions != null && samlResponse.Assertions.Count > 0)
            {
                var assertion = samlResponse.Assertions[0] as Assertion;
                if (assertion != null)
                {
                    return assertion;
                }

                var samlAssertionElement = samlResponse.Assertions[0] as XmlElement;
                if (samlAssertionElement != null) return new Assertion(samlAssertionElement);
            }

            return null;
        }

        internal static XmlElement CheckSsoIsSigned(string base64SamlResponse)
        {
            // Reconstruct the XML.
            XmlElement xmlElement = SAML.FromBase64String(base64SamlResponse);

            // Verify the SAML response signature.
            if (!ResponseSignature.IsSigned(xmlElement))
            {
                throw (new ArgumentException("The SAML response must be signed."));
            }
            return xmlElement;
        }

        internal static void SetSsoMode(string ssoMode, IStateContainer pageState)
        {
            if (!String.IsNullOrEmpty(ssoMode))
            {
                if (ssoMode.Equals(Constants.SSOMode.PATIENTLOCKDOWNMODE, StringComparison.OrdinalIgnoreCase))
                {
                    pageState["SSOMode"] = Constants.SSOMode.PATIENTLOCKDOWNMODE;
                }
                else if (ssoMode.Equals(Constants.SSOMode.TASKMODE, StringComparison.OrdinalIgnoreCase))
                {
                    pageState["SSOMode"] = Constants.SSOMode.TASKMODE;
                }
                else if (ssoMode.Equals(Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE, StringComparison.OrdinalIgnoreCase))
                {
                    pageState["SSOMode"] = Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE;
                }
                else if (ssoMode.Equals(Constants.SSOMode.SSOIDPROOFINGMODE, StringComparison.OrdinalIgnoreCase))
                {
                    pageState["SSOMode"] = Constants.SSOMode.SSOIDPROOFINGMODE;
                }
                else if (ssoMode.Equals(Constants.SSOMode.UTILITYMODE, StringComparison.OrdinalIgnoreCase))
                {
                    pageState["SSOMode"] = Constants.SSOMode.UTILITYMODE;
                }
            }
        }

        public static string CheckForActivationWizardRedirect(Partner partner, Guid userId, IStateContainer pageState, IEPSBroker epsBroker, IActivationWizard activationWizard)
        {
            var actCheckInfo = activationWizard.LoadCheckInfo(userId,pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

            if (partner.AllowShieldEnrollment
                && partner.AllowsUserNameAndPassword
                && actCheckInfo?.Status == ShieldUserStatus.PENDING_ACTIVATION)
            {
                int epsIntTenantId = epsBroker.GetShieldInternalTenantID(actCheckInfo.AccountId);
                var activationCodeResponse = epsBroker.GetNewActivationCode(epsIntTenantId, actCheckInfo.Username);
                pageState["ActivationCodeFromSSO"] = activationCodeResponse.ActivationCode;
                return "~/" + Constants.PageNames.ACTIVATE;
            }
            return null;
        }

        internal static string GetTaskRedirectUrl(IStateContainer pageState)
        {
            if (pageState.GetBooleanOrFalse("IsProvider") || pageState.GetBooleanOrFalse("IsPA"))
            {
                return Constants.PageNames.DOC_REFILL_MENU;
            }

            var userType = pageState.Cast("UserType", Constants.UserCategory.GENERAL_USER);
            if (userType == Constants.UserCategory.GENERAL_USER || userType == Constants.UserCategory.POB_LIMITED)
                return Constants.PageNames.REPORTS;

            return Constants.PageNames.LIST_SEND_SCRIPTS;
        }

        public static string GetRedirect(string currentRedirect, string nextPage, bool isForcePwSetup, IBasePage basePage, IStateContainer session)
        {
            if (isForcePwSetup)
            {
                nextPage = $"{Constants.PageNames.FORCE_PASSWORD_SETUP}?targetURL={nextPage}";
            }
            //first check to see if the user profile is setup correctly
            if (currentRedirect.Contains(Constants.PageNames.EDIT_USER))
            {
                if (currentRedirect[0] == '~') currentRedirect = currentRedirect.Substring(1);

                if (currentRedirect.Contains(Constants.PageNames.SELECT_ACCOUNT_AND_SITE))
                {
                    return $"~/{Constants.PageNames.SET_HEIGHT}?dest={currentRedirect}";
                }

                return $"~/{Constants.PageNames.SET_HEIGHT}?dest={Constants.PageNames.SPA_LANDING}?page={currentRedirect}";
            }

            if (currentRedirect.Contains(Constants.PageNames.SELECT_ACCOUNT_AND_SITE))
            {
                return $"{basePage.GetSiteUrl()}/{Constants.PageNames.SELECT_ACCOUNT_AND_SITE}?TargetURL=/{Constants.PageNames.SET_HEIGHT}?dest={Constants.PageNames.SPA_LANDING}?page={nextPage}";
            }

            return $"{basePage.GetSiteUrl()}/{Constants.PageNames.SET_HEIGHT}?dest={Constants.PageNames.SPA_LANDING}?page={nextPage}";
        }

        internal static string GetTaskRedirectForEpaLockDown(IStateContainer pageState)
        {
            var userType = pageState.Cast("UserType", Constants.UserCategory.GENERAL_USER);

            if (userType == Constants.UserCategory.GENERAL_USER || userType == Constants.UserCategory.POB_LIMITED)
            {
                return Constants.PageNames.REPORTS;
            }

            return Constants.PageNames.DOC_REFILL_MENU;
        }

        internal static void AddPatientCoverage(XmlDocument xmlDocument, IStateContainer pageState, ConnectionStringPointer connectionStringPointer)
        {
            var planid = String.Empty;
            var planname = String.Empty;
            foreach (XmlNode node in xmlDocument.GetElementsByTagName("plan-info"))
            {
                XmlNodeList objXmlNodeList = node.ChildNodes;
                foreach (XmlNode ChildNode in objXmlNodeList)
                {
                    if (ChildNode.Name == "planid")
                    {
                        planid = ChildNode.InnerText;
                    }
                    if (ChildNode.Name == "planname")
                    {
                        planname = ChildNode.InnerText;
                    }
                }

                if (!String.IsNullOrEmpty(planid))
                {
                    Allscripts.Impact.PatientCoverage.AddPatientCoverage(0, pageState.GetStringOrEmpty("PATIENTID"),
                        pageState.GetStringOrEmpty("LICENSEID"), pageState.GetStringOrEmpty("USERID"), planid, planname,
                        String.Empty, String.Empty, String.Empty, String.Empty, connectionStringPointer);
                }
            }
        }

        internal static Guid VerifyUserGuid(AuthenticationStatement authenticationStatement)
        {
            Guid userGuid;
            if (!Guid.TryParse(authenticationStatement.Subject.NameIdentifier.Value, out userGuid))
            {
                logger.Error("User guid is not valid: <UserGuid>{0}<UserGuid><AuthenticationStatement>{1}<AuthenticationStatement>",
                    authenticationStatement.Subject.NameIdentifier.Value, authenticationStatement.ToLogString());
                throw new ArgumentException(string.Format("The user Guid {0} is not valid", authenticationStatement.Subject.NameIdentifier.Value));
            }
            return userGuid;
        }

        public static int GetShieldPasswordExpirationDays(string shieldLoginId)
        {
            return EPSBroker.GetShieldPasswordExpirationDays(shieldLoginId);
        }
    }
}