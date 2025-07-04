/******************************************************************************
**Change History
*******************************************************************************
**Date:      Author:                    Description:
**-----------------------------------------------------------------------------
* 02/19/2010   Anand Kumar Krishnan     Patient NKA is saved from CCR in ProcessCCRAllergies method
* 02/23/2010   Anand Kumar Krishnan     PatientAllergyID matching is done along with ExternalAllergyID 
*                                       if we found multiple PatientAllergyID using Allergyid 
*                                       in ProcessCCRAllergies method 
* 03/09/2010   Anand Kumar Krishnan		Defect#3308 On processing CCR medication 
*                                       Quantity made to 0 when the value is negative
*******************************************************************************/
#define TRACE
using System;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using Allscripts.ePrescribe.CCR;
using Allscripts.Impact;
using System.Collections.Generic;
using Allscripts.Impact.Utilities;
using System.Text;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;

namespace eRxWeb
{
/// <summary>
/// CCRDocumentProcessor contains the logic to process a CCRDocument and populate the patient health data contained in the CCR.
/// </summary>
public class CCRDocumentProcessor
{
	private CCRDocument m_ccr;
	private string m_sPatientID;
	private string m_sLicenseID;

	private bool m_bIncludeMeds;
	private bool m_bIncludeProblems;
	private bool m_bIncludeAllergies;

	public CCRDocumentProcessor(CCRDocument ccr, string patientID, string licenseID)
	{
		m_ccr = ccr;
		m_sPatientID = patientID;
		m_sLicenseID = licenseID;

		//By default, process all the med, dx, and allergies
		m_bIncludeMeds = true;
		m_bIncludeProblems = true;
		m_bIncludeAllergies = true;
	}

	public bool IncludeMedications
	{
		get { return m_bIncludeMeds; }
		set { m_bIncludeMeds = value; }
	}

	public bool IncludeProblems
	{
		get { return m_bIncludeProblems; }
		set { m_bIncludeProblems = value; }
	}

	public bool IncludeAllergies
	{
		get { return m_bIncludeAllergies; }
		set { m_bIncludeAllergies = value; }
	}

	public string PatientID
	{
		get { return m_sPatientID; }
	}

	public string LicenseID
	{
		get { return m_sLicenseID; }
	}

	public void ProcessCCR(string sessionID, ConnectionStringPointer dbID)
	{
		if (this.IncludeAllergies)
			ProcessCCRAllergies(sessionID, dbID);

		if (this.IncludeProblems)
			ProcessCCRProblems(sessionID, dbID);

		if (this.IncludeMedications)
			ProcessCCRMedications(sessionID, dbID); 
	}

