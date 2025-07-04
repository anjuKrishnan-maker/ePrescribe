import { Component, OnInit, ChangeDetectorRef, ElementRef, Input } from '@angular/core';
import { EventService, PDMPService, ContentLoadService } from '../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, PAGE_NAME, COMPONENT_NAME, NAVIGATION_EVENT_ID, PptDetailContext } from '../../../tools/constants';
import { BaseComponent } from '../../common/base.component';
import { ModalPopupControl } from '../controls/modal-popup/modal-popup.control';
import { FunctionDetail, InitalContentPayload } from "../../../model/model.import.def";

@Component({
    selector: 'erx-pdmp-details',
    templateUrl: './pdmp-details.template.html',
    styleUrls:['./pdmp-details.style.css']
})

export class PDMPDetailsComponent extends BaseComponent implements OnInit {    
    public frameSrc: string;
    public originUrl: string = "AnImpossibleOrigin2A1X";
    public pageContext: PptDetailContext;
    public showDetailsUserMessages: boolean = false;
    public detailsUserMessage: string = "";
    public showDetailsIFrame: boolean = false;   

    constructor(private svE: EventService, cd: ElementRef
        , private cdRef: ChangeDetectorRef, private svPdmp: PDMPService, private contentLoadService:ContentLoadService) {
        super(cd, svE);
      
    }

    ngOnInit() {
        if (this.contentLoadService.initalContentPayload.CommonUiInitialPayload !== null) {
            this.originUrl = this.contentLoadService.initalContentPayload.CommonUiInitialPayload.PdmpUrl;
            this.svE.subscribeEvent(this.originUrl, (event) => { this.receiveMessage(event) });
        }
    }

    loadPDMPDetails() {
        this.setToDefault();
        this.frameSrc = "PdmpDetails.aspx?" + Math.random();
        this.showModal();
        this.showDetailsIFrame = true;
        this.cdRef.detectChanges();
    }

    setToDefault() {
        this.frameSrc = null;
        this.showDetailsIFrame = false;
        this.showDetailsUserMessages = false;
        this.detailsUserMessage = "";     
        this.cdRef.detectChanges();
    }

    showModal() {
        $("#pdmp-details-modal").modal();
    }

    hideModal() {
        $("#pdmp-details-modal").modal('hide');
    }


    receiveMessage(event) {
        this.svPdmp.PDMPUserChanges(event.data).subscribe(() => {
            if (event.data === "Pdmp-Cancelled") {
                this.hideModal();
            }
            else if (event.data === "Pdmp-Confirmed") {
                this.hideModal();
                let fnDetail = new FunctionDetail(this, "PDMPCheckRegistry", []);
                this.svE.invokeEvent(EVENT_COMPONENT_ID.CallFrameContentJavaScript, fnDetail);
            }
        });
    }
}