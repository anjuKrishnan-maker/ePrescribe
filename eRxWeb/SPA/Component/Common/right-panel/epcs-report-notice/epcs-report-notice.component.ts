import {Component, ElementRef,Input,NgModule } from '@angular/core';
import {BaseComponent} from '../../base.component';

import { EventService, ComponentCommonService, EPCSService } from '../../../../services/service.import.def';
import { EPCSNotice, RightPanelPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, current_context, PAGE_NAME, ROUTE_NAME } from '../../../../tools/constants';
import { Router } from '@angular/router';
@Component({
    selector: 'erx-epcs-notice',
    templateUrl: './epcs-report-notice.template.html'
}) 
export class EPCSNoticeReportComponent extends BaseComponent  {
    @Input() src: string;
    EPCSNotice: EPCSNotice;
    ShowCSReportNotice: string;
    constructor(epcsService: EPCSService, cd: ElementRef, private svE: EventService, private compSvc: ComponentCommonService, private router: Router) {
        super(cd, svE);
        
        this.EPCSNotice = new EPCSNotice();

//        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
//            this.Hide();
//        });
        
        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
                if (compSvc.CheckVisibility(this.id, current_context.PageName)) {
                    this.EPCSNotice = rightPanelPayload.EPCSNoticePayload;
                    if (this.EPCSNotice != undefined) {
                        if (this.EPCSNotice.Count == undefined) {
                            this.EPCSNotice.Count = "0";
                            this.ShowCSReportNotice = null;
                            this.Hide();
                        }
                        if (this.EPCSNotice.Count != "0") {
                            this.ShowCSReportNotice = "CS reports available (" + this.EPCSNotice.Count + ")";
                            this.Show();
                        }
                    }
                }
        });


    }
    RedirectToEPCSDailyActivityReport() {
        this.router.navigateByUrl(ROUTE_NAME.EPCSDailyReport, { state: { navigateTo: PAGE_NAME.EpcsDailyActivityReport } });
    }  
  
}
