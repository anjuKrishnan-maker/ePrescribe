using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Web;
using Allscripts.ePrescribe.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Medispan.Clinical.Model;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Objects.PPTPlus;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode.PdmpBPL;
using eRxWeb.AppCode.PptPlusBPL;
using eRxWeb.State;
using Allergy = Allscripts.ePrescribe.Medispan.Clinical.Model.Allergy;
using Constants = Allscripts.ePrescribe.Common.Constants;
using Patient = Allscripts.Impact.Patient;

namespace eRxWeb.AppCode.StateUtils
{
    public class PatientInfo
    {

        public static string GetSessionPatientID(IStateContainer session)
        {
            if (session["PATIENTID"] == null)
                return null;

            return session["PATIENTID"].ToString();
        }

        public static void SetPatientInfo(string patientID, IStateContainer session, ConnectionStringPointer dbid)
        {
            ClearPatientInfo(session);
            session["IsRemovedCSMedsFromScriptPad"] = "N";
            session["PATIENTID"] = patientID;
            Patient pat = new Patient(patientID, true, false, false, session["LICENSEID"].ToString(), session["USERID"].ToString(), dbid);
            DataSet patds = pat.DS;
            string patientSelectedMOB = null;

            if (patds != null && patds.Tables["Patient"] != null && patds.Tables["Patient"].Rows.Count > 0)
            {
                string middleName = Convert.ToString(patds.Tables["Patient"].Rows[0]["MiddleName"]);

                session["PATIENTNAME"] = StringUtil.formatDisplayName(
                            patds.Tables["Patient"].Rows[0]["LastName"].ToString(),
                            patds.Tables["Patient"].Rows[0]["FirstName"].ToString(),
                            middleName
                           );
                session["PATIENTFIRSTNAME"] = patds.Tables["Patient"].Rows[0]["FirstName"].ToString();
                session["PATIENTLASTNAME"] = patds.Tables["Patient"].Rows[0]["LastName"].ToString();
                session["PATIENTMIDDLENAME"] = middleName;
                session["SEX"] = patds.Tables["Patient"].Rows[0]["SEX"].ToString();
                session["PATIENTDOB"] = patds.Tables["Patient"].Rows[0]["DOB"].ToString();
                session["PATIENTADDRESS1"] = patds.Tables["Patient"].Rows[0]["Address1"].ToString();
                session["PATIENTADDRESS2"] = patds.Tables["Patient"].Rows[0]["Address2"].ToString();
                session["PATIENTCITY"] = patds.Tables["Patient"].Rows[0]["City"].ToString();
                session["PATIENTSTATE"] = patds.Tables["Patient"].Rows[0]["State"].ToString();
                session["PATIENTZIP"] = patds.Tables["Patient"].Rows[0]["Zip"].ToString();
                session["PATIENTMRN"] = patds.Tables["Patient"].Rows[0]["ChartID"].ToString();
                session["PATIENTNKA"] = patds.Tables["Patient"].Rows[0]["NoKnownAllergies"].ToString();
                session["IsVIPPatient"] = patds.Tables["Patient"].Rows[0]["IsVIPPatient"].ToString();
                session[Constants.SessionVariables.IsRestrictedPatient] = patds.Tables["Patient"].Rows[0]["IsRestrictedPatient"].ToString();
                session[Constants.SessionVariables.IsPatientHealthPlanDisclosed] = patds.Tables["Patient"].Rows[0]["IsPatientHealthPlanDisclosed"].ToString();
              
                if (!session.GetBooleanOrFalse(Constants.SessionVariables.IsRestrictedPatient)) session.Remove(Constants.SessionVariables.RestrictedUserOverridden);

                var weight = new Weight(patds.Tables["Patient"].Rows[0]["WeightKg"] == DBNull.Value ? string.Empty : patds.Tables["Patient"].Rows[0]["WeightKg"].ToString());
                session[Constants.SessionVariables.PatientWeight] = weight;

                session[Constants.SessionVariables.PatientHeight] = new Height(Convert.ToString(patds.Tables["Patient"].Rows[0]["HeightCm"]));

                int noActiveMedication = 0;
                if (!Int32.TryParse(patds.Tables["Patient"].Rows[0]["NoActiveMedication"].ToString(), out noActiveMedication))
                {
                    noActiveMedication = 0;
                }

                session["PATIENTNoActiveMed"] = noActiveMedication == 1 ? true : false;

                session["PATIENT_YEARS_OLD"] = pat.YearsOld;
                session["PATIENT_AGE_CATEGORY"] = Convert.ToInt32(pat.AgeCategory);

                session["PATIENTSTATUS"] = patds.Tables["Patient"].Rows[0]["statusID"].ToString();
                if (patds.Tables["Patient"].Rows[0]["email"] != DBNull.Value)
                {
                    session["PATIENTEMAIL"] = patds.Tables["Patient"].Rows[0]["email"].ToString();
                }

                if (patds.Tables["Patient"].Rows[0]["MobilePhone"] != DBNull.Value)
                {
                    session["PATIENTMOBILEPHONE"] = patds.Tables["Patient"].Rows[0]["MobilePhone"].ToString();
                }

                string LastPhamarcyId = patds.Tables["Patient"].Rows[0]["LASTPHARMACYID"].ToString();
                if (LastPhamarcyId != null && LastPhamarcyId != "" && LastPhamarcyId != "00000000-0000-0000-0000-000000000000")
                {
                    session["LASTPHARMACYID"] = patds.Tables["Patient"].Rows[0]["LASTPHARMACYID"].ToString();
                    session["LASTPHARMACYNAME"] = patds.Tables["Patient"].Rows[0]["LASTPHARMACYNAME"].ToString();

                    if (patds.Tables["Patient"].Rows[0]["PercScriptSuccess"] != DBNull.Value)
                    {
                        session["PharmacySuccessRate"] = patds.Tables["Patient"].Rows[0]["PercScriptSuccess"].ToString();
                    }

                    // EPCS enabled?
                    if (patds.Tables["Patient"].Rows[0]["IsPharmacyEPCSEnabled"] != DBNull.Value)
                    {
                        if (Convert.ToBoolean(patds.Tables["Patient"].Rows[0]["IsPharmacyEPCSEnabled"].ToString()))
                        {
                            session["ISPHARMACYEPCSENABLED"] = true;

                            // get EPCS authorized schedules for pharmacy
                            List<string> authorizedSchedules = new List<string>();

                            DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(session["LASTPHARMACYID"].ToString(), dbid);

                            foreach (DataRow drSchedule in dtSchedules.Rows)
                            {
                                authorizedSchedules.Add(drSchedule[0].ToString());
                            }

                            session["PharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;
                        }
                        else
                        {
                            session["ISPHARMACYEPCSENABLED"] = false;
                        }
                    }
                }
                else
                {
                    session.Remove("LASTPHARMACYID");
                    session.Remove("LASTPHARMACYNAME");
                    session.Remove("PharmacySuccessRate");
                    session.Remove("ISPHARMACYEPCSENABLED");
                }

                //check to see if the patient has manually selected an MOB pharmacy outside of the elig response workflow
                patientSelectedMOB = patds.Tables["Patient"].Rows[0]["MOB_NABP"].ToString();
                if (!string.IsNullOrEmpty(patientSelectedMOB))
                {
                    session["MOB_NABP"] = patientSelectedMOB;
                    session["MOB_PHARMACY_ID"] = patds.Tables["Patient"].Rows[0]["MOB_PharmacyID"].ToString();
                    session["PatientHasMOBCoverage"] = "Y";
                    session["MOB_Name"] = patds.Tables["Patient"].Rows[0]["MOB_PharmacyName"].ToString().Trim();
                    session["MOB_IsElectronicEnabled"] = bool.Parse(patds.Tables["Patient"].Rows[0]["MOB_IsElectronicEnabled"].ToString());

                    if (patds.Tables["Patient"].Rows[0]["MOB_PercScriptSuccess"] != DBNull.Value && patds.Tables["Patient"].Rows[0]["MOB_PercScriptSuccess"].ToString().Trim() != "")
                    {
                        session["MOB_PercScriptSuccess"] = patds.Tables["Patient"].Rows[0]["MOB_PercScriptSuccess"].ToString().Trim();
                    }

                    if (bool.Parse(patds.Tables["Patient"].Rows[0]["MOB_IsEPCSEnabled"].ToString()))
                    {
                        session["MOB_ISEPCSENABLED"] = true;
                        session["MOB_State"] = string.Empty;
                        if (patds.Tables["Patient"].Rows[0]["MOB_State"] != DBNull.Value)
                        {
                            session["MOB_State"] = patds.Tables["Patient"].Rows[0]["MOB_State"].ToString();
                        }

                        // get EPCS authorized schedules for pharmacy
                        List<string> authorizedSchedules = new List<string>();

                        DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(session["MOB_PHARMACY_ID"].ToString(), dbid);

                        foreach (DataRow drSchedule in dtSchedules.Rows)
                        {
                            authorizedSchedules.Add(drSchedule[0].ToString());
                        }

                        session["MOPharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;
                    }
                    else
                    {
                        session["MOB_ISEPCSENABLED"] = false;
                    }
                }

                session["PatientDemographicAlert"] = patds.Tables["Patient"].Rows[0]["DemographicWarning"].ToString();
                session["PatientSwitchMessageID"] = patds.Tables["Patient"].Rows[0]["SwitchMessageID"].ToString() != "" ? patds.Tables["Patient"].Rows[0]["SwitchMessageID"].ToString() : "0";

                session["PatientActive"] = true;

                int statusID = 1;
                if (patds.Tables["Patient"].Rows[0]["StatusID"] != DBNull.Value && !string.IsNullOrEmpty(patds.Tables["Patient"].Rows[0]["StatusID"].ToString()))
                {
                    statusID = int.Parse(patds.Tables["Patient"].Rows[0]["StatusID"].ToString());
                }

                if (statusID == 0)
                {
                    session["PatientActive"] = false;
                }

            }
            else
            {
                session.Remove("PATIENTID");
                session.Remove("SEX");
                session.Remove("PATIENTDOB");
                session.Remove("PATIENT_YEARS_OLD");
                session.Remove("PATIENT_AGE_CATEGORY");
                session.Remove("PATIENTZIP");
                session.Remove("PATIENTMRN");
                session.Remove("PATIENTNKA");
                session.Remove("LASTPHARMACYID");
                session.Remove("LASTPHARMACYNAME");
                session.Remove("PharmacySuccessRate");
                session.Remove("PatientDemographicAlert");
                session.Remove("PatientSwitchMessageID");
                session.Remove("PatientActive");
                session.Remove("ClinicalViewerPostData");
                session.Remove("PATIENTNoActiveMed");
            }

            if (patds.Tables["Plan"] != null && patds.Tables["Plan"].Rows.Count > 0)
            {
                session["INSURANCE"] = patds.Tables["Plan"].Rows[0]["Name"].ToString();
                session["CoverageID"] = patds.Tables["Plan"].Rows[0]["CoverageID"].ToString();
                session["CopayID"] = patds.Tables["Plan"].Rows[0]["CopayID"].ToString();

                var selectedCovId = patds.Tables["Plan"].Rows[0]["PatientCoverageID"].ToString();
                session["SelectedCoverageID"] = selectedCovId;
            }
            else
            {
                session.Remove("INSURANCE");
                session.Remove("CoverageID");
                session.Remove("CopayID");

                if (string.IsNullOrEmpty(patientSelectedMOB))
                {
                    session.Remove("MOB_NABP");
                    session.Remove("PatientHasMOBCoverage");
                    session.Remove("MOB_Name");
                    session.Remove("MOB_PercScriptSuccess");
                    session.Remove("MOB_PHARMACY_ID");
                    session.Remove("MOB_ISEPCSENABLED");
                }

                session.Remove("SelectedCoverageID");
            }

            var patData = patds.Copy();
            var context = HttpContext.Current;
            ThreadPool.QueueUserWorkItem(state =>
            {
                HttpContext.Current = context;
                InitCommonCompPatInfo(session, patData, dbid);
            });

            if (patds.Tables["Formulary"] != null && patds.Tables["Formulary"].Rows.Count > 0)
            {
                session["PlanID"] = patds.Tables["Formulary"].Rows[0]["PlanID"].ToString();
                session["AltPlanID"] = patds.Tables["Formulary"].Rows[0]["AltPlanID"].ToString();
                session["FormularyID"] = patds.Tables["Formulary"].Rows[0]["FormularyID"].ToString();
                session["FormularyActive"] = patds.Tables["Formulary"].Rows[0]["FormularyActive"].ToString();
                session["OTCCoverage"] = patds.Tables["Formulary"].Rows[0]["OTCCoverage"].ToString();
                session["GenericDrugPolicy"] = patds.Tables["Formulary"].Rows[0]["GenericDrugPolicy"].ToString();
                session["UnListedDrugPolicy"] = patds.Tables["Formulary"].Rows[0]["UnlistedDrugPolicy"].ToString();
            }
            else
            {
                session.Remove("PlanID");
                session.Remove("AltPlanID");
                session.Remove("FormularyID");
                session.Remove("FormularyActive");
                session.Remove("OTCCoverage");
                session.Remove("GenericDrugPolicy");
                session.Remove("UnListedDrugPolicy");
            }

            if (patds != null && patds.Tables["PatientAllergy"] != null && patds.Tables["PatientAllergy"].Rows.Count > 0)
            {
                DataRow[] PatAllergy = patds.Tables["PatientAllergy"].Select("Active='Y'");
                string allergy = "";
                var patientAllergies = new List<Allergy>();
                foreach (DataRow dr in PatAllergy)
                {
                    allergy = allergy + dr["AllergyName"].ToString() + ",";

                    int ID = (Convert.ToString(dr["AllergyID"]) == string.Empty) ? 0 : Convert.ToInt32(dr["AllergyID"]);
                    string patientAllergyID = Convert.ToString(dr["PatientAllergyID"]);

                    //2. Allergy Type
                    AllergyType allergyType = DURMedispanUtils.RetrieveAllergyType(Convert.ToString(dr["AllergyType"]));

                    //3. Allergy Reaction Type
                    AllergyReactionType allergyReactionType = DURMedispanUtils.RetrieveAllergyReactionTypeFromAllergyDataset(Convert.ToString(dr["PatientAllergyID"]), dbid);

                    //4. Symptoms List
                    List<string> symptoms = DURMedispanUtils.RetrieveSymptomsFromAllergyDataSet(patds, patientAllergyID);

                    //5. Add 1,2,3,4 to the list
                    patientAllergies.Add(new Allergy(ID, allergyType, allergyReactionType, symptoms, Convert.ToString(dr["AllergyName"])));
                }

                session[Allscripts.ePrescribe.Common.Constants.SessionVariables.DurPatientAllergies] = patientAllergies;

                if (allergy.EndsWith(","))
                {
                    allergy = allergy.Substring(0, allergy.Length - 1);
                }

                session["ALLERGY"] = allergy;
            }
            else
            {
                session.Remove("ALLERGY");
                session.Remove(Allscripts.ePrescribe.Common.Constants.SessionVariables.DurPatientAllergies);
            }

            //Retrieve the patient's distinct active medications 
            DataSet activeMeds = Patient.GetPatientActiveMedications(patientID, session["LICENSEID"].ToString(), session["USERID"].ToString(), dbid);
            if (activeMeds.Tables["Medications"].Rows.Count > 0)
            {
                StringBuilder activeMedications = new StringBuilder();
                List<string> activeMedDDIList = new List<string>();

                foreach (DataRow dr in activeMeds.Tables["Medications"].Rows)
                {
                    if (activeMedications.Length > 0)
                    {
                        activeMedications.Append(", ");
                    }

                    activeMedications.Append(dr["MedicationName"].ToString().Trim());
                    activeMedDDIList.Add(dr["DDI"].ToString());
                }

                session["ACTIVEMEDICATIONS"] = activeMedications.ToString();
                session["ACTIVEMEDDDILIST"] = activeMedDDIList;
                session[Allscripts.ePrescribe.Common.Constants.SessionVariables.ActiveMedicationsTable] = activeMeds.Tables["Medications"];
            }
            else
            {
                session.Remove("ACTIVEMEDICATIONS");
                session.Remove("ACTIVEMEDDDILIST");
                session.Remove(Constants.SessionVariables.ActiveMedicationsTable);
            }
            // set active diagnosis
            string activeDiagnosis = new PatientDiagnosisProvider().GetActiveDiagnosisDisplayString(patientID, dbid).Trim();
            if (!string.IsNullOrEmpty(activeDiagnosis))
            {
                session["ACTIVEDX"] = activeDiagnosis;
            }


            patds.Clear();
            patds.AcceptChanges();
            patds.Dispose();
            patds = null;
        }

        private static void InitCommonCompPatInfo(IStateContainer session, DataSet patds, ConnectionStringPointer dbid)
        {
            try
            {
                if ((PPTPlus.IsPPTPlusEnterpriseOn(session) 
                    || PDMP.IsShowPDMP(session)) && patds!=null && patds.Tables.Contains("Patient") && patds.Tables["Patient"].Rows.Count>0)
                {
                    DataRow patientRow = patds.Tables["Patient"].Rows[0];
                    var patInfo = Allscripts.ePrescribe.Objects.CommonComponent.PatientInfo.Create(patientRow, session.GetBooleanOrFalse("IsRestrictedUser"), 
                                                session.GetBoolean(Constants.SessionVariables.IsPatientHealthPlanDisclosed, true)); //Set default to true to keep consistent with SP: [ppt_GetPatientRtbiData]
                    var pharmInfo = new PptPlusData().GetPharmacyInfo(Convert.ToString(patientRow["LASTPHARMACYID"]).ToGuidOr0x0(), dbid);
                    session[Constants.SessionVariables.CommonCompPatientInfo] = patInfo;
                    session[Constants.SessionVariables.PptPharmacyInfo] = pharmInfo;

                    var selectedCovId = session.GetStringOrEmpty("SelectedCoverageID");
                    if (!string.IsNullOrWhiteSpace(selectedCovId))
                    {
                        var rtbi = new PptPlusData().GetPatientRtbiInto(patInfo.Id, selectedCovId, dbid);
                        session[Constants.SessionVariables.PptRtbiInfo] = rtbi;
                    }
                }
            }
            catch(Exception ex)
            {
                Audit.AddException(session.GetStringOrEmpty("USERID"),
                    session.GetStringOrEmpty("LICENSEID"),
                    string.Format("Exception: {0}", ex),
                    string.Empty, string.Empty, string.Empty, dbid
                    );
            }
        }

        public static void SetPharmInfo(IStateContainer session, string rxID, ConnectionStringPointer DBID)
        {
            var dsPharmacy = Allscripts.Impact.Pharmacy.LoadPharmacy(rxID, DBID);
            if (dsPharmacy != null && dsPharmacy.Tables != null && dsPharmacy.Tables[0].Rows.Count > 0)
            {
                session["ISPHARMACYEPCSENABLED"] = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["EpcsEnabled"]);
                bool isMob = Convert.ToBoolean(dsPharmacy.Tables[0].Rows[0]["MOBFlag"]);

                if (isMob)
                {
                    session["MOB_NABP"] = dsPharmacy.Tables[0].Rows[0]["NABP"];
                    session["MOB_PHARMACY_ID"] = rxID;
                    session["PatientHasMOBCoverage"] = "Y";
                    session["MOB_Name"] = dsPharmacy.Tables[0].Rows[0]["Name"].ToString().Trim();
                    session["MOB_IsElectronicEnabled"] = (dsPharmacy.Tables[0].Rows[0]["ElectronicAllowed"].ToString().Equals("Y") ? true : false);

                    if (dsPharmacy.Tables[0].Rows[0]["PercScriptSuccess"] != DBNull.Value && dsPharmacy.Tables[0].Rows[0]["PercScriptSuccess"].ToString().Trim() != "")
                    {
                        session["MOB_PercScriptSuccess"] = dsPharmacy.Tables[0].Rows[0]["PercScriptSuccess"].ToString().Trim();
                    }

                    if (bool.Parse(session["ISPHARMACYEPCSENABLED"].ToString()))
                    {
                        session["MOB_ISEPCSENABLED"] = true;
                        session["MOB_State"] = string.Empty;
                        if (dsPharmacy.Tables[0].Rows[0]["State"] != DBNull.Value)
                        {
                            session["MOB_State"] = dsPharmacy.Tables[0].Rows[0]["State"].ToString();
                        }
                    }
                }

                // get EPCS authorized schedules for pharmacy
                List<string> authorizedSchedules = new List<string>();

                DataTable dtSchedules = Allscripts.Impact.Pharmacy.GetEPCSAuthorizedSchedulesForPharmacy(rxID, DBID);

                foreach (DataRow drSchedule in dtSchedules.Rows)
                {
                    authorizedSchedules.Add(drSchedule[0].ToString());
                }

                if (isMob)
                {
                    session["MOPharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;
                }
                else
                {
                    session["PharmacyEPCSAuthorizedSchdules"] = authorizedSchedules;
                }
            }
        }

        public static void ClearPatientInfo(IStateContainer session)
        {
            session.Remove("ACTIVEDX");
            session.Remove("ACTIVEMEDICATIONS");
            session.Remove("ACTIVEMEDDDILIST");
            session.Remove("ALLERGY");
            session.Remove("AltPlanID");
            session.Remove("COPAYID");
            session.Remove("COVERAGEID");
            session.Remove("DIAGNOSIS");
            session.Remove("FormularyActive");
            session.Remove("FormularyID");
            session.Remove("GenericDrugPolicy");
            // PageState.Remove("ICD9CODE"); don't think this is needed, as it wipes out ICD9 when overlay prompts user - JMT 02/18/09
            session.Remove("INSURANCE");
            session.Remove("LASTPHARMACYID");
            session.Remove("LASTPHARMACYNAME");
            session.Remove("PharmacySuccessRate");
            session.Remove("MOB_NABP");
            session.Remove("MOB_Name");
            session.Remove("MOB_PercScriptSuccess");
            session.Remove("MOB_PHARMACY_ID");
            session.Remove("MOB_ISEPCSENABLED");
            session.Remove("PatientHasMOBCoverage");
            session.Remove("ORIGINALDDI");
            session.Remove("OTCCoverage");
            session.Remove("PatientDemographicAlert");
            session.Remove("PatientSwitchMessageID");
            session.Remove("PATIENTDOB");
            session.Remove("PATIENT_YEARS_OLD");
            session.Remove("PATIENT_AGE_CATEGORY");
            session.Remove("PATIENTID");
            session.Remove("PATIENTMRN");
            session.Remove("PATIENTNAME");
            session.Remove("PATIENTFIRSTNAME");
            session.Remove("PATIENTLASTNAME");
            session.Remove("PATIENTMIDDLENAME");
            session.Remove("PATIENTNKA");
            session.Remove("PATIENTSTATUS");
            session.Remove("PATIENTADDRESS1");
            session.Remove("PATIENTADDRESS2");
            session.Remove("PATIENTCITY");
            session.Remove("PATIENTSTATE");
            session.Remove("PATIENTZIP");
            session.Remove("PHARMACYID");
            session.Remove("ISPHARMACYEPCSENABLED");
            session.Remove("PATIENTMOBILEPHONE");
            session.Remove("PlanID");
            session.Remove("SelectedCoverageID");
            session.Remove("SEX");
            session.Remove("UnListedDrugPolicy");
            session.Remove("PartnerName");
            session.Remove("PatientActive");
            session.Remove("ClinicalViewerPostData");
            session.Remove("PATIENTNoActiveMed");
            session.Remove("IsVIPPatient");
            session.Remove(Constants.SessionVariables.DurPatientAllergies);
            session["CURRENT_SCRIPT_PAD_MEDS"] = null;
            session.Remove(Constants.SessionVariables.DURCheckResponse);
            session.Remove(Constants.SessionVariables.ActiveMedicationsTable);
            session.Remove(Constants.SessionVariables.CurrentDurWarnings);
            session.Remove("GPI");
            session.Remove(Constants.SessionVariables.SpecialtyMed);
            session.Remove(Constants.SessionVariables.PptPlusResponses);
            session.Remove(Constants.SessionVariables.PptRtbiInfo);
            session.Remove(Constants.SessionVariables.IsPatientHealthPlanDisclosed);
            session.Remove(Constants.SessionVariables.PatientHeight);
            session.Remove(Constants.SessionVariables.InfoSourcePayerId);
        }
    }
}