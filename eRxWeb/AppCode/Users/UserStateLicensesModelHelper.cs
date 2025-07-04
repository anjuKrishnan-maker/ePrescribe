using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.ServerModel;
using eRxWeb.State;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.Users
{
    public static class UserStateLicensesModelHelper
    {
        public static UserStateLicensesModel SetUserStateLicensesModel(IStateContainer session, UserMode userMode, ePrescribeSvc.RxUser rxUser, string practiceState)
        {
            UserStateLicensesModel model = new UserStateLicensesModel();
            UserStateLicense userStateLicense = new UserStateLicense();
            if (userMode != UserMode.AddOtherUser && rxUser != null)
            {
                model.UserStateLicenses = userStateLicense.GetStateLicenses(session, rxUser.UserID);
                model.UserID = rxUser.UserID;
                model.UserType = (UserCategory)rxUser.UserType;
            }
            model.StatesAndLicenseTypes = userStateLicense.GetStatesAndLicenseTypes(session);
            model.UserMode = userMode;
            model.PracticeState = practiceState;
            return model;
        }

    }
}