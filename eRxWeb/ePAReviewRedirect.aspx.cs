using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using TieServiceClient;
using System.Collections.Specialized;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class ePAReviewRedirect : BasePage
{
    public IPlacementResponse PlacementResponse { get; set; }

    private ePAQuestionHeader _baseQuestionSet
    {
        get
        {
            ePAQuestionHeader ret = new ePAQuestionHeader();
            if (Session["ePA_Current_QuestionSet"] != null)
            {
                ret = (ePAQuestionHeader)(Session["ePA_Current_QuestionSet"]);
            }
            else
            {
                ret = new ePAQuestionHeader();
            }

            return ret;
        }
        set
        {
            Session["ePA_Current_QuestionSet"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            switch (Request.QueryString["Action"])
            {
                case "SUBMIT":
                    Session["EPA_REVIEW_ACTION"] = Constants.ePAQuestionReviewUIEvents.Submit;

                    try
                    {
                        _baseQuestionSet.TransPriority = (Constants.ePATransPriority)Enum.Parse(typeof(Constants.ePATransPriority), Request.QueryString["Priority"]);
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "START_OVER":
                    Session["EPA_REVIEW_ACTION"] = Constants.ePAQuestionReviewUIEvents.StartOver;
                    break;
                case "CANCEL":
                    Session["EPA_REVIEW_ACTION"] = Constants.ePAQuestionReviewUIEvents.Cancel;
                    break;
            }

            Response.Redirect(Constants.PageNames.TASK_SUMMARY);
        }

        // Google Analytics
        PlacementResponse = TieUtility.GetAdPlacement(Request.Cookies, new TieUtility(), new TIEServiceManager(), new Allscripts.Impact.Utilities.ConfigurationManager());
    }
}
}