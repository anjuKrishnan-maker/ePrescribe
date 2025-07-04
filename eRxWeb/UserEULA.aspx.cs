using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using Provider = Allscripts.Impact.Provider;

namespace eRxWeb
{
public partial class UserEULA : BasePage
{
    private String sToken;
    private Guid GUIDToken;
    protected void Page_Init(object sender, EventArgs e)
    {
        lnkDefaultStyleSheet.Href +="?version=" + SessionAppVersion;
        if (CurrentUser == null)
        {
            Response.Redirect(Constants.PageNames.LOGOUT);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
        {
            if (base.IsIDProofingEnabled)
            {
                // If user is EPCS enabled, IDProofing is not required
                if (AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState))
                {
                    Provider.UpdateIDProofingSuccessful(base.SessionUserID, "EPCSUser", base.DBID);
                }

                if (PageState.GetBooleanOrFalse(Constants.SessionVariables.IsSSOUser))
                {
                    Int16 IDProofingStatusID = 0;
                    DataTable dtProviderDetails = Provider.LoadProviderDetailsForIDProofing(base.SessionUserID, base.SessionLicenseID, base.DBID);
                    if (dtProviderDetails != null && dtProviderDetails.Rows.Count > 0 && !string.IsNullOrEmpty(dtProviderDetails.Rows[0]["IDProofingStatusID"].ToString()))
                    {
                        //Allscripts.ePrescribe.Common.Constants.IDProofingStatusID
                        IDProofingStatusID = Convert.ToInt16(dtProviderDetails.Rows[0]["IDProofingStatusID"]);
                    }

                    DataTable dtIDProofingDetails = Provider.GetIDProofingDetails(this.SessionUserID, base.DBID);
                    if (dtIDProofingDetails != null && dtIDProofingDetails.Rows.Count > 0)
                    {
                        if (!Convert.ToBoolean(dtIDProofingDetails.Rows[0]["IsManual"].ToString()) && !Convert.ToBoolean(dtIDProofingDetails.Rows[0]["IsSuccessful"].ToString()))
                        {
                            if (IDProofingStatusID == Convert.ToInt16(Allscripts.ePrescribe.Common.Constants.IDProofingStatusID.IDPROOFING_REQUIRED))
                            {
                                if (Request.QueryString["From"] != "IDologySuccessful")
                                {
                                    Response.Redirect(string.Format("{0}?From=UserEULA&TargetUrl={1}", Constants.PageNames.IDPROOFING_REQUIRED_INFO, Request.Params["TargetUrl"]));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (IDProofingStatusID == Convert.ToInt16(Allscripts.ePrescribe.Common.Constants.IDProofingStatusID.IDPROOFING_REQUIRED))
                        {
                            if (Request.QueryString["From"] != "IDologySuccessful")
                            {
                                string UrlForRedirection = Constants.PageNames.UrlForRedirection(Request.Params["TargetUrl"]);
                                if (string.IsNullOrEmpty(UrlForRedirection))
                                {
                                    Response.Redirect(string.Format("{0}?From=UserEULA&TargetUrl={1}", Constants.PageNames.IDPROOFING_REQUIRED_INFO, Constants.PageNames.SELECT_PATIENT));
                                }
                                else
                                {
                                    Response.Redirect(string.Format("{0}?From=UserEULA&TargetUrl={1}", Constants.PageNames.IDPROOFING_REQUIRED_INFO, UrlForRedirection));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AppCode.StateUtils.UserInfo.IsIdProofingRequired(PageState))
                    {
                        Response.Redirect("~/" + Constants.PageNames.REGISTER_WELCOME);
                    }
                }
            }
           

            var homeAddressCheckStatus = PageState.Cast(Constants.SessionVariables.HomeAddressCheckStatus, HOME_ADDRESS_CHECK_STATUS.NOT_VERIFIED);

            homeAddressCheckNonSsoUser(homeAddressCheckStatus);

            if (base.IsNPICheckEnabled)
            {
                // If user is EPCS enabled, NPI check is not required
                if (AppCode.StateUtils.UserInfo.IsPermissionGranted(UserPermissions.EpcsCanPrescribe, PageState))
                {
                    Provider.UpdateNPICheckSuccessful(base.SessionUserID, base.DBID);
                    sToken = this.SessionUserID.ToString();
                    GUIDToken = new Guid(sToken);
                    Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("NPI check is not required, as the user is EPCS enabled"));
                }

                Int16 npiCheckStatusID = 0;
                DataTable dtProviderDetails = Provider.LoadProviderDetailsForIDProofing(base.SessionUserID, base.SessionLicenseID, base.DBID);
                if (dtProviderDetails != null && dtProviderDetails.Rows.Count > 0 && !string.IsNullOrEmpty(dtProviderDetails.Rows[0]["NPICheckStatusID"].ToString()))
                {
                    npiCheckStatusID = Convert.ToInt16(dtProviderDetails.Rows[0]["NPICheckStatusID"]);
                }

                DataTable dtNPICheckDetails = Provider.GetNPICheckDetails(this.SessionUserID, base.DBID);
                if (dtNPICheckDetails != null && dtNPICheckDetails.Rows.Count > 0)
                {
                    if (!Convert.ToBoolean(dtNPICheckDetails.Rows[0]["IsManual"].ToString()) && !Convert.ToBoolean(dtNPICheckDetails.Rows[0]["IsSuccessful"].ToString()))
                    {
                        if (npiCheckStatusID == Convert.ToInt16(Allscripts.ePrescribe.Common.Constants.NPICheckStatusID.NPI_CHECK_REQUIRED))
                        {
                            if (Request.QueryString["From"] != "IDologySuccessful")
                            {
                                Response.Redirect(Constants.PageNames.NPI_CHECK + "?From=UserEULA");
                            }
                            else
                            {
                                Response.Redirect(Constants.PageNames.NPI_ERROR + "?From=UserEULA");
                            }
                        }
                    }
                }
                else
                {
                    if (npiCheckStatusID == Convert.ToInt16(Allscripts.ePrescribe.Common.Constants.NPICheckStatusID.NPI_CHECK_REQUIRED))
                    {
                        if (Request.QueryString["From"] != "IDologySuccessful")
                        {
                            Response.Redirect(Constants.PageNames.NPI_CHECK + "?From=UserEULA");
                        }
                        else
                        {
                            Response.Redirect(Constants.PageNames.NPI_ERROR + "?From=UserEULA");
                        }
                    }

                }

            }

            if (PageState.GetBooleanOrFalse(Constants.SessionVariables.IsSSOUser))
            {
                if (CurrentUser.HasAcceptedEULA && Request.QueryString["Deluxe"] == null && !isPartnerEULA)
                {
                    string UrlForRedirection = Constants.PageNames.UrlForRedirection(TargetUrl);
                    if (string.IsNullOrEmpty(UrlForRedirection))
                    {
                        var userId = PageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
                        switch (homeAddressCheckStatus)
                        {
                            case HOME_ADDRESS_CHECK_STATUS.NOT_VERIFIED:
                                Audit.AddLogEntryIdology(/*Request.UserHostName*/string.Empty, "SelectPatientApiController", userId.ToGuidOr0x0(), "Home address status not verified, sending to IdProofingRequiredInfo");
                                PageState[Constants.SessionVariables.AddressVerifyRetryCount] = 1;
                                Response.Redirect(Constants.PageNames.IDPROOFING_REQUIRED_INFO);
                                break;
                            case HOME_ADDRESS_CHECK_STATUS.LOCKED:
                                Audit.AddLogEntryIdology(/*Request.UserHostName*/string.Empty, "SelectPatientApiController", userId.ToGuidOr0x0(), "Home address status locked, sending to Address error page.");
                                Response.Redirect(Constants.PageNames.ADDRESS_VERIFY_ERROR);
                                break;
                            default:
                                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                                {
                                    PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                                };
                                RedirectToSelectPatient(null, selectPatientComponentParameters);
                                break;
                        }
                    }
                    else
                    {
                        if (UrlForRedirection.Equals(Constants.PageNames.SELECT_PATIENT))
                        {
                            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                            {
                                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                            };
                            RedirectToSelectPatient(null, selectPatientComponentParameters);
                        }
                        else
                        {
                            if (PageState.GetBooleanOrFalse(Constants.SessionVariables.AppComponentAlreadyInitialized))
                            {
                                Response.Redirect(UrlForRedirection);
                            }
                            else
                            {
                                Response.Redirect($"{Constants.PageNames.SPA_LANDING}Page={UrlForRedirection}");
                            }
                        }
                    }
                }
            }


            Page.Title = ConfigurationManager.AppSettings["Appname"].ToString() + Page.Title;

            if (Session["UserName"] != null)
            {
                lblUser.Text = Session["UserName"].ToString();
            }

            if (Session["SITENAME"] != null)
            {
                lblSiteName.Text = Session["SITENAME"].ToString();
            }

            if (isPartnerEULA)
            {
                string partnerEULA = Allscripts.Impact.Partner.GetPartnerEULA(Request.Params["PID"]);

                eulaContent.Controls.Clear();
                eulaContent.Controls.Add(new LiteralControl(partnerEULA));
            }
            else
            {

                ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                eulaContent.Controls.Clear();
                eulaContent.Controls.Add(new LiteralControl(eps.GetEULA()));

            }

            // Google Analytics
            PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());
        }

        private void homeAddressCheckNonSsoUser(HOME_ADDRESS_CHECK_STATUS homeAddressCheckStatus)
        {
            if (!PageState.GetBooleanOrFalse(Constants.SessionVariables.IsSSOUser)
                && homeAddressCheckStatus == HOME_ADDRESS_CHECK_STATUS.NOT_COMPLETED)
            {
                Response.Redirect(Constants.PageNames.UrlForRedirection($"{Constants.PageNames.SPA_LANDING}?page={Constants.PageNames.HOME_ADDRESS}"));
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
    {
        if (isPartnerEULA)
        {
            LiteralControl l = eulaContent.Controls[0] as LiteralControl;

            Allscripts.Impact.Partner.SavePartnerEULA(
                Request.Params["PID"],
                base.SessionUserID,
                true,
                base.DBID);

            Session["Accepted_EPA_EULA"] = true;

            StringBuilder sbPath = new StringBuilder();

            if (Request.Params["RedirectTo"] != null)
            {
                    string redirectTo = Constants.PageNames.UrlForRedirection(Request.Params["RedirectTo"].ToString());
                sbPath.Append(redirectTo).Append("?From=" + Constants.PageNames.USER_EULA);
            }

            if (Request.Params["specialty"] != null)
            {
                sbPath.Append("&specialty=").Append(Request.Params["specialty"].ToString());
            }
            Response.Redirect(sbPath.ToString());
        }
        else
        {
            if (Request.QueryString["Deluxe"] == null)
            {
                ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();

                
                CurrentUser.AcceptEULA(eps.GetEULA(), base.DBID);
                
                Session["HasAcceptedEula"] = true;
            }

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.SSOIDPROOFINGMODE)
            {
                Response.Redirect("/" + Constants.PageNames.LOGOUT + "?SSOAccept=true");
            }
            else
            {
                string UrlForRedirection = Constants.PageNames.UrlForRedirection(TargetUrl);
                if (string.IsNullOrEmpty(UrlForRedirection))
                {
                    SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                    {
                        PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
                    };
                    RedirectToSelectPatient(null, selectPatientComponentParameters);
                }
                else
                {
                    Response.Redirect(UrlForRedirection);
                }
            }
        }
    }

    protected void btnDecline_Click(object sender, EventArgs e)
    {
        if (isPartnerEULA)
        {
            Allscripts.Impact.Partner.SavePartnerEULA(
                Request.Params["PID"],
                base.SessionUserID,
                false,
                base.DBID);
            
            Response.Redirect(Constants.PageNames.UrlForRedirection(fallbackURL));
        }
        else
        {
            ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
            eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
          
            CurrentUser.DeclineEULA(eps.GetEULA(), base.DBID);
            
            Session["HasAcceptedEula"] = false;
            Response.Redirect(Constants.PageNames.LOGOUT);
        }
    }

    protected string TargetUrl
    {
        get
        {
            string url = Request.Params["TargetUrl"];

            if (string.IsNullOrEmpty(url))
            {
                url = AngularStringUtil.CreateInitComponentUrl(Constants.PageNames.SELECT_PATIENT);
            }
            else
            {
                url = Constants.PageNames.UrlForRedirection(url);
                if (string.IsNullOrEmpty(url))
                {
                    url = Constants.PageNames.SPA_LANDING;
                }              
                else if(Request.QueryString["From"] != null)
                {
                    url = string.Concat(url, "?From=", Request.QueryString["From"]);
                }
            }

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.SSOIDPROOFINGMODE)
            {
                url = Constants.PageNames.LOGOUT + "?SSOAccept=true";
            }

            return url;
        }
    }

    private string fallbackURL
    {
        get
        {
            return Request.Params["FallbackURL"];
        }
    }

    private bool isPartnerEULA
    {
        get
        {
            return Request.Params["PID"] != null;

        }
    }
}

}