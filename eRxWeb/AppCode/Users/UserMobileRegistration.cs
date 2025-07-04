using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb;
using eRxWeb.ePrescribeSvc;
using eRxWeb.ServerModel;
using eRxWeb.State;
using static Allscripts.ePrescribe.Common.Constants;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode.Users
{
    public class UserMobileRegistration
    {
        public ePrescribeSvcResponse SendMessageTwilio(VerificationCodeSMSModel verificationCodeSMS, IStateContainer session) 
        {
            var response = EPSBroker.SendMessageTwilio(verificationCodeSMS.phoneNumber, verificationCodeSMS.message);
            session["VerificationCode"] = verificationCodeSMS.otp;

            Allscripts.ePrescribe.Data.SMSAlert.InsertSMSAlertMessages(ApiHelper.GetSessionLicenseID(session),
                              verificationCodeSMS.userID, verificationCodeSMS.message,
                              verificationCodeSMS.phoneNumber,
                              (response.Success ? SMSAlertMessageStatus.Sent : SMSAlertMessageStatus.Error),
                              string.Join(" ", response.Messages).Trim(),
                              SMSAlertMessageType.VerificationCode,
                              ApiHelper.GetDBID(session));
            return response;
        }

        public bool SaveUserMobileNumber(string userID, string phoneNumber, ConnectionStringPointer dbId) 
        {            
            return Allscripts.ePrescribe.Data.SMSAlert.SaveUserMobileNumber(userID, phoneNumber, dbId);
        }

        public UserMobileMessageSettingsModel GetUserMobileMessageSettings(string userID, ConnectionStringPointer dbId) 
        {
            return ConvertDataModelToServerModel(Allscripts.ePrescribe.Data.SMSAlert.GetUserMobileMessageSettings(userID, dbId));
        }
                
        private UserMobileMessageSettingsModel ConvertDataModelToServerModel(Allscripts.ePrescribe.Data.Model.UserMobileMessageSettingModel dataModel)
        {
            UserMobileMessageSettingsModel model = new UserMobileMessageSettingsModel();
            model.UserID = dataModel.UserID;
            model.PhoneNumber = dataModel.MobileNumber;
            model.SmsAlertNewRxError = dataModel.IsNewRxErrorAlertEnabled;
            model.SmsAlertRefillRxError = dataModel.IsRefillRxErrorAlertEnabled;
            model.SmsAlertChangeRxError = dataModel.IsChangeRxErrorAlertEnabled;
            model.SmsAlertCancelRxError = dataModel.IsCancelRxErrorAlertEnabled;
            model.SmsAlertChangeRxRequest = dataModel.IsChangeRxRequestAlertEnabled;
            model.SmsAlertDEALicenseExpiration = dataModel.IsDEALicenseExpirationAlertEnabled;
            model.SmsAlertServiceUpdate = dataModel.IsServiceUpdateAlertEnabled;
            if (model.PhoneNumber != null)
            {
                model.IsNumberAvailable = true;
            }
            return model;
        }


        public void InsertAuditLog(IStateContainer session, VerificationCodeSMSModel verificationCodeSMS ) {
            Allscripts.ePrescribe.Data.SMSAlert.InsertAuditLog(ApiHelper.GetSessionLicenseID(session), 
                                                               verificationCodeSMS.userID, 
                                                               (SMSAlertUserActionType)verificationCodeSMS.status, 
                                                               verificationCodeSMS.phoneNumber, 
                                                               ApiHelper.GetDBID(session));
        }

        public void DeleteUserMobileNumber(string userID, ConnectionStringPointer dbId)
        {
            Allscripts.ePrescribe.Data.SMSAlert.DeleteUserMobileNumber(userID, dbId);
        }
    }
}