import { Component, ElementRef, OnInit } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { EventService, PptPlusService, ComponentCommonService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, PptDetailContext } from '../../../../tools/constants';
import { PPTPlusScriptPadSummaryModel } from '../../../../model/model.import.def';

@Component({
    selector: 'erx-ppt-plus-script-pad-summary',
    templateUrl: './ppt-plus-script-pad-summary.template.html'
})

export class PPTPlusScriptPadSummary extends BaseComponent implements OnInit {
    public pptPlusScriptPadSummaryModel : PPTPlusScriptPadSummaryModel;
    public ShowNoDetailsOverlay: boolean = false;    
    constructor(private cd: ElementRef,
        private svE: EventService,
        private svPptPlus: PptPlusService,
        private componentService: ComponentCommonService) {
        super(cd, svE);

        this.pptPlusScriptPadSummaryModel = new PPTPlusScriptPadSummaryModel();
        this.pptPlusScriptPadSummaryModel.SummaryItems = new Array<string>();

        this.componentService.AddWindowFunction('ShowAllScriptPadMedPPTSummaryUi', () => {
            this.Hide();
            if (current_context.UserPreference.IsPPTPlusEnabled) {
                this.RetrieveAllScriptPadMedSummaryUi();
            }
        });
    }

    ngOnInit() {
        this.Hide();
    }

    RetrieveAllScriptPadMedSummaryUi() {
        if (this.svPptPlus == undefined) return;
        this.svPptPlus.RetrieveAllScriptPadMedSummaryUi().subscribe((response) => {
                this.ShowNoDetailsOverlay = false;
                this.pptPlusScriptPadSummaryModel.SummaryItems = response;
                this.Show();
                this.AutoShowDetailScreen();
            }
        );
    }
    AutoShowDetailScreen() {
        
        this.svPptPlus.IsAutoShowDetailScreen().subscribe(
            tl => {        
                if (tl === true) {
                    this.OpenDetails();
                }
            }
        );
    }
    OpenDetails() {
        
        this.svPptPlus.ShouldShowDetail().subscribe(
            tl => {        
                if (tl === true) {
                    this.svE.invokeEvent(EVENT_COMPONENT_ID.PptPlusDetailsUpdate  , PptDetailContext.ScriptPadMed);
                }
                else {
                    this.ShowNoDetailsOverlay = true;
                }
            }
        );
    }


    CloseNoDetails() {
        this.ShowNoDetailsOverlay = false;
    }


   
}
