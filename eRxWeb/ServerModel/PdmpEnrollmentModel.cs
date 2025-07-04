using System.Collections.Generic;


namespace eRxWeb.ServerModel
{
    public class PdmpEnrollmentModel
    {
        public string Product { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string ContactNameAndTitle { get; set; }
        public string ContactPhoneNo { get; set; }
        public string ContactFax { get; set; }
        public string ContactEmail { get; set; }
        public string NumberOfProviders { get; set; }
        public string TermsAndConditions { get; set; }
        public int TermsAndConditionsID { get; set; }
        public bool TermsAndConditionsUserAcceptance { get; set; }

        public PdmpEnrollmentModel()
        {
            Product = string.Empty;
            AccountName = string.Empty;
            AccountNumber = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            City = string.Empty;
            State = string.Empty;
            ZipCode = string.Empty;
            ContactNameAndTitle = string.Empty;
            ContactPhoneNo = string.Empty;
            ContactFax = string.Empty;
            ContactEmail = string.Empty;
            NumberOfProviders = string.Empty;
            TermsAndConditions = string.Empty;
            TermsAndConditionsUserAcceptance = false;
        }
    }    
}