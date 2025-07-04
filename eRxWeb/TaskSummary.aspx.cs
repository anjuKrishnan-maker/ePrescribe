/*
 * ************************ PLEASE READ **************************
 * 
 * This is a new page that is meant to replace DocRefillMenu.aspx (provider task page)
 * and ListSendScripts.aspx (POB task page). This will eventually be the 1 main page for 
 * all tasking, regardless of user type.
 * 
 * This page was inherited (e.g. copied & pasted) from DocRefillMenu.aspx.
 * 
 * As of v15.1.2 10/27/2010, this page will only be used for Caremark ePA tasking.
 * Given our timeframes I cannot refactor existing tasking to use this page, so we will
 * gradually introduce this page into the overall tasking workflow.
 * 
 * Sorry fellow developers, I hate it too.
 * 
 * Jason Tithof
 * 10/27/2010
 * 
*/

/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
**04/13/2009   Anand Kumar Krishnan		Modified to show Patient as [Unregistered] 
**                                      when patient is not found in our database (issue#2584)
**11/07/2011   Raghavendra Krishnamurthy Added validation to check for QSet expiry 
*******************************************************************************/

using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Telerik.Web.UI;
using System.Linq;
using Allscripts.Impact.Utilities;
using eRxWeb.ServerModel;
using System.Web.Script.Serialization;
using eRxWeb.AppCode;
using eRxWeb.State;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
    public partial class TaskSummary : BasePage
    {
        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).EnablePartialRendering = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //don't even give ASP.NET the opportunity to postback
            if ((Request.Form["__EVENTTARGET"] == rbtnMyTask.ClientID.Replace("_", "$") || Request.Form["__EVENTTARGET"] == rbtnSiteTask.ClientID.Replace("_", "$") ||
                Request.Form["__EVENTTARGET"] == rbtnAdminTask.ClientID.Replace("_", "$")) && !Request.Url.PathAndQuery.ToLower().Contains("listsendscripts"))
            {
                return;
            }

            ucMessage.Visible = false;



            if (!Page.IsPostBack)
            {
                Int16 ePATaskStatus = 3;
                if (Request.QueryString.AllKeys.Contains("ePATaskStatus"))
                {
                    bool res = Int16.TryParse(Request.QueryString["ePATaskStatus"], out ePATaskStatus);
                    ePATaskStatus = res ? ePATaskStatus : (Int16)3;
                }


                string patientID = string.Empty;
                if (Session["PATIENTID"] != null)
                    patientID = Session["PATIENTID"].ToString();

                clearTaskSelection();

                if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    if (Session["DelegateProviderID"] == null)
                    {
                        loadSupervisingProviders();
                        mpeSetSuperVisor.Show();
                    }
                    else
                    {
                        setSupervisingProviderMessage();
                    }
                }

                InsertePALog(ePATaskStatus);

                if (Request.QueryString["Task"] != null && Request.QueryString["Task"] == "Site")
                {
                    rbtnMyTask.Checked = false;
                    rbtnSiteTask.Checked = true;
                    RefillTaskObjDataSource.SelectParameters["group"].DefaultValue = "Y";
                    grdRefillTask.Columns[0].Visible = true;
                    //divProvider.Visible = true;
                    btnPharmRefillReport.Visible = true;
                }
                else if ((Request.QueryString["TaskType"] != null && Request.QueryString["TaskType"] == Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString())
                    || (string.Equals(Session["CameFrom"].ToString(), Constants.PageNames.TASK_SUMMARY, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ePATasksGridChanges();
                }
                else
                {
                    grdRefillTask.Columns[0].Visible = false;
                    btnPharmRefillReport.Visible = true;
                }

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && base.SessionPatientID != null)
                {
                    RefillTaskObjDataSource.SelectParameters["patientID"].DefaultValue = base.SessionPatientID;
 
                }


                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE && patientID != null)
                {
                    ((PhysicianMasterPage)Master).SetPatientInfo(patientID);
                }


                if (Request.QueryString["msg"] != null)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = HttpUtility.UrlDecode(Request.QueryString["msg"]);
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                }
                else
                {
                    ucMessage.Visible = false;
                    ucMessage.MessageText = string.Empty;
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                }

                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty); // remove the query string

                loadTaskFilter();
                loadProviders();

                if (!base.IsPOBUser)
                {
                    ddlProvider.SelectedValue = ePATaskStatus != 3 ? Session["USERID"].ToString() : "All";
                    ePATaskObjDataSource.SelectParameters["physicianID"].DefaultValue = ePATaskStatus != 3 ? base.SessionUserID : string.Empty;
                }

                rbEPATasks.Visible = base.ShowEPA;

                if (Convert.ToBoolean(Session["ISPROVIDER"]) || Convert.ToBoolean(Session["IsPA"]) || Convert.ToBoolean(Session["IsPASupervised"]))
                {
                    rbtnMyTask.Visible = true;
                    rbtnSiteTask.Visible = true;
                    rbPharmRefills.Visible = false;
                }
                else
                {
                    rbtnMyTask.Visible = false;
                    rbtnSiteTask.Visible = false;
                    rbPharmRefills.Visible = true;
                }

                if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)
                {
                    rbEPATasks.Visible = false;
                    rbtnAdminTask.Visible = false;
                    rbtnMyTask.Visible = false;
                    rbPharmRefills.Visible = false;
                    rbtnSiteTask.Visible = false;
                    rbSpecialtyMed.Visible = false;
                    grdListePATasks.MasterTableView.NoMasterRecordsText = "There are no open ePA tasks available for this patient, Please close this window and SSO into standard SSO mode to create the ePA task from provider favorites";
                }
                else
                {
                    rbEPATasks.Visible = base.ShowEPA;
                }

                if (Session["EPA_REVIEW_ACTION"] != null)
                {
                    ePAQuestionReviewEventArgs forceEventHandler = new ePAQuestionReviewEventArgs((Constants.ePAQuestionReviewUIEvents)Convert.ToInt32(Session["EPA_REVIEW_ACTION"]));
                    //ProcessEPAEvent(null, forceEventHandler);
                    Session["EPA_REVIEW_ACTION"] = null;
                }

                if (Session["EPA_INIT_QA_REVIEW_ACTION"] != null)
                {
                    EPAInitiationQuestionSetEventArgs forceEventHandler = new EPAInitiationQuestionSetEventArgs((Constants.EPAInitiationQuestionSetUIEvents)Convert.ToInt32(Session["EPA_INIT_QA_REVIEW_ACTION"]));
                    ProcessEPAIntiationQSEvent(null, forceEventHandler);
                    Session["EPA_INIT_QA_REVIEW_ACTION"] = null;
                }
            }

            Session["CameFrom"] = Constants.PageNames.TASK_SUMMARY;

            if (Session["REFILLMSG"] != null)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = Session["REFILLMSG"].ToString();

                if (Session["REFILLMSG"].ToString().Contains("approved"))
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                else
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;

                Session.Remove("REFILLMSG");
            }
        }


        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            int tasks;

            if (Session["SSoMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && base.SessionPatientID != null)
            {
                if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                {
                    // Include the POB user's ID to filter tasks for Providers associated to this POB 
                    tasks = Patient.GetTaskCountForPatient(base.SessionLicenseID, base.SessionPatientID, base.SessionUserID, base.DBID);

                    // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                    RefillTaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;

                    // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                    ePATaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;
                }
                else
                {
                    tasks = Patient.GetTaskCountForPatient(base.SessionLicenseID, base.SessionPatientID, base.DBID);
                }

                btnBack.Visible = true;
            }
            else if (Session["SSoMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE && base.SessionPatientID != null)
            {
                if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                {
                    // Include the POB user's ID to filter tasks for Providers associated to this POB 
                    tasks = Patient.GetTaskCountForPatient(base.SessionLicenseID, base.SessionPatientID, base.SessionUserID, base.DBID);

                    // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                    RefillTaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;

                    // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                    ePATaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;

                    //Select patient id for lock down mode. Even if it is null procs will be able to handle it.
                    ePATaskObjDataSource.SelectParameters["patientID"].DefaultValue = base.SessionPatientID;


                }
                else
                {
                    tasks = Patient.GetTaskCountForPatient(base.SessionLicenseID, base.SessionPatientID, base.DBID);
                }

                btnBack.Visible = false;
            }
            else
            {
                if (base.SessionUserType == Constants.UserCategory.PROVIDER || base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT)
                {
                    if (ddlProvider.SelectedIndex > 0)
                    {
                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, ddlProvider.SelectedValue, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                }
                else if (base.IsPOBUser & !base.IsPOBViewAllProviders)
                {
                    // get task count only for selected Providers associated to POB
                    tasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, new Guid(base.SessionUserID), (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);

                    // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                    RefillTaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;

                    // add POB ID to paramaters to filter tasks for Providers associated to this POB 
                    ePATaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;
                }
                else
                {
                    // get task count all "assistant" tasks
                    tasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN, base.DBID, base.SessionUserID);
                }

                btnBack.Visible = false;
            }
             UpdateTasksCount();

            ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
        }
        public void UpdateTasksCount()
        {
            IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
            string tasksCount = MenuApiHelper.GetTaskCount(
                    PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.SSOMode),
                    DBID,
                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId)/*same sessionUserId as userId*/, PageState);
            int numberOfRemainingTasks = 0;
            int.TryParse(tasksCount, out numberOfRemainingTasks);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "", "  window.parent.UpdateTasksCount(" + numberOfRemainingTasks + ");", true);
        }
        protected void InsertePALog(Int16 taskType)
        {

            ePATaskObjDataSource.SelectParameters["TaskType"].DefaultValue = taskType.ToString();
            rbtnOpenEpa.Checked = taskType == 1 ? true : false;
            rbtnResolvedEpa.Checked = taskType == 2 ? true : false;
            rbtnAllEpa.Checked = taskType == 3 ? true : false;

            if (taskType == 1 || taskType == 2)
            {
                Allscripts.ePrescribe.Data.ePA.eAuthAlertInsertLog(base.DBID, base.SessionLicenseID, base.SessionUserID, taskType);
            }


        }

        protected void RefillTaskObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue != null)
            {
                foreach (DataRow dr in ((DataSet)e.ReturnValue).Tables[0].Rows)
                {
                    if (dr["OurPatientID"].ToString() == Guid.Empty.ToString())
                    {
                        if (Convert.ToInt32(dr["MedRequest"]) > 1)
                        {
                            dr["Patient"] = dr["Patient"].ToString().Replace("#", string.Empty) + " [Reconciled]";
                        }
                        else
                        {
                            dr["Patient"] = dr["Patient"].ToString().Replace("#", string.Empty) + " [Reconciled]";
                        }
                    }
                }
            }
        }

        protected void grdRefillTask_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                e.Item.Style["cursor"] = "pointer";
                e.Item.Attributes.Add("onclick", "toggleViewScriptButton(this)");

                RadioButton rbSelectedRow = (RadioButton)e.Item.FindControl("rbSelectedRow");
                if (rbSelectedRow != null)
                {
                    rbSelectedRow.Attributes["value"] = e.Item.ItemIndex.ToString();
                }
            }
        }

        protected void grdListePATasks_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (!(e.Item is GridDataItem))
                return;

            GridDataItem tempDataItem = (GridDataItem)e.Item;
            HtmlInputRadioButton rbSelectedRow = (HtmlInputRadioButton)tempDataItem.FindControl("rbSelectedePATask");

            bool IsRepa = Convert.ToBoolean(tempDataItem.GetDataKeyValue("IsRetrospectiveEPA"));
            if (rbSelectedRow != null)
            {
                rbSelectedRow.Attributes.Add("TaskID", tempDataItem.GetDataKeyValue("TaskID").ToString());
                rbSelectedRow.Attributes.Add("onclick", "taskSelectRadio('" + tempDataItem.GetDataKeyValue("TaskID").ToString() +
                                             "', '" + tempDataItem.GetDataKeyValue("PatientID").ToString() +
                                             "', '" + IsRepa.ToString() + "')");
            }

            // Don't show the the destination dropdownlist when a NCPDP EPA Task
            // is in the following status where we don't have a assosiated PACaseID 
            // "ePA Ready", "ePA Saved, Not Submitted", "ePA Submitted", "ePA Approved",
            // "ePA Denied", "ePA Not Available", "ePA Failed", "ePA Removed/Processed",
            // "ePA Closed" 


            // The above rule is not valid for EPA portal


            Constants.ePANCPDPTaskType ePANCPDPTaskType = (Constants.ePANCPDPTaskType)Convert.ToInt32(tempDataItem.GetDataKeyValue("IsNCPDP"));

            string taskStatus = tempDataItem.GetDataKeyValue("Status").ToString();
            if (IsEpaStatusWithPACaseID(taskStatus) || IsEpaRequestedStatus(taskStatus) || IsRepa)
            {
                populateEPADestination(ref e);
            }
            else
            {
                e.Item.FindControl("cbDestination").Visible = false;
            }

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

        protected void rbSpecialtyMedTasks_CheckedChanged(object sender, EventArgs e)
        {            
            Server.Transfer(Constants.PageNames.SPECIALTYMEDTASKS);
        }



        protected void rbEPATasks_CheckedChanged(object sender, EventArgs e)
        {
            //divProvider.Visible = false;
            btnPharmRefillReport.Visible = true;

            RefillTaskObjDataSource.SelectParameters["taskFilter"].DefaultValue = "ePA";
            RefillTaskObjDataSource.Select();

            Server.Transfer(Constants.PageNames.TASK_SUMMARY + "?TaskType=" + Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString());
        }

        protected void ddlProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (grdListePATasks.Visible)
            {
                int tasks = 0;

                LoadePATasks();

                if (base.IsPOBUser)
                {
                    if (ddlProvider.SelectedIndex > 0)
                    {

                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, ddlProvider.SelectedValue, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                }
                else
                {
                    if (ddlProvider.SelectedIndex > 0)
                    {

                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, ddlProvider.SelectedValue, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                }

                rbEPATasks.Text = "ePA Tasks (" + tasks.ToString() + ")";

                ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
            }
            else
            {
                RefillTaskObjDataSource.SelectParameters.Clear();
                if (ddlProvider.SelectedIndex == 0)
                {
                    RefillTaskObjDataSource.SelectParameters.Add("licenseID", base.SessionLicenseID);
                    RefillTaskObjDataSource.SelectParameters.Add("physicianID", base.SessionUserID);
                    RefillTaskObjDataSource.SelectParameters.Add("group", "Y");

                    if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && base.SessionPatientID != null)
                    {
                        RefillTaskObjDataSource.SelectParameters.Add("patientID", base.SessionPatientID);
                    }
                    else
                    {
                        RefillTaskObjDataSource.SelectParameters.Add("patientID", string.Empty);
                    }
                }
                else
                {
                    RefillTaskObjDataSource.SelectParameters.Add("licenseID", base.SessionLicenseID);
                    RefillTaskObjDataSource.SelectParameters.Add("physicianID", ddlProvider.SelectedValue);
                    RefillTaskObjDataSource.SelectParameters.Add("group", "N");

                    if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && base.SessionPatientID != null)
                    {
                        RefillTaskObjDataSource.SelectParameters.Add("patientID", base.SessionPatientID);
                    }
                    else
                    {
                        RefillTaskObjDataSource.SelectParameters.Add("patientID", string.Empty);
                    }

                    Session["DelegateProviderID"] = ddlProvider.SelectedValue;
                }

                if (base.IsPOBUser && !base.IsPOBViewAllProviders)
                {
                    RefillTaskObjDataSource.SelectParameters.Add("pobID", base.SessionUserID);
                }

                RefillTaskObjDataSource.SelectParameters.Add("dbID", Session["DBiD"].ToString());
                RefillTaskObjDataSource.Select();
            }
        }

        protected void btnPharmRefillReport_Click(object sender, EventArgs e)
        {
            launchReport("PharmRefillReport");
        }

        protected void bntConfirmTaskMsgOkay_Click(object sender, EventArgs e)
        {
            if (btnViewScript.Text == "PROCESS TASK")
            {
                #region Process Task
                string postProcess = string.Empty;
                GridDataItem ePAGridDataItem;
                if (getSelectedePAItem(out ePAGridDataItem))
                {
                    RadComboBox cbDestination = (RadComboBox)ePAGridDataItem.FindControl("cbDestination");
                    string ePAtaskID = ePAGridDataItem.GetDataKeyValue("TaskID").ToString();
                    string rxID = ePAGridDataItem.GetDataKeyValue("RxID").ToString();
                    int refills = Convert.ToInt32(ePAGridDataItem.GetDataKeyValue("RefillQuantity").ToString());
                    int daysSupply = Convert.ToInt32(ePAGridDataItem.GetDataKeyValue("DaysSupply").ToString());

                    if (base.IsPOBUser && Session["DelegateProviderID"] == null)
                    {
                        string providerID = string.Empty;
                        providerID = ePAGridDataItem.GetDataKeyValue("CreatedByID").ToString();

                        if (!string.IsNullOrWhiteSpace(providerID))
                        {
                            Session["DelegateProviderID"] = providerID;
                        }
                    }

                    string taskStatus = ePAGridDataItem.GetDataKeyValue("Status").ToString();
                    Constants.ePANCPDPTaskType taskType = (Constants.ePANCPDPTaskType)Convert.ToInt32(ePAGridDataItem.GetDataKeyValue("IsNCPDP"));

                    //string ePARequestID = string.Empty;
                    DataSet dsePARequestID = new DataSet();

                    if (cbDestination.SelectedValue == "PHARM" || cbDestination.SelectedValue == "PRINT")
                    {
                        Prescription.UpdateRxDetailDates(rxID, base.DBID);
                    }

                    switch (cbDestination.SelectedValue)
                    {
                        case "PHARM":
                            if (Session["LASTPHARMACYID"] != null && Session["LASTPHARMACYID"].ToString() != string.Empty)
                            {
                                sendPrescription(rxID, "SENTTOPHARMACY", refills, daysSupply);

                                if (taskType == Constants.ePANCPDPTaskType.NCPDP &&
                                    IsEpaProcessingStatus(taskStatus) && checkValidPACaseID(ePAtaskID))
                                {
                                    sendePACancelRequest(ePAtaskID);
                                }
                                else
                                {
                                    EPSBroker.EPA.UpdateTaskStatus(ePAtaskID, Constants.ePATaskStatus.EPA_REMOVED_PROCESSED, base.DBID);
                                }

                                ucMessage.Visible = true;
                                ucMessage.MessageText = "Prescription successfully sent to pharmacy ";
                                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                                grdListePATasks.Rebind();
                            }
                            else
                            {
                                ucMessage.Visible = true;
                                ucMessage.MessageText = "Patient retail Pharmacy is not available.";
                                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                            }
                            break;
                        case "MOB":
                            if (isPatientEligibleForMailOrderBenefit())
                            {
                                sendPrescriptionToMOB(rxID, "SENTTOPHARMACY", refills, daysSupply);

                                if (taskType == Constants.ePANCPDPTaskType.NCPDP &&
                                    IsEpaProcessingStatus(taskStatus) && checkValidPACaseID(ePAtaskID))
                                {
                                    sendePACancelRequest(ePAtaskID);
                                }
                                else
                                {
                                    EPSBroker.EPA.UpdateTaskStatus(ePAtaskID, Constants.ePATaskStatus.EPA_REMOVED_PROCESSED, base.DBID);
                                }
                                ucMessage.Visible = true;
                                ucMessage.MessageText = "Prescription successfully sent to Mail Order pharmacy ";
                                ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                                grdListePATasks.Rebind();
                            }
                            else
                            {
                                ucMessage.Visible = true;
                                ucMessage.MessageText = "Patient Mail Order Pharmacy is not available.";
                                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                            }
                            break;
                        case "EIE":
                            string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
                            string extGroupID = Session["ExtGroupID"] != null ? Session["ExtGroupID"].ToString() : null;
                            Prescription.EnteredInError(rxID, false, base.SessionUserID, base.SessionLicenseID, extFacilityCode, extGroupID, base.DBID);

                            if (taskType == Constants.ePANCPDPTaskType.NCPDP &&
                                    IsEpaProcessingStatus(taskStatus) && checkValidPACaseID(ePAtaskID))
                            {
                                sendePACancelRequest(ePAtaskID);
                            }
                            else
                            {
                                EPSBroker.EPA.UpdateTaskStatus(ePAtaskID, Constants.ePATaskStatus.EPA_REMOVED_PROCESSED, base.DBID);
                            }

                            ucMessage.Visible = true;
                            ucMessage.MessageText = "ePA Task Successfully Removed/Rx is updated as Entered In Error";
                            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                            grdListePATasks.Rebind();
                            break;
                        case "PRINT":
                            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID, rxID);
                            ArrayList alProcess = new ArrayList();
                            ArrayList alEPAPrintTaskItems = new ArrayList();
                            alProcess.Add(rxID);
                            alEPAPrintTaskItems.Add(new ePA.EpaTaskCancelObject(ePAtaskID, false));


                            if (taskType == Constants.ePANCPDPTaskType.NCPDP &&
                                    IsEpaProcessingStatus(taskStatus) && checkValidPACaseID(ePAtaskID))
                            {
                                alEPAPrintTaskItems.Add(new ePA.EpaTaskCancelObject(ePAtaskID, true));
                            }
                            else
                            {
                                alEPAPrintTaskItems.Add(new ePA.EpaTaskCancelObject(ePAtaskID, false));
                            }

                            if (alProcess != null && alProcess.Count > 0)
                            {
                                // save list of RxIDs that need to be printed
                                PageState["ePATaskStatus"] = rbtnOpenEpa.Checked ? Constants.ePATaskStatusType.ePAOpen : rbtnResolvedEpa.Checked ? Constants.ePATaskStatusType.ePAClosed : Constants.ePATaskStatusType.AllePA;
                                Session["ProcessList"] = alProcess;
                                Session["EPAPrintTaskItems"] = alEPAPrintTaskItems;
                                postProcess = Constants.PageNames.CSS_DETECT + "?PrintScript=YES&To=" + Constants.PageNames.TASK_SUMMARY;
                            }
                            break;
                        default:
                            break;
                    }

                    int tasks = 0;

                    if (base.IsPOBUser)
                    {
                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }
                    else
                    {
                        tasks = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                    }

                    string ePATaskName = "ePA Tasks (" + tasks.ToString() + ")";
                    rbEPATasks.Text = ePATaskName;

                    if (!string.IsNullOrWhiteSpace(postProcess))
                    {
                        Server.Transfer(postProcess);
                    }

                }
                #endregion Process Task
            }
            else if (btnViewScript.Text == "REMOVE TASK")
            {
                #region Remove Task
                GridDataItem ePAGridDataItem;

                if (getSelectedePAItem(out ePAGridDataItem))
                {
                    string taskStatus = ePAGridDataItem.GetDataKeyValue("Status").ToString();
                    Constants.ePANCPDPTaskType taskType = (Constants.ePANCPDPTaskType)Convert.ToInt32(ePAGridDataItem.GetDataKeyValue("IsNCPDP"));
                    string ePAtaskID = ePAGridDataItem.GetDataKeyValue("TaskID").ToString();
                    if (taskType == Constants.ePANCPDPTaskType.NCPDP && IsEpaProcessingStatus(taskStatus) && checkValidPACaseID(ePAtaskID))
                    {
                        sendePACancelRequest(ePAtaskID);
                    }
                    else
                    {
                        EPSBroker.EPA.UpdateTaskStatus(ePAtaskID, Constants.ePATaskStatus.EPA_REMOVED_PROCESSED, base.DBID);
                    }

                    grdListePATasks.Rebind();
                    ePATasksGridChanges();

                }
                #endregion Remove Task
            }

            //Response.Redirect(Constants.PageNames.TASK_SUMMARY);        

        }

        protected void bntSuperVisor_Click(object sender, EventArgs e)
        {
            Session["DelegateProviderID"] = ddlSupervisingProvider.SelectedValue;

            DataSet dsSPI = Provider.GetSPI(Session["USERID"].ToString(), base.DBID);
            if (dsSPI.Tables.Count > 0)
            {
                DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");
                //should only be one row for SURESCRIPTS...grab the first and only
                if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value && drSPI[0]["SenderId"].ToString() != string.Empty)
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

        protected void sendePACancelRequest(string ePATaskID)
        {
            EPSBroker.EPA.SendEpaCancelRequest(ePATaskID, base.SessionLicenseID, base.SessionUserID, base.NcpdpEpaUserShieldSecurityToken, base.DBID);
        }

        protected bool checkValidPACaseID(string ePATaskID)
        {
            return ePA.GetPACaseIDForTask(ePATaskID, base.DBID);
        }

        protected void btnViewScript_Click(object sender, EventArgs e)
        {
            if (hdnFieldbtnViewScript.Value != string.Empty)
            {
                btnViewScript.Text = hdnFieldbtnViewScript.Value;
                hdnFieldbtnViewScript.Value = string.Empty;
            }
            if (btnViewScript.Text == "View Script")
            {
                string patientID;
                string providerID;

                if (getSelectedRow(out patientID, out providerID))
                {
                    string taskType = Convert.ToInt32(Constants.PrescriptionTaskType.EPA).ToString();

                    Server.Transfer(Constants.PageNames.TASK_SCRIPT_LIST + "?PatID=" + Server.UrlEncode(patientID) + "&ProvID=" + Server.UrlEncode(providerID) + "&TaskType=" + taskType);
                }
                else
                {
                    ucMessage.MessageText = "Please select a patient.";
                    ucMessage.Visible = true;
                }
            }
            else if (btnViewScript.Text == "PROCESS TASK" || btnViewScript.Text == "REMOVE TASK")
            {
                ViewState["SelectedItemIndex"] = (grdListePATasks.SelectedIndexes != null && grdListePATasks.SelectedIndexes.Count > 0) ? grdListePATasks.SelectedIndexes[0].ToString() : null;

                GridDataItem ePAGridDataItem;
                if (getSelectedePAItem(out ePAGridDataItem))
                {
                    string taskStatus = ePAGridDataItem.GetDataKeyValue("Status").ToString();
                    Constants.ePANCPDPTaskType taskType = (Constants.ePANCPDPTaskType)Convert.ToInt32(ePAGridDataItem.GetDataKeyValue("IsNCPDP"));
                    if (taskType == Constants.ePANCPDPTaskType.NCPDP)
                    {
                        if (IsEpaProcessingStatus(taskStatus))
                            mpeShowTaskMsgCancel.Show();
                        else if (IsEpaRequestedStatus(taskStatus) || IsEpaFinalStatus(taskStatus))
                            mpeShowConfirmTaskMsgNCPDP.Show();
                    }
                    else
                        mpeShowConfirmTaskMsg.Show();
                }

            }
        }

        protected void grdListePATasks_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "ePAStatus")
            {
                GridDataItem tempItem = e.Item as GridDataItem;
                Constants.ePATaskStatus ePATaskStatus = (Constants.ePATaskStatus)Convert.ToInt32(tempItem.GetDataKeyValue("StatusID"));
                Constants.ePANCPDPTaskType ePANCPDPTaskType = (Constants.ePANCPDPTaskType)Convert.ToInt32(tempItem.GetDataKeyValue("IsNCPDP"));
                string taskID = tempItem.GetDataKeyValue("TaskID").ToString();

                switch (ePATaskStatus)
                {

                    case Constants.ePATaskStatus.EPA_READY:
                    case Constants.ePATaskStatus.EPA_SAVED_NOT_SUBMITTED:
                        showQuestions(ePATaskStatus, taskID, ePANCPDPTaskType);
                        break;
                    case Constants.ePATaskStatus.EPA_DENIED:
                    case Constants.ePATaskStatus.EPA_CLOSED:
                    case Constants.ePATaskStatus.EPA_FAILED:
                    case Constants.ePATaskStatus.EPA_APPROVED:
                    case Constants.ePATaskStatus.EPA_NOT_AVAILABLE:
                        showStatusInformation(ePATaskStatus, taskID, tempItem.GetDataKeyValue("MedicationName").ToString(),
                            tempItem.GetDataKeyValue("Patient").ToString(), ePANCPDPTaskType);
                        break;
                    case Constants.ePATaskStatus.EPA_SUBMITTED:
                        break;
                    case Constants.ePATaskStatus.EPA_REMOVED_PROCESSED:
                        break;
                    case Constants.ePATaskStatus.UNKNOWN:
                        break;
                    case Constants.ePATaskStatus.EPA_REQUESTED:
                        break;
                    default:
                        break;
                }
            }
        }

        private void showStatusInformation(Constants.ePATaskStatus ePATaskStatus, string taskID, string medName, string patName, Constants.ePANCPDPTaskType ePANCPDPTaskType)
        {
            if (ePANCPDPTaskType == Constants.ePANCPDPTaskType.NCPDP)
            {
                ePANcpdpStatusInfomation.TaskID = taskID;
                ePANcpdpStatusInfomation.MedicationName = medName;
                ePANcpdpStatusInfomation.PatientFullName = patName;
                ePANcpdpStatusInfomation.EPATaskStatus = ePATaskStatus;
                ePANcpdpStatusInfomation.Show();
            }
        }

        private void showQuestions(Constants.ePATaskStatus ePATaskStatus, string taskID, Constants.ePANCPDPTaskType ePANCPDPTaskType)
        {
            ViewState["ePATaskStatus"] = rbtnOpenEpa.Checked ? (int)Constants.ePATaskStatusType.ePAOpen : rbtnResolvedEpa.Checked ? (int)Constants.ePATaskStatusType.ePAClosed : (int)Constants.ePATaskStatusType.AllePA;
            if (ePANCPDPTaskType == Constants.ePANCPDPTaskType.NCPDP)
            {
                EPAInitQuestionSet ePaInitQS = new EPAInitQuestionSet(taskID, base.SessionSiteID, ePATaskStatus, base.DBID);
                if (isQuestionSetExpired(ePaInitQS.ExpirationDate))
                {
                    //Load modal box to inform expiry of QSet
                    mpeShowTaskStatusInfo.Show();
                }
                else
                {
                    //Continue with loading QSet
                    ucEPAInitiationResponse.LoadQS(ePaInitQS);
                    ucEPAInitiationResponse.Show(false);
                }

            }
        }

        protected void ProcessEPAIntiationQSEvent(object sender, EPAInitiationQuestionSetEventArgs e)
        {
            switch (e.EventType)
            {
                case Constants.EPAInitiationQuestionSetUIEvents.Review:
                case Constants.EPAInitiationQuestionSetUIEvents.FileAttached:
                    ucEPAInitiationResponse.Show(true);
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.StartOver:
                    ucEPAInitiationResponse.StartOver();
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.Cancel:
                    ucEPAInitiationResponse.Cancel();
                    if (sender != null)
                    {
                        Response.Redirect(Constants.PageNames.TASK_SUMMARY + "?ePATaskStatus=" + ViewState["ePATaskStatus"]);
                    }
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.SaveForLater:
                    ucEPAInitiationResponse.Hide();
                    if (sender != null)
                    {
                        Response.Redirect(Constants.PageNames.TASK_SUMMARY + "?ePATaskStatus=" + ViewState["ePATaskStatus"]);
                    }
                    break;
                case Constants.EPAInitiationQuestionSetUIEvents.Submit:
                    ucEPAInitiationResponse.Submit();
                    grdListePATasks.Rebind();
                    break;
            }
        }

        protected void hiddenSelectTask_Click(object sender, EventArgs e)
        {
            btnViewScript.Enabled = true;
        }

        protected void grdRefillTask_DataBound(object sender, EventArgs e)
        {
            if (grdRefillTask.Items.Count == 0)
            {
                btnViewScript.Enabled = false;
            }
        }

        protected void grdListePATasks_DataBound(object sender, EventArgs e)
        {
            if (grdListePATasks.Items.Count == 0)
            {
                btnViewScript.Enabled = false;
            }
        }

        protected void clearTaskSelection()
        {
            btnViewScript.Enabled = false;
            base.ClearPatientInfo();
            ClientScript.RegisterStartupScript(this.GetType(), "", string.Format("RefreshPatientHeader('{0}');", PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId)), true);
        }

        #endregion

        #region Custom Methods
        private void populateEPADestination(ref GridItemEventArgs e)
        {
            RadComboBox cbDestination = (RadComboBox)e.Item.FindControl("cbDestination");
            Label lblCSMed = (Label)e.Item.FindControl("lblCSMed");
            string controlledSubstanceCode = grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["ControlledSubstanceCode"].ToString().Trim();//ControlledSubstanceCode
            string stateControlledSubstanceCode = grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["StateControlledSubstanceCode"].ToString().Trim();//ControlledSubstanceCode
            bool isControlledSubstance = (!string.IsNullOrEmpty(controlledSubstanceCode) && !controlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(stateControlledSubstanceCode) && !stateControlledSubstanceCode.Equals("U", StringComparison.OrdinalIgnoreCase) && !stateControlledSubstanceCode.Equals("0", StringComparison.OrdinalIgnoreCase));

            lblCSMed.Visible = isControlledSubstance ? true : false;

            bool IsRetrospectiveEPA = Convert.ToBoolean(grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["IsRetrospectiveEPA"]);
            if (IsRetrospectiveEPA)
            {
                cbDestination.Visible = false;
                string pharmacyPhoneNumber = StringHelper.FormatExternalPhoneNumberForDisplay((grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["PharmacyAreaCode"].ToString().Trim() + grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["PharmacyPhone"].ToString().Trim()));
                string rEPAmsg = "Received at: " + grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["PharmacyName"].ToString().Trim() + "<br>" + " Tel: " + pharmacyPhoneNumber;
                Label lblIsRepa = (Label)e.Item.FindControl("lblIsRepa");
                lblIsRepa.Visible = true;
                lblIsRepa.Text = rEPAmsg;
            }
            else
            {
                cbDestination.Visible = true;
                bool sendToPharm = false;
                bool sendToMailOrder = false;
                int i = 0;
                string spi = grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["SenderID"].ToString().Trim();
                Constants.ePANCPDPTaskType ePANCPDPTaskType = (Constants.ePANCPDPTaskType)Convert.ToInt32(grdListePATasks.MasterTableView.DataKeyValues[e.Item.ItemIndex]["IsNCPDP"]);

                // we need to relook at send to pharmacy logic below as and when we add more options to dropdown for ePA tasklist - 17Nov2011
                if (!isControlledSubstance && spi != string.Empty)
                {
                    //If the user is a provider or PA, they will not have erx capability if they don't have a spi. Even if the original script
                    //was sent by a doc that has an spi, this provider will be the provider of record.
                    if ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.PROVIDER || (Constants.UserCategory)Session["UserType"] == Constants.UserCategory.PHYSICIAN_ASSISTANT)
                    {
                        if (Session["SPI"] != null && Session["SPI"].ToString() != string.Empty)
                        {
                            sendToPharm = true;
                            // Adding Send to Mail order only for NCPDP tasks
                            if (ePANCPDPTaskType == Constants.ePANCPDPTaskType.NCPDP)
                                sendToMailOrder = true;
                        }
                    }
                    else
                    {
                        sendToPharm = true;
                        // Adding Send to Mail order only for NCPDP tasks
                        if (ePANCPDPTaskType == Constants.ePANCPDPTaskType.NCPDP)
                            sendToMailOrder = true;
                    }
                }

                if (sendToPharm)
                    cbDestination.Items.Insert(i++, new RadComboBoxItem("Send to Pharmacy", "PHARM"));

                if (sendToMailOrder)
                    cbDestination.Items.Insert(i++, new RadComboBoxItem("Send to Mail Order", "MOB"));


                cbDestination.Items.Insert(i++, new RadComboBoxItem("Print", "PRINT"));
                cbDestination.Items.Insert(i++, new RadComboBoxItem("Entered In Error", "EIE"));
            }
        }

        private bool getSelectedRow(out string patientID, out string providerID)
        {
            bool foundSelectedRow = false;

            patientID = string.Empty;
            providerID = string.Empty;

            foreach (GridDataItem row in grdRefillTask.MasterTableView.Items)
            {
                RadioButton rbSelectedRow = (RadioButton)row.FindControl("rbSelectedRow");
                if (rbSelectedRow != null)
                {
                    if (rbSelectedRow.Checked)
                    {
                        foundSelectedRow = true;

                        patientID = row.GetDataKeyValue("OurPatientID").ToString();
                        providerID = row.GetDataKeyValue("PhysicianId").ToString();
                        break;
                    }
                }
            }

            return foundSelectedRow;
        }

        private bool getSelectedePAItem(out GridDataItem selectedItem)
        {
            bool foundSelectedRow = false;
            selectedItem = null;

            if (ViewState["SelectedItemIndex"] != null)
            {
                selectedItem = grdListePATasks.MasterTableView.Items[int.Parse(ViewState["SelectedItemIndex"].ToString())];
                ViewState["SelectedItemIndex"] = null;
                foundSelectedRow = true;
            }
            else
            {
                foreach (GridDataItem item in grdListePATasks.MasterTableView.Items)
                {
                    HtmlInputRadioButton rbSelectedRow = (HtmlInputRadioButton)item.FindControl("rbSelectedePATask");
                    if (rbSelectedRow != null)
                    {
                        if (rbSelectedRow.Checked)
                        {
                            selectedItem = item;
                            foundSelectedRow = true;
                            break;
                        }
                    }
                }
            }
            return foundSelectedRow;
        }

        protected void launchReport(String REPORT_ID)
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

        protected void loadTaskFilter()
        {
            int adminTaskType = (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN;
            int adminTasks = 0;

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE && base.SessionPatientID != null)
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

            rbtnAdminTask.Visible = true;
            rbtnAdminTask.Text = adminTaskName;

            int tasks = 0;
            if (base.IsPOBUser)
            {
                tasks = ePA.GetePATaskCount(base.SessionLicenseID, base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }
            else
            {
                tasks = ePA.GetePATaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }

            string ePATaskName = "ePA Tasks (" + tasks.ToString() + ")";
            rbEPATasks.Text = ePATaskName;
        }

        protected void loadProviders()
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

            ddlProvider.DataTextField = "ProviderName";
            ddlProvider.DataValueField = "ProviderID";
            ddlProvider.DataSource = activeProvidersView;
            ddlProvider.DataBind();

            if (isSelectedProviders)
            {
                ddlProvider.Items.Insert(0, new ListItem("All Associated Providers", "All Associated Providers"));
            }
            else
            {
                ddlProvider.Items.Insert(0, new ListItem("All Providers", "All"));
            }

            if (Session["DelegateProviderID"] != null)
            {
                ListItem providerItem = ddlProvider.Items.FindByValue(Session["DelegateProviderID"].ToString());
                if (providerItem != null)
                {
                    providerItem.Selected = true;
                }
                else
                {
                    ddlProvider.SelectedIndex = 0;
                }
            }
            else
            {
                ddlProvider.SelectedIndex = 0;
            }

            if (ddlProvider.SelectedIndex > 0)
            {
                int tasks = 0;

                LoadePATasks();

                if (base.IsPOBUser)
                {
                    tasks = ePA.GetePATaskCount(base.SessionLicenseID, ddlProvider.SelectedValue, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                }
                else
                {
                    tasks = ePA.GetePATaskCount(base.SessionLicenseID, ddlProvider.SelectedValue, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
                }

                rbEPATasks.Text = "ePA Tasks (" + tasks.ToString() + ")";
            }
            UpdateSpecialtyMedTaskCount();
        }

        protected void UpdateSpecialtyMedTaskCount()
        {
            int SpecialtyMedTaskCount = 0;
            if (base.IsPOBUser)
            {
                SpecialtyMedTaskCount = Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID,
                    base.SessionDelegateProviderID, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode,
                    base.DBID, base.SessionUserID);
            }
            else
            {
                SpecialtyMedTaskCount = Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }
            rbSpecialtyMed.Text = "Patient Access Services (" + SpecialtyMedTaskCount.ToString() + ")";
        }

        protected void loadSupervisingProviders()
        {
            DataSet drListProvider = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.DBID);
            DataView activeProvidersView = drListProvider.Tables["Provider"].DefaultView;

            activeProvidersView.RowFilter = "UserType = '1' AND Active='Y'";

            if (Session["IsDelegateProvider"] != null && Convert.ToBoolean(Session["IsDelegateProvider"]))
            {
                activeProvidersView.RowFilter = "UserType IN ('1','1000','1001') AND Active='Y'";
            }

            activeProvidersView.Sort = "ProviderName ASC";
            ddlSupervisingProvider.DataTextField = "ProviderName";
            ddlSupervisingProvider.DataValueField = "ProviderID";
            ddlSupervisingProvider.DataSource = activeProvidersView;
            ddlSupervisingProvider.DataBind();
        }

        private void setSupervisingProviderMessage()
        {
            ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.DelegateProviderName + ".";
            ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
            ucSupervisingProvider.Visible = true;
        }

        private void sendPrescription(string rxID, string rxStatus, int refills, int daysSupply)
        {
            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID, rxID);
            Prescription.MarkAsFulfilled(base.SessionLicenseID, base.SessionUserID, rxID, refills, daysSupply, rxStatus, Session["LASTPHARMACYID"].ToString(), false, false, null, false, base.DBID);

            string smid = ScriptMessage.CreateScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID, base.SessionUserID, base.ShieldSecurityToken, base.SessionSiteID, base.DBID);

            if (!string.IsNullOrEmpty(smid) && Session["STANDING"].ToString() == "1")
            {
                ScriptMessage.SendThisMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }

            smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, base.DBID);
            if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(smid)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }
        }

        private bool isQuestionSetExpired(string ePAQSetExpDate)
        {
            try
            {
                DateTime expDate = DateTime.Parse(ePAQSetExpDate);
                return (expDate.Date < DateTime.Now.Date); //True=Expired
            }
            catch (Exception)
            {
                return false;
            }
        }

        // To change ViewScript button Name and show/hide ePA Tasks destination dropdownlist
        private void ePATasksGridChanges()
        {
            btnPharmRefillReport.Visible = false;
            grdRefillTask.Visible = false;
            grdListePATasks.Visible = true;

            btnViewScript.Text = "PROCESS TASK";
            btnViewScript.ToolTip = "Click to process selected ePA Tasks";
            //divProvider.Visible = false;

        }

        private void LoadePATasks()
        {
            if (ddlProvider.SelectedIndex > 0)
            {
                ePATaskObjDataSource.SelectParameters["physicianID"].DefaultValue = ddlProvider.SelectedValue.ToString();
            }
            else
            {
                ePATaskObjDataSource.SelectParameters["physicianID"].DefaultValue = string.Empty;
            }

            if (ePATaskObjDataSource.SelectParameters["patientID"] != null)
            {
                ePATaskObjDataSource.SelectParameters["patientID"].DefaultValue = base.SessionPatientID;
            }
            else
            {
                ePATaskObjDataSource.SelectParameters.Add("patientID", base.SessionPatientID);
            }

            if (base.IsPOBUser)
            {
                ePATaskObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;
            }
            else
            {
                ePATaskObjDataSource.SelectParameters["pobID"].DefaultValue = string.Empty;
            }
            grdListePATasks.Rebind();
        }

        public static readonly string[] EpaStatusWithPACaseID = new[]
    {
        "ePA Ready", "ePA Saved, Not Submitted", "ePA Submitted", "ePA Approved",
        "ePA Denied", "ePA Not Available", "ePA Failed", "ePA Removed/Processed","ePA Closed"
    };

        public static readonly string[] EpaProcessingStatus = new[]
    {
        "ePA Ready", "ePA Saved, Not Submitted", "ePA Submitted", "ePA Failed"
    };

        public static readonly string[] EpaFinalStatus = new[]
    {
        "ePA Approved","ePA Denied", "ePA Closed"
    };
        public static readonly string EpaRequestedStatus = "ePA Requested";
        private static bool IsEpaRequestedStatus(string epaStatus)
        {
            return EpaRequestedStatus.Equals(epaStatus);
        }
        private static bool IsEpaStatusWithPACaseID(string epaStatus)
        {
            return EpaStatusWithPACaseID.Contains(epaStatus);
        }

        private static bool IsEpaProcessingStatus(string epaStatus)
        {
            return EpaProcessingStatus.Contains(epaStatus);
        }

        private static bool IsEpaFinalStatus(string epaStatus)
        {
            return EpaFinalStatus.Contains(epaStatus);
        }

        private bool isPatientEligibleForMailOrderBenefit()
        {
            bool patientEligibleForMailOrderBenefit = false;

            string patientHasMOBCoverage = Session["PatientHasMOBCoverage"] != null ? Session["PatientHasMOBCoverage"].ToString() : null;

            string mobNABP = Session["MOB_NABP"] != null ? Session["MOB_NABP"].ToString() : null;

            if (patientHasMOBCoverage != null && patientHasMOBCoverage.ToString() == "Y")
            {
                if ((!string.IsNullOrEmpty(mobNABP)) && (mobNABP.ToString().Trim() != "X"))
                {
                    patientEligibleForMailOrderBenefit = true;
                }
            }
            return patientEligibleForMailOrderBenefit;
        }

        private void sendPrescriptionToMOB(string rxID, string rxStatus, int refills, int daysSupply)
        {
            string mopharmID = string.Empty;
            if (Session["MOB_PHARMACY_ID"] != null)
            {
                mopharmID = Session["MOB_PHARMACY_ID"].ToString();
            }

            if (string.IsNullOrEmpty(mopharmID))
            {
                DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(Session["MOB_NABP"].ToString(), base.DBID);

                if (mobDS.Tables[0].Rows.Count > 0)
                {
                    mopharmID = mobDS.Tables[0].Rows[0]["PharmacyID"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(mopharmID))
            {
                Prescription.MarkAsFulfilled(base.SessionLicenseID, base.SessionUserID, rxID, refills, daysSupply, rxStatus, mopharmID, true, false, null, false, base.DBID);

                string smid = string.Empty;
                smid = ScriptMessage.CreateScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID,
                                        base.SessionUserID, base.ShieldSecurityToken, base.SessionSiteID, base.DBID);

                string auditLogPatientID = string.Empty;

                ePrescribeSvc.AuditLogPatientRxResponse rxResponse = base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED,
                                                                     base.SessionPatientID, rxID);
                if (rxResponse.Success)
                {
                    auditLogPatientID = rxResponse.AuditLogPatientID;
                }

                long serviceTaskID = -1;
                if (!string.IsNullOrEmpty(smid) && Session["STANDING"].ToString() == "1")
                {
                    serviceTaskID = ScriptMessage.SendThisMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
                }

                ///Check here if it is not -1 then insert it in newly created table. you also have auditLogPatientID.
                ///This will be used from service manager and added last audit log when message is sent to hub.
                if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
                {
                    Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
                }

                smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, base.DBID);

                if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(smid)))
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
                }
            }
        }


        protected void rbgFilterEpaTask_CheckedChanged(object sender, EventArgs e)
        {
            ePATaskObjDataSource.SelectParameters["TaskType"].DefaultValue = rbtnOpenEpa.Checked ? "1" : rbtnResolvedEpa.Checked ? "2" : "3";
            grdListePATasks.Rebind();

        }



        #endregion

        protected void grdListePATasks_DataBinding(object sender, EventArgs e)
        {
           clearTaskSelection();
        }
    }

}
