using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Objects;
using Patient = Allscripts.Impact.Patient;
using Guid = System.Guid;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.State;
using ISpecialtyMed = eRxWeb.AppCode.Interfaces.ISpecialtyMed;

namespace eRxWeb.AppCode
{
    public class DXCUtils : IDXCUtils
    {
        public DataTable GetSiteInformationForDXCQueries(string sessionLicenseId, ConnectionStringPointer baseDBID)
        {
            DataTable dtLicense = new DataTable();
            if (sessionLicenseId != null)
            {
                dtLicense = ApplicationLicense.Load(sessionLicenseId, baseDBID).Tables[0];
            }
            return dtLicense;
        }
        public ePrescribeSvc.RxUser GetRxUserForDXCQueries(string userID, string licenseID, ConnectionStringPointer dbID)
        {
            ePrescribeSvc.RxUser rxUser = new ePrescribeSvc.RxUser();
            ePrescribeSvc.GetUserResponse getUserRes = EPSBroker.GetRxUser(
                       ePrescribeSvc.ValueType.UserGUID,
                       userID,
                       licenseID,
                       userID,
                       licenseID,
                       dbID);

            if (getUserRes.RxUser != null)
            {
                rxUser = getUserRes.RxUser;
            }
            return rxUser;
        }

        public DataSet GetPatientForDXCQueries(string patientID, string licenseID, string userID, ConnectionStringPointer dbID)
        {
            Patient pat1 = new Patient(
                                        patientID,
                                        true,
                                        false,
                                        licenseID,
                                        userID,
                                        dbID);
            DataSet pat1DS = pat1.DS;
            return pat1DS;
        }

        public string GetNPIForDXCQueries(IStateContainer pageState) 
        {
            var NPI = new SpecialtyMed().GetNPI(pageState);//dont call DB if already in session
            return NPI;
        }

        public void UpdateRCPatientInformationFromScriptPad(string PatientId, string LicenseId, string UserId, ConnectionStringPointer DBID, string PatientFirstName, string PatientLastName, string Gender, ISpecialtyMed iSpecialtyMedsWorkflow, ref DXCRecommendationContext context)
        {
            DataSet patientDS = GetPatientForDXCQueries(PatientId, LicenseId, UserId, DBID);
            iSpecialtyMedsWorkflow.UpdateDXCRecommendationContextPatientInformation(
                                                                                patientDS,
                                                                                PatientFirstName,
                                                                                PatientLastName,
                                                                                Gender,
                                                                                ref context);
        }

        public void UpdateRCProviderInformationFromScriptPad(IStateContainer pageState, ISpecialtyMed iSpecialtyMedsWorkflow, ConnectionStringPointer dbID, ref DXCRecommendationContext context)
        {
            string NPI = GetNPIForDXCQueries(pageState);
            ePrescribeSvc.RxUser rxUser = GetRxUserForDXCQueries(
                pageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                dbID);
            iSpecialtyMedsWorkflow.UpdateDXCRecommendationContextProviderInformation(
                                                                                pageState.GetStringOrEmpty(Constants.SessionVariables.DEA),
                                                                                NPI,
                                                                                rxUser,
                                                                                string.Empty,
                                                                                ref context);
        }

        public void UpdateRCOrganizationInformationFromScriptPad(string LicenseId, int siteId, ConnectionStringPointer baseDBID, ISpecialtyMed iSpecialtyMedsWorkflow, ref DXCRecommendationContext context)
        {
            DataTable dtSiteInformation = GetSiteInformationForDXCQueries(LicenseId, baseDBID);
            iSpecialtyMedsWorkflow.UpdateDXCRecommendationContextOrganizationInformation(
                                                                                dtSiteInformation,
                                                                                siteId,
                                                                                ref context);
        }
    }
}