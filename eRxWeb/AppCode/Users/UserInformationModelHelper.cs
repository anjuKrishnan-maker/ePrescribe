using Allscripts.ePrescribe.Common;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Users
{
    public static class UserInformationModelHelper
    {
        public static UserInformationModel SetUserInformationModel(ePrescribeSvc.RxUser rxUser, UserMode userMode)
        {
            var model = new UserInformationModel();

            if (userMode != UserMode.AddOtherUser && rxUser != null)
            {
                model.FirstName = rxUser.FirstName;
                model.LastName = rxUser.LastName;
                model.MI = rxUser.MiddleName ?? rxUser.MiddleInitial;
                model.IsForcePasswordChange = false;
                model.IsActive = rxUser.Active;
                model.IsLocked = false;
                model.LoginID = rxUser.ShieldUserName;
                model.WorkEmail = rxUser.Email;                
            }
            else if (userMode == UserMode.AddOtherUser)
            {
                model.IsActive = true;
            }

            if(userMode == UserMode.EditOtherUser)
            {
                model.IsEditedBySomeoneElse = true;
            }

            model.IsDisplayLoginID = CalculateIfLoginIDShouldBeDisplayed(userMode);            
            return model;
        }

        public static bool CalculateIfLoginIDShouldBeDisplayed(UserMode userMode)
        {
            return userMode == UserMode.SelfEdit || userMode == UserMode.EditOtherUser;
        }        

        public static ePrescribeSvc.RxUser SetRxUserFromUserInformationModel(UserInformationModel model)
        {
            var user = new ePrescribeSvc.RxUser
            {
                FirstName = model.FirstName.TrimIfAllowed(),
                LastName = model.LastName.TrimIfAllowed(),
                MiddleName = model.MI?.TrimIfAllowed(),
                Email = model.WorkEmail.TrimIfAllowed(),
                DefaultFaxSiteID = 1,
                DefaultMobileSiteID = 1
            };

            return user;
        }
    }
}