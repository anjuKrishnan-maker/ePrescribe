using Allscripts.Impact;
using Allscripts.Impact.Ilearn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
    public partial class ILearn : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Get Info From Config
                string username = ConfigKeys.ILearnWebUsername;
                string password = ConfigKeys.ILearnWebPassword;
                string url = ConfigKeys.ILearnBaseURL + "post.php";

                //Get Request Parameters
                string pageName = Request.QueryString["iUrl"] != null ? Request.QueryString["iUrl"] : "All";
                string whatIsNew = Request.QueryString["win"] != null ? Request.QueryString["win"] : "none";

                //Get all parameters
                string screenName = ILearnConfigurationManager.GetScreenName(pageName);
                string clientID = SessionLicenseID;
                string userID = PageState.GetStringOrEmpty("UserCode");
                string userType = ILearnConfigurationManager.GetUserType(PageState.Cast<Allscripts.Impact.RxUser.UserCategory>("UserType", Allscripts.Impact.RxUser.UserCategory.GENERAL_USER));
                string isAdmin = ILearnConfigurationManager.GetAdminString(PageState.GetBooleanOrNull("IsAdmin"));

                //Form Url Parameter
                url = url + new ILearnConfigurationManager().GetPortalUrl(screenName, clientID, userID, isAdmin, userType, whatIsNew);

                //Post
                PostToILearn(url, username, password, screenName, clientID, userID, userType, isAdmin);
            }
            catch(Exception ex)
            {
                //Log ex;
                throw ex;
            }
        }

        public static string ConvertToBase64(string content)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
        }                

        public void PostToILearn(string url, string username, string password, string screenName, string clientID, string userID, string userType, string isAdmin)
        {
            var builder = new StringBuilder();
            builder.Append("<html>");
            builder.Append("<body onload=\"document.forms.launcher.submit()\">");
            builder.AppendFormat("<form id=\"launcher\" method=\"post\" action=\"{0}\">", url);
            builder.AppendFormat("<input type=\"hidden\" name=\"UserName\" value=\"{0}\">", ConvertToBase64(username));
            builder.AppendFormat("<input type=\"hidden\" name=\"Password\" value=\"{0}\">", ConvertToBase64(password));
            builder.AppendFormat("<input type=\"hidden\" name=\"Screen_Name\" value=\"{0}\">", ConvertToBase64(screenName));
            builder.AppendFormat("<input type=\"hidden\" name=\"Client_Id\" value=\"{0}\">", ConvertToBase64(clientID));
            builder.AppendFormat("<input type=\"hidden\" name=\"User_Id\" value=\"{0}\">", ConvertToBase64(userID));
            builder.AppendFormat("<input type=\"hidden\" name=\"User_Type\" value=\"{0}\">", ConvertToBase64(userType));
            builder.AppendFormat("<input type=\"hidden\" name=\"Is_Admin\" value=\"{0}\">", ConvertToBase64(isAdmin));
            builder.Append("</form>");
            builder.Append("</body>");
            builder.Append("</html>");

            var writer = new StreamWriter(Response.OutputStream);
            writer.Write(builder.ToString());
            writer.Close();
        }
    }
}