using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Utilities;
using eRxWeb.AppCode;
using eRxWeb.State;
using Rx = Allscripts.Impact.Rx;
using RxUser = Allscripts.Impact.RxUser;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for RefReq
    /// </summary>
    [Serializable]

    public class RefReq : Rx
    {
        #region Constructors

        public RefReq()
        {
        }

        public RefReq(ScriptMessage sm)
        {
            //set base class vars
            base.LicenseID = sm.DBLicenseID;
            base.PatientID = sm.DBPatientID;
            base.ScriptMessageID = sm.DBScriptMessageID;
            base.RxID = sm.DBRxID;
            base.DDI = sm.DBDDID;

            //set class vars
            this.TransactionControlNumber = sm.TransCtrlNo;
            this.LinkedTransactionControlNumber = sm.LinkedTransCtrlNo;

            this.ProviderDEA = sm.ProviderDEA;
            this.ProviderNPI = sm.ProviderNPI;
            this.ProviderEmail = sm.ProviderEmail;
            this.ProviderID = sm.ProviderID;

            this.SupervisingProviderFirstName = sm.SupervisingProviderFirstName;
            this.SupervisingProviderLastName = sm.SupervisingProviderLastName;

            this.DispensedRxDrugDescription = sm.DispensedRxDrugDescription;
            this.DispensedRxQuantity = sm.DispensedRxQuantity;
            this.DispensedRxSIGText = sm.DispensedRxSIGText;
            this.DispensedRxRefills = sm.DispensedRxRefills;
            this.DispensedRxNotes = sm.DispensedRxNotes;

            this.PharmacyID = sm.DBPharmacyID;
            this.PharmacyNDPDPID = sm.DBPharmacyNetworkID;
            this.PharmacyName = sm.PharmacyName;
            this.PharmacyAddress1 = sm.PharmacyAddress1;
            this.PharmacyAddress2 = sm.PharmacyAddress2;
            this.PharmacyCity = sm.PharmacyCity;
            this.PharmacyState = sm.PharmacyState;
            this.PharmacyZip = sm.PharmacyZip;
            this.PharmacyPhoneNumber = sm.PracticePhoneNumber;
            this.PharmacyFaxNumber = sm.PharmacyFaxNumber;
            this.PharmacyEmail = sm.PharmacyEmail;

            this.PharmacistFirstName = sm.PharmacistFirstName;
            this.PharmacistLastName = sm.PharmacistLastName;

            this.PatientFirstName = sm.PatientFirstName;
            this.PatientLastName = sm.PatientLastName;
            this.PatientMI = sm.PatientMI;
            this.PatientGender = sm.PatientGender;
            this.PatientAddress1 = sm.PatientAddress1;
            this.PatientCity = sm.PatientCity;
            this.PatientState = sm.PatientState;
            this.PatientZip = sm.PatientZip;
            this.PatientPhoneNumber = sm.PatientPhoneNumber;
            this.PatientDOB = sm.PatientDOB;

            this.RawXML = sm.XmlMessage;
        }

        #endregion Constructors

        #region Properties

        public string TransactionControlNumber { get; set; }
        public string LinkedTransactionControlNumber { get; set; }

        public string ProviderDEA { get; set; }
        public string ProviderNPI { get; set; }
        public string ProviderEmail { get; set; }
        public new string ProviderID { get; set; }

        public string SupervisingProviderFirstName { get; set; }
        public string SupervisingProviderLastName { get; set; }

        public string DispensedRxDrugDescription { get; set; }
        public string DispensedRxQuantity { get; set; }
        public string DispensedRxSIGText { get; set; }
        public string DispensedRxRefills { get; set; }
        public string DispensedRxNotes { get; set; }

        public new string PharmacyID { get; set; }
        public string PharmacyNDPDPID { get; set; }
        public new string PharmacyName { get; set; }
        public string PharmacyAddress1 { get; set; }
        public string PharmacyAddress2 { get; set; }
        public string PharmacyCity { get; set; }
        public string PharmacyState { get; set; }
        public string PharmacyZip { get; set; }
        public string PharmacyPhoneNumber { get; set; }
        public string PharmacyFaxNumber { get; set; }
        public string PharmacyEmail { get; set; }

        public string PharmacistFirstName { get; set; }
        public string PharmacistLastName { get; set; }

        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientMI { get; set; }
        public string PatientGender { get; set; }
        public string PatientAddress1 { get; set; }
        public string PatientCity { get; set; }
        public string PatientState { get; set; }
        public string PatientZip { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string PatientDOB { get; set; }

        public string RawXML { get; set; }

        #endregion Properties

        public string GetREFREQApprovalDisplayMessage(string refillDrugDesc, string patientFirstName, string patientLastName)
        {
            string message = String.Empty;
            if (!String.IsNullOrEmpty(refillDrugDesc))
            {
                message = $"Refill for {refillDrugDesc} approved";
            }
            else
            {
                message = $"Refill approved";
            }
            if(!String.IsNullOrEmpty(patientFirstName) && !String.IsNullOrEmpty(patientLastName))
            {
                message += $" for {patientFirstName} {patientLastName}";
            }
            return message;
        }

        public static string SetDispensedAsCurrentAndReturnRedirect(string scriptMessageID, ConnectionStringPointer dbId, IStateContainer session, IPrescription iPrescription)
        {
            Rx rx = new Rx();
            ArrayList rxList = new ArrayList();
            ScriptMessage sm = new ScriptMessage(scriptMessageID, session["LICENSEID"].ToString(), session["USERID"].ToString(), dbId);

            rx.ScriptMessageID = scriptMessageID;

            session["PATIENTID"] = sm.DBPatientID;
            session["PHARMACYID"] = sm.DBPharmacyID;
            session["SentTo"] = sm.PharmacyName;

            rx.ProviderID = sm.ProviderID;
            rx.SigText = sm.DispensedRxSIGText;

            if (!String.IsNullOrWhiteSpace(sm.RxQuantity))
            {
                rx.Quantity = RxUtils.GetDecimal(sm.DispensedRxQuantity, "Quantity");
            }

            rx.DAW = sm.DispensedDaw == "Y";
            rx.DDI = sm.DBDDID;

            if (!String.IsNullOrWhiteSpace(sm.DispensedRxRefills) && !sm.RxRefills.Equals("PRN"))
            {
                //DispensedRxRefills = dispenses (to save as refills we must subtract 1)
                rx.Refills = Convert.ToInt32(sm.DispensedRxRefills) - 1;
            }

            //ok, if there's no DDI AND its not a compound drug, we'll have to search for it
            if (string.IsNullOrEmpty(sm.DBDDID))
            {
                if (string.IsNullOrEmpty(sm.IsRxCompound))
                {
                    var pharmDetails = StringHelper.GetPharmacyName(
                        sm.PharmacyName,
                        sm.PharmacyAddress1,
                        sm.PharmacyAddress2,
                        sm.PharmacyCity,
                        sm.PharmacyState,
                        sm.PharmacyZip,
                        sm.PharmacyPhoneNumber
                    );

                    var rxDetails = StringHelper.GetRxDetails(
                        sm.DispensedRxDrugDescription,
                        sm.DispensedRxSIGText,
                        sm.DispensedRxQuantity,
                        sm.DispensedDaysSupply,
                        sm.DispensedRxRefills,
                        sm.DispensedDaw,
                        sm.DispensedCreated,
                        sm.DispensedDateLastFill,
                        sm.DispensedRxNotes
                    );
                    if (string.IsNullOrEmpty(sm.DispensedControlledSubstanceCode))
                    {
                        session[Constants.SessionVariables.IsReconcileREFREQNonCS] = true;
                    }
                    else if (int.Parse(sm.DispensedControlledSubstanceCode) >= 2 && int.Parse(sm.DispensedControlledSubstanceCode) <= 5) { 
                        session[Constants.SessionVariables.IsCsRefReqWorkflow] = true;
                    }
                    return Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"RefillPharmacy={pharmDetails}&RxDetails={rxDetails}&from={Constants.PageNames.APPROVE_REFILL_TASK}&SearchText=" + HttpUtility.UrlEncode(sm.DispensedRxDrugDescription.Split(' ').FirstOrDefault()))}";
                }
            }

            if (!string.IsNullOrEmpty(rx.DDI))
            {
                string coverageId = session.GetString("SelectedCoverageID", "0");
                string formularyId = session.GetStringOrEmpty("FormularyID");
                string otcCoverage = session.GetStringOrEmpty("OTCCoverage");
                int genericDrugPolicy = session.GetInt("GenericDrugPolicy", 0);
                int unListedDrugPolicy = session.GetInt("UnListedDrugPolicy", 0); ;

                ePrescribeSvc.Medication medicationInfo = EPSBroker.LoadMedicationByDDIAndCoverage(rx.DDI,
                    coverageId,
                    formularyId,
                    otcCoverage,
                    genericDrugPolicy,
                    unListedDrugPolicy,
                    dbId);

                if (medicationInfo != null)
                {
                    rx.MedicationName = medicationInfo.MedicationName;
                    rx.Strength = medicationInfo.Strength;
                    rx.ControlledSubstanceCode = medicationInfo.ControlledSubstanceCode;
                    rx.StrengthUOM = medicationInfo.StrengthUOM;
                    rx.RouteOfAdminCode = medicationInfo.RouteOfAdminCode;
                    rx.RouteOfAdminDescription = medicationInfo.RouteOfAdmin;
                    rx.DosageFormCode = medicationInfo.DosageFormCode;
                    rx.DosageFormDescription = medicationInfo.DosageForm;

                    rx.StateControlledSubstanceCode = Prescription.GetStateControlledSubstanceCode(rx.DDI, session["PRACTICESTATE"].ToString(), sm.PharmacyState, dbId);

                    if (session["FormularyActive"] != null && session["FormularyActive"].ToString() == "Y")
                    {
                        rx.FormularyStatus = medicationInfo.FormularyStatus;
                        rx.LevelOfPreferedness = medicationInfo.LevelOfPreferedness;
                        rx.IsOTC = medicationInfo.IsOTC;
                    }
                    rx.CoverageID = (session["SelectedCoverageID"] != null) ? Convert.ToInt64(session["SelectedCoverageID"].ToString()) : 0;
                    rx.FormularyID = (session["FormularyID"] != null) ? session["FormularyID"].ToString().Trim() : null;
                    rx.PlanID = (session["PlanID"] != null) ? session["PlanID"].ToString().Trim() : null;
                }

                if (iPrescription.IsValidMassOpiate(
                    session.GetStringOrEmpty(Constants.SessionVariables.PracticeState),
                    medicationInfo.GPI,
                    rx.ControlledSubstanceCode,
                    Convert.ToBoolean(session["HasExpiredDEA"]),
                    (List<string>)(session["DEASCHEDULESALLOWED"]))
                )
                {
                    rx.Notes = Constants.PartialFillUponPatientRequest;
                }
            }

            rxList.Add(rx);
            session[Constants.SessionVariables.TaskScriptMessageId] = scriptMessageID;

            session["RxList"] = rxList;

            string requestedCSCodeByNDC = string.Empty;

            if (string.IsNullOrEmpty(sm.DispensedControlledSubstanceCode))
            {
                var dispensedMedByNDC = Allscripts.Impact.Medication.LoadByNDC(sm.DispensedNdc, dbId);
                if(dispensedMedByNDC.Tables[0].Rows.Count > 0)
                {
                    requestedCSCodeByNDC = dispensedMedByNDC.Tables[0].Rows[0]["ControlledSubstanceCode"].ToString().Trim();
                }
            }

            bool isRequestedMedCS = !(string.IsNullOrEmpty(sm.DispensedControlledSubstanceCode) && string.IsNullOrEmpty(requestedCSCodeByNDC));

            //Non CS workflow:
            if (!isRequestedMedCS)
            {
                var userType = session.Cast(Constants.SessionVariables.UserType, Constants.UserCategory.GENERAL_USER);
                if (String.IsNullOrEmpty(sm.IsRxCompound))
                {
                    return (UserRoles.IsPob(userType) ? Constants.PageNames.NURSE_SIG : Constants.PageNames.SIG);
                }
                else
                {
                    var pharmDetails = StringHelper.GetPharmacyName(
                        sm.PharmacyName,
                        sm.PharmacyAddress1,
                        sm.PharmacyAddress2,
                        sm.PharmacyCity,
                        sm.PharmacyState,
                        sm.PharmacyZip,
                        sm.PharmacyPhoneNumber
                    );

                    var rxDetails = StringHelper.GetRxDetails(
                        sm.DispensedRxDrugDescription,
                        sm.DispensedRxSIGText,
                        sm.DispensedRxQuantity,
                        sm.DispensedDaysSupply,
                        sm.DispensedRxRefills,
                        sm.DispensedDaw,
                        sm.DispensedCreated,
                        sm.DispensedDateLastFill,
                        sm.DispensedRxNotes
                    );
                    session[Constants.SessionVariables.IsReconcileREFREQNonCS] = true;
                    return Constants.PageNames.REDIRECT_TO_ANGULAR + "?componentName=" + Constants.PageNames.SELECT_MEDICATION + $"&componentParameters={JsonHelper.ConvertUrlParameters($"RefillPharmacy={pharmDetails}&RxDetails={rxDetails}&from={Constants.PageNames.APPROVE_REFILL_TASK}&SearchText=" + HttpUtility.UrlEncode(sm.DispensedRxDrugDescription.Split(' ').FirstOrDefault()))}";
                }
            }
            //CS Workflow
            else if (isRequestedMedCS && sm.MessageType == Constants.MessageTypes.REFILL_REQUEST) //CS reqref workflow should go to select medication page
            {
                var pharmDetails = StringHelper.GetPharmacyName(
                    sm.PharmacyName,
                    sm.PharmacyAddress1,
                    sm.PharmacyAddress2,
                    sm.PharmacyCity,
                    sm.PharmacyState,
                    sm.PharmacyZip,
                    sm.PharmacyPhoneNumber);
                var rxDetails = StringHelper.GetRxDetails(
                    sm.DispensedRxDrugDescription,
                    sm.DispensedRxSIGText,
                    sm.DispensedRxQuantity,
                    sm.DispensedDaysSupply,
                    sm.DispensedRxRefills,
                    sm.DispensedDaw,
                    sm.DispensedCreated,
                    sm.DispensedDateLastFill,
                    sm.DispensedRxNotes);
                session[Constants.SessionVariables.IsCsRefReqWorkflow] = true;
                //Delete all script pad entries for current user and all providers
                ScriptPadUtil.RemoveAllRxFromScriptPad(session);
                
                return CsMedUtil.RedirectForCSMed(pharmDetails, rxDetails, sm.DispensedRxDrugDescription);
            }

            return string.Empty;
        }
    }
}