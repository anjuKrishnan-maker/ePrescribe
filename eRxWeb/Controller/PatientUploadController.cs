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
using System.Runtime.CompilerServices;

namespace eRxWeb.Controller
{
    public partial class PatientUploadController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();
        IStateContainer pageState;

        public IStateContainer PageState
        {
            get
            {
                if (pageState == null)
                    pageState = new StateContainer(HttpContext.Current.Session);
                return pageState;
            }
            set
            {
                pageState = value;
            }
        }

        [HttpPost]
        public ApiResponse UploadFile([FromBody] string PatientData)
        {
            using (var timer = logger.StartTimer("UploadPatientFile"))
            {
                try
                {
                    var response = PatientFileUpload.SavePatientFileData(PatientData, PageState);
                    return new ApiResponse
                    {
                        Payload = response
                    };
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("PDIInsertJob Exception: " + ex.ToString());
                    return new ApiResponse
                    {
                        Payload = new PatientUploadResponse
                        {
                            ErrorMessage = errorMessage,
                            StatusCode = HttpStatusCode.BadRequest
                        }
                    };
                }
            }
        }

        [HttpGet]
        public ApiResponse GetJobStatus()
        {
            using (var timer = logger.StartTimer("CheckPatientUploadJobStatus"))
            {
                try
                {
                    var response = PatientFileUpload.CheckJobStatus(PageState, new PDI_ImportBatch());
                    return new ApiResponse
                    {
                        Payload = response
                    };
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("GetPDIJobStatus Exception: " + ex.ToString());
                    return new ApiResponse
                    {
                        Payload = new PatientUploadResponse
                        {
                            ErrorMessage = errorMessage,
                            StatusCode = HttpStatusCode.BadRequest
                        }
                    };
                }
            }
        }

        [HttpGet]
        public HttpResponseMessage GenerateReport(int job)
        {
            using (var timer = logger.StartTimer("GeneratePatientUploadReport"))
            {
                try
                {
                    var response = PatientFileUpload.GenerateJobHistoryReport(PageState, new PDI_ImportBatch { ID = job }, new PDI_ImportBatch());

                    HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                    httpResponseMessage.Content = new StringContent(response.CurrentJob.ReportDataStream.ToString());
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "PatientDataImport.csv" };

                    return httpResponseMessage;
                }
                catch (Exception ex)
                {
                    ApiHelper.AuditException(ex.ToString(), PageState);
                    logger.Error("GeneratingPDIJobReport Exception: " + ex.ToString());
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}