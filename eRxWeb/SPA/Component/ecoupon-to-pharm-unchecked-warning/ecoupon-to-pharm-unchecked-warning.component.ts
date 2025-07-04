import { Component, AfterViewInit } from '@angular/core';
import { ComponentCommonService, EcouponToPharmUncheckedService } from '../../services/service.import.def';

@Component({
    selector: 'ecoupon-to-pharm-unchecked-warning',
    templateUrl: './ecoupon-to-pharm-unchecked-warning.template.html'
})

export class PatientProgramUcheckedOverlayComponent implements AfterViewInit {
    private mdlEl: any;
    private isLoaded: Boolean = false;
    private checkboxId: string;
    public neverShowMessageAgain: boolean;

    constructor(private compSvc: ComponentCommonService, private apiSvc: EcouponToPharmUncheckedService) {
        compSvc.AddWindowFunction("showEcouponUncheckedWarning", (checkboxId) => {
            this.checkboxId = checkboxId;
            this.mdlEl.modal('show');
        });
    }

    ngAfterViewInit() {
        this.mdlEl = $('#patProgramUnchecked').modal('hide');
        this.mdlEl.on('hidden.bs.modal', () => {});
    }

    public ContinueWithoutCoupon() {
        //document is found differently in Microsoft browsers vs rest of the world.
        if (window.frames['content-frame'].contentWindow != null) {
            window.frames['content-frame'].contentWindow.document.getElementById(this.checkboxId).checked = false;
        } else {
            window.frames['content-frame'].document.getElementById(this.checkboxId).checked = false;
        }
        
        if (this.neverShowMessageAgain) {
            this.apiSvc.SaveNeverShowPreference().subscribe(() => {});
        }
        this.mdlEl = $('#patProgramUnchecked').modal('hide');
    }

    public IncludeCoupon() {
        if (this.neverShowMessageAgain) {
            this.apiSvc.SaveNeverShowPreference().subscribe(() => { });
        }
        $('#patProgramUnchecked').modal('hide');
    }
}