	private void ProcessCCRMedications(string sessionID, ConnectionStringPointer dbID)
	{
        System.Diagnostics.Trace.Write("Start ProcessCCRMedications()", sessionID);
        
		CCRMedication[] medications = null;

		if (this.IncludeMedications)
			medications = m_ccr.Medications;

		if (medications != null)
		{
			string rxID;
			int refills = 0;
			int daysSupply = 0;
			string route = string.Empty;
			decimal quantity = decimal.Zero;
			string medCodingSystem = string.Empty;
			string medCode = string.Empty; 
			string ddi = string.Empty;
			string dosageForm = string.Empty;
			string issueDate = string.Empty;
			DateTime rxDate, endDate, expirationDate;
            Constants.PrescriptionStatus rxStatus;
			string transmissionMethod = string.Empty;

			string pharmacyID = string.Empty;
			CCRActor pharmacyActor = null;

			DataSet pharmacyDS = null;
			DataSet medDS = null;
			DataTable medDT = null;

			foreach (CCRMedication med in medications)
			{
                System.Diagnostics.Trace.Write("Start individual med processing: " + med.ProductName, sessionID);

				//Load by product code or description code
                if (!string.IsNullOrEmpty(med.ProductCode) && (!string.IsNullOrEmpty(med.MedispanProductCode) || !string.IsNullOrEmpty(med.NDCProductCode)))
				{
					//It's possible to get multiple codes both DDI and NDC. The DDI takes precedence 
					//over the NDC code. 

					if (!string.IsNullOrEmpty(med.MedispanProductCode))
					{
						medCode = med.MedispanProductCode;
						medCodingSystem = CCRConstants.MEDISPAN_CODING_SYSTEM;
					}
					else if (!string.IsNullOrEmpty(med.NDCProductCode))
					{
						medCode = med.NDCProductCode;
						medCodingSystem = CCRConstants.NDC_CODING_SYSTEM;
					}
				}
				else if (!string.IsNullOrEmpty(med.DescriptionCode))
				{
					medCodingSystem = med.DescriptionCodingSystem;
					medCode = med.DescriptionCode;
				}

				if (medCodingSystem == CCRConstants.NDC_CODING_SYSTEM)
				{
					medDS = Medication.LoadByNDC(CCRProcessorUtils.PadLeadingZero(medCode, CCRProcessorUtils.NDC_CODE_LENGTH), dbID);
					medDT = medDS.Tables[0];
				}
				else if (medCodingSystem == CCRConstants.MEDISPAN_CODING_SYSTEM)
				{
                    //now just taking the ddi the partner passes to us to save a database call.
                    ddi = med.MedispanProductCode;
				}

				//If a medication is found, then set the ddi of the med. 
                if (ddi == string.Empty && medDT != null && medDT.Rows.Count > 0)
					ddi = medDT.Rows[0]["DDI"].ToString().Trim();

				if (!int.TryParse(med.DaysSupply, out daysSupply))
					daysSupply = 0;

				//Try parse the refills, if failed set to default value 0 refill
				if (!int.TryParse(med.Refills, out refills))
				{
					refills = 0;
				}

				//Try parse the quantity, if failed set to default value 0 quanity
				if ((!decimal.TryParse(med.Quantity, out quantity))  || (quantity < 0))
				{
					quantity = decimal.Zero;
				}

				//Intepret rxStatus
				rxStatus = CCRProcessorUtils.InterpretRxStatus(med.Status);

				route = med.Route;
				//make sure the Rout of Admin is in ePrescribe
				if (!string.IsNullOrEmpty(route) && Allscripts.ePrescribe.Data.Prescription.LookupRouteofAdmin(route, dbID).Tables[0].Rows.Count == 0)
					route = string.Empty;

				dosageForm = med.ProductForm;
                if (!string.IsNullOrEmpty(dosageForm) && Allscripts.ePrescribe.Data.Prescription.LookupDosageForm(dosageForm, dbID).Tables[0].Rows.Count == 0)
					dosageForm = string.Empty;

				//issue date: SSO sample comes with Entered date instead of Prescription Date type. 
				issueDate = med.IssueDate;
				if (string.IsNullOrEmpty(issueDate))
				{
					issueDate = med.GetExactDateTimeByType("Entered");
				}

				//Set rxDate, startDate
				if (DateTime.TryParse(med.IssueDate, out rxDate))
					rxDate = rxDate.ToUniversalTime();
				else
					rxDate = CCRProcessorUtils.EPRESCRIBE_DEFAULT_DT;

				//Set endDate
				if (DateTime.TryParse(med.EndDate, out endDate))
					endDate = endDate.ToUniversalTime();
				else
					endDate = CCRProcessorUtils.EPRESCRIBE_DEFAULT_DT;

				//Set expirationDate
				if (DateTime.TryParse(med.ExpirationDate, out expirationDate))
					expirationDate = expirationDate.ToUniversalTime();
				else
					expirationDate = CCRProcessorUtils.EPRESCRIBE_DEFAULT_DT;


				pharmacyID = Guid.Empty.ToString(); //default no pharmacy
				//Look up the pharmacy information for the medication. Pharmacy is not supported in medication segment. 
				//This is Veradigm proprietary logic to get around the pharmacy issue in CCR. 
				pharmacyActor = this.m_ccr.GetActorByObjectID(string.Format("{0}", med.CCRDataObjectID));
				if (pharmacyActor != null && pharmacyActor.IDType.ToUpper().Contains(CCRProcessorUtils.PHARMACY_ID_TYPE_NABP))
				{
					pharmacyDS = Allscripts.Impact.Pharmacy.LoadPharmacyByNABP(pharmacyActor.ID, dbID);
					if (pharmacyDS.Tables[0].Rows.Count > 0)
					{
						DataRow[] matchedPharmacy = pharmacyDS.Tables[0].Select(string.Format("substring(Zip, 1, 5)='{0}' and State='{1}'", pharmacyActor.AddressZip.Substring(0, 5), pharmacyActor.AddressState));
						if (matchedPharmacy.Length == 1)
						{
							//if only one pharmacy is found, then that's the one we want. any other cases, bad data exists. 
							pharmacyID = matchedPharmacy[0]["PharmacyID"].ToString();
						}
					}
				}

				//Set the transmission method
				transmissionMethod = CCRProcessorUtils.InterpretTransmissionMethod(med.TransmissionMethod);

				//Try to figure out if there is any existing rx that matches for the patient 
				//based upon the ePrescribeRxID, or externalRxID if present. 
				if (!string.IsNullOrEmpty(med.EPrescribeDocumentID) || !string.IsNullOrEmpty(med.CCRDataObjectID))
				{
					if (!GetRxID(med.EPrescribeDocumentID, med.CCRDataObjectID, ddi, rxDate, med.ProductName, route, dosageForm, med.Strength, refills, med.SigText, quantity, med.FulfillmentInstructionText, dbID, out rxID))
						rxID = Guid.NewGuid().ToString();
				}
				else
				{
					//Fuzzy match will be performed using DDI, RxDate if DDI if present
					//Last resort will be matched by medicationName, route, dosage, strength and rxDate. 
					//If all failed, then insert medication as new for patient. 
					if (!GetRxID(string.Empty, string.Empty, ddi, rxDate, med.ProductName, route, dosageForm, med.Strength, refills, med.SigText, quantity, med.FulfillmentInstructionText, dbID, out rxID))
						rxID = Guid.NewGuid().ToString();
				}

				Allscripts.ePrescribe.Data.Prescription.FilePatientRx(
						this.LicenseID,
						rxID,
						rxDate,
						this.PatientID,
						Guid.Empty.ToString(),
						pharmacyID,
						1,
						string.Empty,
						string.Empty,
						string.Empty,
                        (transmissionMethod == Constants.PrescriptionTransmissionMethod.RECORDED) ? Constants.PrescriptionType.REPORTED : Constants.PrescriptionType.NEW,
						"N",
						string.Empty,
						Guid.Empty.ToString(),
                        Constants.EXTERNAL_RX,
						string.Empty,
						string.Empty,
						1,
						"0",
						ddi,
						med.ProductName,
						route,
						dosageForm,
						med.Strength,
						med.StrengthUOM,
						Guid.Empty.ToString(),
						med.SigText,
						refills,
						quantity,
						daysSupply,
						string.Empty,
						decimal.One,
						1,
						string.Empty,
						string.Empty,
						med.HasDAWInstruction ? "Y" : "N",
						rxDate,
						endDate,
						endDate,
						(int)rxStatus,
						transmissionMethod,
						string.Empty,
						med.FulfillmentInstructionText,
						"Q",
						med.PatientInstructionCode,
						med.PatientInstructionText,
						med.ICD9Code,
                        med.ExternalFacilityCode,
                        med.ExternalGroupID,
						dbID);

				//Add logic to populate the external identifier for the Rx
				if (!string.IsNullOrEmpty(med.CCRDataObjectID))
					Allscripts.ePrescribe.Data.Prescription.FileRxExternalID(med.CCRDataObjectID, LicenseID, rxID, PatientID, dbID);

				//Clean up the values
				rxID = string.Empty;
				refills = 0;
				route = string.Empty;
				quantity = decimal.Zero;
				ddi = string.Empty;
				dosageForm = string.Empty;
                rxStatus = Constants.PrescriptionStatus.NEW;
				medDS = null;
				medDT = null;
				pharmacyID = string.Empty;
				pharmacyDS = null;
				pharmacyActor = null;
				medCodingSystem = string.Empty;
				medCode = string.Empty;
				transmissionMethod = string.Empty;

                System.Diagnostics.Trace.Write("End individual med processing: " + med.ProductName, sessionID);
			}
		}

        System.Diagnostics.Trace.Write("End ProcessCCRMedications()", sessionID);
	}

