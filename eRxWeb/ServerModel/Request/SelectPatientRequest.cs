using System;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.ServerModel.Request
{
    public class SearchPatientsRequest
    {
        public string LicenseGuid { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        public string ChartId { get; set; }
        public string WildCard { get; set; }
        public string UserGuid { get; set; }
        public bool HasVIPPatients { get; set; }
        public UserCategory UserType { get; set; }
        public string PatientGuid { get; set; }
        public bool IncludeInactive { get; set; }
    }
}