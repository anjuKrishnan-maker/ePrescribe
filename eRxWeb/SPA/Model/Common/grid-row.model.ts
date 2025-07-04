export enum GridRowSelectionStatus {
    NotSelected= 0,
    Selected= 1,
    SelectActive = 6
}
export class GridRow {
    index: number;
    RowStatus:GridRowSelectionStatus
}