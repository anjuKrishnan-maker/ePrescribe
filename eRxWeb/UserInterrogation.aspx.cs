using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Threading;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode;
using ConfigKeys = eRxWeb.AppCode.ConfigKeys;
using Provider = Allscripts.Impact.Provider;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using Allscripts.ePrescribe.ExtensionMethods;

namespace eRxWeb
{
    public partial class UserInterrogation : BasePage
    {
        private DataSet dsResults;
        private String IDologyID;
        private int IDQuestionCount = 0;
        private Guid GUIDToken;
        private String RedirectPage;
        private String LogMessage = String.Empty;
        private String sToken;
        protected void Page_Init(object sender, EventArgs e)
        {
            lnkDefaultStyleSheet.Href += "?version=" + SessionAppVersion;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }

            //This is the code for a Single-Click button
            //Prevents a user from submitting a page more than once.
            StringBuilder sb = new StringBuilder();
            //This forces the page validation, if any, to execute.
            sb.Append("if (typeof(Page_ClientValidate) == 'function') { ");
            sb.Append("if (Page_ClientValidate() == false) { return false; }} ");
            //Changes the text of the button. Gives the user a processing message.
            sb.Append("this.value = 'Processing...';");
            //Prevent the button from being pressed a second time.
            sb.Append("this.disabled = true;");
            //Forces the page to postback.
            sb.Append(ClientScript.GetPostBackEventReference(btnNext, string.Empty));
            sb.Append(";");
            btnNext.Attributes.Add("onclick", sb.ToString());
            Session["ProductName"] = "Veradigm ePrescribe™";
            lblProductName.Text = Session["ProductName"].ToString();
            if (Session["PartnerGUID"] != null)
            {
                lblStepNo.Text = "(Step 3 of 3)";
            }
            int TryCnt = 0;
            int MaxAttempts = 2;
            bool bRedirect = false;

            LoadPageVars();

