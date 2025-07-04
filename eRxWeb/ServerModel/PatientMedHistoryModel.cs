using System;

namespace eRxWeb.ServerModel
{
    public class PatientMedHistoryModel
    {
        public string rxDate { get; set; }
        public string diagnosis { get; set; }
        public string status { get; set; }
        public string source { get; set; }
        public string medDetail { get; set; }
    }
}