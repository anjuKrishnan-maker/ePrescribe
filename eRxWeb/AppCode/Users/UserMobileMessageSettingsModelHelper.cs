using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.Users
{
    public static class UserMobileMessageSettingsModelHelper
    {
        public static UserMobileMessageSettingsModel SetUserMobileMessageSettingsModel(UserMode userMode, ePrescribeSvc.RxUser rxUser, bool isSmsEnabled, ConnectionStringPointer dbID)
        {
            var model = new UserMobileMessageSettingsModel();
            var dataModel = Allscripts.ePrescribe.Data.SMSAlert.GetUserMobileMessageSettings(rxUser.UserID, dbID);

            if ((userMode == UserMode.SelfEdit) && isSmsEnabled)
            {
                model.UserID = rxUser.UserID;
                model.UserType = (UserCategory)rxUser.UserType;
                model.IsSMSEnabled = isSmsEnabled;
                if (dataModel != null)
                {                    
                    model.PhoneNumber = dataModel.MobileNumber;
                    model.SmsAlertNewRxError = dataModel.IsNewRxErrorAlertEnabled;
                    model.SmsAlertRefillRxError = dataModel.IsRefillRxErrorAlertEnabled;
                    model.SmsAlertChangeRxError = dataModel.IsChangeRxErrorAlertEnabled;
                    model.SmsAlertCancelRxError = dataModel.IsCancelRxErrorAlertEnabled;
                    model.SmsAlertTransmissionError = dataModel.IsNewRxErrorAlertEnabled
                                                           || dataModel.IsRefillRxErrorAlertEnabled
                                                           || dataModel.IsChangeRxErrorAlertEnabled
                                                           || dataModel.IsCancelRxErrorAlertEnabled;

                    model.SmsAlertChangeRxRequest = dataModel.IsChangeRxRequestAlertEnabled;
                    model.SmsAlertDEALicenseExpiration = dataModel.IsDEALicenseExpirationAlertEnabled;
                    model.SmsAlertServiceUpdate = dataModel.IsServiceUpdateAlertEnabled;
                }
                model.IsNumberAvailable = !string.IsNullOrWhiteSpace(model.PhoneNumber);
                model.IsChanged = false;
                model.IsSMSAlertDailyCountReached = EPSBroker.IsSMSAlertDailyCountReached();
            }

            return model;
        }
    }
}