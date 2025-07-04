using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;
using ComponentSpace.SAML2.Protocols;
using ComponentSpace.SAML2.Bindings;
using ComponentSpace.SAML2.Profiles.SSOBrowser;
using Microsoft.Security.Application;

namespace eRxWeb
{
    public partial class SAML2_IntersiteTransferCaremarkEPA : BasePage
    {
        private X509Certificate2 _x509Certificate2;
        private AsymmetricAlgorithm _privateKey;
        private string _targetURL;
        string _issuer;
        string _errorURL;
        bool _isSpecialtyMed = false;
        ArrayList _audienceRestrictionList;

        private const string ATTRIBUTE_CAS_USER_ID = "CASUserId";
        private const string ATTRIBUTE_CAS_EXT_USER_ID = "CASExternalUserId";
        private const string ATTRIBUTE_ERROR_URL = "errorURL";
        private const string CAREMARK_EPA_PARTNER_ID = "505C0B6C-D1BC-419A-AAE7-D857CC5B9119";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (hasAcceptedEULA)
                {
                    if (Request.QueryString["specialty"] != null)
                    {
                        _isSpecialtyMed = Convert.ToBoolean(Request.QueryString["specialty"].ToString());
                    }

                    processSAML2Transfer();
                }
                else
                {
                    //Fortify issue, _isSpecialtyMed sending as query string validator.
                    if (Request.QueryString["specialty"] != null)
                    {
                        _isSpecialtyMed = Convert.ToBoolean(Request.QueryString["specialty"].ToString());
                        Response.Redirect("../" + Constants.PageNames.USER_EULA + "?PID=" + CAREMARK_EPA_PARTNER_ID + "&RedirectTo=SAML2/" + Constants.PageNames.INTERSITE_TRANSFER_CAREMARK_EPA + "&FallbackURL=" + Constants.PageNames.EULA_DECLINE + "&specialty=" + Encoder.UrlEncode(_isSpecialtyMed.ToString()));
                    }
                    else
                    {
                        Response.Redirect("../" + Constants.PageNames.USER_EULA + "?PID=" + CAREMARK_EPA_PARTNER_ID + "&RedirectTo=SAML2/" + Constants.PageNames.INTERSITE_TRANSFER_CAREMARK_EPA + "&FallbackURL=" + Constants.PageNames.EULA_DECLINE);
                    }
                }
            }
        }

        private bool hasAcceptedEULA
        {
            get
            {
                string eulaFrequency = ConfigurationManager.AppSettings["ePA_EULA_Frequency"];
                bool hasAcceptedEULA = false;

                if (eulaFrequency.Equals("OnceEver", StringComparison.OrdinalIgnoreCase))
                {
                    hasAcceptedEULA = Allscripts.Impact.Partner.UserHasAcceptedPartnerEULA(CAREMARK_EPA_PARTNER_ID, base.SessionUserID, base.DBID);
                }
                else if (eulaFrequency.Equals("OncePerSession", StringComparison.OrdinalIgnoreCase))
                {
                    if (Session["Accepted_EPA_EULA"] != null)
                    {
                        hasAcceptedEULA = bool.Parse(Session["Accepted_EPA_EULA"].ToString());
                    }
                }
                else if (eulaFrequency.Equals("EveryTime", StringComparison.OrdinalIgnoreCase))
                {
                    if (Request.Params["From"] != null && Request.Params["From"].ToLower().Contains(Constants.PageNames.USER_EULA.ToLower()))
                    {
                        hasAcceptedEULA = true;
                    }
                    else
                    {
                        hasAcceptedEULA = false;
                    }
                }

                return hasAcceptedEULA;
            }
        }

        private void processSAML2Transfer()
        {
            try
            {
                loadPartnerVariables();

                if (string.IsNullOrEmpty(_targetURL))
                {
                    lblStatus.Text = "Error: Target URL is empty. SSO failed.";
                    return;
                }

                // create the SAML response
                SAMLResponse samlResponse = createSAML2Response();

                // now send the SAML response to the service provider
                sendSAMLResponse(samlResponse);
            }

            catch (Exception ex)
            {
                lblStatus.Text = "Error: SSO failed. " + ex.Message + " " + ex.StackTrace;

                Allscripts.Impact.Audit.AddException(
                    base.SessionUserID,
                    base.SessionLicenseID,
                    ex.ToString(),
                    Request.UserIpAddress(),
                    null,
                    null,
                    base.DBID);
            }
        }

        private SAMLResponse createSAML2Response()
        {
            DateTime minDateTimeUTC = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5));
            DateTime maxDateTimeUTC = DateTime.UtcNow.AddMinutes(5);

            SAMLResponse samlResponse = new SAMLResponse();
            samlResponse.Destination = _targetURL;

            Issuer samlIssuer = new Issuer(_issuer);
            samlResponse.Issuer = samlIssuer;
            samlResponse.Status = new Status(SAMLIdentifiers.PrimaryStatusCodes.Success, null);

            SAMLAssertion samlAssertion = new SAMLAssertion();
            samlAssertion.Issuer = samlIssuer;

            //set the subject
            Subject subject = new Subject(new NameID(Session["USERCODE"].ToString()));
            SubjectConfirmation subjectConfirmation = new SubjectConfirmation(SAMLIdentifiers.SubjectConfirmationMethods.Bearer);
            SubjectConfirmationData subjectConfirmationData = new SubjectConfirmationData();
            subjectConfirmationData.Recipient = _targetURL;
            subjectConfirmation.SubjectConfirmationData = subjectConfirmationData;
            subjectConfirmationData.NotOnOrAfter = maxDateTimeUTC;
            subject.SubjectConfirmations.Add(subjectConfirmation);
            samlAssertion.Subject = subject;

            //set the conditions
            AudienceRestriction audienceRestriction = new AudienceRestriction();
            Conditions conditions = new Conditions(minDateTimeUTC, maxDateTimeUTC);
            samlAssertion.Conditions = conditions;

            foreach (object o in _audienceRestrictionList)
            {
                audienceRestriction = new AudienceRestriction();
                audienceRestriction.Audiences.Add(new Audience(o.ToString()));
                samlAssertion.Conditions.ConditionsList.Add(audienceRestriction);
            }

            //set the authentication statement
            AuthnStatement authnStatement = new AuthnStatement();
            authnStatement.AuthnContext = new AuthnContext();
            authnStatement.AuthnContext.AuthnContextClassRef = new AuthnContextClassRef(SAMLIdentifiers.AuthnContextClasses.Password);
            authnStatement.SessionIndex = Session.SessionID;
            authnStatement.SessionNotOnOrAfter = maxDateTimeUTC;
            samlAssertion.Statements.Add(authnStatement);

            /*
                10/25/2010 - JT
                Caremark CVS currently only accepts 15 character length usernames. Truncate our username 
                on the SSO post to 15 characters since this is what we've sent them on our registration file.
            */
            string ePrescribeUserName = Session["USERCODE"].ToString();

            if (ePrescribeUserName.Length > 15)
            {
                ePrescribeUserName = ePrescribeUserName.Substring(0, 15);
            }

            samlAssertion.SetAttributeValue(ATTRIBUTE_CAS_EXT_USER_ID, ePrescribeUserName);
            samlAssertion.SetAttributeValue(ATTRIBUTE_ERROR_URL, _errorURL);

            samlResponse.Assertions.Add(samlAssertion);

            return samlResponse;
        }

        private void sendSAMLResponse(SAMLResponse samlResponse)
        {
            //serialize the SAML response for transmission
            XmlElement samlResponseXml = samlResponse.ToXml();

            //sign the SAML response 
            if (_x509Certificate2 == null || _privateKey == null)
            {
                lblStatus.Text = "Error: Certificate or Private Key is null. SSO failed.";
                return;
            }

            SAMLMessageSignature.Generate(samlResponseXml, _privateKey, _x509Certificate2);

            Allscripts.Impact.Audit.PartnerInsert(
                CAREMARK_EPA_PARTNER_ID,
                base.SessionUserID,
                Request.UserIpAddress(),
                "SAML Response XML",
                samlResponseXml.OwnerDocument.InnerXml,
                base.DBID);

            IdentityProvider.SendSAMLResponseByHTTPPost(Response, _targetURL, samlResponseXml, _targetURL);
        }

        private void loadPartnerVariables()
        {
            Partners partners = (Partners)Application[Global.App_Partners];
            bool foundIt = false;

            foreach (Partner p in partners)
            {
                if (p.ID.Equals(CAREMARK_EPA_PARTNER_ID, StringComparison.OrdinalIgnoreCase))
                {
                    _x509Certificate2 = p.Certificate2;
                    _privateKey = _x509Certificate2.PrivateKey;
                    _targetURL = p.DestinationUrl;
                    _issuer = p.Issuer;
                    _errorURL = p.ErrorURL;
                    _audienceRestrictionList = p.AudienceRestrictionList;

                    foundIt = true;

                    break;
                }
            }

            if (!foundIt)
            {
                throw new ApplicationException("Cannot find Caremark ePA PartnerID.");
            }
        }
    }

}