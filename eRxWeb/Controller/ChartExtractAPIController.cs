using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Web;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Allscripts.Impact.PDI;
using System.Net.Http.Headers;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.DatabaseSelector;
using Hl7.Fhir.Model;
using static Allscripts.ePrescribe.Common.Constants;
using Newtonsoft.Json;

namespace eRxWeb.Controller
{
    public partial class ChartExtractAPIController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        IStateContainer pageState;

        public IStateContainer PageState
        {
            get
            {
                if (pageState == null)
                {
                    pageState = new StateContainer(HttpContext.Current.Session);
                }

                return pageState;
            }
            set { pageState = value; }
        }

        [HttpGet]
        public HttpResponseMessage GenerateDownloadFile(string id, string license, long startTicks, long endTicks,
            ChartExtractTypes type)
        {
            var extractIdConversionSuccessful = Guid.TryParse(id, out var extractId);
            var licenseConversionSuccessful = Guid.TryParse(license, out var licenseId);
            var userIdConversionSuccessful = Guid.TryParse(ApiHelper.GetSessionUserID(PageState), out var userGuid);
            var ipAddress = HttpContext.Current.Request.UserIpAddress();
            var userName = ApiHelper.GetSessionShieldUserName(pageState);

            if (!extractIdConversionSuccessful || !licenseConversionSuccessful)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            ExtractRequest req = new ExtractRequest(extractId, licenseId, new DateTime(startTicks),
                new DateTime(endTicks), type);
            using (var timer = logger.StartTimer("GenerateChartExtactDownloadFile"))
            {
                try
                {
                    var dbId = ApiHelper.GetDBID(PageState);

                    List<ExtractChunk> chunks = ExtractUtils.FetchExtractChunksForExtract(req, dbId);
                    var downloadFileContents = ExtractUtils.ConstructOriginalStringFromChunks(chunks);

                    if (downloadFileContents.Length == 0)
                    {
                        return new HttpResponseMessage(HttpStatusCode.NoContent);
                    }

                    HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);

                    httpResponseMessage.Content = new ByteArrayContent(downloadFileContents);

                    httpResponseMessage.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition =
                        new ContentDispositionHeaderValue("attachment") {FileName = "test.txt"};

                    if (userIdConversionSuccessful)
                    {
                        var extendedExtractAudit = ExtractUtils.FormatAuditInfoForChartExtract(userGuid, userName, req,
                            ipAddress, DateTime.UtcNow, dbId);

                        AddExtendedAuditLogForExtractDownload(extendedExtractAudit);

                    }
                    else
                    {
                        //report error for logging.
                    }

                    return httpResponseMessage;

                }
                catch (Exception e)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
        }


        [HttpGet]
        public HttpResponseMessage GetRequests(ChartExtractTypes type)
        {
            using (var timer = logger.StartTimer("GetRequests, type: " + type))
            {
                try
                {
                    string licenseID = ApiHelper.GetSessionLicenseID(PageState);
                    ConnectionStringPointer dbid = ApiHelper.GetDBID(PageState);

                    List<ExtractRequest> allReqs =
                        ExtractUtils.FetchExtractRequestsByLicenseAndType(Guid.Parse(licenseID), type,
                            dbid);

                    if (allReqs == null || allReqs.Count == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    }

                    HttpResponseMessage rtrn = Request.CreateResponse(HttpStatusCode.OK);
                    rtrn.Content = new StringContent(JsonConvert.SerializeObject(allReqs));

                    return rtrn;
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
        }

        private void AddExtendedAuditLogForExtractDownload(ReportAuditExtendedInfo extractReport)
        {
            Audit.AddAuditRecordAndSetExtendedInfoFieldForChartExtract(extractReport,  new AuditLog());
        }
    }
}
