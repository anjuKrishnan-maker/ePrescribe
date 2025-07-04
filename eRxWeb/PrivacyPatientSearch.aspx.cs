using System;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Security.Application;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using PatientPrivacy = Allscripts.Impact.PatientPrivacy;

namespace eRxWeb
{
	
    public partial class PrivacyPatientSearch : BasePage
    {

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
		{
             ucMessage.Visible = false;
		}
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }       

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string strDOB = string.Empty;
            if (rdiDOB.SelectedDate == null && rdiDOB.InvalidTextBoxValue == string.Empty && txtLastName.Text == string.Empty && txtFirstName.Text == string.Empty && txtPatientID.Text == string.Empty)
            {
                errorMessage.Text = "* Please enter Last Name, First Name or Patient ID or enter a Date of Birth.";
                return;
            }
            else if (rdiDOB.InvalidTextBoxValue != string.Empty)
            {
                errorMessage.Text = "* Date of Birth must be in the format mm/dd/yyyy.";
                return;
            }
            else
            {
                errorMessage.Text = string.Empty;
            }

            if (rdiDOB.SelectedDate != null)
            {
                strDOB = ((DateTime)(rdiDOB.SelectedDate)).ToString("MM/dd/yyyy");
            }
            grdViewPatients.DataSourceID = "PatObjDataSource";
            PatObjDataSource.SelectParameters["LastName"].DefaultValue = txtLastName.Text;
            PatObjDataSource.SelectParameters["FirstName"].DefaultValue = txtFirstName.Text;
            PatObjDataSource.SelectParameters["DateOfBirth"].DefaultValue = strDOB;
            PatObjDataSource.SelectParameters["ChartID"].DefaultValue = txtPatientID.Text;
            PatObjDataSource.SelectParameters["includeInactive"].DefaultValue = "false";
            PatObjDataSource.SelectParameters["HasVIPPatients"].DefaultValue = base.SessionLicense.hasVIPPatients.ToString();          

