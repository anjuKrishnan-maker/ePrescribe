using eRxWeb.ePrescribeSvc;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Registration;
using eRxWeb.State;
using Allscripts.ePrescribe.Common;
using System;

namespace eRxWeb.AppCode.Registration
{
    public class RegistrationUser
    {
        private  ActivateAndUpdateUserRequest packageUpUserProfile(IStateContainer pageState, ServerModel.Registration.CreateUserRequest newUserActivationInfo)
        {
            var request = new ActivateAndUpdateUserRequest();
            request.UserName = newUserActivationInfo.UserName;
            request.FirstName = newUserActivationInfo.FirstName;
            request.MiddleName = newUserActivationInfo.MiddleName;
            request.LastName = newUserActivationInfo.LastName;
            request.EmailAddress = newUserActivationInfo.PersonalEmail;
            request.Password = newUserActivationInfo.Password;
            request.ActivationCode = pageState.GetStringOrEmpty(Constants.SessionVariables.ActivationCode);
            request.SecretQuestions = newUserActivationInfo.secretQuestionsField;
            return request;
        }


        public bool IsCaptchaValid(IStateContainer pageState, string captchaText)
        {
            return pageState.GetStringOrEmpty(Constants.SessionVariables.CaptchaText).Equals(captchaText, StringComparison.OrdinalIgnoreCase);
        }

        public CreateUserModel CreateUser(IStateContainer pageState , ServerModel.Registration.CreateUserRequest userInfo)
        {
            CreateUserModel createUserModel = new CreateUserModel();

            if (IsCaptchaValid(pageState, userInfo.CapchaText))
            {
                createUserModel.IsValidCaptcha = true;

                createUserModel.RedirectUrl = Constants.PageNames.LOGIN;
                var request = packageUpUserProfile(pageState, userInfo);

                Constants.Workflow workflow = pageState.Cast(Constants.SessionVariables.ActivationWorkflow, Constants.Workflow.None);

                if (workflow == Constants.Workflow.Utilities)
                {
                    ActivateAndUpdateUserResponse responseActivate = EPSBroker.ActivateAndUpdateUtilitiesUser(request);
                    if (responseActivate.Success)
                    {
                        pageState.Remove(Constants.SessionVariables.ActivationCode);
                        createUserModel.IsDataSaved = true;
                        createUserModel.RedirectUrl = $"{Constants.PageNames.LOGOUT}?msg=Utilities user successfully created";
                    }
                    else
                    { 
                        throw new Exception(String.Join(" ", responseActivate.Messages)); 
                    }
                }
                else
                {
                    var responseActivate = EPSBroker.ActivateAndUpdateShield(request);
                    if (responseActivate.Success)
                    {
                        pageState.Remove(Constants.SessionVariables.ActivationCode);
                        createUserModel.IsDataSaved = true;
                        if (workflow == Constants.Workflow.Main)
                        {
                            new ApiAuthenticationProcessor().SetUserAuthenticationContext(userInfo.UserName, userInfo.Password);
                            createUserModel.RedirectUrl = Constants.PageNames.TWO_N_USER_MEDIATOR;
                        }
                        else if (workflow == Constants.Workflow.Sso)
                        {
                            createUserModel.RedirectUrl = $"{Constants.PageNames.LOGOUT}?SSOActivationSuccess=true";
                        }
                    }
                    else
                    {
                        throw new Exception(String.Join(" ", responseActivate.Messages));
                    }
                }
            }
        
            return createUserModel;
        }

    }
}
