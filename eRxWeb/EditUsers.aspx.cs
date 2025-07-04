using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using Allscripts.Impact;
using Telerik.Web.UI;
using Allscripts.ePrescribe.Common;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using ConfigKeys = Allscripts.Impact.ConfigKeys;

namespace eRxWeb
{
public partial class EditUsers : BasePage
{

    #region Properties

    private string activationCode
    {
        get
        {
            if (ViewState["ActivationCode"] == null)
            {
                ViewState["ActivationCode"] = string.Empty;
            }
            return ViewState["ActivationCode"].ToString();
        }
        set
        {
            ViewState["ActivationCode"] = value;
        }
    }

    private string finalMessage
    {
        get
        {
            string finalMessage;
            if (this.userID == base.SessionUserID)
            {
                finalMessage = "Your profile has been successfully saved.  Please logout and re-login for changes to take effect.";
            }
            else
            {
                finalMessage = "User " +UserName+" has been successfully saved.";
            }
            return finalMessage;
        }
    }

    private string userID
    {
        get
        {
            if (ViewState["UserID"] == null)
            {
                ViewState["UserID"] = string.Empty;
            }
            return ViewState["UserID"].ToString();
        }
        set
        {
            ViewState["UserID"] = value;
        }
    }

    private string UserName
    {
        get
        {
            if (ViewState["UserName"] == null)
            {
                ViewState["UserName"] = string.Empty;
            }
            return ViewState["UserName"].ToString();
        }
        set
        {
            ViewState["UserName"] = value;
        }
    }

    #endregion

    #region Page Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        ucError.Visible = false;
        ucError.MessageText = string.Empty;

        ucMessage.Visible = false;
        ucMessage.MessageText = string.Empty;


        if (!Page.IsPostBack)
        {
            if (Request.QueryString["message"] != null && Request.QueryString["message"].Length > 0)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["message"]);
                ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
            }

            if (!base.IsLicenseShieldEnabled)
            {
                gridUsers.AllowCustomPaging = false;
            }

            // get count of all users for the license
            //gridUsers.VirtualItemCount = EPSBroker.GetUserCountForLicense(Session["AHSAccountID"].ToString(), base.DBID);

