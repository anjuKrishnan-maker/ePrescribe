using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data.Model;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode.AngularHelpers;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.ModelBinding;
using Veradigm.AdAgent.Business.Interfaces;
using static Allscripts.ePrescribe.Common.Constants;
using SimpleResponse = Allscripts.ePrescribe.Objects.SimpleResponse;
using ObjectConstants = Allscripts.ePrescribe.Objects.ObjectConstants;
using ShieldUserStatus = Allscripts.ePrescribe.Objects.ShieldUserStatus;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;
using ShieldTraitValue = eRxWeb.ePrescribeSvc.ShieldTraitValue;
using Allscripts.ePrescribe.ExtensionMethods;

namespace eRxWeb.AppCode.Users
{
    public class User
    {
        private ConnectionStringPointer nodeDBID;
        private string editorUserID;    //User Who is Editing
        private string editeeUserID;    //User Being Edited / Can be same user, another usere, or a new user being added
        private string licenseID;
        private bool isAdmin;
        private UserMode userMode;
        private bool isSendSmsEnabled;
        private bool isEditUserEnabled;

        private bool isSsoUser;
        private bool isPartnerAllowsUserNameAndPassword;
        private bool isForcePasswordSetupForSSOUser;
        private bool isEnterpriseShowChangePassword;
        private UserMobileMessageSettingModel existingSMSSettings;

        public UserModel GetUserInitData(IStateContainer session, UserMode mode, string userID, string ipAddress, bool isNoLic)
        {
            var model = new UserModel();
            InitializeUserAttributes(session, mode, userID);

            model.IsAdmin = this.isAdmin;
            model.UserMode = this.userMode;
            model.UserID = this.editeeUserID; //empty for add user
            model.PracticeState = session.GetStringOrEmpty(Constants.SessionVariables.PracticeState);

            if (this.userMode == UserMode.AddOtherUser)
            {
                model = UpdateUserModelForAddUser(model, session);
            }
            else if (this.userMode == UserMode.EditOtherUser)
            {
                model = UpdateUserModelForEditOtherUser(model, session);
            }
            else
            {
                model = UpdateUserModelForEditSameUser(model, session);
            }

            DisablePanelsForEditUser(model, this.userMode, this.isEditUserEnabled);
            return model;
        }

        public void InitializeUserAttributes(IStateContainer session, UserMode mode, string userID)
        {
            this.licenseID = session.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            this.nodeDBID = session.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            this.editorUserID = session.GetStringOrEmpty(Constants.SessionVariables.UserId);
            this.editeeUserID = mode == UserMode.SelfEdit ? this.editorUserID : userID;
            this.isAdmin = session.GetBooleanOrFalse(Constants.SessionVariables.IsAdmin);
            this.userMode = mode;
            this.isSendSmsEnabled = Settings.IsSendSMSEnabled(session);
            this.isEditUserEnabled = Settings.GetSessionLicense(session).EnterpriseClient.EditUser;

            this.isSsoUser = session.GetBooleanOrFalse(Constants.SessionVariables.IsSSOUser);
            this.isPartnerAllowsUserNameAndPassword = session.GetBooleanOrFalse(Constants.SessionVariables.PartnerAllowsUserNameAndPassword);
            this.isForcePasswordSetupForSSOUser = session.GetBooleanOrFalse(Constants.SessionVariables.ForcePasswordSetupForSSOUser);
            this.isEnterpriseShowChangePassword = Settings.GetSessionLicense(session).EnterpriseClient.ShowChangePassword;

            if (this.isSendSmsEnabled)
            {
                this.existingSMSSettings = Allscripts.ePrescribe.Data.SMSAlert.GetUserMobileMessageSettings(this.editeeUserID, this.nodeDBID);
            }
        }

        public UserModel UpdateUserModelForAddUser(UserModel model, IStateContainer session)
        {
            model.UserInformation = UserInformationModelHelper.SetUserInformationModel(null, this.userMode);
            model.UserSecurity = UserSecurityModelHelper.SetUserSecurityModel(false, this.userMode, null, this.isSsoUser, this.isPartnerAllowsUserNameAndPassword,
                            this.isForcePasswordSetupForSSOUser, this.isEnterpriseShowChangePassword);
            model.UserSettings = UserSettingsModelHelper.SetUserSettingsModel(null, this.editeeUserID, this.licenseID, this.userMode, this.nodeDBID);
            model.UserCredentials = UserCredentialsModelHelper.SetUserCredentialsModel(null, this.userMode, session, this.nodeDBID);
            model.UserDEALicenses = UserDEALicensesModelHelper.SetUserDeaLicensesModel(session, this.userMode, null);
            model.UserStateLicenses = UserStateLicensesModelHelper.SetUserStateLicensesModel(session, this.userMode, null, model.PracticeState);
            model.UserPreferences = UserPreferencesModelHelper.SetUserPreferencesModel(null, this.editeeUserID, this.licenseID, this.userMode, this.nodeDBID);
            return model;
        }