	private bool GetRxID(string ePrescribeRxID, string externalID, string medDDI, DateTime rxDate, string medName, string route, string dosage, string strength, int refillQuantity, string sig, decimal quantity, string comments, ConnectionStringPointer dbID, out string rxID)
	{
		rxID = Allscripts.ePrescribe.Data.Prescription.GetRxIDForExternalRx(this.LicenseID, this.PatientID, ePrescribeRxID, externalID, medDDI, rxDate, medName, route, dosage, strength, refillQuantity, sig, quantity, comments, dbID);

		if (!string.IsNullOrEmpty(rxID) && rxID != Guid.Empty.ToString())
		{
			return true;
		}
		else
		{
			rxID = string.Empty;
			return false;
		}
	}

	private void ProcessCCRAllergies(string sessionID, ConnectionStringPointer dbID)
	{
        System.Diagnostics.Trace.Write("Start ProcessCCRAllergies()", sessionID);

		CCRAllergy[] allergies = null;

		if (this.IncludeAllergies)
		{
			allergies = this.m_ccr.Allergies;
		}

		if ( allergies != null)
		{
			//Load all allergies from patient
			DataSet patientAllergies = Patient.PatientAllergy(this.PatientID, dbID); 

			string patientAllergyID; //unique id for patient allergy
			string allergyType = "M"; //M=medication, C=class
			string allergyID;      // This is DDI value.
            string productCode = string.Empty; //Medispan product code
			string active;
			string start;          // Start date
			string end = string.Empty;            // End Date 
			System.Collections.Generic.List<string> allergenReactionIDs;
			System.Collections.Generic.List<string> otherReactions;
            bool isAllergyByClassID = false;
            bool isAllergyByParID = false;

			string[] allergyReactions;
			DataRow drAllergen;
			DataRow[] drPatientAllergy; 

			foreach (CCRAllergy allergy in allergies)
			{
                System.Diagnostics.Trace.Write("Start individual allergy processing", sessionID);

                // if patient contains "No Known Allergies" there will not be any
                // further allergies. if also any allergies available in CCR we should not process it.
                if ((allergy.Description.ToLower() == "no known allergies") ||
                    (allergy.Description.ToLower() == "no known allergy") ||
                    (allergy.Description.ToLower() == "no known drug allergies") ||
                    (allergy.Description.ToLower() == "no known drug allergy"))
                {
                    Allscripts.Impact.PatientAllergy.MarkNKA(this.PatientID, dbID);
                    break;
                }


				//Perform Allergy Insert / Update here, only Medication and Class allergies are supported in ePrescribe
                if (allergy.AllergyType == "Medication" || allergy.AllergyType == "Class")
				{
					allergyID = string.Empty;
                    productCode = string.Empty;
                    isAllergyByClassID = false;
                    isAllergyByParID = false;

                    if (allergy.AllergyType == "Medication")
                    {
                        allergyType = "M";

                        //Find medication by Medispan Par ID
                        if (!string.IsNullOrEmpty(allergy.MedispanProductCode))
                        {
                            allergyID = CCRProcessorUtils.PadLeadingZero(allergy.MedispanProductCode, CCRProcessorUtils.MEDISPAN_PAR_ID_LENGTH);
                            DataSet medDS = Medication.Load(allergyID, null, dbID);
                            if (medDS.Tables[0].Rows.Count > 0)
                                allergyID = medDS.Tables[0].Rows[0]["DDI"].ToString();
                            else
                                allergyID = string.Empty;
                        }

                        //Find medication by NDC
                        if (string.IsNullOrEmpty(allergyID) &&
                            !string.IsNullOrEmpty(allergy.NDCProductCode))
                        {
                            //Add logic to load medication DDI #
                            DataSet medDS = Medication.LoadByNDC(allergy.AllergyProductCode, dbID);
                            if (medDS.Tables[0].Rows.Count > 0)
                                allergyID = medDS.Tables[0].Rows[0]["DDI"].ToString();
                            else
                                allergyID = string.Empty;
                        }
                    }
                    else if (allergy.AllergyType == "Class")
                    {
                        allergyType = "C";

                        if (allergy.AllergyProductCodingSystem == CCRConstants.MEDISPAN_CLASS_CODING_SYSTEM ||
                            allergy.AllergyClassClassIDProductCodingSystem == CCRConstants.MEDISPAN_CLASS_CODING_SYSTEM)
                        {
                            isAllergyByClassID = true;
                            productCode = allergy.MedispanClassProductCode;
                        }
                        else if (allergy.AllergyProductCodingSystem == CCRConstants.MEDISPAN_CODING_SYSTEM ||
                            allergy.AllergyClassPARIDProductCodingSystem == CCRConstants.MEDISPAN_CODING_SYSTEM)
                        {
                            isAllergyByParID = true;
                            productCode = allergy.MedispanProductCode;
                        }

                        //Find class by Medispan Par Class ID
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            if (isAllergyByClassID)
                            {
                                //verify we have this class ID
                                allergyID = CCRProcessorUtils.PadLeadingZero(productCode, CCRProcessorUtils.MEDISPAN_PAR_CLASS_ID_LENGTH);
                                DataSet parClassDS = Medication.LoadPARClassByClassID(allergyID, dbID);
                                if (parClassDS.Tables[0].Rows.Count > 0)
                                    allergyID = parClassDS.Tables[0].Rows[0]["ClassNumber"].ToString();
                            }
                            else if (isAllergyByParID)
                            {
                                //lookup class ID by par ID
                                allergyID = CCRProcessorUtils.PadLeadingZero(productCode, CCRProcessorUtils.MEDISPAN_PAR_PAR_ID_LENGTH);
                                DataSet parClassDS = Medication.LoadPARClassByParID(allergyID, dbID);
                                if (parClassDS.Tables[0].Rows.Count > 0)
                                    allergyID = parClassDS.Tables[0].Rows[0]["ClassNumber"].ToString();
                            }
                        }
                    }

					if (!string.IsNullOrEmpty(allergyID))
					{
						drPatientAllergy = patientAllergies.Tables[0].Select(string.Format("AllergyID = '{0}'", allergyID));

                        //If we find single matches the allergyID, then update it.
                        //If we find mulitple matches with allergyID, then check along with ExternalAllergyID. if we find matches then update it
                        //Otherwise, create as new allergy
                        if (drPatientAllergy.Length == 1)
                        {
                            patientAllergyID = drPatientAllergy[0]["PatientAllergyID"].ToString();
                        }
                        else if (drPatientAllergy.Length > 1)
                        {
                            drPatientAllergy = patientAllergies.Tables[0].Select(string.Format("AllergyID = '{0}' and ExternalAllergyID = '{1}'", new string[] { allergyID, allergy.CCRDataObjectID }));
                            if (drPatientAllergy.Length > 0)
                            {
                                patientAllergyID = drPatientAllergy[0]["PatientAllergyID"].ToString();
                            }
                            else
                                patientAllergyID = Guid.NewGuid().ToString();
                        }
						else
							patientAllergyID = Guid.NewGuid().ToString();

						active = CCRProcessorUtils.InterpretAllergyStatus(allergy.Status);
						start = allergy.AllergyStartDate;
						if (string.IsNullOrEmpty(start))
							start = "1/1/1900";

						end = allergy.AllergyEndDate;
						if (string.IsNullOrEmpty(end))
							end = "1/1/1900";

						allergenReactionIDs = new List<string>();
						otherReactions = new List<string>();

						allergyReactions = allergy.AllergyReactions;

						if (allergyReactions != null && allergyReactions.Length > 0)
						{
							foreach (string reaction in allergyReactions)
							{
                                drAllergen = Allscripts.Impact.PatientAllergy.GetAllergenReaction(reaction, dbID);
								if (drAllergen != null)
									allergenReactionIDs.Add(drAllergen["AllergenReactionID"].ToString());
								else
									otherReactions.Add(reaction);
							}
						}

						//Save the allergy for patient
                        Allscripts.Impact.PatientAllergy.Save(
							patientAllergyID,
							this.PatientID,
							allergyType,
							allergyID,
							active == "Y" ? true : false,
							start,
							end,
							allergenReactionIDs.ToArray(),
							otherReactions.Count > 0 ? true : false,
							otherReactions.Count > 0 ? otherReactions[0] : string.Empty,
							Guid.Empty.ToString(),
							this.LicenseID,
                            "A",
                            dbID
						);

                        //Add logic to populate the external identifier for the allergy
                        if (!string.IsNullOrEmpty(allergy.CCRDataObjectID))
                            Allscripts.Impact.PatientAllergy.FileAllergyExternalID(allergy.CCRDataObjectID, LicenseID, patientAllergyID, PatientID, dbID); 
					}
				}

                System.Diagnostics.Trace.Write("End individual allergy processing", sessionID);
			}
		}

