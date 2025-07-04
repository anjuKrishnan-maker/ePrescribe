using eRxWeb.AppCode;
using eRxWeb.AppCode.Registration;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.Data.Registration;
using System.Web.Security;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;

namespace eRxWeb
{
    public partial class Register : Page
    {
        const string DEFAULT_APP_NAME = "Veradigm - ePrescribe";
        const string DEFAULT_STYLE_PATH = @"\Style\AllscriptsStyle.css";
        public RegistrationPricingStructure registrationPricingStructure;
        public string AppName { get; set; } = DEFAULT_APP_NAME;

        IStateContainer pageState;
        public IStateContainer PageState
        {
            get
            {
                if (pageState == null)
                    pageState = new StateContainer(HttpContext.Current.Session);
                return pageState;
            }
            set
            {
                pageState = value;
            }
        }

        public string Version
        {
            get
            {
                IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
                return ApiHelper.SessionAppVersion(requestState);
            }
        }

        public bool IsDevelopmentMode { get; } = ConfigurationManager.AppSettings["DevelopmentMode"].AsBool(false);
        public string Login { get; } = FormsAuthentication.LoginUrl;
        public string RegistrantMediator { get; } = PageNames.REGISTRANT_MEDIATOR;
        public string TwoNUserMediator { get; } = PageNames.TWO_N_USER_MEDIATOR;
        public string Logout { get; } = PageNames.LOGOUT;
        public string SupportMailAddress { get; } = Allscripts.Impact.ConfigKeys.VeradigmSupportMailAddress;


        protected void Page_Init(object sender, EventArgs e)
        {
            
            var regData = new RegistrationData();
            string enterpriseClientIDFromUrl = Request.Params.Get("eid");
            registrationPricingStructure = new Registration().SetupRegistrationPricingStructures(regData, new ProductPrice(), enterpriseClientIDFromUrl);
            partnerStyle.Href = DEFAULT_STYLE_PATH;

            
            string enterpriseClientIDFromSession = PageState.GetStringOrEmpty(SessionVariables.RegistrationEnterpriseClientID);

            if (!string.IsNullOrWhiteSpace(enterpriseClientIDFromSession) || !string.IsNullOrWhiteSpace(enterpriseClientIDFromUrl))
            {
                if (!string.IsNullOrWhiteSpace(enterpriseClientIDFromUrl))
                {
                    PageState[SessionVariables.RegistrationEnterpriseClientID] = enterpriseClientIDFromUrl.Trim();                   
                }
                var partnerInfo = new PartnerRegistration().GetPartnerEnterpriseInfo(PageState.GetStringOrEmpty(SessionVariables.RegistrationEnterpriseClientID), regData);
                partnerStyle.Href = string.IsNullOrWhiteSpace(partnerInfo.StylePath) ? DEFAULT_STYLE_PATH :
                    (partnerInfo.StylePath.ToLower().Contains("newco") ? DEFAULT_STYLE_PATH : partnerInfo.StylePath); //Ideally remove this hardcoding by fixing the enterprise client config of newco with allscripts
                partnerStyle.Href = partnerStyle.Href + "?v=" + ApiHelper.SessionAppVersion(PageState); //Cache Busting
                AppName = string.IsNullOrWhiteSpace(partnerInfo.ProductName) ? DEFAULT_APP_NAME : partnerInfo.ProductName;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private string GetCspURL()
        {
            return $"{Allscripts.Impact.ConfigKeys.CSPLoginUrl.Trim()}&ReturnURL={ConfigurationManager.AppSettings["appurl"]?.Trim()}/register/createlicense";
        }

    }
}