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
using System.IO;
using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using eRxWeb.AppCode;

namespace eRxWeb
{
public partial class StyleSpinner : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string[] css = Directory.GetFiles(Server.MapPath("Style"));
        foreach (string ss in css)
        {
            if (ss.ToLower().IndexOf("xsl") == -1)
            {
                ListItem li = new ListItem();
                li.Text = ss.Substring(ss.LastIndexOf("\\")+1);
                li.Value = ss;
                if (Session["Theme"] != null && Session["Theme"].ToString() == ss)
                {
                    li.Selected = true;
                }
                if ((Session["Theme"] == null || Session["Theme"].ToString() == "") && ss.ToLower() == "style.css")
                {
                    li.Selected = true;
                }
                rblStyle.Items.Add(li);
            }
        }
        

        if (IsPostBack)
        {
            string final = rblStyle.SelectedValue.Substring(rblStyle.SelectedValue.LastIndexOf("\\") + 1);
            Session["Theme"] = "Style/" + final;
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = Convert.ToString(Session[Constants.SessionVariables.PatientId])
            };
            RedirectToSelectPatient(null, selectPatientComponentParameters);
        }        
    }
    public void RedirectToSelectPatient(string queryString, SelectPatientComponentParameters selectPatientComponentParameters)
    {
        if (queryString.Contains("StartOver=Y") || Convert.ToBoolean(Session[Constants.SessionVariables.AppComponentAlreadyInitialized]))//since it is coming from Angular component
        {
            //WHEN APP COMPONENT ALREADY EXISTS
            var componentParameters = new JavaScriptSerializer().Serialize(selectPatientComponentParameters);
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_PATIENT + "&componentParameters=" + componentParameters);
        }
        else
        {
            //LOGIN WORKFLOW WHEN NO APP COMPONENT IS THERE
            Session[Constants.SessionVariables.AppComponentAlreadyInitialized] = true;
            Response.Redirect(RedirectHelper.GetRedirectToSPAUrl(string.Empty, false));
        }
    }
    }

}