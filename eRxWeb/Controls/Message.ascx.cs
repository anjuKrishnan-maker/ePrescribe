using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Security.Application;

namespace eRxWeb
{
    public partial class Controls_Message : System.Web.UI.UserControl
    {
        public Controls_Message()
        {
        }

        public Controls_Message(bool isTest)
        {
            if (isTest)
            {
                divMessage = new HtmlGenericControl();
                imgStatus = new Image();
                tbMessage = new HtmlTable();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public enum MessageType
        {
            SUCCESS,
            ERROR,
            INFORMATION
        }

        public string MessageText
        {
            get { return divMessage.InnerText; }
            set
            {
                // fortify fix - Anti XSS
                divMessage.InnerHtml = Encoder.HtmlEncode(value)?.Replace("&#10;", "<br/>");//Replacing Environment.NewLine with HTML break;
            }
        }

        public MessageType Icon
        {
            set
            {
                switch (value)
                {
                    case MessageType.SUCCESS:
                        imgStatus.ImageUrl = "../images/info-global-16-x-16.png";
                        tbMessage.Attributes["class"] = "success-dialog";
                        break;
                    case MessageType.ERROR:
                        imgStatus.ImageUrl = "../images/alert-global-16-x-16.png";
                        tbMessage.Attributes["class"] = "error-dialog";
                        break;
                    case MessageType.INFORMATION:
                        imgStatus.ImageUrl = "../images/warning-global-16-x-16.png";
                        tbMessage.Attributes["class"] = "information-dialog";
                        break;
                    default:
                        imgStatus.ImageUrl = "../images/info-global-16-x-16.png";
                        tbMessage.Attributes["class"] = "success-dialog";
                        break;
                }
            }
        }
    }
}