using System;
using System.Data;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using ConfigKeys = Allscripts.Impact.ConfigKeys;

namespace eRxWeb
{
    public partial class IDologyAnswers : BasePage
    {
        private const String AllCorrect = "result.questions.0.incorrect";
        private const String Missed1Question = "result.questions.1.incorrect";
        private const String Missed2Question = "result.questions.2.incorrect";
        private const String Missed3Question = "result.questions.3.incorrect";
        private const String Missed4Question = "result.questions.4.incorrect";
        private const String Missed5Question = "result.questions.5.incorrect";
        private String IDologyID;
        private String[] aText;
        private String[] aType;
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
            if (base.CurrentUser == null)
            {
                Response.Redirect(Constants.PageNames.LOGOUT);
            }
            LoadPageVars();

            if (!IsPostBack)
            {
                RedirectPage = ValidPageRequest();
                if (RedirectPage.Length != 0)
                    Response.Redirect(RedirectPage);
            }

            if (sToken.Length == 0)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Answers - {0}", LogMessage));
                Session["ErrorNumber"] = 1002;
                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, Guid.Empty));
            }
            else
            {
                GUIDToken = new Guid(sToken);
                DataSet dsResults;
                if (!Page.IsPostBack)
                {
                    dsResults = GetAnswers();
                    if (dsResults.Tables.Count > 0)
                    {
                        if (ValidDataSets(dsResults))
                        {
                            GradeResponses(dsResults);
                        }
                        else
                        {
                            Session["ErrorNumber"] = 1004; //VELOCITY LIMIT EXCEEDED
                            RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                            Response.Redirect(String.Format("{0}?U={1}", RedirectPage, sToken));
                        }
                    }
                    else
                    {
                        //Missing Tables
                        LogMessage = "GetAnswers DataSet (dsResults) is empty.";
                        Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, LogMessage);
                        RedirectPage = Constants.PageNames.USER_CHALLENGE;
                        Response.Redirect(String.Format("{0}?U={1}", RedirectPage, sToken));
                    }
                }
                else
                {
                    //repost to this page
                }
            }
        }

        private bool ValidDataSets(DataSet dsResults)
        {
            bool ValidEntry = true;
            LogMessage = string.Empty;
            foreach (DataTable d in dsResults.Tables)
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

        private void GradeResponses(DataSet dsResults)
        {
            IDologyID = dsResults.Tables["response"].Rows[0]["id-number"].ToString();
            lblCorrect.Text = dsResults.Tables["idliveq-result"].Rows[0]["message"].ToString();
            Session["IDologyNumber"] = IDologyID;
            switch (dsResults.Tables["idliveq-result"].Rows[0]["key"].ToString())
            {
                case (AllCorrect):
                    LogMessage = "All questions answered correctly.";
                    RedirectPage = Constants.PageNames.IDOLOGY_SUCCESSFUL;
                    lblResultCode.Text = AllCorrect;
                    Session["ID"] = "0";
                    break;
                case (Missed1Question):
                    LogMessage = "Missed one question.";
                    RedirectPage = Constants.PageNames.USER_CHALLENGE;
                    lblResultCode.Text = Missed1Question;
                    Session["ID"] = "1";
                    break;
                case (Missed2Question):
                    LogMessage = "Missed two questions.";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    lblResultCode.Text = Missed2Question;
                    Session["ID"] = "1";
                    break;
                case (Missed3Question):
                    LogMessage = "Missed three questions.";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    lblResultCode.Text = Missed3Question;
                    Session["ID"] = "1";
                    break;
                case (Missed4Question):
                    LogMessage = "Missed four questions.";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    lblResultCode.Text = Missed4Question;
                    Session["ID"] = "1";
                    break;
                case (Missed5Question):
                    LogMessage = "Missed five questions.";
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    lblResultCode.Text = Missed5Question;
                    Session["ID"] = "1";
                    break;
                default:
                    RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                    LogMessage = "Error. Default reached on switch";
                    Session["ID"] = "1";
                    break;
            }
            Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, LogMessage);
            Response.Redirect(String.Format("{0}?U={1}", RedirectPage, GUIDToken.ToString()));
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            LoadPageVars();

            if (sToken.Length == 0)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Answers - {0}", LogMessage));
                Session["ErrorNumber"] = 1002;
                RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, Guid.Empty));
            }
            else
            {
                switch (lblResultCode.Text)
                {
                    case (AllCorrect):
                        RedirectPage = Constants.PageNames.IDOLOGY_SUCCESSFUL;
                        break;
                    case (Missed1Question):
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        break;
                    case (Missed2Question):
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        break;
                    case (Missed3Question):
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        break;
                    case (Missed4Question):
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        break;
                    case (Missed5Question):
                        RedirectPage = Constants.PageNames.IDOLOGY_ERROR;
                        break;
                    default:
                        RedirectPage = Constants.PageNames.MANUAL_ID_PROOFING_FORM;
                        break;
                }
                Response.Redirect(String.Format("{0}?U={1}", RedirectPage, sToken));
            }
        }

        private DataSet GetAnswers()
        {
            DataSet ds;
            IDologyClass IDology;
            IDology = new IDologyClass(new AppConfig(), new AppCode.ConfigKeys());
            aText =
               new String[5]
            {
               Session["AText1"].ToString(), Session["AText2"].ToString(), Session["AText3"].ToString(),
               Session["AText4"].ToString(), Session["AText5"].ToString()
            };
            aType =
               new String[5]
            {
               Session["AType1"].ToString(), Session["AType2"].ToString(), Session["AType3"].ToString(),
               Session["AType4"].ToString(), Session["AType5"].ToString()
            };
            IDologyID = Session["IDologyNumber"].ToString();
            IDology.QueryID = IDologyID;
            //Call IDology API to evaluate the User's Answers
            ds = IDology.SendIDologyAnswers(ref aType, ref aText);
            return ds;
        }

        internal void LoadPageVars()
        {
            try
            {
                sToken = Request.QueryString["U"].ToString();
            }
            catch (Exception ex)
            {
                LogMessage = String.Format("Missing GUID - {0}", ex.ToString());
                sToken = String.Empty;
            }
        }

        internal string ValidPageRequest()
        {
            bool ValidPage;
            string ReDirPage = string.Empty;
            PageSecurity PageSec = new PageSecurity();
            string camefrom;
            try
            {
                int seg = Request.UrlReferrer.Segments.GetUpperBound(0);
                camefrom = Request.UrlReferrer.Segments[seg].ToString();
                if (PageSec.ValidConnection(camefrom, Page.ToString()))
                    ValidPage = true;
                else
                    ValidPage = false;
            }
            catch
            {
                camefrom = "Direct";
                ValidPage = false;
            }
            if (!ValidPage)
            {
                Allscripts.Impact.Audit.AddLogEntryIdology(Request.UserHostName, Page.ToString(), GUIDToken, String.Format("Invalid Page Order From: {0}  To: {1}", camefrom, Page.ToString()));
                ReDirPage = PageSec.StartingPage;
            }
            return ReDirPage;
        }
    }
}