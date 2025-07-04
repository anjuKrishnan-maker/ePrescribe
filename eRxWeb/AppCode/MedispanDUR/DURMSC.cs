using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;
using Allscripts.Impact;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using DURSettings = Allscripts.ePrescribe.Medispan.Clinical.Model.Settings.DURSettings;
using Gender = Allscripts.ePrescribe.Medispan.Clinical.Model.Gender;
using Allscripts.ePrescribe.DatabaseSelector;
using FreeFormRxDrug = Allscripts.ePrescribe.Medispan.Clinical.Model.FreeFormRxDrug;
using ServiceStack;

namespace eRxWeb.AppCode
{
    public class DURMSC
    {

        public static DURCheckResponse PerformDURCheck(DURPatient patient, List<FreeFormRxDrug> freeFormRxDrugsList, List<DrugToCheck> drugList, List<int> existingMedsList, List<Allergy> allergyList, DURSettings Settings, InteractionsManagementLevel MinimumManagementLevel)
        {

            DURCheckRequest request = DUR.ConstructDURCheckRequest(patient, drugList, existingMedsList, allergyList, Settings, MinimumManagementLevel);
            DURCheckResponse durCheckResponse = MedispanServiceBroker.PerformDurCheck(request);
            return durCheckResponse;
        }

        public static DURCheckResponse PerformDURCheck(DURCheckRequest request)
        {
            DURCheckResponse durCheckResponse = MedispanServiceBroker.PerformDurCheck(request);
            return durCheckResponse;
        }

        public static DURCheckResponse DURQuickCheck(DURPatient patient, DrugToCheck drugToCheck, List<int> existingMedsList, List<Allergy> allergyList, DURSettings Settings, InteractionsManagementLevel MinimumManagementLevel)
        {
            DURSettings QuickDURSettings = new DURSettings();

            QuickDURSettings.CheckAlcoholInteraction = YesNoSetting.No; //if quickcheck, do not include food or alcohol interactions
            QuickDURSettings.CheckFoodInteraction = YesNoSetting.No;
            QuickDURSettings.CheckDrugToDrugInteraction = Settings.CheckDrugToDrugInteraction == YesNoSetting.Yes ? YesNoSetting.Yes : YesNoSetting.No;
            QuickDURSettings.CheckDuplicateTherapy = Settings.CheckDuplicateTherapy == YesNoSetting.Yes ? YesNoSetting.Yes : YesNoSetting.No;
            QuickDURSettings.CheckDuplicateTherapyRange = Settings.CheckDuplicateTherapyRange == YesNoSetting.Yes ? YesNoSetting.Yes : YesNoSetting.No;
            QuickDURSettings.CheckDuplicateTherapyWarning = Settings.CheckDuplicateTherapyWarning == YesNoSetting.Yes ? YesNoSetting.Yes : YesNoSetting.No;
            QuickDURSettings.CheckMaxConsecutiveDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions
            QuickDURSettings.CheckMaxDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions
            QuickDURSettings.CheckMaxDurationDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions
            QuickDURSettings.CheckMaxIndividualDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions
            QuickDURSettings.CheckMinDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions
            QuickDURSettings.CheckMinDurationDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions          
            QuickDURSettings.CheckPerformDose = YesNoSetting.No; //if quickcheck, do not include dosage interactions
            QuickDURSettings.CheckPar = (Settings.CheckPar == YesNoSetting.Yes && allergyList.Count > 0)? YesNoSetting.Yes : YesNoSetting.No;
            QuickDURSettings.DuplicateTherapyRangeCheckType = Settings.DuplicateTherapyRangeCheckType == DuplicateTherapyRange.FullRange ? DuplicateTherapyRange.FullRange : DuplicateTherapyRange.AbuseOrDependency;
            QuickDURSettings.DuplicateTherapyWarningCheckType = Settings.DuplicateTherapyWarningCheckType ==
                                                                DuplicateTherapyWarning.AllDuplications ? DuplicateTherapyWarning.AllDuplications : DuplicateTherapyWarning.MediSpanDuplicationAllowanceExceeded;
            QuickDURSettings.DuplicateTherapyRangeCheckType = Settings.DuplicateTherapyRangeCheckType == DuplicateTherapyRange.FullRange ? DuplicateTherapyRange.FullRange : DuplicateTherapyRange.AbuseOrDependency;
            QuickDURSettings.InteractionDocumentType = Settings.InteractionDocumentType; 
            QuickDURSettings.InteractionOnsetCheckType = Settings.InteractionOnsetCheckType;
            QuickDURSettings.InteractionSeverityCheckType = Settings.InteractionSeverityCheckType;
           DURCheckRequest request = DUR.ConstructDURCheckRequest(patient, drugToCheck, existingMedsList, allergyList, QuickDURSettings, MinimumManagementLevel);
            return MedispanServiceBroker.PerformDurCheck(request);
        }

        public static DURCheckResponse DURQuickCheck(DURCheckRequest request)
        {
            return MedispanServiceBroker.PerformDurCheck(request);
        }
    }
}