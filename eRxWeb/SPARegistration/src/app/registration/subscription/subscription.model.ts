export class subscription{
    loginUrl: string;
    price: PricingStructurePrice;
    enterprisePricing: boolean; 
}

export class PricingStructurePrice {
    basic: string;
    deluxe: string;
    deluxeLogRx: string;
    deluxeEpcs: string;
    deluxeEpcsLogRx: string;
    epcsSetup: string;
}

export enum PricingStructureEnum {
    CompulsoryBasic = 12,
    DeluxeEpa = 4,
    DeluxeEpaLogRx = 11,
    DeluxeEPCSEpa2017 = 13,
    DeluxeEPCSEpaLogRx2017 = 14,
}