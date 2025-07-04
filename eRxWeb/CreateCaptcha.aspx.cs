using System;
using System.Drawing.Imaging;
using System.Web.UI;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;

namespace eRxWeb
{
    public partial class CreateCaptcha1 : BasePage
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //Create a Captcha image
            String sTemp, sTemp2, sGuid;
            sGuid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6); //.ToUpper();
            sTemp2 = sGuid.Substring(2, 2).ToUpper();
            sTemp = sGuid.Substring(0, 2).ToUpper();

            logger.Debug("CaptchaTextSessionBefore: " + PageState.GetStringOrEmpty("CaptchaText"));

            Session["CaptchaText"] = sTemp + sTemp2 + sGuid.Substring(4, 2).ToUpper();

            logger.Debug("CaptchaTextSessionAfter: " + PageState.GetStringOrEmpty("CaptchaText"));

            CaptchaImage objCI = new CaptchaImage(Session["CaptchaText"].ToString(), "Arial", 200, 50);
            using (objCI)
            {
                //Change response headers to output JPEG content
                Response.Clear();
                Response.ContentType = "image/jpeg";
                //Write the image to the response stream
                objCI.Bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);
            }
        }
    }
}