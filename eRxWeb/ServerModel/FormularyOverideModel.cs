namespace eRxWeb.ServerModel
{
    public class IgnoreReasonsModel
    {
        public string[] IgnoreReasons { get; set; }
    }

    public class MedicationModel
    {
        public string RxID { get; set; }
        public string Medication { get; set; }
    }

    public class FormularyOverideModel
    {
        public IgnoreReasonsModel IgnoreReasons { get; set; }
        public MedicationModel[] Medication { get; set; }
    }

    public class SelectedOverrideReasonModel
    {
        public string RxID { get; set; }
        public int OverideReason { get; set; }
    }
}