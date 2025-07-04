class RxDetailModel {
    Pharmacy: RxDetailPharmacy;
    History: RxHistory;
}

class RxDetailPharmacy {
    PharmacyAddressDetail: string;
    PharmacyCityStateZipDetail: string;
    PharmacyFaxDetail: string;
    PharmacyNameDetail: string;
    PharmacyPhoneDetail: string;
}
class RxHistory {
    lProviderPhone: string;
    PharmacistName: string;
    PharmacyAddress: string;
    PharmacyCityStateZip: string;
    PharmacyEmail: string;
    PharmacyFax: string;
    PharmacyName: string;
    PharmacyPhone: string;
    PracticeName: string;
    ProviderAddress: string;
    ProviderCityStateZip: string;
    ProviderEmail: string;
    ProviderFax: string;
    ProviderName: string;
}

export { RxDetailModel };