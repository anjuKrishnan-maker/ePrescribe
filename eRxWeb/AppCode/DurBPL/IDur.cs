using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using eRxWeb.AppCode.DurBPL.RequestModel;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using eRxWeb.State;
using System.Collections.Generic;
using Telerik.Web.UI;
using static Allscripts.Impact.IgnoreReason;

namespace eRxWeb.AppCode.DurBPL
{
    public interface IDur
    {
        CSMedRefillNotAllowedContactResponse CSMedRefillRequestNotAllowedOnContactProvider(ICSMedRefillNotAllowedContactRequest contactRequest, IImpactDurWraper impactDur);
        CSMedRefillNotAllowedPrintRefillResponse CSMedRefillRequestNotAllowedOnPrintRefillRequest(ICSMedRefillNotAllowedPrintRefillRequest requestOnPrintRefillRequest, IImpactDurWraper impactDur);
        string GetDEANumberToBeUsed(IStateContainer state);
        int getHeaderReason(RadGrid currentGrid);
        
        void SetDurWarnings(DURCheckResponse dURCheckResponse);
        IStateContainer GetPageState();
        RxDurResponseModel GetDurRxList(IeRxWebAppCodeWrapper wrapper);
        DurRedirectModel GetWarningRedirectDetails();
        bool SaveDurWarnings(ISaveDurRequest saveDurRequst, IImpactDurWraper impactDur);
        SubmitDurResponse SaveDurForm(SubmitDurRequest submitDurRequest, IImpactDurWraper impactDur);
        GoBackResponse GoBack(GoBackRequest goBackRequest);
        List<IgnoreReasonsResponse> GetIgnoreReasonsByCategoryAndUserType(IgnoreCategory category,IImpactDurWraper durWrapper);
        EPCSDigitalSigningResponse EPCSDigitalSigningOnDigitalSigningComplete(EPCSDigitalSigningRequest signingRequest, IImpactDurWraper impactDur);
       
      
        DURCheckResponse GetDurWarnings(IeRxWebAppCodeWrapper wrapper);
    }
}       