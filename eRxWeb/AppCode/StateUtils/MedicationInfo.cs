using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using RxUser = Allscripts.Impact.RxUser;
using Rx = Allscripts.Impact.Rx;
using System.Collections;

namespace eRxWeb.AppCode.StateUtils
{
    public static class MedicationInfo
    {
        public static List<Rx> GetScriptPadMeds(IStateContainer session)
        {
            List<Rx> rxList = new List<Rx>();
            string userID = string.Empty;
            string patientId = string.Empty;
            string licenseID = string.Empty;
            string practiceState = string.Empty;
            userID = UserInfo.GetSessionUserID(session);
            patientId = PatientInfo.GetSessionPatientID(session);
            licenseID = UserInfo.GetSessionLicenseID(session);
            practiceState = UserInfo.GetSessionPracticeState(session);

            if (session["CURRENT_SCRIPT_PAD_MEDS"] == null)
            {
                DataTable scripts = CHPatient.GetScriptPad(patientId, licenseID, userID, practiceState, (ConnectionStringPointer)session["DBID"]);

                foreach (DataRow script in scripts.Rows)
                {
                    Rx rx = new Rx(script);
                    rxList.Add(rx);
                }

                session["CURRENT_SCRIPT_PAD_MEDS"] = rxList;
            }
            else
            {
                rxList = session["CURRENT_SCRIPT_PAD_MEDS"] as List<Rx>;
            }

            return rxList;
        }

        public static Rx CurrentRx (IStateContainer state)
        {
            Rx rx = new Rx();

            if (state["RxList"] != null)
            {
                ArrayList rxList = (ArrayList)state["RxList"];
                rx = (Rx)rxList[0];
            }

            return rx;
        }

        public static void ClearMedInfo(IStateContainer Session, bool clearDx)
        {
            Session.Remove("RxList");
            Session.Remove("ControlledSubstanceCode");
            Session.Remove("DDI");
            Session.Remove("Package");
            Session.Remove("PackageSize");
            Session.Remove("PackageUOM");
            Session.Remove("PackageQuantity");
            Session.Remove("PackageDescription");
            Session.Remove("DAW");
            Session.Remove("Refill");
            Session.Remove("DaysSupply");
            Session.Remove("Package");
            Session.Remove("DosageForm");
            Session.Remove("RouteOfAdmin");
            Session.Remove("MEDICATIONNAME");
            Session.Remove("NOTES");
            Session.Remove("PharmacyInfo");
            Session.Remove("ORIGINALDDI");
            Session.Remove("STRENGTH");
            Session.Remove("STRENGTHUOM");
            Session.Remove("GPPC");
            Session.Remove("QUANTITY");
            Session.Remove("SIGTEXT");
            Session.Remove("SIGID");
            Session.Remove("FormularyStatus");
            Session.Remove("SourceFormularyStatus");
            Session.Remove("IsOTC");
            Session.Remove("LevelOfPreferedness");
            Session.Remove("OriginalRxID");
            Session.Remove("RX_WORKFLOW");
            if (clearDx)
            {
                Session.Remove("DIAGNOSIS");
                Session.Remove("ICD10CODE");
            }
        }
    }
}