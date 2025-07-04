import { Component, Input } from '@angular/core';
import { EVENT_COMPONENT_ID, current_context, PAGE_NAME, ROUTE_NAME } from '../../../../tools/constants';
import { FormsModule, NgForm } from '@angular/forms';
import { EventService, SelectPatientService, PatientService } from '../../../../services/service.import.def';
import { DelegateProvider, SupervisingProviderModel, SupervisorProviderInfoRequest, SupervisorProviderInfoResponse } from '../../../../model/model.import.def'
import { Router } from '@angular/router';

@Component({
    selector: 'supervising-provider',
    templateUrl: './supervising-provider.template.html',
    styles: [`
        .overlayFooter{
                padding-top: 1px!important;
                padding-right: 15px!important;
                padding-bottom: 1px!important;
                padding-left: 1px!important;
               
        }
 .form-group{margin-bottom: 0px !important;}
.modal {
  text-align: center;
  padding: 0!important;
}

.modal:before {
  content: '';
  display: inline-block;
  height: 100%;
  vertical-align: middle;
  margin-right: -4px;
}

.modal-dialog {
  display: inline-block;
  text-align: left;
  vertical-align: middle;
}
`]
})

export class SupervisingProviderComponent {
    @Input() providers: Array<DelegateProvider>;

    public currentProvId: string;

    constructor(private selectPatientSvc: SelectPatientService, private ptSvc: PatientService, private evtSvc: EventService, private router : Router) {
        this.currentProvId = '';
        this.providers = [];
    }

    btnSetSupervisingProviderClick() {
        var supervisorProviderInfoRequest = new SupervisorProviderInfoRequest();
        supervisorProviderInfoRequest.SupervisorProviderId = this.currentProvId;

        this.selectPatientSvc.SetSupervisingProviderInfo(supervisorProviderInfoRequest).subscribe((response) => {
            if (response.IsSupervisorProviderInfoSet) {
                $("#mdlSupervisingProvider").modal('toggle');

                // todo: this navigation should be done on the parent
                current_context.PageName = PAGE_NAME.SelectMedication;
                this.ptSvc.PopupNavigation.ContentSrc = '';
                this.router.navigateByUrl(ROUTE_NAME.SelectMedication);
            } else {
                //Send Message
            }
        });
    }

    OpenModal() {
        this.currentProvId = this.providers[0].ProviderId;

        $("#mdlSupervisingProvider").modal({ backdrop: 'static', keyboard: false });
        $("#mdlSupervisingProvider").modal('show');
    }

    Cancel() {
        $("#mdlSupervisingProvider").modal('toggle');
    }
}