        public UserModel UpdateUserModelForEditOtherUser(UserModel model, IStateContainer session)
        {
            ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(ePrescribeSvc.ValueType.UserGUID, this.editeeUserID, this.licenseID,
                   this.editorUserID, this.licenseID, this.nodeDBID);

            session["eRxUserName"] = getUserRes.RxUser.UserName;
            session["ShieldProfileUserID"] = getUserRes.RxUser.ShieldProfileUserID;

            model.UserInformation = UserInformationModelHelper.SetUserInformationModel(getUserRes.RxUser, this.userMode);
            model.UserSecurity = UserSecurityModelHelper.SetUserSecurityModel(this.isAdmin, this.userMode, getUserRes.RxUser, this.isSsoUser, this.isPartnerAllowsUserNameAndPassword,
                            this.isForcePasswordSetupForSSOUser, this.isEnterpriseShowChangePassword);
            model.UserSettings = UserSettingsModelHelper.SetUserSettingsModel(getUserRes.RxUser, this.editeeUserID, this.licenseID, this.userMode, this.nodeDBID);
            model.UserCredentials = UserCredentialsModelHelper.SetUserCredentialsModel(getUserRes.RxUser, this.userMode, session, this.nodeDBID);
            model.UserDEALicenses = UserDEALicensesModelHelper.SetUserDeaLicensesModel(session, this.userMode, getUserRes.RxUser);
            model.UserStateLicenses = UserStateLicensesModelHelper.SetUserStateLicensesModel(session, this.userMode, getUserRes.RxUser, model.PracticeState);
            model.UserPreferences = UserPreferencesModelHelper.SetUserPreferencesModel(getUserRes.RxUser, this.editeeUserID, this.licenseID, this.userMode, this.nodeDBID);
            model.UserEPCSSettings = UserEPCSSettingsModelHelper.SetUserEPCSSettingsModel(this.userMode, model.UserInformation, session, this.nodeDBID);
            model.UserMobileMessageSettings = UserMobileMessageSettingsModelHelper.SetUserMobileMessageSettingsModel(this.userMode, getUserRes.RxUser, this.isSendSmsEnabled, this.nodeDBID);

            return model;
        }

        public UserModel UpdateUserModelForEditSameUser(UserModel model, IStateContainer session)
        {
            //Editing own page.
            ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(ePrescribeSvc.ValueType.UserGUID, this.editeeUserID, this.licenseID,
                   this.editorUserID, this.licenseID, this.nodeDBID);
            session["eRxUserName"] = getUserRes.RxUser.UserName;
            session["ShieldProfileUserID"] = getUserRes.RxUser.ShieldProfileUserID;

            model.UserInformation = UserInformationModelHelper.SetUserInformationModel(getUserRes.RxUser, this.userMode);
            model.UserSecurity = UserSecurityModelHelper.SetUserSecurityModel(this.isAdmin, this.userMode, getUserRes.RxUser, this.isSsoUser, this.isPartnerAllowsUserNameAndPassword,
                            this.isForcePasswordSetupForSSOUser, this.isEnterpriseShowChangePassword);
            model.UserSettings = UserSettingsModelHelper.SetUserSettingsModel(getUserRes.RxUser, this.editeeUserID, this.licenseID, this.userMode, this.nodeDBID);
            model.UserCredentials = UserCredentialsModelHelper.SetUserCredentialsModel(getUserRes.RxUser, this.userMode, session, this.nodeDBID);
            model.UserDEALicenses = UserDEALicensesModelHelper.SetUserDeaLicensesModel(session, this.userMode, getUserRes.RxUser);
            model.UserStateLicenses = UserStateLicensesModelHelper.SetUserStateLicensesModel(session, this.userMode, getUserRes.RxUser, model.PracticeState);
            model.UserPreferences = UserPreferencesModelHelper.SetUserPreferencesModel(getUserRes.RxUser, this.editeeUserID, this.licenseID, this.userMode, this.nodeDBID);
            model.UserEPCSSettings = UserEPCSSettingsModelHelper.SetUserEPCSSettingsModel(this.userMode, model.UserInformation, session, this.nodeDBID);
            model.UserMobileMessageSettings = UserMobileMessageSettingsModelHelper.SetUserMobileMessageSettingsModel(this.userMode, getUserRes.RxUser, this.isSendSmsEnabled, this.nodeDBID);

            return model;
        }

        public UserModel DisablePanelsForEditUser(UserModel model, UserMode mode, bool isUserEditAllowed)
        {
            if (mode != UserMode.AddOtherUser && !isUserEditAllowed)
            {
                model.UserInformation.IsRestrictedMode = true;
                model.UserSecurity.IsRestrictedMode = true;
                model.UserSettings.IsRestrictedMode = true;
                model.UserCredentials.IsRestrictedMode = true;
                model.UserDEALicenses.IsRestrictedMode = true;
                model.UserStateLicenses.IsRestrictedMode = true;
            }
            return model;
        }

