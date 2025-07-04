using System;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.ExtensionMethods;
using Telerik.Web.UI;
using eRxWeb.AppCode;
using eRxWeb.State;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb
{
    public partial class DeluxeBillingPage : BasePage
    {
        DeluxeFeatureStatus currentSubscriptionStatus = DeluxeFeatureStatus.Off;
        const string EPCS_SETUP_FEE_TITLE = " ePrescribe™ EPCS - Setup fee";

        public bool IsEpcsSelected { get {
                if (Session["DeluxePricingStructure"] != null)
                {
                    Constants.DeluxePricingStructure priceStructure = (Constants.DeluxePricingStructure)Session["DeluxePricingStructure"];
                    if (priceStructure == Constants.DeluxePricingStructure.DeluxeEpcs ||
                     priceStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpa ||
                    priceStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpa2017 ||
                    priceStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx ||
                    priceStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017 ||
                    priceStructure == Constants.DeluxePricingStructure.DeluxeEpcsLogRx)
                    { return true; }
                }
                    return false;
            } }
        private bool _updateMode
        {
            get
            {
                string pricingStructure = string.Empty;
                Constants.DeluxePurchaseType purchaseMode;
                string orderProductName = string.Empty;
                int orderProductCount = 0;

                PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, PageState);

                if (Request.QueryString["Mode"] != null && Request.QueryString["Mode"] == "Update")
                    return true;

                if (purchaseMode == Constants.DeluxePurchaseType.MODULE_EDIT)
                    return true;

                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string validationMode = ConfigurationManager.AppSettings["ValidationMode"].ToString();
                if (validationMode == "1")
                {
                    Session["PaymentGateWayTestMode"] = 0;
                }
                else
                {
                    Session["PaymentGateWayTestMode"] = 1;
                }

                if (_updateMode)
                {
                    grdViewCart.Columns[2].Visible = Convert.ToBoolean(Session["IsAdmin"]);
                    chkTermcond.Visible = false;
                    lnkTC.Visible = false;
                    cvCheckTermcond.Visible = false;
                    btCont.Text = "Save";
                }

                if (Request.Form["ProfileCreated"] == "True")
                {
                    string editProfileID = string.Empty;

                    if (Request.Form["editProfileID"] != null && !string.IsNullOrEmpty(Request.Form["editProfileID"]))
                    {
                        editProfileID = Request.Form["editProfileID"];
                    }

                    readCustomerPaymentProfile(editProfileID);
                }

                getPricingStructure();
                setPurchaseModuleHeaderText();
                loadPaymentInfo();

                if (!Convert.ToBoolean(Session["IsAdmin"]))
                {
                    disableEditFeatures();
                }

                Constants.DeluxePurchaseType purchaseMode;
                purchaseMode = getPurchaseMode();
                if (purchaseMode != Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE)
                {
                    checkProviderCount();
                }
            }
            else
            {
                clearWarningMsgLabel();
            }

            IStateContainer requestState = new StateContainer(System.Web.HttpContext.Current.Session);
            bool isAdmin = requestState.GetBooleanOrFalse(Constants.SessionVariables.IsAdmin);

            if (!isAdmin)
            {
                trBillingAddress.Visible = false;
                trBillingInfo.Visible = false;
                trCreditBillingnfo.Visible = false;
            }
        }
        private void checkProviderCount()
        {
            int userCount = Convert.ToInt32(Session["UserCount"]);
            if (userCount == 0)
            {
                ucMessage.MessageText = "Cannot proceed with the purchase - There are no active provider(s)";
                ucMessage.Icon = Controls_Message.MessageType.ERROR;
                ucMessage.Visible = true;
                btCancel.Enabled = true;
                btCont.Visible = false;
                ButtonAddCreditCard.Enabled = false;
                txtEmail.Enabled = false;
                txtEmailConfirm.Enabled = false;
            }


        }

        void getPricingStructure()
        {
            Constants.DeluxePricingStructure selectedDeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;

            string pricingStructure = string.Empty;
            Constants.DeluxePurchaseType purchaseMode;
            string orderProductName = string.Empty;
            int orderProductCount = 0;

            PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, PageState);

            switch (pricingStructure)
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
                case "deluxeepcsepalogrx":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx;
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
                case "deluxeepcsepa2017":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpa2017;
                    break;
                case "deluxeepcsepalogrx2017":
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017;
                    break;
                default:
                    selectedDeluxePricingStructure = Constants.DeluxePricingStructure.Deluxe;
                    break;
            }

            Session["DeluxePricingStructure"] = selectedDeluxePricingStructure;
            hdnSelectedPricingStructure.Value = selectedDeluxePricingStructure.ToString();
        }

        string getPurchaseModuleHeaderDescription()
        {
            Constants.DeluxePricingStructure selectedModule = (Constants.DeluxePricingStructure)Session["DeluxePricingStructure"];
            DataTable dt = DeluxePurchaseManager.GetDeluxeNoteAndDescription((int)selectedModule, null, base.DBID);

            string headerText = dt.Rows[0]["DetailDesc"].ToString();

            return headerText;
        }

        string getPurchaseModuleDescription()
        {
            Constants.DeluxePricingStructure selectedModule = (Constants.DeluxePricingStructure)Session["DeluxePricingStructure"];
            string headerText = "";
            switch (selectedModule)
            {
                case Constants.DeluxePricingStructure.Deluxe:
                    headerText = "Veradigm ePrescribe™ Deluxe";
                    break;
                case Constants.DeluxePricingStructure.DeluxeEpa:
                    headerText = "Veradigm ePrescribe™ Deluxe";
                    break;
                case Constants.DeluxePricingStructure.DeluxeEpaLogRx:
                case Constants.DeluxePricingStructure.LegacyDeluxeEpaLogRx:
                    headerText = "Veradigm ePrescribe™ Deluxe with Sponsored Messages";
                    break;
                case Constants.DeluxePricingStructure.DeluxeEpcs:
                case Constants.DeluxePricingStructure.LegacyDeluxeEpcs:
                    headerText = "Veradigm ePrescribe™ Deluxe with EPCS";
                    break;
                case Constants.DeluxePricingStructure.DeluxeEpcsLogRx:
                case Constants.DeluxePricingStructure.LegacyDeluxeEpcsLogRx:
                case Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017:
                    headerText = "Veradigm ePrescribe™ Deluxe with EPCS with Sponsored Messages";
                    break;
                case Constants.DeluxePricingStructure.DeluxeEpcsEpa:
                case Constants.DeluxePricingStructure.DeluxeEpcsEpa2017:
                    headerText = "Veradigm ePrescribe™ Deluxe and EPCS";
                    break;
                case Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx:
                    headerText = "Veradigm ePrescribe™ Deluxe and EPCS with Sponsored Messages";
                    break;
                case Constants.DeluxePricingStructure.CompulsoryBasic:
                    headerText = "Veradigm ePrescribe™ Basic";
                    break;
                default:
                    headerText = "Veradigm ePrescribe™ Deluxe";
                    break;
            }
            return headerText;
        }

        void setPurchaseModuleHeaderText()
        {
            spanHeader.InnerText = getPurchaseModuleHeaderDescription();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ((PhysicianMasterPageBlank)Master).hideTabs();
            ((PhysicianMasterPageBlank)Master).hideMessageQueue();
            ((PhysicianMasterPageBlank)Master).HideHelp();
        }

        private void setShoppingCartItems(string nextTransDate, bool chargeEpcsSetupFee)
        {
            string pricingStructure = string.Empty;
            Constants.DeluxePurchaseType purchaseMode;
            string orderProductName = string.Empty;
            int orderProductCount = 0;

            PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, PageState);

            DataSet dsPrice = Allscripts.Impact.DeluxePurchaseManager.GetDeluxePrice(base.SessionLicenseID,
                    base.DBID, pricingStructure, orderProductName, orderProductCount);

            var productPrice = new ProductPrice();
            DataTable dtModulePrice = dsPrice.Tables[0];
            DataTable dtTokenPrice = null;

            if (dsPrice.Tables.Count > 1)
                dtTokenPrice = dsPrice.Tables[1];

            DataTable dtCart = new DataTable();
            dtCart.Columns.Add("Item", typeof(String));
            dtCart.Columns.Add("Quantity", typeof(String));
            dtCart.Columns.Add("Amount", typeof(String));
            dtCart.Columns.Add("Note", typeof(String));
            dtCart.Columns.Add("ShowProviderList", typeof(bool));

            string note = string.Empty;

            decimal totalAmount = 0;                //Total amount (including currently not chargable in case of module change (not new purchase)
            decimal currentlyChargableAmount = 0;   //Charged this month
            decimal deluxeSubscriptionAmount = 0;
            decimal hardTokenAmount = 0;
            decimal epcsSetupAmount = 0;

            int userCount = 0;

            Session.Remove("EpcsUsersCharged");
            Session.Remove("EpcsSetupFees");
            if (dtModulePrice.Rows.Count > 0)
            {
                string item = dtModulePrice.Rows[0]["Module"].ToString();
                string moduleHeader = "";

                if (!string.IsNullOrEmpty(item))
                {
                    userCount = Convert.ToInt32(dtModulePrice.Rows[0]["UserCount"]);
                    decimal amount = Convert.ToDecimal(dtModulePrice.Rows[0]["Amount"]);
                    Session["ItemAmount"] = amount;
                    deluxeSubscriptionAmount = userCount * amount;

                    if (currentSubscriptionStatus == DeluxeFeatureStatus.Disabled && !_updateMode)
                        purchaseMode = DeluxePurchaseType.MODULE_NEW_PURCHASE;

                    if (purchaseMode == Constants.DeluxePurchaseType.MODULE_CHANGE_PURCHASE)
                    {
                        note = "Note: Monthly recurring fee of $" + amount.ToString("0.00") + " per provider will occur starting next billing cycle";
                        if (!string.IsNullOrEmpty(nextTransDate))
                            note = note + " on " + nextTransDate;
                        totalAmount = deluxeSubscriptionAmount;
                    }
                    else if ((purchaseMode == Constants.DeluxePurchaseType.MODULE_NEW_PURCHASE) || (purchaseMode == Constants.DeluxePurchaseType.MODULE_EDIT))
                    {
                        totalAmount = deluxeSubscriptionAmount;
                        currentlyChargableAmount = deluxeSubscriptionAmount;
                        panelCharges.Visible = false;
                        panelConfirmCharges.Visible = false;
                    }

                    DataTable dt = DeluxePurchaseManager.GetDeluxeNoteAndDescription(null, item.Trim().ToLower(), base.DBID);
                    moduleHeader = dt.Rows[0]["ShortDesc"].ToString();

                    if (!string.IsNullOrEmpty(moduleHeader))
                    {
                        if (purchaseMode != Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE)
                        {
                            dtCart.Rows.Add(moduleHeader, userCount, "$" + deluxeSubscriptionAmount.ToString("0.00"), note, true);
                        }
                    }

                    lbldesc.Text = moduleHeader;
                }

                if (chargeEpcsSetupFee)
                {
                    epcsSetupAmount = Convert.ToDecimal(productPrice.EpcsRegistration) * userCount;
                    if (purchaseMode != Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE)
                    {
                        dtCart.Rows.Add(EPCS_SETUP_FEE_TITLE, userCount, "$" + epcsSetupAmount.ToString("0.00"), "", false);

                        lblEpcsSetupCount.Text = userCount.ToString();
                        lblEpcsSetupDesc.Text = EPCS_SETUP_FEE_TITLE;
                        lblEpcsSetupAmount.Text = "$" + Convert.ToDecimal(productPrice.EpcsRegistration).ToString("0.00");
                        lblEpcsSetupTotal.Text = "$" + epcsSetupAmount.ToString("0.00");

                        currentlyChargableAmount += epcsSetupAmount;
                        totalAmount += epcsSetupAmount;
                        Session["EpcsUsersCharged"] = userCount;
                        Session["EpcsSetupFees"] = Convert.ToDecimal(productPrice.EpcsRegistration);
                    }
                }
            }
            Session.Remove("TokenCount");
            Session.Remove("TokenAmount");
            if ((dtTokenPrice != null) && (dtTokenPrice.Rows.Count > 0))
            {
                //Hard Token
                string item = dtTokenPrice.Rows[0]["ItemName"].ToString();
                orderProductCount = Convert.ToInt32(dtTokenPrice.Rows[0]["ItemCount"]);
                decimal amount = 0;

                //Charge only when epcs setup fee not charged
                amount = chargeEpcsSetupFee ? 0 : Convert.ToDecimal(dtTokenPrice.Rows[0]["Amount"]);
                hardTokenAmount = amount * orderProductCount;

                Session["TokenCount"] = orderProductCount;
                Session["TokenAmount"] = amount;

                totalAmount += hardTokenAmount;
                currentlyChargableAmount += hardTokenAmount;
            }

            if (purchaseMode == Constants.DeluxePurchaseType.PRODUCT_ONLY_PURCHASE)
            {
                userCount = 0;
                panelCharges.Visible = false;
                panelConfirmCharges.Visible = false;
            }

            Session["UserCount"] = userCount;
            Session["Amount"] = currentlyChargableAmount;
            Session["TotalAmount"] = totalAmount;

            lblCurBalance.Text = "Current balance as of " + DateTime.Today.ToString("MM/dd/yyyy");
            lblCurFees.Text = "$" + currentlyChargableAmount.ToString("0.00");
            lblRecBalance.Text = "Monthly recurring fees scheduled on " + nextTransDate;
            lblRecFees.Text = "$" + deluxeSubscriptionAmount.ToString("0.00");

            lblConfirmCurBalance.Text = "Current balance as of " + DateTime.Today.ToString("MM/dd/yyyy");
            lblConfirmCurFees.Text = "$" + currentlyChargableAmount.ToString("0.00");
            lblConfirmRecBalance.Text = "Monthly recurring fees scheduled on " + nextTransDate;
            lblConfirmRecFees.Text = "$" + deluxeSubscriptionAmount.ToString("0.00");

            grdViewCart.DataSource = dtCart;
            grdViewCart.DataBind();
        }

        private void loadPaymentInfo()
        {
            string billingText = string.Empty;
            string creditCardText = string.Empty;
            string paymentProfileID = string.Empty;
            string billingFirstName = string.Empty;
            string profileEmail = string.Empty;


            DataRow accountInfo = (DataRow)DeluxePurchaseManager.GetDeluxeAccountInfo(base.SessionLicenseID, base.DBID).Tables[0].Rows[0];

            string productName = string.Empty;
            bool chargeEpcsSetupFees = false;

            Session["ChargeEpcsSetupFees"] = false;
            if (accountInfo["ModuleSubscribed"] != DBNull.Value)
            {
                productName = accountInfo["ModuleSubscribed"].ToString().ToLower();
            }
                
            if (accountInfo["DeluxeFeatureStatus"] != DBNull.Value)
            {
                currentSubscriptionStatus = (DeluxeFeatureStatus) accountInfo["DeluxeFeatureStatus"];
            }


            chargeEpcsSetupFees = DeluxeAccountUtil.IsEpcsSetupFeesRequired(productName, Session["DeluxePricingStructure"] != null ? Session["DeluxePricingStructure"].ToString() : string.Empty);
            Session["ChargeEpcsSetupFees"] = chargeEpcsSetupFees;

            if (!string.IsNullOrEmpty(productName))
                Session["CurrentDeluxeSubscription"] = productName;


            string nextTransDate = string.Empty;

            if (accountInfo["TranNextDate"] != DBNull.Value)
                nextTransDate = Convert.ToDateTime(accountInfo["TranNextDate"]).ToString("MM/dd/yyyy");

            if (!string.IsNullOrEmpty(nextTransDate))
                Session["DeluxeNextBillingDate"] = nextTransDate;

            setShoppingCartItems(nextTransDate, chargeEpcsSetupFees);

            if (!string.IsNullOrEmpty(accountInfo["Email"].ToString()))
            {
                if (Session["ProfileEmail"] == null)
                {
                    Session["ProfileEmail"] = accountInfo["Email"].ToString();
                }
                profileEmail = accountInfo["Email"].ToString();
            }

            billingFirstName = accountInfo["FirstName"].ToString();

            if (!string.IsNullOrEmpty(accountInfo["Email"].ToString()))
            {
                string billingTextTemp = accountInfo["FirstName"].ToString() + " " + accountInfo["LastName"].ToString() + "<br />";
                billingTextTemp += accountInfo["Address1"].ToString();
                if (accountInfo["Address1"] != DBNull.Value && !string.IsNullOrEmpty(accountInfo["Address1"].ToString()))
                {
                    billingTextTemp += "<br />" + accountInfo["Address2"].ToString();
                }
                if (accountInfo["Address2"] != DBNull.Value && !string.IsNullOrEmpty(accountInfo["Address2"].ToString()))
                {
                    billingTextTemp += "<br />";
                }
                billingTextTemp += accountInfo["City"].ToString() + ", " + accountInfo["State"].ToString() + " "
                                        + accountInfo["Zipcode"].ToString() + "<br />";
                billingText = billingTextTemp;

                creditCardText = "ending in " + accountInfo["CardNo"].ToString();
            }

            if (!string.IsNullOrEmpty(accountInfo["CustProfileId"].ToString()))
            {
                string CustProfileId = accountInfo["CustProfileId"].ToString();
                Session["CustProfileId"] = CustProfileId;
            }
            else
                Session["CustProfileId"] = "";

            bool doesPaymentProfileExist = false;
            if (!string.IsNullOrEmpty(accountInfo["PaymentProfileID"].ToString()))
            {
                Session["CardFourDigits"] = accountInfo["CardNo"].ToString().Substring(accountInfo["CardNo"].ToString().Length - 4, 4);

                paymentProfileID = accountInfo["PaymentProfileID"].ToString();
                Session["paymentProfileID"] = paymentProfileID;
                DeluxePurchaseManager deluxePurchageManagerObj = getDeluxePurchaseManager();
                doesPaymentProfileExist = deluxePurchageManagerObj.CheckIfPaymentProfileExists(Convert.ToInt64(accountInfo["CustProfileId"].ToString()), Convert.ToInt64(paymentProfileID));
            }
            if (!doesPaymentProfileExist)  //If payment profile not found
            {
                Session["CardFourDigits"] = "";
                Session["paymentProfileID"] = "";
            }

            divShippingInfo.Visible = false;
            tblShipping.Visible = false;

            if (Session["epcsShippingAddress"] != null)
            {
                divShippingInfo.Visible = true;
                tblShipping.Visible = true;
                litShippingAddress.Text = Session["epcsShippingAddress"].ToString();
            }

            if (!string.IsNullOrEmpty(profileEmail))
            {
                txtEmail.Text = Session["ProfileEmail"].ToString();
                litBilling.Text = billingText;

                if (string.IsNullOrEmpty(creditCardText))
                {
                    litCreditCard.Visible = false;
                }
                else
                {
                    litCreditCard.Visible = true;
                    litCreditCard.Text = "Credit card :";
                    litCreditCardNumber.Text = creditCardText;
                }
            }

            if (String.IsNullOrEmpty(paymentProfileID))
            {
                ButtonAddCreditCard.Text = "Add Payment";
                btCont.Visible = false;
            }
            else if (String.IsNullOrEmpty(billingFirstName))
            {
                ButtonAddCreditCard.Text = "Edit Payment";
                btCont.Visible = false;
            }
            else
            {
                ButtonAddCreditCard.Text = "Edit Payment";
                btCont.Visible = true;
            }

            if (Session["chkTermcondValue"] != null)
                chkTermcond.Checked = (bool)Session["chkTermcondValue"];
            if (Session["txtEmailValue"] != null)
                txtEmail.Text = (string)Session["txtEmailValue"];
            if (Session["txtEmailConfirm"] != null)
                txtEmailConfirm.Text = (string)Session["txtEmailConfirm"];
        }

        private void clearWarningMsgLabel()
        {
            lblWarningMsg.Visible = false;
            lblWarningMsg.ForeColor = System.Drawing.Color.Red;
            lblWarningMsg.Text = "";
        }

        protected void btCont_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Session["ProfileEmail"] != null)
                {
                    string ProfileEmail = Session["ProfileEmail"].ToString();
                    if (txtEmail.Text != ProfileEmail)
                    {
                        saveProfileEMail(txtEmail.Text);
                        Session.Remove("ProfileEmail");
                    }
                }

                clearTempSessionValues();

                string customerProfileResults = string.Empty;

                if (Session["customerProfileResults"] != null)
                {
                    customerProfileResults = Session["customerProfileResults"].ToString();
                }

                if ((customerProfileResults == "APPROVED" && !_updateMode) || (!_updateMode && Session["paymentProfileID"] != null))
                {
                    this.showOrderConfirmation();
                }
                else if (customerProfileResults == "APPROVED" || _updateMode)
                {
                    string redirect_url = string.Empty;
                    string msg = string.Empty;
                    //Login _login = new Login(this);
                    //_login.ReLogUserIn(base.SessionUserID, ref redirect_url, ref msg);
                    Response.Redirect("~/" + Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT);
                }
                else
                {
                    Session.Remove("DeluxePurchaseManager");
                    this.showBillingPnl(customerProfileResults);
                }
            }
        }

        private void saveProfileEMail(string email)
        {
            DeluxePurchaseManager deluxePurchageManagerObj = getDeluxePurchaseManager();
            deluxePurchageManagerObj.UpdateProfileEMail(base.DBID, email);
        }

        private void showBillingPnl(string orderResult)
        {
            pnlDeluxePurchase.Visible = true;
            pnlDeluxePurchaseOrder.Visible = false;
            pnlPurchaseConfirmation.Visible = false;
            lblWarningMsg.Visible = true;
            lblWarningMsg.Text = orderResult.ToHTMLEncode();
            pnlDeluxePurchase.Style["Display"] = "inline";
            pnlDeluxePurchaseOrder.Style["Display"] = "none";
            pnlPurchaseConfirmation.Style["Display"] = "none";

            if (!Convert.ToBoolean(Session["IsAdmin"]))
            {
                disableEditFeatures();
            }
            else if (ApplicationLicense.IsForcedEnterpriseLicenseInForcedState(SessionLicense.EnterpriseDeluxeFeatureStatus, SessionLicense.LicenseDeluxeStatus))
            {
                DataTable dt = DeluxePurchaseManager.GetDeluxeAccountInfo(base.SessionLicenseID, base.DBID).Tables[0];
                if (dt.Rows.Count <= 0 || dt.Rows[0]["PaymentProfileID"] == DBNull.Value || dt.Rows[0]["PaidUserCount"] == DBNull.Value)
                {
                    ucMessage.Visible = true;
                    ucMessage.MessageText = "You must purchase a license in order to use the application.";
                    ucMessage.Icon = Controls_Message.MessageType.ERROR;
                }
            }
        }

        private void showOrderConfirmation()
        {
            ucMessage.Visible = false;
            pnlDeluxePurchase.Visible = false;
            pnlDeluxePurchaseOrder.Visible = false;
            pnlPurchaseConfirmation.Visible = true;
            lblWarningMsg.Visible = false;
            pnlDeluxePurchase.Style["display"] = "none";
            pnlDeluxePurchaseOrder.Style["display"] = "none";
            pnlPurchaseConfirmation.Style["display"] = "inline";
            lblCClastfourdigits.Text = Session["CardFourDigits"].ToString();

            int userCount = Convert.ToInt32(Session["UserCount"]);
            lblUserCount.Text = userCount.ToString();
            Decimal itemAmount = Convert.ToDecimal(Session["ItemAmount"]);
            lblItemAmount.Text = "$" + itemAmount.ToString("0.00");
            Decimal itemTotal = itemAmount * userCount;
            lblItemTotal.Text = "$" + itemTotal.ToString("0.00");

            Decimal amount = Convert.ToDecimal(Session["TotalAmount"]);
            lblamount.Text = "$" + amount.ToString("0.00");

            if (userCount <= 0)
                rowItems.Style.Add("display", "none");
        }

        private void showOrderResult(string transactionID)
        {
            ucMessage.Visible = false;
            pnlDeluxePurchase.Visible = false;
            pnlDeluxePurchaseOrder.Visible = true;
            pnlPurchaseConfirmation.Visible = false;
            lblWarningMsg.Visible = false;
            pnlDeluxePurchase.Style["Display"] = "none";
            pnlDeluxePurchaseOrder.Style["Display"] = "inline";
            pnlPurchaseConfirmation.Style["Display"] = "none";
            lbldate.Text = DateTime.Today.ToString("MM/dd/yyyy");

            if (string.IsNullOrEmpty(transactionID))
                lbOrderno.Text = "N/A";
            else
                lbOrderno.Text = transactionID.ToHTMLEncode();

            Decimal amount = Convert.ToDecimal(Session["Amount"]);
            lblOrderamount.Text = amount.ToString("0.00");
            lblOrderClastfour.Text = Session["CardFourDigits"].ToString();

            if (Session["DeluxeNextBillingDate"] != null)
            {
                lblnextOrdderdate.Text = Session["DeluxeNextBillingDate"].ToString();
            }
            else
                lblnextOrdderdate.Text = DateTime.Today.AddMonths(1).ToString("MM/dd/yyyy");

            lblemailID.Text = "Your receipt has been sent to " + txtEmail.Text.Trim() + '.';

            int userCount = Convert.ToInt32(Session["UserCount"]);
            int tokenCount = Convert.ToInt32(Session["TokenCount"]);

            if (userCount <= 0)
            {
                rowPurchaseOrderStatus.Style.Add("display", "none");
                rowPurchaseOrderLogout.Style.Add("display", "none");
                btnContinue.Visible = true;
                btnContinueLogOff.Visible = false;
            }
            else
            {
                litPurchaseOrderDesc.Text = userCount.ToString() + " " + getPurchaseModuleDescription() + " User Licenses (1 Month)";
                rowPurchaseTokenOrderStatus.Style.Add("display", "none");
            }

            string pricingStructure = string.Empty;
            Constants.DeluxePurchaseType purchaseMode;
            string orderProductName = string.Empty;
            int orderProductCount = 0;

            PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, PageState);

            bool isEpcsSetupFeeCharged = (Session["ChargeEpcsSetupFees"] != null && (bool)Session["ChargeEpcsSetupFees"] == true);

            if (isEpcsSetupFeeCharged)
            {
                litPurchaseOrderDesc.Text = litPurchaseOrderDesc.Text + "<BR><span style=\" padding-left:75px;\"></span>";
                litPurchaseOrderDesc.Text = $"{litPurchaseOrderDesc.Text}{userCount.ToString()} {EPCS_SETUP_FEE_TITLE}"; 
            }
            if (String.Compare(pricingStructure, "compulsorybasic", true) == 0)
            {
                spnDeluxeOrBasicActive.InnerHtml = spnDeluxeOrBasicAvailable.InnerHtml = "Basic";
            }
            if (IsEpcsSelected)
            {
                trEPCCS.Visible = true;
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            Session.Remove("OrderProductName");
            Response.Redirect("~/" + Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT);
        }

        private void clearTempSessionValues()
        {
            Session["chkTermcondValue"] = null;
            Session["txtEmailValue"] = null;
            Session["txtEmailConfirm"] = null;
        }

        protected void btCancel_Click(object sender, EventArgs e)
        {
            clearTempSessionValues();
            Session.Remove("DeluxePurchaseManager");

            //Bug 847175 : Application gets logging out when we Clicking on Cancel Button in new Pricing billing page For Platinum Var Force User.
            //if (base.SessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Force)
            //{
            //    string redirect_url = string.Empty;

            //    Login.LogUserOut(this.Page, ref redirect_url);

            //    Response.Redirect(Constants.PageNames.LOGIN);
            //}
            if (Request.QueryString["From"] != null)
            {
                if (Request.QueryString["From"].Contains(".aspx"))
                {
                    Response.Redirect(Constants.PageNames.UrlForRedirection(Request.QueryString["From"].ToString()));
                }      
                else
                {
                    // FORTIFY: Not considered an open re-direct as already redirecting to local page
                    Response.Redirect(Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Request.QueryString["From"].ToString());
                }  
            }
            else
            {
                // FORTIFY: Not considered an open re-direct as already redirecting to local page
                Response.Redirect(Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT);
            }
        }

        protected void btConfirmCancel_Click(object sender, EventArgs e)
        {
            showBillingPnl(string.Empty);
        }

        protected void btContinue_Click(object sender, EventArgs e)
        {
            base.ResetModules();
            string redirect_url = string.Empty;
            string msg = string.Empty;
            Login _login = new Login(this);
            _login.ReLogUserIn(base.SessionUserID, ref redirect_url, ref msg);

            if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Allscripts.ePrescribe.Common.Constants.SSOMode.TASKMODE)
            {
                if (Convert.ToBoolean(Session["IsProvider"]) || Convert.ToBoolean(Session["IsPA"]))
                    Response.Redirect("~/" + Constants.PageNames.DOC_REFILL_MENU);
                else if ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.GENERAL_USER ||
                                ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.POB_LIMITED))
                    Response.Redirect(((PhysicianMasterPageBlank)Master).ReportsLinkURL);
                else
                    Response.Redirect("~/" + Constants.PageNames.LIST_SEND_SCRIPTS);
            }
            else if (Session["SSOMode"] != null && Session["SSOMode"].ToString() == Allscripts.ePrescribe.Common.Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE)
            {
                if (Convert.ToBoolean(Session["IsProvider"]) || Convert.ToBoolean(Session["IsPA"]))
                    Response.Redirect("~/" + Constants.PageNames.DOC_REFILL_MENU);
                else if ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.GENERAL_USER ||
                                ((Constants.UserCategory)Session["UserType"] == Constants.UserCategory.POB_LIMITED))
                    Response.Redirect(((PhysicianMasterPageBlank)Master).ReportsLinkURL);
                else
                    Response.Redirect("~/" + Constants.PageNames.LIST_SEND_SCRIPTS);
            }
            else
            {
                //Default Redirect handles correctly
                if (redirect_url.ToLower().Contains(Constants.PageNames.SELECT_PATIENT))
                {
                    DefaultRedirect();
                }
                else
                {
                    Response.Redirect(redirect_url);
                }
            }
        }

        protected void btProcessOrder_Click(object sender, EventArgs e)
        {
            DeluxePurchaseManager deluxePurchageManagerObj = getDeluxePurchaseManager();
            string deluxeOrderResult = deluxePurchageManagerObj.CreateDeluxeTransaction(base.DBID);
            string[] deluxeOrderResultInfo = deluxeOrderResult.Split('~');

            if (deluxeOrderResultInfo[0].ToString() == "APPROVED")
            {
                Constants.DeluxePurchaseType purchaseMode;
                bool needEnrollment = false;
                purchaseMode = getPurchaseMode();
                needEnrollment = EnrollmentChangesChecker.AnyEnrollmentChangesOccured(purchaseMode, PageState);

                if (needEnrollment)
                {
                    EPSBroker.InsertAllProvidersEnrollmentInLicense(base.SessionLicenseID, 0, base.DBID);
                }

                this.showOrderResult(deluxeOrderResultInfo[1].ToString());
                // log the Terms & Condition accepted 
                ePrescribeSvc.ePrescribeSvc eps = new ePrescribeSvc.ePrescribeSvc();
                eps.Url = ConfigurationManager.AppSettings["ePrescribeSvc.eprescribesvc"].ToString();
                string TERMS = eps.GetEULA();
                Allscripts.ePrescribe.Data.User.SaveTermsNConditions(base.SessionUserID, TERMS, "Y", base.DBID);
            }
            else
            {
                // transaction error          
                this.showBillingPnl(deluxeOrderResultInfo[0].ToString() + ' ' + deluxeOrderResultInfo[2].ToString());
            }

            Session.Remove("DeluxePurchaseManager");
            //    mpeProcess.Hide();
        }

        protected void grdViewCart_ItemDataBound(object sender, GridItemEventArgs e)
        {
            string pricingStructure = string.Empty;
            Constants.DeluxePurchaseType purchaseMode;
            string orderProductName = string.Empty;
            int orderProductCount = 0;

            PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, PageState);

            if (e.Item is GridDataItem)
            {
                if (Convert.ToBoolean(((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray[4]))
                    ((GridDataItem)e.Item).FindControl("lnkProviders").Visible = true;
                else
                    ((GridDataItem)e.Item).FindControl("lnkProviders").Visible = false;

            }
            else if (e.Item is GridFooterItem)
            {
                GridFooterItem footerItem = (GridFooterItem)e.Item;

                Constants.DeluxePricingStructure selectedModule = (Constants.DeluxePricingStructure)Session["DeluxePricingStructure"];
                if (selectedModule == Constants.DeluxePricingStructure.DeluxeEpcs
                    || selectedModule == Constants.DeluxePricingStructure.LegacyDeluxeEpcs
                    || selectedModule == Constants.DeluxePricingStructure.DeluxeEpcsLogRx
                    || selectedModule == Constants.DeluxePricingStructure.DeluxeEpcsEpa
                    || selectedModule == Constants.DeluxePricingStructure.DeluxeEpcsEpa2017
                    || selectedModule == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx
                    || selectedModule == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017)
                {
                    ((GridFooterItem)e.Item).FindControl("lnkFAQ").Visible = true;
                }
                else
                {
                    ((GridFooterItem)e.Item).FindControl("lnkFAQ").Visible = false;
                }

                Decimal amount = Convert.ToDecimal(Session["TotalAmount"]);
                footerItem["Amount"].Text = "$" + amount.ToString("0.00");
            }
        }

        private void disableEditFeatures()
        {
            ucMessage.Visible = true;
            if (Session["DeluxePricingStructure"] != null &&
                   String.Compare(((Constants.DeluxePricingStructure)Session["DeluxePricingStructure"]).ToString(), "CompulsoryBasic", true) == 0)
            {
                ucMessage.MessageText = "Please contact your administrator to purchase ePrescribe.";
            }
            else
            {
                ucMessage.MessageText = "Please contact your administrator to purchase ePrescribe Deluxe.";
            }
            ucMessage.Icon = Controls_Message.MessageType.ERROR;
            btCancel.Enabled = true;
            btCont.Enabled = false;
            ButtonAddCreditCard.Enabled = false;

            txtEmail.Enabled = false;
            txtEmailConfirm.Enabled = false;
        }

        protected void cvCurrentDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            args.IsValid = true;
        }

        Constants.DeluxePurchaseType getPurchaseMode()
        {
            string pricingStructure = string.Empty;
            Constants.DeluxePurchaseType purchaseMode;
            string orderProductName = string.Empty;
            int orderProductCount = 0;

            PurchaseModeRetriever.GetPurchaseMode(out pricingStructure, out purchaseMode, out orderProductName, out orderProductCount, PageState);

            return purchaseMode;
        }

        private DeluxePurchaseManager getDeluxePurchaseManager()
        {
            int userCount = Convert.ToInt32(Session["UserCount"]);
            
            int epcsUsersCharged = Convert.ToInt32(Session["EpcsUsersCharged"] ?? 0);
            decimal epcsSetupFees = Convert.ToDecimal(Session["EpcsSetupFees"] ?? 0);
            decimal amount = Convert.ToDecimal(Session["Amount"]);
            Constants.DeluxePricingStructure selectedModule = (Constants.DeluxePricingStructure)Session["DeluxePricingStructure"];
            DeluxePurchaseManager deluxePurchageManagerObj = null;
            string email = txtEmail.Text.Trim();

            if (Session["DeluxePurchaseManager"] == null)
            {
                deluxePurchageManagerObj = new DeluxePurchaseManager(ConfigurationManager.AppSettings["PaymentGateWayUrl"].ToString(),
                                                                                    ConfigurationManager.AppSettings["APIlogin"].ToString(),
                                                                                    ConfigurationManager.AppSettings["TransactionKey"].ToString(),
                                                                                    base.SessionLicenseID, "", Session["CardFourDigits"].ToString(), "",
                                                                                    "", "", "", "", "", "",
                                                                                    "", "", "", email, "", userCount,epcsUsersCharged, epcsSetupFees,
                                                                                    amount, base.SessionUserID,
                                                                                    ConfigurationManager.AppSettings["ValidationMode"].ToString(),
                                                                                    ConfigurationManager.AppSettings["TransactionType"].ToString(),
                                                                                    ConfigurationManager.AppSettings["EmailFrom"].ToString(),
                                                                                    ConfigurationManager.AppSettings["EmailFromDisplayName"].ToString(),
                                                                                    ConfigurationManager.AppSettings["EmailSubject"].ToString(),
                                                                                    Session["CustProfileId"].ToString(),
                                                                                    Session["paymentProfileID"].ToString(),
                                                                                    Session["CustAccountID"].ToString(),
                                                                                    Session["LicenseName"].ToString(),
                                                                                    base.DBID, getPurchaseMode(),
                                                                                    selectedModule);
                Session["DeluxePurchaseManager"] = deluxePurchageManagerObj;
            }
            else
            {
                deluxePurchageManagerObj = (DeluxePurchaseManager)Session["DeluxePurchaseManager"];
                deluxePurchageManagerObj.SetPrice(userCount,amount, email, epcsUsersCharged, epcsSetupFees, getPurchaseMode());
            }
           
            return deluxePurchageManagerObj;
        }

        protected void ButtonAddCreditCard_Click(object sender, EventArgs e)
        {
            Session["chkTermcondValue"] = chkTermcond.Checked;
            Session["txtEmailValue"] = txtEmail.Text;
            Session["txtEmailConfirm"] = txtEmailConfirm.Text;

            string returnCode = launchCIMPopUP();

            if (returnCode == "CREATED")
            {
                lblWarningMsg.Visible = false;
                lblWarningMsg.ForeColor = System.Drawing.Color.Green;
                lblWarningMsg.Text = "";
            }
            else
            {
                lblWarningMsg.ForeColor = System.Drawing.Color.Red;
                lblWarningMsg.Visible = true;
                lblWarningMsg.Text = "We apologize, but a serious error has occurred - Error Reference ID = " + returnCode.ToHTMLEncode();
            }
        }

        private void readCustomerPaymentProfile(string editProfileID)
        {

            if (Session["chkTermcondValue"] != null)
                chkTermcond.Checked = (bool)Session["chkTermcondValue"];
            if (Session["txtEmailValue"] != null)
                txtEmail.Text = (string)Session["txtEmailValue"];
            if (Session["txtEmailConfirm"] != null)
                txtEmailConfirm.Text = (string)Session["txtEmailConfirm"];

            string returnCode = getCustomerPaymentProfile(editProfileID);

            if (String.IsNullOrEmpty(Session["paymentProfileID"].ToString()))
                ButtonAddCreditCard.Text = "Add Payment";
            else
                ButtonAddCreditCard.Text = "Edit Payment";

            if (returnCode == "APPROVED")
            {
                btCont.Visible = true;
                ButtonAddCreditCard.Visible = true;
                lblWarningMsg.Visible = false;

                lblWarningMsg.Visible = true;
                lblWarningMsg.ForeColor = System.Drawing.Color.Green;

                var context = HttpContext.Current;
                Task.Run(() =>
                {
                    HttpContext.Current = context;
                    CreditCard.UpdateExpiredCreditCardForLicense(SessionLicenseID.ToGuidOr0x0(), DBID);
                });

                if (_updateMode)
                    lblWarningMsg.Text = "Your payment information is successfully saved.";
                else
                    lblWarningMsg.Text = "Your payment information is successfully saved. Click on 'Process Payment' to continue.";
            }
            else if (returnCode == "NOPAYMENTINFO")
            {
                // user has closed the auth.net overlay without entering the credit card info
                // No need to display error message
            }
            else
            {
                lblWarningMsg.ForeColor = System.Drawing.Color.Red;
                lblWarningMsg.Visible = true;
                lblWarningMsg.Text = "We apologize, but a serious error has occurred - Error Reference ID = " + returnCode;
            }
        }

        private string launchCIMPopUP()
        {
            string customerProfileResults = string.Empty;
            string customerProfileID = string.Empty;
            string token = string.Empty;

            Session["editProfileID"] = Session["paymentProfileID"];

            if ((Session["CustProfileId"] != null) && (!string.IsNullOrEmpty(Session["CustProfileId"].ToString())))
                customerProfileID = Session["CustProfileId"].ToString();

            DeluxePurchaseManager deluxePurchageManagerObj = getDeluxePurchaseManager();
            customerProfileResults = deluxePurchageManagerObj.GetCustomerProfileToken(base.DBID, ref customerProfileID, out token);

            if ((customerProfileResults == "CREATED") && (!string.IsNullOrEmpty(customerProfileID)))
            {
                Session["CustProfileId"] = customerProfileID;
                Session["Token"] = token;

                Control myUserControl = (Control)Page.LoadControl("~/Controls/AuthNetCCFrame.ascx");
                Page.Controls.Add(myUserControl);
                return "CREATED";
            }
            else
            {
                return customerProfileResults;
            }
        }

        private string getCustomerPaymentProfile(string editProfileID)
        {
            DeluxePurchaseManager deluxePurchageManagerObj = getDeluxePurchaseManager();

            string paymentProfileID;
            string returnCode = deluxePurchageManagerObj.ReadCustomerPaymentProfile(base.DBID, editProfileID, out paymentProfileID, base.SessionLicenseID);

            Session["customerProfileResults"] = returnCode;
            Session["paymentProfileID"] = paymentProfileID;

            if (returnCode == "APPROVED")
            {
                Session["CardFourDigits"] = deluxePurchageManagerObj.CardNumber;
            }

            return returnCode;
        }
    }
}
