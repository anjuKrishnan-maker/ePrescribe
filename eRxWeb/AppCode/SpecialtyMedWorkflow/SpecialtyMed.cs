using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Data;
using Provider = Allscripts.Impact.Provider;
using eRxWeb.AppCode.SpecialtyMedWorkflow;
using Prescription = Allscripts.ePrescribe.Data.Prescription;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;
using eRxWeb.AppCode.Interfaces;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;
using ISpecialtyMed = eRxWeb.AppCode.Interfaces.ISpecialtyMed;
using Rx = Allscripts.Impact.Rx;
using RxUser = eRxWeb.ePrescribeSvc.RxUser;
using Patient = Allscripts.Impact.Patient;
using Allscripts.Impact.services.SpecialtyMedUtils;
using DXCRecommendationContext = eRxWeb.AppCode.SpecialtyMedWorkflow.DXCRecommendationContext;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using impactPrescription = Allscripts.Impact.Prescription;

namespace eRxWeb.AppCode
{
    public class SpecialtyMed : ISpecialtyMed
    {

        private const string _aTagFormat = @"<a title=""You are now leaving the Veradigm site to a site operated by a third party."" class=""linkButton"" onclick=""confirmOffer('{0}');connectToiFC('{1}', '{2}', '{0}', '{3}')"">{4}</a>";

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

        public string RetrieveProviderIDForSpecialtyMed(Constants.UserCategory userType, string mainProvNPI, DataTable dtProviderDetails)
        {
            string providerIDForSpecialtyMed = string.Empty;
            switch (userType)
            {
                case Constants.UserCategory.PROVIDER:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT:
                    {
                        providerIDForSpecialtyMed = mainProvNPI;// ;
                        break;
                    }
                case Constants.UserCategory.POB_SUPER:
                case Constants.UserCategory.POB_REGULAR:
                case Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED:
                case Constants.UserCategory.POB_LIMITED:
                    {
                        providerIDForSpecialtyMed = NPIFromDelegateProvider(dtProviderDetails);
                        break;
                    }
                default:
                    break;
            }
            return providerIDForSpecialtyMed;
        }

        private long createRxTask(string rxID, Constants.PrescriptionTaskType taskType, string licenseID, string userID, string relatedUserID, string comments, string MessageData, ConnectionStringPointer dbID)
        {
            object ret = Prescription.TaskInsert
                                                                            (rxID,
                                                                                licenseID,
                                                                                Convert.ToInt32(taskType),
                                                                                relatedUserID,
                                                                                Convert.ToInt32(Constants.PrescriptionStatus.NEW),
                                                                                comments,
                                                                                userID,
                                                                                null,
                                                                                MessageData,
                                                                                dbID
                                                                            );
            return Convert.ToInt64(ret);
        }

