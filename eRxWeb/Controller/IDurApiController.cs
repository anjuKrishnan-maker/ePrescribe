using System.Collections.Generic;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Objects;
using eRxWeb.State;

namespace eRxWeb.Controller
{
    public interface IDurApiController
    {
        bool CanApplyFinancialOffers { get; }
        Allscripts.Impact.Rx CurrentRx { get; }
        DURSettings DURSettings { get; }
        DURCheckResponse DURWarnings { get; set; }
        bool IsPOBUser { get; }
        bool IsUserAPrescribingUserWithCredentials { get; }
        IStateContainer PageState { get; }
        List<Allscripts.Impact.Rx> ScriptPadMeds { get; }
        Constants.UserCategory SessionUserType { get; }
    }
}