        System.Diagnostics.Trace.Write("End ProcessCCRAllergies()", sessionID);
	}

	private void ProcessCCRProblems(string sessionID, ConnectionStringPointer dbID)
	{
        System.Diagnostics.Trace.Write("Start ProcessCCRProblems()", sessionID);

		CCRProblem[] problems = null; 

		if (this.IncludeProblems)
		{
			problems = this.m_ccr.Problems;
		}

		if (problems != null)
		{
			//First get the list of dx for patient
			DataSet patientDxDS = Patient.PatientDiagnosis(this.PatientID, this.LicenseID, Guid.Empty.ToString(), dbID); 

			DataTable patientDxTable = patientDxDS.Tables[0];

			DataRow[] patientDxRows;
			string patientDxID = string.Empty;
			string patientDxStatus;
			DateTime patientDxStartDate;

			foreach (CCRProblem problem in problems)
			{
                System.Diagnostics.Trace.Write("Start individual problem processing", sessionID);

                patientDxID = string.Empty;
                patientDxStatus = CCRProcessorUtils.InterpretProblemStatus(problem.Status);
                var tempPatientDxID = Allscripts.Impact.PatientDiagnosis.GetPatientDiagnosisIDByExternalID(this.PatientID, problem.CCRDataObjectID, this.LicenseID, dbID);

                if (!String.IsNullOrEmpty(tempPatientDxID))
                {
                    patientDxID = tempPatientDxID;
                }
                else
                {
                    if (patientDxStatus == "Y")
                    { 
                        patientDxRows = patientDxTable.Select(string.Format("ICD9Code = '{0}' and Active = 'Y'", problem.ICD9Code));
                        if (patientDxRows.Length > 0)
                        {
                            //Existing Dx found match with ICD9Code, then consider as update
                            patientDxID = patientDxRows[0]["PatientDiagnosisID"].ToString();
                        }
                    }

                    if (String.IsNullOrEmpty(patientDxID))
                    {
                        //If not found for current patient, then consider as new Dx
                        patientDxID = Guid.NewGuid().ToString();
                    }
                }
				
				if (DateTime.TryParse(problem.ProblemStartDate, out patientDxStartDate))
					patientDxStartDate = patientDxStartDate.ToUniversalTime(); 
				else
					patientDxStartDate = CCRProcessorUtils.EPRESCRIBE_DEFAULT_DT; 

				//Save the Dx for patient 
                Allscripts.Impact.PatientDiagnosis.Save(
					patientDxID, 
					this.PatientID, 
					problem.ICD9Code, 
					patientDxStatus == "Y" ? true : false, 
					patientDxStartDate.ToString(), 
					Guid.Empty.ToString(), 
					this.LicenseID,
                    dbID);

                //Add logic to populate the external identifier for the allergy
                if (!string.IsNullOrEmpty(problem.CCRDataObjectID))
                    Allscripts.Impact.PatientDiagnosis.FileDiagnosisExternalID(problem.CCRDataObjectID, LicenseID, patientDxID, PatientID, dbID);

                System.Diagnostics.Trace.Write("End individual problem processing", sessionID);
			}
		}

        System.Diagnostics.Trace.Write("End ProcessCCRProblems()", sessionID);
	}
}

