import { Component, OnInit } from '@angular/core';
import { Router, NavigationExtras } from '@angular/router';
import { WelcomeService } from '../../service/welcome-service'
import { RegistrantContext } from '../can-load-registration.guard';

@Component({
    selector: 'welcome-page',
    templateUrl: './welcome-page.component.html',
    styleUrls: ['./welcome-page.component.css']
})

export class WelcomePageComponent {
    public title: string = "ePrescribe";

    constructor(private welcomeService: WelcomeService) {
        let appContext = (window as any).appcontext;
        this.title = appContext.appName;
    }    

    navigateToCSP() {
        this.welcomeService
            .logRegistrantUserNavigationToCSP()
            .subscribe((registrantContext: RegistrantContext) => {
                window.location.href = registrantContext.CspUrl;
            });
    }
}