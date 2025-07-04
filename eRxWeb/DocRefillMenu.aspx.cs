/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
**04/13/2009   Anand Kumar Krishnan		Modified to show Patient as [Unregistered] 
**                                      when patient is not found in our database (issue#2584)
*******************************************************************************/
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
using System.Collections.Generic;
using Allscripts.ePrescribe.Data;
using eRxWeb.AppCode;
using eRxWeb.State;
using ePA = Allscripts.Impact.ePA;
using Provider = Allscripts.Impact.Provider;
using TaskManager = Allscripts.Impact.TaskManager;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;

namespace eRxWeb
{
    public partial class DocRefillMenu : BasePage 
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            logger.Debug("<SupProv>" + PageState.GetStringOrEmpty("DelegateProviderID") + "<SupProv>");
            PageState.Remove(Constants.SessionVariables.RxTask);
            if (base.IsPasswordSetupRequiredForSSOUser)
            {
                Response.Redirect(string.Concat(Constants.PageNames.FORCE_PASSWORD_SETUP, "?targetURL=", Constants.PageNames.DOC_REFILL_MENU));
            }


            Tasksh1Panel.Visible = true;
            Tasksh2Panel.Visible = false;
            Messagesh2Panel.Visible = true;



            //don't even give ASP.NET the opportunity to postback
            if ((Request.Form["__EVENTTARGET"] == rbtnMyTask.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbtnSiteTask.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbtnAdminTask.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbtnMyTask1.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbtnSiteTask1.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbtnAdminTask1.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbEPATasks.ClientID.Replace("_", "$") ||
            Request.Form["__EVENTTARGET"] == rbEPATasks1.ClientID.Replace("_", "$")) &&
            (!Request.Url.PathAndQuery.ToLower().Contains("listsendscripts") &&
            (!Request.Url.PathAndQuery.ToLower().Contains("tasksummary")) &&
            (!Request.Url.PathAndQuery.ToLower().Contains("arntasks")) &&
            (!Request.Url.PathAndQuery.ToLower().Contains("specialtymedtasks")))
            )
            {
                return;
            }

            Session["CameFrom"] = Constants.PageNames.DOC_REFILL_MENU;

            if (!Page.IsPostBack)
            {
                base.ClearPatientInfo();
                ClientScript.RegisterStartupScript(this.GetType(), "", string.Format("RefreshPatientHeader('{0}');", PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)), true);

                if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    if (string.IsNullOrEmpty(PageState.GetStringOrEmpty("DelegateProviderID")))
                    {
                        loadSupervisingProviders();
                        mpeSetSupervisingProvider.Show();
                    }
                    else
                    {
                        setSupervisingProviderMessage();
                    }
                }

                string licenseID = string.Empty;
                if (Session["LICENSEID"] != null)
                {
                    licenseID = Session["LICENSEID"].ToString();
                }
                if (Request.QueryString["Task"] != null && Request.QueryString["Task"] == "Site")
                {
                    rbtnMyTask.Checked = false;
                    rbtnMyTask1.Checked = false;
                    rbtnSiteTask.Checked = true;
                    rbtnSiteTask1.Checked = true;
                    RefillTaskObjDataSource.SelectParameters["group"].DefaultValue = "Y";
                    grdDocRefillTask.Columns[0].Visible = true;
                    divProvider.Visible = true;
                    divProvider1.Visible = true;
                    btnPharmRefillReport.Visible = true;
                    btnPharmRefillReport1.Visible = true;
                }
                else
                {
                    grdDocRefillTask.Columns[0].Visible = false;
                    btnPharmRefillReport.Visible = true;
                    btnPharmRefillReport1.Visible = true;
                }

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
                {
                    RefillTaskObjDataSource.SelectParameters["patientId"].DefaultValue = Session["PATIENTID"].ToString();
                }

                if (Request.QueryString["msg"] != null)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["msg"]);
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                }

                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, ""); // remove the query string

                LoadTaskFilter();
                loadProviders();
                ddlProvider.Items.Insert(0, new ListItem("All Providers", "All"));
                ddlProvider1.Items.Insert(0, new ListItem("All Providers", "All"));

                if (Session["DelegateProviderID"] != null)
                {
                    ListItem providerItem = ddlProvider.Items.FindByValue(Session["DelegateProviderID"].ToString());
                    if (providerItem != null)
                        providerItem.Selected = true;
                }

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)
                {
                    rbEPATasks1.Checked = true;
                    rbEPATasks.Visible = false;
                    rbEPATasks1.Visible = false;
                    rbtnAdminTask.Visible = false;
                    rbtnAdminTask1.Visible = false;
                    rbtnMyTask.Visible = false;
                    rbtnMyTask1.Visible = false;
                    rbtnSiteTask.Visible = false;
                    rbtnSiteTask1.Visible = false;
                    btnPharmRefillReport1.Visible = false;
                    divProvider1.Visible = true;
                    Server.Transfer(Constants.PageNames.TASK_SUMMARY + "?TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString());
                }
                else
                {
                    rbEPATasks.Visible = base.ShowEPA;
                    rbEPATasks1.Visible = base.ShowEPA;
                }
            }

            if (Session["REFILLMSG"] != null)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = Session["REFILLMSG"].ToString();

                if (Session["REFILLMSG"].ToString().Contains("approved") || Session["REFILLMSG"].ToString().Contains("Controlled substance refill printed for faxing"))
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                else
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;

                Session.Remove("REFILLMSG");
            }

            if (Session["SuccessMsg"] != null)
            {
                Session.Remove("SuccessMsg");
            }
            //Update Tasks Count
            UpdateTasksCount();
        }

        public void UpdateTasksCount()
        {
            IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
            string tasksCount =  MenuApiHelper.GetTaskCount(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode),
                    DBID,
                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId)/*same sessionUserId as userId*/,PageState);
            int numberOfRemainingTasks = 0;
            int.TryParse(tasksCount, out numberOfRemainingTasks);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "", "  window.parent.UpdateTasksCount("+numberOfRemainingTasks+");", true);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks;
            string licenseID = string.Empty;
            if (Session["LICENSEID"] != null)
            {
                licenseID = Session["LICENSEID"].ToString();
            }
            if (Session["SSoMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
            {
                tasks = Allscripts.Impact.Patient.GetTaskCountForPatient(licenseID, Session["PatientID"].ToString(), base.DBID);
                btnBack.Visible = true;
            }
            else
            {

                tasks = Allscripts.Impact.Provider.GetTaskCountForProvider(licenseID, Session["USERID"].ToString(), base.DBID, base.SessionUserID);

                btnBack.Visible = false;

                ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
            }
        }

        protected void LoadTaskFilter()
        {


            int adminTaskType = (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN;
            int adminTasks = 0;

            if (Session["SSoMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
            {
                adminTasks = TaskManager.GetTaskListScriptCount(Session["LICENSEID"].ToString(), Session["PATIENTID"].ToString(), adminTaskType, base.DBID, base.SessionUserID);
            }
            else
            {
                adminTasks = TaskManager.GetTaskListScriptCount(Session["LICENSEID"].ToString(), adminTaskType, base.DBID, base.SessionUserID);
            }

            int tasks = 0;
            int SpecMedTaskCount = 0;

            if (base.IsPOBUser)
            {
                tasks = ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                SpecMedTaskCount = Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID,
                    base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode,
                    base.DBID, base.SessionUserID);

            }
            else
            {
                tasks = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                SpecMedTaskCount = Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID,base.SessionUserID);
            }

            string adminTaskName = "Assistant's Tasks (" + adminTasks.ToString() + ")";
            string ePATaskName = "ePA Tasks (" + tasks.ToString() + ")";
            rbtnAdminTask.Visible = true;
            rbtnAdminTask.Text = adminTaskName;
            rbtnAdminTask1.Visible = true;
            rbtnAdminTask1.Text = adminTaskName;
            rbEPATasks.Text = ePATaskName;
            rbEPATasks1.Text = ePATaskName;
            rbSpecialtyMed.Text = "Patient Access Services (" + SpecMedTaskCount.ToString() + ")";
        }

        protected void loadProviders()
        {
            ddlProvider.DataTextField = "ProviderName";
            ddlProvider.DataValueField = "ProviderID";
            DataSet drListProvider = Provider.GetProviders(Session["LICENSEID"].ToString(), Convert.ToInt32(Session["SITEID"]), base.DBID);

            DataView activeProvidersView = drListProvider.Tables["Provider"].DefaultView;
            activeProvidersView.RowFilter = "UserType IN ('1','1000','1001') AND Active='Y'";
            activeProvidersView.Sort = "ProviderName ASC";
            ddlProvider.DataSource = activeProvidersView;
            ddlProvider.DataBind();


            ddlProvider1.DataTextField = "ProviderName";
            ddlProvider1.DataValueField = "ProviderID";
            ddlProvider1.DataSource = activeProvidersView;
            ddlProvider1.DataBind();

        }

        protected void loadSupervisingProviders()
        {
            DataSet drListProvider = Provider.GetProviders(Session["LICENSEID"].ToString(), Convert.ToInt32(Session["SITEID"]), base.DBID);

            DataView activeProvidersView = drListProvider.Tables["Provider"].DefaultView;
            activeProvidersView.RowFilter = "UserType = '1' AND Active='Y'";
            if (Session["IsDelegateProvider"] != null && Convert.ToBoolean(Session["IsDelegateProvider"]))
            {
                activeProvidersView.RowFilter = "UserType IN ('1','1000','1001') AND Active='Y'";
            }
            activeProvidersView.Sort = "ProviderName ASC";

            logger.Debug("<SupProvList>" + drListProvider.Tables["Provider"].ToLogString() + "<SupProvList>");

            ddlSupervisingProvider.DataTextField = "ProviderName";
            ddlSupervisingProvider.DataValueField = "ProviderID";
            ddlSupervisingProvider.DataSource = activeProvidersView;
            ddlSupervisingProvider.DataBind();
        }

        protected void grdDocRefillTask_RowCreated(object sender, GridViewRowEventArgs e)
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

            if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkPatientName = (LinkButton)e.Row.FindControl("lnkPatientName");
                lnkPatientName.CommandArgument = e.Row.RowIndex.ToString();

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
                                {
                                    image.ImageUrl = "images/sort-ascending.gif";
                                    image.ID = "imgAscendingSortable";
                                }
                                else
                                {
                                    image.ImageUrl = "images/sort-Descending.gif";
                                    image.ID = "imgDescendingSortable";
                                }
                            }
                            cell.Controls.Add(image);
                        }
                    }
                }
            }
        }

        protected void grdDocRefillTask_DataBound(object sender, EventArgs e)
        {

        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
            {
                PatientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)
            };
            RedirectToSelectPatient(null, selectPatientComponentParameters);
        }

        protected void rbtnMyTask_CheckedChanged(object sender, EventArgs e)
        {
            divProvider.Visible = false;
            divProvider1.Visible = false;
            btnPharmRefillReport.Visible = true;
            btnPharmRefillReport1.Visible = true;
            grdDocRefillTask.Columns[0].Visible = false;
            RefillTaskObjDataSource.SelectParameters["group"].DefaultValue = "N";
        }

        protected void rbtnSiteTask_CheckedChanged(object sender, EventArgs e)
        {
            divProvider.Visible = true;
            divProvider1.Visible = true;
            RefillTaskObjDataSource.SelectParameters["group"].DefaultValue = "Y";
            grdDocRefillTask.Columns[0].Visible = true;
        }

        protected void rbtnAdminTask_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.LIST_SEND_SCRIPTS + "?tasktype=" + Convert.ToInt32(Constants.PrescriptionTaskType.SEND_TO_ADMIN).ToString());
        }

        protected void rbEPATasks_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.TASK_SUMMARY + "?TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString() + "&ePATaskStatus=3");
        }

        protected void rbSpecialtyMed_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.SPECIALTYMEDTASKS);
        }

        protected void rbtnPrescriptions_CheckedChanged(object sender, EventArgs e)
        {
            //default to My Task
            //rbtnMyTask.Checked = true;
            //rbtnMyTask1.Checked = true;
            //divProvider.Visible = false;
            //btnPharmRefillReport.Visible = true;
            //btnPharmRefillReport1.Visible = true;
            //grdDocRefillTask.Columns[0].Visible = false;
            //RefillTaskObjDataSource.SelectParameters["group"].DefaultValue = "N";

        }

        protected void ddlProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefillTaskObjDataSource.SelectParameters.Clear();

            if (ddlProvider.SelectedIndex == 0)
            {
                RefillTaskObjDataSource.SelectParameters.Add("licenseID", Session["LicenseID"].ToString());

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
                {
                    RefillTaskObjDataSource.SelectParameters.Add("patientID", Session["PATIENTID"].ToString());
                }
                else
                {
                    RefillTaskObjDataSource.SelectParameters.Add("patientID", "");
                }

                RefillTaskObjDataSource.SelectParameters.Add("physicianID", Session["UserID"].ToString());

                RefillTaskObjDataSource.SelectParameters.Add("group", "Y");
            }
            else
            {
                RefillTaskObjDataSource.SelectParameters.Add("licenseID", Session["LicenseID"].ToString());

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
                {
                    RefillTaskObjDataSource.SelectParameters.Add("patientID", Session["PATIENTID"].ToString());
                }
                else
                {
                    RefillTaskObjDataSource.SelectParameters.Add("patientID", "");
                }

                RefillTaskObjDataSource.SelectParameters.Add("physicianID", ddlProvider.SelectedValue);

                Session["DelegateProviderID"] = ddlProvider.SelectedValue;

                RefillTaskObjDataSource.SelectParameters.Add("group", "N");
            }

            RefillTaskObjDataSource.SelectParameters.Add("dbID", Session["DBID"].ToString());
            RefillTaskObjDataSource.Select();
        }
        protected void ddlProvider1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefillTaskObjDataSource.SelectParameters.Clear();

            RefillTaskObjDataSource.SelectParameters.Add("licenseID", base.SessionLicenseID);

            if (ddlProvider1.SelectedIndex == 0)
            {
                RefillTaskObjDataSource.SelectParameters.Add("physicianID", base.SessionUserID);
                RefillTaskObjDataSource.SelectParameters.Add("group", "Y");
            }
            else
            {
                RefillTaskObjDataSource.SelectParameters.Add("physicianID", ddlProvider1.SelectedValue);
                RefillTaskObjDataSource.SelectParameters.Add("group", "N");

                Session["DelegateProviderID"] = ddlProvider1.SelectedValue;
            }

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && Session["PATIENTID"] != null)
            {
                RefillTaskObjDataSource.SelectParameters.Add("patientID", Session["PATIENTID"].ToString());
            }
            else
            {
                RefillTaskObjDataSource.SelectParameters.Add("patientID", "");
            }

            RefillTaskObjDataSource.SelectParameters.Add("taskFilter", "Approvals");
            RefillTaskObjDataSource.SelectParameters.Add("dbID", Session["DBID"].ToString());
            RefillTaskObjDataSource.SelectParameters.Add("userID", base.SessionUserID);
            RefillTaskObjDataSource.Select();
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
                    Server.Transfer(Constants.PageNames.PHARMACY_REFILL_REPORT + "?To=" + Constants.PageNames.DOC_REFILL_MENU.ToLower());
                    break;
                default:
                    break;
            }
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

            RxUser provider = new RxUser(ddlSupervisingProvider.SelectedValue, base.DBID);
            setProviderInformation(provider);
            logger.Debug("<ProvSelectedfromDropdown>" + provider.ToLogString()+"<ProvSelectedfromDropdown>"); 
        }

        private void setSupervisingProviderMessage()
        {
            ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.DelegateProviderName + ".";
            ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
            ucSupervisingProvider.Visible = true;
        }
        protected void grdDocRefillTask_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RefillDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                string selectedPatientID = grdDocRefillTask.DataKeys[rowIndex]["patientid"].ToString();
                string patientName = grdDocRefillTask.DataKeys[rowIndex]["patient"].ToString();
                string physicianId = grdDocRefillTask.DataKeys[rowIndex]["PhysicianId"].ToString();
                if (!string.IsNullOrWhiteSpace(selectedPatientID) && selectedPatientID != Guid.Empty.ToString())
                {
                    bool isUserRestricted = checkIfUserRestricted(selectedPatientID, base.SessionUserID, base.DBID);
                    Session["IsRestrictedUser"] = isUserRestricted ? true : false;
                    Session["PrivacyPatientID"] = selectedPatientID;
                    Session["PhysicianId"] = physicianId;
                    if (!isUserRestricted)
                    {
                        ((PhysicianMasterPage)Master).SetPatientInfo(selectedPatientID);
                        redirectToTaskProcessor(selectedPatientID, patientName, physicianId);
                    }
                }
                else
                {
                    ((PhysicianMasterPage)Master).ClearPatientInfo();
                    redirectToTaskProcessor(selectedPatientID, patientName, physicianId);
                }

            }
        }

        private void redirectToTaskProcessor(string selectedPatientID, string patient, string PhysicianId)
        {
            ucObsoletePARClassMappingChange.LoadPatientActiveObsoletePARClass();
            if (!ucObsoletePARClassMappingChange.ShowIfPatientHasActiveObsoletedParClass())
            {
                string redirectUrl = string.Format("TaskProcessor.aspx?PatientID={0}&Patient={1}&PhyId={2}&From=DocRefillMenu.aspx&To=ApproveRefillTask.aspx",
                                selectedPatientID,
                                patient,
                                PhysicianId);
                Session.Remove("PhysicianId");
                Session.Remove("PatientName");
                Session.Remove("RestrictedUserOverridden");
                Response.Redirect(redirectUrl);
            }
        }

        private void setProviderInformation(RxUser selectedProvider)
        {
            List<string> deaSchedule = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                if (selectedProvider.DEAScheduleAllowed(i))
                {
                    deaSchedule.Add(i.ToString());
                }
            }

            Session["DEASCHEDULESALLOWED_SUPERVISOR"] = deaSchedule;

            deaSchedule.Sort();
            int minsched = 0;

            if (deaSchedule.Count > 0)
            {
                minsched = Convert.ToInt32(deaSchedule[0]);
            }

            Session["MINSCHEDULEALLOWED_SUPERVISOR"] = minsched.ToString();


        }
    }

}