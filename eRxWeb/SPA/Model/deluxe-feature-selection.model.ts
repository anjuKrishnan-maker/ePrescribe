export class DeluxeFeaturePricing {
    PricingStructureDesc: string;
    Price: number;
    Id: number;
       
}

export class DeluxeFeature {
    deluxeFeatureSelection: DeluxeFeatureSelection;
    product: Array<Products> = new Array<Products>();
    forceCompulsoryRestrictions: ForceCompulsoryRestrictions;
    subscribedModule: string;
}
export class DeluxeFeatureSelection {
    deluxeFeaturePricing: Array<DeluxeFeaturePricing> = new Array<DeluxeFeaturePricing>();
    EnterprisePricing: boolean; 
}

export class ForceCompulsoryRestrictions {
    IsForcePaymentState: boolean;
    IsEnterpriseCompulsoryBasic: boolean;    
}

export class Products {
    ProductName: string;
    ProductPrice: number;
}

export enum EnumDeluxeFeature {
    CompulsoryBasic ="CompulsoryBasic",   
    DeluxeEpa ="DeluxeEpa",
    DeluxeEpaLogRx ="DeluxeEpaLogRx",   
    DeluxeEPCSEpa2017 ="DeluxeEPCSEpa2017",  
    DeluxeEPCSEpaLogRx2017 ="DeluxeEPCSEpaLogRx2017",    
}

export class DeluxeFeatureMessageModelTag {
    public static ForcePurchase: string = "ForcePurchase";
    public static ValidationError: string = "ValidationError";
}

export class DeluxeFeatureStaticTexts {
    public static ForceFeatureSelectionErrorMsg: string = "You must purchase a license in order to use the application.";
    public static NoFeatureSelectionErrorMsg: string = "Please select a feature to continue.";
}
