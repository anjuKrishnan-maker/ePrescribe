using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace eRxWeb.AppCode.Authorize
{
    public class ApiAuthorizationFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool skipAuthorization = ShouldSkipAuthorization(actionContext);
            if (skipAuthorization)
                return;
            var state = HttpContext.Current.Session;
            if (state == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return;
            }

            bool isUserHavingAccess = AuthorizationManager.Process(new StateContainer(state),
                actionContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower(),
                actionContext.ActionDescriptor.ActionName.ToLower());
            if (isUserHavingAccess)
                return;

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }

        private static bool ShouldSkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any() ||
                   actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any();
        }
    }
}