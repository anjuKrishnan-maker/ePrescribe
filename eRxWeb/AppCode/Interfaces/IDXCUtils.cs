using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace eRxWeb.AppCode.Interfaces
{
    public interface IDXCUtils
    {
        DataTable GetSiteInformationForDXCQueries(string sessionLicenseId, ConnectionStringPointer baseDBID);
        ePrescribeSvc.RxUser GetRxUserForDXCQueries(string userID, string licenseID, ConnectionStringPointer dbID);
        DataSet GetPatientForDXCQueries(string patientID, string licenseID, string userID, ConnectionStringPointer dbID);
        string GetNPIForDXCQueries(IStateContainer pageState);
        void UpdateRCPatientInformationFromScriptPad(string PatientId, string LicenseId, string UserId, ConnectionStringPointer DBID, string PatientFirstName, string PatientLastName, string Gender, ISpecialtyMed iSpecialtyMedsWorkflow, ref DXCRecommendationContext context);
        void UpdateRCProviderInformationFromScriptPad(IStateContainer pageState, ISpecialtyMed iSpecialtyMedsWorkflow, ConnectionStringPointer dbID, ref DXCRecommendationContext context);
        void UpdateRCOrganizationInformationFromScriptPad(string LicenseId, int siteId, ConnectionStringPointer baseDBID, ISpecialtyMed iSpecialtyMedsWorkflow, ref DXCRecommendationContext context);
    }
}
