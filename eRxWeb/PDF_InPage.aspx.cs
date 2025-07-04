using System;
using System.Collections.Generic;
using BOL = Allscripts.ePrescribe.Objects;
using System.Xml;
using System.Xml.Xsl;
using ExpertPdf.HtmlToPdf;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;

namespace eRxWeb
{
public partial class PDF_InPage : BasePage
{    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["ActivationCode"] != null)
            {
                int tableHeight = ((PhysicianMasterPageBlank)Master).getTableHeight();

                iframe1.Attributes["src"] = Constants.PageNames.PRINT_ACTIVATION_CODE;
                iframe1.Attributes["height"] = tableHeight.ToString();

                panelMessageHeader.Visible = panelMessage.Visible = false;
                
                btnClose.Text = "Next";
            }
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        ((PhysicianMasterPageBlank)Master).hideTabs();
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        if (Session["ActivationCode"] != null)
        {
            Session.Remove("ActivationCode");

            if (Request.QueryString["To"] != null && Request.QueryString["To"].ToString() != string.Empty)
            {
                if (Request.QueryString["To"].Contains(Constants.PageNames.SETTINGS))
                {
                        // FORTIFY: Not considered an open re-direct as already redirecting to local page
                        Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS, new MessageModel(Request.QueryString["Message"])));
                }
                else if(Request.QueryString["To"].Contains(Constants.PageNames.TUTORIALS) || Request.QueryString["To"].Contains(Constants.PageNames.EDIT_USERS))
                {
                        Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()) + "?message=" + Request.QueryString["Message"].ToString());
                }
                else
                {
                    Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()) + "?msg=" + Request.QueryString["Message"].ToString());
                }
            }
            else
            {
                base.DefaultRedirect("msg=" + Request.QueryString["Message"].ToString());
            }
        }
        else
        {
            leavePage();
        }
    }

    private void leavePage()
    {
        string to = string.Empty;

        if (Request.QueryString["To"] != null)
        {
            to = Request.QueryString["To"].ToString();
        }

        to = to.Replace("|", "&");

        if (to != "")
        {
            Response.Redirect(Constants.PageNames.UrlForRedirection(to));
        }

        Response.Redirect(Constants.PageNames.START_OVER + "?Msg=" + Server.UrlEncode("Script pad successfully processed."));
    }
}

}