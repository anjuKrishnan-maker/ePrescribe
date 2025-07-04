import { Component, Input, Output,  EventEmitter } from '@angular/core';
import { EventService, ReviewHistoryService } from '../../../../services/service.import.def';
import { SupervisingProviderModel } from '../../../../model/model.import.def';

@Component({
    selector: 'erx-sup-prov-prompt',
    templateUrl:'./supervising-provider-prompt.template.html'
})
export class SupervisingProviderPrompt
{
    @Output() supervisingProviderSelected = new EventEmitter<any>();
    public providers: SupervisingProviderModel[];
    public currentProvId: string;
    
    constructor(private eventService: EventService, private reviewHistoryService: ReviewHistoryService) {
        
    }
    
    showOverlay()
    {
        $("#mdlPopupSetSupervisorProvider").modal();
    }

    closePopup() {
        $("#mdlPopupSetSupervisorProvider").modal('toggle');
    }

    setSupervisingProvider() {
        this.reviewHistoryService.SetSupervisingProviders(this.currentProvId).subscribe(data => {
            this.supervisingProviderSelected.emit(data);
            this.closePopup();
        });
    }

    retrieveSupervisingProviders() {
        this.reviewHistoryService.GetSupervisingProviders().subscribe((data:SupervisingProviderModel[]) => {
            this.providers = data;
            if(this.providers && this.providers.length > 0)
            this.currentProvId = this.providers[0].ProviderID;
            this.showOverlay();
        });
    }
}