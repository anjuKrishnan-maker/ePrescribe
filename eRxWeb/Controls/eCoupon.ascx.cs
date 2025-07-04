using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;

namespace eRxWeb
{
    public partial class Controls_eCoupon : BaseControl
    {
        public Visibility EcouponWarningOverlay { get; set; }

        public string CouponOfferImageURL
        {
            get { return lbViewOffer.Attributes["OfferImageURL"].ToString(); }
        }

        public bool ECouponPrintOptionChecked
        {
            get { return chkPrintOffers.Checked; }

        }

        public bool ECouponPharmacyNotesChecked
        {
            get { return chkOffersPharmacyNotes.Checked; }
            set { chkOffersPharmacyNotes.Checked = value; }
        }

        public bool ECouponPharmacyNotesEnabled
        {
            get { return chkOffersPharmacyNotes.Enabled; }
            set { chkOffersPharmacyNotes.Enabled = value; }
        }

        public string ECouponDetailID
        {
            get { return this.Attributes["ECouponDetailID"].ToString(); }
        }

        public string PharmacyNotes => this.Attributes["PharmacyNotes"]?.ToString();

        public bool HasECoupon
        {
            get
            {
                if (lbViewOffer != null && lbViewOffer.Attributes["OfferImageURL"] != null)
                {
                    return !string.IsNullOrWhiteSpace(lbViewOffer.Attributes["OfferImageURL"].ToString());
                }
                else
                {
                    string rxID = this.Attributes["RxID"];
                    //Explicit eCoupon Details for ScriptPad grid
                    var eCouponDetailByRxIdResponse = EPSBroker.GetECouponDetailByRxID(rxID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                    return !string.IsNullOrWhiteSpace(eCouponDetailByRxIdResponse.ECouponDetail?.OfferURL);
                }
            }
        }
        public string CheckBoxGoodRxClientId
        {
            get; set;
        }
        public string CheckBoxECouponClientId
        {
            get; set;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (base.CanApplyFinancialOffers)
            {
                EcouponWarningOverlay = ControlState.Cast(Constants.SessionVariables.EcouponUncheckedWarningVisibility, Visibility.Visible);

                string rxID = this.Attributes["RxID"];

                // check for ecoupon list existing
                var eCouponDetailByRxIDResponse = EPSBroker.GetECouponDetailByRxID(rxID, base.SessionLicenseID, base.SessionUserID, base.DBID);
                if (eCouponDetailByRxIDResponse.ECouponDetail != null)
                {
                    var eCouponsList = ControlState.Cast(Constants.SessionVariables.ECouponUrlWhiteList, new List<String>());
                    eCouponsList.Add(eCouponDetailByRxIDResponse.ECouponDetail.OfferURL.ToUrlEncode());
                    ControlState[Constants.SessionVariables.ECouponUrlWhiteList] = eCouponsList;

                    this.Attributes.Add("ECouponDetailID", eCouponDetailByRxIDResponse.ECouponDetail.ECouponDetailID.ToString());
                    this.Attributes.Add("PharmacyNotes", eCouponDetailByRxIDResponse.ECouponDetail.PharmacyNotes.ToString());
                    divApplyCoupons.Visible = true;
                    lbViewOffer.Text = "eCoupon - " + eCouponDetailByRxIDResponse.ECouponDetail.OfferName.ToHTMLEncode();
                    lbViewOffer.Attributes.Add("OfferImageURL", eCouponDetailByRxIDResponse.ECouponDetail.OfferImageURL);
                    lbViewOffer.Attributes.Add("OfferURL", eCouponDetailByRxIDResponse.ECouponDetail.OfferURL);
                    lbViewOffer.OnClientClick = $"return ShowAngularPdfOverlay('{eCouponDetailByRxIDResponse.ECouponDetail.OfferURL.ToUrlEncode()}', 'eCoupon');";
                    if (!string.IsNullOrEmpty(eCouponDetailByRxIDResponse.ECouponDetail.PrescribingURL))
                    {
                        lbPrescribingInfo.Attributes.Add("PrescribingInfoUrl", eCouponDetailByRxIDResponse.ECouponDetail.PrescribingURL);
                    }
                    else
                    {
                        lbPrescribingInfo.Enabled = false;
                        lbPrescribingInfo.ToolTip = "No prescribing info available.";
                    }

                    setControlSize();
                }
            }
        }
        private void setControlSize()
        {
            int pageHeight = 500;
            int pageWidth = 600;

            if (Session["PAGEHEIGHT"] != null && Session["PAGEWIDTH"] != null)
            {
                Int32.TryParse(Session["PAGEHEIGHT"].ToString(), out pageHeight);
                Int32.TryParse(Session["PAGEWIDTH"].ToString(), out pageWidth);
            }
        }
        protected void lbViewOffer_Click(object sender, EventArgs e)
        {
            // Session["OpenURLInFrame"] = lbViewOffer.Attributes["OfferURL"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "ShowPdfOverlay", $"ShowAngularPdfOverlay('{lbViewOffer.Attributes["OfferURL"].ToString()}', 'eCoupon');", true);
        }

        protected void lbPrescribingInfo_Click(object sender, EventArgs e)
        {
            Session["OpenURLInFrame"] = lbPrescribingInfo.Attributes["PrescribingInfoUrl"].ToString();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenUrlInModal", $"OpenUrlInModal('{Constants.PageNames.OPEN_URL_IN_FRAME}', 'eCoupon - Prescribing Information');", true);
        }
    }
}