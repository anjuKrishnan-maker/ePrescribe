using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExpertPdf.HtmlToPdf;
using System.Configuration;
using System.IO;
using eRxWeb.AppCode.PptPlusBPL;
using System.Text;

namespace eRxWeb
{
    public partial class GoodRxCoupon : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> rxIds = PageState.Cast<List<string>>("PrintGoodRxCoupon", new List<string>());
            StringBuilder htmlOutput = new StringBuilder();
            foreach (string rxID in rxIds)
            {
                htmlOutput.Append(RetrieveGoodRxCouponHtmlContent(rxID));
            }

            scriptContent.InnerHtml = htmlOutput.ToString();
        }

        public string RetrieveGoodRxCouponHtmlContent(string rxId)
        {

            string couponContent;
            if (!string.IsNullOrEmpty(rxId))
            {
                couponContent = PPTPlus.GetPptCouponContent(PageState, new PptPlus(), new Guid(rxId));
            }
            else
                couponContent = PPTPlus.GetPptCouponContent(PageState, new PptPlus(), new Guid(Convert.ToString(rxId)));
            return Base64Decode(couponContent);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}