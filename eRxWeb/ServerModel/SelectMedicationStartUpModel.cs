using Allscripts.ePrescribe.Data.Model;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel.Common;
using eRxWeb.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace eRxWeb.ServerModel
{
    public class SelectMedicationStartUpModel
    {
        public List<MessageModel> Messages { get; set; }
        public PatientCoverageHeaderList PatientCoverages { get; set; }
        public string PatientDiagnosis { get; set; }
    }
}