import { Component, OnInit, Input, ViewChild, Output, EventEmitter } from "@angular/core";
import { UserSettingsModel, UserCategory, SupervisingProviderSelectionMode } from "../../../model/user/user.model";
import { UserSupervisingProvidersComponent } from "./user-supervising-provider-modal/user-supervising-provider.component";

@Component({
    selector: 'erx-user-settings',
    templateUrl: './user-settings.template.html',
    styleUrls: ['./user-settings.style.css']
})
export class UserSettingsComponent implements OnInit {
    @Input() userSettings: UserSettingsModel;
    @Output() isUserSettingsFormValid: EventEmitter<boolean> = new EventEmitter<boolean>();
    @ViewChild('UserSupervisingProviders') UserSupervisingProviders: UserSupervisingProvidersComponent;
    userCategoryEnum = UserCategory;
    supervisingProviderSelectionModeEnum = SupervisingProviderSelectionMode;
    initialUserType: UserCategory;

    ngOnInit() {
        this.initialUserType = JSON.parse(JSON.stringify(this.userSettings.UserType));
    }

    openSelectSupervisingProvider() {
        if (!this.userSettings.IsRestrictedMode) {
            this.UserSupervisingProviders.toggleModalPopup();
            this.UserSupervisingProviders.userName = this.userSettings.Name;
            this.UserSupervisingProviders.originalUserSupervisingProvidersList = JSON.parse(JSON.stringify(this.UserSupervisingProviders.userSupervisingProvidersList));
            this.UserSupervisingProviders.originalIsAllCheckboxSelected = JSON.parse(JSON.stringify(this.UserSupervisingProviders.isAllCheckboxSelected));
        }
    }

    IsSwitchingDenied(fromUserType: UserCategory, toUserType: UserCategory): boolean {        
        if (this.initialUserType != -1) {
            if (fromUserType == UserCategory.PROVIDER && (this.isPAUser(this.initialUserType))) {
                return (this.isPAUser(toUserType) || toUserType == UserCategory.PROVIDER) ? false : true;
            }            
            if (this.isPAUser(fromUserType)) {
                if (this.isPAUser(toUserType) || toUserType == UserCategory.PROVIDER)
                    return false;
                else return true;
            }
            if (this.isPOBUser(fromUserType))
                if (this.isPOBUser(toUserType))
                    return false;
                else return true;
            if (fromUserType == UserCategory.GENERAL_USER) {
                return toUserType == UserCategory.GENERAL_USER ? false : true;
            }
            if (fromUserType == UserCategory.PROVIDER) {
                return toUserType == UserCategory.PROVIDER ? false : true;
            }
        }
        else
            return false;
    }

    isPOBUser(type: UserCategory): boolean {
        if (type == UserCategory.POB_LIMITED || type == UserCategory.POB_REGULAR || type == UserCategory.POB_SUPER)
            return true;
        else return false;
    }

    isPAUser(type: UserCategory): boolean {
        if (type == UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED || type == UserCategory.PHYSICIAN_ASSISTANT)
            return true;
        else return false;
    }

    userTypeSet() {
        this.isUserSettingsFormValid.emit(true)
    }

    changeSupervisingProviderSelectionMode(switchToAll :boolean) {
        if (switchToAll) {
            this.userSettings.SupervisingProviderSelectionMode = this.supervisingProviderSelectionModeEnum.All;
        }
    }
}