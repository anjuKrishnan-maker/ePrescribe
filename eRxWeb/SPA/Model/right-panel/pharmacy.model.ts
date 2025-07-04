class BasePharmacyModel {
    Name: string;
    State: string;
    ZipCode: string;
    City: string;
    Address1: string;
    Fax: string;
    Phone: string;
}

class PharmacyModel extends BasePharmacyModel {
    RatingImageUrl: string;
    RatingText: string;
    TotalErrors: string;
    TotalRx: string;
    DeliveryRate: string;
    EPCS: string;
    TransMethod: string;
    CrossStreet: string;
    NABP: string;
}

class PharmacyRequestModel {
    PharmacyID: string;
}

class NewPharmacyModel extends BasePharmacyModel {
    Address2: string;
    Comment: string;
}
// Name, Address1, Address2, City, State, ZipCode, Phone, Fax, Comment
export { PharmacyModel, NewPharmacyModel, PharmacyRequestModel}                     