import { Component, ViewChild, OnInit, Input } from "@angular/core";
import { Router } from "@angular/router";
import { ROUTE_NAME, PAGE_NAME } from "../../../tools/constants";
import { UserActivationInfoModel, UserCommunicationStatusModel } from "../../../model/user/user.model";
import { UserService } from "../../../services/user.service";


@Component({
    selector: 'erx-new-user-activation-modal-popup',
    templateUrl: './new-user-activation-modal-popup.template.html',
    styleUrls: ['./new-user-activation-modal-popup.style.css']
})
export class NewUserActivationModalPopupComponent implements OnInit {
    userActivationInfo: UserActivationInfoModel;
    newUserCommunicationStatus: UserCommunicationStatusModel;
    activationUrl: string;
    showModalPopup: boolean = false;
    isCommunicationModeSelected: boolean = true;  

    constructor(private userSvc: UserService, private router: Router) {
        
    }



    ngOnInit() {    
        this.userActivationInfo = {} as UserActivationInfoModel;
        this.userActivationInfo.UserCommunicationStatus = <UserCommunicationStatusModel>{
            IsEmailChecked: true,
            IsPrintChecked: true
        };
    }

    openNewUserActivationModalPopup(userActivationInfo: UserActivationInfoModel) {
        this.userActivationInfo = userActivationInfo;
        this.userActivationInfo.UserCommunicationStatus = <UserCommunicationStatusModel>{
            IsEmailChecked: true,
            IsPrintChecked: true
        };
        this.showModalPopup = true; 

    }

    validateEmailorPrintChecked(): boolean {
        this.isCommunicationModeSelected = (this.userActivationInfo.UserCommunicationStatus.IsEmailChecked || this.userActivationInfo.UserCommunicationStatus.IsPrintChecked) ? true : false;                    
        return this.isCommunicationModeSelected;
    }
    
    onSubmit() {         
        if (this.validateEmailorPrintChecked()) {
            this.userSvc.processUserActivation(this.userActivationInfo).subscribe(response => {
                if (this.userActivationInfo.UserCommunicationStatus.IsEmailChecked && !this.userActivationInfo.UserCommunicationStatus.IsPrintChecked)
                    this.router.navigateByUrl(ROUTE_NAME.Settings, { state: { messageModel: JSON.stringify(response) } });
                else                    
                    this.router.navigateByUrl(ROUTE_NAME.PdfInPage, { state: { navigateTo: PAGE_NAME.PDFInPage + "?To=" + PAGE_NAME.Settings  + "&Message=" + response.Message } });
            });            
            this.showModalPopup = false;
        }
    }



}