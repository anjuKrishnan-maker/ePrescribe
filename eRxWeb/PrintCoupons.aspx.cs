using System;
using System.Collections.Generic;
using System.Text;
using Allscripts.Impact;
using BOL = Allscripts.ePrescribe.Objects;
using TieServiceClient;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class PrintCoupons : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<string> couponPrintList = new List<string>();
                couponPrintList = (List<string>)Session["CouponPrintList"];

                StringBuilder htmlOutput = new StringBuilder();

                foreach (string eCouponDetailId in couponPrintList)
                {
                    if (htmlOutput.Length > 0)
                    {
                        htmlOutput.Append("<div style='page-break-after:always; height:1px; border-top:1px solid black; margin: 10px 0px 10px 0px;'></div>");
                    }
                    var response = EPSBroker.GetECouponDetailsLiteByID(eCouponDetailId, base.SessionLicenseID, base.SessionUserID, base.DBID);
                    htmlOutput.Append("<div><img src='").Append(response.ECouponDetailLiteModel.OfferImageURL.ToHTMLEncode()).Append("'/></div>");
                }

                litHtmlOutput.Text = htmlOutput.ToString();
            }
        }
    }

}