        public SaveUserResponseModel SaveUser(UserModel userRequest, IStateContainer session, string ipAddress, string appUrl, ILoggerEx logger)
        {
            InitializeUserAttributes(session, userRequest.UserMode, userRequest.UserID);
            var model = new SaveUserResponseModel
            {
                IsSaveSuccessful = false
            };

            bool isProviderOrPa = UserSettingsModelHelper.IsProviderOrPA(userRequest.UserSettings);

            if (Settings.GetSessionLicense(session).EnterpriseClient.EditUser)
            {
                saveUser(model, userRequest, isProviderOrPa, session, ipAddress, appUrl, logger);
            }
            else
            {
                SetCanApproveTrait(session, userRequest.UserEPCSSettings, userRequest.UserInformation);
                saveUserPreferences(this.editeeUserID, UserSettingsModelHelper.IsProviderOrPA(userRequest.UserSettings), userRequest.UserPreferences, session);
                saveSendSMSAlertPreference(isProviderOrPa, userRequest);
                if (this.userMode == UserMode.SelfEdit)
                {
                    model.RedirectUrl = RedirectAfterSave(session);
                }
            }

            return model;
        }

        private void saveUser(SaveUserResponseModel responseModel, UserModel userRequest, bool isProviderOrPa, IStateContainer state, string ipAddress, string appUrl, ILoggerEx logger)
        {            

            logger.Debug("saveUser() - StateLicenseCheck");

            string practiceState = state.GetStringOrEmpty(Constants.SessionVariables.PracticeState);

            if (!didUserCorrectlySelectLicense(isProviderOrPa, userRequest.UserStateLicenses.UserStateLicenses, this.editeeUserID, practiceState, this.userMode, this.nodeDBID))
            {
                string message = $"You do not have a valid state license for {practiceState}. You must add one below before you can proceed.";
                responseModel.MessageList.Add(new MessageModel(message, MessageIcon.Error, false));
                return;
            }


            if (isProviderOrPa)
            {
                logger.Debug("saveUser() - NPI Validation");
                if (!RxUser.IsValidNPI(userRequest.UserCredentials.NPI))
                {
                    responseModel.MessageList.Add(new MessageModel("NPI is Invalid!", MessageIcon.Error, false));
                    return;
                }
            }

            logger.Debug("saveUser() - StateLicenseValid");

            //Check to ensure the pob user actually selected a provider
            if (UserSettingsModelHelper.IsPOB(userRequest.UserSettings) && !UserSettingsModelHelper.HasPOBSelectedAProvider(userRequest.UserSettings))
            {
                responseModel.MessageList.Add(new MessageModel(
                    "You have not selected any providers for the user in User Settings. A Prescribe On Behalf Of (POB) user must be associated to at least one provider before you can proceed.", MessageIcon.Error, false));
                return;
            }

            logger.Debug("saveUser() - ProviderSelected");

            if (isProviderOrPa)
            {
                if (ValidatePrescriberLicenseCount(userRequest.UserStateLicenses.UserStateLicenses, responseModel, logger)) return;
            }

            var rxUser = UserInformationModelHelper.SetRxUserFromUserInformationModel(userRequest.UserInformation);

            // Get Shield Role IDs and Role Names for ePrescribe application
            var appRolesFromShield = ShieldSettings.GetShieldAppRoles(state);
            var appRoles = new List<string>();

            logger.Debug("saveUser() - AddingAppRoles");

            rxUser.UserType = (int)userRequest.UserSettings.UserType;
            UserSettingsModelHelper.AddRoleToAppRoles(appRolesFromShield, userRequest.UserSettings, appRoles);
            UserSecurityModelHelper.CheckAndAddAdminRole(appRolesFromShield.Single(r => r.Name.ToLower() == "admin"), appRoles, userRequest.UserSecurity);

            rxUser.AppRoles = appRoles.ToArray();

            logger.Debug("saveUser() - app Roles added");

            if (isProviderOrPa)
            {
                UserCredentialsModelHelper.LoadPrescriberInfoToRxUser(rxUser, userRequest.UserCredentials, logger);
                if (!userRequest.UserPreferences.IsFaxSiteInvalid)
                {
                    rxUser.DefaultFaxSiteID = int.Parse(userRequest.UserPreferences.DefaultFaxSiteID);
                }
            }

            //For sending it to save and add user method in EPS.
            rxUser.StateLicenses = LoadStateLicenses(userRequest.UserStateLicenses.UserStateLicenses, logger).ToArray();
            rxUser.DEALicenses = LoadDeaLicenses(userRequest.UserDEALicenses.DEALicenses, logger).ToArray();
            rxUser.AppRoles = appRoles.ToArray();

            if (UserSettingsModelHelper.IsPOB(userRequest.UserSettings))
            {
                rxUser.PobAuthorizedProviders = new ePrescribeSvc.PobAuthorizedProviders
                {
                    AllProviders = userRequest.UserSettings.SupervisingProviderSelectionMode == SupervisingProviderSelectionMode.All
                };
                if (!rxUser.PobAuthorizedProviders.AllProviders)
                {
                    rxUser.PobAuthorizedProviders.ProviderGuidList = getSelectedProvidersGuidList(userRequest.UserSettings.UserSupervisingProvidersList).ToArray();
                }
            }


            if (userRequest.UserMode == UserMode.AddOtherUser)
            {
                AddUser(rxUser, responseModel, userRequest, state, ipAddress, appUrl, logger);
            }
            else
            {
                logger.Debug("saveUser() - edit mode");

                //existing user
                if (this.isAdmin)
                {
                    if (!appRoles.Contains("Admin"))
                    {
                        if (EPSBroker.GetAdminCountForLicense(this.licenseID, this.nodeDBID) < 2)
                        {
                            responseModel.MessageList.Add(
                                new MessageModel("You are the only User with Administrative Permissions. You cannot remove this permission without first granting it to another User.",
                                MessageIcon.Error, false));
                            return;
                        }
                    }
                }

                //Fix the IP Address Part
                ApiHelper.AuditLogUserInsert(ePrescribeSvc.AuditAction.USER_MODIFY, this.editeeUserID, this.licenseID, this.editorUserID, ipAddress, this.nodeDBID);

                var userName = state.GetStringOrEmpty("eRxUserName");

                logger.Debug("saveUser() - got erx username {0}", userName);

                ePrescribeSvc.UpdateUserRequest updateUserRequest = EPSBroker.ServiceRequest<ePrescribeSvc.UpdateUserRequest>();

                //Save user specific details.
                rxUser.UserName = userName;
                rxUser.LicenseID = this.licenseID;
                rxUser.UserID = this.editeeUserID;
                rxUser.Active = userRequest.UserInformation.IsActive;
                rxUser.LastPasswordChange = DateTime.Today;

                rxUser.PatientReceipt = true;
                rxUser.DBID = (int)this.nodeDBID;

                logger.Debug("saveUser() - RxUser loaded");

                //Save request specific details.
                updateUserRequest.DbID = (ePrescribeSvc.ConnectionStringPointer)this.nodeDBID;
                updateUserRequest.Application = ePrescribeSvc.ApplicationEnum.ePrescribe;

                if (userRequest.UserMode == UserMode.SelfEdit)
                {
                    //When editing own account for shield enabled user.
                    updateUserRequest.AuditUserID = this.editorUserID;
                }
                updateUserRequest.ShieldInternalAppID = ShieldSettings.GetShieldInternalAppID(state);
                updateUserRequest.ShieldInternalAppInstanceID = ShieldSettings.GetShieldInternalAppInstanceID(state);
                updateUserRequest.ShieldInternalEnvironmentID = ShieldSettings.GetShieldInternalEnvironmentID(state);
                updateUserRequest.EprescribeExternalAppInstanceID = ShieldSettings.ePrescribeExternalAppInstanceId(state);
                updateUserRequest.SAMLTokenString = ShieldInfo.GetShieldSecurityToken(state);
                updateUserRequest.RxUser = rxUser;
                updateUserRequest.IsDEALicenseUpdated = true;
                updateUserRequest.IsStateLicenseUpdated = true;

                updateUserRequest.ShieldLoginID = userRequest.UserInformation.LoginID;
                updateUserRequest.ExternalTenantID = ShieldSettings.GetShieldExternalTenantID(state);

                if (state["ShieldADUserID"] != null)
                {
                    updateUserRequest.ShieldADUserID = state["ShieldADUserID"].ToString();
                }

                if (state["ShieldProfileUserID"] != null)
                {
                    updateUserRequest.ShieldInternalUserProfileID = Convert.ToInt32(state["ShieldProfileUserID"]);
                }

                var isAdmin = rxUser.AppRoles.Any(x => x == Constants.UserRoleNames.ADMIN);

                logger.Debug("saveUser() - UpdateUser in UMP called");

                EPSBroker.UpdateUserInUMP(this.licenseID, userRequest.UserInformation.LoginID, rxUser.FirstName, rxUser.LastName, rxUser.Email,
                    userRequest.UserInformation.IsActive, rxUser.UserID, isAdmin, rxUser.NPI, userRequest.UserCredentials.CurrentNPI, this.nodeDBID);


                logger.Debug("saveUser() - UpdateUser called in EPSBroker");

                ePrescribeSvc.ePrescribeSvcResponse updateUserResponse = EPSBroker.UpdateUser(updateUserRequest);

                logger.Debug("saveUser() - Response {0}", updateUserResponse.ToLogString());

                if (updateUserResponse.Messages.Length > 0 || !updateUserResponse.Success)
                {

                    if (updateUserResponse.Messages.Length > 1)
                    {
                        foreach (string message in updateUserResponse.Messages)
                        {
                            Audit.AddException(Guid.Empty.ToString(), this.licenseID, message, ipAddress, null, null, this.nodeDBID);
                            responseModel.MessageList.Add(new MessageModel(message, MessageIcon.Error, false));
                        }
                    }
                    else
                    {
                        responseModel.MessageList.Add(new MessageModel(updateUserResponse.Messages[0], MessageIcon.Error, false));
                    }

                    return;
                }

                // save POB to Provider associations, eps handles for new users
                if (userRequest.UserInformation.IsActive)
                {
                    if (UserSettingsModelHelper.IsPOB(userRequest.UserSettings))
                    {
                        if (userRequest.UserSettings.SupervisingProviderSelectionMode == SupervisingProviderSelectionMode.All)
                        {
                            RxUser.SavePOBToProviderAssociations(this.editeeUserID, true, this.nodeDBID);
                        }
                        else
                        {
                            RxUser.SavePOBToProviderAssociations(this.editeeUserID, false, new ArrayList(getSelectedProvidersGuidList(userRequest.UserSettings.UserSupervisingProvidersList)), this.nodeDBID);
                        }
                    }
                }

                logger.Debug("saveUser() - set CanApproveTrait");

                SetCanApproveTrait(state, userRequest.UserEPCSSettings, userRequest.UserInformation);
                saveSendSMSAlertPreference(isProviderOrPa, userRequest);
                if (updateUserResponse.Success)
                {
                    responseModel.IsSaveSuccessful = true;
                }
            }

            logger.Debug("saveUser() - saveUserPreferences");

            // update preferences
            saveUserPreferences(this.editeeUserID, isProviderOrPa, userRequest.UserPreferences, state);

            state["DEASTATUS"] = "ACTIVE";

            //if not a test site, enroll with SureScripts and sync with SalesForce
            if (state["STANDING"].ToString() == "1")
            {
                //get the SPI
                if (isProviderOrPa)
                {

                    logger.Debug("saveUser() - GetSPI");

                    new SPI().GetSPI(rxUser.UserID, rxUser.LicenseID, this.editorUserID, state.GetBooleanOrFalse("EnableRenewals"),
                        StateUtils.SiteInfo.GetSessionSiteID(state), this.nodeDBID);
                }
            }

            ClearDeaAndStateLicensesFromSession(state);

            if (responseModel.IsSaveSuccessful)
            {
                responseModel.MessageList.Add(new MessageModel(HttpUtility.UrlDecode(GetFinalMessage(this.userMode, userRequest.UserInformation.FirstName,
                        userRequest.UserInformation.LastName)), MessageIcon.Success, false));
            }
            if (this.userMode == UserMode.SelfEdit)
            {
                responseModel.RedirectUrl = RedirectAfterSave(state);
            }
        }

