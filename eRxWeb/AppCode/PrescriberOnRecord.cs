using System;
using System.Collections.Generic;
using System.Linq;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects.CommonComponent;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class PrescriberOnRecord
    {
        public static string SetInSession(RxUser prescriber, IStateContainer session, IRxUser user, ISPI spi, IDeaScheduleUtility deaUtility)
        {
            if (!prescriber.HasNPI)
            {
                return "Please select a provider with a valid NPI.";
            }

            var licenseId = session.GetGuidOr0x0(Constants.SessionVariables.LicenseId);
            var siteId = session.GetInt(Constants.SessionVariables.SiteId, 1);
            var currentUserId = session.GetGuidOr0x0(Constants.SessionVariables.UserId);
            var dbId = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            
            session.Remove(Constants.SessionVariables.SupervisingProviderId);

            var deaSchedules = deaUtility.ExtractAllowed(prescriber);
            session[Constants.SessionVariables.DelegateDeaAllowed] = deaSchedules;
            session[Constants.SessionVariables.DelegateMinDea] = deaUtility.RetrieveMin(deaSchedules);

            user.UpdatePobProviderUsage(licenseId, siteId, currentUserId, prescriber.UserID.ToGuidOr0x0(), dbId);
            
            var userIdForSpi = session.GetBooleanOrFalse(Constants.SessionVariables.IsPaSupervised)
                ? currentUserId
                : prescriber.UserID.ToGuidOr0x0();

            session[Constants.SessionVariables.SPI] = spi.RetrieveSpiForSession(userIdForSpi, dbId);

            // save the value in session so it's not forcing loading full object of DelegateProvider every time
            session[Constants.SessionVariables.DelegateProviderNPI] = prescriber.NPI;
            session[Constants.SessionVariables.DelegateProviderId] = prescriber.UserID;
            session[Constants.SessionVariables.ShouldPrintOfferAutomatically] = prescriber.ShouldPrintOfferAutomatically;
            session[Constants.SessionVariables.ShouldShowTherapeuticAlternativeAutomatically] = prescriber.ShouldShowTherapeuticAlternativeAutomatically;
            session[Constants.SessionVariables.ShouldShowPpt] = prescriber.ShouldShowPpt;
            session[Constants.SessionVariables.ShouldShowRtbi] = prescriber.ShouldShowRtbi;

            var provInfo = ProviderInfo.Create(prescriber.UserID.ToGuidOr0x0(), prescriber.FirstName, prescriber.LastName, prescriber.DEA, prescriber.NPI, prescriber.Email);
            session[Constants.SessionVariables.CommonCompProviderInfo] = provInfo;

            return null;
        }
    }
}