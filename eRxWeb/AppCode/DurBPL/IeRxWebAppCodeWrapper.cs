using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Request;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.Impact;
using System.Collections.Generic;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.DurBPL
{
    public interface IeRxWebAppCodeWrapper
    {
        DURResponse GetDurWarnings(string patientDob, string patientGender, List<Allscripts.Impact.Rx> currentRxs, List<string> activeRxList,List<Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy> allergies, Allscripts.ePrescribe.Objects.DURSettings durSettings);
        DURCheckResponse PerformDURCheck(DURCheckRequest request);
        PrescriptionTaskType RetrieveTaskType(RxTaskModel rxTask, PrescriptionTaskType TaskType);
        List<Rx> RetrieveDrugsListBasedOnWorkflowType(List<Rx> currentRxList, List<Rx> scriptPadMeds, Allscripts.ePrescribe.Common.Constants.PrescriptionTaskType currentTaskType);
        bool IsAnyDurSettingOn(Allscripts.ePrescribe.Objects.DURSettings durSettings);
        List<FreeFormRxDrug> RetrieveFreeFormDrugs(List<Rx> rxList);
        bool FoodInteractionsHasItems(DURCheckResponse durwarning);
        bool RetrieveFreeFormDrugsHasItems(List<Rx> rxList);
        bool AlcoholInteractionsHasItems(DURCheckResponse durwarning);
        bool PriorAdverseReactionsHasItems(DURCheckResponse durwarning);
        bool DuplicateTherapyHasItems(DURCheckResponse durwarning);
        bool DrugInteractionsHasItems(DURCheckResponse durwarning);
        bool DurWarningsHasItems(DURCheckResponse durwarning);
        bool DosageCheckMedicationsHasItems(DURCheckResponse durwarning);
    }
}