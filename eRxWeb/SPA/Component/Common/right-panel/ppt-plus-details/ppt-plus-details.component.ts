import { Component, ChangeDetectorRef, ElementRef, OnInit } from '@angular/core';
import { EventService, PptPlusService, ContentLoadService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, PAGE_NAME, NAVIGATION_EVENT_ID, PptDetailContext, PPTPlusDetailsUserChangeStatus, ROUTE_NAME } from '../../../../tools/constants';
import { BaseComponent } from '../../../common/base.component';
import { SelectMedicationNavigationDTO } from '../../../../model//model.import.def';
import { Router } from '@angular/router';


@Component({
    selector: 'erx-ppt-plus-details',
    templateUrl: './ppt-plus-details.template.html'
})

export class PPTPlusDetails extends BaseComponent implements OnInit{
    public fhir: string[];
    public frameSrc: string;
    public originUrl: string = "AnImpossibleOrigin2A1X";
    public pageContext: PptDetailContext;
    public showDetailsUserMessages: boolean = false;
    public detailsUserMessage: string = "";
    public showDetailsIFrame: boolean = false;
    

    constructor(private svE: EventService, private svPptPlus: PptPlusService, cd: ElementRef
        , private cdRef: ChangeDetectorRef, private router:Router, private contentLoadService:ContentLoadService) {
        super(cd, svE);
   
    

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.PptPlusDetailsUpdate  , (pptDetailContext: PptDetailContext) => {
           this.setToDefault();
           if (pptDetailContext != null) {
               this.pageContext = pptDetailContext;
           }
           this.frameSrc = "PptDetails.aspx?" + Math.random();
           this.showModal();
           this.showDetailsIFrame = true;
           this.cdRef.detectChanges();
       });
    }

  

    ngOnInit() {             

        if (this.contentLoadService.initalContentPayload.CommonUiInitialPayload !== null) {
            this.originUrl = this.contentLoadService.initalContentPayload.CommonUiInitialPayload.PptUrl;
            this.svE.subscribeEvent(this.originUrl, (event) => { this.receiveMessage(event) });
        }
    }
    
    setToDefault() {
        this.frameSrc = null;
        this.showDetailsIFrame = false;
        this.showDetailsUserMessages = false;
        this.detailsUserMessage = "";
        this.pageContext = PptDetailContext.Unknown;
        this.cdRef.detectChanges();
    }

    showModal() {
        $("#ppt-details-modal").modal();
    }

    hideModal() {
        $("#ppt-details-modal").modal('hide');
    }

    showSpecialMessage(message) {
        this.detailsUserMessage = message;
        this.showDetailsUserMessages = true;
    }

    receiveMessage(event) {        
        if (event.data === "cancel") {
            this.hideModal();
        }
        else {
            try {
               
                let commonUIEvent = JSON.parse(event.data);
                this.svPptPlus.PPTPlusDetailsUserChanges(commonUIEvent, this.pageContext).subscribe((response) => {
                    if (response.Status !== PPTPlusDetailsUserChangeStatus.Fail) {
                        this.hideModal();
                        if (response.Status === PPTPlusDetailsUserChangeStatus.PharmacyChangeOnly) {
                            if (response.PageContext === PptDetailContext.ScriptPadMed) {
                                this.router.navigateByUrl(ROUTE_NAME.Scriptpad, { state: { navigateTo: PAGE_NAME.ScriptPad + '?v=' + Math.random() + "&from=" + PAGE_NAME.ScriptPad } });                                
                            }
                            else {                                
                                this.svE.get(EVENT_COMPONENT_ID.PptPlusSummaryUpdate).emit({
                                    MedSearchIndexes: response.MedSearchIndexes
                                });                                
                            }
                        }
                        else if (response.Status === PPTPlusDetailsUserChangeStatus.MedChange) {
                            this.router.navigateByUrl(ROUTE_NAME.SelectMedication, {
                                state: {
                                    searchText: { SearchText: response.DrugName } as SelectMedicationNavigationDTO
                                }
                            });
                        }
                        else if (response.Status === PPTPlusDetailsUserChangeStatus.PriorAuthChangeOnly) {
                            if (response.PageContext === PptDetailContext.ScriptPadMed) {
                                this.router.navigateByUrl(ROUTE_NAME.Scriptpad, { state: { navigateTo: PAGE_NAME.ScriptPad + '?v=' + Math.random() + "&from=" + PAGE_NAME.ScriptPad } });
                            }                           
                        }                       
                    }                   
                });
            }
            catch (err) {               
            }
        }
    }   
   
}