        private string RedirectAfterSave(IStateContainer state)
        {
            bool ssoTaskMode = string.Compare(state.GetStringOrEmpty(Constants.SessionVariables.SSOMode), Constants.SSOMode.TASKMODE, true) == 0;
            bool epaTaskMode = string.Compare(state.GetStringOrEmpty(Constants.SessionVariables.SSOMode), Constants.SSOMode.EPAPATIENTLOCKDOWNTASKMODE, true) == 0;

            switch ((Constants.UserCategory)state[Constants.SessionVariables.UserType])
            {
                case Constants.UserCategory.GENERAL_USER:
                case Constants.UserCategory.POB_LIMITED:
                    if (ssoTaskMode || epaTaskMode)
                    {
                        return Constants.PageNames.REPORTS;
                    }
                    else
                    {
                        return Constants.PageNames.SELECT_PATIENT;
                    }
                default:
                    if (ssoTaskMode || epaTaskMode)
                    {
                        return Constants.PageNames.DOC_REFILL_MENU;
                    }
                    else
                    {
                        return Constants.PageNames.SELECT_PATIENT;
                    }
            }
        }

        private void AddUser(ePrescribeSvc.RxUser user, SaveUserResponseModel responseModel, UserModel requestModel, IStateContainer state, string ipAddress, string appUrl, ILoggerEx logger)
        {
            string userName;
            logger.Debug("saveUser() - Add Mode");

            userName = EPSBroker.GetNewUniqueUserName(user.FirstName, user.MiddleName.SubstringOrEmpty(0, 1), user.LastName);

            logger.Debug("saveUser() - Got eRx unique username {0}", userName);

            var addUserRequest = EPSBroker.ServiceRequest<eRxWeb.ePrescribeSvc.CreateUserRequest>();

            //Save user specific details.
            user.UserName = userName;
            user.LicenseID = this.licenseID;
            user.AccountID = state.GetStringOrEmpty("AHSAccountID");
            user.UserID = this.editeeUserID;
            user.Active = requestModel.UserInformation.IsActive;
            user.LastPasswordChange = DateTime.Today;
            user.ShieldStatus = (int)ShieldUserStatus.PENDING_ACTIVATION;

            user.PatientReceipt = true;
            user.SpecialtyCode1 = requestModel.UserCredentials.Specialty1;
            user.SpecialtyCode2 = requestModel.UserCredentials.Specialty2;
            user.DBID = (int)this.nodeDBID;

            //Save request specific details.

            addUserRequest.DbID = (eRxWeb.ePrescribeSvc.ConnectionStringPointer)this.nodeDBID;
            addUserRequest.Application = ePrescribeSvc.ApplicationEnum.ePrescribe;

            addUserRequest.ShieldInternalAppID = ShieldSettings.GetShieldInternalAppID(state);
            addUserRequest.ShieldInternalEnvironmentID = ShieldSettings.GetShieldInternalEnvironmentID(state);
            addUserRequest.EprescribeExternalAppInstanceID = ShieldSettings.ePrescribeExternalAppInstanceId(state); ;

            addUserRequest.RxUser = user;
            addUserRequest.IsAppIdentityEnabled = requestModel.UserInformation.IsActive;
            addUserRequest.Password = string.Empty;

            logger.Debug("saveUser() - calling shield add user");

            var resp = EPSBroker.AddUser(addUserRequest);

            if (!string.IsNullOrWhiteSpace(resp.ActivationCode))
            {
                //user account successfully created in 
                responseModel.IsSaveSuccessful = true;
                responseModel.UserActivationInfo = UserActivationInfoModelHelper.CreateUserInforActiavationModel(resp.ActivationCode, user.FirstName, user.LastName,
                    user.Email, appUrl);
                this.editeeUserID = user.UserID = resp.UserGUID;
                ApiHelper.AuditLogUserInsert(ePrescribeSvc.AuditAction.USER_ADD, this.editeeUserID, this.licenseID, this.editorUserID, ipAddress, this.nodeDBID);
            }
            else
            {
                foreach (string message in resp.Messages)
                {
                    Audit.AddException(Guid.Empty.ToString(), this.licenseID, message, ipAddress, null, null, this.nodeDBID);
                    responseModel.MessageList.Add(new MessageModel(message, MessageIcon.Error, false));
                }
                responseModel.IsSaveSuccessful = false;
            }
        }

