using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class ScriptPadUtil
    {
        public static void RemoveScript(IStateContainer pageState, string rxID)
        {
            var ScriptPadMeds = GetScriptPadMeds(pageState);
            for (int i = 0; i < ScriptPadMeds.Count; i++)
            {
                if ((ScriptPadMeds[i] as Rx).RxID == rxID)
                {

                    ScriptPadMeds.RemoveAt(i);
                    PptPlusBPL.PPTPlus.RemoveScriptPadMedByRxID(pageState, new PptPlus(), Guid.Parse(rxID));
                    Prescription.Delete(rxID, ApiHelper.GetDBID(pageState));
                    break;
                }
            }
            Prescription.Delete(rxID, ApiHelper.GetDBID(pageState));
        }

        public static List<Rx> GetScriptPadMeds(IStateContainer pageState)
        {

            string userID = string.Empty;
            userID = ApiHelper.GetSessionUserID(pageState);
            var rxList = pageState.Cast<List<Rx>>("CURRENT_SCRIPT_PAD_MEDS", null);
            if (rxList == null)
            {
                rxList = new List<Rx>();
                DataTable dt = CHPatient.GetScriptPad(ApiHelper.GetSessionPatientId(pageState), ApiHelper.GetSessionLicenseID(pageState),
                    userID, ApiHelper.GetSessionPracticeState(pageState), ApiHelper.GetDBID(pageState));

                foreach (DataRow dr in dt.Rows)
                {
                    Rx rx = new Rx(dr);
                    rxList.Add(rx);
                }

            }

            return rxList;

        }

        public static void RemoveAllRxFromScriptPad (IStateContainer pageState)
        {
            //get all scripts 
            var ScriptPadMeds = GetScriptPadMeds(pageState);

            foreach (var item in ScriptPadMeds)
            {
                PptPlusBPL.PPTPlus.RemoveScriptPadMedByRxID(pageState, new PptPlus(), Guid.Parse(item.RxID));
                Prescription.Delete(item.RxID, ApiHelper.GetDBID(pageState));
            }

            ScriptPadMeds.Clear();
        }

        /**
         * Checks if the state CS code is less than the federal CS code,
         * if it is, check if the refill quantity conforms to CS refill rules
         * 
         * If the quatity is acceptable, return true
         * Otherwise return false AND display an error
         * 
         * @params: rx - the prescription being checked
         */
        public static bool IsRefillQuantConsistentWithStateCS(Rx rx, IPharmacy pharm, IPrescription script, Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer dbid, out string stateCSCode, out string destinationState, out string refillText)
        {
            DataSet dsPharmacy = pharm.LoadPharmacyById(rx.PharmacyID, dbid);

            destinationState = dsPharmacy.Tables["Pharmacy"].Rows[0]["State"].ToString();

            stateCSCode = script.GetStateControlledSubstanceCode(rx.DDI, null, destinationState, dbid);

            refillText = null;

            int stateCS;

            //If no state cs code then no need to check because fed CS refill rules were checked on Sig page
            if (!int.TryParse(stateCSCode, out stateCS))
            {
                return true;
            }

            int fedCS;
            int.TryParse(rx.ControlledSubstanceCode, out fedCS);

            if (fedCS == 0 || stateCS < fedCS)
            {
                switch (stateCS)
                {
                    case 5:
                    case 4:
                    case 3:
                        if (rx.Refills > 5)
                        {
                            refillText = "no more than 5";
                        }
                        break;
                    case 2:
                        if (rx.Refills > 0)
                        {
                            refillText = "exactly 0";
                        }
                        break;
                }
                if (!string.IsNullOrWhiteSpace(refillText))
                {
                    return false;
                }
            }
            return true;

        }
    }
}