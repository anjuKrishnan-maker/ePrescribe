import { Component, Output, EventEmitter } from '@angular/core';
import {  ElementRef, ChangeDetectorRef, ViewChild } from '@angular/core';
import { EventService, ComponentCommonService, PDMPService } from '../../services/service.import.def';
import { BaseComponent } from '../common/base.component';
import { ModalPopupControl } from '../shared/controls/modal-popup/modal-popup.control';
@Component({
    selector: 'erx-pdmp-epcs-teaser',
    templateUrl: './pdmp-epcs-teaser.template.html',
    styleUrls: ['./pdmp-epcs-teaser.style.css']
})
export class pdmpEpcsTeaserComponent extends BaseComponent {    
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    @Output() closeModalEvent = new EventEmitter<boolean>();
    constructor(private comonSvc: ComponentCommonService, private svE: EventService, cd: ElementRef, private cdRef: ChangeDetectorRef, private svPdmp: PDMPService
    ) {
        super(cd, svE);
    }
    
    onCloseModal(event: any) {        
        this.modalPopup.ClosePopup();
        this.EndLoading();
        this.closeModalEvent.emit(false);
    }
    Close() {
        this.modalPopup.ClosePopup();
        this.EndLoading();
    }
    
    ShowPdmpEnrollmentForm() {  
        this.modalPopup.OpenPopup();
        this.StartLoading();
    }
}




