import { Component, Input, OnInit } from '@angular/core';
import { HelpContentService } from '../../../../services/service.import.def';
import { EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { BaseComponent } from '../../base.component';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { RightPanelPayload } from '../../../../model/model.import.def';

@Component({
    selector: 'erx-help-panel',
    templateUrl: './help.template.html'
})
export class HelpComponent extends BaseComponent implements OnInit {
    hlpContent: string = '';
    isCollapsed: boolean;
    constructor(private svE: EventService, private compSvc: ComponentCommonService) {
        super(null, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
            this.isCollapsed = true;
            this.Collapse();
        });


        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.hlpContent = rightPanelPayload.HelpContentPayload;
                if (this.hlpContent != undefined && this.hlpContent.length > 0) {
                    this.Show();
                }
            }
        });        
    }    

    ngOnInit() {
        this.hlpContent = '';
    }
}
