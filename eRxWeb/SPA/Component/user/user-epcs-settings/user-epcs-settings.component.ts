import { Component, OnInit, Input } from "@angular/core";
import { UserModel, UserCategory } from "../../../model/user/user.model";
import { UserEPCSSettingModel } from '../../../model/epcs-settings.model';
import { ROUTE_NAME, PAGE_NAME } from "../../../tools/constants";
import { Router } from "@angular/router";

@Component({
    selector: 'erx-user-epcs-settings',
    templateUrl: './user-epcs-settings.template.html',
    styleUrls: ['./user-epcs-settings.style.css']
})
export class UserEpcsSettingsComponent implements OnInit {
    user: UserModel;    
    @Input() userEpcsSettings: UserEPCSSettingModel;  
    @Input() redirectionParam: string;
    showDefinitionsPopup: boolean;
    showAlertsPopup: boolean;
    constructor(private router: Router) {

    }
    ngOnInit() {
                    
    }

    

    openDefinitionsPopup() {
        this.showDefinitionsPopup = true; 
    }

    closeDefinitionsPopup() {
        this.showDefinitionsPopup = false;
    }

    manageEpcsApprovals() {
        if (!this.redirectionParam) {
            this.redirectionParam = '';
        }
        this.router.navigateByUrl(ROUTE_NAME.ManageAccount, { state: { navigateTo: PAGE_NAME.EpcsRegManageMode + "&From=EditUser" + this.redirectionParam.replace('?','&') } });
    }

    manageSecondFactorForms() {
        this.showAlertsPopup = true;
    }

    closeAlertPopup() {
        this.showAlertsPopup = false;
    }

    proceedSecondFactorForms() {
        window.open(this.userEpcsSettings.SecondFactorFormUrl,'_blank');
        this.showAlertsPopup = false;
    }

    redirectToiLearn() {        
        window.open(this.userEpcsSettings.SecondFactorHelpUrl, '_blank');
    }
}