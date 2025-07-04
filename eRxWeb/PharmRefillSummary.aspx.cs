using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Telerik.Web.UI;

namespace eRxWeb
{
public partial class PharmRefillSummary : BasePage
{
    #region Properties

    private string _scriptMessageID
    {
        get
        {
            if (ViewState["ScriptMessageID"] == null)
            {
                ViewState["ScriptMessageID"] = true;
            }

            return ViewState["ScriptMessageID"].ToString();
        }
        set
        {
            ViewState["ScriptMessageID"] = value;
        }
    }

    private string _patientID
    {
        get
        {
            if (ViewState["PatientID"] == null)
            {
                ViewState["PatientID"] = true;
            }

            return ViewState["PatientID"].ToString();
        }
        set
        {
            ViewState["PatientID"] = value;
        }
    }

    #endregion 

    #region Page/Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        int tasks = 0;
    
        ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);

        if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
        {
            PharmRefillObjDataSource.SelectParameters["patientID"].DefaultValue = Session["PatientID"].ToString();
        }
            PharmRefillObjDataSource.SelectParameters["userID"].DefaultValue = base.SessionUserID;

            if (!Page.IsPostBack)
        {
                PageState.Remove(Constants.SessionVariables.IsReconcileREFREQNonCS);
                base.ClearPatientInfo();

            if (base.IsPOBUser & !base.IsPOBViewAllProviders)
            {
                PharmRefillObjDataSource.SelectParameters["pobID"].DefaultValue = base.SessionUserID;
            }

            loadTaskFilter();
            loadProviders();

            if (Session["IsPASupervised"] != null && Convert.ToBoolean(Session["IsPASupervised"]))
            {
                setSupervisingProviderMessage();
            }

            
            string messageText = string.Empty;

            if (Request.QueryString["Msg"] != null)
            {
                messageText = HttpUtility.UrlDecode(Request.QueryString["Msg"]);
            }
            else
            {
                var refillMsg = PageState.GetStringOrEmpty(Constants.SessionVariables.RefillMsg);
                if (!string.IsNullOrEmpty(refillMsg))
                {
                    messageText = refillMsg;
                    PageState.Remove(Constants.SessionVariables.RefillMsg);
                }
            }

            if (!string.IsNullOrEmpty(messageText))
            {
                ucMessage.MessageText = messageText;

                if (messageText.Contains("approved") || messageText.Contains("denied") || messageText.Contains("Controlled substance refill printed for faxing"))
                {
                    ucMessage.Icon = Controls_Message.MessageType.SUCCESS;
                }
                else if (messageText.Contains("You do not have permission"))
                {
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                }
                else
                {
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }

                ucMessage.Visible = true;
                HttpContext.Current.RewritePath(Request.Path, Request.PathInfo, string.Empty);
            }

            rbEPATasks.Visible = base.ShowEPA;
        }
    }

    protected void grdPharmRefill_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem tempDataItem = (GridDataItem)e.Item;
            DataTable dtRefillRequest = ((DataRowView)tempDataItem.DataItem).DataView.Table;
            DataRow dr = dtRefillRequest.Rows[tempDataItem.ItemIndex];

            string ddi = dr["DDI"].ToString().Trim();

            //display brand med in bold.
            if (!(string.IsNullOrEmpty(ddi)))
            {
                bool isGeneric = Allscripts.Impact.Medication.IsGenericMed(ddi, base.DBID);
                if (!isGeneric)
                {
                    e.Item.Cells[3].Font.Bold = true;
                }
            }
        }
    }

    protected void grdPharmRefill_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName == "ViewDetails")
        {
            string patientID = grdPharmRefill.Items[e.Item.ItemIndex].GetDataKeyValue("PatientID").ToString();
            if (!string.IsNullOrWhiteSpace(patientID) && patientID != Guid.Empty.ToString())
            {
                ((PhysicianMasterPage)Master).SetPatientInfo(patientID);
            }
            else
            {
                ((PhysicianMasterPage)Master).ClearPatientInfo();
            }
            
            ucObsoletePARClassMappingChange.LoadPatientActiveObsoletePARClass();
            if (!ucObsoletePARClassMappingChange.ShowIfPatientHasActiveObsoletedParClass())
            {
                RxUser provider = new RxUser(e.CommandArgument.ToString(), base.DBID);
                if (provider != null)
                {
                    string scriptMessageID = grdPharmRefill.Items[e.Item.ItemIndex].GetDataKeyValue("ScriptMessageID").ToString();
                    if (provider.UserType == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                    {
                        // save for later response.redirect querystring
                        _scriptMessageID = scriptMessageID;
                        _patientID = patientID;

                        setProviderInformation(provider);

                        loadSupervisingProviders();

                        // have the POB user select a supervising provider for the supervised physician assistant
                        mpeSetSupervisingProvider.Show();
                    }
                    else if (setProviderInformation(provider))
                    {
                        Response.Redirect("PharmRefillDetails.aspx?MessageID=" + scriptMessageID + "&PatientID=" + patientID + "&ProviderID=" + e.CommandArgument.ToString());
                    }
                }
            }
        }
    }
    
    protected void rbAdminTask_Changed(object sender, EventArgs e)
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

        protected void btnPharmRefillReport_Click(object sender, EventArgs e)
    {
        launchReport("PharmRefillReport");
    }

    protected void ddlProvider_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlProvider.SelectedIndex == 0)
        {
            PharmRefillObjDataSource.SelectParameters["licenseID"].DefaultValue = Session["LicenseID"].ToString();
            PharmRefillObjDataSource.SelectParameters["providerID"].DefaultValue = null;
            Session["ProviderID"] = null;
        }
        else
        {
            PharmRefillObjDataSource.SelectParameters["licenseID"].DefaultValue = Session["LicenseID"].ToString();
            PharmRefillObjDataSource.SelectParameters["providerID"].DefaultValue = ddlProvider.SelectedValue;
            Session["ProviderID"] = ddlProvider.SelectedValue;
        }

        PharmRefillObjDataSource.Select();
    }
    
    protected void btnSetSupervisingProvider_Click(object sender, EventArgs e)
    {
        RxUser superVisiorProvider = new RxUser(ddlSupervisingProvider.SelectedValue, base.DBID);

        if (setSupervisingProviderInformation(superVisiorProvider))
        {
            Response.Redirect("PharmRefillDetails.aspx?MessageID=" + _scriptMessageID + "&PatientID=" + _patientID + "&ShowSupProv=Y");
        }
    }

    #endregion

    #region Private Methods

    protected void launchReport(String REPORT_ID)
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

    protected void loadProviders()
    {
        DataSet dsListProvider = new DataSet();
        DataView activeProvidersView = new DataView();

        if (base.IsPOBUser & !base.IsPOBViewAllProviders)
        {
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

        if (base.IsPOBUser & !base.IsPOBViewAllProviders)
        {
            ddlProvider.Items.Insert(0, new ListItem("All Associated Providers", "All Associated Providers"));
        }
        else
        {
            ddlProvider.Items.Insert(0, new ListItem("All Providers", "All"));
        }

        if (Session["ProviderID"] != null)
        {
            ListItem providerItem = ddlProvider.Items.FindByValue(Session["ProviderID"].ToString());
            if (providerItem != null)
            {
                providerItem.Selected = true;
                PharmRefillObjDataSource.SelectParameters["providerID"].DefaultValue = ddlProvider.SelectedValue;
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
    }

    protected void loadTaskFilter()
    {
        int adminTaskType = (int)Constants.PrescriptionTaskType.SEND_TO_ADMIN;
        int adminTasks = 0;

        int tasks = 0;

        if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Constants.SSOMode.PATIENTLOCKDOWNMODE)
        {
            if (base.IsPOBUser & !base.IsPOBViewAllProviders)
            {
                adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, base.SessionPatientID, base.SessionUserID, adminTaskType, base.DBID, base.SessionUserID);

                tasks = Patient.GetTaskCountForPatient(base.SessionLicenseID, base.SessionPatientID, base.SessionUserID, base.DBID);
            }
            else
            {
                adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, base.SessionPatientID, adminTaskType, base.DBID, base.SessionUserID);

                tasks = Patient.GetTaskCountForPatient(base.SessionLicenseID, base.SessionPatientID, base.DBID);
            }

            ((PhysicianMasterPage)Master).toggleTabs("tasks", tasks);
        }
        else
        {
            if (base.IsPOBUser & !base.IsPOBViewAllProviders)
            {
                adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, new Guid(base.SessionUserID), adminTaskType, base.DBID, base.SessionUserID);
            }
            else
            {
                adminTasks = TaskManager.GetTaskListScriptCount(base.SessionLicenseID, adminTaskType, base.DBID, base.SessionUserID);
            }
        }

        string adminTaskName = "Assistant's Tasks (" + adminTasks.ToString() + ")";

        rbAdminTask.Text = adminTaskName;
        rbPharmRefills.Checked = true;
             
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
    }

    protected void loadSupervisingProviders()
    {
        DataSet drListProvider = Provider.GetProviders(base.SessionLicenseID, base.SessionSiteID, base.DBID);
        DataView activeProvidersView = drListProvider.Tables["Provider"].DefaultView;

        activeProvidersView.RowFilter = "UserType = '1' AND Active='Y'";

        activeProvidersView.Sort = "ProviderName ASC";
        ddlSupervisingProvider.DataTextField = "ProviderName";
        ddlSupervisingProvider.DataValueField = "ProviderID";
        ddlSupervisingProvider.DataSource = activeProvidersView;
        ddlSupervisingProvider.DataBind();
    }

    private bool setProviderInformation(RxUser provider)
    {
        List<string> deaSchedule = new List<string>();
        for (int i = 1; i < 6; i++)
        {
            if (provider.DEAScheduleAllowed(i))
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

        if (provider.HasNPI)
        {
            SetDelegateProvider(provider.UserID);
            return true;
        }
        else
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = "Please select a provider with a valid NPI.";
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            return false;
        }
    }

    private void SetDelegateProvider(string providerID)
    {
        Session["DelegateProviderID"] = providerID;

        if (base.IsPOBUser)
        {
            RxUser.UpdatePOBProviderUsage(base.SessionLicenseID, base.SessionSiteID, base.SessionUserID, providerID, base.DBID);
        }

        // this is the provider ID from the refill request
        setSPI(providerID);
    }

    private void setSPI(string providerID)
    {
        DataSet dsSPI = Provider.GetSPI(providerID, base.DBID);

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
    }

    private bool setSupervisingProviderInformation(RxUser selectedProvider)
    {
        List<string> deaSchedule = new List<string>();
        for (int i = 1; i < 6; i++)
        {
            if (selectedProvider.DEAScheduleAllowed(i))
            {
                deaSchedule.Add(i.ToString());
            }
        }

        Session["PASUPERVISOR_DEASCHEDULESALLOWED"] = deaSchedule;

        deaSchedule.Sort();
        int minsched = 0;

        if (deaSchedule.Count > 0)
        {
            minsched = Convert.ToInt32(deaSchedule[0]);
        }

        Session["PASUPERVISIOR_MINSCHEDULEALLOWED"] = minsched.ToString();

        if (selectedProvider.HasNPI)
        {
            SetSupervisingProvider(selectedProvider.UserID);
            return true;
        }
        else
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = "Please select a provider with a valid NPI.";
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            return false;
        }
    }

    private void SetSupervisingProvider(string providerID)
    {
        Session["SUPERVISING_PROVIDER_ID"] = providerID;
    }

    private void setSupervisingProviderMessage()
    {
        ucSupervisingProvider.MessageText = "Tasks are being processed under the supervision of " + base.DelegateProviderName + ".";
        ucSupervisingProvider.Icon = Controls_Message.MessageType.INFORMATION;
        ucSupervisingProvider.Visible = true;
    }

    #endregion
}

}