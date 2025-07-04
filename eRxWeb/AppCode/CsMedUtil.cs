using System;
using System.Web;
using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using ServiceStack;
using System.Linq;

namespace eRxWeb.AppCode
{
    public class CsMedUtil
    {
        public static bool ShouldShowCsRegistryControls(IStateContainer pageState)
        {
            return pageState.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ") ||
                   (!String.IsNullOrEmpty(pageState.GetStringOrEmpty("STATEREGISTRYURL")) &&
                    Boolean.Parse((pageState.GetStringOrEmpty("ShowCSRegistry"))));
        }

        public static bool ShouldShowCsRegistryControls(bool hasCsMeds, bool showCsRegistryEnterpriseSetting, IStateContainer pageState)
        {
            return hasCsMeds &&  
                   (pageState.GetBooleanOrFalse("ISCSREGISTRYCHECKREQ") || 
                    (!pageState.GetStringOrEmpty("STATEREGISTRYURL").IsNullOrEmpty() && showCsRegistryEnterpriseSetting));
        }

        public static string RedirectForCSMed(string pharmDetails, string rxDetails, string MedName)
        {
            return AngularStringUtil.CreateUrl(Constants.PageNames.SELECT_MEDICATION, JsonHelper.ConvertUrlParameters($"RefillPharmacy={pharmDetails}&RxDetails={rxDetails}&from={Constants.PageNames.APPROVE_REFILL_TASK}&SearchText=" + HttpUtility.UrlEncode(MedName.Split(' ').FirstOrDefault())));
        }
        public static string RedirectForCSMedChangeRx(string pharmDetails, string rxDetails, string MedName, string scriptMessageId)
        {
            return AngularStringUtil.CreateUrl(Constants.PageNames.SELECT_MEDICATION, JsonHelper.ConvertUrlParameters($"ChangeRxPharmacy={pharmDetails}&ScriptMessageGuid={scriptMessageId}&RxDetails={rxDetails}&from={Constants.PageNames.APPROVE_REFILL_TASK}&SearchText=" + HttpUtility.UrlEncode(MedName)));
        }
    }
}