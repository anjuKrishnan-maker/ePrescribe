import { Component, Input, Inject } from "@angular/core";
import { Router } from '@angular/router';
import { LinkExistingUserModel, ErrorReason } from '../link-account-request-model'
import { LinkExistingAccount } from '../../activate-user/link-existing-account/link-existing-account-component'
import { Workflow } from '../link-account-request-model'
import { LoaderService } from '../../../service/loader.service';

@Component({
    selector: 'link-account-confirmation-popup',
    templateUrl: './link-account-confirmation-popup-template.html',
    styleUrls: ['./link-account-confirmation-popup.style.css']
})
export class LinkAccountConfirmationPopupComponent  {
    linkExistingUserResponse: LinkExistingUserModel = new LinkExistingUserModel();
    linkExistingAccount: LinkExistingAccount;
   showLinkedAccountConfPopup: boolean=false;
    showLinkedAccountConfLayout: boolean;
    workFlowType: Workflow;
    showMessage: boolean;
    linkConfMessageText: string;
    linkConfImageUrl: string ;                
    imageIcon: string;
    constructor(private router: Router,
        private loaderService: LoaderService,
        @Inject('window') private window: any) {
    }
    
    showLinkedAccountConfirmation() {
        if (this.linkExistingUserResponse.IsValid) {            
            if (this.workFlowType == Workflow.Utilities) {
                this.showMessage = true;
                this.linkConfMessageText = "User successfully linked. You will now be directed to Activation home page.";
                this.setImageIcon("SUCCESS");    
            }           
            this.showLinkedAccountConfPopup = true;
            this.showLinkedAccountConfLayout = true;
        }
        else {
            this.displayErrorMessage(this.linkExistingUserResponse);
        }
    }
    goNext() {
        this.loaderService.show(true);
        if (this.workFlowType == Workflow.Utilities) {
            this.window.open(this.window?.appcontext?.logout + "?msg=Utlities user successfully activated", "_self");
        }
        else if (this.workFlowType == Workflow.Sso) {
            this.window.open(this.window?.appcontext?.logout + "?SSOActivationSuccess=true", "_self");
        }
        else
        {
            this.window.open(this.window?.appcontext?.twoNUserMediator, "_self");
        }
    }
    setImageIcon(value: string) {
        switch (value.toUpperCase()) {
            case "SUCCESS": 
                this.linkConfImageUrl = "../images/info-global-16-x-16.png";
                this.imageIcon = "success-dialog";
                break;          
            case "INFORMATION":
                this.linkConfImageUrl = "../images/warning-global-16-x-16.png";
                this.imageIcon = "information-dialog";
                break;
            default:
                this.linkConfImageUrl = "../images/info-global-16-x-16.png";
                this.imageIcon = "success-dialog";
                break;
        }
    }
    displayErrorMessage(linkResponse: LinkExistingUserModel) {
        switch (linkResponse.ErrorReason) {          
            case ErrorReason.ERX_UPDATE_FAILURE:
                {
                    this.showMessage = true;
                    this.linkConfMessageText = "Your existing Veradigm Security Account will successfully be linked to ePrescribe account upon login.";
                    this.setImageIcon("INFORMATION");  
                    this.showLinkedAccountConfPopup = true;
                    this.showLinkedAccountConfLayout = true;
                    break;
                }
            default:
                {
                    this.showMessage = true;                    
                    this.showLinkedAccountConfPopup = false;
                    this.showLinkedAccountConfLayout = false;
                    break;
                }
            
        }
    }
}