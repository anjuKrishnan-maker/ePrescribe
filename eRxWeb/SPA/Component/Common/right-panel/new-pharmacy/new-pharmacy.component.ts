import { Component, ElementRef, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BaseComponent } from '../../base.component';
import { PharmacyService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { NewPharmacyModel, RightPanelPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';

@Component({
    selector: 'erx-new-pharmacy',
    templateUrl: './new-pharmacy.template.html',
    providers: [PharmacyService],
    styles: [`
        label{width:111px !important;font-weight:100;}
        .form-control{ width:45% !important;display:inline}
        .form-group{margin-bottom: 0px !important;}
        .response-info{display:table;text-align:center;}
        .response-info span{font-weight:bold;}
`]
})
export class NewPharmacyComponent extends BaseComponent implements OnInit {
    submitted: boolean = false;
    isValid: boolean = false;
    Response: string = '';
    pharmaModel: NewPharmacyModel = new NewPharmacyModel();
    constructor(private pharmSvc: PharmacyService, cd: ElementRef, private svE: EventService, private compSvc: ComponentCommonService) {
        super(null, svE);         
        this.pharmaModel.State = 'IL'; 

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        });
    }


    validate(d: NgForm) {
        this.isValid = d.valid;
        return;
    }
    onSubmit() {
        this.submitted = true;
        this.pharmaModel.Comment = typeof this.pharmaModel.Comment === 'undefined' ? '' : this.pharmaModel.Comment;
        this.pharmaModel.Fax = typeof this.pharmaModel.Fax === 'undefined' ? '' : this.pharmaModel.Fax;
        this.pharmaModel.Address2 = typeof this.pharmaModel.Address2 === 'undefined' ? '' : this.pharmaModel.Address2;
        if (this.isValid)
            this.pharmSvc.RequestAddPharmacy({ Name: this.pharmaModel.Name, Address1: this.pharmaModel.Address1,
                Address2: this.pharmaModel.Address2, City: this.pharmaModel.City, State: this.pharmaModel.State,
                ZipCode: this.pharmaModel.ZipCode, Phone: this.pharmaModel.Phone, Fax: this.pharmaModel.Fax, Comment: this.pharmaModel.Comment})
                .subscribe(response => {
                    this.pharmaModel = new NewPharmacyModel();
                    this.submitted = false;
                    this.Response = response;
                });

        
    }

    ngOnInit() {
        this.Response = '';
    }

}
