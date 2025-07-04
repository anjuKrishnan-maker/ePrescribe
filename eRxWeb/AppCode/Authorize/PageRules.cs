using Allscripts.ePrescribe.Common;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Authorize
{
    public static class Pages
    {
        private static string[] PagesWithAnonymousAccess = new string[] { Constants.PageNames.LOGIN,                                                                                                                                                  
                                                                          Constants.PageNames.LOGOUT,
                                                                          Constants.PageNames.EXCEPTION,
                                                                          Constants.PageNames.REGISTER,                                                                                                                                                  
                                                                          Constants.PageNames.CONTACT_US,
                                                                          Constants.PageNames.FORGOT_PASSWORD_WIZARD,
                                                                          Constants.PageNames.AD_PRIVACY_POLICY,
                                                                          Constants.PageNames.VERSION_CHECK,
                                                                          Constants.PageNames.CREATE_CAPTCHA,
                                                                          Constants.PageNames.SAML_SSO,
                                                                          Constants.PageNames.CACHE_RESET,
                                                                          Constants.PageNames.SELECT_ACCOUNT_AND_SITE,
                                                                          Constants.PageNames.USER_EULA,
                                                                          Constants.PageNames.HELP_DEFAULT,
                                                                          Constants.PageNames.HELP_ADD_ON
    };
        private static NestedDictionary<IList<Rule>> _rule = AppilcationRules._rule;

        private static IList<Rule> SetRule(params (UserCategory role, UserPrivilege? privilege)[] rule)
        {
            return RuleMapper.SetRule(rule);
        }

        internal static bool ShouldSkipAuthorization(string pageName)
        {
            return PagesWithAnonymousAccess.Any(x => x.ToLower() == pageName.ToLower());
        }


        internal static void SetRules()
        {
            _rule[Constants.PageNames.ADD_ALLERGY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));            
            _rule[Constants.PageNames.ADD_PATIENT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.ADD_PATIENT_DIAGNOSIS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.ADDRESS_VERIFY_ERROR].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.APPROVE_REFILL_TASK].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.APPROVE_SCRIPT_MESSAGE_PATIENT].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.ASSIST_RX_LAUNCH_PAGE].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.BASIC_NAME].Value = SetRule((UserCategory.All, UserPrivilege.ANY));            
            _rule[Constants.PageNames.CHANGE_PATIENT_INSURANCE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.CSS_DETECT].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.DELUXE_ACCOUNT_MANAGEMENT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.DELUX_AD_CONTAINER].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.DELUXE_BILLING_PAGE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.DELUXE_TERMS_CONDITIONS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.DOC_REFILL_MENU].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.EDIT_USERS].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.EPA_FILE_UPLOADER].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPA_INIT_QA_FILE_UPLOADER].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPA_INIT_QA_REVIEW_REDIRECT].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPA_REVIEW_REDIRECT].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPCS_DAILY_ACTIVITY_REPORT].Value = SetRule((UserCategory.PROVIDER, UserPrivilege.ANY), (UserCategory.PHYSICIAN_ASSISTANT, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPCS_REGISTRATION].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPCS_REPORT].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.EPCS_RIGHTS_ASSIGNMENT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.EULA].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.EULA_DECLINE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.EXCEPTION].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.EXPORT_REPORTS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.FORCE_PASSWORD_SETUP].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.FREE_FORM_DRUG].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.FULL_SCRIPT_EDIT].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.GOODRX_COUPON].Value = SetRule((UserCategory.All, UserPrivilege.ANY));            
            _rule[Constants.PageNames.HELP_DESK].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_EPA_HELP].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_FAQ].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_IMPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_INTERFACE_LIST].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_INTERFACE_REQUEST].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_INTERFACE_REQUEST_CONFIRMATION].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_MOBILE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_TRAINING].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.HELP_TUTORIAL].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.I_LEARN].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.IDOLOGY_ANSWERS].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.IDOLOGY_ERROR].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.IDOLOGY_FINAL_ERROR].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.IDOLOGY_SUCCESSFUL].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.IDPROOFING_REQUIRED_INFO].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.INTEGRATION_SOLUTIONS_LIST].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.INTERSITE_TRANSFER_CAREMARK_EPA].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.INTERSTITIAL_AD].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.LIBRARY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.LIST_SEND_SCRIPTS].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.LOGRX_DISPLAY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.MANUAL_ID_PROOFING_FORM].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.MANUAL_NPI_CHECK_FORM].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.MERGE_PATIENTS].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.MESSAGE_QUEUE_TX].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.MULTIPLE_VIEW_CSS].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.MULTIPLE_VIEW_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.MY_PROFILE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.NPI_CHECK].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.NPI_ERROR].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.NURSE_FREE_FORM_DRUG].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.NURSE_SIG].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.OPEN_URL_IN_FRAME].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_ACTIVE_MED_REPORT].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_ADD_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_ALLERGY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_AMENDMENTS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_DIAGNOSIS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_EDUCATION].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_MED_HISTORY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_MED_REC_DETAIL_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_MED_REC_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_MED_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PATIENT_RECEIPT].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.PDF_INPAGE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PDF].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.OPNE_PDF_VIEWER].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PDMP_DETAILS].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Constants.PageNames.PHARMACY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PHARMACY_FAVORITES].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.PHARMACY_REFILL_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PHARMACY_REFILL_SUMMARY].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PHARMACY_REFILL_DETAILS].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PHARMACY_SUMMARY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.POB_TO_PROVIDER_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PPT_DETAILS].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRE_BUILT_MED_SELECT_SIG].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRE_BUILT_PRESCRIPTIONS].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.PRESCRIPTION_DETAIL].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRE_BUILT_PRESCRIPTION_ADD_OR_EDIT].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.PRE_BUILT_SELECT_MED].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.PRESCRIPTION_DETAIL_CAS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRESCRIPTION_DETAIL_POB].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRINT_ACTIVATION_CODE].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN)); 
            _rule[Constants.PageNames.PRINT_COUPONS].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRINT_CS_REPORTS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRINT_PDF].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRINT_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRINT_RX_INFO].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRINT_SCRIPT_CSS].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.PRIVACY_PATIENT_SEARCH].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));
            _rule[Constants.PageNames.PROCESS_LINK].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PROCESS_PATIENT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PROCESS_PATIENT_SSO].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PROTECTED_STORE_EPCS_REPORTS].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.PROVIDER_ERX_ACTIVITY_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.PROVIDER_TO_POB_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.REDIRECT_TO_ANGULAR].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.REFILL_HISTORY].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.REGISTRANT_MEDIATOR].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));
            _rule[Constants.PageNames.REGISTRY_CHECKED_REPORT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.RX_DUR_REVIEW].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.RX_DUR_REVIEW_MULTI_SELECT].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.RX_PROCESSOR].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));           
            _rule[Constants.PageNames.SAML_SSO_ERROR].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));
            _rule[Constants.PageNames.SAML_SSO_LOGOUT].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));
            _rule[Constants.PageNames.SCRIPT_PAD].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.SELECT_DX].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.SELF_REPORTED_MED].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.SET_HEIGHT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.SESSION_TIME_OUT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.SIG].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.SITE_MANAGEMENT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.SPA_LANDING].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.SPECIALTYMEDTASKS].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.SPONSORED_LINK].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.START_NEW_RX_PROCESS].Value = SetRule((UserCategory.All, UserPrivilege.ANY), (UserCategory.PRESCRIBER, UserPrivilege.ANY), (UserCategory.POB_SUPER, UserPrivilege.ANY));
            _rule[Constants.PageNames.START_OVER].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.STYLE_SPINNER].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));                        
            _rule[Constants.PageNames.TASK_SCRIPT_LIST].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.TASK_SUMMARY].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.TASK_PROCESSOR].Value = SetRule((UserCategory.EXCEPT_GENERAL_USER_AND_POB_LIMITED, UserPrivilege.ANY));
            _rule[Constants.PageNames.TUTORIALS].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.TWO_N_USER_MEDIATOR].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));
            _rule[Constants.PageNames.UPDATE_ALLERGY].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Constants.PageNames.USER_CHALLENGE].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.USER_PREFERENCES].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.USER_INTERROGATION].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Constants.PageNames.VIEW_AUDIT_LOG].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            

        }
    }
}