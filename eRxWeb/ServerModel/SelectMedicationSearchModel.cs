using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;

namespace eRxWeb.ServerModel
{
    public class SelectMedicationSearchModel
    {
    }

    public class RetrieveTaskTypeParameters
    {
        public Constants.PrescriptionTaskType PrescriptionTaskType { get; set; }
        public bool IsCsRefReqWorkflow { get; set; }
        public bool IsCsRxChgWorkflow { get; set; }
        public bool IsReconcileREFREQNonCS { get; set; }
        public RetrieveTaskTypeParameters(Constants.PrescriptionTaskType taskType, bool isCsRefReqWorkflow, bool isCsRxChgWorkflow, bool isReconcileREFREQNonCS)
        {
            this.PrescriptionTaskType = taskType;
            this.IsCsRefReqWorkflow = isCsRefReqWorkflow;
            this.IsCsRxChgWorkflow = isCsRxChgWorkflow;
            this.IsReconcileREFREQNonCS = isReconcileREFREQNonCS;
        }
    }
    public class SelectMedicationSearchStartUpParameters
    {
        public string DoctorHistoryOptionDisplayText { get; set; }
        public bool IsShowPreBuiltGroup { get; set; }
        public bool IsSearchDisabled { get; set; }

        public SelectMedicationSearchStartUpParameters(string doctorHistoryOptionDisplayText, bool isShowPreBuiltGroup, bool isCSTask)
        {
            this.DoctorHistoryOptionDisplayText = doctorHistoryOptionDisplayText;
            this.IsShowPreBuiltGroup = isShowPreBuiltGroup;
            this.IsSearchDisabled = isCSTask;
        }
    }
}