public class CCRProcessorUtils
{
	public const int MEDISPAN_PAR_ID_LENGTH = 6;
    public const int MEDISPAN_PAR_CLASS_ID_LENGTH = 5;
    public const int MEDISPAN_PAR_PAR_ID_LENGTH = 3;
	public const int NDC_CODE_LENGTH = 11;
	
	public const string PHARMACY_ID_TYPE_NABP = "NABP";
	
	public static DateTime EPRESCRIBE_DEFAULT_DT
	{
		get
		{
			return DateTime.Parse("1/1/1900");
		}
	}

	public static string PadLeadingZero(string value, int length)
	{
		StringBuilder paddedValue = new StringBuilder();
		if (value.Length < length)
		{
			paddedValue.Append('0', length - value.Length);
		}
		paddedValue.Append(value);

		return paddedValue.ToString();
	}

	public static string InterpretAllergyStatus(string status)
	{
		string interpretedStatus;

		switch (status.ToLower())
		{
			case "active":
				interpretedStatus = "Y";
				break;
			case "inactive":
				interpretedStatus = "N";
				break;
			case "eie":
			case "entered in error":
				interpretedStatus = "E";
				break;
			default:
				interpretedStatus = "Y";
				break;
		}

		return interpretedStatus;
	}

	public static string InterpretProblemStatus(string status)
	{
		string interpretedStatus;

		switch (status.ToLower())
		{
			case "active":
				interpretedStatus = "Y";
				break;
			case "inactive":
				interpretedStatus = "N";
				break;
			case "eie":
			case "entered in error":
				interpretedStatus = "E";
				break;
			default:
				interpretedStatus = "Y";
				break;
		}

		return interpretedStatus;
	}

