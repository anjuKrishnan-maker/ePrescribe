import { Component, OnInit, Input } from "@angular/core";
import { UserPreferencesModel } from "../../../model/user/user.model";
import { Router } from "@angular/router";
import { ROUTE_NAME, PAGE_NAME } from "../../../tools/constants";

@Component({
    selector: 'erx-user-preferences',
    templateUrl: './user-preferences.template.html',
    styleUrls: ['./../user.style.css', './user-preferences.style.css']
})
export class UserPreferencesComponent implements OnInit {
    @Input() userPreferences: UserPreferencesModel;
    @Input() redirectionParam: string;
    constructor(private router: Router) {

    }
    ngOnInit() {

    }

    navigateToRxFavourites() {
        if (!this.redirectionParam) {
            this.redirectionParam = '';
        }
        this.router.navigateByUrl(ROUTE_NAME.RxFavourites, { state: { navigateTo: PAGE_NAME.RxFavourites + this.redirectionParam } });
    }

    updateRtbChildCheckBoxCheckState() {
        this.userPreferences.AutoTherapeuticAlternativesPreference = this.userPreferences.RTBPreference;
        this.userPreferences.RTPSPreference = this.userPreferences.RTBPreference;
    }

    updateUserPreferencePPTChildCheckState() {
        this.userPreferences.PPTOffersPreference = this.userPreferences.PPTPreference;
    }
}