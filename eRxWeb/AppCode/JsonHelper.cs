using System.Text;
using System.Web;

namespace eRxWeb.AppCode
{
    public class JsonHelper
    {
        public static string ConvertUrlParameters(string urlParams)
        {
            if (string.IsNullOrWhiteSpace(urlParams)) return "";

            var paramsCol = HttpUtility.ParseQueryString(urlParams);
            
            var sb = new StringBuilder();
            foreach (var paramKey in paramsCol.AllKeys)
            {
                sb.Append($"\"{paramKey}\": \"{paramsCol[paramKey]}\",");
            }

            return sb.Length < 1 ? "" : $"{{{HttpUtility.ParseQueryString(sb.ToString(0, sb.Length-1))}}}";
        }
    }
}