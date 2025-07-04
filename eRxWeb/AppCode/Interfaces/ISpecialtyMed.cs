using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using System.Data;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using eRxWeb.State;
using Allscripts.Impact.services.SpecialtyMedUtils;
using System.Web.UI.WebControls;
using Allscripts.Impact.Interfaces;
using Telerik.Web.UI;
using DXCRecommendationContext = eRxWeb.AppCode.SpecialtyMedWorkflow.DXCRecommendationContext;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ISpecialtyMed
    {
        DataTable LoadProviderDetails(string delegateProviderID, string licenseID, ConnectionStringPointer dbID);
        string NPIFromDelegateProvider(DataTable dtProviderDetails);
        string RetrieveProviderIDForSpecialtyMed(Constants.UserCategory userType, string mainProvNPI, DataTable dtProviderDetails);
        Int64 SendToSpecialtyMedTasksList(string rxID, string licenseID, string userID, ConnectionStringPointer dbID);
        ePrescribeSvc.StatusMessageResult GetStatusUpdate(DXCRecommendationContext dxcRecommendationContext, ConnectionStringPointer dbID);
        ePrescribeSvc.InitiationResult SendProcessInitiationRequest(DXCRecommendationContext dxcRecommendationContext, DXCPatientQueryResult dxcPatientQueryResult, ConnectionStringPointer dbID);
        void UpdateDXCRecommendationContextSessionInformation(string LicenseID, Guid UserID, ConnectionStringPointer dbID, ref DXCRecommendationContext recommendationContext);
        void UpdateDXCRecommendationContextPatientInformation(DataSet patientDS, string patientFirstName, string patientLastName, string patientGender, ref DXCRecommendationContext recommendationContext);
        void UpdateDXCRecommendationContextProviderInformation(string DEA, string NPI, ePrescribeSvc.RxUser rxUser, string providerDegree, ref DXCRecommendationContext recommendationContext);
        void UpdateDXCRecommendationContextOrganizationInformation(DataTable dtSiteInformation, int siteId, ref DXCRecommendationContext recommendationContext);
        void UpdateDXCPatientQueryRequestMedicationInformation(Allscripts.Impact.Rx rx, DataTable dtMeds, string frequency, ref DXCPatientQueryResult patientQueryResult);
        void UpdateDXCPatientQueryRequestInsuranceInformation(DataSet patDS, ref DXCPatientQueryResult patientQueryResult);
        void UpdateDXCPatientQueryRequestDiagnosisInformation(string diagnosis, DataSet drRxDetails, ref DXCPatientQueryResult patientQueryResult);
        DataSet SpecialtyMedLoadAttachmentsCountPerRxTaskID(ConnectionStringPointer dbID);
        DataSet SpecialtyMedRetrieveDocumentsPerRxTaskID(Int64 rxTaskID, ConnectionStringPointer dbID);
        string GetNPI(IStateContainer pageState);
        void SaveInitiationResult(Int64 rxTaskID, ePrescribeSvc.InitiationResult initiationResult, ConnectionStringPointer dbID);
        Dictionary<string,string> RetrieveDestinationsForSpecialtyMedTasks(ISpecialtyMedDestinationOptionsParameters parameters);
        Dictionary<AttachmentTypes, bool> RetrieveDocumentsVisibility(DataSet documents);
        SpecialtyMedTaskProcessResult ProcessTask(ITelerik tempDataItem, IStateContainer pageState, string ipaddress);

        SpecialtyMedTaskProcessResult ProcessMailOrder(string selectedRxID, int selectedRefills, int selectedDaysSupply,
            IStateContainer pageState, string ipaddress,  ISpecialtyMed ispecialtymed);
        bool IsPatientEligibleForMailOrderBenefit(IStateContainer pageState);
        SpecialtyMedTaskProcessResult ProcessPrint(string selectedRxID, IStateContainer pageState);
        SpecialtyMedTaskProcessResult ProcessEnteredInError(string selectedRxID, IStateContainer pageState);
        SpecialtyMedTaskProcessResult SendPrescription(string rxID, string rxStatus, int refills, int daysSupply, string pharmacy, bool isLimitedOrMailOrderPharmacy, IStateContainer pageState, string ipaddress, IScriptMessage iScriptMessage, IPrescription iPrescription, IEPSBroker iEPSBroker, ConnectionStringPointer DBID);
    }
}
