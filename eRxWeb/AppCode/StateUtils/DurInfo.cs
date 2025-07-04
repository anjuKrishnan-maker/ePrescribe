using System.Collections.Generic;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.State;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Constants = Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.StateUtils
{
    public class DurInfo
    {
        public static IPrescriptionDurHandler RetrievePrescriptionDurHandler(IStateContainer session)

        {
            return new PrescriptionDurHandler(
                    session.GetStringOrEmpty(Constants.SessionVariables.PatientDOB),
                    session.GetStringOrEmpty(Constants.SessionVariables.Gender),
                    session.Cast(Constants.SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
                    session.Cast(Constants.SessionVariables.DurPatientAllergies, new List<Allergy>()),
                    session.Cast(Constants.SessionVariables.DURSettings, new DURSettings()));

        }

        public static List<Allergy> GetDurPatientAllergies(IStateContainer session)
        {
            string userID = UserInfo.GetSessionUserID(session);
            string patientId = PatientInfo.GetSessionPatientID(session);
            string licenseID = UserInfo.GetSessionLicenseID(session);

            var allergies = session.Cast(Constants.SessionVariables.DurPatientAllergies, new List<Allergy>());
            if (!allergies.HasItems())
            {
                allergies = DURMedispanUtils.RetrieveAllergy(patientId, licenseID, userID, (ConnectionStringPointer)session["DBID"]);
                session[Constants.SessionVariables.DurPatientAllergies] = allergies;
            }
            return allergies;
        }

        public static DURSettings GetDURSettings(IStateContainer session)
        {
            DURSettings settings = new DURSettings();
            settings = (DURSettings)session[Constants.SessionVariables.DURSettings];
            return settings;
        }



    }
}