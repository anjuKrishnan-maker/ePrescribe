using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Text;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class Help_InterfaceRequest : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Form.Attributes.Add("autocomplete", "off");                
        if (!IsPostBack)
        {
            ((HelpMasterPageNew)this.Master).CurrentPage = HelpMasterPageNew.HelpPage.INTERFACE_REQUEST;

            if (!string.IsNullOrEmpty(base.SessionLicenseID))
            {
                licenseID.Text = base.SessionLicenseID;
            }
            else
            {
                licenseID.Text = string.Empty;
            }

            bool accountNumberNeeded = false;
            if (Request.QueryString["interfaceid"] != null)
            {
                ((HelpMasterPageNew)this.Master).LogOutLandingPage = "~/Help/" + Constants.PageNames.HELP_INTERFACE_REQUEST + "?interfaceid=" + Request.QueryString["interfaceid"].ToString();
                ((HelpMasterPageNew)this.Master).LogInLandingPage = "~/Help/" + Constants.PageNames.HELP_INTERFACE_REQUEST + "?interfaceid=" + Request.QueryString["interfaceid"].ToString();
                int interfaceID = Convert.ToInt32(Request.QueryString["interfaceid"].ToString());
                DataTable dt = SystemConfig.GetInterfaceSystemByID(interfaceID);

                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    decimal tierOneBase = -1;
                    decimal tierOneMonth = -1;
                    decimal tierTwoBase = -1;
                    decimal tierTwoMonth = -1;                 

                    decimal.TryParse(dt.Rows[0]["TierOneBase"].ToString(), out tierOneBase);
                    decimal.TryParse(dt.Rows[0]["TierOneMonth"].ToString(), out tierOneMonth);
                    decimal.TryParse(dt.Rows[0]["TierTwoBase"].ToString(), out tierTwoBase);
                    decimal.TryParse(dt.Rows[0]["TierTwoMonth"].ToString(), out tierTwoMonth);

                    if (!string.IsNullOrEmpty(base.SessionLicense.EnterpriseClient.InterfaceBillingParty))
                    {
                        tierOneBase = 0;
                        tierOneMonth = 0;
                        tierTwoBase = 0;
                        tierTwoMonth = 0;
                    }

                    string sTierOneBase = getPriceDisplay(tierOneBase, true);
                    string sTierOneMonth = getPriceDisplay(tierOneMonth, false);
                    string sTierTwoBase = getPriceDisplay(tierTwoBase, true);
                    string sTierTwoMonth = getPriceDisplay(tierTwoMonth, false);
                   

                    lblSystemName.InnerText = "Interface Request - " + dt.Rows[0]["Name"].ToString();

                    accountNumberNeeded = dt.Rows[0]["Supplier"].ToString().Trim().ToLower() == "allscripts"
                                            || dt.Rows[0]["Supplier"].ToString().Trim().ToLower() == "advancedmd"
                                            || dt.Rows[0]["Supplier"].ToString().Trim().ToLower() == "misys"
                                            || dt.Rows[0]["Supplier"].ToString().Trim().ToLower() == "misys divisional";

                    rfvAccountID.Enabled = accountNumberNeeded;
                    rfvAccountID.Visible = accountNumberNeeded;
                    txtPMSAccountNumber.Visible = accountNumberNeeded;
                    pmsRequired.Visible = accountNumberNeeded;
                    pmsLabel.Visible = accountNumberNeeded;
                  
                }
            }

            if (accountNumberNeeded)
            {
                txtPMSAccountNumber.Focus();
            }
            else
            {
                txtFirstName.Focus();
            }
                    
        }
    }

    protected void btCont_Click(object sender, EventArgs e)
    {
        string interfaceSupplier = "";
        string interfaceType = "";
        string licenseID = "";
        string accountID = "";
        string siteName = "";
        string siteAddress1 = "";
        string siteAddress2 = "";
        string siteCity = "";
        string siteState = "";
        string siteZip = "";
        string firstName = txtFirstName.Text;
        string lastName = txtLastName.Text;
        string title = string.Empty;
        string phone = txtPhone.Text;
        string email = txtEmail.Text;
        string systemName = string.Empty;
        string allscriptsClientID = string.Empty;
        string pmsAccountNumber = string.Empty;
        int interfaceID = 0;
        if (Request.QueryString["interfaceid"] != null)
        {
            interfaceID = Convert.ToInt32(Request.QueryString["interfaceid"].ToString());
            DataTable dt = Allscripts.Impact.SystemConfig.GetInterfaceSystemByID(interfaceID);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                interfaceSupplier = dt.Rows[0]["Supplier"].ToString().Trim();
                interfaceType = dt.Rows[0]["Type"].ToString().Trim();
                systemName = dt.Rows[0]["Name"].ToString();

                if (dt.Rows[0]["Supplier"].ToString().Trim().ToLower() == "allscripts")
                {
                    allscriptsClientID = txtPMSAccountNumber.Text;
                }
                else
                {
                    pmsAccountNumber = txtPMSAccountNumber.Text;
                }
            }
        }

        if (SessionLicenseID != null)
        {
            DataTable dtLicense = ApplicationLicense.Load(SessionLicenseID, base.DBID).Tables[0];

            licenseID = dtLicense.Rows[0]["LicenseID"].ToString();
            accountID = dtLicense.Rows[0]["AccountID"].ToString();
            siteName = dtLicense.Rows[0]["PracticeName"].ToString();
            siteAddress1 = dtLicense.Rows[0]["Address1"].ToString();
            siteAddress2 = dtLicense.Rows[0]["Address2"].ToString();
            siteCity = dtLicense.Rows[0]["City"].ToString();
            siteState = dtLicense.Rows[0]["State"].ToString();
            siteZip = dtLicense.Rows[0]["ZipCode"].ToString();
        }

        //now check if this is a HealthVision partner license. If so, they shouldn't be adding an interface request.
        DataTable dtHVPartner = SystemConfig.PartnerLicenseSiteCheck(licenseID, "9A21308B-B164-4040-A760-F15D67F0FFEF", base.DBID);
        if (dtHVPartner != null && dtHVPartner.Rows != null && dtHVPartner.Rows.Count > 0)
        {
            //this is a HV license, so don't let them through!
            ucMessage.Visible = true;
            ucMessage.MessageText = "Due to the agreement between HealthVision and Allscripts, your license is not eligible to order a PMS interface.";
            return;
        }

        //don't save the keynote interface requests
        if (!licenseID.Equals("975DF2CB-EEC7-46F8-9C15-BB14064505BE", StringComparison.OrdinalIgnoreCase))
        {
            SystemConfig.SaveInterfaceRequest(
                licenseID,
                accountID,
                siteName,
                siteAddress1,
                siteAddress2,
                siteCity,
                siteState,
                siteZip,
                firstName,
                lastName,
                title,
                phone,
                email,
                systemName,
                allscriptsClientID,
                pmsAccountNumber,
                interfaceID,
                licenseID,
                ConnectionStringPointer.REGISTRATION_DB,
                base.DBID);
        }       

        //don't send emails for the Keynote test license
        if (!licenseID.Equals("975DF2CB-EEC7-46F8-9C15-BB14064505BE", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                SendEmail(licenseID, accountID, siteName, siteAddress1, siteAddress2, siteCity, siteState, siteZip, firstName, lastName, title,
                    email, phone, systemName, allscriptsClientID, pmsAccountNumber, interfaceSupplier);
            }
            catch (Exception)
            {
            }

            try
            {
                SendEmailToClient(licenseID, accountID, siteName, siteAddress1, siteAddress2, siteCity, siteState, siteZip, firstName, lastName, title,
                    email, phone, systemName, allscriptsClientID, pmsAccountNumber, interfaceSupplier);
            }
            catch (Exception)
            {
            }
        }

        Response.Redirect(Constants.PageNames.HELP_INTERFACE_REQUEST_CONFIRMATION);
    }

    private void SendEmailToClient(string licenseID, string accountID, string siteName, string siteAddress1, string siteAddress2, string siteCity, string siteState,
            string siteZip, string firstName, string lastName, string title, string email, string phone, string pmsName, string allscriptsClientID, string accountNumber,
            string interfaceSupplier)
    {

        StringBuilder emailBody = new StringBuilder();

        emailBody.Append("Dear ");
        emailBody.Append(firstName);
        emailBody.Append(" ");
        emailBody.Append(lastName);
        emailBody.Append(":<br /><br />");

        //Added misys divisional supplier in the condition.
        if (interfaceSupplier.ToLower() == "allscripts" || interfaceSupplier.ToLower() == "misys" || interfaceSupplier.ToLower() == "misys divisional")
        {
            emailBody.Append(System.Configuration.ConfigurationManager.AppSettings["AllscriptsInterfaceEmailMessage"]);
            emailBody.Append("<br /><br />");
        }
        else
        {
            emailBody.Append("Thank you for submitting an interface request for ePrescribe. A representative from the interface vendor will be contacting you shortly.<br /><br />");
            emailBody.Append("The details of your request are below:<br /><br />");
        }

        emailBody.Append("Account ID: ");
        emailBody.Append(accountID);
        emailBody.Append("<br />");

        emailBody.Append("Site Name: ");
        emailBody.Append(siteName);
        emailBody.Append("<br />");

        emailBody.Append("Address: ");
        emailBody.Append(siteAddress1);
        emailBody.Append("<br />");

        emailBody.Append("City: ");
        emailBody.Append(siteCity);
        emailBody.Append("<br />");

        emailBody.Append("State: ");
        emailBody.Append(siteState);
        emailBody.Append("<br />");

        emailBody.Append("ZIP: ");
        emailBody.Append(siteZip);
        emailBody.Append("<br />");

        emailBody.Append("First Name: ");
        emailBody.Append(firstName);
        emailBody.Append("<br />");


        emailBody.Append("Last Name: ");
        emailBody.Append(lastName);
        emailBody.Append("<br />");

        emailBody.Append("Title: ");
        emailBody.Append(title);
        emailBody.Append("<br />");

        emailBody.Append("Email: ");
        emailBody.Append(email);
        emailBody.Append("<br />");

        emailBody.Append("Phone: ");
        emailBody.Append(phone);
        emailBody.Append("<br />");

        emailBody.Append("PMS: ");
        emailBody.Append(pmsName);
        emailBody.Append("<br />");

        if (allscriptsClientID != null && allscriptsClientID != "")
        {
            emailBody.Append("Veradigm Client ID: ");
            emailBody.Append(allscriptsClientID);
            emailBody.Append("<br />");
        }

        emailBody.Append("Account Number: ");
        emailBody.Append(accountNumber);
        emailBody.Append("<br />");

        MailMessage objEmail = new MailMessage();
        objEmail.To.Add(email);
        objEmail.From = new MailAddress(ConfigurationManager.AppSettings["FromInterfaceEmail"]);
        objEmail.Subject = ConfigurationManager.AppSettings["InterfaceEmailSubject"];
        objEmail.IsBodyHtml = true;
        objEmail.Body = emailBody.ToString();

        MailNotifier.SendMail(objEmail);
    }

    private void SendEmail(string licenseID, string accountID, string siteName, string siteAddress1, string siteAddress2, string siteCity, string siteState,
        string siteZip, string firstName, string lastName, string title, string email, string phone, string pmsName, string allscriptsClientID, string accountNumber,
        string interfaceSupplier)
    {

        StringBuilder emailBody = new StringBuilder();

        emailBody.Append("LicenseID: ");
        emailBody.Append(licenseID);
        emailBody.Append("<br />");

        emailBody.Append("AccountID: ");
        emailBody.Append(accountID);
        emailBody.Append("<br />");

        emailBody.Append("Site Name: ");
        emailBody.Append(siteName);
        emailBody.Append("<br />");

        string deluxe = "No";
        if (    base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On
            ||  base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
        {
            deluxe = "Yes";
        }

        emailBody.Append("10% Discount: ");
        emailBody.Append(deluxe);
        emailBody.Append("<br />");

        emailBody.Append("Address 1: ");
        emailBody.Append(siteAddress1);
        emailBody.Append("<br />");

        emailBody.Append("City: ");
        emailBody.Append(siteCity);
        emailBody.Append("<br />");

        emailBody.Append("State: ");
        emailBody.Append(siteState);
        emailBody.Append("<br />");

        emailBody.Append("ZIP Code: ");
        emailBody.Append(siteZip);
        emailBody.Append("<br />");

        emailBody.Append("First Name: ");
        emailBody.Append(firstName);
        emailBody.Append("<br />");


        emailBody.Append("Last Name: ");
        emailBody.Append(lastName);
        emailBody.Append("<br />");

        emailBody.Append("Title: ");
        emailBody.Append(title);
        emailBody.Append("<br />");

        emailBody.Append("Email: ");
        emailBody.Append(email);
        emailBody.Append("<br />");

        emailBody.Append("Phone: ");
        emailBody.Append(phone);
        emailBody.Append("<br />");

        emailBody.Append("PMS: ");
        emailBody.Append(pmsName);
        emailBody.Append("<br />");

        if (interfaceSupplier.ToLower() == "allscripts")
        {
            emailBody.Append("Veradigm Client ID: ");
            emailBody.Append(allscriptsClientID);
            emailBody.Append("<br />");
        }
        //Added misys divisional supplier in the condition.
        else if (interfaceSupplier.ToLower() == "misys" || interfaceSupplier.ToLower() == "misys divisional")
        {
            emailBody.Append("Misys Client ID: ");
            emailBody.Append(accountNumber);
            emailBody.Append("<br />");
        }
        else
        {
            emailBody.Append("Account Number: ");
            emailBody.Append(accountNumber);
            emailBody.Append("<br />");
        }

        emailBody.Append("Registered: ");
        emailBody.Append("YES");
        emailBody.Append("<br />");

        emailBody.Append("IP: ");
        emailBody.Append(Request.UserHostName);
        emailBody.Append("<br />");

        MailMessage objEmail = new MailMessage();
       // objEmail.To.Add(email);
        objEmail.To.Add(ConfigurationManager.AppSettings["ToInterfaceEmailAllscripts"]);
        objEmail.Bcc.Add(ConfigurationManager.AppSettings["BccInterfaceEmailAllscripts"]);
        objEmail.From = new MailAddress(ConfigurationManager.AppSettings["FromInterfaceEmail"]);
        objEmail.Subject = ConfigurationManager.AppSettings["InterfaceEmailSubject"];
        objEmail.IsBodyHtml = true;
        objEmail.Body = emailBody.ToString();
        

        MailNotifier.SendMail(objEmail);
    }

    private string getPriceDisplay(decimal price, bool isBase)
    {
        if (price < 0)
        {
            if (isBase)
            {
                return "Setup fees vary";
            }
            else
            {
                return "Monthly fees vary";
            }
        }
        else if (price == 0)
        {
            if (isBase)
            {
                return "No Setup Fee";
            }
            else
            {
                return "No Monthly Fees";
            }
        }
        else
        {
            if (isBase)
            {
                return "$" + price.ToString("0.00") + " Setup";
            }
            else
            {
                return "$" + price.ToString("0.00") + " a month";
            }
        }
    }
}

}

