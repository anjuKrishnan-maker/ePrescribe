using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using eRxWeb.AppCode.AngularHelpers;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Users
{
    public static class UserCredentialsModelHelper
    {
        public static UserCredentialsModel SetUserCredentialsModel(ePrescribeSvc.RxUser rxUser, UserMode userMode, IStateContainer session, ConnectionStringPointer dbID)
        {
            var model = new UserCredentialsModel();

            if (userMode != UserMode.AddOtherUser && rxUser != null)
            {
                model.Title = rxUser.Title;
                model.Suffix = rxUser.Suffix;
                model.NPI = rxUser.NPI;
                model.Assmca = rxUser.GenericLicense;
                model.Specialty1 = rxUser.SpecialtyCode1.TrimIfAllowed();
                model.Specialty2 = rxUser.SpecialtyCode2.TrimIfAllowed();               
            }
            model.IsPRSite = session.GetStringOrEmpty(Constants.SessionVariables.PracticeState) == Constants.PeurtoRicoStateCode;            
            DataSet specialityDataSet = PaI.GetSpecialtyList(dbID);
            model.SpecialtyList = ConvertSpecialtyDataTableToList(specialityDataSet?.Tables[0]);
            return model;
        }

        public static void LoadPrescriberInfoToRxUser(ePrescribeSvc.RxUser user, UserCredentialsModel model, Allscripts.ePrescribe.Shared.Logging.ILoggerEx logger)
        {
            logger.Debug("LoadPrescriberInfoToRxUser() - Edit User");
            
            user.Title = model.Title.TrimIfAllowed();
            user.Suffix = model.Suffix.TrimIfAllowed();
            user.NPI = model.NPI.TrimIfAllowed();
            user.GenericLicense = model.Assmca.TrimIfAllowed();

            user.SpecialtyCode1 = model.Specialty1;
            user.SpecialtyCode2 = model.Specialty2 == "none" ? string.Empty : model.Specialty2;
        }

        public static List<DropDownListElement> ConvertSpecialtyDataTableToList(DataTable dtSpecialty)
        {
            var specialtyList = new List<DropDownListElement>();
            specialtyList.Add(new DropDownListElement
            {
                Value = string.Empty,
                Description = "-- Select a Specialty --"
            });
            foreach (DataRow row in dtSpecialty.Rows)
            {
                specialtyList.Add(new DropDownListElement
                {
                    Value = row["Specialty_CD"].ToString(),
                    Description = row["Specialty"].ToString()
                });
            }
            return specialtyList;
        }
    }
}