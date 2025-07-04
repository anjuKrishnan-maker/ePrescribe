using System;
using System.Data;
using System.Text;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.AppCode;

namespace eRxWeb
{
    public partial class UserChallenge : BasePage
    {

        private DataSet dsResults;
        private int IDQuestionCount = 0;
        private Guid GUIDToken;
        private String RedirectPage;
        private String LogMessage = String.Empty;
        private String sToken;
        private bool bMoveOn;
        private const String AllCorrect = "result.challenge.0.incorrect";
        private const String Missed1Question = "result.challenge.1.incorrect";
        private const String Missed2Question = "result.challenge.2.incorrect";

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

            lblProductName.Text = Session["ProductName"].ToString();
            if (Session["PartnerGUID"] != null)
            {
                lblStepNo.Text = "(Step 3 of 3)";
            }

            bool bRedirect = false;
            LoadPageVars();

            if (sToken.Length == 0)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("UserChallenge - {0}", LogMessage));
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
                    IDVerify = new IDologyClass(new AppConfig(), new AppCode.ConfigKeys());
                    try
                    {
                        DataSet ds = Allscripts.ePrescribe.Data.User.GetIDologyID(GUIDToken, ConnectionStringPointer.REGISTRATION_DB);
                        if (ds.Tables.Count > 0)
                        {
                            IDVerify.QueryID = Session["IDologyNumber"] != null ? Session["IDologyNumber"].ToString() : String.Empty;

                            //Call IDology API to get Challenge Questions
                            dsResults = IDVerify.GetIDologyChallengeQuestions(GUIDToken);

                            if (IDVerify.hasFatalError)
                            {
                                LogMessage = "Ingenix Record Match Found";
                                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("UserChallenge - {0}", LogMessage));
                                Session["ErrorNumber"] = "1001";
                                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                                bRedirect = true;
                            }
                            else
                            {
                                if (dsResults.Tables.Count > 0) //Were the data tables returned?
                                {
                                    if (dsResults.Tables["Question"] == null)
                                    {
                                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, "Question Table is Not Present");
                                        Session["ErrorNumber"] = "1009";
                                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                                        bRedirect = true;
                                    }
                                    else
                                    {
                                        IDologyNumber.Text = IDVerify.QueryID;
                                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Loading Challenge Questions"));
                                        LoadChallenge();
                                    }
                                }
                                else
                                {
                                    Session["ErrorNumber"] = "1001";
                                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                                    bRedirect = true;
                                }
                            }
                        }
                        else
                        {
                            //no tables returned.
                            Session["ErrorNumber"] = "1010";
                            RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                            bRedirect = true;
                        }
                    }
                    catch (ApplicationException ex)
                    {
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Unexpected ApplicationException: {0}", ex.ToString()));
                        Session["ErrorNumber"] = "1020";
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        bRedirect = true;
                    }
                    catch (Exception excep)
                    {
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Unexpected Exception: {0}", excep.ToString()));
                        Session["ErrorNumber"] = "1020";
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        bRedirect = true;
                    }
                    if (bRedirect)
                        Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
                }
            }
        }
                
        protected void btnNext_Click(object sender, EventArgs e)
        {
            LoadPageVars();

            if (sToken.Length == 0)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("UserChallenge - {0}", LogMessage));
                Session["ErrorNumber"] = 1002;
                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, Guid.Empty));
            }
            else
            {
                GUIDToken = new Guid(sToken);

                try
                {
                    /// Save Answer
                    Session["AText1"] = rblQuestion1.SelectedValue;
                    Session["AText2"] = rblQuestion2.SelectedValue;

                    Session["AType1"] = lblQ1Type.Text;
                    Session["AType2"] = lblQ2Type.Text;
                    GradeResponses(GetAnswers());
                }
                catch (Exception ex)
                {
                    Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Error Collecting Challenge Answers: {0}", ex.ToString()));
                    Session["ErrorNumber"] = "1013";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                }
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
            }
        }

        private void GradeResponses(DataSet dsResults)
        {
            switch (dsResults.Tables["idliveq-challenge-result"].Rows[0]["key"].ToString())
            {
                case (AllCorrect):
                    bMoveOn = true;
                    LogMessage = "All Challenge questions answered correctly.";
                    RedirectPage = Constants.PageNames.IDOLOGY_SUCCESSFUL;
                    break;
                case (Missed1Question):
                    bMoveOn = true;
                    LogMessage = "Missed one Challenge question.";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    break;
                case (Missed2Question):
                    bMoveOn = true;
                    LogMessage = "Missed two Challenge questions.";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    break;
                default:
                    bMoveOn = false;
                    LogMessage = "Error. Challenge Default reached on switch";
                    break;
            }
            Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, LogMessage);
            if (bMoveOn)
            {
                //Fortify fix, sending proper Redirection Url and Query String.
                RedirectUser(RedirectPage, GUIDToken);
                
            }
        }

        private void RedirectUser(string RedirectPage, Guid GUIDToken)
        {
            string UrlForRedirection = Constants.PageNames.UrlForRedirection(RedirectPage);
            string userToken = UserTokenParse(GUIDToken);

            if (!string.IsNullOrEmpty(UrlForRedirection))
            {
                Response.Redirect(String.Format("{0}?U={1}", UrlForRedirection, userToken));
            }
            else
            {
                Response.Redirect(Constants.PageNames.IDOLOGY_ERROR);
            }
        }

        private string UserTokenParse(Guid GUIDToken)
        {
            Guid newGuid;
            string userToken = string.Empty;
            if (Guid.TryParse(GUIDToken.ToString(), out newGuid))
            {
                userToken = newGuid.ToString();
            }

            return Microsoft.Security.Application.Encoder.UrlEncode(userToken);
        }

        private DataSet GetAnswers()
        {
            DataSet ds;
            IDologyClass IDology;
            IDology = new IDologyClass(new AppConfig(), new AppCode.ConfigKeys());
            String[] aText =
               new String[2]
            {
               Session["AText1"].ToString(), Session["AText2"].ToString()
            };
            String[] aType =
               new String[2]
            {
               Session["AType1"].ToString(), Session["AType2"].ToString()
            };
            string IDologyID = Session["IDologyNumber"].ToString();
            IDology.QueryID = IDologyID;
            //Call IDology API to evaluate the User's Answers
            ds = IDology.SendIDologyChallengeAnswers(ref aType, ref aText);
            return ds;
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
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
        }

        internal void LoadPageVars()
        {
            try
            {
                sToken = Request.QueryString["U"].ToString();
            }
            catch (Exception ex)
            {
                LogMessage = String.Format("Missing GUID - {0}", ex.Message);
                sToken = String.Empty;
            }
        }
    }
}