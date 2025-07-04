using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.Impact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.ExtensionMethods;
namespace eRxWeb.AppCode.DurBPL
{
    public class eRxWebAppCodeWrapper : IeRxWebAppCodeWrapper
    {
        
        public DURCheckResponse PerformDURCheck(DURCheckRequest request)
        {
            return DURMSC.PerformDURCheck(request);
        }
        public DURResponse GetDurWarnings(string patientDob, string patientGender, List<Rx> currentRxs, List<string> activeRxList, List<Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy> allergies, Allscripts.ePrescribe.Objects.DURSettings durSettings)
        {
            DURResponse res = new DURResponse();
            var request = DURMedispanUtils.ConstructDurCheckRequest(patientDob,
                                                                        patientGender,
                                                                        currentRxs,
                                                                       activeRxList,
                                                                        allergies,
                                                                        durSettings);
            

            res.DurCheckResponse = DURMSC.PerformDURCheck(request);
            var systemConfig = new Allscripts.Impact.SystemConfig();
            res.MedispanCopyright =
                  systemConfig.GetAppSetting("MedispanCopyright");
            return res;
        }
        public PrescriptionTaskType RetrieveTaskType(RxTaskModel rxTask, PrescriptionTaskType TaskType)
        {
            return DURMedispanUtils.RetrieveTaskType(rxTask, TaskType);
        }
        public List<Rx> RetrieveDrugsListBasedOnWorkflowType(List<Rx> currentRxList, List<Rx> scriptPadMeds, PrescriptionTaskType currentTaskType)
        {
            return DURMedispanUtils.RetrieveDrugsListBasedOnWorkflowType(
               currentRxList,
                scriptPadMeds,
                currentTaskType);
        }
       public bool IsAnyDurSettingOn(Allscripts.ePrescribe.Objects.DURSettings durSettings)
        {
            return DURMedispanUtils.IsAnyDurSettingOn(durSettings);
        }
        public List<FreeFormRxDrug> RetrieveFreeFormDrugs(List<Rx> rxList)
        {
            return DURMedispanUtils.RetrieveFreeFormDrugs(rxList);
        }
        public bool FoodInteractionsHasItems(DURCheckResponse durWarning)
        {
            return durWarning.FoodInteractions.Interactions.HasItems();
        }
        public bool RetrieveFreeFormDrugsHasItems(List<Rx> rxList)
        {
            return RetrieveFreeFormDrugs(rxList).HasItems();
        }
        public bool AlcoholInteractionsHasItems(DURCheckResponse durWarning)
        {
            return durWarning.AlcoholInteractions.Interactions.HasItems();
        }
        public bool PriorAdverseReactionsHasItems(DURCheckResponse durWarning)
        {
            return durWarning.PriorAdverseReactions.Reactions.HasItems();
        }
        public bool DuplicateTherapyHasItems(DURCheckResponse durWarning)
        {
            return durWarning.DuplicateTherapy.Results.HasItems();
        }
        public bool DrugInteractionsHasItems(DURCheckResponse durWarning)
        {
            return durWarning.DrugInteractions.Interactions.HasItems();
        }
        public bool DurWarningsHasItems(DURCheckResponse durWarning)
        {
            return durWarning.Warnings.HasItems();
        }
        public bool DosageCheckMedicationsHasItems(DURCheckResponse durWarning)
        {
            return durWarning.Dosage.DosageCheckMedications.HasItems();
        }
    }
    public class DURResponse
    {
        public string MedispanCopyright { get; set; }
        public DURCheckResponse DurCheckResponse { get; set; }

    }
}