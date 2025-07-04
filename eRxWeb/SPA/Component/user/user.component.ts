import { Component, OnInit, ViewChild } from "@angular/core";
import { UserService } from "../../services/service.import.def";
import { Router } from "@angular/router";
import { UserRequest, UserMode, UserModel, UserCategory, UserActivationInfoModel, UserBackButtonModel } from "../../model/user/user.model";
import 'rxjs/add/operator/filter';
import { UserTypeDefinitionComponent } from "./user-settings/user-type-definition/user-type-definition.component";
import { UserGuidModalComponent } from "./user-settings/user-guid-modal/user-guid-modal.component";
import { MessageModel, MessageIcon } from "../../model/message.model";
import { UserMobileMessageDescriptionComponent } from "./user-mobile-messaging-settings/user-mobile-messaging-description/user-mobile-messaging-description.component";
import { NewUserActivationModalPopupComponent } from "./new-user-activation-modal-popup/new-user-activation-modal-popup.component";
import { ROUTE_NAME, PAGE_NAME } from "../../tools/constants";

@Component({
    selector: 'erx-user',
    templateUrl: './user.template.html',
    styleUrls: ['./user.style.css']
})
export class UserComponent implements OnInit {
    public user: UserModel;
    public userRequest: UserRequest;
    public backButtonModel: UserBackButtonModel;
    public hasDEALicense: boolean;
    public initialUrlQuery: string;
    @ViewChild('NewUserActivationModalPopup') newUserActivationModalPopup: NewUserActivationModalPopupComponent;
    @ViewChild('UserTypeDefinition') UserTypeDefinition: UserTypeDefinitionComponent;
    @ViewChild('UserGuidModal') UserGuidModal: UserGuidModalComponent;
    @ViewChild('UserMobileMessageDescription') UserMobileMessageDescription: UserMobileMessageDescriptionComponent;

    userCategoryEnum = UserCategory;

    public namePattern = "^([a-zA-Z]+[\\s-'.]{0,35})*";
    public isSaveAllowed = true;
    public userInformationValidationStatus = true;
    public userCredentialsValidationStatus = true;
    public userSettingsValidationStatus = true;
    messageStrip: Array<MessageModel> = [];
    userTypes = UserCategory;
    saveUserText = 'Save';

    constructor(private userSvc: UserService, private router: Router) {
        let currentState = this.router.getCurrentNavigation().extras.state;
        this.userRequest = this.createUserRequestModel(currentState);
        this.backButtonModel = this.createBackButtonModel(currentState, this.userRequest.UserMode);
        this.userRequest.IsNoLic = this.backButtonModel.NoLic;
        this.initializeValidations(this.userRequest.UserMode);
        this.UserGuidModal = new UserGuidModalComponent();
    }

    ngOnInit() {        
        this.userSvc.userInit(this.userRequest).subscribe((response) => {
            this.user = response;
            if (this.user?.UserPreferences?.IsFaxSiteInvalid) {
                this.messageStrip = [];
                this.createMessageModelAndPush('Fax site previously associated with your account is Inactive. Please choose different  fax default site.', MessageIcon.Error);
            }
            //display relevant msg to user
            this.displayRedirectionStatusMsg(this.backButtonModel, this.user);
            this.hasDEALicense = (this.user.UserDEALicenses?.DEALicenses == null || this.user.UserDEALicenses?.DEALicenses.length == 0) ? false : true;
        });
    }

    initializeValidations(userMode: UserMode) {
        if (userMode === UserMode.EditOtherUser || userMode === UserMode.SelfEdit) {
            this.isSaveAllowed = true; //Assumption is that, its coming from a previous stable state, so true, unless something modified
            this.userInformationValidationStatus = true;
            this.userCredentialsValidationStatus = true;
            this.userSettingsValidationStatus = true;
        } else if (userMode === UserMode.AddOtherUser) {
            this.isSaveAllowed = false; //Enable dynamically
            this.userInformationValidationStatus = false;
            this.userCredentialsValidationStatus = false;
            this.userSettingsValidationStatus = false;
        }
    }

    //must write unit test
    createBackButtonModel(urlState, userMode: UserMode): UserBackButtonModel {
        let backButtonModel = {} as UserBackButtonModel;

        if (userMode === UserMode.SelfEdit) {
            //Return Page
            if (urlState?.cameFrom) {
                backButtonModel.CameFrom = urlState.cameFrom;
                this.initialUrlQuery = '?CameFrom=' + urlState.cameFrom;
            }
            if (urlState?.status) {
                backButtonModel = this.setBackButtonStatus(urlState.status.toLowerCase(), backButtonModel);
            }

            try {            //Forced Redirection Scenarios
                //ex: "EditUser?Status=NoLic"
                let splitUrl = urlState.navigateTo?.split('?');
                if (splitUrl?.length > 1) {
                    this.initialUrlQuery = '?' + splitUrl[1]; //Retaining query for navigation outside this page

                    let urlParamArray = splitUrl[1]?.split('&');
                    for (let i = 0; i < urlParamArray.length; i++) {
                        let param = urlParamArray[i].split('=');
                        let paramKey = param[0];
                        if (paramKey?.toLowerCase() === 'status') {
                            let statusValue = param[1]?.toLowerCase();
                            backButtonModel = this.setBackButtonStatus(statusValue, backButtonModel);
                        }
                    }
                }
            } catch {
                //Nothing to update in back button model
            }
        }

        return backButtonModel;
    }

