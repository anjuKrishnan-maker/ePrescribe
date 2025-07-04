using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class PdmpEnrollmentSubmissionResponse
    {
        public bool IsSubmissionSuccessful { get; set; }
        public string EnrollmentSuccessMessage { get; set; }
    }
}