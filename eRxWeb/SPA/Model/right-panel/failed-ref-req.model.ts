export class FailedRefReqModel {
    PatientName: string;
    PatientDOB: string;
    DrugDescription: string;
    Refills: string;
    PharmacyName: string;
    PharmacyAddress: string;
    PharmacyPhone: string;
    PharmacyFax: string;
}

export class DeniedRefReqMessages {
    RefReqErrors: FailedRefReqModel[];
    DeniedRefReqs: FailedRefReqModel[];   
}