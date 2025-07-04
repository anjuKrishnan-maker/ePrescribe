using iTextSharp.text;
using System;
using System.Collections.Generic;


namespace eRxWeb.AppCode.Authorize
{

    public static class ApiRules
    {
        private static NestedDictionary<IList<Rule>> _rule = AppilcationRules._rule;
        private static IList<Rule> SetRule(params (UserCategory role, UserPrivilege? privilege)[] rule)
        {
            return RuleMapper.SetRule(rule);
        }

        public static void SetRules()
        {
            //This is going to be a long list- keep it alphabetical with a line between controllers.
            _rule[Controllers.ALLERGY_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY),
                                                           (UserCategory.All, UserPrivilege.ADMIN));

            _rule[Controllers.APP_BUILD_UP_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.AUDIT_VIEW_INFO_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.CANCEL_RX_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY),
                                                             (UserCategory.All, UserPrivilege.ADMIN));

            _rule[Controllers.CHANGE_PASSWORD_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY),
                                                                     (UserCategory.All, UserPrivilege.ADMIN));

            _rule[Controllers.CHART_EXTRACT_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.CONTENT_LOAD].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.CONTENT_LOAD][Actions.RETRIEVE_INITIAL_PAYLOAD].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.CONTENT_LOAD][Actions.RETRIEVE_MEDICATION_LOADED_PAYLOAD].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY),
                                                                                                         (UserCategory.All, UserPrivilege.ADMIN));
            _rule[Controllers.CONTENT_LOAD][Actions.RETRIEVE_REFRESH_PAYLOAD].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.DELUXE_FEATURE_SELECTION_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY),
                                                                (UserCategory.All, UserPrivilege.ADMIN));

            _rule[Controllers.ELIGIBILITY_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.FAILED_REFREQ_MESSAGES_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.FAILED_RX_MESSAGES_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.FEATURE_COMPARISON_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.FORMULARY_OVERRIDE_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.GET_EPCS_SEND_TO_PHARMACY_API].Value = SetRule((UserCategory.PROVIDER, UserPrivilege.ANY));

            _rule[Controllers.HELP_CONTENT_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.HOME_ADDRESS_API].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));

            _rule[Controllers.LICENSE_CREATION].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));

            _rule[Controllers.MESSAGE_QUEUE_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PATIENT_HEADER_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PATIENT_MED_HISTORY_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.PATIENT_MED_REC_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.PATIENT_MED_REC_API][Actions.GET_MED_RECONCILIATION_INFO].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PATIENT_UPLOAD].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PDF_STREAM].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PDMP_API][Actions.GET_COMMONUI_URL].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY)); ;
            _rule[Controllers.PDMP_API][Actions.GET_PDMP_ENROLLMENT_FORM_INFO].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.PDMP_API][Actions.GET_PDMP_SUMMARY].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY)); ;
            _rule[Controllers.PDMP_API][Actions.PDMP_DETAILS_BUTTON_HANDLER].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY)); ;
            _rule[Controllers.PDMP_API][Actions.PDMP_ENROLLMENT_FORM_SUBMIT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PHARMACY_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PPT_PLUS_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.PRIVACY_OVERRIDE_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.PROGRAM_ALERT_API][Actions.IS_PROVIDER_ENROLLED_IN_SPECIALTY_MED].Value = SetRule((UserCategory.PROVIDER, UserPrivilege.ANY)); ;
            _rule[Controllers.PROGRAM_ALERT_API][Actions.IS_SCRIPTPAD_HAS_SPEC_MED].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.REGISTRANT_STATUS].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));

            _rule[Controllers.REPORTS_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.REVIEW_HISTORY_API][Actions.ASSIGN_SUPERVISING_PROVIDER].Value = SetRule((UserCategory.PA_SUP_AND_POBs, UserPrivilege.ANY));
            _rule[Controllers.REVIEW_HISTORY_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.RX_DETAIL_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.RX_DUR_REVIEW_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Controllers.RX_DUR_REVIEW_API][Actions.CS_MED_REFILL_REQUEST_NOT_ALLOWED_ON_CONTACT_PROVIDER].Value = SetRule((UserCategory.POB_SUPER | UserCategory.POB_REGULAR, UserPrivilege.ANY));


            _rule[Controllers.SCRIPT_PAD_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.SELECT_MEDICATION_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.SELECT_MEDICATION_SEARCH_API].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));

            _rule[Controllers.SELECT_PATIENT_API][Actions.CHECK_IN_PATIENT].Value = SetRule((UserCategory.PA_SUP_AND_POBs | UserCategory.GENERAL_USER, UserPrivilege.ANY));
            _rule[Controllers.SELECT_PATIENT_API][Actions.Get_Start_Up_Parameters].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.SELECT_PATIENT_API][Actions.LOAD_PROVIDERS_FOR_SUPERVISED_PA].Value = SetRule((UserCategory.PA_SUP_AND_POBs, UserPrivilege.ANY));
            _rule[Controllers.SELECT_PATIENT_API][Actions.SEARCH_PATIENTS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.SELECT_PATIENT_API][Actions.SET_PATIENT_INFO].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.SELECT_PATIENT_API][Actions.SET_PROVIDER_INFORMATION].Value = SetRule((UserCategory.PRESCRIBER_OR_POB, UserPrivilege.ANY));
            _rule[Controllers.SELECT_PATIENT_API][Actions.SET_SUPERVISING_PROVIDER_INFO].Value = SetRule((UserCategory.POB, UserPrivilege.ANY));

            _rule[Controllers.SETTINGS_API].Value = SetRule((UserCategory.All, UserPrivilege.ADMIN));

            _rule[Controllers.SUPERVISING_PROVIDER_API].Value = SetRule((UserCategory.POB, UserPrivilege.ANY));

            _rule[Controllers.UPDATE_USER_CSP].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.URGENT_MESSAGE_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.USER_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.USER_API][Actions.SAVE_SECRET_QUESTION_ANSWERS].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.USER_API][Actions.SAVE_USER].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.USER_API][Actions.USER_INIT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));
            _rule[Controllers.USER_API][Actions.USER_INIT].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.USER_PREFERENCE_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.USER_SESSION_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.USER_REGISTRATION].Value = SetRule((UserCategory.All, UserPrivilege.ANY));

            _rule[Controllers.WELCOME].Value = SetRule((UserCategory.PRESCRIBER, UserPrivilege.ANY));
            _rule[Controllers.WELCOME][Actions.GETURL_FOR_NAVIGATION_TO_CSP].Value = SetRule((UserCategory.NONE, UserPrivilege.ANY));

            _rule[Controllers.WELCOME_TOUR_API].Value = SetRule((UserCategory.All, UserPrivilege.ANY));


        }
    }
}