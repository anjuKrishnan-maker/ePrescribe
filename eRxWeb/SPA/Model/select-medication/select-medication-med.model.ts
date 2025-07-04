import { ImageModel,GridRow } from '../model.import.def';
export enum MedNameDisplayOption {
    DisplayLinkWithAdd = 1,
    DisplayLabel = 5,
    DisplayLinkWithLibraryLink = 10
}

//Reduce the number of fields here
export class SelectMedicationMedModel extends GridRow {
    DDI: string;
    DrugName: string;
    DrugExt: string;
    Quantity: number;
    DAW: boolean;
    Refill: number;
    DayOfSupply: number;
    BenefitImageUrl: string;
    LevelOfPreferedness: string;
    FormularyImageUrl: number;
    FormularyStatus: number;
    Selected: boolean;
    LibraryUrl: string;
    BenefitImage: ImageModel;
    FormularyImage: ImageModel;
    DURImage: ImageModel;
    IsGeneric: boolean;
    DrugNameDisplayOption: MedNameDisplayOption;
    Strength: string;
    StrengthUOM: string;
    DosageForm: string;
    RouteofAdmin: string;
    RxDate: string;
    RxStatus: string;   
    RxStatusInfo: string;
    FormStatusCode: string;
    IsOTC: boolean;
    SIGID: string;
    ControlledSubstanceCode: string;
    GPPC: string;
    PackageSize: string;
    PackageQuantity: string;
    PackageUOM: string;
    PackageDescription: string;
    MedicationName: string;
    RouteofAdminCode: string;
    DosageFormCode: string;
    SIGText: string;
    PriorAuth: boolean;
    NDC: string;
    IsCouponAvailable: boolean;
    IsSpecialtyMed: boolean;
    GPI: string;
    PreDURDrugDrug: string;
    PreDURDUP: string;
    PreDURPAR: string;
    SIGTypeID: number;
}