using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eRxWeb
{
    internal class MassOpiateMessage : IMassOpiateMessage
    {
        public string GenerateMassOpiateMessage(string controlledSubstanceCode, string previousNotes, string state, string GPI, bool isDEAExpired, List<string> schedulesAllowed)
        {
            IPrescription prescription = new Prescription();
            if (prescription.IsValidMassOpiate(
                 state,
                 GPI,
                 controlledSubstanceCode,
                 isDEAExpired,
                 schedulesAllowed) &&
                 (previousNotes == null || previousNotes.Contains(Constants.PartialFillUponPatientRequest)))
            {
                StringBuilder sbComments = new StringBuilder();
                sbComments.Append(Constants.PartialFillUponPatientRequest);
                if (!string.IsNullOrEmpty(previousNotes))
                    sbComments.Append("<BR>" + previousNotes);
                return sbComments.ToString();
            }
            return String.IsNullOrEmpty(previousNotes) ? "" : previousNotes;
        }
    }
}