    setBackButtonStatus(status:string, backButtonModel: UserBackButtonModel): UserBackButtonModel {
        switch (status) {
            case 'nonpi': backButtonModel.NoNpi = true;
                break;
            case 'expdea': backButtonModel.ExpDea = true;
                break;
            case 'nolic': backButtonModel.NoLic = true;
                break;
            default:
                break;
        }
        return backButtonModel;
    }

    //must write unit test
    createUserRequestModel(urlState): UserRequest {
        let userRequest = {} as UserRequest;

        if (urlState) {
            if (urlState.mode && urlState.mode.toLowerCase() === 'add') {
                this.setAddUserMode(userRequest);
            } else if (urlState.userid && urlState.userid.length > 0) {
                this.setEditOtherUserMode(userRequest, urlState.userid);
            }
            else {
                userRequest = this.setSelfEditMode(userRequest);
            }
        } else {
            userRequest = this.setSelfEditMode(userRequest);
        }

        return userRequest;
    }

    setAddUserMode(request: UserRequest): UserRequest {
        request.UserMode = UserMode.AddOtherUser;
        request.UserID = '';
        this.saveUserText = 'Add User';
        this.isSaveAllowed = false;
        return request;
    }

    setEditOtherUserMode(request: UserRequest, userID: string): UserRequest {
        request.UserMode = UserMode.EditOtherUser;
        request.UserID = userID;
        this.saveUserText = 'Save User';
        this.isSaveAllowed = true;
        return request;
    }

    setSelfEditMode(request: UserRequest): UserRequest {
        request.UserMode = UserMode.SelfEdit;
        request.UserID = '';
        this.saveUserText = 'Save';
        this.isSaveAllowed = true;
        return request;
    }

    displayRedirectionStatusMsg(backButtomModel: UserBackButtonModel, user: UserModel) {
        if (backButtomModel.NoLic) {
            this.createMessageModelAndPush('You do not have a valid state license for ' + user?.PracticeState + '. You must add one below before you can proceed.', MessageIcon.Error);
        }

        if (backButtomModel.NoNpi) {
            this.createMessageModelAndPush('NPI is now required. Please provide a valid NPI below.', MessageIcon.Error);
        }

        if (backButtomModel.ExpDea) {
            this.createMessageModelAndPush('Your DEA is expired. Please update it below.', MessageIcon.Error);
        }
    }

    createMessageModelAndPush(message: string, messageIcon: MessageIcon) {
        let messageModel: MessageModel = {
            Icon: messageIcon,
            ShowCloseButton: false,
            Tag: '',
            Message: message
        };
        this.messageStrip.push(messageModel);
    }

    openNewActivationModalPopup(userActivationInfo: UserActivationInfoModel) {
        this.newUserActivationModalPopup.openNewUserActivationModalPopup(userActivationInfo);
    }

    onSubmit(data: any) {

        if (this.user.UserMode === UserMode.SelfEdit) {
            this.messageStrip = [];
            this.createMessageModelAndPush('Saving user details..', MessageIcon.Success);
        }
        console.log(this.user);
        this.userSvc.saveUser(this.user).subscribe((response) => {
            this.messageStrip = response.MessageList;
            if (this.user.UserMode === UserMode.AddOtherUser) {
                if (response.IsSaveSuccessful) {
                    this.openNewActivationModalPopup(response.UserActivationInfo);
                }
            } else if (this.user.UserMode === UserMode.SelfEdit) {
                this.backButtonModel.IsSaveSuccessful = response.IsSaveSuccessful;
                this.backButtonModel.RedirectUrl = response.RedirectUrl;
            } else if (this.user.UserMode === UserMode.EditOtherUser) {
                if (response.IsSaveSuccessful) {
                    this.navigateToEditUsers(response.MessageList[0].Message);
                }
            }
        });
    }

    onBackButtonClick() {
        if (this.user.UserMode === UserMode.AddOtherUser) {
            //navigate to settings page
            this.router.navigateByUrl(ROUTE_NAME.Settings);
        } else if (this.user.UserMode === UserMode.EditOtherUser) {
            //navigate to Edit Users (plural) page 
            this.navigateToEditUsers('');
        } else {
            if (this.backButtonModel.IsSaveSuccessful && this.backButtonModel.RedirectUrl?.trim().length > 0) {
                if (this.backButtonModel.CameFrom?.length > 0) {
                    this.redirectToCameFromPage(this.backButtonModel.CameFrom);
                } else {
                    this.redirectAfterSave(this.backButtonModel.RedirectUrl);
                }
            } else {
                if (this.backButtonModel.NoNpi || this.backButtonModel.NoLic) {
                    this.userLogout('?expiresitecookie');
                } else if (this.backButtonModel.ExpDea) {
                    this.navigateToSelectPatient();
                } else if (this.backButtonModel.CameFrom?.length > 0) { //If the user shouldn't be logged out, camefrom should be set.
                    this.redirectToCameFromPage(this.backButtonModel.CameFrom);
                } else {
                    this.userLogout('?expiresitecookie');
                }
            }
        }
    }

