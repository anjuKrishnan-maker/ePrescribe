using eRxWeb.State;
using System;
using System.IO;

namespace eRxWeb
{
    public partial class PrintPDF : BasePage
    {
        private IStateContainer pageState;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            pageState = new StateContainer(Session);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            MemoryStream oStream = this.pageState.Cast<MemoryStream>("printPdf", null);
            if (oStream != null)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(oStream.ToArray());
                this.pageState.Remove("printPdf");
                Response.End();
            }
        }
    }
}