using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.ServerModel;

namespace eRxWeb.AppCode.Users
{
    public static class UserActivationInfoModelHelper
    {
        public static UserActivationInfoModel CreateUserInforActiavationModel(string activationCode, string firstName, string lastName, string personalEmail, string appUrl)
        {
            var model = new UserActivationInfoModel
            {
                ActivationCode = activationCode,
                FirstName = firstName,
                LastName = lastName,
                PersonalEmail = personalEmail,
                RegistrationUrl = GetRegistrationUrl(appUrl)
            };
            return model;
        }
        
        public static string GetRegistrationUrl(string appUrl)
        {
            return string.Concat(appUrl.Trim(), "/register/activateuser");
        }
    }
}