    public static Constants.PrescriptionStatus InterpretRxStatus(string status)
	{
        Constants.PrescriptionStatus rxStatus;

		switch (status.ToLower())
		{
			case "0":
			case "new":
                rxStatus = Constants.PrescriptionStatus.NEW;
				break;
			case "1":
                rxStatus = Constants.PrescriptionStatus.PENDING_APPROVAL;
				break;
			case "2":
			case "rejected":
                rxStatus = Constants.PrescriptionStatus.REJECTED;
				break;
			case "3":
                rxStatus = Constants.PrescriptionStatus.PENDING_TRANSMISSION;
				break;
			case "4":
			case "discontinued":
                rxStatus = Constants.PrescriptionStatus.DISCONTINUED;
				break;
			case "5":
			case "entered in error":
			case "entered_in_error":
			case "eie":
                rxStatus = Constants.PrescriptionStatus.ENTERED_IN_ERROR;
				break;
			case "6":
                rxStatus = Constants.PrescriptionStatus.SCRIPT_PAD;
				break;
			case "7":
			case "completed":
                rxStatus = Constants.PrescriptionStatus.COMPLETE;
				break;
			case "8":
                rxStatus = Constants.PrescriptionStatus.REJECTED_FROM_PENDING;
				break;
			default:
                rxStatus = Constants.PrescriptionStatus.NEW;
				break;
		}

		return rxStatus;
	}

	public static string InterpretTransmissionMethod(string transMethod)
	{
		string rxTransmissionMethod;

		switch (transMethod.ToUpper())
		{
			case "P":
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.PRINTED;
				break;
			case "S":
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.SENT;
				break;
			case "H":
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.HISTORY;
				break;
			case "R":
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.RECORDED;
				break;
			case "D":
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.DISPENSED;
				break;
			case "U":
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.UNFULFILLED;
				break;
			default:
				//default as history transmission. 
                rxTransmissionMethod = Constants.PrescriptionTransmissionMethod.HISTORY;
				break;
		}

		return rxTransmissionMethod;
	}
}

}