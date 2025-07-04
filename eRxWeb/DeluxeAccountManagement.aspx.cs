using System;
using System.Data;
using System.Configuration;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Telerik.Web.UI;
using eRxWeb.AppCode;
using eRxWeb.State;

namespace eRxWeb
{
    public partial class DeluxeAccountManagement : BasePage
    {
        const string EPCS_SETUP_FEE_TITLE = "Veradigm ePrescribe™ EPCS - Setup fee(s)";

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["CurrentModule"] = null;


            if (!IsPostBack)
            {
                Constants.DeluxePricingStructure pricingStructureType = Constants.DeluxePricingStructure.Deluxe;
                
                //show back button only when not forced into this page.
                backButton.Visible = !BillingUtil.ShouldUserFinishAPayment(new StateContainer(Session));

                if (AppCode.CompulsoryBasicUtil.IsCompulsoryBasicPurchased(SessionLicense.IsEnterpriseCompulsoryBasicOrForceCompulsoryBasic(),
                                SessionLicense.IsPricingStructureCompulsoryBasic()))
                {
                    spnEnrolledProviderFeature.InnerHtml = "Pricing for ePrescribe™ Basic is based on the number of enrolled providers. <br/>" +
                                        "Sharing user IDs is a violation of the terms of service and can result in immediate deactivation.";
                    hdnBasicOrDeluxe.Value = "Basic";
                    pricingStructureType = Constants.DeluxePricingStructure.CompulsoryBasic;
                    Image1.ImageUrl = "~/images/eprescribebasic.gif";
                }
                if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Disabled)
                {
                    onlHold.Style["Display"] = "inline";
                    onHoldMessage.Visible = true;
                    divEdit.Visible = false;
                    spanStartDate.Visible = false;
                    lblStartDate.Visible = false;
                    spanNextBilling.Visible = false;
                    lblEndDate.Visible = false;
                }
                else if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                {
                    cancelled.Style["Display"] = "inline";
                    cancelledMessage.Visible = true;
                    updateLink.Style["Display"] = "none";
                    divDeluxeCancel.Visible = false;
                    divEdit.Visible = false;
                }
                else if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Off)
                {
                    divDeluxeCancel.Visible = false;
                }

                DeluxeAccountUtil.ConstructCancelReasons(pricingStructureType, cancelReason);

                lnkEdit.HRef = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=DeluxeFeatureSelectionComponent" + "?From=" + Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT;


