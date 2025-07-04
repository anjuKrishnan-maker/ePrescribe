
using eRxWeb.AppCode.Authorize;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

public class WebApiConfig
{
    public static string UrlPrefixRelative
    {
        get
        {
            return "api/";
        }
    }

    public static void Register(HttpConfiguration configuration)
    {
        configuration.MapHttpAttributeRoutes();
        configuration.Filters.Add(new ApiAuthorizationFilter());
        configuration.Routes.MapHttpRoute("APIPost", "api/{controller}");
        configuration.Routes.MapHttpRoute("APIDefault", "api/{controller}/{Action}/{id}",
          new { id = RouteParameter.Optional });


    }


}
