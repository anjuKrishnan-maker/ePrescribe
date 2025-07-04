using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Web.Http;
using eRxWeb.Controller;

namespace eRxWeb.Controllers
{
    public partial class RightBoxApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetRightBoxData()
        {
            using (var timer = logger.StartTimer("GetRightBoxData"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = PrepareRightBoxData(pageState);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetRightBoxData Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Response>{0}</Response>", response.ToLogString());
                return response;
            }
        }

        internal static RightBoxModel PrepareRightBoxData(IStateContainer pageState)
        {
            RightBoxModel data = new RightBoxModel();

            //add header if there is one
            if (IsAvailableSession("RightBoxHeaderText", pageState))
            {

                data.RightBoxHeaderText = pageState.GetStringOrEmpty("RightBoxHeaderText");
            }

            //add image if there is one
            if (IsAvailableSession("RightBoxImageURL", pageState))
            {
                data.RightBoxImageURL = pageState.GetStringOrEmpty("RightBoxImageURL");
            }

            data.RightBoxBodyText = pageState.GetStringOrEmpty("RightBoxBodyText");

            //add each of the links if there are some
            if (IsAvailableSession("RightBoxLinks", pageState) )
            {
                StringBuilder linkSB;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(pageState.GetStringOrEmpty("RightBoxLinks"));

                foreach (XmlNode linksNode in xmlDoc.ChildNodes)
                {
                    XmlNode linkNode = linksNode.SelectSingleNode("Link");

                    foreach (XmlNode n in linksNode.ChildNodes)
                    {
                        RightBoxLink lnk = new RightBoxLink();
                        linkSB = new StringBuilder();
                        
                        linkSB.Append(Constants.PageNames.PROCESS_LINK + "?clientid=");
                        linkSB.Append(pageState.GetStringOrEmpty("EnterpriseID"));
                        linkSB.Append("&&linkid=");
                        linkSB.Append(n.SelectSingleNode("linkid").InnerText);
                        linkSB.Append("&&linkurl=");
                        linkSB.Append(n.SelectSingleNode("linkurl").InnerText);
                        linkSB.Append("&&userid=");
                        linkSB.Append(pageState.GetStringOrEmpty("UserID"));
                        lnk.Url = linkSB.ToString();
                        lnk.Text = n.SelectSingleNode("linktext").InnerText;
                        data.Links.Add(lnk);
                    }
                }
            }
            return data;
        }
        private static bool IsAvailableSession(string key, IStateContainer pageState)
        {
            return pageState[key] != null && !string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty(key));
        }
    }
}
