using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.ePrescribeSvc;
using eRxWeb.ServerModel;
using eRxWeb.State;
using static Allscripts.ePrescribe.Common.Constants;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using ShieldTraitName = eRxWeb.ePrescribeSvc.ShieldTraitName;

namespace eRxWeb.AppCode.Users
{
    public static class UserEPCSSettingsModelHelper
    {
        public static UserEPCSSettingsModel SetUserEPCSSettingsModel(UserMode userMode, UserInformationModel userInformation, IStateContainer session, ConnectionStringPointer dbID)
        {

            UserEPCSSettingsModel epcsSettingsModel = new UserEPCSSettingsModel();

            bool isUserEPCSCanEnroll = false;
            bool isUserEpcs2ndApprover = false;
            string identityProofingStatus = string.Empty;
            bool isUserEpcsCanPrescribe = false;
            bool isUserEpcsCompromised = false;
            List<ShieldTraitInfo> shieldTraitInfo = new List<ShieldTraitInfo>();

            epcsSettingsModel.IsEnterpriseClientEPCSEnabled = session.GetBooleanOrFalse(SessionVariables.IsEnterpriseEpcsEnabled);
            epcsSettingsModel.IsLicenseEPCSPurchased = EPCSWorkflowUtils.IsLicenseEpcsPurchased(StateUtils.UserInfo.GetSessionLicense(session));
            epcsSettingsModel.IsLicenseShieldEnabled = session.GetBooleanOrFalse(SessionVariables.IsLicenseShieldEnabled);

            if (userMode == UserMode.AddOtherUser)
            {
                //by passing user lookup when adding new user
            }
            else
            {
                if (string.IsNullOrEmpty(userInformation.LoginID))
                    shieldTraitInfo = null;
                else
                {
                    shieldTraitInfo = EPSBroker.GetShieldUserTraits(userInformation.LoginID,
                                                                                        new List<ShieldTraitName>
                                                                                        {
                                                                                          ShieldTraitName.CanEnroll,
                                                                                          ShieldTraitName.CanApprove,
                                                                                          ShieldTraitName.CanPrescribe,
                                                                                          ShieldTraitName.IsIdentityCompromised,
                                                                                          ShieldTraitName.IDProofingStatus
                                                                                        },
                                                                                        ShieldSettings.ePrescribeExternalAppInstanceId(session),
                                                                                        ShieldInfo.GetShieldSecurityToken(session));

                    isUserEPCSCanEnroll = IsUserEPCSShieldTrait(shieldTraitInfo, ShieldTraitName.CanEnroll);
                    isUserEpcs2ndApprover = IsUserEPCSShieldTrait(shieldTraitInfo, ShieldTraitName.CanApprove);
                    identityProofingStatus = GetUserIdentityProofingStatus(shieldTraitInfo);
                    isUserEpcsCanPrescribe = IsUserEPCSShieldTrait(shieldTraitInfo, ShieldTraitName.CanPrescribe);
                    isUserEpcsCompromised = IsUserEPCSShieldTrait(shieldTraitInfo, ShieldTraitName.IsIdentityCompromised);
                }
            }


            //setting up EPCS Approver Checkbox
            if (userMode == UserMode.AddOtherUser)
            {
                epcsSettingsModel.ChkEPCSApproverEnabled = false;
            }
            else
            {
                if (shieldTraitInfo == null)
                {
                    epcsSettingsModel.ChkEPCSApproverEnabled = false;
                }
                else
                {
                    var epcsApproverTrait = shieldTraitInfo.Find(x => x.TraitName == ShieldTraitName.CanApprove);

                    epcsSettingsModel.IsEPCSApprover = epcsApproverTrait != null && epcsApproverTrait.TraitValueEnum == ShieldTraitValue.YES;
                    epcsSettingsModel.ChkEPCSApproverEnabled = epcsApproverTrait != null && epcsApproverTrait.UserMaySet;
                }
            }

            if (isUserEpcsCompromised)
            {
                epcsSettingsModel.ShowEPCSRegistartionLink = false;
                epcsSettingsModel.EPCSPermissionLabel = CommonTerms.Disabled;
            }
            else if (!string.IsNullOrEmpty(identityProofingStatus))
            {
                epcsSettingsModel.ShowEPCSRegistartionLink = false;
                epcsSettingsModel.EPCSPermissionLabel = identityProofingStatus;

                if (identityProofingStatus.Equals(EPCSIdProofingStatus.PASSED, StringComparison.OrdinalIgnoreCase))
                {
                    if (isUserEpcsCanPrescribe)
                    {
                        epcsSettingsModel.EPCSPermissionLabel = CommonTerms.On;
                    }
                    else
                    {
                        epcsSettingsModel.EPCSPermissionLabel = CommonTerms.Passed;
                    }

                    if (userMode == UserMode.SelfEdit)
                    {
                        epcsSettingsModel.ShowManageSecondFactorForms = true;
                        epcsSettingsModel.SecondFactorFormUrl = Allscripts.Impact.ConfigKeys.VerizonSecondFactorManagementURL;
                        if (session[SessionVariables.IsTenantShieldCSPAppInstanceAvailable] == null)
                        {
                            GetUserShieldCspStatusInfoResponse getUserShieldCspStatusInfoResponse = EPSBroker.GetUserShieldCspStatusInfo(dbID,
                                                                                                                                         session.GetStringOrEmpty(SessionVariables.ShieldSecurityToken),
                                                                                                                                         session.GetStringOrEmpty(SessionVariables.ShieldExternalTenantID),
                                                                                                                                         session.GetStringOrEmpty(SessionVariables.UserId),
                                                                                                                                         session.GetStringOrEmpty(SessionVariables.SessionLicense));
                            session[SessionVariables.IsTenantShieldCSPAppInstanceAvailable] = getUserShieldCspStatusInfoResponse.IsTenantShieldCSPAppInstanceAvailable;
                        }
                        if (session.GetBooleanOrFalse(SessionVariables.IsTenantShieldCSPAppInstanceAvailable))
                        {
                            epcsSettingsModel.ZentryUser = false;
                            epcsSettingsModel.CspUser = true;
                            epcsSettingsModel.SecondFactorFormUrl = new EPSBroker().GetIdProofingUrl(StateUtils.UserInfo.GetIdProofingMode(session),
                                                                                               session.GetStringOrEmpty(SessionVariables.SessionShieldUserName));
                        }

                        epcsSettingsModel.SecondFactorHelpUrl = HttpContext.Current.Request.UserIpAddress() + Allscripts.Impact.Ilearn.ILearnConfigurationManager.GetErxILearnPageUrl("EPCS");
                    }
                }
                else if (identityProofingStatus.Equals(EPCSIdProofingStatus.PENDING, StringComparison.OrdinalIgnoreCase))
                {
                    epcsSettingsModel.EPCSPermissionLabel = CommonTerms.Pending;
                    if (userMode == UserMode.SelfEdit)
                    {
                        var epcsRegistrationLink = new EPSBroker().GetIdProofingUrl(StateUtils.UserInfo.GetIdProofingMode(session),
                                                                              session.GetStringOrEmpty(SessionVariables.SessionShieldUserName));
                        epcsSettingsModel.ShowEPCSRegistartionLink = true;
                        epcsSettingsModel.EpcsRegistrationLink = epcsRegistrationLink;
                        epcsSettingsModel.EPCSPermissionLabel = "";
                        epcsSettingsModel.EpcsRegistrationLinkLabel = "Resume EPCS Registration";
                    }
                }
                else if (isUserEPCSCanEnroll)
                {
                    if (userMode == UserMode.SelfEdit)
                    {
                        var epcsRegistrationLink = new EPSBroker().GetIdProofingUrl(StateUtils.UserInfo.GetIdProofingMode(session),
                                                                               session.GetStringOrEmpty(SessionVariables.SessionShieldUserName));
                        epcsSettingsModel.ShowEPCSRegistartionLink = true;
                        epcsSettingsModel.EpcsRegistrationLink = epcsRegistrationLink;
                        epcsSettingsModel.EPCSPermissionLabel = "";
                        epcsSettingsModel.EpcsRegistrationLinkLabel = "Start EPCS Registration";
                    }
                    else
                    {
                        epcsSettingsModel.EPCSPermissionLabel = CommonTerms.Registered;
                    }
                }
                else if (identityProofingStatus.Equals(EPCSIdProofingStatus.NOTREGISTERED, StringComparison.OrdinalIgnoreCase))
                {
                    epcsSettingsModel.EPCSPermissionLabel = CommonTerms.NotRegistered;
                }
            }

            else
            {
                epcsSettingsModel.ShowEPCSRegistartionLink = false;
                epcsSettingsModel.EPCSPermissionLabel = CommonTerms.Off;
            }

            if (userMode == UserMode.SelfEdit)
            {
                epcsSettingsModel.ShowEPCSSecondFactor = true;
                if (isUserEpcs2ndApprover)
                {
                    epcsSettingsModel.ShowManageEpcsApprovalsLink = true;

                    session.Remove("EpcsApproverFromEditUser");
                    session["EpcsApproverFromEditUser"] = true;
                }
                else
                {
                    epcsSettingsModel.ShowManageEpcsApprovalsLink = false;
                    epcsSettingsModel.EPCSSecondFactorLabel = CommonTerms.Off;
                }

            }

            return epcsSettingsModel;
        }


        private static bool IsUserEPCSShieldTrait(List<ShieldTraitInfo> traits, ShieldTraitName shieldTraitName)
        {
            bool traitValue = false;

            var trait = traits.Find(x => x.TraitName == shieldTraitName);

            if (trait != null && trait.TraitValueEnum == ShieldTraitValue.YES)
            {
                traitValue = true;
            }
            return traitValue;
        }

        private static string GetUserIdentityProofingStatus(List<ShieldTraitInfo> traits)
        {
            string proofingStatus = string.Empty;
            var IdProofingTrait = traits.Find(x => x.TraitName == eRxWeb.ePrescribeSvc.ShieldTraitName.IDProofingStatus);
            if (IdProofingTrait != null)
            {
                proofingStatus = IdProofingTrait.TraitValueString;
            }
            return proofingStatus;
        }


    }
}