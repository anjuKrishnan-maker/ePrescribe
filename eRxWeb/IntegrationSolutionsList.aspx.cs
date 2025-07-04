// Revision History
/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 02/08/2010   Anand Kumar Krishnan     Defect#3335 - Task count is retained when Tool tab is selected. 
********************************************************************************/
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
using Telerik.Web.UI;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
namespace eRxWeb
{
    public partial class IntegrationSolutionsList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AllModules.SelectParameters["AdvertiseDeluxe"].DefaultValue = base.SessionLicense.AdvertiseDeluxe.ToString();
                toolShowcase.Visible = false;
                if (Request.QueryString["Module"] != null)
                {
                    Module.ModuleType moduleType = (Module.ModuleType)Enum.Parse(typeof(Module.ModuleType), Request.QueryString["Module"].ToString());

                    Module currentModule = new Module(moduleType, base.SessionLicenseID, base.SessionUserID, base.DBID);

                    toolShowcase.Visible = true;
                    if( ( currentModule.Type != Module.ModuleType.DELUXE ) && (currentModule.Type != Module.ModuleType.EPCS))
                    {
                        imShowCase.ImageUrl = currentModule.LargeAdPath;
                    }
                    else
                    {
                        imShowCase.Visible = false;
                    }

                    showCaseLiteral.Style["display"] = "inline";

                    if (Convert.ToBoolean(currentModule.Type == Module.ModuleType.DELUXE))
                    {
                        Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=DeluxeFeatureSelectionComponent" + "?CameFrom=" + Constants.PageNames.INTEGRATION_SOLUTIONS_LIST);

                    }

                    if (currentModule.Type != Module.ModuleType.EPCS)
                        showCaseLiteral.Controls.Add(new LiteralControl(currentModule.FullDetail));
                    else
                    {
                        Control myUserControl = (Control)Page.LoadControl("~/Controls/EPCSSignup.ascx");
                        showCaseLiteral.Controls.Add(myUserControl);
                    }

                    grdIntegration.Visible = false;
                }


            }

            //if (SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.On)
            //{
            //    // Get AD placement from TIE service.
            //    GetAdPlacement(Constants.TIELocationPage.IntegrationSolutionsList_Page);
            //}
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks = 0;
            if (Session["LICENSEID"] != null)
            {
                string licenseID = Session["LICENSEID"].ToString();

                if (Session["UserType"] != null && (Convert.ToInt32(Session["UserType"]) == 1 || Convert.ToInt32(Session["UserType"]) == 1000 || Convert.ToInt32(Session["UserType"]) == 1001))
                {
                    tasks = Provider.GetTaskCountForProvider(licenseID, Session["USERID"].ToString(), base.DBID, base.SessionUserID);
                }
                else
                {
                    if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                    {
                        // get task count only for selected Providers associated to POB
                        tasks = TaskManager.GetTaskListScriptCount(licenseID, new Guid(base.SessionUserID), (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        // get task count all "assistant" tasks
                        tasks = TaskManager.GetTaskListScriptCount(licenseID, (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                    }
                }
            }
            ((PhysicianMasterPageBlank)Master).toggleTabs(Constants.TabName.MANAGE_ACCOUNT, tasks);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "", "  window.parent.UpdateActiveMenu('" + Constants.TabName.MANAGE_ACCOUNT + "');", true);
        }

        protected void grdIntegration_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem && e.Item.OwnerID == "ctl00_ContentPlaceHolder1_grdIntegration_ctl00")
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;

                Image img = tempDataItem["Briefing"].FindControl("imEnrolledImage") as Image;
                Label subscriptionType = tempDataItem["Briefing"].FindControl("lblSubscriptionType") as Label;
                Label lblHeaderText = tempDataItem["Briefing"].FindControl("lblHeaderText") as Label;
                Button manage = tempDataItem["Briefing"].FindControl("btnEnroll") as Button;
                Button addFeature = tempDataItem["Briefing"].FindControl("btnAddFeature") as Button;



                string display = ((Allscripts.Impact.Module)(e.Item.DataItem)).EnabledDisplay;
                string module = ((Allscripts.Impact.Module)(e.Item.DataItem)).ModuleID;


                if (Convert.ToBoolean(module.ToLower().Contains("deluxe")))
                {
                    manage.Visible = true;

                    DataTable dt = DeluxePurchaseManager.GetDeluxeAccountInfo(base.SessionLicenseID, base.DBID).Tables[0];
                    if (dt.Rows.Count > 0
                        && dt.Rows[0]["PaymentProfileID"] != DBNull.Value)
                    {
                        DataRow accountInfo = (DataRow)dt.Rows[0];

                        if (accountInfo["ModuleSubscribed"] != DBNull.Value)
                        {
                            string productName = accountInfo["ModuleSubscribed"].ToString().ToLower();

                            if (!string.IsNullOrWhiteSpace(productName))
                            {
                                Session["CurrentDeluxeSubscription"] = productName;

                                if (Convert.ToBoolean(tempDataItem.GetDataKeyValue("Enabled")))
                                {
                                    if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On)
                                    {
                                        addFeature.Visible = true;
                                    }
                                    if(SessionLicense.IsPricingStructureCompulsoryBasic())
                                    {
                                        addFeature.Visible = true;
                                        Literal literalBrief = tempDataItem["Briefing"].FindControl("literalBrief") as Literal;
                                        literalBrief.Text = "You are a current user of ePrescribe™ Basic. Click below to manage your account.";
                                        Image imModuleImage = tempDataItem["Briefing"].FindControl("imModuleImage") as Image;
                                        imModuleImage.ImageUrl = "Modules/DELUXE/LargeAdBasic.jpg";
                                    }
                                }

                            }

                            //Get the Module Subscribed.
                            subscriptionType.Text = getModuleSubscribed(productName, base.DBID);
                            lblHeaderText.Visible = false;
                        }
                        else
                        {
                            lblHeaderText.Visible = true;
                        }
                    }
                }

                //if enrolled, change the button text
                if (Convert.ToBoolean(tempDataItem.GetDataKeyValue("Enabled")))
                {
                    img.ImageUrl = "~/images/check.png";
                    manage.Text = "Manage";
                    Session["EnrollText"] = manage.Text;

                }
                else
                {
                    img.Visible = false;
                }

            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            if (Session["EnrollText"] != null && Convert.ToBoolean(Session["EnrollText"].ToString().Contains("Manage")))
            {
                Session.Remove("EnrollText");
                Response.Redirect(Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT + "?From=" + Constants.PageNames.INTEGRATION_SOLUTIONS_LIST);
            }
            else
            {
                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=DeluxeFeatureSelectionComponent&componentParameters=" + Constants.PageNames.INTEGRATION_SOLUTIONS_LIST);
            }
        }


        protected void btnAddFeature_Click(object sender, EventArgs e)
        {
            Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=DeluxeFeatureSelectionComponent&componentParameters=" + Constants.PageNames.INTEGRATION_SOLUTIONS_LIST);
        }
    }
}
    
