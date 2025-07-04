using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.State;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Text;

namespace eRxWeb.AppCode
{
    public class PptPlusServiceBroker : IPptPlusServiceBroker
    {
        private IStateContainer _pageState;
        public IStateContainer PageState
        {
            get { return _pageState ?? (_pageState = new StateContainer(HttpContext.Current.Session)); }
        }
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public string InitiatePricingInquiry(string fhirRequest, string pptTokenB64)
        {
            var request = ConstructRequest(fhirRequest, pptTokenB64);

            string response = null;
            using (var timer = logger.StartTimer("InitiatePricingInquiry"))
            {
                timer.Message = $"<Request>{request}</Request>";
                if (!string.IsNullOrWhiteSpace(fhirRequest))
                {
                    response = ExecuteApiRequest(request, GetAcknowledgmentEndpoint(), Allscripts.Impact.ConfigKeys.PptPlusRequestTimeOut);
                    timer.Message += $"<Response>{response}</Response>";
                }
            }
            return response;
        }


        private string ExecuteApiRequest(RestRequest request, string endpoint,int timeout)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(endpoint),
                Timeout= timeout
            };
            var response = client.Execute(request);
            string result = default(string);

            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<string>(response.Content);
                }
                catch (Exception ex)
                {
                    AuditLog.ExceptionAdd(PageState.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString(), PageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString(),
                        $"PPTExecuteApiDeserializeException:<Exception>{ex}</Exception><ResponseContent>{response.Content}</ResponseContent>", HttpContext.Current.Request.UserIpAddress(),  null, null, PageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                    return String.Empty;
                }

            }

            if (response.ErrorException != null)
            {
                logger.Error($"PPT+ServiceBroker Exception: <EX>{response.ErrorException}</EX>");
                AuditLog.ExceptionAdd(PageState.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString(), PageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString(),
                                response.ErrorException.ToString(), HttpContext.Current.Request.UserIpAddress(),  null, null, PageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                return "";
            }

            return result;
        }

        private RestRequest ConstructRequest(string request, string pptTokenB64)
        {
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("X-Authorization", pptTokenB64);
            restRequest.AddParameter("application/json", request, ParameterType.RequestBody);
            restRequest.AddJsonBody(request);

            return restRequest;
        }

        private static string GetAcknowledgmentEndpoint()
        {
            var endpoint = Convert.ToString(ConfigurationManager.AppSettings["PptPlusProcessMessageEndpoint"]);
            return string.IsNullOrEmpty(endpoint) ? Allscripts.Impact.ConfigKeys.PptPlusProcessMessageEndpoint : endpoint;
        }

        private static string GetPricingInfoEndpoint()
        {
            var endpoint = Convert.ToString(ConfigurationManager.AppSettings["PptPlusPricingInfoEndpoint"]);
            return string.IsNullOrEmpty(endpoint) ? Allscripts.Impact.ConfigKeys.PptPlusPricingInfoEndpoint : endpoint;
        }

        private static string GetCouponInfoEndpoint()
        {
            var endpoint = Convert.ToString(ConfigurationManager.AppSettings["PptPlusCouponInfoEndpoint"]);
            return string.IsNullOrEmpty(endpoint) ? Allscripts.Impact.ConfigKeys.PptPlusCouponInfoEndpoint : endpoint;
        }

        private static string GetUsageReportEndpoint()
        {
            var endpoint = Convert.ToString(ConfigurationManager.AppSettings["PptPlusUsageReportEndpoint"]);
            return string.IsNullOrEmpty(endpoint) ? Allscripts.Impact.ConfigKeys.PptPlusUsageReportEndpoint : endpoint;
        }

        public string RetrievePricingInfo(string trnsId, string accountId, string ipAddress, string pptTokenB64)
        {
            var now = DateTime.Now;
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Authorization", pptTokenB64);
            request.AddParameter("trackingId", trnsId, ParameterType.QueryString);
            request.AddParameter("accountId", accountId, ParameterType.QueryString);
            request.AddParameter("transactionId", "123", ParameterType.QueryString);
            request.AddParameter("requestDateTime", now, ParameterType.QueryString);
            request.AddParameter("sendingMachine", ipAddress, ParameterType.QueryString);

            string response = null;
            using (var timer = logger.StartTimer("RetrievePricingInfo"))
            {
                timer.Message = $"<Request><trackingId>{trnsId}</trackingId><accountId>{accountId}</accountId><requestDateTime>{now}</requestDateTime><sendingMachine>{ipAddress}</sendingMachine></Request>";
                response = ExecuteApiRequest(request, GetPricingInfoEndpoint(), Allscripts.Impact.ConfigKeys.PptPlusRequestTimeOut);
                timer.Message += $"<Response>{response}</Response>";
            }
            return response;
        }

        public string RetrieveCouponInfo(string fhirRequest, string pptTokenB64)
        {
            var request = ConstructRequest(fhirRequest, pptTokenB64);

            string response = null;
            using (var timer = logger.StartTimer("RetrieveCouponInfo"))
            {
                timer.Message = $"<Request>{fhirRequest}</Request>";
                response = ExecuteApiRequest(request, GetCouponInfoEndpoint(), Allscripts.Impact.ConfigKeys.PptPlusRequestTimeOut);
                timer.Message += $"<Response>{response}</Response>";
            }
            return response;

        }

        public string SubmitUsageReportRequest(string fhirRequest, string pptTokenB64)
        {
            var request = ConstructRequest(fhirRequest, pptTokenB64);

            string response = null;
            using (var timer = logger.StartTimer("SubmitUsageReportRequest"))
            {
                timer.Message = $"<Request>{fhirRequest}</Request>";
                response = ExecuteApiRequest(request, GetUsageReportEndpoint(), Allscripts.Impact.ConfigKeys.PptPlusRequestTimeOut);
                timer.Message += $"<Response>{response}</Response>";
            }
            return response;

        }
    }
}