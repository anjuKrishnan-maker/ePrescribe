using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.Data.CommonComponent;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects.CommonComponent;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Allscripts.ePrescribe.Common.Constants;
using ConfigKeys = Allscripts.Impact.ConfigKeys;

namespace eRxWeb
{
    public partial class PptDetails : BasePage
    {
        protected string hiddenAccountID;
        protected string hiddenApplicationName;
        protected string hiddenApplicationVersion;

        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                try
                {
                    dtlFrmToBePosted.Style["display"] = "Inline";
                    dtlFrmToBePosted.Action = ConfigKeys.PptPlusCommonUIEndpoint;

                    var siteInfo = PageState.Cast(Constants.SessionVariables.CommonCompSiteInfo, default(SiteInfo));
                    var pptPlusSaml = PPTPlus.GetPptPlusSamlToken(PageState);
                    var pptPlusResponse = PageState.Cast(Constants.SessionVariables.PptPlusResponses, default(PptPlusResponseContainer));
                    var dbId = PageState.Cast(Constants.SessionVariables.DbId, default(ConnectionStringPointer));
                    var userId = PageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
                    var patientId = PageState.GetStringOrEmpty(Constants.SessionVariables.PatientId);


                    string saml = pptPlusSaml.RawSamlToken;
                    string[] fhirArray = PptPlus.GetArrangedFhirArray(pptPlusResponse);
                    CompressedSAMLFhir.Value = PptPlus.CombineAndCompressSamlFhirs(saml, fhirArray);

                    var bytes = Encoding.ASCII.GetBytes(PPTPlus.GetCommonHints(PageState, new PptPlus()));
                    this.CommonUIHints.Value = Convert.ToBase64String(bytes);

                    hiddenAccountID = siteInfo.AccountId;
                    hiddenApplicationName = "ePrescribe";
                    hiddenApplicationVersion = PageState.GetStringOrEmpty("EPRESCRIBE_APP_VERSION");

                    CommonComponentData commonComponentData = new CommonComponentData();
                    var commonUIRequestId = commonComponentData.InsertCommonUIRequest(CompressedSAMLFhir.Value,this.CommonUIHints.Value, Convert.ToInt32(Constants.CommonUiRequestTypes.PPT), userId, patientId, dbId);
                    PageState[Constants.SessionVariables.PptPlusCommonUiRequestId] = commonUIRequestId;
                    PPTPlus.UpdateDetailShowStatus(PageState);
                }
                catch (Exception ex)
                {
                    logger.Debug($"PPTDetails+ <PPTDetailsFormPostValues>{ex}</PPTDetailsFormPostValues>");
                }
            }
        }

    }
}