            // default sort to Last Name/Ascending
            GridSortExpression sortExpression = new GridSortExpression();
            sortExpression.FieldName = "LastName";
            sortExpression.SortOrder = GridSortOrder.Ascending;
            gridUsers.MasterTableView.SortExpressions.AddSortExpression(sortExpression);
            PageState.Remove("arrUsersResults");

            }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    protected void gridUsers_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = (GridDataItem)e.Item;
            LinkButton lbActionLink = null;

            string shieldStatus = dataBoundItem.GetDataKeyValue("ShieldStatus") == null ? string.Empty : dataBoundItem.GetDataKeyValue("ShieldStatus").ToString();

            bool isUserShieldEnabled = base.IsLicenseShieldEnabled;

            switch (shieldStatus)
            {
                case "0":
                case "1":
                    // user that has not been migrated to Shield (legacy user active or inactive in ePrescribe DB?)
                    // hide "edit" link, set username to red and italicized, set Status to "Pending Migration" set Action link to "Reset Password"

                    dataBoundItem["Edit"].Controls[0].Visible = false;
                    dataBoundItem["LoginID"].ForeColor = System.Drawing.Color.Red;
                    dataBoundItem["LoginID"].Font.Italic = true;

                    lbActionLink = (LinkButton)dataBoundItem["ActionLink"].Controls[0];
                    if (lbActionLink != null)
                    {
                        lbActionLink.CommandName = "ChangePassword";
                        lbActionLink.Text = "Change Password";
                    }
                    if (shieldStatus == "0")
                    {
                        dataBoundItem["Status"].Text = "Pending Migration";
                    }

                    if (!((UsersResults)dataBoundItem.DataItem).Active)
                    {
                        dataBoundItem["Status"].Text = "Inactive";
                    }
                    break;
                case "2":
                    // user that has been to Shield via Add User or Registration but has not completed the account setup
                    // set Status to "Pending Activation" set Action link to "Request New Activation Code"
                    lbActionLink = (LinkButton)dataBoundItem["ActionLink"].Controls[0];
                    if (lbActionLink != null)
                    {
                        lbActionLink.Text = "Request New Activation Code";
                        lbActionLink.CommandName = "NewActivationCode";
                    }

                    if (!((UsersResults) dataBoundItem.DataItem).Active)
                    {
                        dataBoundItem["Status"].Text = "Inactive";
                    }

                    break;
                case "3":
                    // user with a Shield account
                    // set Action link to "Reset Password" 
                    lbActionLink = (LinkButton)dataBoundItem["ActionLink"].Controls[0];
                    if (lbActionLink != null)
                    {
                        lbActionLink.Text = "Reset Password";
                        lbActionLink.CommandName = "ResetPassword";
                        lbActionLink.OnClientClick = "javascript:if(!confirm('Are you sure you want to reset this user\\'s Password?')){return false;}";
                    }

                    dataBoundItem["Edit"].Controls[0].Visible = true;

                    if (((UsersResults)dataBoundItem.DataItem).Active)
                    {
                        dataBoundItem["Status"].Text = "Active";
                    }
                    else
                    {
                        dataBoundItem["Status"].Text = "Inactive";
                    }

                    break;
            }

            if (!SessionLicense.EnterpriseClient.EditUser && lbActionLink != null)
            {
                lbActionLink.Enabled = false;
                lbActionLink.ForeColor = System.Drawing.Color.LightGray;
                lbActionLink.Font.Underline = true;
                lbActionLink.OnClientClick = "";
            }
        }

        else if (e.Item is GridPagerItem)
        {
            GridPagerItem pager = (GridPagerItem)e.Item;
            Label lbl = (Label)pager.FindControl("ChangePageSizeLabel");
            lbl.Visible = false;

            RadComboBox combo = (RadComboBox)pager.FindControl("PageSizeComboBox");
            combo.Visible = false;
        }
    }

    protected void gridUsers_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        string userName = string.Empty;

        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = (GridDataItem)e.Item;
            this.userID = dataBoundItem.GetDataKeyValue("UserID").ToString();
            this.UserName = dataBoundItem.GetDataKeyValue("FirstName").ToString() + " " + dataBoundItem.GetDataKeyValue("LastName").ToString();                  

            switch (e.CommandName)
            {
                case "NewActivationCode":
                    ePrescribeSvc.GetNewActivationCodeResponse getNewACResponse = EPSBroker.GetNewActivationCode(base.ShieldInternalTenantID, dataBoundItem.GetDataKeyValue("ShieldProfileName").ToString());
                    if (getNewACResponse.Success)
                    {
                        this.activationCode = getNewACResponse.ActivationCode;
                        litActivationCode.Text = string.Format("Enter this Activation Code: {0}", this.activationCode.ToHTMLEncode());
						lblShieldPortalLink.Text = string.Concat(ConfigurationManager.AppSettings["appurl"], "/register/activateuser");
                        txtEmailAC.Text = dataBoundItem.GetDataKeyValue("Email").ToString();
                        mpeNewActivationCode.Show();
                    }
                    else
                    {
                        if (getNewACResponse.Messages.Length > 0)
                        {
                            Audit.AddException(base.SessionUserID, base.SessionLicenseID, getNewACResponse.Messages[0].ToString(), Request.UserIpAddress(), null, null, base.DBID);
                            // Actually this user is created successfully in shield. But not updated in our ePrescribe database. So if we get New Activate Response as 
                            // "This profile is already activated", we are going to show the message "This user is already activated" and updating our database 
                            // with shield username and shield status as Activation Completed. 
                            if (string.Equals(getNewACResponse.Messages[0].ToString(), "This profile is already activated.", StringComparison.CurrentCultureIgnoreCase))
                            {

                                ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                                       ePrescribeSvc.ValueType.UserGUID,
                                       this.userID,
                                       base.SessionLicenseID,
                                       base.SessionUserID,
                                       base.SessionLicenseID,
                                       base.DBID);

                                if (getUserRes.RxUser != null)
                                {
                                    ePrescribeSvc.RxUser rxUser = getUserRes.RxUser;

                                    //update erx db with shield info.
                                    EPSBroker.UpdateSecUsersInfoFromShield(
                                        rxUser.UserID,
                                        rxUser.UserName,
                                        rxUser.ShieldUserName,
                                        rxUser.FirstName,
                                        rxUser.LastName,
                                        ShieldUserStatus.ACTIVATION_COMPLETE,
                                        null,
                                        base.DBID
                                        );

                                    litExceptionText.Text = "This user is already activated.";
                                    mpeException.Show();
                                    break;
                                }
                            }
                        }

                        litExceptionText.Text = "Error attempting to generate Activation Code. Please contact Support.";
                        mpeException.Show();
                    }
                    break;
                case "Edit":
                        string componentParameter = string.Empty;
                        if (base.Session[Constants.SessionVariables.UserId].ToString() == dataBoundItem.GetDataKeyValue("UserID").ToString())
                        {
                            componentParameter = $"{{\"mode\": \"Edit\",\"CameFrom\": \"{Constants.PageNames.EDIT_USERS}\"}}";
                        }
                        else
                        {
                            componentParameter = $"{{\"mode\": \"Edit\",\"CameFrom\": \"{Constants.PageNames.EDIT_USERS}\",\"userid\": \"{dataBoundItem.GetDataKeyValue("UserID").ToString()}\"}}";
                        }
                    Response.Redirect($"{ Constants.PageNames.REDIRECT_TO_ANGULAR}?componentName={Constants.PageNames.EDIT_USER}&componentParameters={componentParameter}");
                    break;

                case "ChangePassword":
                    Response.Redirect(Constants.PageNames.RESET_PASSWORD + "?UserID=" + dataBoundItem.GetDataKeyValue("UserID").ToString() + "&UName=" + dataBoundItem.GetDataKeyValue("UserName").ToString());
                    break;
                case "ResetPassword":
                    ePrescribeSvc.GetNewPasswordResponse response = new ePrescribeSvc.GetNewPasswordResponse();
                    response = EPSBroker.ResetAndGeneratePasswordForUser(base.ShieldInternalTenantID, Convert.ToInt32(dataBoundItem.GetDataKeyValue("ShieldProfileUserID").ToString()));

                    if (response.Success)
                    {
                        litNewPassword.Text = response.NewPassword;
                        mpeResetPassword.Show();
                    }
                    else
                    {
                        ucMessage.Visible = true;
                        ucMessage.MessageText = "Could not generate a new password.";
                        ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                    }

                    break;
            }
        }
    }

    protected void btnSendACEmail_Click(object sender, EventArgs e)
    {
        if (
                (!string.IsNullOrWhiteSpace(txtEmailAC.Text.Trim()) && !isValidEmail(txtEmailAC.Text.Trim())) ||
                (string.IsNullOrWhiteSpace(txtEmailAC.Text.Trim()) && chkEmailAC.Checked)
           )
        {
            lblErrorMessage.Text = "ERROR: Please enter a valid email address.";
            mpeNewActivationCode.Show();
        }
        else
        {
            //string usersName = string.Concat(txtFirstName.Text.Trim() + " " + txtLastName.Text.Trim());
            string usersName = this.UserName;
            bool emailAttempted = false;
            bool emailSuccessfullySent = false;

            mpeNewActivationCode.Hide();

            if (chkEmailAC.Checked)
            {
                emailAttempted = true;
                emailSuccessfullySent = sendActivationCode(usersName);

                if (!chkPrintAC.Checked)
                {
                    if (emailSuccessfullySent)
                    {
                        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS, new MessageModel(Server.UrlEncode(this.finalMessage + " Activation code sent to " + txtEmailAC.Text.Trim() + "."))));
                    }
                    else
                    {
                        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS, new MessageModel(Server.UrlEncode(this.finalMessage + " However, there was an error in emailing the Activation Code. Please provide this Activation Code to the user. Activation Code = " + this.activationCode), MessageIcon.Information)));
                    }
                }
            }

            if (chkPrintAC.Checked)
            {
                printActivationCode(usersName, emailAttempted, emailSuccessfullySent);
            }
        }
    }

    protected void gridUsers_PageIndexChanged(object sender, GridPageChangedEventArgs e)
    {
        gridUsers.CurrentPageIndex = e.NewPageIndex;
    }

    protected void gridUsers_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        loadUsers();
    }

    protected void rblSearch_SelectedIndexChanged(object sender, EventArgs e)
    {
        gridUsers.CurrentPageIndex = 0;
        loadUsers();
        gridUsers.Rebind();
    }
    protected void btnUserSearch_Click(object sender, EventArgs e)
    {
         gridUsers.CurrentPageIndex = 0;
         loadUsers();
         gridUsers.Rebind();
    }

        #endregion

        #region Custom Methods

    private void loadUsers()
    {
        ePrescribeSvc.Status myStatus;
        Enum.TryParse(rblSearch.SelectedValue, out myStatus);
        ePrescribeSvc.GetUsersResponse response = new ePrescribeSvc.GetUsersResponse();
        response = EPSBroker.GetUsers(base.ShieldInternalTenantID, txtFirstName.Text, txtLastName.Text, myStatus, gridUsers.PageSize, gridUsers.CurrentPageIndex + 1, Session["AHSAccountID"].ToString(), false, base.DBID);

        if (response.Success || response.Messages.Length == 0)
        {
            IEnumerable<UsersResults> queryUserResults =
            from users in response.RxUsers.AsEnumerable()
            select new UsersResults {
                UserID = users.UserID,
                UserName = users.UserName,
                ShieldStatus = users.ShieldStatus,
                ShieldProfileName = users.ShieldProfileName,
                ShieldProfileUserID = users.ShieldProfileUserID,
                Email = users.Email,
                FirstName = users.FirstName,
                LastName = users.LastName, 
                StatusDescription = users.StatusDescription,
                LoginID = users.LoginID,
                Active = users.Active
            };
                    
            gridUsers.VirtualItemCount = response.pageCount;
            gridUsers.DataSource = queryUserResults.ToList();
            //gridUsers.Rebind();
        }
        else
        {
            ucError.Visible = true;
            ucError.MessageText = "Error retrieving list of users.";
            ucError.Icon = Controls_Message.MessageType.ERROR;
        }
        
    }

    private void printActivationCode(string name, bool emailAttempted, bool emailSentSuccessfully)
    {
        StringBuilder sbRedirectTo = new StringBuilder();
        string message = null;

        sbRedirectTo.Append(Constants.PageNames.PDF_INPAGE);

        if (emailAttempted)
        {
            if (emailSentSuccessfully)
            {
                message = this.finalMessage + " Activation code printed and email sent to " + txtEmailAC.Text.Trim() + ".";
            }
            else
            {
                message = this.finalMessage + " The Activation code was printed, however there was an error in emailing it. Please provide this Activation Code to the user. Activation Code = " + this.activationCode;
            }
        }
        else
        {
            message = this.finalMessage + " Activation code printed.";
        }

        if (Request.QueryString["To"] != null && Request.QueryString["To"].ToString() != string.Empty)
        {
            sbRedirectTo.Append("?To=").Append(Request.QueryString["To"].ToString()).Append("&Message=").Append(Server.UrlEncode(message));
        }
        else
        {
            sbRedirectTo.Append("?To=" + Constants.PageNames.EDIT_USERS).Append("&Message=").Append(Server.UrlEncode(message));
        }

        Session["ActivationCodeName"] = name;
        Session["ActivationCode"] = this.activationCode;
        Response.Redirect(Constants.PageNames.UrlForRedirection(sbRedirectTo.ToString()));
    }

    private bool sendActivationCode(string name)
    {
        bool successfullyEmailed = false;

        // create and send email, and redirect to corresponding page
        string xslPath = Server.MapPath("Scripts/ActivationCode.xslt");

        StringWriter writer = new StringWriter();
        XsltArgumentList xslArg = new XsltArgumentList();
        XslCompiledTransform transformContent = new XslCompiledTransform();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml("<Root><Name></Name><ActivationCodeURL></ActivationCodeURL><ActivationCode></ActivationCode></Root>");

        xslArg.AddParam("name", "", name);
		xslArg.AddParam("shieldActivationCodeURL", "", string.Concat(ConfigurationManager.AppSettings["appurl"], "/register/activateuser"));
        xslArg.AddParam("activationCode", "", this.activationCode);
        xslArg.AddParam("shieldHelpURL", "", ConfigKeys.ShieldHelpURL);

        transformContent.Load(xslPath);
        transformContent.Transform(xmlDoc, xslArg, writer);

        try
        {
            MailMessage mail = new MailMessage();
			MailAddress from = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailFromDisplayName"].ToString());

            mail.From = from;
            mail.Sender = from;
            mail.To.Add(txtEmailAC.Text.Trim());
            mail.Subject = ConfigurationManager.AppSettings["NewUserACEmailSubject"].ToString();
            mail.IsBodyHtml = true;
            mail.Body = writer.ToString();

            MailNotifier.SendMail(mail);

            successfullyEmailed = true;
        }
        catch (Exception ex)
        {
            Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Error sending activation code email: " + ex.ToString(), string.Empty, string.Empty, string.Empty, base.DBID);
            successfullyEmailed = false;
        }

        return successfullyEmailed;
    }

    private bool isValidEmail(string email)
    {
        bool isValid = false;
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(
            @"^([a-zA-Z0-9_\-\.\']+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        isValid = regex.IsMatch(email);

        return isValid;
    }

        #endregion

        #region Class
        [Serializable]
        private class UsersResults
        {
            public string UserID { get; set; }
            public string UserName { get; set; }
            public int ShieldStatus { get; set; }
            public string ShieldProfileName { get; set; }
            public int ShieldProfileUserID { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string LoginID { get; set; }
            public string StatusDescription { get; set; }
            public bool Active { get; set; }
        }

        #endregion

    protected void btnBack_OnClick(object sender, EventArgs e)
    {
        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
    }
}

}