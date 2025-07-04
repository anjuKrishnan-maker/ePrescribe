using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public class PdfStreamController : ApiController
    {
        //  [Route("api/PdfStream/GetPdfContent/{ulr}")]
        [HttpGet]
        public HttpResponseMessage GetPdfContent(string url)
        {
            using (HttpClient cl = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = null;
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                List<string> eCouponUrlWhiteList = session.Cast(Constants.SessionVariables.ECouponUrlWhiteList, new List<string>());

                if (eCouponUrlWhiteList.Contains(url.ToUrlEncode()))
                {
                    cl.BaseAddress = new Uri(url);
                    httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                    var t = cl.GetByteArrayAsync("");
                    t.Wait();
                    var dataBytes = t.Result;
                    var dataStream = new MemoryStream(dataBytes);
                    httpResponseMessage.Content = new StreamContent(dataStream);
                    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = "test.pdf";
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                }
                else // if URL is not in the session white list
                {
                    string exceptionMsg = "the eCoupon listed at " + url + " is invalid as it has not in the allowed list of avaliable coupons." + "\n" + "Please contact Verdigm Support";
                    AuditLog.ExceptionAdd(session.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString(), session.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString(),
                     exceptionMsg, HttpContext.Current.Request.UserIpAddress(), 
                     null, null, session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

                    //return invalid content pdf message
                    httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

                    var fileName = HttpContext.Current.Server.MapPath(Constants.StaticContentPaths.InvalidContentMessagePDFFilePath);
                    var fileBytes = File.ReadAllBytes(fileName);
                    httpResponseMessage.Content = new StreamContent(new MemoryStream(fileBytes));
                    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = "VeradigmError.pdf";
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                }

                return httpResponseMessage;
            }
        }
    }
}
