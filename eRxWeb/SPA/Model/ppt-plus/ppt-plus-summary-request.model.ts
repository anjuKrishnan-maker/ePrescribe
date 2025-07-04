import {GridRowSelectionStatus } from '../model.import.def';
export class PptPlusSummaryRequest {
    RemovedIndexes: string[];
    DDI: string;
    GPPC: string;
    PackUom: string;
    Quantity: string;
    PackSize: string;
    PackQuantity: string;
    Refills: string;
    DaysSupply: string;
    IsDaw: string;
    MedSearchIndex: string;
    Medname: string;
    Strength: string;
    RowSelectionStatus: GridRowSelectionStatus;
    Index: number;
    MedExtension: string;
}

export interface IPptPlusSummaryRequestEventArgs {
    PptPlusSummaryRequests: PptPlusSummaryRequest[];
}