        private List<string> getSelectedProvidersGuidList(List<UserSupervisingProviderModel> providerList)
        {
            var selectedProviders = providerList.FindAll(x => x.Selected == true).Select(y => y.ProviderId).ToList();
            return selectedProviders;
        }

        private List<eRxWeb.ePrescribeSvc.StateLicense> LoadStateLicenses(StateLicense[] userStateLicenses, ILoggerEx logger)
        {
            var stateLicenses = new List<eRxWeb.ePrescribeSvc.StateLicense>();

            if (userStateLicenses != null)
            {
                foreach (StateLicense item in userStateLicenses)
                {
                    var license = new eRxWeb.ePrescribeSvc.StateLicense
                    {
                        ExpDate = DateTime.Parse(item.ExpiringDate),
                        LicenseNumber = item.LicenseNo,
                        State = item.State,
                        LicenseType = (ePrescribeSvc.ProviderLicenseType)RxUser.GetLicenseTypeDescriptionID(item.LicenseType)
                    };

                    stateLicenses.Add(license);
                }
            }
            logger.Debug("saveUser() - Got Licenses");
            return stateLicenses;
        }

        private List<eRxWeb.ePrescribeSvc.DEALicense> LoadDeaLicenses(DEALicense[] dEALicenses, ILoggerEx logger)
        {
            var deaLicenses = new List<eRxWeb.ePrescribeSvc.DEALicense>();

            if (dEALicenses != null)
            {
                foreach (DEALicense item in dEALicenses)
                {
                    ePrescribeSvc.DEALicense dealicense = new ePrescribeSvc.DEALicense();
                    dealicense.DEALicenseID = item.DEALicenseID;
                    dealicense.DEANumber = item.DEANumber;
                    dealicense.DEAExpirationDate = DateTime.Parse(item.DEAExpirationDate);
                    dealicense.DEAIIAllowed = item.DEAIIAllowed;
                    dealicense.DEAIIIAllowed = item.DEAIIIAllowed;
                    dealicense.DEAIVAllowed = item.DEAIVAllowed;
                    dealicense.DEAVAllowed = item.DEAVAllowed;
                    dealicense.DeaLicenseTypeId = (ePrescribeSvc.DeaLicenseType)item.DEALicenseTypeId;

                    deaLicenses.Add(dealicense);
                }
            }

            logger.Debug("saveUser() - Got dea numbers");
            return deaLicenses;
        }



