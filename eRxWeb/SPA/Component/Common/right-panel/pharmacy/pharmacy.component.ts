import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { PharmacyService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { PharmacyModel, PharmacyRequestModel, RightPanelPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';

@Component({
    selector: 'erx-pharmacy-info',
    templateUrl: './pharmacy.template.html',
    providers: [PharmacyService]
})
export class PharmacyComponent extends BaseComponent implements OnInit {
    pharmaModel: PharmacyModel;

    constructor(private pharmSvc: PharmacyService, private cd: ChangeDetectorRef, private svE: EventService, private compSvc: ComponentCommonService) {
        super(null, svE);  


        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {  
            this.pharmaModel = null;
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {                        
                        this.Show();                    
                }
                else {
                    this.Hide();
                }            
        });       

    }

    ngOnInit() {
        this.pharmaModel = null;
    }  

    LoadPharmacy(pharmacyRequestModel: PharmacyRequestModel) {
        this.pharmSvc.GetPharmacy(pharmacyRequestModel).subscribe(response => {
            this.pharmaModel = response;
            this.Show();
            this.cd.detectChanges();
        });
    }            
}
