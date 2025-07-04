using System;
using System.Web;
using System.Web.Http;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;

namespace eRxWeb.Controller
{
    public partial class WelcomeTourApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse WelcomeTourDoNotShowAgain(WelcomeTourDoNotShowAgainRequest request)
        {
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            return DoNotShowAgain(request.TourType, session);
        }

        private ApiResponse DoNotShowAgain(int tourType, IStateContainer session)
        {
            using (SelfStoppingTimer timer = logger.StartTimer("Welcome Tour - Do Not Show Again"))
            {
                ApiResponse response = new ApiResponse();

                Constants.WelcomeTourType wtt = (Constants.WelcomeTourType)(tourType);
                if (session.GetStringOrEmpty("USERID") != string.Empty)
                {
                    Guid userId = new Guid(session.GetStringOrEmpty("USERID"));
                    if (wtt == Constants.WelcomeTourType.NewUser)
                    {
                        EPSBroker.AddMessageTrackingAck(new Guid("d7687d09-07ea-458d-8546-97d6c195f89d"), userId);
                    } else if (wtt == Constants.WelcomeTourType.NewRelease)
                    {
                        EPSBroker.AddMessageTrackingAck(new Guid("7593cd82-5b81-4ede-a80e-05a6223f2cc4"), userId);
                    }
                }

                return response;
            }
        }
    }
}