export class CancelRxItemModel {
    RxID: string;
    Prescription: string; 
    OriginalNewRxTrnsCtrlNo: string;
    IsSelected: boolean;

    public constructor(rxID: string, prescription: string, trnsCtrlNo: string) {
        this.RxID = rxID;
        this.Prescription = prescription;
        this.IsSelected = false;
        this.OriginalNewRxTrnsCtrlNo = trnsCtrlNo;
    }

}

export enum CancelRxActions {
    ContinueWithoutCancelRx,
    ContinueWithCancelRx,
    Back
}

export class CancelRxDialogArgs {
    Action: CancelRxActions;
    Items: CancelRxItemModel[];

    constructor(action: CancelRxActions, items: CancelRxItemModel[]) {
        this.Action = action;
        this.Items = items;
    }
}

export class SendCancelRxRequestModel {
    RxID: string;
    OriginalNewRxTrnsCtrlNo: string;

    constructor(rxID: string, trnsCtlNo: string) {
        this.RxID = rxID;
        this.OriginalNewRxTrnsCtrlNo = trnsCtlNo;
    }
}
