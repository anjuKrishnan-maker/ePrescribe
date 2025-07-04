import { Component, ElementRef, ViewChild } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { EventService, CancelRxService } from '../../../../services/service.import.def';
import { CancelRxItemModel, CancelRxActions, CancelRxDialogArgs, SendCancelRxRequestModel } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';


@Component({
    selector: 'erx-completed-rx-cancel-rx',
    templateUrl: './completed-rx-cancel-rx.template.html',
})
export class CompletedRxCancelRxComponent extends BaseComponent {
    CancelRxItems: Array<CancelRxItemModel>;
    AllSelected: boolean;
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    callBack: (data: CancelRxDialogArgs) => void;

    constructor(private svE: EventService, cd: ElementRef, private cancelRxSvc: CancelRxService) {
        super(cd);
        this.svE.subscribeEvent(EVENT_COMPONENT_ID.CompletedRxCancelRxOpen, (val: any) => {
            this.CancelRxItems = val.CancelRxItems;
            if (val.callback != undefined && typeof (val.callback) == 'function') {
                this.callBack = val.callback;
            }
            this.modalPopup.OpenPopup();
        });
    }

    public SelectAllCheckedForCancelRx(isSelected: boolean) {
        this.CancelRxItems.forEach(item => item.IsSelected = isSelected);
    }

    IsAnyMedSelected() {
        if (this.CancelRxItems === undefined || this.CancelRxItems === null)
            return false;

        return this.CancelRxItems.filter(item => item.IsSelected).length > 0;
    }

    public ExecuteAction(action: CancelRxActions) {
        if (action == CancelRxActions.ContinueWithCancelRx) {
            var itemsToCancel = this.CancelRxItems.map(item => new SendCancelRxRequestModel(item.RxID, item.OriginalNewRxTrnsCtrlNo));

            if (itemsToCancel.length > 0) {
                this.cancelRxSvc.SendCompletedRxCancelRx(itemsToCancel).subscribe();
            }
        }
        let crda = new CancelRxDialogArgs(action, null);
        this.callBack(crda);
        this.modalPopup.ClosePopup();
    }   
}
