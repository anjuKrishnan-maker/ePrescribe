import { Component, ElementRef, ViewChild } from '@angular/core';

import { BaseComponent } from '../../base.component';

import { ScriptPadservice, EventService } from '../../../../services/service.import.def';
import { MessageQueInfoModel, MedCompletionHistoryModel, CancelRxItemModel, MedCompletionHistoryArgs, MedCompletionHistoryAction } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, current_context } from '../../../../tools/constants';

import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';
@Component({
    selector: 'erx-med-hist-competion',
    templateUrl: './med-history-completion.template.html',
    styleUrls: ['./med-history-completion.style.css']
})
export class MedHistoryCompletionComponent extends BaseComponent {
    messageQueInfo: MessageQueInfoModel;
    medCompletionHistories: MedCompletionHistoryModel[];
    selectAll: boolean = false;
    anySelected: boolean = true;
    warningMessage: string;
    ScriptsToCancel: CancelRxItemModel[] = [];
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    callBack: (data: MedCompletionHistoryArgs) => void;

    constructor(private scrptSvc: ScriptPadservice, cd: ElementRef, private svE: EventService) {
        super(cd);
        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ReviewScriptOpenModal, (val: any) => {
            this.medCompletionHistories = val.MedCompletionHistory;
            this.selectAll = false;
            this.anySelected = true;
            this.warningMessage = '';
            if (val.callback != undefined && typeof (val.callback) == 'function') {
                this.callBack = val.callback;
            }
            this.modalPopup.OpenPopup();
        });
    }
    content: any;
    CallChildFunction(arg: any) {

    }
    checkbox(val) {
        this.selectAll = !val;
        for (let data of this.medCompletionHistories) {
            data.Checked = this.selectAll;
        }
    }
    checkIndividual(val: MedCompletionHistoryModel) {
        val.Checked = !val.Checked;
    }

    CompleteAndContinue() {
        let selected = this.medCompletionHistories.filter((x) => x.Checked);
        if (selected.length <= 0) {
            this.anySelected = false;
            this.warningMessage = 'Please select at least one medication from the list.';
        }
        else {
            let data = [];
            for (let x of selected) {
                data.push({ RxID: x.RxID });
            }
            this.scrptSvc.CompleteScriptPadMedHistory({ medCompleteHistories: data }).subscribe((response) => {
                    this.modalPopup.ClosePopup();
                    let mcha = new MedCompletionHistoryArgs();
                    mcha.Action = MedCompletionHistoryAction.CompleteAndContinue;
                    mcha.Data = response;
                    this.callBack(mcha);
                }
            );
        }
    }
    Cancel() {
        this.modalPopup.ClosePopup();
    }
    Continue() {
        this.modalPopup.ClosePopup();
        let mcha = new MedCompletionHistoryArgs();
        mcha.Action = MedCompletionHistoryAction.Continue;
        this.callBack(mcha);
    }

}