        private bool didUserCorrectlySelectLicense(bool isProviderOrPa, StateLicense[] stateLicenses, string userID, string practiceState, UserMode userMode, ConnectionStringPointer dbID)
        {
            //check to ensure the user actually entered a state license like we asked them to
            if (isProviderOrPa && userMode == UserMode.SelfEdit)
            {
                if (!RxUser.VerifyUserStateLicense(userID, practiceState, dbID))
                {
                    return false;
                }
            }
            return true;
        }

        public bool ValidatePrescriberLicenseCount(StateLicense[] licenses, SaveUserResponseModel responseModel, ILoggerEx logger)
        {
            logger.Debug("saveUser() - isProviderOrPA");

            int licenseCount = licenses.Length;

            if (licenseCount <= 0)
            {
                responseModel.MessageList.Add(
                    new MessageModel("Please provide at least one state license.",
                                      MessageIcon.Error,
                                      false));
                return true;
            }
            return false;
        }

        private void SetCanApproveTrait(IStateContainer session, UserEPCSSettingsModel epcsSettingsModel, UserInformationModel userInformation)
        {
            ShieldTraitValue traitValue = epcsSettingsModel.IsEPCSApprover ? ShieldTraitValue.YES : ShieldTraitValue.NO;

            if (!string.IsNullOrEmpty(userInformation.LoginID))
            {
                eRxWeb.Users.SetTraitForUser(userInformation.LoginID,
                                             ShieldTraitName.CanApprove,
                                             traitValue,
                                             ShieldSettings.ePrescribeExternalAppInstanceId(session),
                                             ShieldInfo.GetShieldSecurityToken(session));
            }
        }