            grdViewPatients.DataBind();
        }        

        protected void btnManagePrivacyRestrictions_Click(object sender, EventArgs e)
        {
            ucPrivacyMessageStatus.Visible = false;
            chkAlias.Checked = false;
            bool isVIP = (bool)grdViewPatients.SelectedValues["IsVIPPatient"];
            if(isVIP)
            {
                getPatientAlias();
            }
            else
            {
                rqAliasFirstName.Enabled = false;
                rqAliasLastName.Enabled = false;
            }
         
                getRestrictedUsersFromDB();
                loadActiveUsers();
            chkAlias.Text = "Safeguard patient name " + "\"" + Encoder.HtmlEncode(hiddenPatientName.Value.ToString().Trim()) + "\"" + " and display an alias instead. "
                            + "Alias restriction will apply to all users checkmarked below.";
            lblSelectedPatientHeader.Text = "Privacy Restrictions for " + Encoder.HtmlEncode(hiddenPatientName.Value.ToString().Trim());
            lblSelectedPatient.Text = "Restrict selected users from accessing patient data for " + Encoder.HtmlEncode(hiddenPatientName.Value.ToString().Trim()) + ":";
            mpeRestrictedUser.Show();
        }

        protected void getPatientAlias()
        {
            DataRow dr = Allscripts.ePrescribe.Data.Patient.GetPatientAlias(grdViewPatients.SelectedValue.ToString(), base.DBID);
            txtAliasFirstName.Text = dr[0].ToString();
            txtAliasFirstName.Attributes.Remove("Disabled");
            txtAliasLastName.Text = dr[1].ToString();
            txtAliasLastName.Attributes.Remove("Disabled");
            chkAlias.Checked = true;
            rqAliasFirstName.Enabled = true;
            rqAliasLastName.Enabled = true;

        }

        protected void grdViewPatients_RowDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (GridDataItem)e.Item;
                tempDataItem.Style["cursor"] = "pointer";
                HtmlInputRadioButton rbSelect = tempDataItem.FindControl("rbSelect") as HtmlInputRadioButton;
                if (rbSelect != null)
                {
                    rbSelect.Attributes.Add("PatientID", tempDataItem.GetDataKeyValue("PatientID").ToString());
                    rbSelect.Attributes.Add("onclick", "patientSelectRadio('" + tempDataItem.GetDataKeyValue("PatientID").ToString() + "', '" + tempDataItem.GetDataKeyValue("Name").ToString() + "')");                    
                }
                           if ((bool)tempDataItem.GetDataKeyValue("IsVIPPatient") || (bool)tempDataItem.GetDataKeyValue("IsRestrictedPatient"))
                {
                    Image img = new Image();
                    img.ImageUrl = @"~\images\PrivacyImages\sensitivehealth-global-16-x-16.png";
                    tempDataItem["ImgColumn"].Controls.Add(img);
                    Label lbl = new Label();
                    lbl.Text = " " + StringHelper.ConvertToUxName(tempDataItem.Cells[4].Text);
                    tempDataItem["ImgColumn"].Controls.Add(lbl);
                }
            }
        }
        protected void rgRestrictedUser_PageIndexChanged(object source, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            rgRestrictedUser.CurrentPageIndex = e.NewPageIndex;
            loadActiveUsers();
        }

        protected void rgRestrictedUser_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;

                if (Session["listRestrictedUsersFromDb"] != null)
                {
                    string s = item.GetDataKeyValue("UserGUID").ToString().ToUpper();
                    if (((List<string>)Session["listRestrictedUsersFromDb"]).Contains(item.GetDataKeyValue("UserGUID").ToString().ToUpper()))
                    {
                        item.Selected = true;
                    }

                }
                
            }
        }
     
        protected void btnRestrictedUserSave_Click(object sender, EventArgs e)
        {
            bool isVIP = (bool)grdViewPatients.SelectedValues["IsVIPPatient"];
            ucPrivacyMessageStatus.Icon = Controls_Message.MessageType.SUCCESS;

            if (chkAlias.Checked)
            {
                DataRow dr = SavePatientAlias(true, isVIP);
                ucPrivacyMessageStatus.MessageText = string.Format(dr[0].ToString());
                ucPrivacyMessageStatus.Visible = true;
                if (ucPrivacyMessageStatus.MessageText.Contains("already exists"))
                {
                    ucPrivacyMessageStatus.Icon = Controls_Message.MessageType.ERROR;
                    grdViewPatients.SelectedValues["IsVIPPatient"] = false;
                }
                else
                {
                    grdViewPatients.SelectedValues["IsVIPPatient"] = true;
                }
                savePatientPrivacyRequest();
                getRestrictedUsersFromDB();
                base.SessionLicense.hasVIPPatients = true;
            }
            else if(!chkAlias.Checked && isVIP)
            {
                DataRow dr = SavePatientAlias(false, isVIP);
                ucPrivacyMessageStatus.MessageText = string.Format(dr[0].ToString());
                ucPrivacyMessageStatus.Visible = true;
                mpeRestrictedUser.Show();
                savePatientPrivacyRequest();
                getRestrictedUsersFromDB();
                base.SessionLicense.hasVIPPatients = Convert.ToBoolean(dr[1]);
                grdViewPatients.SelectedValues["IsVIPPatient"] = false;
                rqAliasFirstName.Enabled = false;
                rqAliasLastName.Enabled = false;
            }
            else
            {
                ucPrivacyMessageStatus.MessageText = "Successfully Saved";
                ucPrivacyMessageStatus.Visible = true;
                savePatientPrivacyRequest();
                getRestrictedUsersFromDB();
                rqAliasFirstName.Enabled = false;
                rqAliasLastName.Enabled = false;
            }

            loadActiveUsers();

        }

        protected DataRow SavePatientAlias(bool status, bool isVIP)
      
        {
            return Allscripts.ePrescribe.Data.Patient.SavePatientAlias
                (grdViewPatients.SelectedValue.ToString(),
                PageState.GetStringOrEmpty("userID"),
                (int)ePrescribeSvc.ePrescribeApplication.MainApplication,
                SessionLicenseID,
                txtAliasFirstName.Text,
                txtAliasLastName.Text,
                isVIP,
                status,
                Request.UserIpAddress(),base.DBID);
        }

        protected void btnRestrictedUserSaveNClose_Click(object sender, EventArgs e)
        {
            bool isVIP = (bool)grdViewPatients.SelectedValues["IsVIPPatient"];
            ucPrivacyMessageStatus.Icon = Controls_Message.MessageType.SUCCESS;

            if (chkAlias.Checked)
            {
                DataRow dr = SavePatientAlias(true , isVIP);
                ucMessage.MessageText = string.Format(dr[0].ToString());
                ucPrivacyMessageStatus.MessageText = string.Format(dr[0].ToString());
                if (ucPrivacyMessageStatus.MessageText.Contains("already exists"))
                {
                    ucPrivacyMessageStatus.Visible = true;
                    chkAlias.Checked = true;
                    txtAliasFirstName.Attributes.Remove("disabled");
                    txtAliasLastName.Attributes.Remove("disabled");
                    ucPrivacyMessageStatus.Icon = Controls_Message.MessageType.ERROR;
                    mpeRestrictedUser.Show();
                    savePatientPrivacyRequest();
                }
                else
                {
                    ucMessage.Visible = true;
                    mpeRestrictedUser.Hide();
                    grdViewPatients.Rebind();
                    savePatientPrivacyRequest();
                    base.SessionLicense.hasVIPPatients = true;
                }
            }
            else if (!chkAlias.Checked && isVIP)
            {
                DataRow dr = SavePatientAlias(false, isVIP);
                ucMessage.Visible = true;
                ucMessage.MessageText = string.Format(dr[0].ToString());
                savePatientPrivacyRequest();
                mpeRestrictedUser.Hide();
                grdViewPatients.Rebind();
                base.SessionLicense.hasVIPPatients = Convert.ToBoolean(dr[1]);
            }

            else
            {
                savePatientPrivacyRequest();
                getRestrictedUsersFromDB();
                loadActiveUsers();
                ucMessage.Visible = true;
                ucMessage.MessageText = "Successfully Saved";
                Session.Remove("listRestrictedUsersFromDb");
                grdViewPatients.Rebind();
            }
                    
        }

        protected void btnRestrictedUserCancel_Click(object sender, EventArgs e)
        {
            mpeRestrictedUser.Hide();
            Session.Remove("listRestrictedUsersFromDb");
            grdViewPatients.Rebind();
        }


        #endregion

        #region Page Methods
        
        private void loadActiveUsers()
        {
            DataSet ds = Allscripts.ePrescribe.Data.AuditLog.GetActiveUsersByLicense(base.SessionLicenseID, base.DBID);
            rgRestrictedUser.DataSource = ds;
            rgRestrictedUser.DataBind();
        }

        private void getRestrictedUsersFromDB()
        {
            if (Session["listRestrictedUsersFromDb"] == null)
            {
                List<string> listRestrictedUsersFromDb = new List<string>();
                DataTable dtRestrictedUsersFromDb = new PatientPrivacy().GetPatientPrivacyRequestID(hiddenPatientID.Value.ToString(), null, base.DBID);
                if (dtRestrictedUsersFromDb.Rows.Count > 0)
                {
                    foreach (DataRow row in dtRestrictedUsersFromDb.Rows)
                    {
                        listRestrictedUsersFromDb.Add(row["UserGUID"].ToString().ToUpper());
                    }
                }

                if (listRestrictedUsersFromDb.Count > 0)
                {
                    Session["listRestrictedUsersFromDb"] = listRestrictedUsersFromDb;
                }
                else
                {
                    listRestrictedUsersFromDb = null;
                }
            }
        }

        private static void CheckRestrictedUsers(GridDataItem item, DataTable dtRestrictedUsers)
        {
            if (dtRestrictedUsers.Rows.Count > 0)
            {
                foreach (DataRow r in dtRestrictedUsers.Rows)
                {
                    string stat = r["State"].ToString();
                    if (stat == item.GetDataKeyValue("State").ToString())
                    {
                        item.Selected = true;
                        // btnSelectState.Enabled = true;
                    }
                }
            }
        }

        private void savePatientPrivacyRequest()
        {
            List<String> listRestrictedUsersFromDb = new List<string>();
            if (Session["listRestrictedUsersFromDb"] != null)
            {
                listRestrictedUsersFromDb = (List<string>)Session["listRestrictedUsersFromDb"];
            }
            List<String> deselectedList = new List<string>();
            foreach (GridDataItem items in rgRestrictedUser.MasterTableView.Items)
            {
                if (!items.Selected && listRestrictedUsersFromDb.Contains(items.GetDataKeyValue("UserGUID").ToString().ToUpper()))
                {
                    listRestrictedUsersFromDb.Remove(items.GetDataKeyValue("UserGUID").ToString().ToUpper());
                }

                if (items.Selected && !listRestrictedUsersFromDb.Contains(items.GetDataKeyValue("UserGUID").ToString().ToUpper()))
                {
                    listRestrictedUsersFromDb.Add(items.GetDataKeyValue("UserGUID").ToString().ToUpper());
                }
            }

            new PatientPrivacy().SavePatientPrivacyRequest(hiddenPatientID.Value.ToString(), listRestrictedUsersFromDb, (int)ePrescribeSvc.ePrescribeApplication.MainApplication, base.SessionLicenseID, base.SessionUserID, Request.UserIpAddress(), base.DBID);
            Session.Remove("listRestrictedUsersFromDb");
        }

        #endregion

        protected void btnBack_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(AngularStringUtil.CreateUrl(Constants.PageNames.SETTINGS));
        }
    }
}