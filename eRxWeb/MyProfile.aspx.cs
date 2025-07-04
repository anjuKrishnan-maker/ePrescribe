using System;
using System.Globalization;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.ePrescribe.Objects;
using eRxWeb;
using Provider = Allscripts.Impact.Provider;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;

namespace Allscripts.Web.UI
{
    public partial class MyProfile : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ucMessage.Visible = false;

            if (!IsPostBack)
            {
                if (Session["EditUser"] != null && bool.Parse(Session["EditUser"].ToString()))
                {
                    if (Convert.ToBoolean(PageState[Constants.SessionVariables.IsSSOUser]))
                    {
                        pnlChangePasswordLink.Visible = ShowChangePasswordForSSOUser();
                    }

                    pnlChangePassword.Visible = true;
                }
                else
                {
                    pnlChangePassword.Visible = false;
                }

                int tasks = 0;
                if (Session["LICENSEID"] != null)
                {
                    string licenseID = Session["LICENSEID"].ToString();

                    if (base.IsUserAPrescribingUserWithCredentials)
                    {
                        tasks = Provider.GetTaskCountForProvider(licenseID, Session["USERID"].ToString(), base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                        {
                            // get task count only for selected Providers associated to POB
                            tasks = TaskManager.GetTaskListScriptCount(licenseID, new Guid(base.SessionUserID), (int) Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                        }
                        else
                        {
                            // get task count all "assistant" tasks
                            tasks = TaskManager.GetTaskListScriptCount(licenseID, (int) Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                        }
                    }

                    Impact.RxUser rxUser = new Impact.RxUser(base.SessionUserID, base.DBID);
                    lblFirstName.Text = rxUser.FirstName;
                    lblLastName.Text = rxUser.LastName;
                    lblMI.Text = rxUser.MiddleInitial;
                    lblEmail.Text = Session["USEREMAIL"] == null ? "" : Session["USEREMAIL"].ToString();
                    lblUserName.Text = base.SessionShieldUserName;

                    foreach (Role appRole in rxUser.GetAllRoles())
                    {
                        if (lblRoles.Text == string.Empty)
                        {
                            lblRoles.Text += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(appRole.Name);
                        }
                        else
                        {
                            lblRoles.Text += ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(appRole.Name);
                        }
                    }
                }

                
                ((PhysicianMasterPageBlank)Master).toggleTabs("myerx", tasks);
            }

            if (SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On)
            {
              //  divHideTools_Help.Style.Add("display", "none");// Hiding HelpContent if the User is a Basic user (to highlight LogRx ads)
            }

            if (Request.QueryString["Msg"] != null && Request.QueryString["Msg"].Length > 0)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["Msg"]);
                ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            }
        }

        protected void rbtnMyProfile_CheckedChanged(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.MY_PROFILE);
        }

        private int GetePATaskCount()
        {
            int returnValue = 0;

            if (base.IsPOBUser)
            {
                returnValue = ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }
            else
            {
                returnValue = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }

            return returnValue;
        }

        private bool ShowChangePasswordForSSOUser() { 
            return base.SessionLicense.EnterpriseClient.ShowChangePassword &&
                   Convert.ToBoolean(PageState[Constants.SessionVariables.PartnerAllowsUserNameAndPassword]) &&
                 !(Convert.ToBoolean(PageState[Constants.SessionVariables.ForcePasswordSetupForSSOUser]));
        }

        protected void EditProfileButton_Click(object sender, EventArgs e)
        {
            var componentParameter = new ComponentRedirectionModel();
            componentParameter.CameFrom = Constants.PageNames.MY_PROFILE;
            Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.EDIT_USER, componentParameter));
        }
    }
}
