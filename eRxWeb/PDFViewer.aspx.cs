using System;
using System.Web.UI;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
    public partial class PDFViewer : Page
    {
        public string PdfUrl { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.QueryString["pdf"] != null)
            {
                PdfUrl = Request.QueryString["pdf"].ToString().ToHTMLEncode();
            }
        }
    }
}