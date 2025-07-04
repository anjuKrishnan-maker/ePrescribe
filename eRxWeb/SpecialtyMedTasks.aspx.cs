using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Telerik.Web.UI;
using System.IO;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.ePrescribeSvc;
using Allscripts.Impact.services.SpecialtyMedUtils;
using Patient = Allscripts.Impact.Patient;
using Prescription = Allscripts.Impact.Prescription;
using Provider = Allscripts.Impact.Provider;
using Rx = Allscripts.Impact.Rx;
using Allscripts.ePrescribe.Shared.Logging;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using TaskManager = Allscripts.Impact.TaskManager;
using ISpecialtyMed = eRxWeb.AppCode.Interfaces.ISpecialtyMed;
using SpecialtyMed = eRxWeb.AppCode.SpecialtyMed;
using ePA = Allscripts.Impact.ePA;
using DXCRecommendationContext = eRxWeb.AppCode.SpecialtyMedWorkflow.DXCRecommendationContext;


namespace eRxWeb
{
    public partial class SpecialtyMedTasks : BasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        private List<LinkButton> listLinkButtonsSpecMedsDocuments = new List<LinkButton>();

        private DataSet TasksWithDocuments
        {
            get { return ViewState["TasksWithDocuments"] as DataSet; }
            set { ViewState["TasksWithDocuments"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).EnablePartialRendering = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckForStatusUpdate();
            if (!Page.IsPostBack)
            {
                LoadAssistRxToken();
                ClearPatientInfo();
                ((PhysicianMasterPage)Master).UpdatePatientHeader();

                if (ddlProvider.Items.Count == 0)
                {
                    loadProviders();
                }

                btnProcessTask.Enabled = !rbtnResolved.Checked;
                UpdateSpecialtyMedTaskCount();

                //Load SpecialtyMed Tasks with attachments received from AssistRx
                SpecialtyMed specialtyMed = new SpecialtyMed();
                TasksWithDocuments = specialtyMed.SpecialtyMedLoadAttachmentsCountPerRxTaskID(DBID);

                if (Convert.ToBoolean(Session["ISPROVIDER"]) || Convert.ToBoolean(Session["IsPA"]) ||
                    Convert.ToBoolean(Session["IsPASupervised"]))
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


                if ((Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES") &&
                    (Request.QueryString["From"] != null &&
                     Request.QueryString["From"].ToLower().Contains(Constants.PageNames.SPECIALTYMEDTASKS.ToLower())) &&
                     Request.QueryString["From"].Contains("Rx Printed"))
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Successfully Processed. 1 Rx Printed.";
                }
            }

            LoadTaskFilter();
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
                SpecMedTaskCount = Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID, string.Empty, string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);
            }

            string adminTaskName = "Assistant's Tasks (" + adminTasks.ToString() + ")";
            string ePATaskName = "ePA Tasks (" + tasks.ToString() + ")";
            rbtnAdminTask.Visible = true;
            rbtnAdminTask.Text = adminTaskName;
            
            rbEPATasks.Text = ePATaskName;
            
