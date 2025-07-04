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
namespace eRxWeb
{
    public partial class ePAInitQAReviewRedirect : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch (Request.QueryString["Action"])
                {
                    case "SUBMIT":
                        Session["EPA_INIT_QA_REVIEW_ACTION"] = Constants.EPAInitiationQuestionSetUIEvents.Submit;
                        break;
                    case "START_OVER":
                        Session["EPA_INIT_QA_REVIEW_ACTION"] = Constants.EPAInitiationQuestionSetUIEvents.StartOver;
                        break;
                    case "CANCEL":
                        Session["EPA_INIT_QA_REVIEW_ACTION"] = Constants.EPAInitiationQuestionSetUIEvents.Cancel;
                        break;
                }

                Response.Redirect(Constants.PageNames.TASK_SUMMARY);
            }
        }
    }
}