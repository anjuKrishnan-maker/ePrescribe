import { Component, Input, Output, EventEmitter } from '@angular/core';
import { EventService } from '../../../../services/service.import.def';
import { CancelRxItemModel, CancelRxActions, CancelRxDialogArgs } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';


@Component({
    selector: 'erx-cancel-rx',
    templateUrl: './cancel-rx.template.html',
    inputs: ['CancelRxItems']
})
export class CancelRxComponent {
    @Output()
    SelectionComplete: EventEmitter<CancelRxDialogArgs> = new EventEmitter<CancelRxDialogArgs>();
    @Input()
    CancelRxItems: Array<CancelRxItemModel>;

    constructor(private evSvc: EventService) {
        this.AllSelected = false;
    }

    public SelectAllCheckedForCancelRx(isSelected: boolean) {
        this.CancelRxItems.forEach(item => item.IsSelected = isSelected);
    }

    AllSelected: boolean;
    currentAction: string;

    public ExecuteAction(action: CancelRxActions) {
        switch (action) {
        case CancelRxActions.ContinueWithCancelRx:
        {
            this.SelectionComplete.emit(new CancelRxDialogArgs(action, this.CancelRxItems.filter(item => item.IsSelected)));
            break;
        }
        default:
        {
            this.SelectionComplete.emit(new CancelRxDialogArgs(action, this.CancelRxItems.filter(item => item.IsSelected)));
            break;
        }
        }
    }

    public PrepareToLaunch(actionType: string) {
        this.AllSelected = false;
        this.currentAction = actionType;
        switch (actionType) {
            case "Complete": { this.currentAction = "Completed"; break; }
            case "Discontinue": { this.currentAction = "Discontinued"; break; }
            case "EIE": { this.currentAction = "Entered In Error"; break; }
            default: { this.currentAction = actionType; break; }
        }
    }

    IsAnyMedSelected() {
        if (this.CancelRxItems === undefined || this.CancelRxItems === null)
            return false;

        return this.CancelRxItems.filter(item => item.IsSelected).length > 0;
    }   
}
