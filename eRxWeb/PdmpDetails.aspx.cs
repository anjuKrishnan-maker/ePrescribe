using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.ePrescribe.Data.PDMP;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using Allscripts.Impact.PdmpBPL;
using eRxWeb.AppCode.PdmpBPL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb
{
    public partial class PdmpDetails : BasePage
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                    dtlFrmToBePosted.Style["display"] = "Inline";
                    dtlFrmToBePosted.Action = ConfigKeys.PdmpCommonUiEndpoint + "/Details";
                    
                    var dbId = PageState.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
                    var userId = PageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    var patientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId);

                    var pdmpPlusSaml = PDMP.GetPDMPSamlToken(PageState);
                    string saml = pdmpPlusSaml.RawSamlToken;
                    string pdmpReportRequest = PDMP.GetPdmpReportRequest(PageState, new Pdmp());
                    string commonUIHint = Pdmp.GetStaticCommonUiHints();
                    string compressePayload = Pdmp.CombineAndCompressSamlFhirUiHint(saml, pdmpReportRequest, commonUIHint);
                    CompressedSamlPayload.Value = compressePayload;

                    CommonComponentData commonComponentData = new CommonComponentData();
                    var commonUIRequestId = commonComponentData.InsertCommonUIRequest(pdmpReportRequest, string.Empty, Convert.ToInt32(Constants.CommonUiRequestTypes.PDMP), userId, patientId, dbId);
                    PageState[Constants.SessionVariables.PdmpCommonUiRequestId] = commonUIRequestId;
                    PDMP.InsertReportViewStatus(PageState, new PdmpData());
                }
                catch (Exception ex)
                {
                    logger.Debug($"PDMPDetails <PDMPCommonUIException>{ex}</PDMPCommonUIException>");
                }
            }
        }
    }
}