        private void saveSendSMSAlertPreference(bool isProviderOrPa, UserModel userRequest)
        {
            bool isAdmin = UserSecurityModelHelper.IsUserAdminChecked(userRequest.UserSecurity);

            // Save only if SMS Alert controls are shown and SendSMS is changed.
            if (this.userMode == UserMode.SelfEdit && this.isSendSmsEnabled && userRequest.UserMobileMessageSettings.IsChanged)
            {
                UserMobileMessageSettingModel userMobileMessagingSettingParam = null;
                if (isProviderOrPa)
                {
                    userMobileMessagingSettingParam = new UserMobileMessageSettingModel()
                    {
                        UserID = this.editeeUserID,
                        MobileNumber = userRequest.UserMobileMessageSettings.PhoneNumber,
                        IsNewRxErrorAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertNewRxError,
                        IsRefillRxErrorAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertRefillRxError,
                        IsCancelRxErrorAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertCancelRxError,
                        IsChangeRxErrorAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertChangeRxError,
                        IsChangeRxRequestAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertChangeRxRequest,
                        IsDEALicenseExpirationAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertDEALicenseExpiration,
                        IsServiceUpdateAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertServiceUpdate
                    };
                }
                else  // isOtherUserSMSSettingSave
                {
                    userMobileMessagingSettingParam = new UserMobileMessageSettingModel()
                    {
                        UserID = this.editeeUserID,
                        MobileNumber = userRequest.UserMobileMessageSettings.PhoneNumber,
                        IsNewRxErrorAlertEnabled = false,
                        IsRefillRxErrorAlertEnabled = false,
                        IsCancelRxErrorAlertEnabled = false,
                        IsChangeRxErrorAlertEnabled = false,
                        IsChangeRxRequestAlertEnabled = false,
                        IsDEALicenseExpirationAlertEnabled = false,
                        IsServiceUpdateAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertServiceUpdate
                    };
                }

                Allscripts.ePrescribe.Data.SMSAlert.UpdateUserMobileMessageSettings(userMobileMessagingSettingParam, this.nodeDBID);
                mobileMessageSettingProcessAuditLog(userMobileMessagingSettingParam, userRequest.UserMobileMessageSettings.PhoneNumber);
            }
            else // if not edit self
            {
                if (userRequest.UserMobileMessageSettings.IsChanged)
                {
                    if (!isProviderOrPa)
                    {
                        if (isAdmin)
                        {
                            if (userRequest.UserMobileMessageSettings.SmsAlertNewRxError
                                || userRequest.UserMobileMessageSettings.SmsAlertRefillRxError
                                || userRequest.UserMobileMessageSettings.SmsAlertChangeRxError
                                || userRequest.UserMobileMessageSettings.SmsAlertCancelRxError
                                || userRequest.UserMobileMessageSettings.SmsAlertChangeRxRequest
                                || userRequest.UserMobileMessageSettings.SmsAlertDEALicenseExpiration)
                            {
                                UserMobileMessageSettingModel userMobileMessagingSettingParam = new UserMobileMessageSettingModel()
                                {
                                    UserID = this.editeeUserID,
                                    MobileNumber = userRequest.UserMobileMessageSettings.PhoneNumber,
                                    IsNewRxErrorAlertEnabled = false,
                                    IsRefillRxErrorAlertEnabled = false,
                                    IsCancelRxErrorAlertEnabled = false,
                                    IsChangeRxErrorAlertEnabled = false,
                                    IsChangeRxRequestAlertEnabled = false,
                                    IsDEALicenseExpirationAlertEnabled = false,
                                    IsServiceUpdateAlertEnabled = userRequest.UserMobileMessageSettings.SmsAlertServiceUpdate
                                };
                                Allscripts.ePrescribe.Data.SMSAlert.UpdateUserMobileMessageSettings(userMobileMessagingSettingParam, this.nodeDBID);
                            }
                        }
                        else
                        {
                            Allscripts.ePrescribe.Data.SMSAlert.DeleteUserMobileNumber(this.editeeUserID, this.nodeDBID);
                            userRequest.UserMobileMessageSettings.PhoneNumber = string.Empty;
                            userRequest.UserMobileMessageSettings.IsNumberAvailable = false;
                            userRequest.UserMobileMessageSettings.SmsAlertCancelRxError = false;
                            userRequest.UserMobileMessageSettings.SmsAlertChangeRxError = false;
                            userRequest.UserMobileMessageSettings.SmsAlertNewRxError = false;
                            userRequest.UserMobileMessageSettings.SmsAlertDEALicenseExpiration = false;
                            userRequest.UserMobileMessageSettings.SmsAlertRefillRxError = false;
                            userRequest.UserMobileMessageSettings.SmsAlertServiceUpdate = false;
                            userRequest.UserMobileMessageSettings.SmsAlertChangeRxRequest = false;
                        }
                    }
                }

            }
        }


        private void mobileMessageSettingProcessAuditLog(UserMobileMessageSettingModel changedUserMobileMessageSetting, string phoneNumber)
        {
            if (this.existingSMSSettings.IsNewRxErrorAlertEnabled != changedUserMobileMessageSetting.IsNewRxErrorAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsNewRxErrorAlertEnabled ? SMSAlertUserActionType.NewRxErrorAlertOn
                                                                         : SMSAlertUserActionType.NewRxErrorAlertOff, phoneNumber);
            }
            if (this.existingSMSSettings.IsRefillRxErrorAlertEnabled != changedUserMobileMessageSetting.IsRefillRxErrorAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsRefillRxErrorAlertEnabled ? SMSAlertUserActionType.RefillRxErrorAlertOn
                                                                         : SMSAlertUserActionType.RefillRxErrorAlertOff, phoneNumber);
            }

