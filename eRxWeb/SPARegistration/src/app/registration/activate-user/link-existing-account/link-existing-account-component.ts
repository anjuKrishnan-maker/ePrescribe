import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ActivationCodeService } from '../../../service/activation-code.service'
import { LinkAccountRequestModel, LinkExistingUserModel, ErrorReason } from '../link-account-request-model'
import { LinkAccountConfirmationPopupComponent } from '../../activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component'
import { UserExistingAccountComponent } from '../../user-existing-account/user-existing-account.component';
import { Subject } from 'rxjs'
import { ExistingUserAccount } from '../../user-existing-account/user-existing-account.model';
@Component({
    selector: 'link-existing-account-template',
    templateUrl: './link-existing-account-template.html',
    styleUrls: ['./link-existing-account.style.css']
})
export class LinkExistingAccount implements OnInit {
    isValid: boolean;
    @ViewChild('linkedAccountConfirmPopup') linkedAccountConfirmPopup: LinkAccountConfirmationPopupComponent;
    @ViewChild('userExisting') userExisting: UserExistingAccountComponent;
    IsLinkedUser: boolean;
 
    linkAccountModel: LinkAccountRequestModel = new LinkAccountRequestModel();
    isExistingAccountValid: boolean = false;
    linkMessageText: string;
    showLinkMessage: boolean;
    linkImageUrl: string;
    linkImageIcon: string ;
    constructor(private router: Router, private activationService: ActivationCodeService) {
    }

    ngOnInit() {     
    }
   
    createUser() {
        this.router.navigate(["register/createAccount"]);
    }
    linkUserToExistingAccount() {
        this.getExistingAccountDetails();
        if (this.isExistingAccountValid) {
            this.activationService.linkToExistingUser(this.linkAccountModel).subscribe((linkShieldResponse: LinkExistingUserModel) => {
                this.linkedAccountConfirmPopup.linkExistingUserResponse = linkShieldResponse;
                if (this.linkedAccountConfirmPopup.linkExistingUserResponse.IsValid) {
                    this.IsLinkedUser = false;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfLayout = true;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfPopup = true;
                    this.linkedAccountConfirmPopup.workFlowType = linkShieldResponse.WorkflowType
                    this.linkedAccountConfirmPopup.showLinkedAccountConfirmation();
                }
                else {

                    this.DisplayErrorMessage(this.linkedAccountConfirmPopup.linkExistingUserResponse);
                }
            });
        }
    } 

    setMessageImageIcon(value: string) {
        switch (value.toUpperCase()) {
            case "SUCCESS":
                this.linkImageUrl = "../images/info-global-16-x-16.png";
                this.linkImageIcon = "success-dialog";
                break;
            case "INFORMATION":
                this.linkImageUrl = "../images/warning-global-16-x-16.png";
                this.linkImageIcon = "information-dialog";
                break;
            case "ERROR":
                this.linkImageUrl = "../images/alert-global-16-x-16.png";
                this.linkImageIcon = "error-dialog";
                break;
            default:
                this.linkImageUrl = "../images/info-global-16-x-16.png";
                this.linkImageIcon = "success-dialog";
                break;
        }
    }
    DisplayErrorMessage(linkResponse: LinkExistingUserModel) {
        switch (linkResponse.ErrorReason) {
            case ErrorReason.NOT_AUTHENTICATED:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Incorrect Login ID or Password was specified.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case ErrorReason.FAILED_TO_LINK_USER:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Unable to link your Veradigm Security Account to your ePrescribe Account, Please try again.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case ErrorReason.ERX_UPDATE_FAILURE:
                {
                    this.IsLinkedUser = false;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfLayout = true;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfPopup = true;
                    this.linkedAccountConfirmPopup.showLinkedAccountConfirmation();                    
                    return;
                }
            case ErrorReason.FAILED_TO_RETRIEVE_USER_GUID:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Failed to retrieve your eRx information from your Security Account.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case ErrorReason.USER_PROFILE_ALREADY_LINKED_INSIDE_THIS_TENANT:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Your Login ID is already linked to another profile in this license.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
            case ErrorReason.USERNAME_ALREADY_EXISTS_FOR_LICENSE:
                {
                    this.showLinkMessage = true;
                    this.linkMessageText = "Your username already exists within this license.";
                    this.setMessageImageIcon("ERROR");
                    break;
                }
        }
        this.linkedAccountConfirmPopup.showLinkedAccountConfLayout = false;
        this.linkedAccountConfirmPopup.showLinkedAccountConfPopup = false;       
        return true;
    }    


    getExistingAccountDetails() {
        let existingUserAccount: ExistingUserAccount;
        existingUserAccount = this.userExisting.GetExistingAccountDetails();
        this.linkAccountModel.shieldUserName = existingUserAccount.shieldUserName;
        this.linkAccountModel.shieldPassword = existingUserAccount.password;
        this.isExistingAccountValid = existingUserAccount.isValid;
    }
}