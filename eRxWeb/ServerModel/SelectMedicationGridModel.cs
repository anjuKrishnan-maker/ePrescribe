using System.Collections.Generic;
using eRxWeb.ServerModel.Request;
using System.Linq;

namespace eRxWeb.ServerModel
{
    public class SelectMedicationGridModel
    {
        public SelectMedicationGridModel()
        {
            this.Meds = new List<SelectMedicationMedModel>();
        }
        public bool IsAllMedSearch { get { return this.RequestFor == SelectMedicationRequestType.AllMedication; } }
        public List<SelectMedicationMedModel> Meds { get; set; }
        public List<MessageModel> Messages { get; internal set; }
        public SelectMedicationRequestType RequestFor { get; set; }
        public PatientRecordStatus PatientRecordStatus { get; set; }
    }
}