using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode.Interfaces;
using Prescription = Allscripts.Impact.Prescription;
using Rx = Allscripts.Impact.Rx;
using Allscripts.Impact;

namespace eRxWeb.AppCode
{
    public class EPCSWorkflowUtils: IEPCSWorkflowUtils
    {
        public static bool IsLicenseEpcsPurchased(ApplicationLicense sessionLicense)
        {
            return (sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled) && (
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcs ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.LegacyDeluxeEpcs ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.LegacyDeluxeEpcsLogRx ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsLogRx ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpa ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpa2017 ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx ||
                    sessionLicense.DeluxePricingStructure == Constants.DeluxePricingStructure.DeluxeEpcsEpaLogRx2017);
        }

        public static bool IsEnterpriseEpcsLicense(ApplicationLicense sessionLicense, bool isEnterpriseEpcsEnabled)
        {
            return ((sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.On ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.AlwaysOn ||
                 sessionLicense.LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Cancelled) && 
                 isEnterpriseEpcsEnabled);
        }
        public bool IsEPCSWorkflowExpected(EPCSParameters epcsParams)
        {
            bool bReturn = false;
            // Check if med is a CS med
            if (IsControlledSubstanceMed(epcsParams.ReconciledControlledSubstanceCode, epcsParams.IsFreeFormMedControlSubstance))
            {
                //
                // "CanTryEPCS" is true so that means the physician is EPCS authorized and the 
                // Enterprise Client associated with this license is EPCS enabled
                //
                if (epcsParams.CanTryEPCS)
                {
                    //
                    // check if Med is Federal controlled substance (schedule 2,3,4,5) OR
                    // Med is a state controlled substance in the state the provider's practice is in AND 
                    // Med is a state controlled substance in the state of the pharmacy where the script is being sent
                    //
                    if (Prescription.IsCSMedEPCSEligible(epcsParams.FedCSCode, epcsParams.StateCSCodeForPharmacy, epcsParams.StateCSCodeForPractice))
                    {
                        epcsParams.ReconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(epcsParams.FedCSCode, epcsParams.StateCSCodeForPharmacy, epcsParams.StateCSCodeForPractice);

                        //
                        // check if the state in which the site's practice is in, is EPCS authorized for the 
                        // CS schedule of the selected med
                        //
                        if (epcsParams.SiteEPCSAuthorizedSchedulesList.Contains(epcsParams.ReconciledControlledSubstanceCode))
                        {
                            //
                            // check if pharmacy is EPCS enabled
                            //
                            if (epcsParams.DsPharmacy.Tables[0].Rows[0]["EpcsEnabled"].ToString() == "1")
                            {
                                //
                                // get EPCS authorized schedules for pharmacy
                                //
                                List<string> authorizedSchedules = new List<string>();

                                DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(epcsParams.DsPharmacy.Tables[0].Rows[0]["PharmacyID"].ToString(), epcsParams.DBID);

                                foreach (DataRow drSchedule in dtSchedules.Rows)
                                {
                                    authorizedSchedules.Add(drSchedule[0].ToString());
                                }

                                //
                                // check if the state in which the pharmacy is in, is EPCS authorized for the 
                                // CS schedule of the selected med
                                //
                                if (authorizedSchedules.Contains(epcsParams.ReconciledControlledSubstanceCode))
                                {
                                    bReturn = true;
                                }
                            }
                        }
                    }
                }

            }
            return bReturn;
        }

        public bool IsControlledSubstanceMed(string reconciledControlledSubstanceCode, bool IsFreeFormMedControlSubstance)
        {
            bool bReturn = false;
            if (
                    (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                    reconciledControlledSubstanceCode.ToUpper() != "U" &&
                    reconciledControlledSubstanceCode != "0")
                    ||
                    (IsFreeFormMedControlSubstance)
                )
            {
                bReturn = true;
            }
            return bReturn;

        }

        public static bool IsCSMedIncuded(List<Rx> medications)
        {
            bool bReturn = false;
            if ( medications.Where(x =>
                (!string.IsNullOrWhiteSpace(Prescription.ReconcileControlledSubstanceCodes(x.ControlledSubstanceCode, x.StateControlledSubstanceCode))
                 && Prescription.ReconcileControlledSubstanceCodes(x.ControlledSubstanceCode, x.StateControlledSubstanceCode).ToUpper() != "U"
                 && Prescription.ReconcileControlledSubstanceCodes(x.ControlledSubstanceCode,x.StateControlledSubstanceCode)!="0"
                 ) || (x.IsFreeFormMedControlSubstance)
                ).Count()>0)
            {
                bReturn = true;
            }
            return bReturn;
        }

        public List<Rx> UpdateAndReturnEpcsMedList(List<Rx> epcsMedList, Rx rx)
        {
            if (rx == null)
            {
                throw new ArgumentException("Null Rx supplied");
            }
            else
            {
                if (epcsMedList != null)
                {
                    epcsMedList.Add(rx);
                }
            }
            return epcsMedList;

        }
        public bool ShouldLaunchEPCSWorkflow(bool canStartEpcs, List<Rx> epcsMedList)
        {
            return canStartEpcs && epcsMedList?.Count > 0;
        }

        public EPCSParameters BuildEpcsWorkflowParameterObject(RxTaskModel currentTask)
        {
            EPCSParameters epcsDataFromSigPage = new EPCSParameters();
            epcsDataFromSigPage.DBID = currentTask.DbId;
            var sm = currentTask.ScriptMessage;
            var rx = currentTask.Rx;
            epcsDataFromSigPage.FedCSCode = rx.ControlledSubstanceCode;
            epcsDataFromSigPage.DsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(sm.DBPharmacyID, currentTask.DbId);
            epcsDataFromSigPage.StateCSCodeForPharmacy = Prescription.GetStateControlledSubstanceCode(rx.DDI, null, epcsDataFromSigPage.DsPharmacy.Tables[0].Rows[0]["State"].ToString(), currentTask.DbId);
            epcsDataFromSigPage.ReconciledControlledSubstanceCode = Prescription.ReconcileControlledSubstanceCodes(epcsDataFromSigPage.FedCSCode, epcsDataFromSigPage.StateCSCodeForPharmacy);
            epcsDataFromSigPage.StateCSCodeForPractice = Prescription.GetStateControlledSubstanceCode(rx.DDI, currentTask.PracticeState, currentTask.DbId);
            epcsDataFromSigPage.IsFreeFormMedControlSubstance = currentTask.Rx.IsFreeFormMedControlSubstance;
            epcsDataFromSigPage.CanTryEPCS = currentTask.CanTryEPCS;
            epcsDataFromSigPage.SiteEPCSAuthorizedSchedulesList = currentTask.SiteEPCSAuthorizedSchedules;
            return epcsDataFromSigPage;
        }
    }
}