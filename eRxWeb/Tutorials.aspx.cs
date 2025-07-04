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
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
public partial class Tutorials : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["PAGEHEIGHT"] == null)
            {
                Server.Transfer(Constants.PageNames.SET_HEIGHT + "?dest=" + Constants.PageNames.TUTORIALS.ToLower());
            }
            if (Request.QueryString["message"] != null &&
                Request.QueryString["message"].Length > 0)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = Request.QueryString["message"];

                ucMessage.Icon =
                    ucMessage.MessageText.Contains("success") ?
                        Controls_Message.MessageType.SUCCESS :
                        Controls_Message.MessageType.ERROR;

                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, "");
            }
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }
    protected void btnProceed_Click(object sender, EventArgs e)
    {
        string[] tutorialsViewed = null;
        bool updateTutorial = true;

        for (int i = 0; i < tutorialsGridView.Rows.Count; i++)
        {
            updateTutorial = true;
            CheckBox dontShowAgain = tutorialsGridView.Rows[i].FindControl("chkDontShowAgain") as CheckBox;

            if (Session["TutorialsViewed"] != null)
                tutorialsViewed = Session["TutorialsViewed"].ToString().Split(new char[] {','});

            if (tutorialsViewed != null)
            {
                for (int q = 0; q < tutorialsViewed.Length; q++)
                {
                    if (tutorialsGridView.DataKeys[i].Value.ToString() == tutorialsViewed.GetValue(q).ToString())
                    {
                        updateTutorial = false;
                        break;
                    }
                }
            }

            if (updateTutorial)
            {
                RxUser.UpdateUserTutorial(
                    Session["UserID"].ToString(),
                    tutorialsGridView.DataKeys[i].Value.ToString(),
                    "N",
                    dontShowAgain.Checked ? "Y" : "N",
                    Session["LicenseID"].ToString(),
                    base.DBID);
            }
        }

        Session.Remove("TutorialsViewed");

        DefaultRedirect();

    }
    protected void tutorialsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "View")
        {
            RxUser.UpdateUserTutorial(
                Session["UserID"].ToString(),
                e.CommandArgument.ToString(),
                "Y",
                "N",
                Session["LicenseID"].ToString(),
                base.DBID);

            btnProceed.Text = "Proceed >>";

            ucMessage.Visible = true;
            ucMessage.MessageText = "You have viewed a tutorial. When you are finished viewing the tutorials, click the 'Proceed >>' button below.";

            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;

            if (Session["TutorialsViewed"] == null)
                Session["TutorialsViewed"] = e.CommandArgument.ToString();
            else
                Session["TutorialsViewed"] = Session["TutorialsViewed"].ToString() + "," + e.CommandArgument.ToString();
        }
    }
    protected void tutorialsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton link = e.Row.FindControl("lnkBtnTutorial") as LinkButton;
            link.Attributes.Add("onclick", "openTutorial('" + ((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[2].ToString() + "')");
        }
    }
}

}