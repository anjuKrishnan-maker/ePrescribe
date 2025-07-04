using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Allscripts.Impact.PDI;

namespace eRxWeb.ServerModel
{
    public class PatientUploadResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool UploadSuccess { get; set; } = false;
        public PDI_ImportBatch CurrentJob { get; set; }
        public IList<PDI_ImportBatch> ImportBatchJobHistory{ get; set;}
    }
}