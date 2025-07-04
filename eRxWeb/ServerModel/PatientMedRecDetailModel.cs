namespace eRxWeb.ServerModel
{
    public class PatientMedRecDetailModel
    {
        public string Type { get; set; }
        public string ReconciliationMessage { get; set; }

        public bool DoesPatientHaveValidMedAndAllergy { get; set; }
    }
}