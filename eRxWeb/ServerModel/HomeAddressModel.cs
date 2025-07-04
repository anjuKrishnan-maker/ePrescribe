
using Allscripts.ePrescribe.Data;
using System.Collections.Generic;

namespace eRxWeb.ServerModel
{
    public class HomeAddressStartUpModel
    {
        public List<StateModel> States { get; set; }
    }

    public class HomeAddressSaveRequest
    {
        public string HomeAddress { get; set; }
        public string HomeAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class HomeAddressSaveModel
    {
        public bool IsSaveSuccessful { get; set; }
        public string RedirectUrl { get; set; }
    }
}