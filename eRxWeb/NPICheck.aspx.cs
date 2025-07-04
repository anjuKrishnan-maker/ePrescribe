using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class NPICheck : BasePage
    {
        private int _npiTryCnt;
        private int MaxAttempts = 2;
        private String sToken;
        private Guid GUIDToken;
        private bool isvalidNPI;

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

                if (base.CurrentUser != null)
                {
                    DataTable dtNPICheckDetails = Provider.GetNPICheckDetails(this.SessionUserID, base.DBID);
                    if (dtNPICheckDetails != null && dtNPICheckDetails.Rows.Count > 0)
                    {
                        try
                        {
                            if (!Convert.ToBoolean(dtNPICheckDetails.Rows[0]["IsManual"].ToString()) && !Convert.ToBoolean(dtNPICheckDetails.Rows[0]["IsSuccessful"].ToString()))
                            {
                                Response.Redirect(Constants.PageNames.MANUAL_NPI_CHECK_FORM);
                            }
                        }
                        catch (Exception ex)
                        {
                            sToken = this.SessionUserID.ToString();
                            GUIDToken = new Guid(sToken);
                            Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Exception Details: {0} -", ex.ToString()));
                        }
                    }

                    DataTable dtProviderDetails = Provider.LoadProviderDetailsForIDProofing(this.SessionUserID, base.SessionLicenseID, base.DBID);
                    if (dtProviderDetails != null && dtProviderDetails.Rows.Count > 0)
                    {
                        txtfirstname.Text = dtProviderDetails.Rows[0]["FirstName"].ToString();
                        txtlastname.Text = dtProviderDetails.Rows[0]["LastName"].ToString();
                        txtNPI.Text = dtProviderDetails.Rows[0]["NPI"].ToString();
                    }
                }
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if(isvalidNPI)
            {
                string providerFName = string.Empty;
                string providerLName = string.Empty;
                string providerNPI = string.Empty;
                bool npiVerified = false;
                int validateProviderNPI = 0;

                if (Session["NPICheck_Attempt"] != null && !string.IsNullOrEmpty(Session["NPICheck_Attempt"].ToString()))
                {
                    _npiTryCnt = 0;
                    int.TryParse(Session["NPICheck_Attempt"].ToString(), out _npiTryCnt);
                    _npiTryCnt++;
                }
                else
                {
                    _npiTryCnt = 1;
                }

                Session["NPICheck_Attempt"] = _npiTryCnt;
                Session["IDFirstName"] = txtfirstname.Text.Trim();
                Session["IDLastName"] = txtlastname.Text.Trim();
                Session["IDNPI"] = txtNPI.Text.Trim();

                providerFName = Session["IDFirstName"].ToString();
                providerLName = Session["IDLastName"].ToString();
                providerNPI = Session["IDNPI"].ToString();
                sToken = this.SessionUserID.ToString();
                GUIDToken = new Guid(sToken);
                Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Max Attempts: {0} - Try Count: {1}", MaxAttempts, _npiTryCnt));
                validateProviderNPI = Provider.ValidateProviderNPI(providerFName, providerLName, providerNPI);

                if (validateProviderNPI > 0)
                {
                    npiVerified = true;
                }

                if (npiVerified)
                {
                    Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("NPI Checking Successful in {0} attempts.", _npiTryCnt));
                    Response.Redirect(Constants.PageNames.IDOLOGY_SUCCESSFUL + "?From=NPICheck");
                }
                else
                {
                    Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("NPI Checking Failed.Max Attempts: {0} - Try Count: {1}.", MaxAttempts, _npiTryCnt));
                    Response.Redirect(Constants.PageNames.NPI_ERROR);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.LOGOUT);
        }

        protected void setNpiStatus(ref ServerValidateEventArgs args, bool npiStatus)
        {
            args.IsValid = npiStatus;
            isvalidNPI = npiStatus;
        }
        protected void cvNPI_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string NPI = txtNPI.Text.Trim();

            if (NPI.Length == 0)
            {
                setNpiStatus(ref args, false);
                return;
            }

            if (NPI.Length != 10 && NPI.Length > 0) // Must be exactly 10 characters
            {
                setNpiStatus(ref args, false);
            }

                Int64 dummy = 0;
            if (!Int64.TryParse(NPI, out dummy))    // Must be all numeric	
            {
                setNpiStatus(ref args, false);
            }


            // example#   1234567893
            // 1.   Prepend 80840 to get 808401234567893
            // 2.   Starting with the 9 (ignore last digit - 3), double every 2nd digit moving to the left. 
            //      Sum up the individual digits from the numbers that were doubled e.g. 9 doubled is 18 and 1 + 8 = 9
            // 3.   Sum up the other digits (that were not doubled) except the last one and add this sum to the previous sum.
            // 4.   Round the total sum up to the nearest multiple of 10 and subtract the unrounded sum from that.
            // 5.   Compare the difference of Rounded sum to UnRounded sum with the last digit

            try
            {
                NPI = "80840" + NPI;    //special digits prepended for Check Algorithm

                char[] NPIChars = NPI.ToCharArray();
                int[] NPIints = new int[15];

                // Convert all the chars to numbers
                for (int n = 0; n < 15; n++)
                    NPIints[n] = NPIChars[n] - '0';

                // double alternating digits starting at n-1 position going backwards
                string tmpDoubled = "";
                for (int n = 13; n >= 1; n -= 2)
                {
                    NPIints[n] = NPIints[n] * 2;
                    tmpDoubled += NPIints[n].ToString();
                }

                //Sum up each individual digit in the set that was doubled
                int Doubled = 0;
                int RoundedUp = 0;
                foreach (char num in tmpDoubled)
                {
                    Doubled += Convert.ToInt32(num - '0');
                }

                //Sum up the other digits that were not doubled (except the last one) and add to Doubled
                for (int n = 12; n >= 0; n -= 2)
                    Doubled += NPIints[n];

                // Round up to the nearest multiple of 10
                // WRONG: RoundedUp = Convert.ToInt32(Math.Round((Doubled / 10.0), MidpointRounding.AwayFromZero) * 10);
                int doubledTemp = Doubled + 10;
                RoundedUp = doubledTemp - (doubledTemp % 10);   // bug fix 2-27-2008

                // Compare the last digit with difference of RoundedUp and Doubled
                int diff = RoundedUp - Doubled;

                // WRONG: Didn't handle case of NPI ending in zero
                //if (NPIints[14] == diff)
                if (NPIints[14] == (diff % 10))
                {
                    setNpiStatus(ref args, true);
                }

                else
                {
                    setNpiStatus(ref args, false);
                }
            }
            catch
            {
                setNpiStatus(ref args, false);
            }
        }
    }
}