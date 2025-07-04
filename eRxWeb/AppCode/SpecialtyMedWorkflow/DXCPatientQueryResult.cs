using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.SpecialtyMedWorkflow
{
    public class DXCPatientQueryResult
    {
        #region MedicationInformation
        public bool EndDateSpecified { get; set; }
        public string MedicationCodeValue { get; set;}
        public string BrandName { get; set; }
        public string PrescriptionID { get; set; }
        public string MedicationName { get; set; }
        public int Quantity { get; set; }
        public int DaysSupply { get; set; }
        public bool DAW { get; set; }
        public string ProductConcentration { get; set; }
        public int RefillsTotal { get; set; }
        public string DeliveryMethod { get; set; }
        public bool Administered { get; set; }
        public string Frequency { get; set; }
        public string Dose { get; set; }
        public string FreeSigText { get; set; }
        public string PharmacyID { get; set; }
        public string PharmacyName { get; set; }
        public DateTime OrderDateTime { get; set; }
        public DateTime StartDateTime { get; set; }
        #endregion

        #region InsuranceInformation
        public string InsuranceType { get; set; }
        public string InsuranceInformationSourceID { get; set; }
        public string InsuranceCodeValue { get; set; }
        public string InsuranceInformationSourceName { get; set; }
        public string InsuranceRelationToSubscriber { get; set; }
        #endregion

        #region DiagnosisInformation
        public string DiagnosisCodingSystem { get; set; }
        public string DiagnosisCodingValue { get; set; }
        #endregion
    }
}