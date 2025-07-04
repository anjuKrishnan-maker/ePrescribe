import { Component, OnInit, Inject } from '@angular/core';
import { LicenseInfo, StateInfo } from '../license-creation/license-creation.model'
import { LicenseCreateService } from '../../service/license-create.service';
import { UserCreateService } from '../../service/user-create.service';
import { filter } from 'rxjs/operators';
import { LoaderService } from '../../service/loader.service';


@Component({
    selector: 'app-license-creation',
    templateUrl: './license-creation.component.html',
    styleUrls: ['./license-creation.component.css']
})
export class LicenseCreationComponent implements OnInit {
    constructor(private licenseCreateService: LicenseCreateService,
        private loaderService: LoaderService,
        private userCreateService: UserCreateService,
        @Inject('window') private window: any) { }
    licenseInfo: LicenseInfo = new LicenseInfo();
    stateList: StateInfo[];
    phoneFaxPattern = "\\(?\\d{3}\\)?-? *\\d{3}-? *-?\\d{4}";
    cityPattern = "^([a-zA-Z]+[\\s-'.]{0,20})*";
    zipCodePattern = "^(\\d{5})(?:\\d{4})?$";
    getStateList() {
        this.userCreateService.getInitialPageData().subscribe(stateList => {
            if (stateList.States != null && stateList.States != undefined) {
                this.stateList = stateList.States as StateInfo[];
            }
        });
    }

    ngOnInit() {
        this.getStateList();
    }

    phoneFaxFormatting(phoneFaxNumber: string) {
        return phoneFaxNumber.replace(" ", "").replace("(", "").replace(")", "").replace("-", "");
    }

    onSubmit(formData: any) {
        if (!formData.valid) {
            Object.keys(formData.controls).forEach(field => {
                const control = formData.controls[field];
                control.markAsTouched({ onlySelf: true });
            })
        }
        else {
            this.licenseInfo.PhoneNumber = this.phoneFaxFormatting(this.licenseInfo.PhoneNumber);
            this.licenseInfo.FaxNumber = this.phoneFaxFormatting(this.licenseInfo.FaxNumber);
            this.licenseCreateService
                .updateRegistrantPracticeDetails(this.licenseInfo)
                .pipe(filter((result) => !!result))
                .subscribe((response) => {                
                    this.loaderService.show(true);
                    this.window.open(this.window?.appcontext?.mediator, "_self");
                });
        }
    }

}
