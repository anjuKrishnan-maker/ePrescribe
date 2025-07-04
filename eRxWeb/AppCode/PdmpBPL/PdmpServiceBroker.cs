using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.State;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;

namespace eRxWeb.AppCode.PdmpBPL
{
    public class PdmpServiceBroker : IPdmpServiceBroker
    {
        private IStateContainer _pageState;
        public IStateContainer PageState
        {
            get { return _pageState ?? (_pageState = new StateContainer(HttpContext.Current.Session)); }
        }
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        public string PatientRetrieveResponse(string fhirRequest, string pdmpTokenB64)
        {
            var request = ConstructRequest(fhirRequest, pdmpTokenB64);

            string response = null;
            using (var timer = logger.StartTimer("RetrivePatientResponse"))
            {
                timer.Message = $"<Request>{request}</Request>";
                if (!string.IsNullOrWhiteSpace(fhirRequest))
                {
                    response = ExecuteApiRequest(request, GetPatientRetriveEndpoint(), Allscripts.Impact.ConfigKeys.PdmpRequestTimeOut);
                    timer.Message += $"<Response>{response}</Response>";
                }
            }
            return response;
        }

        private string ExecuteApiRequest(RestRequest request, string endpoint, int timeout)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(endpoint),
                Timeout = timeout
            };
            var response = client.Execute(request);
            string result = default(string);

            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                try
                {
                    result = response.Content;
                }
                catch (Exception ex)
                {
                    AuditLog.ExceptionAdd(PageState.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString(), PageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString(),
                        $"PDMPExecuteApiDeserializeException:<Exception>{ex}</Exception><ResponseContent>{response.Content}</ResponseContent>", HttpContext.Current.Request.UserIpAddress(), null, null, PageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                    return String.Empty;
                }

            }

            if (response.ErrorException != null)
            {
                logger.Error($"PDMPServiceBroker Exception: <EX>{response.ErrorException}</EX>");
                AuditLog.ExceptionAdd(PageState.GetGuidOr0x0(Constants.SessionVariables.UserId).ToString(), PageState.GetGuidOr0x0(Constants.SessionVariables.LicenseId).ToString(),
                                response.ErrorException.ToString(), HttpContext.Current.Request.UserIpAddress(),  null, null, PageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));
                return "";
            }

            return result;
        }

        private RestRequest ConstructRequest(string request, string pdmpTokenB64)
        {
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("X-Authorization", pdmpTokenB64);
            restRequest.AddParameter("application/json", request, ParameterType.RequestBody);
            restRequest.AddJsonBody(request);

            return restRequest;
        }
        
        private static string GetPatientRetriveEndpoint()
        {
            var endpoint = Convert.ToString(ConfigurationManager.AppSettings["PdmpRetrivePatientEndpoint"]);
            return string.IsNullOrEmpty(endpoint) ? Allscripts.Impact.ConfigKeys.PdmpPatientRetriveEndpoint : endpoint;
        }
       
    }
}