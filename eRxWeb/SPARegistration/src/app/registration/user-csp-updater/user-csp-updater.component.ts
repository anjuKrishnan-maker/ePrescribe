import { Component, OnInit } from '@angular/core';
import { RegistrationService } from '../../service/registration.service';
import { RegistrantContext } from '../can-load-registration.guard';

@Component({
    selector: 'user-csp-updater',
    templateUrl: './user-csp-updater.template.html',
})

export class UserCspUpdaterComponent implements OnInit {
    constructor(private registrationService:RegistrationService) {

    }
    ngOnInit() {
        this.registrationService.updateUserCsp().subscribe((registrantContext: RegistrantContext) => {
            window.location.href = registrantContext.RedirectUrl;
        })
    }
}