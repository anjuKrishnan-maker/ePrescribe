using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Common;
using System.Threading;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using eRxWeb.ServerModel.Request;

namespace eRxWeb.Controllers
{
    public partial class RobustLinkApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetRobustLinks(MedicationSelectedRequest Med)
        {
            using (var timer = logger.StartTimer("GetRobustLinks"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                        response.Payload = GetRobustLinksData(Med, pageState);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetRobustLinks Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request><DDI>{0}</DDI><FormularyStatus>{1}</FormularyStatus></Request><Response>{2}</Response>", Med.DDI, Med.FormularyStatus.ToString(), response.ToLogString());
                return response;
            }
        }

        internal static List<SponsoredLinkModel> GetRobustLinksData(MedicationSelectedRequest med, IStateContainer pageState)
        {
            var retrunData = new List<SponsoredLinkModel>();

            if (ApiHelper.ShouldDisplayInfoScripts(pageState))
            {
                List<SponsoredLink> sponLinks = GetLinkData(pageState, med.DDI, med.FormularyStatus);

                RxUser user = GetCurrentUser(pageState);
                string userState = ApiHelper.GetSessionPracticeState(pageState);
                string userSpecialty = string.Empty;
                if (user.GetAttribute("Specialty_CD") != null)
                    userSpecialty = user.GetAttribute("Specialty_CD").ToString();


                foreach (SponsoredLink sponLink in sponLinks)
                {
                    List<string> sponsoredSpecialitiesList = GetSponsoredSpecialisties(sponLink);

                    List<string> listState = GetSponsoredLinkStates(sponLink);
                    if (SatisfyVisibliltyCondition(pageState, listState, sponsoredSpecialitiesList, userState, userSpecialty))
                    {
                        var model = new SponsoredLinkModel();
                        model.MessageId = sponLink.MessageID;
                        model.MessageLines = sponLink.MessageLines as List<SponsoredLinkMessageLines>;
                        retrunData.Add(model);

                        SponsoredLink.InsertSponsoredLinkAuditLog(sponLink.MessageAuditID, Constants.SponsoredLinkLogType.SHOW, user.UserID);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(updateCounter), sponLink.MessageID);
                    }
                }
            }

            return retrunData;
        }

        private static void updateCounter(object messageID)
        {
            SponsoredLink.UpdateSponsoredLinks((int)messageID);
        }
        private static List<SponsoredLink> GetLinkData(IStateContainer pageState, string linkItemId, int formularyStatus)
        {
            return new Allscripts.Impact.SponsoredLink().LoadSponsoredLinks(linkItemId, 0,
                 pageState.Cast<DataSet>("FormularyAltData", null), pageState.GetStringOrEmpty("OTCCoverage"),
                 pageState.GetStringOrEmpty("GenericDrugPolicy"), pageState.GetStringOrEmpty("UnListedDrugPolicy"),
                 pageState.GetStringOrEmpty("FormularyID"), pageState.GetStringOrEmpty("SelectedCoverageID"),
                 formularyStatus, pageState.GetStringOrEmpty("PlanID"),
                 pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1)
                 );
        }

        private static bool SatisfyVisibliltyCondition(IStateContainer pageState, List<string> listState, List<string> sponsoredSpecialitiesList, string userState, string userSpecialty)
        {
            var visible = true;

            if (listState.Contains(userState))
                visible = false;
            if (!string.IsNullOrEmpty(userSpecialty) && (sponsoredSpecialitiesList.Count != 0) &&
                (!sponsoredSpecialitiesList.Contains(userSpecialty)))
                visible = false;
            if (!string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("REFREQ")))
                visible = false;
            return visible;
        }
        private static List<string> GetSponsoredLinkStates(SponsoredLink sponLink)
        {
            DataSet dsStates = Allscripts.Impact.SponsoredLink.LoadSponsoredLinkStates(sponLink.MessageID);
            List<string> listState = new List<string>();
            foreach (DataRow row in dsStates.Tables[0].Rows)
            {
                listState.Add(row["State"].ToString());
            }
            return listState;
        }

        private static List<string> GetSponsoredSpecialisties(SponsoredLink sponLink)
        {
            DataSet dsSpecialties = Allscripts.Impact.SponsoredLink.LoadSponsoredLinkSpecialties(sponLink.MessageID);
            List<string> list = new List<string>();
            foreach (DataRow row in dsSpecialties.Tables[0].Rows)
            {
                list.Add(row["Specialty_CD"].ToString());
            }
            return list;
        }

        private static RxUser GetCurrentUser(IStateContainer pageState)
        {

            RxUser currentUser;

            if (pageState.Cast<RxIdentity>("UserIdentity", null) != null)
            {
                currentUser = (pageState.Cast<RxIdentity>("UserIdentity", null)).User;
            }
            else
            {
                currentUser = new RxUser(ApiHelper.GetSessionUserID(pageState), ApiHelper.GetDBID(pageState));
            }

            return currentUser;

        }
    }

}