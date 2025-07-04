/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
**04/29/2009   Anand Kumar Krishnan		Added PRINTINGPREFERENCE(1Up/4Up) print preview 
**                                      options. Option Features to print either four or one script 
**                                      per piece of paper. 
**              Kat Ernhart             Added sleep in page_load else for 3113
 * 03/04/2010   Sonal                   Added PrintBy as Plain paper and Secured Paper in Footer.
*******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using BOL = Allscripts.ePrescribe.Objects;
using AjaxControlToolkit;
using eRxWeb.Controls;
using System.Web.UI.HtmlControls;
using eRxWeb.AppCode;
using eRxWeb.AppCode.PptPlusBPL;
using PptPlusData = Allscripts.ePrescribe.Data.PptPlusData;
using CommonComponentData = Allscripts.ePrescribe.Data.CommonComponent.CommonComponentData;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
    public partial class MultipleViewCSS : BasePage
    {
        #region Member Fields

        string[] RxIdArray;// 
        Int64[] TaskIDArray;

        #endregion

        #region Properties

        private string printPref
        {
            get
            {
                int stateFormats = SystemConfig.GetStatePrintFormats(Session["PRACTICESTATE"].ToString());
                bool bFourUp = SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On && SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURUP, stateFormats);
                bool bOneUp = SystemConfig.AllowPrintFormat(Constants.PrintFormats.STANDARD, stateFormats);
                bool bFourRow = SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURROW, stateFormats);
                string ret = string.Empty;
                if (Session["PRINTINGPREFERENCE"] == null)
                {
                    if (SystemConfig.AllowPrintFormat(Constants.PrintFormats.STANDARD, stateFormats))
                    {
                        ret = "1Up";
                    }
                    else if (SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURUP, stateFormats) && SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On)
                    {
                        ret = "4Up";
                    }
                    else if (SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURROW, stateFormats))
                    {
                        ret = "4Row";
                    }
                }
                else
                {
                    if (Session["PRINTINGPREFERENCE"].ToString() == "1" && bOneUp)
                    {
                        ret = "1Up";
                    }
                    else if (Session["PRINTINGPREFERENCE"].ToString() == "4" && bFourUp)
                    {
                        ret = "4Up";
                    }
                    else if (Session["PRINTINGPREFERENCE"].ToString() == "R" && bFourRow)
                    {
                        ret = "4Row";
                    }
                    else
                    {
                        if (bFourUp && SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On)
                        {
                            ret = "4Up";
                        }
                        else if (bFourRow)
                        {
                            ret = "4Row";
                        }
                        else
                        {
                            ret = "1Up";
                        }
                    }
                }

                return ret;
            }
        }

        #endregion

        #region Page Methods and Handled Events
        protected void Page_Init(object sender, EventArgs e)
        {
            printPreviewGoodRxCoupons();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                if (Session["ProcessList"] != null || Session["HTTaskRxID"] != null)
                {
                    Random random = new Random();
                    pdfDocumentFrame.Attributes["name"] = "pdfDocumentFrame" + random.Next(999);

                    int stateFormats = SystemConfig.GetStatePrintFormats(Session["PRACTICESTATE"].ToString());
                    bool bMultiFormats = SystemConfig.AllowsMultipleFormats(stateFormats, SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On);
                    if (bMultiFormats)
                    {
                        if (SystemConfig.AllowPrintFormat(Constants.PrintFormats.STANDARD, stateFormats))
                        {
                            rdoPrintingOption.Items.Add(new ListItem("1Up"));
                        }

                        if (SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURUP, stateFormats) && SessionLicense.GetFeatureStatus(Constants.DeluxeFeatureType.FourUpPrinting) == Constants.DeluxeFeatureStatus.On)
                        {
                            rdoPrintingOption.Items.Add(new ListItem("4Up"));
                        }

                        if (SystemConfig.AllowPrintFormat(Constants.PrintFormats.FOURROW, stateFormats))
                        {
                            rdoPrintingOption.Items.Add(new ListItem("4Row"));
                        }

                        rdoPrintingOption.Attributes.Add("onclick", "javascript:SetStatus('" + rdoStatus.ClientID + "');");
                    }
                    else
                    {
                        rdoPrintingOption.Style["display"] = "none";
                    }

                    checkPaper();

                    if ((Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES") &&
                        (Request.QueryString["From"] != null && (Request.QueryString["From"].Contains(Constants.PageNames.SCRIPT_PAD.ToLower()) || Request.QueryString["From"].Contains(Constants.PageNames.TASK_SUMMARY))))
                    {
                        //straightforward workflow. allow for cancelling here.
                        btnCancel.Visible = true;
                        btnPrint.Visible = true;
                        btnCancel.Enabled = true;
                        btnPrint.Enabled = true;
                    }
                    else
                    {
                        //
                        // TO DO: is this necessary? its the same as above.
                        //
                        btnCancel.Visible = true;
                        btnPrint.Visible = true;
                        btnCancel.Enabled = true;
                        btnPrint.Enabled = true;
                    }

                    //ProcessList is for non-task Printing
                    if (Session["ProcessList"] != null)
                    {
                        int rxCount = ((ArrayList)Session["ProcessList"]).Count;
                        if (rxCount > 0)
                        {
                            RxIdArray = new string[rxCount];
                            int currRx = 0;
                            foreach (object item in (ArrayList)Session["ProcessList"])
                            {
                                if (item.GetType() == typeof(string))
                                {
                                    RxIdArray[currRx] = item.ToString();
                                    currRx++;
                                }
                            }

                            Session.Remove("ProcessList");
                            Session["RxIDArray"] = RxIdArray;
                            Session["MultipleRxID"] = RxIdArray[0];
                        }
                    }

                    if (Session["HTTaskRxID"] != null)
                    // AFter printing and sending script the session to be cleared.(IMP)
                    {
                        // Iterate through the Hashtable Extract the values and then updated the table.
                        Hashtable htScriptTaskRxID = (Hashtable)Session["HTTaskRxID"];
                        Session.Remove("HTTaskRxID");
                        RxIdArray = new string[htScriptTaskRxID.Count];
                        htScriptTaskRxID.Values.CopyTo(RxIdArray, 0);

                        foreach (string rxID in RxIdArray)
                        {
                            base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, rxID);
                        }

                        try
                        {
                            TaskIDArray = new Int64[htScriptTaskRxID.Count];
                            htScriptTaskRxID.Keys.CopyTo(TaskIDArray, 0);
                        }
                        catch (Exception ex)
                        {
                            //BAD WAY TO HANDLE THIS, BUT WE NEED TO FIGURE OUT A BETTER WAY TO PORT AROUND TASKS and RXs
                            Allscripts.Impact.Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Task Array wasn't populated properly in MultipleViewCSS.aspx.cs" + ex.ToString(), "", "", "", base.DBID);
                        }

                        Session["TaskIDArray"] = TaskIDArray;
                        Session["RxIDArray"] = RxIdArray;
                    }

                    printTabs.Tabs[0].HeaderText = "Scripts (" + RxIdArray.Length.ToString() + ")";

                    // set IFrame to printscriptcss.aspx, which will render the html for the prescriptions
                    printScript(rdoPrintingOption.SelectedValue);

                    if (rdoPrintingOption.Items.Count > 0)
                    {
                        rdoPrintingOption.SelectedValue = printPref;
                    }
                }
                else
                {
                    printTabs.Tabs[0].Visible = false;
                    printTabs.Tabs[0].HeaderText = string.Empty;
                }

                if (Session["PatientReceiptList"] != null|| Session["AllowPatientReceiptsList"] != null)
                {
                    if (Session["PatientReceiptList"] != null)
                    {
                        printTabs.Tabs[3].HeaderText = "Patient Receipts (" + ((List<string>)Session["PatientReceiptList"]).Count.ToString() + ")";
                    }

                    if (Session["AllowPatientReceiptsList"] != null)
                    {
                        printTabs.Tabs[3].HeaderText = "Patient Informational Copy (" + ((List<string>)Session["AllowPatientReceiptsList"]).Count.ToString() + ")";
                    }

                    printPatientReceiptScript();
                }
                else
                {
                    printTabs.Tabs[3].Visible = false;
                    printTabs.Tabs[3].HeaderText = string.Empty;
                }
                printPreviewGoodRxCoupons();
                printPreviewEcoupons();
                printPreviewRxInfoScript();

            }
        }

        public void printPreviewGoodRxCoupons()
        {
            if (PageState["PrintGoodRxCoupon"] != null)
            {

                goodRxCouponsFrame.Attributes["src"] = Constants.PageNames.GOODRX_COUPON;
                goodRxCouponsFrame.Attributes["width"] = "90%";
                goodRxCouponsFrame.Attributes["align"] = "center";
                List<string> rxIds = PageState.Cast<List<string>>("PrintGoodRxCoupon", new List<string>());
                if (rxIds.Count > 0)
                {
                    printTabs.Tabs[1].HeaderText = "GoodRx Coupons (" + rxIds.Count.ToString() + ")";
                }
                else
                {
                    printTabs.Tabs[1].Visible = false;
                    printTabs.Tabs[1].HeaderText = string.Empty;
                }
            }
            else
            {
                if(!setPrintTabDisplay(1))
                {
                    donePrinting();
                }
            }
        }

        
        private void printPreviewEcoupons()
        {
            if (PageState["AppliedCoupons"] != null || PageState["NotAppliedCoupons"] != null ||
                    PageState["PrintAppliedCoupons"] != null || PageState["PrintNotAppliedCoupons"] != null)
            {
                PageState.Remove("CouponPrintList");

                List<string> couponPrintList = new List<string>();

                if (PageState["AppliedCoupons"] != null)
                {
                    foreach (string eCouponDetailId in (List<string>)Session["AppliedCoupons"])
                    {
                        couponPrintList.Add(eCouponDetailId);
                    }
                }
                if (PageState["NotAppliedCoupons"] != null)
                {
                    foreach (string eCouponDetailId in (List<string>)Session["NotAppliedCoupons"])
                    {
                        couponPrintList.Add(eCouponDetailId);
                    }
                }
                if (PageState["PrintAppliedCoupons"] != null)
                {
                    foreach (string eCouponDetailId in (List<string>)Session["PrintAppliedCoupons"])
                    {
                        couponPrintList.Add(eCouponDetailId);
                    }
                }
                if (PageState["PrintNotAppliedCoupons"] != null)
                {
                    foreach (string eCouponDetailId in (List<string>)Session["PrintNotAppliedCoupons"])
                    {
                        couponPrintList.Add(eCouponDetailId);
                    }
                }

                if (couponPrintList.Count > 0)
                {
                    PageState["CouponPrintList"] = couponPrintList;

                    printTabs.Tabs[2].HeaderText = "Coupons (" + couponPrintList.Count.ToString() + ")";

                    printCoupons();
                }
                else
                {
                    printTabs.Tabs[2].Visible = false;
                    printTabs.Tabs[2].HeaderText = string.Empty;
                }
            }
            else
            {
                printTabs.Tabs[2].Visible = false;
                printTabs.Tabs[2].HeaderText = string.Empty;
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
        }

        protected void rdoPrintingOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            printScript(rdoPrintingOption.SelectedValue);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Convert.ToBoolean(Session["isCSRefillNotAllowed"]))
            {
                string[] RxID = Session["RxIDArray"] as string[];
                Prescription.Delete(RxID[0], base.DBID);
                Session.Remove("isCSRefillNotAllowed");
            }
            cancelPrinting();
        }

        protected void hiddenPrintTrigger_Click(object sender, EventArgs e)
        {
            //Changed it so that it will update the SenderBy field for having only 3 columns displayed April 12th 2007
            string[] RxID = Session["RxIDArray"] as string[];
            string RxStatus = "PRINTED"; //If you want to capture the PrintedBYID just uncomment this and then comment the senttopharmacy.

            if (Convert.ToBoolean(Session["isCSRefillNotAllowed"]))
            {
                if (base.SessionUserType == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED && !string.IsNullOrWhiteSpace(Session["DelegateProviderID"].ToString()))
                {
                    Prescription.UpdateRxDetailStatus(base.SessionLicenseID, Session["DelegateProviderID"].ToString(), RxID[0], "AUTHORIZEBY", base.DBID);
                }
                if (base.IsPOBUser && Session["SUPERVISING_PROVIDER_ID"] != null)
                {
                    // update the authorized id as the supervising physician's id
                    Prescription.UpdateRxDetailStatus(base.SessionLicenseID, Session["SUPERVISING_PROVIDER_ID"].ToString(), RxID[0], "AUTHORIZEBY", base.DBID);
                }

                ScriptMessage.RejectMessage(
                    Session[Constants.SessionVariables.TaskScriptMessageId].ToString(),
                    "AP",
                    "Request already responded to by other means(e.g.phone or fax)",
                    base.SessionUserID,
                    base.SessionLicenseID,
                    Guid.Empty.ToString(),
                    base.ShieldSecurityToken,
                    base.SessionSiteID,
                    base.DBID);

                Session.Remove(Constants.SessionVariables.TaskScriptMessageId);

                Session["REFILLMSG"] = "Controlled substance refill printed for faxing for " + 
                                        PageState.GetStringOrEmpty(Constants.SessionVariables.PatientLastName)+ 
                                        ", " +
                                        PageState.GetStringOrEmpty(Constants.SessionVariables.PatientFirstName) + 
                                        ". A deny response was sent to the pharmacy.";

                base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, base.SessionPatientID, RxID[0]);

            }

            foreach (string rx in RxID)
            {
                Allscripts.Impact.Prescription.UpdatePrescriptionStatus(rx, 1, Constants.PrescriptionStatus.NEW, base.SessionUserID, base.DBID);
                string extFacilityCode = Session["ExtFacilityCd"] != null ? Session["ExtFacilityCd"].ToString() : null;
                Allscripts.Impact.Prescription.UpdateRxDetailStatus(base.SessionLicenseID, base.SessionUserID, rx, RxStatus, extFacilityCode, base.DBID);
                sendPrintRxToSwitch(rx);

                //display success message after printing a script from Task tab
                //string successMessage = string.Empty;
                DataSet ds = Prescription.Load(rx, base.DBID);
                if (Request.QueryString["Patient"] != null)
                {
                    Session["SuccessMsg"] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + Request.QueryString["Patient"].ToString() + ".";
                }
                else if (Session["PATIENTID"] != null)
                {
                    DataSet dsPat = CHPatient.PatientSearchById(Session["PATIENTID"].ToString(), Session["LICENSEID"].ToString(), string.Empty, base.DBID);
                    Session["SuccessMsg"] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                }
            }

            deleteCurrentTask();

            Session.Remove("RxIDArray");

            sendECouponConfirmations("PrintOnlyCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTE);

            if (PageState["CouponPrintList"] == null)
            {
                sendECouponConfirmations("PrintNotAppliedCoupons", Constants.ECouponOfferDeliveryType.PRINTED);
                sendECouponConfirmations("PrintedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTEANDPRINTED);
                sendECouponConfirmations("PrintAppliedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTE);
            }
            else
            {
                CopyPageStateStringListToOther("PrintNotAppliedCoupons", "NotAppliedCoupons");
                PageState.Remove("PrintNotAppliedCoupons");

                CopyPageStateStringListToOther("PrintAppliedCoupons", "AppliedCoupons");
                PageState.Remove("PrintAppliedCoupons");
            }

            SendPPTUsageReport();

            setPageInfoMessage("Your Scripts have been sent to the printer.");

            checkForPrintingCompletion(tabToHideIndex:0);
            Session.Remove("isCSRefillNotAllowed");
        }

        private void SendPPTUsageReport()
        {
            if (PPTPlus.IsPPTPlusPreferenceOn(PageState))
            {
                var printedGoodRxCoupons = PageState.Cast<List<string>>("PrintedGoodRxCoupon", new List<string>());
                foreach (string goodRxCouponRxId in printedGoodRxCoupons)
                {
                    var sentToPharmacyCheckedGoodRxCoupon = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon, new List<string>());
                    bool isSentToPharmacyCheckedGoodRxCoupon = sentToPharmacyCheckedGoodRxCoupon.Contains(goodRxCouponRxId);
                    
                    var rxsSentToPharmacy = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyGoodRxCoupon, new List<string>());
                    bool isGoodRxCouponSentToPharmacy = rxsSentToPharmacy.Contains(goodRxCouponRxId);
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(goodRxCouponRxId), true, 
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        ,new CommonComponentData());
                    PPTPlus.AuditCouponStatus(PageState, Guid.Parse(goodRxCouponRxId), false, true, 
                            isSentToPharmacyCheckedGoodRxCoupon, isGoodRxCouponSentToPharmacy, new PptPlusData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(goodRxCouponRxId));
                }
                PageState.Remove("PrintedGoodRxCoupon");

                var notPrintedGoodRxCoupons = PageState.Cast<List<string>>("NotPrintedGoodRxCoupon", new List<string>());
                foreach (string goodRxCouponRxId in notPrintedGoodRxCoupons)
                {
                    var sentToPharmacyCheckedGoodRxCoupon = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon, new List<string>());
                    bool isSentToPharmacyCheckedGoodRxCoupon = sentToPharmacyCheckedGoodRxCoupon.Contains(goodRxCouponRxId);

                    var rxsSentToPharmacy = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyGoodRxCoupon, new List<string>());
                    bool isGoodRxCouponSentToPharmacy = rxsSentToPharmacy.Contains(goodRxCouponRxId);
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(goodRxCouponRxId), false, 
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        , new CommonComponentData());
                    PPTPlus.AuditCouponStatus(PageState, Guid.Parse(goodRxCouponRxId), false, false,
                            isSentToPharmacyCheckedGoodRxCoupon, isGoodRxCouponSentToPharmacy, new PptPlusData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(goodRxCouponRxId));
                }
                PageState.Remove("NotPrintedGoodRxCoupon");

                var printOnlyScriptsWithGoodRxCoupons = PageState.Cast<List<string>>("PrintOnlyScriptsWithGoodRxCoupons", new List<string>());
                foreach (string pptRxId in printOnlyScriptsWithGoodRxCoupons)
                {
                    var sentToPharmacyCheckedGoodRxCoupon = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon, new List<string>());
                    bool isSentToPharmacyCheckedGoodRxCoupon = sentToPharmacyCheckedGoodRxCoupon.Contains(pptRxId);

                    var rxsSentToPharmacy = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyGoodRxCoupon, new List<string>());
                    bool isGoodRxCouponSentToPharmacy = rxsSentToPharmacy.Contains(pptRxId);
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(pptRxId), false, 
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        , new CommonComponentData());
                    PPTPlus.AuditCouponStatus(PageState, Guid.Parse(pptRxId), false, false,
                            isSentToPharmacyCheckedGoodRxCoupon, isGoodRxCouponSentToPharmacy, new PptPlusData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(pptRxId));
                }
                PageState.Remove("PrintOnlyScriptsWithGoodRxCoupons");

                var printOnlyScriptsWithOutGoodRxCoupons = PageState.Cast<List<string>>("PrintOnlyScriptsWithOutGoodRxCoupons", new List<string>());
                foreach (string pptRxId in printOnlyScriptsWithOutGoodRxCoupons)
                {
                    var rxsSentToPharmacy = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyGoodRxCoupon, new List<string>());
                    bool isGoodRxCouponSentToPharmacy = rxsSentToPharmacy.Contains(pptRxId);
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(pptRxId), false, 
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        , new CommonComponentData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(pptRxId));
                }
                PageState.Remove("PrintOnlyScriptsWithOutGoodRxCoupons");
            }
        }

        protected void btnPrintCouponsCancel_Click(object sender, EventArgs e)
        {
            sendECouponConfirmations("AppliedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTE);
            PageState.Remove("NotAppliedCoupons");
            PageState.Remove("PrintNotAppliedCoupons");

            if (PageState["RxIDArray"] == null)
            {
                sendECouponConfirmations("PrintAppliedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTE);
            }

            if (setPrintTabDisplay(2))
            {
                PageState.Remove("CouponPrintList");
            }
            else
            {
                cancelPrinting();
            }
        }

        protected void printCouponsTrigger_Click(object sender, EventArgs e)
        {
            sendECouponConfirmations("AppliedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTEANDPRINTED);
            sendECouponConfirmations("NotAppliedCoupons", Constants.ECouponOfferDeliveryType.PRINTED);

            if (Session["RxIDArray"] == null)
            {
                // scripts have already been printed so complete any necessary processing for any coupons
                sendECouponConfirmations("PrintAppliedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTEANDPRINTED);
                sendECouponConfirmations("PrintNotAppliedCoupons", Constants.ECouponOfferDeliveryType.PRINTED);
            }
            else
            {
                CopyPageStateStringListToOther("PrintAppliedCoupons", "PrintedCoupons");
                PageState.Remove("PrintAppliedCoupons");
            }

            couponFrame.Attributes["src"] = string.Empty;
            setPageInfoMessage("Your Coupons have been sent to the printer.");
            checkForPrintingCompletion(tabToHideIndex: 2);
        }

        protected void btnGoodRxCoupontrigger_Click(object sender, EventArgs e)
        {
            setPageInfoMessage("You have printed your GoodRx Coupon.");
            var goodRxCouponList = PageState.Cast<List<string>>("PrintGoodRxCoupon", new List<string>());
            var printedGoodRxCoupons = PageState.Cast<List<string>>("PrintedGoodRxCoupon", new List<string>());
            var rxsSentToPharmacy = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyGoodRxCoupon, new List<string>());
            var rxNotPrintedYet = (PageState["RxIDArray"] != null) ? new List<string>((string[])PageState["RxIDArray"]) : new List<string>();
            var sentToPharmacyCheckedGoodRxCoupon = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon, new List<string>());

            foreach (string rxId in goodRxCouponList)
            {
                if (rxNotPrintedYet.Contains(rxId))
                {
                    printedGoodRxCoupons.Add(rxId);
                }
                else
                {
                    bool isSentToPharmacyCheckedGoodRxCoupon = sentToPharmacyCheckedGoodRxCoupon.Contains(rxId);
                    bool isGoodRxCouponSentToPharmacy = rxsSentToPharmacy.Contains(rxId);
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(rxId), true,
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        , new CommonComponentData());
                    PPTPlus.AuditCouponStatus(PageState, Guid.Parse(rxId), false, true,
                            isSentToPharmacyCheckedGoodRxCoupon, isGoodRxCouponSentToPharmacy, new PptPlusData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxId));
                }
            }
            PageState["PrintedGoodRxCoupon"] = printedGoodRxCoupons;
            PageState.Remove("PrintGoodRxCoupon");
            checkForPrintingCompletion(tabToHideIndex: 1);
        }

        protected void btnGoodRxCouponCancel_Click(object sender, EventArgs e)
        {
            PageState.Remove("NotPrintedGoodRxCoupon");
            var goodRxCouponList = PageState.Cast<List<string>>("PrintGoodRxCoupon", new List<string>());
            var rxNotPrintedYet = (PageState["RxIDArray"] != null) ? new List<string>((string[])PageState["RxIDArray"]) : new List<string>();
            var notPrintedGoodRxCoupons = PageState.Cast<List<string>>("NotPrintedGoodRxCoupon", new List<string>());
            var rxsSentToPharmacy = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyGoodRxCoupon, new List<string>());
            var sentToPharmacyCheckedGoodRxCoupon = PageState.Cast<List<string>>(Constants.SessionVariables.SentToPharmacyCheckedGoodRxCoupon, new List<string>());

            foreach (string rxId in goodRxCouponList)
            {
                if (rxNotPrintedYet.Contains(rxId))
                {
                    notPrintedGoodRxCoupons.Add(rxId);
                }
                else
                {
                    bool isSentToPharmacyCheckedGoodRxCoupon = sentToPharmacyCheckedGoodRxCoupon.Contains(rxId);
                    bool isGoodRxCouponSentToPharmacy = rxsSentToPharmacy.Contains(rxId);
                    PPTPlus.SendUsageReport(PageState, new PptPlus(), Guid.Parse(rxId), false, 
                        isGoodRxCouponSentToPharmacy, new PptPlusServiceBroker(), new PptPlusData()
                        ,new CommonComponentData());
                    PPTPlus.AuditCouponStatus(PageState, Guid.Parse(rxId), false, false,
                        isSentToPharmacyCheckedGoodRxCoupon, isGoodRxCouponSentToPharmacy, new PptPlusData());
                    PPTPlus.RemoveScriptPadMedByRxID(PageState, new PptPlus(), Guid.Parse(rxId));
                }
            }
            PageState["NotPrintedGoodRxCoupon"] = notPrintedGoodRxCoupons;
            PageState.Remove("PrintGoodRxCoupon");

            if (!setPrintTabDisplay(1))
            {
                cancelPrinting();
            }
        }
        protected void btnRxInfoCancel_Click(object sender, EventArgs e)
        {
            if (setPrintTabDisplay(4))
            {
                PageState.Remove("RxInfoDetailsIdList");
            }
            else
            {
                cancelPrinting();
            }
        }

        protected void btnPatientReceiptCancel_Click(object sender, EventArgs e)
        {
            if (setPrintTabDisplay(3))
            {
                PageState.Remove("PatientReceiptList");
                PageState.Remove("AllowPatientReceiptsList");
            }
            else
            {
                cancelPrinting();
            }
        }

        protected void hiddenReceiptPrintTrigger_Click(object sender, EventArgs e)
        {
            setPageInfoMessage("Your Patient Receipts have been sent to the printer.");
            checkForPrintingCompletion(tabToHideIndex: 3);
        }

        protected void printRxInfoTrigger_Click(object sender, EventArgs e)
        {
            sendRxInfoDelivery();
            setPageInfoMessage("Your RxInfo have been sent to the printer.");
            checkForPrintingCompletion(tabToHideIndex: 4);
        }
        private void setPageInfoMessage(string message)
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = message;
        }
        private void sendRxInfoDelivery()
        {
            List<string> rxInfoDetailsList = (List<string>)Session["RxInfoDetailsIdList"];
            foreach (var rxInfoDetailsId in rxInfoDetailsList)
            {
                EPSBroker.ConfirmRxInfoDelivery(rxInfoDetailsId, Constants.RxInfoOfferDeliveryType.PRINT, base.SessionLicenseID, base.SessionUserID, base.DBID);
            }
        }

        private void checkForPrintingCompletion(int tabToHideIndex)
        {
            if (!setPrintTabDisplay(tabToHideIndex))
            {
                donePrinting();
            }
        }
        private bool setPrintTabDisplay(int tabToHideIndex)
        {
            printTabs.Tabs[tabToHideIndex].Visible = false;
            printTabs.Tabs[tabToHideIndex].HeaderText = string.Empty;

            for (int i = 0; i < printTabs.Tabs.Count; i++)
            {
                if (printTabs.Tabs[i].Visible)
                {
                    printTabs.ActiveTabIndex = printTabs.Tabs[i].TabIndex;
                    return true;
                }
            }

            return false;
        }


        #endregion

        #region Custom Methods

        private void cancelPrinting()
        {
            sendECouponConfirmations("AppliedCoupons", Constants.ECouponOfferDeliveryType.PHARMACYNOTE);
            PageState.Remove("NotAppliedCoupons");
            PageState.Remove("PrintAppliedCoupons");
            PageState.Remove("PrintNotAppliedCoupons");
            PageState.Remove("PrintOnlyCoupons");

            PageState.Remove("PrintedCoupons");
            PageState.Remove("CouponPrintList");

            Session.Remove("RxInfoDetailsIdList");
            PageState.Remove("PrintGoodRxCoupon");
            PageState.Remove("PrintedGoodRxCoupon");
            PageState.Remove("NotPrintedGoodRxCoupon");
            PageState.Remove("PrintOnlyScriptsWithGoodRxCoupons");
            PageState.Remove("PrintOnlyScriptsWithOutGoodRxCoupons");

            if (Session["RxIDArray"] != null)
            {
                //
                // There are scripts that still need to be process from some workflow, so go back to where you came from
                //
                CancelREFREQPrinting(); //Cancel the REFREQ task printing
                deleteCurrentTask();
                Session["REMOVETASKATPRINT"] = "N";
                Session.Remove("MultipleRxID");
                Session.Remove("MultipleTaskID");
                Session.Remove("TASKID");
                Session.Remove("TASKTYPE");
                Session.Remove("RxIDArray");
                Session["REFILLMSG"] = null;

                if (base.SessionLicense.EnterpriseClient.ShowEPA)
                {
                    if (Session["EPAPrintTaskItems"] != null)
                    {
                        Session.Remove("EPAPrintTaskItems");
                    }
                }

                if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString().Contains(Constants.PageNames.SPECIALTYMEDTASKS))
                {
                    Response.Redirect(Constants.PageNames.SPECIALTYMEDTASKS);
                }

                if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString().Contains(Constants.PageNames.DOC_REFILL_MENU))
                {
                    Session.Remove("RefillTasksToPrint");
                    Response.Redirect(Constants.PageNames.DOC_REFILL_MENU);
                }

                if (Request.QueryString["To"] != null)
                {
                    if (PageState["ePATaskStatus"] != null)
                    {
                        int ePATaskStatus = (int)PageState["ePATaskStatus"];
                        PageState.Remove("ePATaskStatus");
                        Response.Redirect(Allscripts.ePrescribe.Common.Constants.PageNames.UrlForRedirection(Request.QueryString["To"]) + "?ePATaskStatus=" + ePATaskStatus);
                    }
                    else
                        Response.Redirect(Allscripts.ePrescribe.Common.Constants.PageNames.UrlForRedirection(Request.QueryString["To"]));
                }
                else
                {
                    Response.Redirect(Constants.PageNames.SCRIPT_PAD + "?from=" + Constants.PageNames.MULTIPLE_VIEW_CSS.ToLower());
                }
            }
            else
            {
                Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
            }
        }

        private void checkPaper()
        {
            if (Allscripts.Impact.SystemConfig.CheckSecurityPaper(Session["PRACTICESTATE"].ToString()))
            {
                //string msg = "Prescriptions printed in this state require special security paper.  If you have not obtained this special paper, please visit our paper vendor, <a target=_paper href='http://www.rxpaper.com/ePrescribe/'>http://www.rxpaper.com/ePrescribe/</a>.  Select the state you are practicing in and follow the link for instructions on ordering.  It is the users' responsibility to obtain and use the proper paper to comply with state requirements.";
                //panelMessageHeader.Style.Add("BACKGROUND", "#ad0808");
                //lblPrintingInfo.Style.Add("BACKGROUND", "#ad0808");
                //((LiteralControl)panelMessage.Controls[0]).Text = msg;
            }
        }

        /// <summary>
        /// Cancel the REFREQ task printing
        /// </summary>
        protected void CancelREFREQPrinting()
        {
            if (Session["PRINTCSREFREQ"] != null && Session["PRINTCSREFREQ"].ToString() == "Y")
            {
                foreach (Int64 task in (Int64[])Session["TaskIDArray"])
                {
                    Prescription.UpdateRxTask(task, Constants.PrescriptionTaskType.REFREQ, Constants.PrescriptionTaskStatus.NEW, Constants.PrescriptionStatus.NEW, string.Empty, string.Empty, base.SessionUserID, base.DBID);
                }
                Session.Remove("PRINTCSREFREQ");
            }
        }


        protected void deleteCurrentTask()
        {
            if (Session["REMOVETASKATPRINT"] != null && Session["REMOVETASKATPRINT"].ToString() == "Y")
            {
                string printInd = Request.Form["PrintInd"].Trim();

                //If the script is printed, then update the rx status and delete the task from user's task queue
                if (printInd.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    //update the prescription after print                               
                    foreach (string rx in (string[])Session["RxIdArray"])
                    {
                        Allscripts.Impact.Prescription.UpdatePrescriptionStatus(
                            rx,
                            1,
                            Constants.PrescriptionStatus.NEW,
                            base.SessionUserID,
                            base.DBID);
                    }

                    foreach (Int64 task in (Int64[])Session["TaskIDArray"])
                    {
                        Prescription.UpdateRxTask(task, "", "", (int)Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, base.SessionUserID, base.SessionLicenseID, base.DBID);
                    }
                }
            }
        }

        private void donePrinting()
        {
            Session["REMOVETASKATPRINT"] = "N";
            Session.Remove("PRINTCSREFREQ");
            Session.Remove("MultipleRxID");
            Session.Remove("MultipleTaskID");
            Session.Remove("RxIDArray");
            Session.Remove("CouponPrintList");
            Session.Remove("RxInfoDetailsIdList");

            if (base.SessionLicense.EnterpriseClient.ShowEPA)
            {
                if (Session["EPAPrintTaskItems"] != null)
                {
                    int printTaskCount = ((ArrayList)Session["EPAPrintTaskItems"]).Count;
                    if (printTaskCount > 0)
                    {
                        foreach (ePA.EpaTaskCancelObject epaTaskCancelObject in (ArrayList)Session["EPAPrintTaskItems"])
                        {
                            if (epaTaskCancelObject.IsCancelTask)
                            {
                                EPSBroker.EPA.SendEpaCancelRequest(epaTaskCancelObject.TaskID, base.SessionLicenseID, base.SessionUserID, base.NcpdpEpaUserShieldSecurityToken, base.DBID);
                            }
                            else
                            {
                                EPSBroker.EPA.UpdateTaskStatus(epaTaskCancelObject.TaskID, Constants.ePATaskStatus.EPA_REMOVED_PROCESSED, base.DBID);
                            }
                        }
                        Session.Remove("EPAPrintTaskItems");
                    }

                }
            }

            if (Session["CameFrom"] != null && Session["CameFrom"].ToString().Contains(Constants.PageNames.SPECIALTYMEDTASKS))
            {
                Server.Transfer(Constants.PageNames.SPECIALTYMEDTASKS);
            }

            if (Session["CameFrom"] != null && Session["CameFrom"].ToString().Contains(Constants.PageNames.TASK_SCRIPT_LIST))
            {
                Server.Transfer(Constants.PageNames.TASK_SCRIPT_LIST);
            }

            if (Session["CameFrom"] != null && Session["CameFrom"].ToString().Contains(Constants.PageNames.PHARMACY))
            {
                //close out the task
                if (Session["TASKID"] != null && Session["TASKTYPE"] != null)
                {
                    Int64 taskID = Convert.ToInt64(Session["TASKID"]);
                    Prescription.UpdateRxTask(taskID, "", "", (int)Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, base.SessionUserID, base.SessionLicenseID, base.DBID);
                }
                Session["SentTo"] = "Printer";
                Session["PrintInd"] = "Y";
                Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
            }

            // Added by AKS as on Sep 13th 2006.
            if (Session["CameFrom"] != null && Session["CameFrom"].ToString().Contains(Constants.PageNames.REVIEW_HISTORY))
            {
                string componentName = Allscripts.ePrescribe.Common.Constants.PageNames.REVIEW_HISTORY;
                string componentParameters = string.Empty;
                Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR+"?componentName=" + componentName + "&componentParameters=" + componentParameters);
            }

            if (Session["CameFrom"] != null && Session["CameFrom"].ToString().Contains(Constants.PageNames.TASK_SUMMARY))
            {
                if (PageState["ePATaskStatus"] != null)
                {
                    int ePATaskstatus = (int)PageState["ePATaskStatus"];
                    PageState.Remove("ePATaskStatus");
                    Response.Redirect(Constants.PageNames.TASK_SUMMARY + "?Msg=" + Server.UrlEncode("Prescription successfully processed.") + "&ePATaskStatus=" + ePATaskstatus);
                }
                else
                    Response.Redirect(Constants.PageNames.TASK_SUMMARY + "?Msg=" + Server.UrlEncode("Prescription successfully processed."));
            }

            if (Request.QueryString["To"] != null &&
                (Request.QueryString["To"].ToString().Contains(Constants.PageNames.LIST_SEND_SCRIPTS)))
            {
                // To show the success message, we are appending the query string with the flag TaskProcessed=true.
                string queryString = Request.QueryString["To"].ToString() + "&TaskProcessed=true";
                Server.Transfer(queryString);
            }

            if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString().Contains(Constants.PageNames.DOC_REFILL_MENU))
            {
                if (Session["RefillTasksToPrint"] != null)
                {
                    foreach (var refillTask in (List<RxTaskModel>)Session["RefillTasksToPrint"])
                    {
                        Prescription.UpdateRxTask(refillTask.RxTaskId, refillTask.PhysicianComments, refillTask.IsPatientVisitRq, (int)Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, Session["USERID"].ToString(), Session["LICENSEID"].ToString(), base.DBID);
                    }
                }
                Session.Remove("RefillTasksToPrint");
                Response.Redirect(Constants.PageNames.DOC_REFILL_MENU + "?Msg=" + Server.UrlEncode("Prescription successfully processed."));
            }

            if (Request.QueryString["PrintScript"] != null && Request.QueryString["PrintScript"] == "YES")
            {
                Session["PrintInd"] = Request.Form["PrintInd"];
                Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
            }

            if (Request.QueryString["To"] != null)
            {
                Server.Transfer(Request.QueryString["To"]);
            }
            else
            {
                Server.Transfer(Constants.PageNames.START_NEW_RX_PROCESS);
            }
        }

        public int getTableHeight()
        {
            int tableHeight = 0;
            if (Session["PAGEHEIGHT"] != null)
            {
                tableHeight = Convert.ToInt32(Session["PAGEHEIGHT"]) - 220;
            }
            else
                Server.Transfer(Constants.PageNames.LOGOUT + "?Timeout=YES");
            return tableHeight;
        }

        private void printCoupons()
        {
            couponFrame.Attributes["src"] = Constants.PageNames.PRINT_COUPONS;
            couponFrame.Attributes["height"] = getTableHeight().ToString();
            couponFrame.Attributes["width"] = "90%";
            couponFrame.Attributes["align"] = "center";
        }

        private void printScript(string printPreference)
        {
            string overrideState = string.Empty;
            string printingOption = string.Empty;

            if (Request.QueryString["StateOverride"] != null)
            {
                overrideState = "&StateOverride=" + Request.QueryString["StateOverride"];
            }

            printingOption = rdoPrintingOption.SelectedValue;
            if (string.IsNullOrEmpty(printingOption))
            {
                printingOption = printPref;
            }

            pdfDocumentFrame.Attributes["src"] = Constants.PageNames.PRINT_SCRIPT_CSS + "?PrintBy=" + printingOption + overrideState;
          ///  pdfDocumentFrame.Attributes["height"] = getTableHeight().ToString();
            pdfDocumentFrame.Attributes["width"] = "90%";
            pdfDocumentFrame.Attributes["align"] = "center";
        }

        private void printPatientReceiptScript()
        {
            btnPatientReceiptPrint.Visible = true;
            btnPatientReceiptCancel.Visible = true;

            //((LiteralControl)panelHelp.Controls[0]).Text = "Printing will be performed based on your web browser print settings.";

            //string msg = "This is not a prescription. This is an informational copy of medications that have been prescribed and/or sent electronically to the pharmacy during the patient's visit with the doctor.";
            //panelMessageHeader.Style.Add("BACKGROUND", "#ad0808");
            //lblPrintingInfo.Style.Add("BACKGROUND", "#ad0808");
            //((LiteralControl)panelMessage.Controls[0]).Text = msg;

            patientReceiptFrame.Attributes["src"] = Constants.PageNames.PATIENT_RECEIPT;
            patientReceiptFrame.Attributes["height"] = getTableHeight().ToString();
            patientReceiptFrame.Attributes["width"] = "90%";
            patientReceiptFrame.Attributes["align"] = "center";
        }

        private void printPreviewRxInfoScript()
        {
            if (Session["RxInfoDetailsIdList"] != null)
            {
                printTabs.Tabs[4].HeaderText = "RxInfo (" + ((List<string>)Session["RxInfoDetailsIdList"]).Count.ToString() + ")"; ;
                btnRxInfoPrint.Visible = true;
                btnRxInfoCancel.Visible = true;

                rxInfoFrame.Attributes["src"] = Constants.PageNames.PRINT_RX_INFO;
                rxInfoFrame.Attributes["height"] = getTableHeight().ToString();
                rxInfoFrame.Attributes["width"] = "90%";
                rxInfoFrame.Attributes["align"] = "center";
            }
            else
            {
                printTabs.Tabs[4].Visible = false;
                printTabs.Tabs[4].HeaderText = string.Empty;
            }
        }

        private void sendECouponConfirmations(string eCouponListName, Constants.ECouponOfferDeliveryType eCouponOfferDeliveryType)
        {
            if (PageState[eCouponListName] != null)
            {
                List<string> eCouponList = new List<string>();
                eCouponList = (List<string>)PageState[eCouponListName];

                foreach (string item in eCouponList)
                {
                    EPSBroker.ConfirmECouponDelivery(item, eCouponOfferDeliveryType, base.SessionLicenseID, base.SessionUserID, base.DBID);
                }

                PageState.Remove(eCouponListName);
            }
        }
        private void CopyPageStateStringListToOther(string srcistName, string destListName)
        {
            List<string> finalList = new List<string>();

            if (PageState[destListName] != null)
            {
                finalList = (List<string>)PageState[destListName];
            }
            if (PageState[srcistName] != null)
            {
                List<string> tempList = (List<string>)PageState[srcistName];
                foreach (string item in tempList)
                {
                    if (!finalList.Contains(item))
                    { finalList.Add(item); }
                }
            }

            PageState[destListName] = finalList;
        }

        private void sendPrintRxToSwitch(string rxID)
        {
            try
            {
                base.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, base.SessionPatientID, rxID);

                string messageID = ScriptMessage.CreateScriptMessage(
                    rxID,
                    1,
                    Constants.MessageTypes.PRINTRX,
                    base.SessionLicenseID,
                    base.SessionUserID,
                    string.Empty, //base.ShieldSecurityToken,
                    base.SessionSiteID,
                    base.DBID);

                if (!string.IsNullOrEmpty(messageID))
                {
                    if (Session["STANDING"].ToString() == "1")
                    {
                        ScriptMessage.SendOutboundInfoScriptMessage(messageID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                    }
                }
                else
                {
                    Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Error sending PRTRX to switch: ScriptMessageID is null. RxID: " + rxID, Request.UserIpAddress(), "", "", base.DBID);
                }

                messageID = ScriptMessage.CreateDUREVTScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, base.DBID);
                if ((Session["STANDING"].ToString() == "1") && (!string.IsNullOrEmpty(messageID)))
                {
                    ScriptMessage.SendOutboundInfoScriptMessage(messageID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                }
            }
            catch (Exception ex)
            {
                Audit.AddException(base.SessionUserID, base.SessionLicenseID, "Error sending PRTRX to switch: " + ex.ToString(), Request.UserIpAddress(), "", "", base.DBID);
            }
        }

        #endregion

    }

}