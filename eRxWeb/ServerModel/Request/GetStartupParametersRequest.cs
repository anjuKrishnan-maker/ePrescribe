using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.AppCode;

namespace eRxWeb.ServerModel.Request
{
    public class GetStartupParametersRequest
    {
        public PatientDemographics PatientDemographics { get; set; }
    }
}