        public long SendToSpecialtyMedTasksList(string rxID, string licenseID, string userID, ConnectionStringPointer dbID)
        {
            //need to create RX_TASK
            return createRxTask(rxID, Constants.PrescriptionTaskType.SPECIALTY_MED_ASSIST_RX, licenseID, userID, null, null, null, dbID);
        }
        public StatusMessageResult GetStatusUpdate(DXCRecommendationContext dxcRecommendationContext, ConnectionStringPointer dbID)
        {
            return EPSBroker.GetStatusUpdate(dxcRecommendationContext, dbID);
        }
        public InitiationResult SendProcessInitiationRequest(DXCRecommendationContext dxcRecommendationContext, DXCPatientQueryResult dxcPatientQueryResult, ConnectionStringPointer dbID)
        {
            //need to create RX_TASK
            return EPSBroker.ProcessInitiationRequest(dxcRecommendationContext, dxcPatientQueryResult, dbID);
        }
        public void UpdateDXCRecommendationContextSessionInformation(string LicenseID, Guid UserID, ConnectionStringPointer dbID, ref DXCRecommendationContext recommendationContext)
        {
            recommendationContext.LicenseID = LicenseID;
            recommendationContext.DbID = dbID;
        }
        public void UpdateDXCRecommendationContextPatientInformation(DataSet patientDS, string patientFirstName, string patientLastName, string patientGender, ref DXCRecommendationContext recommendationContext)
        {
            if (patientDS != null && patientDS.Tables["Patient"].Rows.Count > 0)
            {
                recommendationContext.PatientMRN = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["ChartID"].ToString());
                recommendationContext.PatientID = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["PatientGUID"].ToString());
                recommendationContext.PatientDOB = Convert.ToDateTime(patientDS.Tables["Patient"].Rows[0]["DOB"].ToString());
                //Patient Address
                recommendationContext.PatientStreetAddress1 = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["Address1"].ToString());
                recommendationContext.PatientStreetAddress2 = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["Address2"].ToString());
                recommendationContext.PatientCity = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["City"].ToString());
                recommendationContext.PatientState = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["State"].ToString());
                recommendationContext.PatientPostalCode = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["Zip"].ToString());
                recommendationContext.PatientTelephone = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["Phone"].ToString());
                recommendationContext.PatientMobile = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["MobilePhone"].ToString());
                recommendationContext.PatientEmail = Helper.GetStringOrEmpty(patientDS.Tables["Patient"].Rows[0]["Email"].ToString());
            }
            else
            {
                recommendationContext.PatientMRN = string.Empty;
                recommendationContext.PatientID = string.Empty;
                recommendationContext.PatientDOB = DateTime.UtcNow;
                //Patient Address
                recommendationContext.PatientStreetAddress1 = string.Empty;
                recommendationContext.PatientStreetAddress2 = string.Empty;
                recommendationContext.PatientCity = string.Empty;
                recommendationContext.PatientState = string.Empty;
                recommendationContext.PatientPostalCode = string.Empty;
                recommendationContext.PatientTelephone = string.Empty;
                recommendationContext.PatientMobile = string.Empty;
                recommendationContext.PatientEmail = string.Empty;
            }
            recommendationContext.PatientFirstName = patientFirstName;
            recommendationContext.PatientLastName = patientLastName;
            recommendationContext.PatientGender = patientGender;
        }
        public void UpdateDXCRecommendationContextProviderInformation(string DEA, string NPI, RxUser rxUser, string providerDegree, ref DXCRecommendationContext recommendationContext)
        {
            recommendationContext.DEANumber = DEA;
            recommendationContext.ProviderID = NPI;
            recommendationContext.ProviderFirstName = (rxUser != null && !string.IsNullOrEmpty(rxUser.FirstName)) ? rxUser.FirstName : string.Empty;
            recommendationContext.ProviderLastName = (rxUser != null && !string.IsNullOrEmpty(rxUser.LastName)) ? rxUser.LastName : string.Empty;
            recommendationContext.ProviderDegree = providerDegree;
        }

        public void UpdateDXCRecommendationContextOrganizationInformation(DataTable dtSiteInformation, int siteId, ref DXCRecommendationContext recommendationContext)
        {
            if (dtSiteInformation != null && dtSiteInformation.Rows.Count > 0)
            {
                int siteIdIndex = dtSiteInformation.Rows.OfType<DataRow>().Select(dr => dr.Field<int>("SiteId")).ToList().FindIndex(c => c == siteId); ;
                recommendationContext.OrganizationName = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["PracticeName"].ToString());
                recommendationContext.OrganizationStreetAddress1 = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["Address1"].ToString());
                recommendationContext.OrganizationStreetAddress2 = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["Address2"].ToString());
                recommendationContext.OrganizationCity = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["City"].ToString());
                recommendationContext.OrganizationState = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["State"].ToString());
                recommendationContext.OrganizationPostalCode = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["ZipCode"].ToString());
                recommendationContext.OrganizationPhone = Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["PhoneAreaCode"].ToString()) + Helper.GetStringOrEmpty(dtSiteInformation.Rows[siteIdIndex]["PhoneNumber"].ToString());
            }
            else
            {
                recommendationContext.OrganizationName = string.Empty;
                recommendationContext.OrganizationStreetAddress1 = string.Empty;
                recommendationContext.OrganizationStreetAddress2 = string.Empty;
                recommendationContext.OrganizationCity = string.Empty;
                recommendationContext.OrganizationState = string.Empty;
                recommendationContext.OrganizationPostalCode = string.Empty;
                recommendationContext.OrganizationPhone = string.Empty;
            }
        }
        public void UpdateDXCPatientQueryRequestMedicationInformation(Rx rx, DataTable dtMeds, string frequency, ref DXCPatientQueryResult patientQueryResult)
        {
            patientQueryResult.EndDateSpecified = false;
            if (rx != null)
            {
                patientQueryResult.MedicationCodeValue = Helper.GetStringOrEmpty(rx.DDI);
                if (dtMeds != null && dtMeds.Rows.Count > 0)
                {
                    patientQueryResult.StartDateTime = Convert.ToDateTime(Helper.GetStringOrEmpty(dtMeds.Rows[0]["MedicationStartDate"].ToString()));
                    patientQueryResult.OrderDateTime = Convert.ToDateTime(Helper.GetStringOrEmpty(dtMeds.Rows[0]["MedicationOrderDate"].ToString()));
                    patientQueryResult.MedicationName = Helper.GetStringOrEmpty(dtMeds.Rows[0]["MedicationName"].ToString().TrimEnd());
                    patientQueryResult.DaysSupply = Helper.GetStringOrEmpty(dtMeds.Rows[0]["DaysSupply"].ToString()).ToIntOrZero();
                    patientQueryResult.DAW = Helper.GetStringOrEmpty(dtMeds.Rows[0]["DAW"].ToString()).GetBooleanFromYOrN();
                    patientQueryResult.Quantity = Helper.GetStringOrEmpty(dtMeds.Rows[0]["Quantity"].ToString()).ToIntOrZero();
                    patientQueryResult.BrandName = Helper.GetStringOrEmpty(dtMeds.Rows[0]["BrandName"].ToString());
                    patientQueryResult.PrescriptionID = Helper.GetStringOrEmpty(dtMeds.Rows[0]["RxID"].ToString());
                    patientQueryResult.MedicationCodeValue = Helper.GetStringOrEmpty(rx.DDI);
                    if (!string.IsNullOrEmpty(rx.Strength) && !string.IsNullOrEmpty(rx.StrengthUOM))
                    {
                        patientQueryResult.ProductConcentration = rx.Strength + " " + rx.StrengthUOM;
                    }
                    else
                    {
                        patientQueryResult.ProductConcentration = string.Empty;
                    }
                    patientQueryResult.RefillsTotal = rx.Refills;
                    patientQueryResult.DeliveryMethod = Helper.GetStringOrEmpty(dtMeds.Rows[0]["RouteOfAdminCode"].ToString());
                    patientQueryResult.Administered = false;
                    patientQueryResult.Frequency = frequency;
                    patientQueryResult.Dose = Helper.GetStringOrEmpty(rx.Strength);
                    patientQueryResult.FreeSigText = Helper.GetStringOrEmpty(rx.SigText);
                    patientQueryResult.PharmacyID = string.Empty;
                    patientQueryResult.PharmacyName = string.Empty;
                }
            }
            else
            {
                patientQueryResult.StartDateTime = DateTime.UtcNow;
                patientQueryResult.OrderDateTime = DateTime.UtcNow;
                patientQueryResult.MedicationName = string.Empty;
                patientQueryResult.DaysSupply = 0;
                patientQueryResult.Quantity = 0;
                patientQueryResult.BrandName = string.Empty;
                patientQueryResult.PrescriptionID = string.Empty;
                patientQueryResult.MedicationCodeValue = string.Empty;
                patientQueryResult.ProductConcentration = string.Empty;
                patientQueryResult.RefillsTotal = 0;
                patientQueryResult.DeliveryMethod = string.Empty;
                patientQueryResult.Administered = false;
                patientQueryResult.Frequency = string.Empty;
                patientQueryResult.Dose = string.Empty;
                patientQueryResult.FreeSigText = string.Empty;
                patientQueryResult.PharmacyID = string.Empty;
                patientQueryResult.PharmacyName = string.Empty;
            }
        }
        public void UpdateDXCPatientQueryRequestInsuranceInformation(DataSet patDS, ref DXCPatientQueryResult patientQueryResult)
        {
            if (patDS != null && patDS.Tables["PatientCoverage"] != null && patDS.Tables["PatientCoverage"].Rows.Count > 0)
            {
                patientQueryResult.InsuranceInformationSourceID = Helper.GetStringOrEmpty(patDS.Tables["PatientCoverage"].Rows[0]["InfoSourcePayerID"].ToString());
                patientQueryResult.InsuranceInformationSourceName = Helper.GetStringOrEmpty(patDS.Tables["PatientCoverage"].Rows[0]["InfoSourceName"].ToString());
                patientQueryResult.InsuranceRelationToSubscriber = Helper.GetStringOrEmpty(patDS.Tables["PatientCoverage"].Rows[0]["RelToCardHolder"].ToString());
            }
            else
            {
                patientQueryResult.InsuranceInformationSourceID = string.Empty;
                patientQueryResult.InsuranceInformationSourceName = string.Empty;
                patientQueryResult.InsuranceRelationToSubscriber = string.Empty;
            }
        }

        public void UpdateDXCPatientQueryRequestDiagnosisInformation(string diagnosis, DataSet drRxDetails, ref DXCPatientQueryResult patientQueryResult)
        {
            if (drRxDetails.Tables[0] != null && drRxDetails.Tables[0].Rows.Count > 0)
            {
                patientQueryResult.DiagnosisCodingValue = Helper.GetStringOrEmpty(drRxDetails.Tables[0].Rows[0]["ICD10code"].ToString());
                patientQueryResult.DiagnosisCodingSystem = "ICD10CM";
            }
            else
            {
                patientQueryResult.DiagnosisCodingValue = string.Empty;
                patientQueryResult.DiagnosisCodingSystem = "ICD10CM";
            }
        }
        public string GetNPI(IStateContainer pageState)
        {
            if (pageState == null)
                return string.Empty;
            var thisUserNPI = pageState.GetStringOrEmpty(Constants.SessionVariables.NPI);

            var npi = string.IsNullOrEmpty(thisUserNPI)
                ? pageState.GetStringOrEmpty(Constants.SessionVariables.DelegateProviderNPI)
                : thisUserNPI;
            return npi;
        }
        private void CreateSpecialtyMedTask(long rxTaskID, bool isPatientConsentAchieved, bool isActive, int activityID, string transactionId, string vendorApplicationURL, SpecialtyMedSpecialtyEnrollmentStatus statusEnrollment, SpecialtyMedPriorAuthorizationStatus statusEPA, ConnectionStringPointer dbID)
        {
            Prescription.SpecialtyMedTaskInsert(
                                                                        rxTaskID,
                                                                        isPatientConsentAchieved,
                                                                        isActive,
                                                                        activityID,
                                                                        transactionId,
                                                                        vendorApplicationURL,
                                                                        Convert.ToInt32(statusEnrollment),
                                                                        Convert.ToInt32(statusEPA),
                                                                        dbID
                                                                    );
        }
        public void SaveInitiationResult(long rxTaskID, InitiationResult initiationResult, ConnectionStringPointer dbID)
        {
            if (initiationResult != null)
            {
                bool isPatientConsentAchieved = false;
                bool isActive = true;
                int activityID = initiationResult.ActivityId;
                string vendorApplicationURL = initiationResult.VendorApplicationUrl;
                string transactionId = initiationResult.VendorTransactionId;
                CreateSpecialtyMedTask(rxTaskID, isPatientConsentAchieved, isActive, activityID, transactionId, vendorApplicationURL, SpecialtyMedSpecialtyEnrollmentStatus.READY, SpecialtyMedPriorAuthorizationStatus.READY, dbID);
            }
        }

        public DataSet SpecialtyMedLoadAttachmentsCountPerRxTaskID(ConnectionStringPointer dbID)
        {
            return SpecialtyMeds.SpecialtyMedLoadAttachmentsCountPerRxTaskID(dbID);
        }
        public DataSet SpecialtyMedRetrieveDocumentsPerRxTaskID(long rxTaskID, ConnectionStringPointer dbID)
        {
            return SpecialtyMeds.SpecialtyMedRetrieveDocumentsPerRxTaskID(rxTaskID, dbID);
        }



        public static bool IsProviderEnrolledInSpecialtyMed(IStateContainer pageState)
        {
            if (pageState.ContainsKey(Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed))
            {
                return pageState.GetBoolean(Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed, false);
            }

            var thisUserNPI = pageState.GetStringOrEmpty(Constants.SessionVariables.NPI);

            var npi = string.IsNullOrEmpty(thisUserNPI)
                ? pageState.GetStringOrEmpty(Constants.SessionVariables.DelegateProviderNPI)
                : thisUserNPI;

            var result = EPSBroker.IsProviderEnrolledInSpecialtyMed(pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId), npi, pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT));

            pageState[Constants.SessionVariables.IsProviderEnrolledInSpecialtyMed] = result;

            return result;
        }

        public void ConfirmOffer(IStateContainer pageState, string rxTaskId, ConnectionStringPointer dbID)
        {
            //Confirm Offer Needs RecommendationContext And ConfirmOfferRequest
            DataTable dtConfirmOfferDetails = SpecialtyMeds.SpecialtyMedGetConfirmOfferDetails(Convert.ToInt64(rxTaskId), dbID).Tables[0];
            DXCRecommendationContext context = BuildDXCRecommendationContext(new DXCUtils(), pageState, dtConfirmOfferDetails, dbID);
            DXCConfirmOffer confirmOffer = BuildConfirmOffer(dtConfirmOfferDetails);
            EPSBroker.ConfirmRequestForSpecialtyMeds(context, confirmOffer, dbID);
        }

        public DXCRecommendationContext BuildDXCRecommendationContext(IDXCUtils dxcUtils, IStateContainer PageState, DataTable dtConfirmOfferDetails, ConnectionStringPointer dbID)
        {
            DXCRecommendationContext context = new DXCRecommendationContext();
            //Fill Session
            UpdateDXCRecommendationContextSessionInformation(
                PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                PageState.GetGuidOr0x0(Constants.SessionVariables.UserId),
                dbID,
                ref context
                );

            //Fill Provider
            RxUser rxUser = dxcUtils.GetRxUserForDXCQueries(
                PageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                dbID);
            UpdateDXCRecommendationContextProviderInformation(
                                                                PageState.GetStringOrEmpty(Constants.SessionVariables.DEA),
                                                                PageState.GetStringOrEmpty(Constants.SessionVariables.NPI),
                                                                rxUser,
                                                                string.Empty,
                                                                ref context);
            //Fill Patient
            if (dtConfirmOfferDetails != null && dtConfirmOfferDetails.Rows.Count > 0)
            {
                DataSet patientDS = dxcUtils.GetPatientForDXCQueries(
                    Helper.GetStringOrEmpty(Convert.ToString(dtConfirmOfferDetails.Rows[0]["PatientID"])),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                    PageState.GetStringOrEmpty(Constants.SessionVariables.UserId), dbID);
                UpdateDXCRecommendationContextPatientInformation(
                                                                patientDS,
                                                                Helper.GetStringOrEmpty(Convert.ToString(dtConfirmOfferDetails.Rows[0]["FirstName"])),
                                                                Helper.GetStringOrEmpty(Convert.ToString(dtConfirmOfferDetails.Rows[0]["LastName"])),
                                                                Helper.GetStringOrEmpty(Convert.ToString(dtConfirmOfferDetails.Rows[0]["Sex"])),
                                                                ref context);
            }

            //Fill Organization
            DataTable dtSiteInformation = dxcUtils.GetSiteInformationForDXCQueries(
                PageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                dbID);
            UpdateDXCRecommendationContextOrganizationInformation(
                                                                    dtSiteInformation,
                                                                    Convert.ToInt32(PageState.GetStringOrEmpty(Constants.SessionVariables.SiteId)),
                                                                    ref context);
            return context;
        }

        public DXCConfirmOffer BuildConfirmOffer(DataTable dtConfirmOfferDetails)
        {
            DXCConfirmOffer confirmOffer = new DXCConfirmOffer();
            if (dtConfirmOfferDetails != null && dtConfirmOfferDetails.Rows.Count > 0)
            {
                confirmOffer.ActivityID = Convert.ToInt32(dtConfirmOfferDetails.Rows[0]["ActivityID"]);
                confirmOffer.ConfirmOffer = true;
                confirmOffer.PatientEmail = Helper.GetStringOrEmpty(Convert.ToString(dtConfirmOfferDetails.Rows[0]["Email"]));
                confirmOffer.PatientMobilePhone = Helper.GetStringOrEmpty(Convert.ToString(dtConfirmOfferDetails.Rows[0]["MobilePhone"]));
                confirmOffer.TimeStamp = DateTime.UtcNow;
            }
            return confirmOffer;
        }

        public Dictionary<AttachmentTypes, bool> RetrieveDocumentsVisibility(DataSet documents)
        {
            Dictionary<AttachmentTypes, bool> documentsVisibility = new Dictionary<AttachmentTypes, bool>();
            if (documents != null && documents.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in documents.Tables[0].Rows)
                {
                    if (!documentsVisibility.ContainsKey(AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE) &&
                        Convert.ToInt32(dr["SpecialtyMedAttachmentTypeID"]) == (int)AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE
                        )
                    {
                        documentsVisibility.Add(AttachmentTypes.SPECIALTY_ENROLLMENT_STATUS_FILE, true);
                    }
                    if (!documentsVisibility.ContainsKey(AttachmentTypes.PRIOR_AUTHORIZATION_FILE) &&
                        Convert.ToInt32(dr["SpecialtyMedAttachmentTypeID"]) == (int)AttachmentTypes.PRIOR_AUTHORIZATION_FILE
                        )
                    {
                        documentsVisibility.Add(AttachmentTypes.PRIOR_AUTHORIZATION_FILE, true);
                    }
                }
            }
            return documentsVisibility;
        }

        public Dictionary<string, string> RetrieveDestinationsForSpecialtyMedTasks(ISpecialtyMedDestinationOptionsParameters parameters)
        {
            Dictionary<string, string> destinationOptions = new Dictionary<string, string>();

            //1. 'Send to LDPL' option checker
            bool isSendToLimitedDistributionPharmacyOptionAvailable =
                parameters.IsSendToLimitedDistributionPharmacyOptionAvailable();
            if (isSendToLimitedDistributionPharmacyOptionAvailable)
            {
                destinationOptions.Add(Constants.DefaultDestinationText,
                    Constants.SpecialtyMedDefaultDestinationValue);
                destinationOptions.Add(Patient.SEND_TO_SPECIALTYMED_LIMITED_DISTRIBUTION_PHARMACY,
                    Patient.SPECIALTYMED_LIMITED_DISTRIBUTION_PHARMACY);
            }
            //2. 'Send to Pharmacy' option checker
            bool isSendToPharmacyOptionAvailable = parameters.IsSendToPharmacyOptionAvailable();
            if (isSendToPharmacyOptionAvailable)
            {
                destinationOptions.Add(Patient.SEND_TO_PHARMACY, Patient.PHARM);
            }
            //3. 'Send to mail order' option checker
            bool isSendToMailOrderOptionAvailable = parameters.IsSendToMailOrderOptionAvailable();
            if (isSendToMailOrderOptionAvailable)
            {
                destinationOptions.Add(Patient.SEND_TO_MAIL_ORDER, Patient.MOB);
            }
            //4. 'Print' option 
            bool isPrintOptionAvailable = parameters.IsPrintOptionAvailable();
            if (isPrintOptionAvailable)
            {
                destinationOptions.Add(Patient.PRINT_KEY, Patient.PRINT);
            }
            //5. 'EIE' Entered in Error option 
            bool isEIEOptionAvailable = parameters.IsEIEOptionAvailable();
            if (isEIEOptionAvailable)
            {
                destinationOptions.Add(Patient.ENTERED_IN_ERROR_DESTINATION_OPTION, Patient.ENTERED_IN_ERROR);
            }
            if (destinationOptions.Count == 0)
            {
                destinationOptions.Add(Constants.DestinationProcessedText,
                    Constants.SpecialtyMedProcessedValue);
            }

            return destinationOptions;
        }

        public static void EditDestinationDropdownControl(ITelerik iTelerik)
        {
            var ddlDestination = iTelerik.FindControl("ddlDest") as DropDownList;
            if (ddlDestination != null)
            {
                var hdnDestinationSelected = iTelerik.FindControl("hdnDestinationSelected") as Button;
                var ddi = iTelerik.GetDataKeyValue("DDI") as string;
                if (hdnDestinationSelected != null)
                {
                    hdnDestinationSelected.Attributes.Add("rowIndex", Convert.ToString(iTelerik.RowIndex));
                    hdnDestinationSelected.Attributes.Add("ddi", ddi);
                    ddlDestination.Attributes.Add("onchange", $"showSpecialtyRxPharmacyOverlay({hdnDestinationSelected.ClientID}, {ddlDestination.ClientID});");
                }
            }
        }

        public static void SetActionlinksForStatus(ITelerik iTelerik)
        {
            var url = Convert.ToString(iTelerik.GetDataKeyValue("URL"));
            SetSpecialtyPaStatusLabels(iTelerik, url);
            SetEnrollmentLabels(iTelerik, url);
        }

        public static void SetSpecialtyPaStatusLabels(ITelerik iTelerik, string url)
        {
            var priorAuthStatus = iTelerik.GetDataKeyValue("PriorAuthorizationStatus").ToEnum<SpecialtyMedPriorAuthorizationStatus>();
            var litSpecPaStatus = iTelerik.FindControl("litSpecPAStatus") as Literal;
            var specPaStatus = Convert.ToString(iTelerik.GetDataKeyValue("SpecPAStatus"));
            var rxTaskID = Convert.ToInt64(iTelerik.GetDataKeyValue("RxTaskID"));
            var activityId = Convert.ToInt64(iTelerik.GetDataKeyValue("ActivityID"));
            var patientId = Convert.ToString(iTelerik.GetDataKeyValue("PatientGUID"));

            if (litSpecPaStatus != null && !string.IsNullOrEmpty(url))
            {
                var showActiveLinkButton = priorAuthStatus == SpecialtyMedPriorAuthorizationStatus.SAVED || priorAuthStatus == SpecialtyMedPriorAuthorizationStatus.READY;
                if (showActiveLinkButton)
                {
                    litSpecPaStatus.Text = string.Format(_aTagFormat, rxTaskID, url, activityId, patientId, specPaStatus);
                }
                else
                {
                    litSpecPaStatus.Text = specPaStatus;
                }
            }
        }

        public static void SetEnrollmentLabels(ITelerik iTelerik, string url)
        {
            var enrollmentStatus = iTelerik.GetDataKeyValue("SpecialtyEnrollmentStatus").ToEnum<SpecialtyMedSpecialtyEnrollmentStatus>();
            var litSpecEnrollStatus = iTelerik.FindControl("litSpecEnrollStatus") as Literal;
            var specEnrollStatus = Convert.ToString(iTelerik.GetDataKeyValue("SpecEnrollStatus"));
            var rxTaskID = Convert.ToInt64(iTelerik.GetDataKeyValue("RxTaskID"));
            var activityId = Convert.ToInt64(iTelerik.GetDataKeyValue("ActivityID"));
            var patientId = Convert.ToString(iTelerik.GetDataKeyValue("PatientGUID"));

            if (litSpecEnrollStatus != null && !string.IsNullOrEmpty(url))
            {
                var showActiveLinkButton = enrollmentStatus == SpecialtyMedSpecialtyEnrollmentStatus.READY || enrollmentStatus == SpecialtyMedSpecialtyEnrollmentStatus.SAVED;
                if (showActiveLinkButton)
                {
                    litSpecEnrollStatus.Text = string.Format(_aTagFormat, rxTaskID, url, activityId, patientId, specEnrollStatus);
                }
                else
                {
                    litSpecEnrollStatus.Text = specEnrollStatus;
                }
            }
        }

        public static void SetRadioButton(ITelerik tempDataItem)
        {
            var rbSelectedRow = tempDataItem.FindControl("rbSelectedTask") as HtmlInputRadioButton;
            if (rbSelectedRow != null)
            {
                rbSelectedRow.Attributes.Add("RxTaskID", tempDataItem.GetDataKeyValue("RxTaskID").ToString());
                rbSelectedRow.Attributes.Add("onclick", $"taskSelectedRadio('{tempDataItem.GetDataKeyValue("RxTaskID")}'," +
                                                        $" '{tempDataItem.GetDataKeyValue("PatientGUID")}')");
            }
        }

        public static DXCRecommendationContext GenerateRecommendationContext(ISpecialtyMed iSpecialtyMedWorkflow, IDXCUtils dxcUtils, IStateContainer pageState)
        {
            DXCRecommendationContext context = new DXCRecommendationContext();
            //eRxSession
            iSpecialtyMedWorkflow.UpdateDXCRecommendationContextSessionInformation(
                pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                pageState.GetGuidOr0x0(Constants.SessionVariables.UserId),
                pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT),
                ref context);
            var dbId = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            //Patient
            dxcUtils.UpdateRCPatientInformationFromScriptPad(
                pageState.GetStringOrEmpty(Constants.SessionVariables.PatientId),
                pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                pageState.GetStringOrEmpty(Constants.SessionVariables.UserId),
                dbId,
                pageState.GetStringOrEmpty(Constants.SessionVariables.PatientFirstName),
                pageState.GetStringOrEmpty(Constants.SessionVariables.PatientLastName),
                pageState.GetStringOrEmpty(Constants.SessionVariables.Gender),
                iSpecialtyMedWorkflow,
                ref context);

            //Provider
            dxcUtils.UpdateRCProviderInformationFromScriptPad(
                pageState,
                iSpecialtyMedWorkflow,
                dbId,
                ref context);

            //Organization
            dxcUtils.UpdateRCOrganizationInformationFromScriptPad(
                pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId),
                Convert.ToInt32(pageState.GetStringOrEmpty(Constants.SessionVariables.SiteId)),
                dbId,
                iSpecialtyMedWorkflow,
                ref context);


            return context;

        }
        public SpecialtyMedTaskProcessResult ProcessTask(ITelerik tempDataItem, IStateContainer pageState, string ipaddress)
        {
            string selectedRxID = tempDataItem.GetDataKeyValue("RxID").ToString();
            int selectedRefills = (int)tempDataItem.GetDataKeyValue("RefillQuantity");
            int selectedDaysSupply = (int)tempDataItem.GetDataKeyValue("DaysSupply");
            ConnectionStringPointer DBID = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);

            DropDownList ddlDest = (DropDownList)tempDataItem.FindControl("ddlDest");
            SpecialtyMedTaskProcessResult specialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult();
            specialtyMedTaskProcessResult.RedirectString = String.Empty;
            specialtyMedTaskProcessResult.MessageText = String.Empty;
            specialtyMedTaskProcessResult.MessageType = (int) MessageType.ERROR;
            specialtyMedTaskProcessResult.MessageVisible = true;

            if (ddlDest.SelectedValue == Patient.PHARM || ddlDest.SelectedValue == Patient.PRINT)
            {
                Allscripts.Impact.Prescription.UpdateRxDetailDates(selectedRxID, DBID);
            }

            switch (ddlDest.SelectedValue)
            {
                case Patient.PHARM:
                    if (pageState.GetStringOrEmpty("LASTPHARMACYID") != null && pageState.GetStringOrEmpty("LASTPHARMACYID") != string.Empty)
                    {
                        specialtyMedTaskProcessResult = SendPrescription(selectedRxID, "SENTTOPHARMACY", selectedRefills, selectedDaysSupply,
                                pageState.GetStringOrEmpty("LASTPHARMACYID"), false, pageState, ipaddress, new ScriptMessage(), new impactPrescription(), new EPSBroker(), DBID);
                    }
                    break;

                case Patient.MOB:
                    specialtyMedTaskProcessResult = ProcessMailOrder(selectedRxID, selectedRefills, selectedDaysSupply, pageState, ipaddress, this);
                    break;

                case Patient.PRINT:
                    specialtyMedTaskProcessResult = ProcessPrint(selectedRxID, pageState);
                    break;

                case Patient.ENTERED_IN_ERROR:
                    specialtyMedTaskProcessResult = ProcessEnteredInError(selectedRxID, pageState);
                    break;

                case Patient.SPECIALTYMED_LIMITED_DISTRIBUTION_PHARMACY:
                    string pharmId = pageState.GetStringOrEmpty(Constants.SessionVariables.LTD_PHARMACY_ID);
                    if (pharmId != string.Empty)
                    {
                        specialtyMedTaskProcessResult = SendPrescription(selectedRxID, "SENTTOPHARMACY", selectedRefills, selectedDaysSupply, pharmId,
                             true, pageState, ipaddress, new ScriptMessage(), new impactPrescription(), new EPSBroker(), DBID);
                    }
                    break;
            } //end switch
            return specialtyMedTaskProcessResult;
        }

        public SpecialtyMedTaskProcessResult SendPrescription(string rxID, string rxStatus, int refills, int daysSupply, string pharmacy, bool isLimitedOrMailOrderPharmacy, IStateContainer pageState, string ipaddress, IScriptMessage iScriptMessage, IPrescription iPrescription, Interfaces.IEPSBroker iEPSBroker, ConnectionStringPointer DBID)
        {
            SpecialtyMedTaskProcessResult specialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult();
            string auditLogPatientID = string.Empty;
            string patientId = pageState.GetStringOrEmpty(Constants.SessionVariables.PatientId);
            string licenseId = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            string userId = pageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
            int siteId = Convert.ToInt32(pageState.GetStringOrEmpty(Constants.SessionVariables.SiteId));
            string ShieldSecurityToken = pageState.GetStringOrEmpty(Constants.SessionVariables.ShieldSecurityToken);
            try
            {


                iPrescription.MarkAsFulfilled(licenseId, userId, rxID, refills, daysSupply, rxStatus, pharmacy, isLimitedOrMailOrderPharmacy, false, null, DBID);

                string smid = iScriptMessage.CreateScriptMessage(rxID, 1, Constants.MessageTypes.NEWRX, licenseId, userId, ShieldSecurityToken, siteId, DBID);
                long serviceTaskID = -1;
                if (!string.IsNullOrEmpty(smid))
                {
                    serviceTaskID = iScriptMessage.SendThisMessage(smid, licenseId, userId, DBID);
                }
                smid = iScriptMessage.CreateDUREVTScriptMessage(rxID, 1, licenseId, userId, string.Empty, DBID);
                if ((pageState.GetStringOrEmpty(Constants.SessionVariables.STANDING) == "1") && (!string.IsNullOrEmpty(smid)))
                {
                    iScriptMessage.SendOutboundInfoScriptMessage(smid, licenseId, userId, DBID);
                }

                var rxResponse =
                iEPSBroker.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_RX_CREATED, licenseId, userId, patientId, ipaddress, rxID, DBID, null);

                if (rxResponse.Success)
                {
                    auditLogPatientID = rxResponse.AuditLogPatientID;
                }

                //This will be used from service manager and added to last audit log when message is sent to hub.
                if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
                {
                    Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, DBID);
                }

                specialtyMedTaskProcessResult.RedirectString = string.Empty;
                specialtyMedTaskProcessResult.MessageText = "Prescription sent to pharmacy";
                specialtyMedTaskProcessResult.MessageVisible = true;
                specialtyMedTaskProcessResult.MessageType = (int)Controls_Message.MessageType.SUCCESS;
                return specialtyMedTaskProcessResult;

            }
            catch (Exception)
            {
                specialtyMedTaskProcessResult.RedirectString = string.Empty;
                specialtyMedTaskProcessResult.MessageText = "Error Occurred";
                specialtyMedTaskProcessResult.MessageVisible = true;
                specialtyMedTaskProcessResult.MessageType = (int)Controls_Message.MessageType.ERROR;
                return specialtyMedTaskProcessResult;
            }
        }

        public SpecialtyMedTaskProcessResult ProcessMailOrder(string selectedRxID, int selectedRefills, int selectedDaysSupply,
            IStateContainer pageState, string ipaddress, ISpecialtyMed iSpecialtyMed)
        {
            SpecialtyMedTaskProcessResult specialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult();
            string MOB_PHARMACY_ID = pageState.GetStringOrEmpty(Constants.SessionVariables.MOB_PHARMACY_ID);
            string MOB_NABP = pageState.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP);
            ConnectionStringPointer DBID = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            {
                if (IsPatientEligibleForMailOrderBenefit(pageState))
                {
                    string mopharmID = MOB_PHARMACY_ID;
                    if (string.IsNullOrEmpty(mopharmID))
                    {
                        DataSet mobDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(MOB_NABP,
                            DBID);

                        if (mobDS.Tables[0].Rows.Count > 0)
                        {
                            mopharmID = mobDS.Tables[0].Rows[0]["PharmacyID"].ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(mopharmID))
                    {
                        specialtyMedTaskProcessResult = iSpecialtyMed.SendPrescription(selectedRxID, "SENTTOPHARMACY", selectedRefills, selectedDaysSupply, mopharmID,
                            true, pageState, ipaddress, new ScriptMessage(), new impactPrescription(), new EPSBroker(), DBID);

                        string auditLogPatientID = string.Empty;

                        long serviceTaskID = -1;

                        ///This will be used from service manager and added last audit log when message is sent to hub.
                        if (serviceTaskID != -1 && !string.IsNullOrEmpty(auditLogPatientID))
                        {
                            Audit.InsertAuditLogPatientServiceTask(serviceTaskID, auditLogPatientID, DBID);
                        }
                    }
                }
                else
                {
                    specialtyMedTaskProcessResult.MessageVisible = true;
                    specialtyMedTaskProcessResult.MessageText = "Patient Mail Order Pharmacy is not available.";
                    specialtyMedTaskProcessResult.MessageType = (int) MessageType.ERROR;
                }
            }

            return specialtyMedTaskProcessResult;
        }

        public bool IsPatientEligibleForMailOrderBenefit(IStateContainer pageState)
        {
            bool patientEligibleForMailOrderBenefit = false;
            string patientHasMOBCoverage = pageState.GetStringOrEmpty(Constants.SessionVariables.PatientHasMOBCoverage);
            string mobNABP = pageState.GetStringOrEmpty(Constants.SessionVariables.MOB_NABP);
            if ((patientHasMOBCoverage == "Y") && (mobNABP.Trim() != "X"))
            {
                patientEligibleForMailOrderBenefit = true;
            }
            return patientEligibleForMailOrderBenefit;
        }

        public SpecialtyMedTaskProcessResult ProcessPrint(string selectedRxID, IStateContainer pageState)
        {
            impactPrescription presc = new impactPrescription();
            string SessionUserID = pageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
            string SessionLicenseID = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            ConnectionStringPointer DBID = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            SpecialtyMedTaskProcessResult specialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult();

            presc.LoadFromExistingMed( selectedRxID, null, SessionUserID, SessionLicenseID, DBID);

            // need to redirect to printing pages
            string postProcess = Constants.PageNames.CSS_DETECT + "?PrintScript=YES&From=" + Constants.PageNames.SPECIALTYMEDTASKS;

            specialtyMedTaskProcessResult.RedirectString = postProcess;
            specialtyMedTaskProcessResult.MessageVisible = true;
            specialtyMedTaskProcessResult.MessageText =
                "Task successfully sent to printer.";
            specialtyMedTaskProcessResult.MessageType = (int)MessageType.SUCCESS;

            return specialtyMedTaskProcessResult;
        }

        public SpecialtyMedTaskProcessResult ProcessEnteredInError(string selectedRxID, IStateContainer pageState)
        {
            string extFacilityCode = pageState.GetStringOrEmpty(Constants.SessionVariables.ExternalFacilityCd);
            string extGroupID = pageState.GetStringOrEmpty(Constants.SessionVariables.ExternalGroupID);
            string SessionUserID = pageState.GetStringOrEmpty(Constants.SessionVariables.UserId);
            string SessionLicenseID = pageState.GetStringOrEmpty(Constants.SessionVariables.LicenseId);
            ConnectionStringPointer DBID = pageState.Cast(Constants.SessionVariables.DbId, ConnectionStringPointer.ERXDB_DEFAULT);
            SpecialtyMedTaskProcessResult specialtyMedTaskProcessResult = new SpecialtyMedTaskProcessResult();
            try
            {
                impactPrescription.EnteredInError(selectedRxID, false, SessionUserID, SessionLicenseID, extFacilityCode,
                    extGroupID, DBID);

                specialtyMedTaskProcessResult.RedirectString = string.Empty;
                specialtyMedTaskProcessResult.MessageVisible = true;
                specialtyMedTaskProcessResult.MessageText =
                    "Task successfully removed/Rx is updated as Entered In Error.";
                specialtyMedTaskProcessResult.MessageType = (int)MessageType.SUCCESS;
            }

            catch
            {
                specialtyMedTaskProcessResult.RedirectString = string.Empty;
                specialtyMedTaskProcessResult.MessageVisible = true;
                specialtyMedTaskProcessResult.MessageText = "Task failure.";
                specialtyMedTaskProcessResult.MessageType = (int)MessageType.ERROR;
            }
            return specialtyMedTaskProcessResult;

        }
    }
    public enum MessageType
    {
        SUCCESS,
        ERROR,
        INFORMATION
    }
}



