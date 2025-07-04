using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects.Registrant;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Security;
using Allscripts.ePrescribe.Data;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.AppCode.Interfaces;
using GetUserShieldCspStatusInfoResponse = eRxWeb.ePrescribeSvc.GetUserShieldCspStatusInfoResponse;
using System.Configuration;

namespace eRxWeb.Controller
{
    [RoutePrefix("api")]
    public class RegistrationStatusController : ApiController
    {
        [Route("registrantstatus/{setloastatus}")]
        public ApiResponse GetRegistrantStatus(bool setloastatus = false)
        {
            var response = new ApiResponse();
            try
            {
                var progressChecker = new ProgressChecker();
                RegistrantContext registrantContext = progressChecker.GetContext(setloastatus);

                if (!registrantContext.IsUserCreated && !registrantContext.IseRxUser)
                    RemoveAuthenticationContext();

                response.Payload = registrantContext;
            }
            catch(Exception ex)
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpPost]
        [Route("update-user-csp")]
        public ApiResponse UpdateUserCsp()
        {
            var response = new ApiResponse();
            var registrantContext = new RegistrantContext();

            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            try
            {
                registrantContext.RedirectUrl = ConfigurationManager.AppSettings[AppConfigVariables.AppUrl]?.Trim() + "/" + Constants.PageNames.REGISTER_WELCOME;
                if (Users.UpdateUserIdproofingStatus(session))
                {
                    RemoveAuthenticationContext();
                    registrantContext.RedirectUrl = ConfigurationManager.AppSettings[AppConfigVariables.AppUrl]?.Trim() + "/"+ Constants.PageNames.LOGOUT + "?reauthenticate=true";
                }
            }
            catch(Exception ex)
            { 
                ApiHelper.AuditException(ex.ToString(), session);
                
                // Do not worry about not updating the id proofing information. We have it covered if he logs in back to application. 
                registrantContext.RedirectUrl = ConfigurationManager.AppSettings[AppConfigVariables.AppUrl]?.Trim() + "/" + Constants.PageNames.LOGOUT + "?reauthenticate=true";
            }
            response.Payload = registrantContext;
            return response;
        }

        private void RemoveAuthenticationContext()
        {
            FormsAuthentication.SignOut();
        }
    }

    internal class ProgressChecker
    {
        public RegistrantContext GetContext(bool setLoa3Status)
        {
            RegistrantContext registrantContext = new RegistrantContext();

            //When I am here.Form auth cookie is already set.
            IStateContainer session = new StateContainer(HttpContext.Current.Session);

            if (!string.IsNullOrEmpty(session.GetStringOrEmpty(SessionVariables.UserId)))
            {
                registrantContext.IseRxUser = true;
                return registrantContext;
            }

            if (!session.ContainsKey(SessionVariables.RegistrantInfo))
                return registrantContext;
           
            RegistrantInfo registrantInfo = session.Cast(SessionVariables.RegistrantInfo, new RegistrantInfo());
            
            //And registrant info is available in the session as well.
            if (registrantInfo.RegistrantId > 0 && registrantInfo.ShieldObjectId != null && registrantInfo.ShieldObjectId != Guid.Empty)
                registrantContext.IsUserCreated = true;

            if (registrantContext.IsUserCreated)
                registrantContext.IsLOA3StatusConfirmed = GetRegistrantLoa3Status(session, registrantInfo, setLoa3Status);

            return registrantContext;
        }



        private bool GetRegistrantLoa3Status(IStateContainer session, RegistrantInfo registrantInfo, bool setLoa3Status)
        {
            //Loa3Status should be in session except for the request from the licence page.
            //And only for license page if the LOA status is not 3 neeed refreshing!.
            if (setLoa3Status && registrantInfo.LevelOfAssurance.As<int>(0) != 3)
                SetLOA3Status(session, registrantInfo);
            return registrantInfo.LevelOfAssurance.As<int>(0) == 3;
        }

        private void SetLOA3Status(IStateContainer session, RegistrantInfo registrantInfo)
        {
            string shieldIdentityToken = session.GetStringOrEmpty(SessionVariables.ShieldIdentityToken);
            string loa3Status = ReattemptLOA3fetch(registrantInfo, shieldIdentityToken);
            registrantInfo.LevelOfAssurance = loa3Status;
            //update session
            session[SessionVariables.RegistrantInfo] = registrantInfo;
        }

        private string ReattemptLOA3fetch(RegistrantInfo registrantInfo, string shieldIdentityToken)
        {
            if (string.IsNullOrWhiteSpace(shieldIdentityToken) || registrantInfo?.RegistrantId <= 0)
                return "";
            var response = EPSBroker.GetRegistrantIdProofingLevelOfAssurance(registrantInfo.RegistrantId, shieldIdentityToken);
            return response.LevelOfAssurance;
        }
    }

    internal class RegistrantContext
    {
        public bool IsUserCreated { get; set; }//Is False- redirect to create user.
        public bool IsLOA3StatusConfirmed { get; set; }//Is false then welcome & true Licence page  
        public bool IseRxUser { get; set; }
        public string RedirectUrl { get; set; }
        public string CspUrl { get; set; }
    }

}