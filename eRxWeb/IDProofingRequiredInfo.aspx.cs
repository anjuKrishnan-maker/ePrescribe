using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using RxUser = Allscripts.Impact.RxUser;
using Provider = Allscripts.Impact.Provider;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;

namespace eRxWeb
{
    public partial class IDProofingRequiredInfo : BasePage
    {
        private int TryCnt;

        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
            if (base.CurrentUser == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Page.Title = ConfigurationManager.AppSettings["Appname"].ToString() + Page.Title;
                LoadStates();
                Session["TargetUrl"] = TargetUrl;
                if (CurrentUser != null)
                {
                    DataTable dtIDProofingDetails = Provider.GetIDProofingDetails(this.SessionUserID, base.DBID);
                    DataTable dtProviderDetails = Provider.LoadProviderDetailsForIDProofing(this.SessionUserID, base.SessionLicenseID, base.DBID);

                    if (dtIDProofingDetails != null && dtIDProofingDetails.Rows.Count > 0 && dtProviderDetails != null && dtProviderDetails.Rows.Count > 0)
                    {
                        var idProofingStatus = Convert.ToInt16(dtProviderDetails.Rows[0]["IDProofingStatusID"]).ToEnum<Constants.IDProofingStatusID>();

                        if (idProofingStatus == Constants.IDProofingStatusID.IDPROOFING_REQUIRED)
                        {
                            try
                            {
                                if (!Convert.ToBoolean(dtIDProofingDetails.Rows[0]["IsManual"].ToString()) && !Convert.ToBoolean(dtIDProofingDetails.Rows[0]["IsSuccessful"].ToString()))
                                {
                                    Response.Redirect(Constants.PageNames.MANUAL_ID_PROOFING_FORM);
                                }
                            }
                            catch (Exception)
                            {
                                //Log the fail information
                            }
                        }
                    }

                    var addressVerifyAttempts = PageState.Cast(Constants.SessionVariables.AddressVerifyRetryCount, 0);
                    lblErrorMessage.Visible = addressVerifyAttempts > 0;
                    if (addressVerifyAttempts > 2)
                    {
                        PageState[Constants.SessionVariables.HomeAddressCheckStatus] = Provider.UpdateHomeAddressCheckStatus(SessionUserID.ToGuidOr0x0(), HOME_ADDRESS_CHECK_STATUS.LOCKED, Request.UserIpAddress(), Page.ToString(), DBID);
                        Response.Redirect(Constants.PageNames.ADDRESS_VERIFY_ERROR);
                    }

                    if (dtProviderDetails != null && dtProviderDetails.Rows.Count > 0)
                    {
                        txtfirstname.Text = dtProviderDetails.Rows[0]["FirstName"].ToString();
                        txtlastname.Text = dtProviderDetails.Rows[0]["LastName"].ToString();
                        txtaddress.Text = dtProviderDetails.Rows[0]["HomeAddress"].ToString();
                        txtcity.Text = dtProviderDetails.Rows[0]["City"].ToString();
                        txtzip.Text = dtProviderDetails.Rows[0]["ZipCode"].ToString();
                        txtEmail.Text = dtProviderDetails.Rows[0]["Email"].ToString();
                        txtDEA.Text = dtProviderDetails.Rows[0]["DEANumber"].ToString();
                        txtUPIN.Text = dtProviderDetails.Rows[0]["NPI"].ToString();
                        ddlstate.SelectedValue = dtProviderDetails.Rows[0]["State"].ToString();
                        ePrescribeSvc.DEALicense[] deaList = EPSBroker.GetProviderDEALicenses(this.SessionUserID, base.DBID);
                        if (deaList.Length > 0)
                        {
                            chkDEAList.Items[0].Selected = deaList[0].DEAIIAllowed;
                            chkDEAList.Items[1].Selected = deaList[0].DEAIIIAllowed;
                            chkDEAList.Items[2].Selected = deaList[0].DEAIVAllowed;
                            chkDEAList.Items[3].Selected = deaList[0].DEAVAllowed;
                        }
                    }
                }
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            if(PageState[Constants.SessionVariables.AddressVerifyRetryCount] == null)
            {
                mpeWarning.Show();
            }
            else
            {
                btnProceed_Click(sender, e);
            }
        }
           

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            Provider.SaveIdProofingDetails(SessionUserID, txtaddress.Text, txtcity.Text, ddlstate.SelectedValue, txtzip.Text, DBID);

             if (Session["TryCount"] != null && !string.IsNullOrEmpty(Session["TryCount"].ToString()))
            {
                TryCnt = 0;
                int.TryParse(Session["TryCount"].ToString(), out TryCnt);
                TryCnt++;
            }
            else
            {
                TryCnt = 1;
            }

            Session["TryCount"] = TryCnt;
            Session["IDFirstName"] = txtfirstname.Text.Trim();
            Session["IDLastName"] = txtlastname.Text.Trim();
            Session["IDHomeAddress"] = txtaddress.Text.Trim();
            Session["IDCity"] = txtcity.Text.Trim();
            Session["IDState"] = ddlstate.SelectedItem.Value;
            Session["IDZIP"] = txtzip.Text.Trim();
            Session["IDYear"] = txtdobYear.Text.Trim();
            Session["IDEmail"] = txtEmail.Text.Trim();
            Session["IDSSN"] = txtssnLast4.Text.Trim();
            Session["IDDEA"] = txtDEA.Text.Trim();
            Session["IDNPI"] = txtUPIN.Text.Trim();
            Response.Redirect(Constants.PageNames.USER_INTERROGATION);
        
        }

        protected void btnDecline_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.LOGOUT);
        }

        protected string TargetUrl
        {
            get
            {
                string url = Request.Params["TargetUrl"];

                if (string.IsNullOrEmpty(url))
                {
                    if (CurrentUser.IsProvider)
                    {
                        url = Constants.PageNames.SELECT_PATIENT;
                    }
                    else
                    {
                        url = Constants.PageNames.SELECT_PATIENT;
                    }
                }
                else
                {
                    url = Constants.PageNames.UrlForRedirection(url);
                    if (string.IsNullOrEmpty(url))
                    {
                        var homeAddressCheckStatus = PageState.Cast(Constants.SessionVariables.HomeAddressCheckStatus, HOME_ADDRESS_CHECK_STATUS.NOT_VERIFIED);
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
                    else if(Request.QueryString["From"] != null)
                    {
                        url = string.Concat(url, "?From=", Request.QueryString["From"]);
                    }
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

        private void LoadStates()
        {

            DataTable dtState = RxUser.ChGetState(base.DBID);

            if (dtState != null || dtState.Rows.Count > 0)
            {
                ddlstate.DataSource = dtState;
                ddlstate.DataTextField = "Description";
                ddlstate.DataValueField = "state";
                ddlstate.DataBind();

                string selectState = "Select a State";
                ddlstate.Items.Insert(0, selectState);
                rfvState.InitialValue = selectState;
            }
        }
    }
}