    redirectAfterSave(pageName: string) {

        switch (pageName.toLowerCase()) {
            case PAGE_NAME.SelectPatient.toLowerCase():
                this.navigateToSelectPatient();
                break;
            case PAGE_NAME.Reports.toLowerCase():
                this.router.navigateByUrl(ROUTE_NAME.Reports, { state: { navigateTo: PAGE_NAME.Reports } });
                break;
            case PAGE_NAME.Tasks.toLowerCase():
                this.router.navigateByUrl(ROUTE_NAME.Tasks, { state: { navigateTo: PAGE_NAME.Tasks } });
                break;
            default:
                this.userLogout('?expiresitecookie');
        }
    }

    userLogout(param: string) {
        (<any>window).location = 'Logout.aspx' + param;
    }

    setChildFormStatus(value: boolean, formName: string) {
        if (formName === 'userinformation') {
            this.userInformationValidationStatus = value;
        }
        else if (formName === 'usercredentials') {
            this.userCredentialsValidationStatus = value;
        }
        else if (formName === 'usersettings') {
            this.userSettingsValidationStatus = value;
        }
        this.setSaveButtonStatus();
    }

    //must write unit test
    setSaveButtonStatus() {
        if (this.isEditUser()) {
            this.isSaveAllowed = this.verifyEditValidationStatus();
        } else if (this.isAddUser()) {
            this.isSaveAllowed = this.verifyAddUserValidationStatus();
        }
    }

    //must write unit test
    verifyEditValidationStatus(): boolean {
        let status = false;

        if (this.isUserPrescriber()) {
            if (this.userInformationValidationStatus && this.userCredentialsValidationStatus) {
                status = true;
            }
        } else {
            if (this.userInformationValidationStatus) {
                status = true;
            }
        }
        return status;
    }

    //must write unit test
    verifyAddUserValidationStatus(): boolean {
        let status = false;

        if (this.isUserPrescriber()) {
            if (this.userInformationValidationStatus && this.userCredentialsValidationStatus) {
                status = true;
            }
        } else {
            if (this.userInformationValidationStatus && this.userSettingsValidationStatus) {
                status = true;
            }
        }
        return status;
    }

    redirectToCameFromPage(url: string) {
        url = url.toLowerCase();
        if (url === PAGE_NAME.MyProfile.toLowerCase()) {
            this.router.navigateByUrl(ROUTE_NAME.MyProfile, { state: { navigateTo: PAGE_NAME.MyProfile } });
        }
        else if (url === PAGE_NAME.EditUsers.toLowerCase()) {
            this.navigateToEditUsers('');
        }
        else if (url === 'siteinfo' || url === PAGE_NAME.SelectPatient.toLowerCase()) {
            this.navigateToSelectPatient();
        }
    }

    navigateToSelectPatient() {
        this.router.navigateByUrl(ROUTE_NAME.SelectPatient, { state: { navigateTo: PAGE_NAME.SelectPatient } });
    }

    navigateToEditUsers(message: string) {
        if (message === '') {
            this.router.navigateByUrl(ROUTE_NAME.EditUsers, { state: { navigateTo: PAGE_NAME.EditUsers } });
        }
        else {
            this.router.navigateByUrl(ROUTE_NAME.EditUsers, {
                state: { navigateTo: PAGE_NAME.EditUsers + "?Message=" + message }
            });
        }
    }

    displayUserTypeDefinition() {
        this.UserTypeDefinition.toggleModalPopup();
    }

    displayUserMoreInformation(userId: string) {
        this.UserGuidModal.toggleModalPopup(userId);
    }

    displayUserMobileMessageDescription() {
        this.UserMobileMessageDescription.toggleModalPopup();
    }

    //must write unit test
    isUserPrescriber() {
        let userType = this.user?.UserSettings?.UserType;
        return userType === this.userCategoryEnum.PHYSICIAN_ASSISTANT ||
            userType === this.userCategoryEnum.PHYSICIAN_ASSISTANT_SUPERVISED ||
            userType === this.userCategoryEnum.PROVIDER;
    }

    //must write unit test
    isSelfEdit() {
        return this.user?.UserMode === UserMode.SelfEdit;
    }

    //must write unit test
    isAddUser(): boolean {
        return this.user.UserMode === UserMode.AddOtherUser;
    }

    //must write unit test
    isEditUser(): boolean {
        return (this.user.UserMode === UserMode.SelfEdit || this.user.UserMode === UserMode.EditOtherUser);
    }

    updateHasDEALicense($event: boolean) {
        if (this.user.UserCredentials.IsPRSite) {
            this.hasDEALicense = $event;
        }
    }
}