                DataTable dt = DeluxePurchaseManager.GetDeluxeAccountInfo(base.SessionLicenseID, base.DBID).Tables[0];
                if (dt.Rows.Count > 0
                    && dt.Rows[0]["PaymentProfileID"] != DBNull.Value)
                {
                    DataRow accountInfo = (DataRow)dt.Rows[0];

                    if (accountInfo["ModuleSubscribed"] != DBNull.Value)
                    {
                        string productName = accountInfo["ModuleSubscribed"].ToString().ToLower();

                        if (!string.IsNullOrEmpty(productName))
                            Session["CurrentDeluxeSubscription"] = productName;

                        string productDesc = getProductDescription(productName);
                        lblProductName.Text = productDesc;
                        lblProviders.Text = productDesc + " - Enrolled Providers";

                    }

                    if (accountInfo["DeluxeTranStartDate"] is DBNull)
                    {
                        lblStartDate.Text = "";
                        divDeluxeCancel.Visible = false;
                        updateLink.Style["Display"] = "none";
                    }
                    else
                        lblStartDate.Text = Convert.ToDateTime(accountInfo["DeluxeTranStartDate"]).ToString("dd MMM yyyy");


                    if (accountInfo["TranNextDate"] != DBNull.Value)
                    {
                        cancelDate.InnerText = Convert.ToDateTime(accountInfo["TranNextDate"]).ToString("MM/dd/yyyy");
                        cancelledMessage.Text = cancelledMessage.Text + Convert.ToDateTime(accountInfo["TranNextDate"]).ToString("MM/dd/yyyy") + ".";
                    }

                    if (base.SessionLicense.LicenseDeluxeStatus != Constants.DeluxeFeatureStatus.Cancelled)
                    {
                        if (!string.IsNullOrEmpty(accountInfo["TranNextDate"].ToString()))
                        {
                            lblEndDate.Text = Convert.ToDateTime(accountInfo["TranNextDate"]).ToString("dd MMM yyyy");
                        }
                        else
                        {
                            lblEndDate.Text = "N/A ";
                        }
                    }

                    string billingText = accountInfo["FirstName"].ToString() + " " + accountInfo["LastName"].ToString() + "<br />";

                    billingText += accountInfo["Address1"].ToString();

                    if (accountInfo["Address1"] != DBNull.Value && !string.IsNullOrEmpty(accountInfo["Address1"].ToString()))
                    {
                        billingText += "<br />" + accountInfo["Address2"].ToString();
                    }
                    if (accountInfo["Address2"] != DBNull.Value && !string.IsNullOrEmpty(accountInfo["Address2"].ToString()))
                    {
                        billingText += "<br />";
                    }
                    billingText += accountInfo["City"].ToString() + ", " + accountInfo["State"].ToString() + " " + accountInfo["Zipcode"].ToString() + "<br /><br />";
                    litCreditCard.Text = "Credit Card ending in: " + accountInfo["CardNo"].ToString();

                    litBilling.Text = billingText;

                    if (!Convert.ToBoolean(Session["IsAdmin"]))
                    {
                        disableEditFeatures();
                    }
                }
                else
                {
                    cancelledMessage.Text = "N/A";
                    updateLink.Visible = false;
                    btnPaidProviders.Visible = false;
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "You're currently using a free trial of ePrescribe Deluxe. Please contact support to purchase or cancel.";
                    ucMessage.Icon = Controls_Message.MessageType.INFORMATION;
                }
            }

            IStateContainer requestState = new StateContainer(System.Web.HttpContext.Current.Session);
            bool isAdmin = requestState.GetBooleanOrFalse(Constants.SessionVariables.IsAdmin);

            if (!isAdmin)
            {
                trBillingInfo.Visible = false;
                trAddressInfo.Visible = false;
                trCreditInfo.Visible = false;
                LoadingPanel1.Visible = false;
                divEdit.Visible = false;
                divDeluxeCancel.Visible = false;
                grdViewHistory.Visible = false;
                trAccountHistory.Visible = false;
            }
        }

        private string getProductDescription(string productName)
        {
            string description = string.Empty;

            //Get the Module Subscribed.
            description = getModuleSubscribed(productName, base.DBID);
            if (productName.ToLower().Trim().Contains("hard token"))
            {
                description = "Veradigm ePrescribe™ EPCS Hard Token Device";
            }
            return description;

        }

        private string getProductFullDescription(string productName)
        {
            string description = string.Empty;
            //Get the Module Subscribed.
            description = getModuleSubscribed(productName, base.DBID);
            if (productName.Contains("logrx"))
            {
                description = description + " with Sponsored Messages";
            }
            return description;

        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
            ((PhysicianMasterPageBlank)Master).hideMessageQueue();
            ((PhysicianMasterPageBlank)Master).HideHelp();

        }
        protected void btnTransAction_Click(object sender, EventArgs e)
        {
            string status = (sender as Button).Attributes["TransStatus"];

            if (status != "0")
            {
                if (status == "1")
                {
                    if (!string.IsNullOrEmpty((sender as Button).Attributes["OrderNumber"].ToString()))
                        rcptOrderStatus.InnerText = "Order Number " + (sender as Button).Attributes["OrderNumber"] + ".";
                    else
                        rcptOrderStatus.InnerText = "";

                    rcptOrderInfo.InnerText = "Your receipt was sent to " + (sender as Button).Attributes["Email"] + ".";

                    if (!string.IsNullOrEmpty((sender as Button).Attributes["OrderNumber"].ToString()))
                        lblOrderNumber.Text = " Order Number :" + (sender as Button).Attributes["OrderNumber"];
                    else
                        lblOrderNumber.Text = "Order Number : N/A";

                    btPrint.Text = "Print Receipt";
                }
                else
                {
                    rcptOrderStatus.InnerText = "ORDER PENDING";
                    rcptOrderInfo.InnerText = "Your credit card is scheduled to be billed on " + (sender as Button).Attributes["OrderDate"] + " for the following: ";
                    lblOrderNumber.Text = "Order Number : N/A";
                    btPrint.Text = "Print Invoice";
                }

                lblOrderDescription.Text = (sender as Button).Attributes["Description"];
                lblOrderCCNo.Text = (sender as Button).Attributes["CardNo"];
                // lblOrderCCExpDate.Text = (sender as Button).Attributes["ExpDate"];
                lblOrderAmount.Text = "$" + Convert.ToDecimal((sender as Button).Attributes["Amount"]).ToString("0.00");
                lblOrderDate.Text = (sender as Button).Attributes["OrderDate"];

                pnlAccount.Style["Display"] = "none";
                pnlPurchaseReceipt.Style["Display"] = "inline";
                backButton.Style["Display"] = "none";
                btnStayOnPage.Visible = true;
                btnContinueLogOff.Visible = false;
                btnPaidProviders.Visible = false;
            }
            else
            {
                string orderNumber = (sender as Button).Attributes["OrderNumber"];
                this.resubmit_transaction();
            }
        }

        protected void grdViewHistory_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem tempDataItem = (e.Item as GridDataItem);
                Label orderStatus = (tempDataItem["OrderStatus"].FindControl("lblOrderStatus") as Label);
                Label description = (tempDataItem["Description"].FindControl("lblDescription") as Label);
                string descriptionText = BusinessTextHelper.FormatPricingStructureDescriptionText(tempDataItem.GetDataKeyValue("Description").ToString(), Constants.ApplicationEnum.ePrescribe);
                description.Text = descriptionText;
                Button actionButton = (tempDataItem["Action"].FindControl("btnTransAction") as Button);
                int status = Convert.ToInt32(tempDataItem.GetDataKeyValue("TransStatus"));
                string orderNumber = tempDataItem.GetDataKeyValue("OrderNumber").ToString();
                string orderDate = Convert.ToDateTime(tempDataItem.GetDataKeyValue("OrderDate")).ToString("MM/dd/yyyy");

                if (actionButton != null)
                {
                    actionButton.Attributes.Clear();
                    actionButton.Attributes.Add("TransStatus", status.ToString());
                    actionButton.Attributes.Add("OrderNumber", orderNumber);
                    actionButton.Attributes.Add("Description", descriptionText); ;
                    actionButton.Attributes.Add("CardNo", tempDataItem.GetDataKeyValue("CardNo").ToString());

                    if ((tempDataItem.GetDataKeyValue("Amount") == null) ||
                        (string.IsNullOrEmpty(tempDataItem.GetDataKeyValue("Amount").ToString())))
                        actionButton.Attributes.Add("Amount", "0");
                    else
                        actionButton.Attributes.Add("Amount", tempDataItem.GetDataKeyValue("Amount").ToString());

                    actionButton.Attributes.Add("Email", tempDataItem.GetDataKeyValue("Email").ToString());
                    actionButton.Attributes.Add("OrderDate", orderDate);
                    if (orderStatus != null)
                    {
                        switch (status)
                        {
                            case 0:
                                //FAILED
                                tempDataItem["OrderDate"].ForeColor = System.Drawing.Color.Red;
                                orderStatus.ForeColor = System.Drawing.Color.Red;
                                orderStatus.Text = "FAILED";
                                actionButton.Text = "Resubmit";
                                actionButton.Visible = true;
                                if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled)
                                {
                                    actionButton.Enabled = false;
                                }
                                break;
                            case 1:
                                //APPROVED
                                orderStatus.Text = orderNumber;
                                actionButton.Text = "View Receipt";
                                actionButton.Visible = true;
                                break;
                            case 2:
                                //PENDING
                                tempDataItem["OrderDate"].Text = string.Empty;
                                orderStatus.ForeColor = Colors.PrimaryColor;
                                orderStatus.Text = "PENDING " + orderDate;
                                actionButton.Text = "View Invoice";
                                actionButton.Visible = true;
                                break;
                            case 3: // FAILED - DON'T DISPLAY 'RESUBMIT' BUTTON
                                tempDataItem["OrderDate"].ForeColor = System.Drawing.Color.Red;
                                orderStatus.ForeColor = System.Drawing.Color.Red;
                                orderStatus.Text = "FAILED";
                                actionButton.Text = "";
                                actionButton.Visible = false;
                                break;
                        }
                    }
                }
            }
        }

        //Used only for renewal
        Constants.DeluxePricingStructure getPricingStructure(string pricingStructure)
        {
            Constants.DeluxePricingStructure selectedDeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;

            switch (pricingStructure.ToLower().Trim())
            {
                case "deluxe":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
                    break;
                case "deluxeepcs":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcs;
                    break;
                case "legacy deluxeepcs":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.LegacyDeluxeEpcs;
                    break;
                case "deluxeepcslogrx":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsLogRx;
                    break;
                case "deluxeepa":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpa;
                    break;
                case "deluxeepalogrx":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpaLogRx;
                    break;
                case "deluxeepcsepa":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpa;
                    break;
                case "deluxeepcsepa2017":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpa2017;
                    break;
                case "deluxeepcsepalogrx":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx;
                    break;
                case "deluxeepcsepalogrx2017":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
                    break;
                case "legacy deluxeepcslogrx":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.LegacyDeluxeEpcsLogRx;
                    break;
                case "legacy deluxeepalogrx":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.LegacyDeluxeEpaLogRx;
                    break;
                case "compulsorybasic":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.CompulsoryBasic;
                    break;
                default:
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
                    break;
            }
            Session["DeluxePricingStructure"] = selectedDeluxePricingStructure;
            return selectedDeluxePricingStructure;
        }
        private void resubmit_transaction()
        {
            DataRow accountInfo = (DataRow)DeluxePurchaseManager.GetDeluxeAccountInfo(base.SessionLicenseID, base.DBID).Tables[0].Rows[0];

            string productName = accountInfo["ModuleSubscribed"].ToString().ToLower();
            Constants.DeluxePricingStructure ps = getPricingStructure(productName);

            bool chargeSetupFee = false;
            if (ps == Constants.DeluxePricingStructure.DeluxeEpcsEpa2017 || ps == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017)
            {
                chargeSetupFee = true;
            }

            DeluxePurchaseManager DeluxePurchaseManagerObj = new DeluxePurchaseManager(ConfigurationManager.AppSettings["PaymentGateWayUrl"].ToString(),
                                                                        ConfigurationManager.AppSettings["APIlogin"].ToString(),
                                                                        ConfigurationManager.AppSettings["TransactionKey"].ToString(),
                                                                        base.SessionLicenseID, string.Empty, accountInfo["CardNo"].ToString(), string.Empty,
                                                                         string.Empty, accountInfo["FirstName"].ToString(), accountInfo["LastName"].ToString(),
                                                                        accountInfo["Company"].ToString(), accountInfo["Address1"].ToString(),
                                                                        accountInfo["Address2"].ToString(),
                                                                        accountInfo["City"].ToString(), accountInfo["State"].ToString(),
                                                                        accountInfo["Zipcode"].ToString(),
                                                                        accountInfo["Email"].ToString(), accountInfo["PhoneNo"].ToString(),
                                                                        Convert.ToInt32(accountInfo["TotalDeluxeUsers"].ToString()),
                                                                        Convert.ToInt32(accountInfo["EpcsUsersCharged"].ToString()),
                                                                        Convert.ToDecimal(accountInfo["EpcsSetupFees"].ToString()),
                                                                        Convert.ToDecimal(accountInfo["TotalAmount"].ToString()),
                                                                        base.SessionUserID, ConfigurationManager.AppSettings["ValidationMode"].ToString(),
                                                                        ConfigurationManager.AppSettings["TransactionType"].ToString(),
                                                                        ConfigurationManager.AppSettings["EmailFrom"].ToString(),
                                                                        ConfigurationManager.AppSettings["EmailFromDisplayName"].ToString(),
                                                                        ConfigurationManager.AppSettings["EmailSubject"].ToString(),
                                                                        accountInfo["CustProfileID"].ToString(),
                                                                        accountInfo["PaymentProfileID"].ToString(),
                                                                        Session["CustAccountID"].ToString(),
                                                                        Session["LicenseName"].ToString(), base.DBID, Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE,
                                                                        ps, chargeSetupFee);

            int epcsUsers = Convert.ToInt32(accountInfo["EpcsUsersCharged"].ToString());
            decimal epcsSetupFees = Convert.ToDecimal(Convert.ToDecimal(accountInfo["EpcsSetupFees"]));
            decimal totalAmount = Convert.ToDecimal(accountInfo["TotalAmount"].ToString());

            string deluxeOrderResult = DeluxePurchaseManagerObj.CreateDeluxeTransaction(base.DBID);
            string[] deluxeOrderResultInfo = deluxeOrderResult.Split('~');
            lblLogout.Text = "";

            string deluxeSubscription = Session["CurrentDeluxeSubscription"].ToString();
            string deluxeSubscriptionDesc = getProductFullDescription(deluxeSubscription);

            string basicOrDeluxe = String.Empty;
            basicOrDeluxe = deluxeSubscription.ToLower().Contains("basic") ? "Basic" : "Deluxe";

            lblOrderDescription.Text = deluxeSubscriptionDesc + "(1 Month) - " + accountInfo["TotalDeluxeUsers"].ToString() + " Provider(s) ";
            decimal epcsSetupTotal;
            epcsSetupTotal = epcsUsers * epcsSetupFees;
            totalAmount += epcsSetupTotal;

            if (epcsUsers > 0)
            {
                lblOrderDescription.Text += " <br>" + EPCS_SETUP_FEE_TITLE + " - " + epcsUsers.ToString() + " Provider(s) ";
            }

            if (deluxeOrderResultInfo[0].ToString() == "APPROVED")
            {
                rcptOrderStatus.InnerText = "Congratulations! Your ePrescribe " + basicOrDeluxe + " account is now active.";
                rcptOrderInfo.InnerText = "Your receipt has been sent to " + accountInfo["Email"].ToString() + ".";
                lblOrderNumber.Text = " Order Number : " + deluxeOrderResultInfo[1].ToString().ToHTMLEncode();
                btPrint.Text = "Print Receipt";

                lblOrderCCNo.Text = accountInfo["CardNo"].ToString();
                // lblOrderCCExpDate.Text = accountInfo["ExpDate"].ToString();

                lblOrderAmount.Text = "$" + totalAmount.ToString("0.00");
                lblOrderDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
                pnlAccount.Style["Display"] = "none";
                pnlPurchaseReceipt.Style["Display"] = "inline";
                btnStayOnPage.Visible = false;
                btnPaidProviders.Visible = false;
                btnContinueLogOff.Visible = true;
                backButton.Visible = false;
                lblLogout.Text = "You must log out before ePrescribe™ " + basicOrDeluxe + " features are available.";
            }
            else
            {
                // transaction error 
                rcptOrderStatus.InnerText = "Unable to process the ORDER";
                rcptOrderInfo.InnerText = deluxeOrderResultInfo[0].ToString() + " " + deluxeOrderResultInfo[2].ToString();
                lblOrderNumber.Text = "Order Number : No Order is being generated";
                btPrint.Text = "Print Receipt";
                lblOrderCCNo.Text = accountInfo["CardNo"].ToString();
                // lblOrderCCExpDate.Text = accountInfo["ExpDate"].ToString();
                lblOrderAmount.Text = "$" + totalAmount.ToString("0.00");
                lblOrderDate.Text = DateTime.Today.ToString("MM/dd/yyyy");
                pnlAccount.Style["Display"] = "none";
                pnlPurchaseReceipt.Style["Display"] = "inline";
                btnStayOnPage.Visible = true;
                btnContinueLogOff.Visible = false;
                btnPaidProviders.Visible = false;
            }
        }

        private void disableEditFeatures()
        {
            ucMessage.Visible = true;
            ucMessage.MessageText = "Please contact your administrator to update billing information.";
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            updateLink.Visible = false;
            divDeluxeCancel.Visible = false;
            grdViewHistory.Columns[3].Visible = false;
        }


        protected void backButton_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["From"] != null)
            {
                // FORTIFY: Not considered an open re-direct as already redirecting to local page
                Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["From"].ToString()));
            }
            else
            {
                // FORTIFY: Not considered an open re-direct as already redirecting to local page
                Response.Redirect(Constants.PageNames.INTEGRATION_SOLUTIONS_LIST);
            }
        }

        protected void btnCancelSubscriptionConfirm_Click(object sender, EventArgs e)
        {
            DeluxePurchaseManager.CancelDeluxeLicense(base.SessionLicenseID, string.Empty, string.Empty, base.DBID);
            showCancelFeedback();
        }

        private void showCancelFeedback()
        {
            ucMessage.Visible = false;
            pnlAccount.Style["Display"] = "none";
            backButton.Style["Display"] = "none";
            pnlFeedbackCancel.Style["Display"] = "inline";
            pnlCancelled.Style["Display"] = "none";
            btnPaidProviders.Visible = false;
        }

        protected void btnCancelFeedbackContinue_Click(object sender, EventArgs e)
        {
            string cancelReasonCode = cancelReason.SelectedValue.Trim();
            string cancelComments = txtComments.Text;
            DeluxePurchaseManager.AddCancelDeluxeLicenseFeedback(base.SessionLicenseID, cancelReasonCode, cancelComments, base.DBID);
            showCancelled();
        }

        private void showCancelled()
        {
            ucMessage.Visible = false;
            //pnlDeluxePurchase.Style["Display"] = "none";
            //pnlDeluxePurchaseOrder.Style["Display"] = "none";
            //pnlPurchaseConfirmation.Style["Display"] = "none";
            pnlFeedbackCancel.Style["Display"] = "none";
            pnlCancelled.Style["Display"] = "inline";
            btnPaidProviders.Visible = false;
        }

        protected void btnStayOnPage_Click(object sender, EventArgs e)
        {
            grdViewHistory.Rebind();

            pnlPurchaseReceipt.Style["Display"] = "none";
            pnlAccount.Style["Display"] = "inline";
            backButton.Style["Display"] = "inline";

            btnPaidProviders.Visible = true;

            if (Session["CurrentDeluxeSubscription"] != null)
            {
                string productName = Session["CurrentDeluxeSubscription"].ToString().ToLower();
            }
        }
    }

}
