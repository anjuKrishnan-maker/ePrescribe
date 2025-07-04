
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.State;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb.Controllers
{
    public partial class CancelRxApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public void SendCancelRx(IEnumerable<SendCancelRxRequestModel> cancelRxItemList)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            var ip = HttpContext.Current.Request.UserIpAddress();
            SendCancelRx(session, ip, cancelRxItemList);
        }

        public static Guid RetrieveCanceledById(Guid loggedInUserId)
        {
            return loggedInUserId;
        }

        public static Guid RetrieveCanceledByProviderId(Constants.UserCategory userCategory, Guid loggedInUserGuid, Guid delegateProviderGuid)
        {
            if (userCategory == Constants.UserCategory.PROVIDER ||
                userCategory == Constants.UserCategory.PHYSICIAN_ASSISTANT ||
                userCategory == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                return loggedInUserGuid;
            }
            else
            {
                return delegateProviderGuid;
            }
        }

        public static Guid RetrieveCanceledBySupervisingProviderId(Constants.UserCategory userCategory, Guid delegateProviderGuid, Guid supervisingProviderGuid)
        {
            if (userCategory == Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                return delegateProviderGuid;
            }
            else
            {
                return supervisingProviderGuid;// For non POB users, this is null
            }
            
        }
        public static void SendCancelRx(IStateContainer session, string ip, IEnumerable<SendCancelRxRequestModel> cancelRxItemList)
        {
            string cancelRxScriptMessageId = string.Empty;
            var dbId = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            var userId = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            var licenseId = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            var patientId = session.GetStringOrEmpty(Constants.SessionVariables.PatientId);
            var siteId = Convert.ToInt32(session.GetStringOrEmpty(Constants.SessionVariables.SiteId));
            var canceledById = RetrieveCanceledById(session.GetGuidOr0x0(Constants.SessionVariables.UserId));
            var canceledByProviderId = RetrieveCanceledByProviderId((Constants.UserCategory)session[Constants.SessionVariables.UserType], session.GetGuidOr0x0(Constants.SessionVariables.UserId), session.GetGuidOr0x0(Constants.SessionVariables.DelegateProviderId));
            var canceledBySupervisingProviderId = RetrieveCanceledBySupervisingProviderId(
                AppCode.StateUtils.UserInfo.GetSessionUserType(session),
                session.GetGuidOr0x0(Constants.SessionVariables.DelegateProviderId),
                session.GetGuidOr0x0(Constants.SessionVariables.SupervisingProviderId));
            

            foreach (var rx in cancelRxItemList)
            {
                try
                {
                    using (var timer = logger.StartTimer("SendCancelRx"))
                    {
                        eRxWeb.AppCode.CancelRx.ProcessCancelRx(
                            new ScriptMessage(),
                            new Allscripts.Impact.EPSSupport.EPSBroker(), 
                            rx,
                            ip,
                            licenseId,
                            userId,
                            patientId,
                            siteId,
                            canceledById,
                            canceledByProviderId,
                            canceledBySupervisingProviderId,
                            dbId);
                        timer.Message = $"<dbid>{dbId}</dbid><cancelRxScriptMessageId>{cancelRxScriptMessageId}</cancelRxScriptMessageId>";
                    }
                }
                catch (Exception ex)
                {
                    var exMsg = $"Exception while attempting to send cancelRx. <originalNewRxId>{rx.RxID}</originalNewRxId><CancelScriptMessageId>{cancelRxScriptMessageId}</CancelScriptMessageId><Exception>{ex.ToString()}</Exception>";
                    Audit.AddException(userId, licenseId, exMsg, ip, null, null, dbId);
                    logger.Error(exMsg);
                }
            }
        }
    }
}