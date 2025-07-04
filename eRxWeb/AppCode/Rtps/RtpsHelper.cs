using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.Impact.PPTPlusBPL;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode.Rtps
{
    public class RtpsHelper: IRtpsHelper
    {
        public void CheckAndSendDisposition(string userId, 
            string licenseId, 
            string patientId, 
            string relatesToTxId, 
            Disposition disposition, 
            IRtpsDisposition rtpsDisposition,
            ConnectionStringPointer dbId)
        {
            if (!GetDispositionConfigValue(disposition))
                return;
            if (string.IsNullOrWhiteSpace(relatesToTxId))
                return;
            rtpsDisposition.RtpsSendDisposition(
                            userId,
                            licenseId,
                            patientId,
                            Guid.NewGuid().ToString(),
                            relatesToTxId,
                            disposition,
                            dbId);
        }

        private bool GetDispositionConfigValue(Disposition disposition)
        {
            bool configValue = false;

            if (disposition == Disposition.D)
                configValue = Allscripts.Impact.ConfigKeys.RTPSRxDispositionDisplayed;
            else if (disposition == Disposition.A)
                configValue = Allscripts.Impact.ConfigKeys.RTPSRxDispositionAlternateMedication;

            return configValue;
        }
    }
}
