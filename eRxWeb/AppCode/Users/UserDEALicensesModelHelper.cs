using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.State;
using static Allscripts.ePrescribe.Common.Constants;
using eRxWeb.ServerModel;

namespace eRxWeb.AppCode.Users
{
    public static class UserDEALicensesModelHelper
    {

        public static UserDEALicensesModel SetUserDeaLicensesModel(IStateContainer session, UserMode userMode, ePrescribeSvc.RxUser rxUser)
        {
            UserDEALicensesModel model = new UserDEALicensesModel();
            UserDEALicense userDEALicense = new UserDEALicense();

            if (userMode != UserMode.AddOtherUser)
            {
                model.DEALicenses = userDEALicense.GetDEALicenses(session, rxUser.UserID);
                model.UserID = rxUser.UserID;
            }
            model.UserMode = userMode;
            return model;
        }
    }
}