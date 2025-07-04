using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using eRxWeb.Controller;
using System.Web;
using System.Web.Http;
using System.Security.Principal;
using System.Web.Http.Routing;
using System.Web.Routing;
using System;
using System.Collections.Specialized;

using System.Web.SessionState;
using System.Reflection;
using System.IO;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests
{
    [TestClass()]
    public class EPCSApiControllerTests
    {
        const string URL= "http://localhost/api/EPCSApi/DisplayEpscLink";
        public static HttpContext MockHttpContext()
        {
            var httpRequest = new HttpRequest("", URL, "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);
          

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });
            
            //SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);
            return httpContext;
        }
        private HttpContextBase GetMockedHttpContext()
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            var request = MockRepository.GenerateMock<HttpRequestBase>();
            var response = MockRepository.GenerateMock<HttpResponseBase>();
            var session = MockRepository.GenerateMock<HttpSessionStateBase>();
            var server = MockRepository.GenerateMock<HttpServerUtilityBase>();
            var user = MockRepository.GenerateMock<IPrincipal>();
            var identity = MockRepository.GenerateMock<IIdentity>();
            var urlHelper = MockRepository.GenerateMock<UrlHelper>();

            RequestContext requestContext = new RequestContext();
            
            requestContext.Stub(x => x.HttpContext).Return(context);
            context.Stub(ctx => ctx.Request).Return(request);
            context.Stub(ctx => ctx.Response).Return(response);
            context.Stub(ctx => ctx.Session).Return(session);
            context.Stub(ctx => ctx.Server).Return(server);
            context.Stub(ctx => ctx.User).Return(user);
            user.Stub(ctx => ctx.Identity).Return(identity);
            identity.Stub(id => id.IsAuthenticated).Return(true);
            identity.Stub(id => id.Name).Return("test");
            request.Stub(req => req.Url).Return(new Uri("/api/EPCSApi/DisplayEpscLink"));
            request.Stub(req => req.RequestContext).Return(requestContext);
            requestContext.Stub(x => x.RouteData).Return(new RouteData());
            request.Stub(req => req.Headers).Return(new NameValueCollection());

            return context;
        }
        [TestMethod()]
        public void DisplayEpscLinkTest()
        {
            EPCSApiController epcsApiCont = new EPCSApiController();
            HttpContext.Current = MockHttpContext();
            var result = epcsApiCont.DisplayEpscLink();
        }
    }
    public class HttpContextManager
    {
        private static HttpContextBase m_context;
        public static HttpContextBase Current
        {
            get
            {
                if (m_context != null)
                    return m_context;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");

                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            m_context = context;
        }
        
    }
}