            if (sToken.Length == 0)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Verifing Data - {0}", LogMessage));
                Session["ErrorNumber"] = 1002;
                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, Guid.Empty));
            }
            else
            {
                GUIDToken = new Guid(sToken);

                if (!Page.IsPostBack)
                {
                    IDologyClass IDVerify;
                    IDVerify = new IDologyClass(new AppConfig(), new ConfigKeys());

                    timer.Interval = int.Parse(ConfigurationManager.AppSettings["IDVerifyTimeoutInMinutes"].ToString()) * 60000;

                    if (Session["TryCount"] != null)
                    {
                        TryCnt = 1;
                        int.TryParse(Session["TryCount"].ToString(), out TryCnt);
                        Session["TryCount"] = TryCnt;
                    }

                    try
                    {
                        IDVerify.FirstName = Session["IDFirstName"].ToString();
                        IDVerify.LastName = Session["IDLastName"].ToString();
                        IDVerify.Address = Session["IDHomeAddress"].ToString();
                        IDVerify.City = Session["IDCity"].ToString();
                        IDVerify.State = Session["IDState"].ToString();
                        IDVerify.Zip = Session["IDZIP"].ToString();
                        IDVerify.DOBYear = Session["IDYear"].ToString();
                        IDVerify.L4SSN = Session["IDSSN"].ToString();
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Getting IDology Questions for: {0}", IDVerify.LastName));
                        dsResults = IDVerify.GetIDologyQuestions(GUIDToken);


                        //New user
                        if (PageState[Constants.SessionVariables.AddressVerifyRetryCount] == null)
                        {
                            if (!IDVerify.IsAddressValidatedByIDology)
                            {
                                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, Guid.Empty));
                                //If not validated by IDology no need to update DB as status =0 (NotVerified) is default
                            }
                            else
                            {
                                //Update in DB with status = 1 (Verified)
                                Allscripts.ePrescribe.Data.Provider.UpdateHomeAddressCheckStatus(
                                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                                    Convert.ToInt32(HOME_ADDRESS_CHECK_STATUS.VERIFIED),
                                    DBID
                                    );
                                //Update session variable too
                                PageState[Constants.SessionVariables.HomeAddressCheckStatus] = HOME_ADDRESS_CHECK_STATUS.VERIFIED;
                            }
                        }
                        //Existing user verifying home address
                        else 
                        {
                            CheckAddressVerifyForExistingUser(IDVerify.IsAddressValidatedByIDology);
                        }

                        if (ValidDataSets(ref dsResults))
                        {
                            Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("IDVerify.hasFatalError: {0} - (IDVerify.isValid: {1} - if(Result) {2}", IDVerify.hasFatalError, (IDVerify.isValid), (IDVerify.hasFatalError || !(IDVerify.isValid))));
                            Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Max Attempts: {0} - Try Count: {1}", MaxAttempts, TryCnt));
                            if (IDVerify.hasFatalError || !(IDVerify.isValid))
                            {
                                LogMessage = String.Format("Error Table: {0}; Valid ID: {1}.", IDVerify.ErrorTableNames, IDVerify.isValid);
                                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("IDology Fatal Error Received. - {0}", LogMessage));

                                if (TryCnt > MaxAttempts)
                                {
                                    Session["ErrorNumber"] = "1005";
                                    RedirectPage = Constants.PageNames.IDOLOGY_FINAL_ERROR;
                                    bRedirect = true;
                                }
                                else
                                {
                                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                                    Session["Verify"] = "Verify";
                                    bRedirect = true;
                                }
                            }
                            else
                            {
                                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("IDVerify.GetIDologyQuestions Table Count: {0}", dsResults.Tables.Count));
                                if (dsResults.Tables.Count > 0) //Were the data tables returned?
                                {
                                    IDologyID = dsResults.Tables["response"].Rows[0]["id-number"].ToString();
                                    Session["IDologyNumber"] = IDologyID;
                                    IDVerify.QueryID = IDologyID;
                                    Allscripts.ePrescribe.Data.User.UpdateIDologyID(GUIDToken, IDologyID, "");
                                    IDologyNumber.Text = IDologyID.ToString();
                                    if (!IDVerify.hasFatalError)
                                    {
                                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Loading Questions"));
                                        LoadChallenge();
                                    }
                                }
                                else
                                {
                                    Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("IDVerify.GetIDologyQuestions Table Count = 0"));
                                    Session["ErrorNumber"] = "1010";
                                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                                    bRedirect = true;
                                }
                            }
                        }
                        else
                        {
                            Session["ErrorNumber"] = 1004; //VELOCITY LIMIT EXCEEDED
                            RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                            bRedirect = true;
                        }
                    }
                    catch (ThreadAbortException ThreadEx)
                    {
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, "Thread - caught ThreadAbortException - resetting.");
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("ThreadAbortException (Page_Load): {0}", ThreadEx.Message));
                        Thread.ResetAbort();
                    }
                    catch (ApplicationException ex)
                    {
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Unexpected ApplicationException (Page_Load): {0}", ex.ToString()));
                        Session["ErrorNumber"] = "1020";
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    }
                    catch (Exception excep)
                    {
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Unexpected Exception (Page_Load): {0}", excep.Message));
                        Session["ErrorNumber"] = "1020";
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        bRedirect = true;
                    }

                    if (bRedirect)
                    {
                        Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
                    }
                }
            }
        }

        private void CheckAddressVerifyForExistingUser(bool addressVerified)
        {
            if (!addressVerified)
            {
                var retryCount = PageState.Cast(Constants.SessionVariables.AddressVerifyRetryCount, 0) + 1;
                PageState[Constants.SessionVariables.AddressVerifyRetryCount] = retryCount;
                Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, $"Home address not verified, setting retry count to {retryCount}");
                Response.Redirect(Constants.PageNames.IDPROOFING_REQUIRED_INFO);
            }
            else
            {
                
                PageState[Constants.SessionVariables.HomeAddressCheckStatus] = Provider.UpdateHomeAddressCheckStatus(GUIDToken, HOME_ADDRESS_CHECK_STATUS.VERIFIED, Request.UserIpAddress(), Page.ToString(), DBID);
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
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            LoadPageVars();

            if (sToken.Length == 0)
            {
                Session["ErrorNumber"] = 1002;
                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, Guid.Empty));
            }
            else
            {
                GUIDToken = new Guid(sToken);
                /// Save Answer
                try
                {
                    Session["AText1"] = rblQuestion1.SelectedValue;
                    Session["AText2"] = rblQuestion2.SelectedValue;
                    Session["AText3"] = rblQuestion3.SelectedValue;
                    Session["AText4"] = rblQuestion4.SelectedValue;
                    Session["AText5"] = rblQuestion5.SelectedValue;

                    Session["AType1"] = lblQ1Type.Text;
                    Session["AType2"] = lblQ2Type.Text;
                    Session["AType3"] = lblQ3Type.Text;
                    Session["AType4"] = lblQ4Type.Text;
                    Session["AType5"] = lblQ5Type.Text;
                    RedirectPage = Constants.PageNames.IDOLOGY_ANSWERS;
                }
                catch (Exception ex)
                {
                    Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Error Collecting Answers: {0}", ex.ToString()));
                    Session["ErrorNumber"] = "1012";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                }
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
            }
        }

        private void LoadChallenge()
        {
            try
            {
                DataView dvQuestion = new DataView(dsResults.Tables["Question"]);
                DataView dvAnswer = new DataView(dsResults.Tables["Answer"]);

                // The number of questions returned.
                IDQuestionCount = dvQuestion.Table.Rows.Count;

                if (IDQuestionCount >= 1)
                {
                    lblQuestion1.Text = String.Format("{0}.  {1}", 1, dvQuestion.Table.Rows[0]["prompt"].ToString());
                    lblQ1Type.Text = String.Format("{0}", dvQuestion.Table.Rows[0]["type"].ToString());
                    dvAnswer.RowFilter = "Question_ID=0";
                    rblQuestion1.DataSource = dvAnswer;
                    rblQuestion1.DataTextField = "Answer_Text";
                    rblQuestion1.DataValueField = "Answer_Text";
                    rblQuestion1.DataBind();
                }
                else
                {
                    panQ1.Visible = false;
                }

                if (IDQuestionCount >= 2)
                {
                    lblQuestion2.Text = String.Format("{0}.  {1}", 2, dvQuestion.Table.Rows[1]["prompt"].ToString());
                    lblQ2Type.Text = String.Format("{0}", dvQuestion.Table.Rows[1]["type"].ToString());
                    dvAnswer = new DataView(dsResults.Tables["Answer"]);
                    dvAnswer.RowFilter = "Question_ID=1";
                    rblQuestion2.DataSource = dvAnswer;
                    rblQuestion2.DataTextField = "Answer_Text";
                    rblQuestion2.DataValueField = "Answer_Text";
                    rblQuestion2.DataBind();
                }
                else
                {
                    panQ2.Visible = false;
                }

                if (IDQuestionCount >= 3)
                {
                    lblQuestion3.Text = String.Format("{0}.  {1}", 3, dvQuestion.Table.Rows[2]["prompt"].ToString());
                    lblQ3Type.Text = String.Format("{0}", dvQuestion.Table.Rows[2]["type"].ToString());
                    dvAnswer = new DataView(dsResults.Tables["Answer"]);
                    dvAnswer.RowFilter = "Question_ID=2";
                    rblQuestion3.DataSource = dvAnswer;
                    rblQuestion3.DataTextField = "Answer_Text";
                    rblQuestion3.DataValueField = "Answer_Text";
                    rblQuestion3.DataBind();
                }
                else
                {
                    panQ3.Visible = false;
                }

                if (IDQuestionCount >= 4)
                {
                    lblQuestion4.Text = String.Format("{0}.  {1}", 4, dvQuestion.Table.Rows[3]["prompt"].ToString());
                    lblQ4Type.Text = String.Format("{0}", dvQuestion.Table.Rows[3]["type"].ToString());
                    dvAnswer = new DataView(dsResults.Tables["Answer"]);
                    dvAnswer.RowFilter = "Question_ID=3";
                    rblQuestion4.DataSource = dvAnswer;
                    rblQuestion4.DataTextField = "Answer_Text";
                    rblQuestion4.DataValueField = "Answer_Text";
                    rblQuestion4.DataBind();
                }
                else
                {
                    panQ4.Visible = false;
                }

                if (IDQuestionCount >= 5)
                {
                    lblQuestion5.Text = String.Format("{0}.  {1}", 5, dvQuestion.Table.Rows[4]["prompt"].ToString());
                    lblQ5Type.Text = String.Format("{0}", dvQuestion.Table.Rows[4]["type"].ToString());
                    dvAnswer = new DataView(dsResults.Tables["Answer"]);
                    dvAnswer.RowFilter = "Question_ID=4";
                    rblQuestion5.DataSource = dvAnswer;
                    rblQuestion5.DataTextField = "Answer_Text";
                    rblQuestion5.DataValueField = "Answer_Text";
                    rblQuestion5.DataBind();
                }
                else
                {
                    panQ5.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Unexpected Exception: {0}", ex.ToString()));
                Session["ErrorNumber"] = "1020";
                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
            }
        }

        internal void LoadPageVars()
        {
            try
            {
                sToken = base.SessionUserID;
            }
            catch (Exception ex)
            {
                LogMessage = String.Format("Missing GUID - {0}", ex.ToString());
                sToken = String.Empty;
            }
        }

        private bool ValidDataSets(ref DataSet dsSearch)
        {
            bool ValidEntry = true;
            LogMessage = string.Empty;
            foreach (DataTable d in dsSearch.Tables)
            {
                if (d.ToString() == "error")
                {
                    ValidEntry = false;
                    LogMessage += d.ToString();
                }
                if (d.ToString() == "failed")
                {
                    ValidEntry = false;
                    LogMessage += d.ToString();
                }
                if (d.ToString() == "restriction")
                {
                    ValidEntry = false;
                    LogMessage += d.ToString();
                }
                if (d.ToString() == "velocity-results")
                {
                    ValidEntry = false;
                    LogMessage += d.ToString();
                }
                if (d.ToString() == "velocity-result")
                {
                    ValidEntry = false;
                    LogMessage += d.ToString();
                }
            }
            if (!ValidEntry)
            {
                LogMessage = string.Format("{0} Table(s) Found.", LogMessage);
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, LogMessage);
            }
            return ValidEntry;
        }

        protected void timer_Tick(object sender, EventArgs e)
        {
            Session["ErrorNumber"] = 1021;
            Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, "Identity question timeout (Current Timeout Setting = " + ConfigurationManager.AppSettings["IDVerifyTimeoutInMinutes"].ToString() + " mins)");
            Response.Redirect(String.Format("{0}?U={1}", Constants.PageNames.IDOLOGY_ERROR, GUIDToken.ToString()));
        }
    }
}