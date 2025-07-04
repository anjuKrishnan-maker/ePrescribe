using System;
using System.Web.UI;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.PptPlusBPL;

namespace eRxWeb
{
    public partial class Controls_PptPlusCoupon : BaseControl
    {
        public bool CouponPrintOptionChecked
        {
            get { return chkPrintGoodRxOffers.Checked; }
        }
        public bool SendOfferToPharmacyChecked
        {
            get { return chkSendOfferToPharmacy.Checked; }
        }
        public bool IsPPTCouponAvailable
        { get; set; }

        public string ScriptPadDestination { get; set; }
        public bool GoodRxCouponPharmacyNotesChecked
        {
            get { return chkSendOfferToPharmacy.Checked; }
            set { chkSendOfferToPharmacy.Checked = value; }
        }

        public bool GoodRxCouponPharmacyNotesEnabled
        {
            get { return chkSendOfferToPharmacy.Enabled; }
            set { chkSendOfferToPharmacy.Enabled = value; }
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
            RenderPptPlusCoupon();
        }
        public void RenderPptPlusCoupon()
        {
            string rxID = this.Attributes["RxID"];
            string couponPrice;
            if (PPTPlus.IsPptCouponAvailable(ControlState, Guid.Parse(rxID), out couponPrice))
            {
                IsPPTCouponAvailable = true;
                divPptCoupons.Visible = true;
            }
            if (!string.IsNullOrWhiteSpace(couponPrice))
            {
                decimal decimalCouponPrice = 0;
                decimal.TryParse(couponPrice, out decimalCouponPrice);
                couponPrice = decimalCouponPrice.ToString("#,#0.00#");
            }
            couponPrice = !string.IsNullOrWhiteSpace(couponPrice) ? string.Format(" for ${0}", couponPrice) : "";
            lbViewOffer.Text = string.Format("Rx Cash Discount Offer{0}", couponPrice);
            lbViewOffer.Attributes.Add("OfferURL", "GoodRxCoupon.aspx");
            setControlSize();
            if (!IsPostBack && IsPPTCouponAvailable)
            {
                chkPrintGoodRxOffers.Checked = Convert.ToBoolean(ControlState[Constants.SessionVariables.ShouldPrintOfferAutomatically]);
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
            Session["GoodRxCouponRxID"] = this.Attributes["RxID"];
            Session["OpenURLInFrame"] = null;
            string couponContent;
            if (Session["GoodRxCouponRxID"] != null)
            {
                string RxID = Session["GoodRxCouponRxID"].ToString();
                couponContent = PPTPlus.GetPptCouponContent(ControlState, new PptPlus(), new Guid(RxID));
            }
            else
                couponContent = PPTPlus.GetPptCouponContent(ControlState, new PptPlus(), new Guid(Convert.ToString(Session["GoodRxCouponRxID"])));

            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "OpenGoodRxCouponInModal", string.Format("OpenGoodRxCouponInModal(\"{0}\");", couponContent), true);
        }

    }
}