
/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
**08/13/2009   Sonal                    Task doesn't get deleted after clicking cancel 
 *                                      to pharmacy selection(issue#2592)
 *11/16/2009    Sonal                   Issue(#2973)- Change the patient review history page to allow edit 
 *                                      functionality to the patient pharmacy and patient mail
 *                                      order pharmacy screens.  
**11/21/2009	Anand Kumar			    Defect#2985: Set NABP value as session variable after saving tmail order pharmacy
**03/30/2010    Subhasis Nayak          #3371:Update rxdetail startdate and expiration date.
**04/07/2010	Anand Kumar Krishnan    Defect#3380: "Add Site Pharmacy" & "Add to Practice Favorite"
 *                                      functionality is added.
 *                                      Site Pharmacy is shown along with Practice Favorites on 
 *                                      Edit patient pharmacy flow.
**04/23/2010    Dharani Reddem          Defect# 3410 - Choose Pharmacy Page | Selecting the 'Mail Order Pharmacies' radio button after 
 *                                      selecting the 'Practice Favorites' radio button system hangs
**05/25/2010    Sonal                   #3469-Change "cancel/done" button inconsistencies
**08/19/2010    Subhasis Nayak          artf576035:Added & modified code to properly display retail and MO pharmacy detail. 
**09/10/2010    Matthew Molina          artf406013: Add ability to search for Mail Order Pharmacies. 
 *                                      Also removed commented out code and organized code into regions.
**02/09/2011    Subhasis Nayak          artf699129:Pharmacy search does not fetch results if a retail pharmacysearch is performed by 
 *                                      selecting Practice Favorites or All radio buttons
 *01/26/2012    Harit Shah              Adding special case for Print Center and PR state task processing.
*******************************************************************************/
using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Xml;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using System.Collections.Specialized;
using eRxWeb.AppCode.StateUtils;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
public partial class Pharmacy : BasePage
{
    #region Member Variables

    string licenseID = string.Empty;
    string rxID = Guid.NewGuid().ToString();
    string patientID = "";
    string pharmacyID = "";
    bool doNotAddToFavsClicked = false;

    #endregion

    #region Handled Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "Edit")
        {
            lblInfo.Text = "To add or modify this patient's pharmacy, search for and select a pharmacy. Then click 'Set as Patient Pharmacy'.";
            btnSetPatientPharm.Visible = true;
            btnNewRx.Visible = false;
            btnSendScript.Visible = false;
            btnContinue.Visible = false;
            btnPrint.Visible = false;
        }
        else
        {
            btnSetPatientPharm.Visible = false;
        }

        // subscribe the OnDigitalSigning event handler
        this.ucEPCSDigitalSigning.OnDigitalSigningComplete += new Controls_EPCSDigitalSigning.DigitalSigningCompleteHandeler(ucEPCSDigitalSigning_OnDigitalSigningComplete);
        this.ucCSMedRefillRequestNotAllowed.OnPrintRefillRequest += new EventHandler(ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest);
        //base.SetSingleClickButton(btnSendScript);

        //Cant use base method here because it is causing a validation check and it check validation on search controls.
        StringBuilder sb = new StringBuilder();
        sb.Append("this.value = 'Processing...';");
        //Prevent the button from being pressed a second time.
        sb.Append("this.disabled = true;");
        //Forces the page to postback.
        sb.Append(ClientScript.GetPostBackEventReference(btnSendScript, null));
        sb.Append(";");
        btnSendScript.Attributes.Add("onclick", sb.ToString());

        
        if (Session["PHARMACYID"] == null && Session["LASTPHARMACYID"] == null && Session["MOB_NABP"] == null)
        {
            if ((Request.QueryString["from"] != null && Request.QueryString["from"].ToLower().ToString() == Constants.PageNames.SCRIPT_PAD.ToLower() && Request.QueryString["FromPharmacy"] == null))
            {
                btnCancelRx.Text = "Cancel";
                btnCancel.Text = "Cancel";
            }
        }

        if (!Page.IsPostBack)
        {
            this.Form.DefaultButton = btnGo.UniqueID;

            txtNameSearch.Focus();
            btnSendScript.Enabled = false;

            GetStateList();
            GetStatesForSearch();
            SetStateSearchRequirement(false);

            if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"].ToString() == "AddSitePharmacy")
            {
                lblInfo.Text = "To add Pharmacy for site, search for and select a pharmacy. Then click 'Add to Site Pharmacy / Add to Practice Favorite'.";
                rblSearchBy.Items[0].Enabled = false;
                rblSearchBy.Items[1].Enabled = false;
                rblSearchBy.Items[2].Enabled = true;
                rblSearchBy.SelectedIndex = 2;
                rblMailOrder.Checked = false;
                rblMailOrder.Enabled = false;
                btnSetPatientPharm.Visible = false;
                btnNewRx.Visible = false;
                btnSendScript.Visible = false;
                btnContinue.Visible = false;
                btnPrint.Visible = false;
                btnAddAsSitePharmacy.Visible = true;
                btnAddPracticeFavorite.Visible = true;
                btnSubmitRefillRequest.Visible = false;
                btnCancel.Visible = true;
                btnCancelRx.Visible = false;
                }
            else
            {
                Session["PracticeFavoritesExist"] = Allscripts.Impact.ApplicationLicense.SiteHasPharmacyFavorites(Session["LicenseID"].ToString(), int.Parse(Session["SiteID"] != null ? Session["SiteID"].ToString() : "1"), base.DBID);
                Session["PatientPharmacyHistoryExists"] = Allscripts.Impact.Patient.HasPharmacyHistory(Session["PatientID"].ToString(), Session["LicenseID"].ToString(), Session["UserID"].ToString(), base.DBID);

                bool isTestPatient = false;

                //check for test patient
                if (Session["PATIENTSTATUS"] != null && Session["PATIENTSTATUS"].ToString() == "99")
                {
                    txtNameSearch.Text = "eRxNOW";
                    ddlStateSearch.SelectedValue = "IL";
                    txtZipSearch.Text = "60048";
                    lblInfo.Text = "For this test patient, you must choose the eRxNOW Test Pharmacy.";
                    btnGo.Enabled = false;
                    grdViewPharmacy.DataSourceID = "PharmacyObjDataSource";
                    isTestPatient = true;
                }
                else
                {
                    grdViewPharmacy.DataSourceID = "PatientPharmacyObjDataSource";
                }

                if (!(Request["from"] != null && Request["from"].ToLower().Contains("taskscriptlist")))
                {
                    if (Request.Path.Contains(Constants.PageNames.REFILL_HISTORY))
                    {
                        btnPrint.Visible = false;
                        btnSendScript.Visible = false;
                    }
                    else
                    {
                        btnSubmitRefillRequest.Visible = false;
                    }
                }

                if (Request["from"] != null && Request["from"].ToLower().Contains("taskscriptlist"))
                {
                    btnCancel.Visible = true;
                    btnCancelRx.Visible = false;
                    btnContinue.Visible = false;
                    btnGo.Visible = true;
                    btnNewRx.Visible = false;
                    btnPrint.Visible = false;
                    btnSendScript.Visible = false;
                    btnSubmitRefillRequest.Visible = false;
                    btnSetPatientPharm.Visible = true;
                   // btnSubmit.Visible = false;

                    if (!isTestPatient)
                    {
                        if (Session["LASTPHARMACYID"] == null)
                        {
                            lblInfo.Text = "There is no pharmacy selected for this patient.  Please choose one or click Cancel to continue without one.";
                        }
                        else
                        {
                            lblInfo.Text = "<b>Current view: </b>Patient pharmacy history";
                        }
                    }
                }
                else if (Request["from"] != null && Request["from"].Contains(Constants.PageNames.SCRIPT_PAD.ToLower()))
                {
                    if (!isTestPatient)
                    {
                        if (Session["LASTPHARMACYID"] == null)
                        {
                            lblInfo.Text = "There is no pharmacy selected for this patient.  Please choose one or click Cancel to continue without one.";
                        }
                        else
                        {
                            lblInfo.Text = "<b>Current view: </b>Patient pharmacy history";
                            //lblInfo.Text = "You may update the patient's preferred pharmacy.  Please choose one or click Cancel to continue without one.";
                        }
                    }
                    
                    btnCancel.Visible = true;
                    btnCancelRx.Visible = false;
                    btnContinue.Visible = true;
                    btnGo.Visible = true;
                    btnNewRx.Visible = false;
                    btnPrint.Visible = false;
                    btnSendScript.Visible = false;
                    btnSubmitRefillRequest.Visible = false;
                    
                    if ((Request.QueryString["SetRetail"] != null) && (Request.QueryString["SetRetail"] == "true"))
                    {
                        rblMailOrder.Enabled = false;
                    }
                }
                else
                {
                    rblMailOrder.Enabled = false;
                    lblInfo.Text = "<b>Current view: </b>Patient pharmacy history";
                }

                if ((Request.QueryString["SetMO"] != null) && (Request.QueryString["SetMO"] == "true"))
                {
                    rblMailOrder.Checked = true;
                    rblMailOrder.Enabled = true;
                    rblSearchBy.Enabled = false;
                    SetAsMailOrder();
                }
            }
        }
        else
        {
            if (Request["from"] != null && Request["from"].ToLower().Contains("taskscriptlist"))
            {
                btnCancel.Visible = true;
                btnCancelRx.Visible = false;
                btnContinue.Visible = false;
                btnGo.Visible = true;
                btnNewRx.Visible = false;
                btnPrint.Visible = false;
                btnSendScript.Visible = false;
                btnSubmitRefillRequest.Visible = false;
                btnSetPatientPharm.Visible = true;
                ///btnSubmit.Visible = false;
            }

            //pharmInfoObjDataSource.Select();
        }
        //Disable Practice Favorite radio button item if Practice Favorite doesn't exists.
        rblSearchBy.Items[1].Enabled = (Session["PracticeFavoritesExist"] != null && Convert.ToInt16(Session["PracticeFavoritesExist"]) == 0 ) ? false : true;

        if ((bool)Session["SHOW_SEND_TO_ADM"] == false)
        {
            cboSendToADM.Visible = false;
        }
        else
        {
            if (btnPrint.Visible == true || btnSendScript.Visible == true )
            {
                cboSendToADM.Visible = true;
            }
            else
            {
                cboSendToADM.Visible = false;
            }
        }

        if (!Convert.ToBoolean(Session["IsProvider"]) && !Convert.ToBoolean(Session["IsPA"]) && !Convert.ToBoolean(Session["IsPASupervised"]))
        {
            btnPrint.Visible = false;
            btnCancelRx.Visible = false;
            btnNewRx.Visible = false;
            btnCancel.Visible = true;

            //panelMessage.Visible = false;
            //panelMessageHeader.Visible = false;
        }
        else
        {
            if (Session["SPI"] == null)
            {
                string spimsg = string.Empty;
                if (SessionLicense != null)
                {
                    if (base.SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.CallSupport) == Constants.DeluxeFeatureStatus.On)
                    {
                        spimsg = "<b>Your account is currently not enabled for <br>e-prescribing.  You can only print Rxs at this time.</b><br> Please call " + base.SessionLicense.EnterpriseClient.HelpPhoneNumber + " for Veradigm Technical Support.<br><hr>";
                    }
                    else
                    {
                        spimsg = "<b>Your account is currently not enabled for <br>e-prescribing.  You can only print Rxs at this time.</b><br> Please contact <a href=\"mailto:eprescribesupport@allscripts.com\">ePrescribe Support</a> for Veradigm Technical Support.<br><hr>";
                    }
                }

                //panelMessage.Controls.Add(new LiteralControl(spimsg));
            }
            else
            {
                //panelMessage.Visible = false;
                //panelMessageHeader.Visible = false;
            }
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        //if (Session["ISPROVIDER"] != null && !Convert.ToBoolean( Session["ISPROVIDER"]))
        {
            LinkButton lnk = Master.FindControl("lnkPatient") as LinkButton;
            if (lnk != null)
            {
                lnk.Enabled = false;
            }
            
            lnk = Master.FindControl("lnkMedication") as LinkButton;
            if (lnk != null)
            {
                lnk.Enabled = false;
            }
            
            lnk = Master.FindControl("lnkDiagnosis") as LinkButton;
            if (lnk != null)
            {
                lnk.Enabled = false;
            }
            
            ((PhysicianMasterPage)Master).hideTabs();
        }

        if (!Page.IsPostBack)
        {
            this.pharmacyGridTableRow.Attributes["style"] = "height:" + ((PhysicianMasterPage)Master).getTableHeight() + "px;";
        }
    }

    protected void rblSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        rblMailOrder.Checked = false;
        ShowSearchFields(true);
        grdViewPharmacy.Columns[1].Visible = false;
        grdViewPharmacy.Columns[7].Visible = true;

        grdViewPharmacy.DataKeyNames = new string[] { "PharmacyID", "NABP", "TransMethod", "EpcsEnabled" };
        if (rblSearchBy.SelectedValue == "PatHis")
        {
            ClearSearchCriteria();
            SetStateSearchRequirement(false);
            SetSearchText(rblSearchBy.SelectedValue);
            grdViewPharmacy.DataSourceID = "PatientPharmacyObjDataSource";
        }
        else if (rblSearchBy.SelectedValue == "PracFav")
        {
            ddlStateSearch.ClearSelection();
            SetStateSearchRequirement(false);
            SetSearchText(rblSearchBy.SelectedValue);
            grdViewPharmacy.DataKeyNames = new string[] { "PharmacyID", "NABP", "PracticeFavorite", "TransMethod", "EpcsEnabled" };
            grdViewPharmacy.DataSourceID = "PharmacyFavoriteObjDataSource";
            grdViewPharmacy.DataBind();
        }
        else if (rblSearchBy.SelectedValue == "All")
        {
            SetStateSearchRequirement(true);
            lblInfo.Text = "<b>Current view:</b> None";
            if (Session["PRACTICESTATE"] != null)// && ddlState.Items.FindByValue(Session["PRACTICESTATE"].ToString()) != null)
            {
                if (ddlStateSearch.SelectedValue == "--")
                {
                    ddlStateSearch.ClearSelection();
                    ddlStateSearch.Items.FindByText(Session["PRACTICESTATE"].ToString()).Selected = true; ;
                }
            }

            if(PageState.GetBooleanOrFalse("CameFromPharmFav"))
            {
                grdViewPharmacy.DataSourceID = "PharmacyObjDataSource";
                grdViewPharmacy.DataBind();
                PageState.Remove("CameFromPharmFav");
                return;
            }

            if (grdViewPharmacy.DataSourceID != string.Empty)
            {
                grdViewPharmacy.Sort(string.Empty, SortDirection.Ascending);
            }
            grdViewPharmacy.DataSourceID = String.Empty;
        }

        if (btnSetPatientPharm.Visible)
            btnSetPatientPharm.Text = "Set as Patient Pharmacy";
    }

    protected void rblMailOrder_CheckedChanged(object sender, EventArgs e)
    {
        if (rblMailOrder.Checked)
        {
            SetAsMailOrder();
        }
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
           // detailsViewPharmInfo.Visible = false;

            if (rblSearchBy.SelectedValue == "All")
            {
                grdViewPharmacy.DataSourceID = "PharmacyObjDataSource";
                grdViewPharmacy.DataBind();
            }

            if (rblSearchBy.SelectedValue == "PracFav")
            {
                    grdViewPharmacy.DataSourceID = "PharmacyFavoriteObjDataSource";
                    grdViewPharmacy.DataBind();

                    if (grdViewPharmacy.Rows.Count == 0)
                    {
                        PageState["CameFromPharmFav"] = true;
                        grdViewPharmacy_Load(sender, e);
                    }
            }

                if (rblMailOrder.Checked)
            {
                SetSearchText("MOB");
            }
               
            else
                SetSearchText(rblSearchBy.SelectedValue);               
            }
      
        else
        {
            if (rblSearchBy.SelectedValue == "All" && ddlStateSearch.SelectedIndex==0 && txtZipSearch.Text==String.Empty)
            {
                SetStateSearchRequirement(true);
            }
            else
            {
                SetStateSearchRequirement(false);
            }
        }
    }

    protected void cvStateZipSearch_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (ddlStateSearch.Items.FindByText("--").Selected && txtZipSearch.Text.Trim() == "")
        {
            args.IsValid = false;
        }
        else
        {
            args.IsValid = true;
        }
    }
    
    //protected void pharmInfoObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    //{
    //        DataSet ds = (DataSet)e.ReturnValue;
    //        if (ds != null && ds.Tables.Count > 0)
    //        {
    //            DataTable dt = ds.Tables[0];
    //            if (dt.Rows.Count > 0)
    //            {
    //                string pharmacyID = dt.Rows[0].ItemArray[1].ToString();
    //                dt.Columns.Add("EPCS", typeof(string));

    //                if (dt.Rows[0]["EpcsEnabled"].ToString() == "1")
    //                {
    //                    dt.Rows[0]["EPCS"] = "YES";
    //                }
    //                else
    //                {
    //                    dt.Rows[0]["EPCS"] = "NO";
    //                }

    //                detailsViewPharmInfo.DataSource = dt;
    //                detailsViewPharmInfo.DataBind();
    //                detailsViewPharmInfo.Visible = true;

    //                Label phoneCtrl = (Label)detailsViewPharmInfo.FindControl("lblPhone");
    //                string phone = "";

    //                try
    //                {
    //                    phone = dt.Rows[0]["PhoneAreaCode"].ToString() + dt.Rows[0]["PhoneNumber"].ToString();
    //                    phoneCtrl.Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(phone));
    //                }
    //                catch
    //                {
    //                    phoneCtrl.Text = phone;
    //                }

    //                Label faxCtrl = (Label)detailsViewPharmInfo.FindControl("lblFax");
    //                string fax = "";

    //                try
    //                {
    //                    fax = dt.Rows[0]["FaxAreaCode"].ToString() + dt.Rows[0]["FaxNumber"].ToString();
    //                    faxCtrl.Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(fax));
    //                }
    //                catch
    //                {
    //                    faxCtrl.Text = fax;
    //                }

    //                Label totalRxCtrl = (Label)detailsViewPharmInfo.FindControl("lblTotalRx");
    //                Label totalErrorsCtrl = (Label)detailsViewPharmInfo.FindControl("lblTotalErrors");
    //                Label deliveryRateCtrl = (Label)detailsViewPharmInfo.FindControl("lblDeliveryRate");
    //                Image ratingImgCtrl = (Image)detailsViewPharmInfo.FindControl("imgPharmacyRating");
    //                Label ratingTextCtrl = (Label)detailsViewPharmInfo.FindControl("lblPharmacyRatingText");

    //                StringBuilder deliveryRate = new StringBuilder();
    //                StringBuilder totalRx = new StringBuilder();
    //                StringBuilder totalErrors = new StringBuilder();

    //                try
    //                {
    //                    if (dt.Rows[0]["PercScriptSuccess"] != DBNull.Value && dt.Rows[0]["PercScriptSuccess"].ToString().Trim() != "")
    //                    {
    //                        decimal successRate = decimal.Parse(dt.Rows[0]["PercScriptSuccess"].ToString());

    //                        if (successRate >= decimal.Parse("99.50"))
    //                        {
    //                            ratingImgCtrl.ImageUrl = "images/green_star.gif";
    //                            ratingTextCtrl.Text = "> 99.5% delivery success rate*";
    //                        }
    //                        else if (successRate >= decimal.Parse("98.00") && successRate < decimal.Parse("99.50"))
    //                        {
    //                            ratingImgCtrl.ImageUrl = "images/yellow_star.gif";
    //                            ratingTextCtrl.Text = "98.0% - 98.5% delivery success rate*";
    //                        }
    //                        else if (successRate < decimal.Parse("98.00"))
    //                        {
    //                            ratingImgCtrl.ImageUrl = "images/red_star.gif";
    //                            ratingTextCtrl.Text = "< 98.0% delivery success rate*";
    //                        }
    //                    }
    //                    else
    //                    {
    //                        ratingImgCtrl.ImageUrl = "images/grey_star_question.gif";
    //                        ratingTextCtrl.Text = "Insufficient data*";
    //                    }
    //                }
    //                catch
    //                {
    //                    ratingImgCtrl.ImageUrl = "images/grey_star_question.gif";
    //                    ratingTextCtrl.Text = "Insufficient data*";
    //                }

    //                try
    //                {
    //                    if (dt.Rows[0]["SentScriptCnt"] != DBNull.Value && dt.Rows[0]["SentScriptCnt"].ToString().Trim() != "")
    //                    {
    //                        totalRx.Append(dt.Rows[0]["SentScriptCnt"].ToString());

    //                        if (totalRx.ToString() == "1")
    //                        {
    //                            totalRx.Append(" total Rx");
    //                        }
    //                        else
    //                        {
    //                            totalRx.Append(" total Rx's");
    //                        }

    //                        if (dt.Rows[0]["ErrorScriptCnt"] != DBNull.Value && dt.Rows[0]["ErrorScriptCnt"].ToString().Trim() != "")
    //                        {
    //                            totalErrors.Append("(");
    //                            totalErrors.Append(dt.Rows[0]["ErrorScriptCnt"].ToString());

    //                            if (dt.Rows[0]["ErrorScriptCnt"].ToString() == "1")
    //                            {
    //                                totalErrors.Append(" error)");
    //                            }
    //                            else
    //                            {
    //                                totalErrors.Append(" errors)");
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        totalRx.Append("N/A");
    //                    }

    //                    if (dt.Rows[0]["PercScriptSuccess"] != DBNull.Value && dt.Rows[0]["PercScriptSuccess"].ToString().Trim() != "")
    //                    {
    //                        deliveryRate.Append(decimal.Round(decimal.Parse(dt.Rows[0]["PercScriptSuccess"].ToString()), 0).ToString());
    //                        deliveryRate.Append("%");
    //                    }
    //                    else
    //                    {
    //                        deliveryRate.Append("N/A");
    //                    }
    //                }
    //                catch
    //                {
    //                    totalRx.Remove(0, totalRx.Length);
    //                    totalErrors.Remove(0, totalRx.Length);
    //                    deliveryRate.Remove(0, totalRx.Length);

    //                    totalRx.Append("N/A"); ;
    //                    deliveryRate.Append("N/A");
    //                }

    //                totalRxCtrl.Text = totalRx.ToString();
    //                totalErrorsCtrl.Text = totalErrors.ToString();
    //                deliveryRateCtrl.Text = deliveryRate.ToString();

    //                Label transMethodCtrl = (Label)detailsViewPharmInfo.FindControl("lblTransMethod");

    //                if (dt.Rows[0]["TransMethod"] != DBNull.Value && dt.Rows[0]["TransMethod"].ToString().Trim() != "")
    //                {
    //                    if (dt.Rows[0]["TransMethod"].ToString().ToUpper() == "FAX")
    //                    {
    //                        transMethodCtrl.Text = "Fax";
    //                    }
    //                    else
    //                    {
    //                        transMethodCtrl.Text = "Electronic";
    //                    }
    //                }
    //            }
    //        }
    //    }

    protected void grdViewPharmacy_Load(object sender, EventArgs e)
    {        
        if (grdViewPharmacy.Rows.Count == 0 && grdViewPharmacy.DataSourceID == "PatientPharmacyObjDataSource")
        {
            rblSearchBy.ClearSelection();

            if ((Session["PatientPharmacyHistoryExists"] != null) &&
                 (!bool.Parse(Session["PatientPharmacyHistoryExists"].ToString())))
            {
                rblSearchBy.Items[0].Enabled = false;
            }

            rblSearchBy.SelectedIndex = 1;
            rblSearchBy_SelectedIndexChanged(sender, e);
            grdViewPharmacy_Load(sender, e);
        }
        else if (grdViewPharmacy.Rows.Count == 0 && grdViewPharmacy.DataSourceID == "PharmacyFavoriteObjDataSource")
        {
            rblSearchBy.ClearSelection();

            if ((Session["PracticeFavoritesExist"] != null) &&
                 (!bool.Parse(Session["PracticeFavoritesExist"].ToString())))
            {
                rblSearchBy.Items[1].Enabled = false;
            }

            rblSearchBy.SelectedIndex = 2;
            rblSearchBy_SelectedIndexChanged(sender, e);
            grdViewPharmacy_Load(sender, e);
        }       
    }
    
    protected void grdViewPharmacy_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
			string transMethod = grdViewPharmacy.DataKeys[e.Row.RowIndex]["TransMethod"].ToString();

            e.Row.Attributes.Add("id", e.Row.RowIndex.ToString());
            e.Row.Style["cursor"] = "pointer";
            e.Row.Attributes.Add("onclick", "onRowClick(this,'" + transMethod + "')");

            //format the phone number
            string phone = ((DataRowView)(e.Row.DataItem)).Row["PhoneAreaCode"].ToString() +
                ((DataRowView)(e.Row.DataItem)).Row["PhoneNumber"].ToString();

            if (phone.Trim().Length == 10)
            {
                try
                {
                    e.Row.Cells[7].Text = String.Format("{0:(###) ###-####}", Convert.ToInt64(phone));
                }
                catch
                {
                    e.Row.Cells[7].Text = phone;
                }
            }

            RadioButton rbSelectedRow = (RadioButton)e.Row.FindControl("rbSelectedRow");
            if (rbSelectedRow != null)
            {
                rbSelectedRow.Attributes["Value"] = ((DataRowView)e.Row.DataItem)["PharmacyID"].ToString();
            }

            Label lblAddress = (Label)e.Row.FindControl("lblAddress");
            if (lblAddress != null)
            {
                lblAddress.Text = ((DataRowView)e.Row.DataItem)["Address1"].ToString() + " " + ((DataRowView)e.Row.DataItem)["Address2"].ToString();

                if ((grdViewPharmacy.DataSourceID.ToUpper() == "PHARMACYFAVORITEOBJDATASOURCE") &&
                    (grdViewPharmacy.DataKeys[e.Row.RowIndex]["PracticeFavorite"].ToString().ToUpper() == "N"))
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Font.Bold = true;
                    }

                    lblAddress.Font.Bold = true;
                }
            }

            // may be removed in the future...
            #region CS Image column
            Image imgEpcs = (Image)e.Row.FindControl("imgEpcs");

            if (imgEpcs != null)
            {
                string pharmacyID = ((DataRowView)e.Row.DataItem)["PharmacyID"].ToString();

                if (((DataRowView)e.Row.DataItem)["EpcsEnabled"].ToString() == "1")
                {
                    imgEpcs.ImageUrl = "images/ControlSubstance_sm.gif";
                    imgEpcs.Visible = true;
                }
                else
                {
                    imgEpcs.Visible = false;
                }
            } 
            #endregion
        }
       
    }

    protected void grdViewPharmacy_RowCreated(object sender, GridViewRowEventArgs e)
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
                            {
                                image.ImageUrl = "images/sort-ascending.gif";
                            }
                            else
                            {
                                image.ImageUrl = "images/sort-Descending.gif";
                            }
                        }

                        cell.Controls.Add(image);
                    }
                }
            }
        }
    }

    protected void grdViewPharmacy_SelectedIndexChanged(object sender, EventArgs e)
    {
        //
        // FYI, this event will not be triggered becuase the gridview is not using a select column
        //
        btnSendScript.Enabled = true;

        if ((bool)Session["SHOW_SEND_TO_ADM"] == true)
        {
            cboSendToADM.Visible = true;
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        //UpdatePanel1.Update();
    }

    protected void btnNewRx_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");
        NewRx();
    }

    protected void btnCancelRx_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");
        //NEED BETTER DEFINITION OF WHEN WE SHOULD BE DELETING HERE TODO

        //DON'T DELETE RX IF CAME FROM APPROVEREFILLTASK PAGE. BY DHIRAJ ON 02 FEB 07
        if (Session["RXID"] != null && Request.QueryString["From"] != null && Request.QueryString["From"].ToUpper() != Constants.PageNames.APPROVE_REFILL_TASK.ToUpper())
        {
            string rxid = Session["RXID"].ToString();
            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK);
        }

        if (Request.QueryString["From"] != null && Request.QueryString["From"].ToUpper() == Constants.PageNames.APPROVE_REFILL_TASK.ToUpper())
        {
            Response.Redirect(Constants.PageNames.APPROVE_REFILL_TASK);
        }
        else if (Request.QueryString["From"] != null && Request.QueryString["From"].ToUpper() == Constants.PageNames.REVIEW_HISTORY.ToUpper())
        {
            string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
            string componentParameters = string.Empty;
                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + componentName + "&componentParameters=" + componentParameters);
            }
        else if (Request.QueryString["From"] != null && Request.QueryString["From"].ToUpper().Contains(Constants.PageNames.ADD_PATIENT.ToUpper()))
        {
            Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["From"].ToString()));
        }
        else
        {
            NewRx();
        }
    }

    protected void SubmitRefillRequest_Click(object sender, EventArgs e)
    {
        System.Collections.Generic.List<String> RxList = new System.Collections.Generic.List<string>();
        if (Session["RXIDLIST"] != null)
        {
            RxList = Session["RXIDLIST"] as System.Collections.Generic.List<String>;
            Session.Remove("RXIDLIST");
        }

        if (RxList.Count == 0)
        {
            return;
        }
        else
        {
            string sourceRxID = RxList[0].ToString();

            if (!string.IsNullOrEmpty(sourceRxID))
            {
                string physicianId = (Session["PHYSICIANID"] != null ? Session["PHYSICIANID"].ToString() : "");
                string PatientComments = (Session["PATIENTCOMMENTS"] != null ? Session["PATIENTCOMMENTS"].ToString() : "");

                if (base.SessionPatientID != null)
                {
                    patientID = base.SessionPatientID;
                }
                else
                {
                    patientID = System.Guid.Empty.ToString();
                }

                foreach (GridViewRow row in grdViewPharmacy.Rows)
                {
                    RadioButton rbSelectedRow = (RadioButton)row.FindControl("rbSelectedRow");
                    if (rbSelectedRow != null)
                    {
                        if (rbSelectedRow.Checked)
                        {
                            grdViewPharmacy.SelectedIndex = row.RowIndex;
                            break;
                        }
                    }
                }

                if (grdViewPharmacy.SelectedIndex > -1)
                {
                    pharmacyID = grdViewPharmacy.SelectedDataKey.Value.ToString();
                }
                else
                {
                    pharmacyID = System.Guid.Empty.ToString();
                }

                if (base.SessionLicenseID != null)
                {
                    licenseID = base.SessionLicenseID;
                }

                Prescription sourcePrescription = null;
                Prescription targetPrescription = null;
                Int64 taskID = -1;
                string message = string.Empty;

                try
                {
                    sourcePrescription = new Prescription();
                    sourcePrescription.LoadFromExistingMed(sourceRxID, DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, base.DBID);


                    targetPrescription = sourcePrescription.RenewPrescriptionMed(physicianId, Session["PERFORM_FORMULARY"].ToString(), DurInfo.RetrievePrescriptionDurHandler(PageState), SessionLicenseID, SessionUserID, Convert.ToInt32(Session["SITEID"]), base.DBID);

                    //Now the prescription is created successfully, need to pend the prescription
                    if (targetPrescription != null)
                    {
                        //Update the pharmacy of the target prescription
                        Prescription.UpdatePharmacyID(targetPrescription.ID, pharmacyID, base.DBID);

                        //Pend the prescriptoin for approval
                        //Prescription.PendPrescription(targetPrescription.ID, 1, licenseID, Session["USERID"].ToString());

                        taskID = Prescription.SendRefillRequestToPhysician(targetPrescription.rxID, licenseID, base.SessionUserID, physicianId, PatientComments, base.DBID);

                        if (taskID > 0)
                        {
                            message = "Renewed prescription is pending for provider approval.";
                        }
                        else
                        {
                            message = "Failed to renew the prescription.  Task could not be created.";
                        }
                    }
                    else
                    {
                        message = "Failed to renew the prescription.";
                    }
                }
                catch (Exception)
                {
                    //If any exception happend, then we need to undo the changes. 
                    if (targetPrescription != null)
                    {
                        targetPrescription.Delete();
                    }

                    //TODO
                    //Need to implement logic to revert the prescription back to the original status instead of leaving it at complete status. 

                    message = "Failed to renew the prescription.";
                }

                Response.Redirect(Constants.PageNames.SELECT_PATIENT + "?Msg=" + Server.UrlEncode(message));
            }
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");
        rxID = Session["RXID"].ToString();
        Hashtable htTaskRxID = new Hashtable();

        if (cboSendToADM.Checked == true)
        {
            ScriptMessage.SendNotificationTask(rxID, Session["UserID"].ToString(), Session["LicenseID"].ToString(), Session["PATIENTID"].ToString(), null, base.DBID);
        }

        if (Session["TASKID"] != null)
        {
            Session["REMOVETASKATPRINT"] = "Y";
            htTaskRxID.Add(Int64.Parse(Session["TASKID"].ToString()), rxID);
        }
        else
        {
            htTaskRxID.Add(-1, rxID);
        }

        Session["HTTaskRxID"] = htTaskRxID;

        if (Session["TASKID"] != null && Session["TASKTYPE"] != null)
        {
            Int64 taskID = Convert.ToInt64(Session["TASKID"]);
            Constants.PrescriptionTaskType taskType = (Constants.PrescriptionTaskType)Convert.ToInt32(Session["TASKTYPE"]);
            
            if (taskType == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
            {
                updateProviderOfRecord(rxID, false);
            }
            else if (taskType == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
            {
                updateProviderOfRecord(rxID, false);

                //Mark the script as approved (NEW)
                Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);
                
                //Update the filll task data
                if (Session["RefilltaskData"] != null)
                {
                    RxTaskModel refilltask = (RxTaskModel)Session["RefilltaskData"];
                    Constants.PrescriptionTaskStatus taskstatus = (refilltask.RxRequestType == RequestType.APPROVE ? Constants.PrescriptionTaskStatus.ONE : Constants.PrescriptionTaskStatus.PROCESSED);

                    Prescription.UpdateRxTask(refilltask.RxTaskId, refilltask.PhysicianComments, refilltask.IsPatientVisitRq, (int)taskstatus, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                    Session.Remove("RefilltaskData");
                }
            }
            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_PRINTED, base.SessionPatientID, rxID);
        }

        Prescription.UpdateRxDetailDates(rxID, base.DBID); //3371:Update rxdetail startdate and expiration date

        //let's remove this now since the print logic doesn't need them anymore and it was causing other workflow issues
        Session.Remove("TASKID");
        Session.Remove("TASKTYPE");

        //Server.Transfer("MultipleView.aspx");  
        if (Request.QueryString != null && Request.QueryString.ToString() != string.Empty)
        {
            NameValueCollection qscoll = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            StringBuilder sb = new StringBuilder();
            foreach (var v in qscoll.AllKeys)
            {
                sb.Append(v + "=" + Microsoft.Security.Application.Encoder.UrlEncode(qscoll[v]) + "&");
            }
            
            sb = sb.ToString().EndsWith("&") ? sb.Remove((sb.Length - 1), 1) : sb;
            Response.Redirect(Constants.PageNames.CSS_DETECT + "?" + sb.ToString());
        }
        else
        {
            Response.Redirect(Constants.PageNames.CSS_DETECT);
        }

    }

        
    protected void btnSendScrip_Click(object sender, EventArgs e)
    {
        sendScript();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");
        Session.Remove("RxList");

        if (Request["from"] != null)
        {
            if (Request["from"].ToLower().Contains(Constants.PageNames.SCRIPT_PAD.ToLower()))
            {
                Response.Redirect(Constants.PageNames.SCRIPT_PAD + "?from=" + Constants.PageNames.PHARMACY);
            }
            if (Request["from"].ToLower().Contains(Constants.PageNames.PHARMACY_FAVORITES.ToLower()))
            {
                Response.Redirect(Constants.PageNames.PHARMACY_FAVORITES + "?siteid=" + Microsoft.Security.Application.Encoder.UrlEncode(Request["siteid"].ToString()) + "");
            }
            else
            {
                Response.Redirect(Constants.PageNames.UrlForRedirection(Request["from"].ToString()));
            }
        }
        else
        {
            Response.Redirect(Constants.PageNames.REFILL_HISTORY);
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");

        //// use StringBuilder instead
        //string msgbody = "";

        //msgbody = Session["UserName"].ToString();
        //msgbody += " from " + Session["SiteName"].ToString() + " (" + Session["AHSAccountID"].ToString() + ") has made a request to add:\n\n";
        //msgbody += "Pharmacy Name: " + txtPharmacy.Text + "\n";
        //msgbody += "Address1: " + txtAddress1.Text + "\n";

        //if (txtAddress2.Text.Length > 0)
        //{
        //    msgbody += "Address2: " + txtAddress2.Text + "\n";
        //}

        //msgbody += "City: " + txtCity.Text + "\n";
        //msgbody += "State: " + ddlState.SelectedValue + "\n";
        //msgbody += "ZIP: " + txtZIP.Text + "\n";
        //msgbody += "Phone: " + txtPhone.Text + "\n";
        
        //if (txtFax.Text.Length > 0)
        //{
        //    msgbody += "Fax: " + txtFax.Text + "\n";
        //}

        //if (txtComment.Text.Length > 0)
        //{
        //    msgbody += "Comment:" + txtComment.Text + "\n";
        //}

        //SystemConfig.AddUserRequest(Session["LicenseID"].ToString(), Session["UserID"].ToString(), Constants.RequestType.NEW_PHARMACY, msgbody, base.DBID);

        //lblPharmInfo.Text = "The following message has been sent:<br>\n" + msgbody.Replace("\n", "<BR>");

        //txtAddress1.Text = "";
        //txtAddress2.Text = "";
        //txtCity.Text = "";
        //txtComment.Text = "";
        //txtFax.Text = "";
        //txtPharmacy.Text = "";
        //txtPhone.Text = "";
        //txtZIP.Text = "";

        GetStateList();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");
        //update the pharmacy and go back to ScriptPad
        foreach (GridViewRow row in grdViewPharmacy.Rows)
        {
            RadioButton rbSelectedRow = (RadioButton)row.FindControl("rbSelectedRow");
            if (rbSelectedRow != null)
            {
                if (rbSelectedRow.Checked)
                {
                    grdViewPharmacy.SelectedIndex = row.RowIndex;
                    break;
                }
            }
        }

        if (grdViewPharmacy.SelectedIndex > -1)
        {
            pharmacyID = grdViewPharmacy.SelectedDataKey.Value.ToString(); // Get the PhramacyId and Update
            string nabp = grdViewPharmacy.SelectedDataKey.Values["NABP"].ToString();

            DataSet ds = Patient.UpdatePharmacyID(
                Session["PatientID"].ToString(),
                pharmacyID,
                rblMailOrder.Checked,
                base.DBID);

            Label lblAddress = (Label)grdViewPharmacy.SelectedRow.Cells[3].FindControl("lblAddress");
            string pharmacyDetail = grdViewPharmacy.SelectedRow.Cells[3].Text.Trim() + ", " +
                                    lblAddress.Text.Trim() + ", " +
                                    grdViewPharmacy.SelectedRow.Cells[4].Text.Trim() + ", " +
                                    grdViewPharmacy.SelectedRow.Cells[5].Text.Trim();

            if (rblMailOrder.Checked)
            {
                Session.Remove("MOB_IsElectronicEnabled");
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_MAIL_PHARMACY_MODIFY, base.SessionPatientID);
                Session["MOB_NABP"] = nabp;
                Session["PatientHasMOBCoverage"] = "Y";
                Session["MOB_Name"] = pharmacyDetail;
                Session["MOB_PHARMACY_ID"] = pharmacyID; 

                if (grdViewPharmacy.SelectedDataKey.Values["EpcsEnabled"].ToString() == "1")
                {
                    Session["MOB_ISEPCSENABLED"] = true;
                    Session["MOB_State"] = grdViewPharmacy.SelectedDataKey.Values["State"].ToString();

                    // get EPCS authorized schedules for pharmacy
                    List<string> authorizedSchedules = new List<string>();

                    DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(pharmacyID, DBID);

                    foreach (DataRow drSchedule in dtSchedules.Rows)
                    {
                        authorizedSchedules.Add(drSchedule[0].ToString());
                    }

                    Session["MOPharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;
                    Session["MOB_IsElectronicEnabled"] = true;// Update the Session data. Issue 733417 Script Pad - POB user - When a mail order is added on Script Pad the Send to Physician w Mail Order desitination does not display for CS Meds
                }
                else
                {
                    Session["MOB_ISEPCSENABLED"] = false; 
                    Session.Remove("PharmacyEPCSAuthorizedSchdules");
                }
            }
            else
            {
                base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RETAIL_PHARMACY_MODIFY, base.SessionPatientID);
                Session["SentTo"] = grdViewPharmacy.SelectedRow.Cells[2].Text;
                if (Request.QueryString["from"] != Constants.PageNames.SCRIPT_PAD.ToLower())
                {
                    Session["PHARMACYID"] = pharmacyID;
                }
                Session["LASTPHARMACYID"] = pharmacyID;
                Session["LASTPHARMACYNAME"] = pharmacyDetail;

                if (AppCode.PptPlusBPL.PPTPlus.IsPPTPlusPreferenceOn(PageState))
                {
                    Allscripts.ePrescribe.Data.PptPlusData pptdata = new Allscripts.ePrescribe.Data.PptPlusData();
                    Session[Constants.SessionVariables.PptPharmacyInfo] = pptdata.GetPharmacyInfo(PageState.GetGuidOr0x0("LastPharmacyID"), DBID);
                }
                    if (grdViewPharmacy.SelectedDataKey.Values["EpcsEnabled"].ToString() == "1")
                {
                    Session["ISPHARMACYEPCSENABLED"] = true;

                    // get EPCS authorized schedules for pharmacy
                    List<string> authorizedSchedules = new List<string>();

                    DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(Session["LASTPHARMACYID"].ToString(), DBID);

                    foreach (DataRow drSchedule in dtSchedules.Rows)
                    {
                        authorizedSchedules.Add(drSchedule[0].ToString());
                    }

                    Session["PharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;
                }
                else
                {
                    Session["ISPHARMACYEPCSENABLED"] = false;
                    Session.Remove("PharmacyEPCSAuthorizedSchdules");
                }

                if (!doNotAddToFavsClicked && !pharmacyExistsInSiteFavorites(pharmacyID))
                {
                    modalAddToFavoritesPopup.Show();
                    ViewState["ClickIndicator"] = "btnContinue";
                    ViewState["SelectedPharmacyID"] = pharmacyID;
                    return;
                }
            }
           
            Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["from"] + "?from=" + Constants.PageNames.PHARMACY));
        }
    }

    protected void PharmacyObjDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.ReturnValue != null)
        {
            DataSet ds = (DataSet)e.ReturnValue;

            if (ds != null && ds.Tables[0].Rows.Count > 50)
            {
                lblInfo.Text = "<b>Current view:</b> Your search returned more than 50 results. Please consider refining your search.";
            }

            ds.Dispose();
        }        
    }

    protected void btnSetPatientPharm_Click(object sender, EventArgs e)
    {            
                Session.Remove("PracticeFavoritesExist");

                DataSet ds = Patient.UpdatePharmacyID(
                    Session["PatientID"].ToString(),
                    selectedPharmacy.Value,
                    rblMailOrder.Checked,
                    base.DBID);


            //save patient pharmacy if it wasn't mail order
            if (!rblMailOrder.Checked)
                {
                    base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_RETAIL_PHARMACY_MODIFY, base.SessionPatientID);

                    Session["LASTPHARMACYID"] = selectedPharmacy.Value;
                if (AppCode.PptPlusBPL.PPTPlus.IsPPTPlusPreferenceOn(PageState))
                {
                    Allscripts.ePrescribe.Data.PptPlusData pptdata = new Allscripts.ePrescribe.Data.PptPlusData();
                    Session[Constants.SessionVariables.PptPharmacyInfo] = pptdata.GetPharmacyInfo(PageState.GetGuidOr0x0("LastPharmacyID"), DBID);
                }

                if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows != null)
                        {
                            Session["LASTPHARMACYNAME"] = ds.Tables[0].Rows[0]["LastPharmacyName"].ToString();
                        }
                    }

                    if (!doNotAddToFavsClicked && !pharmacyExistsInSiteFavorites(selectedPharmacy.Value))
                    {
                        modalAddToFavoritesPopup.Show();
                        ViewState["ClickIndicator"] = "btnSetPatientPharm";
                        ViewState["SelectedPharmacyID"] = selectedPharmacy.Value; //Session was not updated before Rx was sent... Destination was recieved from session
                        return;
                    }
                }
                else
                {
                    base.AuditLogPatientInsert(ePrescribeSvc.AuditAction.PATIENT_MAIL_PHARMACY_MODIFY, base.SessionPatientID);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows != null)
                        {
                            Session["MOB_NABP"] = ds.Tables[0].Rows[0]["NABP"].ToString();
                        }

                        Session["MOB_Name"] = ds.Tables[0].Rows[0]["LastPharmacyName"].ToString();
                        Session["MOB_Pharmacy_Id"] = selectedPharmacy.Value;
                        Session["PatientHasMOBCoverage"] = "Y";
                    }
                }

            Redirect();
            
    }

    protected void btnAddToFavorites_Click(object sender, EventArgs e)
    {
        Allscripts.Impact.Pharmacy.InsertPharmacyFavorite(
            base.SessionLicenseID,
            base.SessionSiteID,
            selectedPharmacy.Value,
            base.DBID);

        HandleRedirect(sender, e);
    }

    protected void btnDoNotAddToFavorites_Click(object sender, EventArgs e)
    {
        Session.Remove("PracticeFavoritesExist");
        doNotAddToFavsClicked = true;
        HandleRedirect(sender, e);
    }

    protected void btnAddPracticeFavorite_Click(object sender, EventArgs e)
    {
        int siteID = Convert.ToInt32(Request["siteid"].ToString());

        base.AuditLogLicenseInsert(ePrescribeSvc.AuditAction.LICENSE_SITE_PHARMACY_FAVORITE_EDIT, siteID);

        Allscripts.Impact.Pharmacy.InsertPharmacyFavorite(
            base.SessionLicenseID,
            siteID,
            selectedPharmacy.Value,
            base.DBID);
        Response.Redirect(Constants.PageNames.PHARMACY_FAVORITES + "?siteid=" + Microsoft.Security.Application.Encoder.UrlEncode(Request["siteid"].ToString()) + "");
    }

    protected void btnAddAsSitePharmacy_Click(object sender, EventArgs e)
    {
        Allscripts.Impact.Pharmacy.AddSitePharmacy(
            base.SessionLicenseID,
            Convert.ToInt32(Request["siteid"].ToString()),
            selectedPharmacy.Value,
            base.DBID);
        Response.Redirect(Constants.PageNames.PHARMACY_FAVORITES + "?siteid=" + Microsoft.Security.Application.Encoder.UrlEncode(Request["siteid"].ToString()) + "");
    }

    void ucEPCSDigitalSigning_OnDigitalSigningComplete(DigitalSigningEventArgs dsEventArgs)
    {
        if (dsEventArgs.Success)
        {
            if (ucEPCSDigitalSigning.IsApprovalRequestWorkflow)
            {
                rxID = Session["RXID"].ToString();
                //Copy and paste all the required code and just execute it
                //Check the redirection logic.
                if (Session["ApproveWorkflowRefillTaskId"] != null)
                {
                    long taskID = (long)Session["ApproveWorkflowRefillTaskId"];

                    //If the task type is to approve, then mark the rx to approved status (NEW)
                    Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                    //Update the task status
                    Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, Session["USERID"].ToString(), base.DBID);

                    long serviceTaskID = -1;
                    foreach (KeyValuePair<string, string> kvp in dsEventArgs.SignedMeds)
                    {
                        if (kvp.Key.Equals(rxID))
                        {
                            if (!string.IsNullOrEmpty(kvp.Value) && Session["STANDING"].ToString() == "1")
                            {
                                serviceTaskID = ScriptMessage.SendThisEPCSMessage(kvp.Value, base.SessionLicenseID, base.SessionUserID, base.DBID);
                            }
                        }
                    }

                    base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);

                    string RxStatus = "SENTTOPHARMACY"; //Setting the status of the Prescription April 3 2007..
                    Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), rxID, RxStatus, base.DBID);

                    setSuccessMessage(false);

                    Session["ApproveWorkflowRefillTaskId"] = null;

                    if (Request.QueryString["To"] != null &&
                            (Request.QueryString["To"].ToString().Contains(Constants.PageNames.APPROVE_REFILL_TASK)))
                    {
                        if (Request.QueryString["To"].ToString().Equals(Constants.PageNames.APPROVE_REFILL_TASK))//Checking if the URL contains only the page name, if yes then include '?Msg' otherwise append '&Msg'
                        {
                            Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()) + "?Msg=" + Server.UrlEncode(Session["SuccessMsg"].ToString()), true);
                        }
                        else
                        {
                            Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()) + "&Msg=" + Server.UrlEncode(Session["SuccessMsg"].ToString()), true);
                        }
                    }

                    if (Request.QueryString["To"] != null)
                    {
                        Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString()));
                    }
                    else
                    {
                        Server.Transfer(Constants.PageNames.APPROVE_REFILL_TASK);
                    }

                    //let's remove this now since the print logic doesn't need them anymore and it was causing other workflow issues
                    Session.Remove("TASKID");
                    Session.Remove("TASKTYPE");
                }              
            }
        }
        else
        {
            if (dsEventArgs.ForceLogout)
            {
                //force the user to log out if they've entered invalid credentials 3 times in a row
                Response.Redirect(Constants.PageNames.LOGOUT);
            }
            else if (string.IsNullOrEmpty(dsEventArgs.Message))
            {
                ucMessage.MessageText = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";                
            }
            else
            {
                ucMessage.MessageText = dsEventArgs.Message;
            }

            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            ucMessage.Visible = true;
        }
    }

    void ucCSMedRefillRequestNotAllowed_OnPrintRefillRequest(object sender, EventArgs e)
    {
        //it will come here only incase of approve request workflow.

        long taskID = (long)Session["ApproveWorkflowRefillTaskId"];

        //If the task type is to approve, then mark the rx to approved status (NEW)
        Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

        //Update the task status
        //Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, Session["USERID"].ToString(), base.DBID);

        //Done on printing page.
        //string RxStatus = "SENTTOPHARMACY"; //Setting the status of the Prescription April 3 2007..
        //Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), rxID, RxStatus, base.DBID);

        setSuccessMessage(false);


        Session["ApproveWorkflowRefillTaskId"] = null;
        //Session["CameFrom"] = Constants.PageNames.PHARMACY;

        Session["REFILLMSG"] = "Controlled substance prescription has been printed.";

        base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);

        //let's remove this now since the print logic doesn't need them anymore and it was causing other workflow issues
        Session.Remove("TASKID");
        Session.Remove("TASKTYPE");

        //Server.Transfer(Constants.PageNames.CSS_DETECT + "?To=" + HttpUtility.HtmlEncode(Constants.PageNames.APPROVE_REFILL_TASK));
        Server.Transfer(string.Concat(Constants.PageNames.CSS_DETECT, Request.QueryString != null ? string.Concat("?", Request.QueryString.ToString()) : string.Empty));

    }

    #endregion

    #region Custom Methods

    protected void Redirect()
        {
            if (Request.QueryString["From"] != null)
            {
                if (Request.QueryString["From"].Contains(Constants.PageNames.ADD_PATIENT))
                {
                    Response.Redirect(Constants.PageNames.ADD_PATIENT + "?Mode=Edit&Msg=" + Server.UrlEncode("Patient pharmacy set successfully."));
                }
                else if (Request.QueryString["From"].ToString() == Constants.PageNames.TASK_SCRIPT_LIST.ToLower() ||
                         Request.QueryString["From"].ToString() == Constants.PageNames.REVIEW_HISTORY)
                {
                    Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["From"].ToString()));
                }
                else
                {
                    DefaultRedirect("Msg=" + Server.UrlEncode("Patient pharmacy set successfully."));
                }
            }
            else
            {
                DefaultRedirect("Msg=" + Server.UrlEncode("Patient pharmacy set successfully."));
            }
        }

    private void SetSearchText(string selectedValue)
    {
        StringBuilder searchText = new StringBuilder();
        searchText.Append("<b>Current view:</b> ");
        string searchParams = RenderSearchParamText().ToHTMLEncode();

        switch (selectedValue)
        {
            case "PatHis":
                if (searchParams.Length > 0)
                {
                    searchText.Append("Search results for ");
                    searchText.Append(searchParams);
                    searchText.Append(" (Patient pharmacy history)");
                }
                else
                {
                    searchText.Append("All patient pharmacy history");
                }
                break;
            case "PracFav":
                if (searchParams.Length > 0)
                {
                    searchText.Append("Search results for ");
                    searchText.Append(searchParams);
                    searchText.Append(" (Practice favorites)");
                }
                else
                {
                    searchText.Append("All practice favorites");
                }
                break;
            case "All":
                searchText.Append("Search results for ");
                searchText.Append(searchParams);
                searchText.Append(" (All retail pharmacies)");
                break;
            case "MOB":
                if (searchParams.Length > 0)
                {
                    searchText.Append("Search results for ");
                    searchText.Append(searchParams);
                    searchText.Append(" (Mail Order pharmacies)");
                }
                else
                {
                    searchText.Append("All mail order pharmacies");
                }
                break;
            default:
                break;
        }

        lblInfo.Text = searchText.ToString();
    }

    private string RenderSearchParamText()
    {
        StringBuilder searchText = new StringBuilder();

        if (txtNameSearch.Text.Trim() != "")
        {
            searchText.Append("'");
            searchText.Append(txtNameSearch.Text.Trim());
            searchText.Append("' ");
        }

        if (txtAddressSearch.Text.Trim() != "")
        {
            if (txtNameSearch.Text.Trim() != "")
            {
                searchText.Append("at ");
            }

			searchText.Append("'");
			searchText.Append(txtAddressSearch.Text.Trim());
			searchText.Append("' ");
        }

        if (txtCitySearch.Text.Trim() != "")
        {
            if (txtNameSearch.Text.Trim() != "")
            {
                searchText.Append("in ");
            }

            searchText.Append("'");
            searchText.Append(txtCitySearch.Text.Trim());
            searchText.Append("'");
        }

        if (ddlStateSearch.SelectedValue != "" && ddlStateSearch.SelectedValue != "--")
        {
            if (txtCitySearch.Text.Trim() != "")
            {
                searchText.Append(", ");
            }
            else if (txtNameSearch.Text.Trim() != "" || txtAddressSearch.Text.Trim() != "")
            {
                searchText.Append("in ");
            }

            searchText.Append("'");
            searchText.Append(ddlStateSearch.SelectedValue);
            searchText.Append("' ");
        }

        if (txtZipSearch.Text.Trim() != "")
        {
            searchText.Append("'");
            searchText.Append(txtZipSearch.Text.Trim());
            searchText.Append("' ");
        }

        if (txtPhoneAreaCodeSearch.Text.Trim() != "")
        {
            searchText.Append("at ");

            searchText.Append("'");
            searchText.Append(txtPhoneAreaCodeSearch.Text.Trim());

            if (txtPhoneBodySearch.Text.Trim() != "")
            {
                searchText.Append("-");
                searchText.Append(txtPhoneBodySearch.Text.Trim());
            }

            searchText.Append("'");
        }
        else if (txtPhoneBodySearch.Text.Trim() != "")
        {
            searchText.Append("at ");

            searchText.Append("'");
            searchText.Append(txtPhoneBodySearch.Text.Trim());
            searchText.Append("'");
        }

        return searchText.ToString();
    }

    protected void GetStateList()
    {
        //DataTable dtLisState = RxUser.ChGetState(base.DBID);
        //ddlState.DataSource = dtLisState;
        //ddlState.DataTextField = "State";
        //ddlState.DataValueField = "State";
        //ddlState.DataBind();

        //if (Session["PRACTICESTATE"] != null && ddlState.Items.FindByValue(Session["PRACTICESTATE"].ToString()) != null)
        //{
        //    ddlState.Items.FindByText(Session["PRACTICESTATE"].ToString()).Selected = true;
        //}
    }

    private void GetStatesForSearch()
    {
        DataTable dtLisState = RxUser.ChGetState(base.DBID);
        ddlStateSearch.DataSource = dtLisState;
        ddlStateSearch.DataTextField = "State";
        ddlStateSearch.DataValueField = "State";
        ddlStateSearch.DataBind();
        ddlStateSearch.Items.Insert(0, "--");
    }

    private void SetAsMailOrder()
    {
        rblSearchBy.ClearSelection();
        ClearSearchCriteria();
        SetStateSearchRequirement(false);
        ShowSearchFields(true);

        if (btnSetPatientPharm.Visible)
        {
            btnSetPatientPharm.Text = "Set as Preference & Continue";
        }

        SetSearchText("MOB");

        grdViewPharmacy.Columns[1].Visible = true;
        grdViewPharmacy.Columns[7].Visible = false;
        grdViewPharmacy.DataKeyNames = new string[] { "PharmacyID", "NABP", "TransMethod", "EpcsEnabled", "State" };
        grdViewPharmacy.DataSourceID = "MailOrderPharmacyObjDataSource";
        grdViewPharmacy.DataBind();
    }

    private void ShowSearchFields(bool SearchRetail)
    {
        txtNameSearch.Enabled = SearchRetail;
        txtAddressSearch.Enabled = SearchRetail;
        txtCitySearch.Enabled = SearchRetail;
        txtZipSearch.Enabled = SearchRetail;
        txtPhoneAreaCodeSearch.Enabled = SearchRetail;
        txtPhoneBodySearch.Enabled = SearchRetail;
        ddlStateSearch.Enabled = SearchRetail;
        btnGo.Enabled = SearchRetail;
        lblName.Enabled = SearchRetail;
        lblStreet.Enabled = SearchRetail;
        lblCity.Enabled = SearchRetail;
        lblState.Enabled = SearchRetail;
        lblZipCode.Enabled = SearchRetail;
        lblPhone.Enabled = SearchRetail;
        btnGo.Enabled = SearchRetail;
    }

    private void SetStateSearchRequirement(bool isEnabled)
    {
        if (isEnabled)
        {
            lblStateAsterisk.Text = "*";
        }
        else
        {
            lblStateAsterisk.Text = "";
        }

        cvStateZipSearch.Enabled = isEnabled;
    }

    private void ClearSearchCriteria()
    {
        txtNameSearch.Text = "";
        txtAddressSearch.Text = "";
        txtCitySearch.Text = "";
        txtZipSearch.Text = "";
        txtPhoneAreaCodeSearch.Text = "";
        txtPhoneBodySearch.Text = "";
        ddlStateSearch.ClearSelection();
        ddlStateSearch.Items.FindByText("--").Selected = true;
    }

    protected void UpdatePrescription(string TaskRxID, Int64 TaskID)
    {
        // June 22 to update the RX_HEADER and REMOVE the Task From Service task

        if (Session["LICENSEID"] != null)
        {
            licenseID = Session["LICENSEID"].ToString();

            Allscripts.Impact.Prescription.UpdatePharmacyID(TaskRxID, pharmacyID, rblMailOrder.Checked, base.DBID); // Used to update the Pharmacy Id  

            //Commented by AKS for Deployment as on AUG 21 need to ask erik/dhiraj for this
            try
            {
                string scriptId = ScriptMessage.CreateScriptMessage(TaskRxID, 1, Constants.MessageTypes.NEWRX, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.ShieldSecurityToken,base.SessionSiteID, base.DBID);

                if (Session["STANDING"].ToString() == "1")
                {
                    ScriptMessage.SendThisMessage(scriptId, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                }

                scriptId = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, base.DBID);

                if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(scriptId)))
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(scriptId, base.SessionLicenseID, base.SessionUserID, base.DBID);
                }
            }
            catch (Exception)
            {
            }

            Prescription.UpdateRxTask(TaskID, string.Empty, string.Empty, 1, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
        }
    }

    private void NewRx()
    {
        ClearMedicationInfo(false);

        Session.Remove("RXID");
        Session.Remove("TASKTYPE");
        Session.Remove("TASKID");

        DefaultRedirect();
    }

   
    private void updateProviderOfRecord(string rxID, bool attemptingToSendToPharmacy)
    {
        //logic to determine whether or not to update the providerid and authorizebyid to the current user
        if (Session["UserType"] != null)
        {
            if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PROVIDER ||
                Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT ||
                Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                string authorizeByID = base.SessionUserID;

                if (Convert.ToInt32(Session["UserType"]) == (int)Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
                {
                    authorizeByID = Session["DelegateProviderID"] != null ? Session["DelegateProviderID"].ToString() : base.SessionUserID;
                }

                if (!attemptingToSendToPharmacy)
                {
                    Prescription.UpdateProviderOfRecord(rxID, base.SessionUserID, authorizeByID, base.SessionUserID, base.SessionLicenseID, base.DBID);
                }
                else if (attemptingToSendToPharmacy && Session["SPI"] != null && Session["SPI"].ToString() != "")
                {
                    Prescription.UpdateProviderOfRecord(rxID, base.SessionUserID, authorizeByID, base.SessionUserID, base.SessionLicenseID, base.DBID);
                }
            }
        }
    }

    private void sendScript()
    {
        Session.Remove("PracticeFavoritesExist");
        bool pharmIsErxEnabled = true;
        bool isControledSubstance = false;
        string pharmDesc = string.Empty;

        foreach (GridViewRow row in grdViewPharmacy.Rows)
        {
            RadioButton rbSelectedRow = (RadioButton)row.FindControl("rbSelectedRow");
            if (rbSelectedRow != null)
            {
                if (rbSelectedRow.Checked)
                {
                    grdViewPharmacy.SelectedIndex = row.RowIndex;
                    pharmIsErxEnabled = grdViewPharmacy.DataKeys[row.RowIndex]["TransMethod"].ToString().Equals("SureScripts", StringComparison.OrdinalIgnoreCase);
                    break;
                }
            }
        }

        if (grdViewPharmacy.SelectedIndex > -1)
        {
            pharmacyID = grdViewPharmacy.SelectedDataKey.Value.ToString(); // Get the PhramacyId and Update
            Session["SentTo"] = grdViewPharmacy.SelectedRow.Cells[2].Text;
        }

        //Check if the destination is 10.6 and if the sigtext is longer than 140 chars.
        //If true, do not send the script; instead, show a message that the sigtext is too long.
        if (this.CurrentRx?.SigText.Length > 140)
        {
            DataSet dsPharm = new Allscripts.Impact.Pharmacy().LoadPharmacyById(pharmacyID, DBID);
            if (!Allscripts.Impact.Pharmacy.Is6XPharmacy(dsPharm.Tables[0]?.Rows?[0]))
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "ShowPharmacySigAlert", $"ShowPharmacySigAlert(\"" + "Sig text is currently " + this.CurrentRx.SigText.Length + " characters, the chosen pharmacy has a maximum allowed sigtext length of 140." + "\");", true);
                btnSendScript.Enabled = true;
                return;
            }
        }

        if (!doNotAddToFavsClicked && !rblMailOrder.Checked && !pharmacyExistsInSiteFavorites(pharmacyID))
        {
            modalAddToFavoritesPopup.Show();
            ViewState["ClickIndicator"] = "btnSendScrip";
            ViewState["SelectedPharmacyID"] = pharmacyID;
            setSuccessMessage(false);      
            return;
        }

        //This value is set in TaskScriptList page...  
        if (Session["ISPROVIDER"] != null && !Convert.ToBoolean(Session["ISPROVIDER"]) && Session["HTTaskRxID"] != null)  //We know the values in the HashTable are present. 
        // AFter printing and sending script the session to be cleared.(IMP)
        {
            // Iterate through the Hashtable Extract the values and then updated the table.
            Hashtable htScriptTaskRxID = (System.Collections.Hashtable)Session["HTTaskRxID"];

            IDictionaryEnumerator myEnumerator = htScriptTaskRxID.GetEnumerator();
            while (myEnumerator.MoveNext())
            {
                Int64 TaskID = Convert.ToInt64(myEnumerator.Key);       // Key has the TaskId
                string TaskRxID = myEnumerator.Value.ToString();        // Value is the RxID

                if (cboSendToADM.Checked == true)
                {
                    ScriptMessage.SendNotificationTask(TaskRxID, Session["UserID"].ToString(), Session["LicenseID"].ToString(), Session["PATIENTID"].ToString(), null, base.DBID);
                }

                // Iterate and update the rx_header table and delete the task from the Service Table.
                UpdatePrescription(TaskRxID, TaskID);

                base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);
            }

            Session.Remove("HTTaskRxID");
            Response.Redirect(Constants.PageNames.LIST_SEND_SCRIPTS);
        }
        else
        {
            if (Session["RXID"] != null)
            {
                rxID = Session["RXID"].ToString();

                if (cboSendToADM.Checked == true)
                {
                    ScriptMessage.SendNotificationTask(rxID, Session["UserID"].ToString(), Session["LicenseID"].ToString(), Session["PATIENTID"].ToString(), null, base.DBID);
                }

                updateProviderOfRecord(rxID, true);

                Allscripts.Impact.Prescription.UpdatePharmacyID(rxID, pharmacyID, rblMailOrder.Checked, base.DBID);
                Prescription.UpdateRxDetailDates(rxID, base.DBID); //3371:Update rxdetail startdate and expiration date

                
                
                //Check for nulls and do the if null check.
                Int64 taskID = Convert.ToInt64(Session["TASKID"]);
                Constants.PrescriptionTaskType taskType = (Constants.PrescriptionTaskType)Convert.ToInt32(Session["TASKTYPE"]);
                Rx rx = this.CurrentRx;
                    
                string reconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(rx.ControlledSubstanceCode, rx.StateControlledSubstanceCode, rx.StateCSCodeForPractice);

                isControledSubstance = (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) && reconciledControlledSubstanceCode.ToUpper() != "U" &&
                    reconciledControlledSubstanceCode != "0") || rx.IsFreeFormMedControlSubstance;


                //if ( rx != null && (!string.IsNullOrEmpty(rx.ControlledSubstanceCode) && !rx.ControlledSubstanceCode.Equals("U")))
                //{
                //    isControledSubstance = true;
                //}

                if (isControledSubstance && taskType == (int)Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                {
                    bool isPharmacyEPCSEnabled = false;
                    bool isShowPrintOptionOverlay = false;
                    DataSet dsPharmacy = null;
                    if (!string.IsNullOrEmpty(pharmacyID))
                    {
                        dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyID, DBID);
                        if (dsPharmacy != null && dsPharmacy.Tables != null && dsPharmacy.Tables.Count > 0)
                        {
                            isPharmacyEPCSEnabled = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"]);
                            DataRow pharmacyDR = dsPharmacy.Tables["Pharmacy"].Rows[0];
                            StringBuilder pharmacyDesc = new StringBuilder();

                            pharmacyDesc.Append(pharmacyDR["Name"].ToString().Trim()).Append(", ");

                            pharmacyDesc.Append(pharmacyDR["Address1"].ToString().Trim()).Append(" ");
                            string address2 = pharmacyDR["Address2"].ToString().Trim();
                            if (!string.IsNullOrEmpty(address2))
                            {
                                pharmacyDesc.Append(address2).Append(" ");
                            }
                            pharmacyDesc.Append(pharmacyDR["City"].ToString().Trim()).Append(", ");
                            pharmacyDesc.Append(pharmacyDR["State"].ToString().Trim()).Append(" ");
                            pharmacyDesc.Append(pharmacyDR["Zip"].ToString().Trim());

                            pharmDesc = pharmacyDesc.ToString();
                        }
                    }


                    if (Session["SPI"] != null && base.CanTryEPCS)
                    {
                        if (!isPharmacyEPCSEnabled)
                        {
                            //show overlay dialog here.
                            isShowPrintOptionOverlay = true;
                        }
                        else
                        {
                            //Check for free form med.
                            string sControlledSubstanceCode = string.Empty;
                            if (!string.IsNullOrEmpty(rx.DDI))
                            {
                                DataSet dsCSMed = Allscripts.Impact.Medication.Load(rx.DDI, Guid.Empty.ToString(), base.DBID);

                                if (dsCSMed != null && dsCSMed.Tables != null && dsCSMed.Tables.Count > 0 && dsCSMed.Tables[0].Rows != null && dsCSMed.Tables[0].Rows.Count > 0)
                                {
                                    sControlledSubstanceCode = dsCSMed.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                                }
                            }
                            else
                            {
                                DataSet dsRxDetailCS = Prescription.GetCSCodeFromRxDetailCS(rx.RxID, base.DBID);
                                if (dsRxDetailCS != null && dsRxDetailCS.Tables.Count > 0 && dsRxDetailCS.Tables[0].Rows.Count > 0)
                                {
                                    sControlledSubstanceCode = dsRxDetailCS.Tables[0].Rows[0]["RxScheduleUsed"].ToString();
                                }
                            }
                                


                            string stateCSCodeForSite = Prescription.GetStateControlledSubstanceCode(rx.DDI, Session["PRACTICESTATE"].ToString(), null, base.DBID);
                            string stateCSCodeForPharmacy = Allscripts.Impact.Prescription.GetStateControlledSubstanceCode(base.CurrentRx.DDI, null,
                                                                                dsPharmacy.Tables[0].Rows[0]["State"].ToString(), base.DBID);

                            int scheduleUsed = 0;
                            int.TryParse(Prescription.ReconcileControlledSubstanceCodes(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite), out scheduleUsed);
                            //controlSubstanceCode = scheduleUsed;

                            if (Prescription.IsCSMedEPCSEligible(sControlledSubstanceCode, stateCSCodeForPharmacy, stateCSCodeForSite)) /*|| rx.IsFreeFormMedControlSubstance*/
                            {
                                // get EPCS authorized schedules for pharmacy
                                List<string> authorizedSchedules = new List<string>();
                                DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(pharmacyID, DBID);

                                foreach (DataRow drSchedule in dtSchedules.Rows)
                                {
                                    authorizedSchedules.Add(drSchedule[0].ToString());
                                }

                                Session["PharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;

                                if (Session["PharmacyEPCSAuthorizedSchdules"] != null && ((List<string>)Session["PharmacyEPCSAuthorizedSchdules"]).Contains(scheduleUsed.ToString()))
                                {
                                    if (base.SiteEPCSAuthorizedSchedules.Contains(scheduleUsed.ToString()))
                                    {
                                        List<Rx> rxList = new List<Rx>();

                                        bool MOBPharmacy = false;
                                        MOBPharmacy = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["MOBFlag"]);
                                        if (MOBPharmacy)
                                        {
                                            rx.Destination = Patient.MOB;
                                        }
                                        else
                                        {
                                            rx.Destination = Patient.PHARM;
                                        }

                                        rxList.Add(rx);

                                        //show EPCS Overlay and sign process.
                                        ucEPCSDigitalSigning.IsScriptForNewRx = false;
                                        ucEPCSDigitalSigning.PharmacyTask = MasterPage.RxTask;
                                        ucEPCSDigitalSigning.IsApprovalRequestWorkflow = true;
                                        ucEPCSDigitalSigning.EPCSMEDList = rxList;
                                    }
                                    else
                                    {
                                        //show overlay dialog here.
                                        isShowPrintOptionOverlay = true;
                                    }
                                }
                                else
                                {
                                    //show overlay dialog here.
                                    isShowPrintOptionOverlay = true;
                                }
                            }
                            else
                            {
                                //show overlay dialog here.
                                isShowPrintOptionOverlay = true;
                            }
                        }
                    }
                    else
                    {
                        //show overlay dialog here.
                        isShowPrintOptionOverlay = true;
                    }

                    if (isShowPrintOptionOverlay)
                    {
                        //show overlay dialog here.
                        ucCSMedRefillRequestNotAllowed.ShowPopUp();
                        ucEPCSDigitalSigning.EPCSMEDList = null;
                        //redirect to print page where we'll update, send and print
                        Hashtable htTaskRxID = new Hashtable();
                        htTaskRxID.Add(Convert.ToInt64(taskID), rx.RxID);
                        Session["HTTaskRxID"] = htTaskRxID;
                        Session["REMOVETASKATPRINT"] = "Y";
                    }
                }
                else
                {
                    string RxStatus = "SENTTOPHARMACY"; //Setting the status of the Prescription April 3 2007..
                    Allscripts.Impact.Prescription.UpdateRxDetailStatus(Session["LICENSEID"].ToString(), Session["USERID"].ToString(), rxID, RxStatus, base.DBID);

                    //display success message after sending the prescription to pharmacy from task tab
                    setSuccessMessage(false);

                    try
                    {

                        string scriptId = ScriptMessage.CreateScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.ShieldSecurityToken, base.SessionSiteID, base.DBID);

                        if (Session["STANDING"].ToString() == "1")
                        {
                            ScriptMessage.SendThisMessage(scriptId, Session["LicenseID"].ToString(), Session["USERID"].ToString(), base.DBID);
                        }

                        scriptId = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, base.DBID);

                        if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(scriptId)))
                        {
                            ScriptMessage.SendOutboundInfoScriptMessage(scriptId, base.SessionLicenseID, base.SessionUserID, base.DBID);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                

                if (Session["TASKID"] != null && Session["TASKTYPE"] != null)
                {
                    taskID = Convert.ToInt64(Session["TASKID"]);
                    Session["ApproveWorkflowRefillTaskId"] = taskID;
                    taskType = (Constants.PrescriptionTaskType)Convert.ToInt32(Session["TASKTYPE"]);

                    if (taskType == Constants.PrescriptionTaskType.APPROVAL_REQUEST)
                    {
                        //if epcs modal is shown here then do all these in the complete event.
                        if (isControledSubstance)
                        {
                            //EPCS overlay is shown. or print denied overlay is shown. In both case do post processing work in complete event.
                            Session["ApproveWorkflowRefillTaskId"] = taskID;
                        }
                        else
                        {                            
                            //If the task type is to approve, then mark the rx to approved status (NEW)
                            Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                            //Update the task status
                            Prescription.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, Session["USERID"].ToString(), base.DBID);

                            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);
                        }
                            PageState.Remove("TASKID");
                            PageState.Remove("TASKTYPE");
                        }
                    else if (taskType == Constants.PrescriptionTaskType.RENEWAL_REQUEST)
                    {
                        //Mark the script as approved (NEW)
                        Prescription.ApprovePrescription(rxID, 1, Session["LICENSEID"].ToString(), Session["USERID"].ToString(), base.DBID);

                        //Update the filll task data
                        if (Session["RefilltaskData"] != null)
                        {
                            RxTaskModel refilltask = (RxTaskModel)Session["RefilltaskData"];
                            Constants.PrescriptionTaskStatus taskstatus = (refilltask.RxRequestType == RequestType.APPROVE ? Constants.PrescriptionTaskStatus.ONE : Constants.PrescriptionTaskStatus.PROCESSED);

                            Prescription.UpdateRxTask(refilltask.RxTaskId, refilltask.PhysicianComments, refilltask.IsPatientVisitRq, (int)taskstatus, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);
                            Session.Remove("RefilltaskData");
                        }
                    }                

                    Session["LASTPHARMACYID"] = pharmacyID;

                    if (string.IsNullOrEmpty(pharmDesc))
                    {
                        Session["LASTPHARMACYNAME"] = grdViewPharmacy.SelectedRow.Cells[3].Text;
                    }
                    else
                    {
                        Session["LASTPHARMACYNAME"] = pharmDesc;
                    }
                    

                    if (ucEPCSDigitalSigning != null && ucEPCSDigitalSigning.EPCSMEDList != null && ucEPCSDigitalSigning.EPCSMEDList.Count > 0)
                    {
                        ucEPCSDigitalSigning.ShouldShowEpcsSignAndSendScreen();                       
                    }
                    else if(!isControledSubstance)
                    {
                        string redirectTo = Constants.PageNames.UrlForRedirection(Request.QueryString["To"].ToString());
                        if (redirectTo != null && (redirectTo.Contains(Constants.PageNames.APPROVE_REFILL_TASK)))
                        {
                            setSuccessMessage(false);
                            Response.Redirect(redirectTo + ((redirectTo.IndexOf('?') == -1)? "?" : "&") + "Msg=" + Server.UrlEncode(Session["SuccessMsg"].ToString()));
                        }

                        if (Request.QueryString["To"] != null)
                        {
                            Response.Redirect(redirectTo);
                        }

                        //let's remove this now since the print logic doesn't need them anymore and it was causing other workflow issues
                        Session.Remove("TASKID");
                        Session.Remove("TASKTYPE");
                    }

                }

                if (!isControledSubstance)
                {
                    Response.Redirect(Constants.PageNames.START_NEW_RX_PROCESS);
                }
                
            }
        }
    }

    private void HandleRedirect(object sender, EventArgs e)
    {
        if (ViewState["ClickIndicator"] != null)
        {
            switch (ViewState["ClickIndicator"].ToString())
            {
                case "btnContinue":
                        Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["from"].ToString()) + "?from=" + Constants.PageNames.PHARMACY);
                    break;
                case "btnSetPatientPharm":
                    Redirect();
                    break;
                case "btnSendScrip":
                    btnSendScrip_Click(sender, e);
                    break;
                default:
                    base.DefaultRedirect();
                    break;
            }
        }
        else
        {
            base.DefaultRedirect();
        }
    }

    private bool pharmacyExistsInSiteFavorites(string pharmacyID)
    {
        return Allscripts.Impact.Pharmacy.FavoritePharmacyExists(base.SessionLicenseID, Convert.ToInt32(Session["SiteID"].ToString()), pharmacyID, base.DBID);
    }

    protected void setSuccessMessage(bool msgType)
    {
        if (Session["RXID"] != null)
        {
            rxID = Session["RXID"].ToString();

            if (msgType)
            {
                DataSet dsMedName = Prescription.ChGetRXDetails(rxID, base.DBID);

                if (Request.QueryString["Patient"] != null)
                {
                    Session["SuccessMsg"] = "Rx for " + dsMedName.Tables[0].Rows[0]["MedicationName"].ToString() + " approved and successfully sent to default printer for " + Request.QueryString["Patient"].ToString() + ".";
                }
                else if (Session["PATIENTID"] != null)
                {
                    DataSet dsPat = CHPatient.PatientSearchById(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), string.Empty, base.DBID);
                    Session["SuccessMsg"] = "Rx for " + dsMedName.Tables[0].Rows[0]["MedicationName"].ToString() + " approved and successfully sent to default printer for " + dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                }

            }
            else if (Session["SuccessMsg"] == null)
            {
                if (Request.QueryString["Patient"] != null)
                {
                    DataSet ds = Prescription.Load(rxID, base.DBID);
                    Session["SuccessMsg"] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + Request.QueryString["Patient"].ToString() + ".";
                }
                else if (Session["PATIENTID"] != null)
                {
                    DataSet ds = Prescription.Load(rxID, base.DBID);
                    DataSet dsPat = CHPatient.PatientSearchById(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), string.Empty, base.DBID);
                    Session["SuccessMsg"] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                }
            }
        }
    }


    #endregion
}

    

}