            if (this.existingSMSSettings.IsChangeRxErrorAlertEnabled != changedUserMobileMessageSetting.IsChangeRxErrorAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsChangeRxErrorAlertEnabled ? SMSAlertUserActionType.ChangeRxErrorAlertOn
                                                                         : SMSAlertUserActionType.ChangeRxErrorAlertOff, phoneNumber);
            }
            if (this.existingSMSSettings.IsCancelRxErrorAlertEnabled != changedUserMobileMessageSetting.IsCancelRxErrorAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsCancelRxErrorAlertEnabled ? SMSAlertUserActionType.CancelRxErrorAlertOn
                                                                         : SMSAlertUserActionType.CancelRxErrorAlertOff, phoneNumber);
            }
            if (this.existingSMSSettings.IsChangeRxRequestAlertEnabled != changedUserMobileMessageSetting.IsChangeRxRequestAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsChangeRxRequestAlertEnabled ? SMSAlertUserActionType.ChangeRxRequestAlertOn
                                                                         : SMSAlertUserActionType.ChangeRxRequestAlertOff, phoneNumber);
            }
            if (this.existingSMSSettings.IsDEALicenseExpirationAlertEnabled != changedUserMobileMessageSetting.IsDEALicenseExpirationAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsDEALicenseExpirationAlertEnabled ? SMSAlertUserActionType.DEALicenseExpirationAlertOn
                                                                         : SMSAlertUserActionType.DEALicenseExpirationAlertOff, phoneNumber);
            }
            if (this.existingSMSSettings.IsServiceUpdateAlertEnabled != changedUserMobileMessageSetting.IsServiceUpdateAlertEnabled)
            {
                mobileMessageSettingAuditLog(changedUserMobileMessageSetting.IsServiceUpdateAlertEnabled ? SMSAlertUserActionType.ServiceUpdateAlertOn
                                                                         : SMSAlertUserActionType.ServiceUpdateAlertOff, phoneNumber);
            }
        }


        private void mobileMessageSettingAuditLog(SMSAlertUserActionType smsAlertUserActionType, string mobileNumber)
        {
            Allscripts.ePrescribe.Data.SMSAlert.InsertAuditLog(this.licenseID,
                               this.editeeUserID,
                               smsAlertUserActionType,
                               mobileNumber,
                               this.nodeDBID);
        }

        private string GetFinalMessage(UserMode userMode, string firstName, string lastName)
        {
            string finalMessage = string.Empty;

            if (userMode == UserMode.SelfEdit)
            {
                finalMessage = "Your profile has been successfully saved. You will have to log out for changes to take effect.";
            }
            else if (userMode == UserMode.EditOtherUser)
            {
                finalMessage = $"User {firstName} {lastName} has been successfully saved.";
            }
            else if (userMode == UserMode.AddOtherUser)
            {
                finalMessage = $"User {firstName} {lastName} has been successfully created.";
            }
            return finalMessage;
        }

        private void saveUserPreferences(string userID, bool isProviderOrPA, UserPreferencesModel model, IStateContainer state)
        {
            List<Preference> preferences = new List<Preference>();

            if (isPPTPlusEnabled(isProviderOrPA, state))
            {
                preferences.Add(Preference.GetPreference(model.RTBPreference,
                    PreferenceCategory.PROVIDER_OPTIONS,
                    Constants.ProviderPreferences.SHOW_RTB));

                preferences.Add(Preference.GetPreference(model.AutoTherapeuticAlternativesPreference,
                    PreferenceCategory.PROVIDER_OPTIONS,
                    Constants.ProviderPreferences.SHOW_RTB_AUTO_THERAPEUTIC_ALTERNATIVE));

                preferences.Add(Preference.GetPreference(model.RTPSPreference,
                    PreferenceCategory.PROVIDER_OPTIONS,
                    Constants.ProviderPreferences.SHOW_RTPS));

                preferences.Add(Preference.GetPreference(model.PPTPreference,
                    PreferenceCategory.PROVIDER_OPTIONS,
                    Constants.ProviderPreferences.SHOW_PPT));

                preferences.Add(Preference.GetPreference(model.PPTOffersPreference,
                    PreferenceCategory.PROVIDER_OPTIONS,
                    Constants.ProviderPreferences.SHOW_PPT_OFFER));
            }

            Preference.SaveUserPreferences(userID, PreferenceCategory.PROVIDER_OPTIONS, preferences.ToArray(), this.nodeDBID);
        }

        private bool isPPTPlusEnabled(bool isProviderOrPA, IStateContainer state)
        {
            return isProviderOrPA
                && (this.isAdmin || this.userMode == UserMode.SelfEdit || this.userMode == UserMode.AddOtherUser)
                && Settings.GetSessionLicense(state).EnterpriseClient.ShowPPTPlus;
        }

        private void ClearDeaAndStateLicensesFromSession(IStateContainer state)
        {
            state["StateLicenseDataTable"] = null;
            state["DEALicenseDataTable"] = null;
        }
    }
}