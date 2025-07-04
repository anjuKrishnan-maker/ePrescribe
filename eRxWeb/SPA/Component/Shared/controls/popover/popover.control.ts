import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { EventService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID } from '../../../../tools/constants';

@Component({
    selector: 'erx-popover-control',
    templateUrl: './popover.template.html',
    styleUrls: ['./popover.style.css']
})

export class PopoverControl {
    popoverVisible: boolean = false;
    @Input() title: string;
    @Input() PopoverId: string;
    constructor(private evSvc: EventService, private cdRef: ChangeDetectorRef) {
        this.evSvc.subscribeEvent(EVENT_COMPONENT_ID.ClosePopover, () => {
            this.ClosePopOver();
        });
    
    }

    OpenPopOver() {
        this.popoverVisible = true;
    }

    ClosePopOver() {
        this.popoverVisible = false;
    }
}