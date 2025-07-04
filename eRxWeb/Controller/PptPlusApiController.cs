using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.State;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.ServerModel;
using ConfigKeys = Allscripts.Impact.ConfigKeys;
using Allscripts.ePrescribe.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Web.Http;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel.Request;
using Allscripts.ePrescribe.Shared.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allscripts.ePrescribe.Data.CommonComponent;
using eRxWeb.AppCode.Rtps;

namespace eRxWeb.Controllers
{
    public partial class PptPlusApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse InitiatePricingInquiry(PptPlusSummaryRequest data)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            using (var timer = logger.StartTimer("InitiatePricingInquiry"))
            {
                var response = new ApiResponse
                {
                    Payload = PPTPlus.HandleInitPricing(data, session,
                    new PptPlus(), new PptPlusServiceBroker(),
                    null, new PptPlusData(),
                    new CommonComponentData(),
                    new RtpsHelper()).ToString()
                };

                timer.Message = $"<Request>{data.ToLogString()}</Request><Response>{response}</Response>";
                return response;
            }
        }

        [HttpPost]
        public async Task<ApiResponse> InitiatePricingInquiryBulk(PptPlusSummaryRequest[] requests)
        {
            using (var timer = logger.StartTimer("InitiatePricingInquiryBulk"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    var pricingTasks = new Task<int>[requests.Count()];
                    var context = HttpContext.Current;
                    for (var i = 0; i < requests.Count(); i++)
                    {
                        var data = requests[i];
                        pricingTasks[i] = Task.Run( () =>
                        {
                            HttpContext.Current = context;
                            int responseTime = PPTPlus.HandleInitPricing(data, session, new PptPlus(), 
                                new PptPlusServiceBroker(), null, 
                                new PptPlusData(), new CommonComponentData(), 
                                new RtpsHelper());
                            return responseTime;
                        });


                    }
                    await Task.WhenAll(pricingTasks.ToArray());
                    response.Payload = pricingTasks[pricingTasks.Count() - 1].Result;
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                }
                return  response;
            }
        }


        [HttpPost]
        public ApiResponse InitiatePricingInquiryUsingSessionData(PptPlusSummaryRequest data)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var pptPlusResponse = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            var med = pptPlusResponse.Candidates.FirstOrDefault(_ => _.MedListIndex == Convert.ToInt32(data.MedSearchIndex));
            using (var timer = logger.StartTimer("InitiatePricingInquiryUsingSessionData"))
            {
                var response = new ApiResponse
                {
                    Payload = PPTPlus.HandleInitPricing(new PptPlusSummaryRequest {
                        DDI = med.DDI,
                        GPPC = med.GPPC,
                        PackUom = med.PackageUOM,
                        Quantity = med.Quantity,
                        PackSize = med.PackSize,
                        PackQuantity = med.PackQuantity,
                        Refills = med.Refills,
                        DaysSupply = med.DaysSupply,
                        IsDaw = med.IsDaw.ToString(),
                        MedSearchIndex = med.MedListIndex.ToString() }, 
                        session, new PptPlus(), new PptPlusServiceBroker(),
                        med.PharmacyID, new PptPlusData(), 
                        new CommonComponentData(), 
                        new RtpsHelper()).ToString()
                };

                timer.Message = $"<Request>{data.ToLogString()}</Request><Response>{response}</Response>";
                return response;
            }
        }

        [HttpPost]
        public ApiResponse RetrieveSummaryUi(PptPlusSummaryRequest data)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            using (var timer = logger.StartTimer("RetrieveSummaryUi"))
            {
                var response = new ApiResponse
                {
                    Payload = PPTPlus.RetrieveSummaryUiFromPricingInfo(Convert.ToInt32(data.MedSearchIndex), session,
                        new PptPlus(), new PptPlusServiceBroker(), new CommonComponentData(), new RtpsHelper(), new EPSBroker())
                };

                timer.Message = $"<Request>{data.ToLogString()}</Request><Response>{response}</Response>";
                return response;
            }
        }

        [HttpPost]
        public ApiResponse RemoveUnselectedRows(PptPlusSummaryRequest request)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var responseContainer = session.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
            using (var timer = logger.StartTimer("RemoveUnselectedRows"))
            {
                foreach (var removedIndex in request.RemovedIndexes)
                {
                    var candidateToRemove =
                        responseContainer?.Candidates?.FirstOrDefault(_ => _.MedListIndex.ToString() == removedIndex);
                    if (candidateToRemove != null)
                    {
                        responseContainer.Candidates.Remove(candidateToRemove);
                    }
                }

                session[Constants.SessionVariables.PptPlusResponses] = responseContainer;
                timer.Message = request.ToLogString();
            }

            return new ApiResponse{Payload = ""};
        }

        [HttpPost]
        public ApiResponse RetrieveAllScriptPadMedSummaryUi()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("RetrieveAllScriptPadMedSummaryUi"))
                {
                    var ui = PPTPlus.RetrieveAllScriptPadMedSummaryUi(session, new PptPlus()).ToArray();
                    timer.Message = ui.ToLogString();
                    response.Payload = ui;
                }
            }
            catch (Exception ex)
            {
                response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
            }
            return response;
        }

        [HttpPost]
        public dynamic IsAutoShowDetailScreen()
        {
            using (var timer = logger.StartTimer("IsAutoShowDetailScreen"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = PPTPlus.IsAutoShowDetailScreen(session);
                }
                catch (Exception ex)
                {
                    response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
                }
                return response;
            }
        }

        [HttpPost]
        public ApiResponse ShouldShowDetail()
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("ShouldShowDetail"))
                {
                    var isAvail = PPTPlus.IsPPTDetailInfoAvailable(session, new PptPlus());
                    timer.Message = isAvail.ToString();
                    response.Payload = isAvail;
                }
            }
            catch (Exception ex)
            {
                response.ErrorContext = new ErrorContextModel() { Error = ErrorTypeEnum.ServerError, Message = ex.Message };
            }
            return response;
        }

        [HttpPost]
        public ApiResponse GetCommonUIUrl()
        {
            using (var timer = logger.StartTimer("GetCommonUIUrl"))
            {
                return new ApiResponse { Payload = ConfigKeys.PptPlusCommonUIEndpoint };
            }
        }

        [HttpPost]
        public ApiResponse PptDetailsHandleOKClick(PPTPlusUserChanges request)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("PptDetailsHandleOKClick"))
                {
                    response.Payload = PPTPlus.PptDetailsHandleOKClick(request, session, new EPSBroker(), new RtpsHelper());
                    timer.Message = $"<Request>{request.ToLogString()}</Request><Response>{response.Payload}</Response>";
                }
            }
            catch(Exception ex)
            {
                string exceptionID = PPTPlus.AddException(ex.ToString(), session);
                var ucResponse = new PPTPlusDetailsUserChangesResponse();
                ucResponse.Status = PPTPlusDetailsUserChangeStatus.Fail;
                ucResponse.Message = $"We apologize, but an error has occurred. Error Reference ID = " + exceptionID + ". You can click OK to retry or Cancel to close this popup.";
                response.Payload = ucResponse;
            }

            return response;
        }        
    }
}