
namespace eRxWeb.ServerModel
{
    class RxDetailModel
    {
        public RxDetailModel()
        {
            Pharmacy = new RxDetailPharmacy();
            History = new RxHistory();
        }
        public RxDetailPharmacy Pharmacy { get; set; }
        public RxHistory History { get; set; }
    }

    class RxDetailPharmacy
    {
        public string PharmacyAddressDetail { get; internal set; }
        public string PharmacyCityStateZipDetail { get; internal set; }
        public string PharmacyFaxDetail { get; internal set; }
        public string PharmacyNameDetail { get; internal set; }
        public string PharmacyPhoneDetail { get; internal set; }
    }
    class RxHistory
    {
        internal string lProviderPhone;

        public string PharmacistName { get; internal set; }
        public string PharmacyAddress { get; internal set; }
        public string PharmacyCityStateZip { get; internal set; }
        public string PharmacyEmail { get; internal set; }
        public string PharmacyFax { get; internal set; }
        public string PharmacyName { get; internal set; }
        public string PharmacyPhone { get; internal set; }
        public string PracticeName { get; internal set; }
        public string ProviderAddress { get; internal set; }
        public string ProviderCityStateZip { get; internal set; }
        public string ProviderEmail { get; internal set; }
        public string ProviderFax { get; internal set; }
        public string ProviderName { get; internal set; }
    }
}