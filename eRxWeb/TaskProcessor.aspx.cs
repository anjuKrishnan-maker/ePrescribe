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
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class TaskProcessor : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string taskType = Request.QueryString["TaskType"];
        string patientID = Request.QueryString["PatientID"];
        string patientName = Request.QueryString["Patient"];
        string physicianID = Request.QueryString["PhyID"];
        string fromPage = Constants.PageNames.UrlForRedirection(Request.QueryString["From"]);
        string toPage = Constants.PageNames.UrlForRedirection(Request.QueryString["To"]);

            string redirectPage;

        if (!string.IsNullOrEmpty(taskType) && taskType.Equals("ePA", StringComparison.OrdinalIgnoreCase))
        {
            redirectPage = Constants.PageNames.TASK_SCRIPT_LIST + "?PatID=" + patientID + "&ProvID=" + physicianID + "&TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString();
        }
        else
        {
            redirectPage = toPage + "?PatientID=" + patientID + "&Patient=" + patientName + "&PhyId=" + physicianID + "&From=" + fromPage;
        }


        Response.Redirect(redirectPage);
    }
}

}