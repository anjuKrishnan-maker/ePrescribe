/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 06/17/2009   Dharani Reddem           Added PRESCRIPTION_TO_PROVIDER to navigate the correct pages
*******************************************************************************/
using System;
using System.Text;
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
    public partial class ListSendScripts : BasePage 
    {
        string PatID; //scope only for this page
        string ProvID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.IsPasswordSetupRequiredForSSOUser)
            {
                Response.Redirect(string.Concat(Constants.PageNames.FORCE_PASSWORD_SETUP, "?targetURL=", Constants.PageNames.LIST_SEND_SCRIPTS));
            }

            Tasksh1Panel.Visible = true;
            Tasksh2Panel.Visible = false;
            Messagesh2Panel.Visible = true;

            if (Session["PrintLabels"] != null)
            {
                Session["ProcessList"] = Session["LabelProcessList"];
                Session["LabelProcessList"] = null;
                Response.Redirect(Session["PrintLabels"].ToString());
            }

            if (!Page.IsPostBack)
            {
                base.ClearPatientInfo();

                LoadTaskFilter();
                LoadTaskSource();

                if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    if (Session["DelegateProviderID"] == null)
                    {
                        loadSupervisingProviders();
                        mpeSetSupervisingProvider.Show();
                    }
                    else
                    {
                        setSupervisingProviderMessage();
                    }
                }

                rbEPATasks.Visible = base.ShowEPA;
                rbEPATasks1.Visible = base.ShowEPA;  
            }

            btnPharmRefillReport.Visible = false;
            btnPharmRefillReport1.Visible = false;
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks = 0;

            if (!string.IsNullOrEmpty(base.SessionLicenseID))
            {
                string licenseID = base.SessionLicenseID;

                int taskType = (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN;

                if (Request.QueryString["tasktype"] != null && Request.QueryString["tasktype"].ToString() != "")
                {
                    taskType = Convert.ToInt32(Request.QueryString["tasktype"].ToString());
                }

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
                {
          
                        if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                        {
                            // Include the POB user's ID to filter tasks for Providers associated to this POB 
                            tasks = Patient.GetTaskCountForPatient(licenseID, base.SessionPatientID, base.SessionUserID, base.DBID);

                            // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                            TaskObjDataSource.SelectParameters["POBID"].DefaultValue = base.SessionUserID;
                        }
                        else
                        {
                            tasks = Patient.GetTaskCountForPatient(licenseID, base.SessionPatientID, base.DBID);
                        }
                    

                    if ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.PROVIDER || (Constants.UserCategory)Session["UserType"] == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED || (Constants.UserCategory)Session["UserType"] == Constants.UserCategory.PHYSICIAN_ASSISTANT)
                    {
                        ucTaskAlert.Visible = false;
                    }
                    else
                    {
                        int physicianTasks = Patient.GetTaskCountForPatient(licenseID, base.SessionPatientID, (int)Constants.PrescriptionTaskType.APPROVAL_REQUEST, base.DBID);

                        if (physicianTasks > 0)
                        {
                            ucTaskAlert.Visible = true;

                            ucTaskAlert.Icon = Controls_Message.MessageType.INFORMATION;
                            ucTaskAlert.MessageText = "There are prescriptions available for authorization.";
                        }
                        else
                        {
                            ucTaskAlert.Visible = false;
                        }
                    }

                    TaskObjDataSource.SelectParameters["PatientID"].DefaultValue = base.SessionPatientID;

                    btnSelectPat.Visible = true;
                }
                else
                {
               
                        if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                        {
                            // get task count only for selected Providers associated to POB
                            tasks = TaskManager.GetTaskListScriptCount(licenseID, new Guid(base.SessionUserID), taskType, base.DBID, base.SessionUserID);

                            // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                            TaskObjDataSource.SelectParameters["POBID"].DefaultValue = base.SessionUserID;
                        }
                        else
                        {
                            // get task count all "assistant" tasks
                            tasks = TaskManager.GetTaskListScriptCount(licenseID, taskType, base.DBID, base.SessionUserID);
                        }
                   
                    //Displaying Success message when assistant processess the task successfully.
                    if (Request.QueryString["TaskProcessed"] != null && Request.QueryString["TaskProcessed"].ToString() == "true")
                    {
                        ucMessage.MessageText = "Task processed successfully.";
                        ucMessage.Visible = true;
                        ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                    }
                    else
                    {
                        ucMessage.Visible = false;
                    }

                    ucTaskAlert.Visible = false;
                    btnSelectPat.Visible = false;

                }

                ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);


            }
        }

        protected void LoadTaskFilter()
        {
            Constants.PrescriptionTaskType adminTaskType = Constants.PrescriptionTaskType.SEND_TO_ADMIN;
            int adminTasks = 0;

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
            {
                if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                {
                    // get task count only for selected Providers associated to POB
                    adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, base.SessionPatientID, base.SessionUserID, Convert.ToInt32(adminTaskType), base.DBID, base.SessionUserID);
                }
                else
                {
                    adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, base.SessionPatientID, Convert.ToInt32(adminTaskType), base.DBID, base.SessionUserID);
                }
            }
            else
            {
                if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                {
                    // get task count only for selected Providers associated to POB
                    adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, new Guid(base.SessionUserID), Convert.ToInt32(adminTaskType), base.DBID, base.SessionUserID);
                }
                else
                {
                    // get task count all "assistant" tasks
                    adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, Convert.ToInt32(adminTaskType), base.DBID, base.SessionUserID);
                }
            }

            string adminTaskName = "Assistant's Tasks (" + adminTasks.ToString() + ")";

            int tasks = 0;
            if (base.IsPOBUser)
            {
                tasks = Allscripts.Impact.ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }
            else
            {
                tasks = Allscripts.Impact.ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }

            string ePATaskName = "ePA Tasks (" + tasks.ToString() + ")";
            rbEPATasks.Text = ePATaskName;
            rbEPATasks1.Text = ePATaskName;

            if (Session["ISPROVIDER"] != null && Convert.ToBoolean(Session["ISPROVIDER"]))
            {
                //Options for provider
                rbtnMyTask.Visible = true;
                rbtnSiteTask.Visible = true;
                rbtnAdminTask.Visible = true;
                rbtnAdminTask.Text = adminTaskName;
                rbtnAdminTask.Checked = true;

                rbtnMyTask1.Visible = true;
                rbtnSiteTask1.Visible = true;
                rbtnAdminTask1.Visible = true;
                rbtnAdminTask1.Text = adminTaskName;
                rbtnAdminTask1.Checked = true;
                rbSpecialtyMed1.Visible = true;
                Session["TaskTypeForList"] = Convert.ToInt32(adminTaskType);
            }
            else
            {
                switch ((Constants.UserCategory)Session["UserType"])
                {
                    case Constants.UserCategory.GENERAL_USER:
                    case Constants.UserCategory.POB_SUPER:
                    case Constants.UserCategory.POB_REGULAR:
                        rbtnMyTask.Visible = false;
                        rbtnSiteTask.Visible = false;
                        rbPharmRefills.Visible = true;
                        rbtnMyTask1.Visible = false;
                        rbtnSiteTask1.Visible = false;
                        rbPharmRefills1.Visible = true;
                        rbSpecialtyMed1.Visible = true;
                        break;
                    case Constants.UserCategory.POB_LIMITED:
                        rbtnMyTask.Visible = false;
                        rbtnSiteTask.Visible = false;
                        rbPharmRefills.Visible = true;
                        rbtnMyTask1.Visible = false;
                        rbtnSiteTask1.Visible = false;
                        rbPharmRefills1.Visible = true;
                        break;
                    case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                        rbtnMyTask.Visible = true;
                        rbtnSiteTask.Visible = true;
                        rbPharmRefills.Visible = false;
                        rbtnMyTask1.Visible = true;
                        rbtnSiteTask1.Visible = true;
                        rbPharmRefills1.Visible = false;
                        rbSpecialtyMed.Visible = true;
                        rbSpecialtyMed1.Visible = true;
                        break;
                    default:
                        break;
                }

                rbtnAdminTask.Visible = true;
                rbtnAdminTask.Checked = true;
                rbtnAdminTask.Text = adminTaskName;
                rbtnAdminTask1.Visible = true;
                rbtnAdminTask1.Checked = true;
                rbtnAdminTask1.Text = adminTaskName;
            }

            if (rbSpecialtyMed1.Visible)
            {
                int SpecialtyMedCount = SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID, string.Empty,
                                     string.Empty, string.Empty, base.SessionSSOMode, base.DBID, base.SessionUserID);
                rbSpecialtyMed1.Text = "Patient Access Services (" + SpecialtyMedCount.ToString() + ")"; 
            }
        }
        protected void grdViewTasks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowState | DataControlRowState.Selected) == e.Row.RowState)
                {
                    e.Row.RowState = e.Row.RowState ^ DataControlRowState.Selected;
                }
                e.Row.Style["cursor"] = "pointer";
                e.Row.Attributes.Add("onclick", "onRowClick(this)");
            }
        }

        //This method gets the Selected row from the Grid.. 
        //and assigns the RXID and the TASKID to the variables..
        //Before sending it to the Destination Page.

        private void GetSelectedTask(string selectedRowValue)
        {
            var values = selectedRowValue.Split('|');
            PatID = values[0];
            ProvID = values[1];
            PageState["DelegateProviderID"] = ProvID;
        }

        protected void LoadTaskSource()
        {
            DataSet dsListProvider = new DataSet();
            DataView activeProvidersView = new DataView();
            bool isSelectedProviders = false;

            if (base.IsPOBUser & !base.IsPOBViewAllProviders)
            {
                isSelectedProviders = true;
                dsListProvider = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.SessionUserID, base.DBID);
            }
            else
            {
                dsListProvider = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.DBID);
            }

            activeProvidersView = dsListProvider.Tables["Provider"].DefaultView;
            activeProvidersView.RowFilter = "UserType IN ('1','1000','1001') AND Active='Y'";
            activeProvidersView.Sort = "ProviderName ASC";

            ddlTaskSource.DataTextField = "ProviderName";
            ddlTaskSource.DataValueField = "ProviderID";
            ddlTaskSource.DataSource = activeProvidersView;
            ddlTaskSource.DataBind();

            ddlTaskSource1.DataTextField = "ProviderName";
            ddlTaskSource1.DataValueField = "ProviderID";
            ddlTaskSource1.DataSource = activeProvidersView;
            ddlTaskSource1.DataBind();

            if (isSelectedProviders)
            {
                ddlTaskSource.Items.Insert(0, new ListItem("All Associated Providers", "All Associated Providers"));
                ddlTaskSource1.Items.Insert(0, new ListItem("All Associated Providers", "All Associated Providers"));
            }
            else
            {
                ddlTaskSource.Items.Insert(0, new ListItem("All Providers", "All"));
                ddlTaskSource1.Items.Insert(0, new ListItem("All Providers", "All"));
            }

            if (Session["DelegateProviderID"] != null)
            {
                ListItem providerItem = ddlTaskSource.Items.FindByValue(Session["DelegateProviderID"].ToString());
                if (providerItem != null)
                {
                    providerItem.Selected = true;
                }
                else
                {
                    ddlTaskSource.SelectedIndex = 0;
                }

                ListItem providerItem1 = ddlTaskSource1.Items.FindByValue(Session["DelegateProviderID"].ToString());
                if (providerItem1 != null)
                {
                    providerItem1.Selected = true;
                }
                else
                {
                    ddlTaskSource1.SelectedIndex = 0;
                }
            }
            else
            {
                ddlTaskSource.SelectedIndex = 0;
                ddlTaskSource1.SelectedIndex = 0;
            }

            ExecuteSourceFilter();
            ExecuteSourceFilter1();
        }

        protected void grdViewTasks_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Pager)
            {
                Table t = e.Row.Cells[0].Controls[0] as Table;
                TableRow r = t.Rows[0];
                foreach (TableCell cell in r.Cells)
                {
                    object ctl = cell.Controls[0];
                    if (ctl is Label)
                    {
                        ((Label)ctl).Text = "Page " + ((Label)ctl).Text;
                        ((Label)ctl).CssClass = "CurrentPage";
                    }
                    else
                    {
                        ((LinkButton)ctl).Text = "[" + ((LinkButton)ctl).Text + "]";
                    }
                }
            }

            if (e.Row != null && e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell cell in e.Row.Cells)
                {
                    if (cell.HasControls())
                    {
                        LinkButton button = cell.Controls[0] as LinkButton;
                        if (button != null)
                        {
                            Image image = new Image();
                            image.ImageUrl = "images/sort-sortable.gif";
                            if (((System.Web.UI.WebControls.GridView)(sender)).SortExpression == button.CommandArgument)
                            {
                                if (((System.Web.UI.WebControls.GridView)(sender)).SortDirection == SortDirection.Ascending)
                                    image.ImageUrl = "images/sort-ascending.gif";
                                else
                                    image.ImageUrl = "images/sort-Descending.gif";
                            }
                            cell.Controls.Add(image);
                        }
                    }
                }
            }
        }

        protected void btnSelectPat_Click(object sender, EventArgs e)
        {
            base.DefaultRedirect();
        }

        protected void btnViewScript_Click(object sender, EventArgs e)
        {
            // Get All the tasks and then in the destination page you need to 
            // send all the selected values and then update the Rx Detail and 
            // the RX header tables with the values May 29

            this.GetSelectedTask(Request.Form["rdSelectRow"]);
            string admin = Convert.ToInt32(Constants.PrescriptionTaskType.SEND_TO_ADMIN).ToString();
            
            //ucPrivacyOverride
            bool isUserRestricted = checkIfUserRestricted(PatID, base.SessionUserID, base.DBID);
            if (!isUserRestricted)
            {
               // ucPrivacyOverride.Visible = false;
                Server.Transfer(Constants.PageNames.TASK_SCRIPT_LIST + "?PatID=" + Server.UrlEncode(PatID) + "&ProvID=" + Server.UrlEncode(ProvID) + "&TaskType=" + admin);
            }
            else
            {
                //ucPrivacyOverride.Visible = true;
                //ucPrivacyOverride.RedirectURL = Constants.PageNames.TASK_SCRIPT_LIST + "?PatID=" + Server.UrlEncode(PatID) + "&ProvID=" + Server.UrlEncode(ProvID) + "&TaskType=" + admin;
                //ucPrivacyOverride.CancelURL = Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=4";
                //ucPrivacyOverride.showOverlay(PatID);
            }
            
        }

        protected void rbtnMyTask_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.DOC_REFILL_MENU + "?Task=MY");
        }

        protected void rbtnSiteTask_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.DOC_REFILL_MENU + "?Task=Site");
        }

        protected void rbtnAdminTask_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=" + Convert.ToInt32(Constants.PrescriptionTaskType.SEND_TO_ADMIN).ToString());
        }

        protected void rbPharmRefills_Changed(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.PHARMACY_REFILL_SUMMARY);
        }

        protected void rbtnPrescriptions_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.DOC_REFILL_MENU + "?Task=MY");
        }

        protected void rbEPATasks_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.TASK_SUMMARY + "?TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString() + "&ePATaskStatus=3");
        }
        protected void rbSpecialtyMedTasks_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.SPECIALTYMEDTASKS);
        }

        protected void ddlTaskSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteSourceFilter();
        }

        protected void ddlTaskSource1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExecuteSourceFilter1();
        }

        protected void btnPharmRefillReport_Click(object sender, EventArgs e)
        {
            LaunchReport("PharmRefillReport");
        }

        protected void LaunchReport(String REPORT_ID)
        {
            switch (REPORT_ID)
            {
                case "PharmRefillReport":
                    Server.Transfer(Constants.PageNames.PHARMACY_REFILL_REPORT);
                    break;
                default:
                    break;
            }
        }

        protected void ExecuteSourceFilter()
        {
            TaskObjDataSource.FilterParameters.Clear();

            if (ddlTaskSource.SelectedIndex != 0)
            {
                TaskObjDataSource.FilterExpression = "ProviderID='{0}'";
                TaskObjDataSource.FilterParameters.Add("ProviderID", ddlTaskSource.SelectedValue);

                Session["DelegateProviderID"] = ddlTaskSource.SelectedValue;
            }
            else
            {
                TaskObjDataSource.FilterExpression = "Filter='{0}'";
                TaskObjDataSource.FilterParameters.Add("Filter", "1");
            }

            TaskObjDataSource.DataBind();
        }

        protected void ExecuteSourceFilter1()
        {
            TaskObjDataSource.FilterParameters.Clear();

            if (ddlTaskSource1.SelectedIndex != 0)
            {
                TaskObjDataSource.FilterExpression = "ProviderID='{0}'";
                TaskObjDataSource.FilterParameters.Add("ProviderID", ddlTaskSource1.SelectedValue);

                Session["DelegateProviderID"] = ddlTaskSource1.SelectedValue;
            }
            else
            {
                TaskObjDataSource.FilterExpression = "Filter='{0}'";
                TaskObjDataSource.FilterParameters.Add("Filter", "1");
            }

            TaskObjDataSource.DataBind();
        }

        protected void loadSupervisingProviders()
        {
            DataSet drListProvider = Provider.GetProviders(Session["LICENSEID"].ToString(), Convert.ToInt32(Session["SITEID"]), base.DBID);

            DataView activeProvidersView = drListProvider.Tables["Provider"].DefaultView;
            activeProvidersView.RowFilter = "UserType = '1' AND Active='Y'";
            activeProvidersView.Sort = "ProviderName ASC";

            ddlSupervisingProvider.DataTextField = "ProviderName";
            ddlSupervisingProvider.DataValueField = "ProviderID";
            ddlSupervisingProvider.DataSource = activeProvidersView;
            ddlSupervisingProvider.DataBind();
        }

        protected void btnSetSupervisingProvider_Click(object sender, EventArgs e)
        {
            Session["DelegateProviderID"] = ddlSupervisingProvider.SelectedValue;

            DataSet dsSPI = Provider.GetSPI(Session["USERID"].ToString(), base.DBID);
            if (dsSPI.Tables.Count > 0)
            {
                DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");
                //should only be one row for SURESCRIPTS...grab the first and only
                if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value && drSPI[0]["SenderId"].ToString() != "")
                {
                    Session["SPI"] = drSPI[0]["SenderID"].ToString();
                }
                else
                {
                    Session["SPI"] = null;
                }
            }
            else
            {
                Session["SPI"] = null;
            }

            setSupervisingProviderMessage();
        }

        private void setSupervisingProviderMessage()
        {
            ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.DelegateProviderName + ".";
            ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
            ucSupervisingProvider.Visible = true;
        }

        protected void grdViewTasks_Sorting(object sender, GridViewSortEventArgs e)
        {
            ExecuteSourceFilter1();
        }
    }
}