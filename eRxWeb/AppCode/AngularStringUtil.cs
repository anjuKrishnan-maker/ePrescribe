
using System.Text;
using System.Web;
using Allscripts.ePrescribe.Common;
using Newtonsoft.Json;

namespace eRxWeb.AppCode
{
    public class AngularStringUtil
    {
        public static string CreateUrl(string pageName, object parameters = null)
        {
            return Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + pageName + "&componentParameters=" + JsonConvert.SerializeObject(parameters);
        }

        public static string CreateUrl(string pageName, string serializedObject)
        {
            return Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + pageName + "&componentParameters=" + serializedObject;
        }

        public static string CreateInitComponentUrl(string component)
        {
            return $"{Constants.PageNames.SPA_LANDING}";
        }
    }
}