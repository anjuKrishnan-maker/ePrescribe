export class ReviewHistory {

    HistoryItems: ReviewHistoryItem[]
}

export class ReviewHistoryItem {
    // items from service
    RxID: string;
    RxDate: Date;
    Diagnosis: string;
    Status: number;
    StatusDescription: string;
    RxSource: string;
    MedHistoryExists: boolean;
    Prescription: string;
    SelectionGroupId: SelectionGroup;
    ExtraDetailAvailable: boolean;
    IsScriptCancelRxEligible: boolean;
    OriginalNewRxTrnsCtrlNo: string;
    IsPbmMed: boolean;
    //....

    // internal vars
    Selected: boolean;
    IsSelectionDisabled: boolean;
    DisplayFillDetails: boolean;
    FillDetailsHtml: string;
}

export class DataRetrievalContext {
    SortColumnName: string;
    SortDirection: SortDirectionEnum;
    SkipRows: number;
    FetchRows: number;

    constructor(column: string, direction: SortDirectionEnum, skipRows: number, fetchRows: number) {

        this.SortColumnName = column;
        this.SortDirection = direction;
        this.SkipRows = skipRows;
        this.FetchRows = fetchRows;
    }

    public ReverseSort(pageSize: number) {
        if (this.SortDirection === SortDirectionEnum.ASC) {
            this.SortDirection = SortDirectionEnum.DESC;
        } else {
            this.SortDirection = SortDirectionEnum.ASC;
        }

        this.FetchRows = pageSize;
        this.SkipRows = 0;
    }
}

export enum SortDirectionEnum {
    ASC = 0,
    DESC = 1
}

export enum StatusFilterEnum {
      Active = 0,
      Inactive = 1,
      All = 2
}

export class FillHistoryTimelineModel {
    TimeLineLabel: string;
    FillDate: Date;
}

export enum SelectionGroup {
    EieComplete = 0,
    DiscontinueComplete = 1,
    DiscontinueEieComplete = 2,
    Eie = 3,
    AlwaysDisabled = 4

}

export class EieActionRequestModel {
    
    RxID: string;
    IsPbmMed: boolean;

    constructor(rxID: string, isPbmMed: boolean) {
        this.RxID = rxID;
        this.IsPbmMed = isPbmMed;
    }

}

export class ReviewHistoryStartupParameters {
    IsAddDiagnosisVisible: boolean;
    IsSsoLockdownMode: boolean; 
    IsInactivePatient: boolean;
    Providers: Array<ReviewHistoryProvider>;
    DelegateProviderId: string;
    UserType: ReviewHistoryUserType;
    IsRestrictedPatient: boolean;
}

export class ReviewHistoryProvider {
    ProviderName: string;
    ProviderId: string;
    UserTypeID: number;
}

export enum ReviewHistoryUserType {
    Provider = 1,
    PAwithSupervision = 2,
    Staff = 3
}

