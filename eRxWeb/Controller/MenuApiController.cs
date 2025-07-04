using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact;
using Allscripts.Impact.Utilities;

namespace eRxWeb.Controllers
{
    public partial class MenuApiController : ApiController
    {
        const string IDENTIFY = "?From=Menu";
        private static ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetMenu()
        {
            using (var timer = logger.StartTimer("GetMenu"))
            {
                IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
                List<MenuItem> menuItems = new List<MenuItem>();
                var response = new ApiResponse();
                try
                {

                    menuItems = PopulateMenuItmes(requestState, new ConfigurationManager(), new MenuApiHelper());
                    var menuItemsData = string.Join(",", menuItems.Select(u => " Name: " + u.Name + " NavigationUrl: " + u.NavigationUrl + " Order: " + u.Order));
                    response.Payload = menuItems;

                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), requestState);
                    logger.Error("GetMenu Exception: " + ex.ToString());

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
        internal static List<MenuItem> PopulateMenuItmes(IStateContainer session)
        {
            return PopulateMenuItmes(session, new ConfigurationManager(), new MenuApiHelper());
        }
        internal static List<MenuItem> PopulateMenuItmes(IStateContainer session, IConfigurationManager configManager, IMenuApiHelper menuHelper)
        {
            List<MenuItem> menuItems = new List<MenuItem>();
            var userType = session.Cast("UserType", Constants.UserCategory.GENERAL_USER);
            var license = session.Cast(Constants.SessionVariables.License, new ApplicationLicense());
            var ssoMode = session.GetStringOrEmpty("SSOMode");
            var isMyErxVisible = Convert.ToBoolean(configManager.GetValue("IsMyErxTabVisible") ?? "false");
            var isAdmin = session.GetBooleanOrFalse(Constants.SessionVariables.IsAdmin);
            var sessionLicense = session.Cast(Constants.SessionVariables.SessionLicense,
                                             default(ApplicationLicense));
            bool isEnterpriseEpcsApplyToLicense = EPCSWorkflowUtils.IsEnterpriseEpcsLicense(sessionLicense,
                       session.GetBooleanOrFalse(Constants.SessionVariables.IsEnterpriseEpcsEnabled));
            bool isLicenseEPCSPurchased = EPCSWorkflowUtils.IsLicenseEpcsPurchased(sessionLicense);
            bool isAdvertiseDeluxe = sessionLicense.AdvertiseDeluxe;
            switch (ssoMode)
            {
                case Constants.SSOMode.UTILITYMODE:
                    menuItems.AddNotNull(CreateMyErxTab(isMyErxVisible, 1));
                    menuItems.AddNotNull(CreateReportsTab(2));
                    menuItems.AddNotNull(CreateSettingsTab(isAdmin, 3));
                    menuItems.AddNotNull(CreateGetEPCSTab(isAdvertiseDeluxe, isAdmin, isEnterpriseEpcsApplyToLicense, isLicenseEPCSPurchased));
                    break;

                case Constants.SSOMode.TASKMODE:
                case Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE:
                    menuItems.AddNotNull(CreateTasksTab(userType, session, menuHelper));
                    menuItems.AddNotNull(CreateReportsTab());
                    menuItems.AddNotNull(CreateSettingsTab(isAdmin));
                    menuItems.AddNotNull(CreateToolsTab(license.EnterpriseClient.ShowIntegrationSolutions));
                    menuItems.AddNotNull(CreateLibraryTab(license.GetFeatureStatus(Constants.DeluxeFeatureType.iFC)));
                    menuItems.AddNotNull(CreateMyErxTab(isMyErxVisible));
                    menuItems.AddNotNull(CreateGetEPCSTab(isAdvertiseDeluxe, isAdmin, isEnterpriseEpcsApplyToLicense, isLicenseEPCSPurchased));
                    break;

                case Constants.SSOMode.PATIENTLOCKDOWNMODE:
                    menuItems.AddNotNull(CreatePatientTab(ssoMode));
                    menuItems.AddNotNull(CreateTasksTab(userType, session, menuHelper));
                    menuItems.AddNotNull(CreateMyErxTab(isMyErxVisible));
                    break;

                default:
                    menuItems.AddNotNull(CreatePatientTab(ssoMode));
                    menuItems.AddNotNull(CreateTasksTab(userType, session, menuHelper));
                    menuItems.AddNotNull(CreateReportsTab());
                    menuItems.AddNotNull(CreateSettingsTab(isAdmin));
                    menuItems.AddNotNull(CreateToolsTab(license.EnterpriseClient.ShowIntegrationSolutions));
                    menuItems.AddNotNull(CreateLibraryTab(license.GetFeatureStatus(Constants.DeluxeFeatureType.iFC)));
                    menuItems.AddNotNull(CreateMyErxTab(isMyErxVisible));
                    menuItems.AddNotNull(CreateGetEPCSTab(isAdvertiseDeluxe, isAdmin, isEnterpriseEpcsApplyToLicense, isLicenseEPCSPurchased));
                    break;
            }
           
            
            return menuItems;
        }
        internal static MenuItem CreateGetEPCSTab(bool isAdvertiseDeluxe, bool isAdmin,bool isEnterpriseEpcsApplyToLicense,bool isLicenseEPCSPurchased)
        {
            bool bEPCSPurchase = !(isEnterpriseEpcsApplyToLicense || isLicenseEPCSPurchased);
            bool bDisplayGetEpcs = isAdvertiseDeluxe && isAdmin && bEPCSPurchase;
            return bDisplayGetEpcs ? new MenuItem { Name = Constants.TabName.GET_EPCS, NavigationUrl = Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=DeluxeFeatureSelectionComponent" + IDENTIFY, Order = 8 } : null;
            
        }
        public static MenuItem CreatePatientTab(string ssoMode)
        {
            return new MenuItem { Name = "Patient", Order = 1 };
        }

        public static MenuItem CreateTasksTab(Constants.UserCategory userType, IStateContainer session, IMenuApiHelper helper)
        {
            var taskCount = helper.GetTaskCount(session.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                session.GetStringOrEmpty(Constants.SessionVariables.UserId),
                session.GetStringOrEmpty(Constants.SessionVariables.PatientId),
                session.GetStringOrEmpty(Constants.SessionVariables.SSOMode),
                ApiHelper.GetDBID(session),
                ApiHelper.GetSessionUserID(session), session);

            var mi = new MenuItem { Name = string.IsNullOrWhiteSpace(taskCount) ? "Tasks" : $"Tasks({taskCount})", Order = 2 };

            switch (userType)
            {
                case Constants.UserCategory.GENERAL_USER:
                case Constants.UserCategory.POB_LIMITED:
                    mi = null;
                    break;

                case Constants.UserCategory.POB_SUPER:
                case Constants.UserCategory.POB_REGULAR:
                    mi.NavigationUrl = Constants.PageNames.LIST_SEND_SCRIPTS;
                    break;

                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                case Constants.UserCategory.PROVIDER:
                    mi.NavigationUrl = Constants.PageNames.DOC_REFILL_MENU;
                    break;
            }

            return mi;
        }

        public static MenuItem CreateLibraryTab(Constants.DeluxeFeatureStatus featureStatus)
        {
            if (featureStatus == Constants.DeluxeFeatureStatus.On
                || featureStatus == Constants.DeluxeFeatureStatus.Disabled
                || featureStatus == Constants.DeluxeFeatureStatus.Off)
            {
                return new MenuItem { Name = "Library", NavigationUrl = Constants.PageNames.LIBRARY, Order = 3 };
            }

            return null;
        }

        public static MenuItem CreateReportsTab(int index = 4)
        {
            return new MenuItem { Name = "Reports", NavigationUrl = Constants.PageNames.REPORTS, Order = index };
        }

        public static MenuItem CreateSettingsTab(bool isAdmin, int index = 5)
        {
            return isAdmin ? new MenuItem { Name = "Settings", NavigationUrl = Constants.PageNames.SETTINGS, Order = index } : null;
        }

        public static MenuItem CreateToolsTab(bool showIntegrationSolutions)
        {
            return showIntegrationSolutions ? new MenuItem { Name = Constants.TabName.MANAGE_ACCOUNT, NavigationUrl = Constants.PageNames.INTEGRATION_SOLUTIONS_LIST, Order = 6 } : null;
        }

        public static MenuItem CreateMyErxTab(bool isMyErxVisible, int index = 7)
        {
            return isMyErxVisible ? new MenuItem { Name = "My eRx", NavigationUrl = Constants.PageNames.MY_PROFILE, Order = index } : null;
        }
    }
}