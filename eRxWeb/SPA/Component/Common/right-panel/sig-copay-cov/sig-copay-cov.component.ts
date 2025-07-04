import { Component, ElementRef, ViewChild, NgModule } from '@angular/core';

import { BaseComponent } from '../../base.component';
import { CopayCoverageComponent } from '../right-panel.component.import.def';
import { RightPanelPayload } from '../../../../model/model.import.def';

import { FormularyAlternativeService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
@Component({
    selector: 'erx-sig-copay',
    templateUrl: './sig-copay-cov.template.html'

})
export class SigCopayCoverageComponent extends BaseComponent {
    @ViewChild(CopayCoverageComponent) copayCoverageComponent: CopayCoverageComponent;

    FormularyAlternative: any;
    constructor(private formularyAlternativeService: FormularyAlternativeService, cd: ElementRef,
        private svE: EventService, private svC: ComponentCommonService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.StartLoading();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {           
            if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
                this.FormularyAlternative = rightPanelPayload.SigCopayPayload;
                this.Show();
                this.EndLoading();
            }
            else {
                this.Hide();
            }
        });
    }
}

