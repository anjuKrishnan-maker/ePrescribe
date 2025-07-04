using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Data;
using Provider = Allscripts.Impact.Provider;

namespace eRxWeb.AppCode
{
    public class AssistRx : IAssistRx
    {
        public DataTable LoadProviderDetails(string delegateProviderID, string licenseID, ConnectionStringPointer dbID)
        {
            return Provider.LoadProviderDetailsForIDProofing(delegateProviderID, licenseID, dbID);
        }

        public string NPIFromDelegateProvider(DataTable dtProviderDetails)
        {
            string NPI = string.Empty;
            if (dtProviderDetails != null && dtProviderDetails.Rows.Count > 0)
            {
                NPI = dtProviderDetails.Rows[0]["NPI"].ToString();
            }
            return NPI;
        }

        public string RetrieveProviderIDForAssistRx(Constants.UserCategory userType, string mainProvNPI, DataTable dtProviderDetails)
        {
            string providerIDForAssistRx = string.Empty;
            switch (userType)
            {
                case Constants.UserCategory.PROVIDER:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    {
                        providerIDForAssistRx = mainProvNPI;// ;
                        break;
                    }
                case Constants.UserCategory.POB_SUPER:
                case Constants.UserCategory.POB_REGULAR:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                case Constants.UserCategory.POB_LIMITED:
                    {
                        providerIDForAssistRx = NPIFromDelegateProvider(dtProviderDetails);
                        break;
                    }
                default:
                    break;
            }
            return providerIDForAssistRx;
        }
        public static string GetNPIForAssistRx(Guid delegateProviderID, Guid LicenseId, ConnectionStringPointer dbId, Constants.UserCategory userType, string NPI)
        {
            AssistRx specialtyMeds = new AssistRx();
            DataTable dtProviderDetails = specialtyMeds.LoadProviderDetails(
                                                                        delegateProviderID.ToString(),
                                                                        LicenseId.ToString(),
                                                                        dbId
                                                                    );
            string NPIForAssistRx = specialtyMeds.RetrieveProviderIDForAssistRx(
                                                                        userType,
                                                                        NPI,
                                                                        dtProviderDetails
                                                                    );
            return NPIForAssistRx;
        }
        public AssistRxWorkflow GetUpdatedAssistRxWorkflow(bool isProviderEnrolledInAssistRx, Guid userId, ConnectionStringPointer dbId, Guid LicenseId, Guid delegateProviderID, Constants.UserCategory userType, string NPI)
        {
            return new Allscripts.ePrescribe.Objects.AssistRxWorkflow
            {
                IsProviderEnrolledInAssistRx = isProviderEnrolledInAssistRx,
                LicenseId = LicenseId,
                UserId = userId,
                DbId = dbId,
                NPIForAssistRx = GetNPIForAssistRx(delegateProviderID, LicenseId, dbId, userType, NPI),
                DelegateProviderId = delegateProviderID
            }; 
        }

        private Int64 createRxTask(string rxID, Constants.PrescriptionTaskType taskType, string licenseID, string userID, string relatedUserID, string comments, string MessageData, ConnectionStringPointer dbID)
        {
            object ret = Allscripts.ePrescribe.Data.Prescription.TaskInsert
                                                                            (rxID,
                                                                                licenseID,
                                                                                Convert.ToInt32(taskType),
                                                                                relatedUserID,
                                                                                Convert.ToInt32(Constants.PrescriptionTaskStatus.NEW),
                                                                                comments,
                                                                                userID,
                                                                                null,
                                                                                MessageData,
                                                                                dbID
                                                                            );
            return Convert.ToInt64(ret);
        }

        public Int64 SendToSPRXTasksList(string rxID, string licenseID, string userID, ConnectionStringPointer dbID)
        {
            //need to create RX_TASK
            return createRxTask(rxID, Constants.PrescriptionTaskType.SPECIALTY_MED_ASSIST_RX, licenseID, userID, null, null, null, dbID);
        }
    }
}