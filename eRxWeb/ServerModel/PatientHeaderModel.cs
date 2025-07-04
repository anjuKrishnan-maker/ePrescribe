using System.Collections.Generic;


namespace eRxWeb.ServerModel
{
    public class PatientHeaderModel
    {
    }
    public class PatientHeader
    {
        public PatientHeader()
        {
            this.ActiveDignosises = new List<ActiveDignosis>();
            this.ActiveAllergies = new List<ActiveAllergy>();
            this.ActiveMeds = new List<ActiveMeds>();
        }
        public string ActiveMed { get; internal set; }
        public string Allergy { get; internal set; }
        public string DOB { get; internal set; }
        public string Dx { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; internal set; }
        public object LastPharmacyName { get; internal set; }
        public int MaiIndicator { get; internal set; }
        public string MaiIndicatorImageUrl { get; internal set; }
        public string MaiIndicatorToolTip { get; internal set; }
        public bool IsMoEpcs { get; internal set; }
        public bool MoreActiveMedVisible { get; internal set; }
        public bool MoreMailOrderPharmVisible { get; internal set; }
        public string MRN { get; internal set; }
        public bool IsRetailEpcs { get; internal set; }
        public string PrefMOP { get; internal set; }
        public bool RemMOPharmVisible { get; internal set; }
        public bool RemPharmacyVisible { get; internal set; }
        public string PatientID { get; internal set; }
        public string RealName { get; internal set; }
        public bool AllowPatientEdit { get; set; }

        public bool AllowAllergyEdit { get; set; }
        public bool AllowPharmacyEdit { get; set; }
        public bool AllowDiagnosisEdit { get; set; }

        public List<ActiveDignosis> ActiveDignosises { get; set; }
        public List<ActiveAllergy> ActiveAllergies { get; set; }
        public List<ActiveMeds> ActiveMeds { get; set; }
        public bool MoreActiveProblem { get; internal set; }
        public bool MoreActiveAllergy { get; internal set; }
        public bool MoreRetailPharm { get; internal set; }

        public bool IsRestrictedUser { get; set; }
        public bool IsRestrictedPatient { get; set; }
        public bool IsVipPatient { get; set; }
        public string ToolTip { get; set; }
        public string HeightLabel { get; set; }
        public string WeightLabel { get; set; }
    }

    public class PatientResponse
    {
        public string PatientId { get; set; }
    }
    public  class ActiveDignosis
    {
        public string Diagnosis { get; set; }
        public string StartDate { get; set; }
    }

    public class ActiveAllergy
    {
        public string Name { get; set; }
        public string StartDate { get; set; }
    }

    public class ActiveMeds
    {
        public string Name { get; set; }
        public string StartDate { get; set; }
    }
}