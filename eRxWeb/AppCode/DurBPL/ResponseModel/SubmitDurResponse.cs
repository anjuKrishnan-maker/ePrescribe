using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.ResponseModel
{
    public class SubmitDurResponse
    {
        public bool IsGatherWarningsAndInfoToWriteToDB  { get; set; }
        public string RxID { get; set; }
        public string DurGoNext { get; set; }
        public string Url { get; set; }
        public bool IsCsMedWorkFlow { get; set; }
        public bool IsstarsAlign { get; set; }
        public bool IsCSMedRefillRequestNotAllowed { get; set; }
        public bool IsUserAPrescribingUserWithCredentials { get; set; }
        public DURCheckResponse DURWarnings { get; set; }
        public bool RefreshUcEPCSDigitalSigning { get; set; }
        public bool IsDurWarningNull { get; set; }
        public List<Rx> EpcsMedList { get; set; }
        public bool IsIgnoreReasonsNotSelected { get; set; }
        public bool IsRefreshActiveMeds { get; set; }
        public bool isRedirect { get; set; }
        public SelectPatientComponentParameters selectPatientComponentParameters { get; set; }
    }
}