            rbSpecialtyMed.Text = "Patient Access Services (" + SpecMedTaskCount.ToString() + ")";
        }

        private void CheckForStatusUpdate()
        {
            var activityId = hdnActivityId.Value;
            if (!string.IsNullOrWhiteSpace(activityId))
            {
                hdnActivityId.Value = "";
                MasterPage.UpdatePatientHeader(hiddenPatientID.Value.ToGuidOr0x0());
                mpeIFC.Hide();

                EPSBroker.GetStatusByActivityIdAndUpdateDatabase(SpecialtyMed.GenerateRecommendationContext(new SpecialtyMed(), new DXCUtils(), PageState), Convert.ToInt32(activityId), DBID);
            }
        }
        private void LoadAssistRxToken()
        {
            var iAssistTokenObject = PageState.Cast(Constants.SessionVariables.iAssistTokenObject, new GetiAssistSamlTokenResponse());
            if (string.IsNullOrEmpty(iAssistTokenObject.Base64SamlToken))
            {
                iAssistTokenObject = EPSBroker.GetiAssistSamlToken(PageState.GetStringOrEmpty(Constants.SessionVariables.ShieldIdentityToken), ShieldExternalTenantID);
            }

            if (DateTime.UtcNow > iAssistTokenObject.TokenRefreshTime)
            {
                iAssistTokenObject = EPSBroker.GetiAssistSamlToken(PageState.GetStringOrEmpty(Constants.SessionVariables.ShieldIdentityToken), ShieldExternalTenantID);
            }

            PageState[Constants.SessionVariables.iAssistTokenObject] = iAssistTokenObject;
        }

        public string SpecialtyPharmacyRowIndex
        {
            get
            {
                return Convert.ToString(ViewState["SpecialtyPharmacyRowIndex"]);
            }
            set
            {
                ViewState["SpecialtyPharmacyRowIndex"] = value;
            }
        }

        protected void loadProviders()
        {
            DataSet dsListProvider = new DataSet();
            DataView activeProvidersView = new DataView();
            bool isSelectedProviders = false;

            if (base.IsPOBUser & !base.IsPOBViewAllProviders)
            {
                isSelectedProviders = true;
                dsListProvider = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.SessionUserID,
                    base.DBID);
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
            ddlProvider.SelectedIndex = 0;
        }
        private void setPatientInfo(string patientID)
        {
            if (!string.IsNullOrEmpty(patientID))
            {
                ((PhysicianMasterPage)Master).SetPatientInfo(patientID);
                ((PhysicianMasterPage)Master).UpdatePatientHeader();
            }
            else
            {
                base.ClearPatientInfo();
                ((PhysicianMasterPage)Master).UpdatePatientHeader();
            }
            ucMessage.Visible = false;
            ucMessage.MessageText = string.Empty;
        }

        protected void hiddenSelectTask_Click(object sender, EventArgs e)
        {
            setPatientInfo(hiddenPatientID.Value);

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

        protected void rbtnePA_CheckedChanged(object sender, EventArgs e)
        {
            Server.Transfer(Constants.PageNames.TASK_SUMMARY + "?TaskType=" +
                                Convert.ToInt16(Constants.PrescriptionTaskType.EPA).ToString() + "&ePATaskStatus=3");
        }

        protected void ddlProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            int SpecialtyMedTaskCount;

            SpecialtyMedTaskObjDataSource.SelectParameters.Clear();

            if (ddlProvider.SelectedIndex == 0)
            {
                SpecialtyMedTaskObjDataSource.SelectParameters.Add("licenseId", Session["LicenseID"].ToString());

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("physicianID", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("pobID", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("patientID", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("SSOMode", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("taskType", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("dbID", Session["DBID"].ToString());
                SpecialtyMedTaskObjDataSource.SelectParameters.Add("UserID", Session["USERID"].ToString());
                SpecialtyMedTaskObjDataSource.Select();



                SpecialtyMedTaskCount =
                Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID,
                string.Empty, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode,
                base.DBID, base.SessionUserID);

                rbSpecialtyMed.Text = "Patient Access Services (" + SpecialtyMedTaskCount.ToString() + ")";
            }

            else
            {
                SpecialtyMedTaskObjDataSource.SelectParameters.Add("licenseId", Session["LicenseID"].ToString());

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("physicianID", ddlProvider.SelectedValue);

                Session["DelegateProviderID"] = ddlProvider.SelectedValue;
                SpecialtyMedTaskObjDataSource.SelectParameters.Add("pobID", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("patientID", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("SSOMode", string.Empty);

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("taskType", string.Empty);


                SpecialtyMedTaskObjDataSource.SelectParameters.Add("dbID", Session["DBID"].ToString());

                SpecialtyMedTaskObjDataSource.SelectParameters.Add("UserID", Session["USERID"].ToString());
                SpecialtyMedTaskObjDataSource.Select();

                SpecialtyMedTaskCount =
                Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID,
                ddlProvider.SelectedValue, base.SessionUserID, base.SessionPatientID, base.SessionSSOMode,
                base.DBID, base.SessionUserID);

                rbSpecialtyMed.Text = "Patient Access Services (" + SpecialtyMedTaskCount.ToString() + ")";
            }

        }

        protected void UpdateSpecialtyMedTaskCount()
        {
            int SpecialtyMedTaskCount =
                    Allscripts.Impact.SpecialtyMedData.GetSpecialtyMedTaskCount(base.SessionLicenseID, string.Empty,
                        string.Empty, base.SessionPatientID, base.SessionSSOMode, base.DBID, base.SessionUserID);

            rbSpecialtyMed.Text = "Patient Access Services (" + SpecialtyMedTaskCount.ToString() + ")";
        }


        protected void grdSpecialtyMedTasks_ItemCommand(object sender, GridCommandEventArgs e)
        {
        }
        public string RetrieveFormName(AttachmentTypes specialtyMedDocAttachmentType)
        {
            string fileName = "PatientForm.pdf";
            switch (specialtyMedDocAttachmentType)
            {
                case AttachmentTypes.PRIOR_AUTHORIZATION_FILE:
                    fileName = "PriorAuthorizationForm.pdf";
                    break;
                case AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE:
                    fileName = "SpecialtyMedEnrollmentForm.pdf";
                    break;
            }
            return fileName;
        }
        public void OpenDocument(DataSet documents, AttachmentTypes attachmentType)
        {
            if (documents != null && documents.Tables[0].Rows.Count > 0)
            {
                byte[] buffer = new Byte[10000];
                Stream inputStream = null;
                foreach (DataRow dr in documents.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr["SpecialtyMedAttachmentTypeID"]) == (int)attachmentType)
                    {
                        try
                        {
                            inputStream = new MemoryStream((byte[])dr["SpecialtyMedAttachmentBody"]);
                            string fileName = RetrieveFormName((AttachmentTypes)Enum.Parse(typeof(AttachmentTypes), Convert.ToString(dr["SpecialtyMedAttachmentTypeID"])));
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                            Response.ContentType = "application/pdf";
                            int count = inputStream.Read(buffer, 0, buffer.Length);
                            while (count > 0)
                            {
                                Response.BinaryWrite(buffer);
                                count = inputStream.Read(buffer, 0, buffer.Length);
                                if (!Response.IsClientConnected)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Response.Write("Error : " + ex.Message);
                        }
                        finally
                        {
                            if (inputStream != null)
                            {
                                inputStream.Close();
                            }
                        }
                    }
                }
                Response.Flush();
                Response.End();

            }
        }
        protected void linkButtonSpecialtyEnrollmentDoc_Click(object sender, EventArgs e)
        {
            //Retrieve the row information for which this linkbutton was clicked
            LinkButton btn = (LinkButton)sender;
            GridDataItem item = (GridDataItem)btn.NamingContainer;
            int i = Convert.ToInt32(item.RowIndex);
            if (!string.IsNullOrEmpty(item["RxTaskID"].Text))
            {
                //extract all docs associated with this RxTaskId
                SpecialtyMed specialtyMed = new SpecialtyMed();
                DataSet documents = specialtyMed.SpecialtyMedRetrieveDocumentsPerRxTaskID(Convert.ToInt64(item.GetDataKeyValue("RxTaskID")), DBID);
                OpenDocument(documents, AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE);
            }

        }
        protected void linkButtonPriorAuthorizationDoc_Click(object sender, EventArgs e)
        {
            //Retrieve the row information for which this linkbutton was clicked
            LinkButton btn = (LinkButton)sender;
            GridDataItem item = (GridDataItem)btn.NamingContainer;
            int i = Convert.ToInt32(item.RowIndex);
            if (!string.IsNullOrEmpty(item["RxTaskID"].Text))
            {
                //extract all docs associated with this RxTaskId
                SpecialtyMed specialtyMed = new SpecialtyMed();
                DataSet documents = specialtyMed.SpecialtyMedRetrieveDocumentsPerRxTaskID(Convert.ToInt64(item.GetDataKeyValue("RxTaskID")), DBID);
                OpenDocument(documents, AttachmentTypes.PRIOR_AUTHORIZATION_FILE);
            }

        }
        public void populateDocumentsDropDown(ref GridDataItem tempDataItem, ISpecialtyMed specialtyMed)
        {
            DataSet documents = specialtyMed.SpecialtyMedRetrieveDocumentsPerRxTaskID(Convert.ToInt64(tempDataItem.GetDataKeyValue("RxTaskID")), DBID);
            Dictionary<AttachmentTypes, bool> docsAvailability = specialtyMed.RetrieveDocumentsVisibility(documents);
            bool isSpecialtyEnrollmentFileAvailable = false;
            if (docsAvailability.TryGetValue(AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE, out isSpecialtyEnrollmentFileAvailable))
            {
                LinkButton linkButtonSpecialtyEnrollmentFile = tempDataItem.FindControl("LinkButtonSpecialtyEnrollmentForm") as LinkButton;
                linkButtonSpecialtyEnrollmentFile.Visible = isSpecialtyEnrollmentFileAvailable;
            }
            bool isPriorAuthFileAvailable = false;
            if (docsAvailability.TryGetValue(AttachmentTypes.PRIOR_AUTHORIZATION_FILE, out isPriorAuthFileAvailable))
            {
                LinkButton linkButtonPrioAuthFile = tempDataItem.FindControl("LinkButtonPriorAuthorizationForm") as LinkButton;
                linkButtonPrioAuthFile.Visible = isPriorAuthFileAvailable;
            }
        }
        protected void grdSpecialtyMedTasks_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var tempDataItem = e.Item as GridDataItem;
            if (tempDataItem != null)
            {
                //build the dropdownlist dynamically
                populateDestination(ref tempDataItem, new SpecialtyMed(), new Prescription());
                SpecialtyMed.EditDestinationDropdownControl(new Allscripts.Impact.Telerik(tempDataItem));
                SpecialtyMed.SetActionlinksForStatus(new Allscripts.Impact.Telerik(tempDataItem));
                SpecialtyMed.SetRadioButton(new Allscripts.Impact.Telerik(tempDataItem));

                DropDownList ddlDest = (DropDownList)tempDataItem.FindControl("ddlDest");

                if (ddlDest.Text == Constants.SpecialtyMedProcessedValue)
                {
                    ddlDest.Enabled = false;
                }
                string tooltipActivityId = tempDataItem.GetDataKeyValue("ActivityID").ToString();
                ddlDest.ToolTip = "ActivityID: " + tooltipActivityId;

                //Update the documents visibility
                DataRow[] foundRows;
                foundRows = TasksWithDocuments.Tables[0].Select("RxTaskID = " + Convert.ToInt64(tempDataItem.GetDataKeyValue("RxTaskID")));
                LinkButton linkButtonImgDoc = e.Item.FindControl("linkButtonImgDoc") as LinkButton;
                if (linkButtonImgDoc != null && foundRows != null && foundRows.Length > 0)
                {
                    linkButtonImgDoc.Visible = true;
                }
                populateDocumentsDropDown(ref tempDataItem, new SpecialtyMed());
            }
        }

        public void populateDestination(ref GridDataItem tempDataItem, ISpecialtyMed specialtyMed, IPrescription prescription)
        {
            DropDownList ddlDestination = (DropDownList)tempDataItem.FindControl("ddlDest");
            SpecialtyMedDestinationOptionsParameters parameters = new SpecialtyMedDestinationOptionsParameters();

            String mobPharmID = Convert.ToString(tempDataItem.GetDataKeyValue("mobPharmID"));
            String lastPharmacyID = Convert.ToString(tempDataItem.GetDataKeyValue("LastPharmacyID"));
            int RX_DETAIL_status = Convert.ToInt16(tempDataItem.GetDataKeyValue("RX_DETAIL_status"));
            String providerID = Convert.ToString(tempDataItem.GetDataKeyValue("ProviderID"));

            parameters.HasLastPharmacy = !string.IsNullOrEmpty(lastPharmacyID) && lastPharmacyID != Guid.Empty.ToString();
            parameters.HasValidMobPharmacy = !string.IsNullOrEmpty(mobPharmID) && mobPharmID != Guid.Empty.ToString();
            parameters.RX_DETAIL_status = RX_DETAIL_status;
            parameters.ProviderSPI = getSPI(providerID);   //PageState.GetStringOrEmpty(Constants.SessionVariables.SPI);
            parameters.RxID = Convert.ToString(tempDataItem.GetDataKeyValue("RxID"));
            parameters.DBID = DBID;
            parameters.IsLimitedDistributionMedication = Convert.ToBoolean(tempDataItem.GetDataKeyValue("IsLimitedDistributionMedication"));
            DataTable dtRxDetails = prescription.GetRXDetails(parameters.RxID, DBID).Tables[0];
            if (dtRxDetails != null && dtRxDetails.Rows.Count > 0)
            {
                DataRow drRX = dtRxDetails.Rows[0];
                if (!string.IsNullOrWhiteSpace(Convert.ToString(drRX["ControlledSubstanceCode"])))
                {
                    parameters.IsControlledSubstanceMedication = true;
                }
            }
            foreach (KeyValuePair<string, string> destination in specialtyMed.RetrieveDestinationsForSpecialtyMedTasks(parameters))
            {
                ddlDestination.Items.Add(new ListItem(destination.Key, destination.Value));
            }
        }

        protected string getSPI(string providerId)
        {
            string sessionSPI = PageState.GetStringOrEmpty(Constants.SessionVariables.SPI);
            if (!string.IsNullOrEmpty(sessionSPI))
            {
                return sessionSPI;
            }
            else
            {
                DataSet dsSPI = Provider.GetSPI(providerId, base.DBID);
                if (dsSPI.Tables.Count > 0)
                {
                    DataRow[] drSPI = dsSPI.Tables[0].Select("ScriptSwId='SURESCRIPTS'");
                    //should only be one row for SURESCRIPTS...grab the first and only
                    if (drSPI.Length > 0 && drSPI[0] != null && drSPI[0]["SenderId"] != DBNull.Value &&
                        drSPI[0]["SenderId"].ToString() != string.Empty)
                    {
                        return drSPI[0]["SenderID"].ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        protected void grdSpecialtyMedTasks_RowBound(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow rowSelect = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
            int rowindex = rowSelect.RowIndex;
        }
        protected void grdSpecialtyMedTasks_DataBound(object sender, EventArgs e)
        {

        }


        protected void btnBack_Click(object sender, EventArgs e)
        {

            Server.Transfer(Constants.PageNames.DOC_REFILL_MENU);
        }

        protected void btnProcessTask_Click(object sender, EventArgs e)
        {
            if (grdSpecialtyMedTasks.SelectedItems.Count != 1)
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "Please select a task to process.";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
            else
            {
                GridDataItem selectedItem = (GridDataItem)grdSpecialtyMedTasks.SelectedItems[0];
                DropDownList ddlDest = (DropDownList)selectedItem.FindControl("ddlDest");
                if (ddlDest.SelectedValue == Constants.SpecialtyMedProcessedValue)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Selected task has already been processed.";
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                }
                else if (ddlDest.SelectedValue == "SPECIALTYMEDBLANK")
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "Please select a destination.";
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                }
                else
                {
                    mpeConfirmProcess.Show();
                }
            }
        }

        private void processTask()
        {
            if (grdSpecialtyMedTasks.SelectedItems.Count == 1)
            {
                GridDataItem selectedItem = (GridDataItem)grdSpecialtyMedTasks.SelectedItems[0];
                ITelerik selectedItelerik = new Allscripts.Impact.Telerik(selectedItem);
                string redirectString = String.Empty;

                SpecialtyMed specialtyMed = new SpecialtyMed();

                SpecialtyMedTaskProcessResult specialtyMedTaskProcessResult = specialtyMed.ProcessTask(selectedItelerik, PageState, Request.UserIpAddress());
                redirectString = specialtyMedTaskProcessResult.RedirectString;

                if (redirectString != String.Empty)
                {
                    string selectedRxID = selectedItelerik.GetDataKeyValue("RxID").ToString();
                    redirectPrint(redirectString, selectedRxID);

                }
                ucMessage.Visible = specialtyMedTaskProcessResult.MessageVisible;
                ucMessage.MessageText = specialtyMedTaskProcessResult.MessageText;
                ucMessage.Icon = (Controls_Message.MessageType)specialtyMedTaskProcessResult.MessageType;
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "Please select a task to process.";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }

            ClearPatientInfo();
            ((PhysicianMasterPage)Master).UpdatePatientHeader();
            SpecialtyMedTaskObjDataSource.Select();
        }


        private void redirectPrint(string postProcess, string selectedRxID)
        {
            ArrayList alProcess = new ArrayList();
            alProcess.Add(selectedRxID);
            PageState["ProcessList"] = alProcess;
            PageState["IsWebPrint"] = true;
            PageState["CameFrom"] = Constants.PageNames.SPECIALTYMEDTASKS;
            Response.Redirect(postProcess + "?Msg=" + Server.UrlEncode("Rx Printed"));
        }

        private void sendPrescription(string rxID, string rxStatus, int refills, int daysSupply, string pharmacy, bool isLimitedOrMailOrderPharmacy)
        {
            string auditLogPatientID = string.Empty;

            ePrescribeSvc.AuditLogPatientRxResponse rxResponse =
            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID, rxID);

            if (rxResponse.Success)
            {
                auditLogPatientID = rxResponse.AuditLogPatientID;
            }

            Prescription.MarkAsFulfilled(base.SessionLicenseID, base.SessionUserID, rxID, refills, daysSupply, rxStatus, pharmacy, isLimitedOrMailOrderPharmacy, false, null, false, base.DBID);

            string smid = ScriptMessage.CreateScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, base.SessionLicenseID, base.SessionUserID, base.ShieldSecurityToken, base.SessionSiteID, base.DBID);
            long serviceTaskID = -1;
            if (!string.IsNullOrEmpty(smid) && Session["STANDING"].ToString() == "1")
            {
                serviceTaskID = ScriptMessage.SendThisMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }

            smid = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, base.DBID);
            if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(smid)))
            {
                ScriptMessage.SendOutboundInfoScriptMessage(smid, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }

            //This will be used from service manager and added to last audit log when message is sent to hub.
            if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
            {
                Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, base.DBID);
            }
            ucMessage.MessageText = "Prescription sent to pharmacy";
            ucMessage.Visible = true;
            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
        }

        protected void hiddenSelect_Click(object sender, EventArgs e)
        {
            ucMessage.Visible = false;
            ucMessage.MessageText = string.Empty;
            ucMessage.Icon = Controls_Message.MessageType.SUCCESS;

            GridDataItem selectedItem = (GridDataItem)grdSpecialtyMedTasks.SelectedItems[0];
            DropDownList ddlDest = (DropDownList)selectedItem.FindControl("ddlDest");

            string selectedPatientID = selectedItem["PatientGUID"].Text;

            if (!string.IsNullOrWhiteSpace(selectedPatientID) && selectedPatientID != Guid.Empty.ToString())
            {
                ((PhysicianMasterPage)Master).SetPatientInfo(selectedPatientID);
                ((PhysicianMasterPage)Master).UpdatePatientHeader();
            }
        }

        protected void rbgFilterSpecMed_CheckedChanged(object sender, EventArgs e)
        {
            SpecialtyMedTaskObjDataSource.SelectParameters["TaskType"].DefaultValue = rbtnOpen.Checked ? "1" : rbtnResolved.Checked ? "2" : "3";
            grdSpecialtyMedTasks.Rebind();
            btnProcessTask.Enabled = !rbtnResolved.Checked;
            ucMessage.Visible = false;
            ucMessage.MessageText = String.Empty;
        }

        protected void hdnDestinationSelected_OnClick(object sender, EventArgs e)
        {
            logger.Debug("hdnDestinationSelected_OnClick() - Method Entry");
            var hdnDestinationSelected = sender as Button;
            var rowIndex = hdnDestinationSelected?.Attributes["RowIndex"];

            if (grdSpecialtyMedTasks.SelectedItems.Count == 1)
            {
                ucMessage.Visible = false;
                var ddi = hdnDestinationSelected?.Attributes["DDI"];
                logger.Debug("hdnDestinationSelected_OnClick() - DDI: {0} RowIndex: {1}", ddi, rowIndex);
                if (!string.IsNullOrEmpty(ddi))
                {
                    SpecialtyPharmacyRowIndex = rowIndex;
                    logger.Debug(
                        "hdnDestinationSelected_OnClick() - Calling Allscripts.ePrescribe.Data.Pharmacy.GetSpecialtyRxPharmacysForDdi");
                    List<SpecialtyPharmacy> specialtyMedPharmacy =
                        Allscripts.ePrescribe.Data.Pharmacy.GetSpecialtyRxPharmacysForDdi(ddi, DBID);
                    grdSpecPharm.DataSource = specialtyMedPharmacy;
                    logger.Debug(
                        "hdnDestinationSelected_OnClick() - DataSource count: {0} returned from Allscripts.ePrescribe.Data.Pharmacy.GetSpecialtyRxPharmacysForDdi",
                        specialtyMedPharmacy.Count);
                    grdSpecPharm.DataBind();
                    mpeSpecialtyRxPharmacy.Show();
                }
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "Please select a task.";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                //setDdlto Selected
                ((DropDownList)grdSpecialtyMedTasks.Items[rowIndex].FindControl("ddlDest")).SelectedIndex = 0;
            }
            logger.Debug("hdnDestinationSelected_OnClick() - Method Exit");
        }

        protected void btnSpecialtyRxPharmacyOk_OnClick(object sender, EventArgs e)
        {
            lblPharmacyErrorMessage.Visible = false;
            var selectedItems = grdSpecPharm.SelectedItems;
            if (selectedItems.Count == 1)
            {
                var hdnLimitedPharmacySelected = grdSpecialtyMedTasks.Items[SpecialtyPharmacyRowIndex]?.FindControl("hdnLimitedPharmacySelected") as HiddenField;
                if (hdnLimitedPharmacySelected != null)
                {
                    hdnLimitedPharmacySelected.Value = Convert.ToString(grdSpecPharm.MasterTableView.Items[grdSpecPharm.SelectedItems[0].ItemIndexHierarchical].GetDataKeyValue("Id"));
                    PageState[Constants.SessionVariables.LTD_PHARMACY_ID] = hdnLimitedPharmacySelected.Value;
                    btnProcessTask.Enabled = true;
                }
            }
            else
            {
                lblPharmacyErrorMessage.Visible = true;
                mpeSpecialtyRxPharmacy.Show();
            }
        }

        protected void btnCancel_OnClick(object sender, EventArgs e)
        {
            var ddlDestination = grdSpecialtyMedTasks.Items[SpecialtyPharmacyRowIndex]?.FindControl("ddlDest") as DropDownList;
            if (ddlDestination != null) ddlDestination.SelectedIndex = 0;
        }

        protected void btnOk_OnClick(object sender, EventArgs e)
        {
            if (grdSpecialtyMedTasks.SelectedItems.Count == 1)
            {
                processTask();
            }
            else
            {
                ucMessage.Visible = true;
                ucMessage.MessageText = "Please select a task to process.";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
            }
        }
    }
}
