using Allscripts.ePrescribe.Objects.StoredProcReturnObjects;

namespace eRxWeb.ServerModel
{
    public class FillHistoryTimeLineModel
    {
        public string TimeLineLabel { get; set; }
        public string FillDate { get; set; }

        public FillHistoryTimeLineModel(GetFillDatesForRxIdRow fillRecord)
        {
            string pharmName = string.IsNullOrWhiteSpace(fillRecord.PharmName) ? "Unknown" : fillRecord.PharmName;

            if (string.IsNullOrWhiteSpace(fillRecord.PBMSource))
            {
                TimeLineLabel = $"RxFill {fillRecord.RxFillStatus}, {fillRecord.Message} Pharmacy: {pharmName}";
            }
            else
            {
                TimeLineLabel = $"{fillRecord.PBMSource}, Pharmacy: {pharmName}";
            }
            FillDate = fillRecord.FillDate.ToShortDateString();
        }
    }
}