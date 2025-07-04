import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserComponent } from '../../component/user/user.component';
import { UserService, MobileRegistrationService } from '../../services/service.import.def';
import { UserStateLicenseComponent } from '../../component/user/user-state-license/user-state-license.component';
import { AddUserStateLicenseComponent } from '../../component/user/user-state-license/add-user-state-license/add-user-state-license.component';
import { SharedComponentsModule } from '../shared-components-module/shared-components.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatExpansionModule } from '@angular/material/expansion';
import { UserSecuritySettingsComponent } from '../../component/user/user-security-settings/user-security-settings.component';
import { UserSettingsComponent } from '../../component/user/user-settings/user-settings.component';
import { UserInformationComponent } from '../../component/user/user-information/user-information.component';
import { UserCredentialsComponent } from '../../component/user/user-credentials/user-credentials.component';
import { UserDeaLicenseComponent } from '../../component/user/user-dea-license/user-dea-license.component';
import { UserEpcsSettingsComponent } from '../../component/user/user-epcs-settings/user-epcs-settings.component';
import { UserMobileMessagingSettingsComponent } from '../../component/user/user-mobile-messaging-settings/user-mobile-messaging-settings.component';
import { UserPreferencesComponent } from '../../component/user/user-preferences/user-preferences.component';
import { AddUserDeaLicenseComponent } from '../../component/user/user-dea-license/add-user-dea-license/add-user-dea-license.component';
import { DeaLicenseService } from '../../services/service.import.def';
import { MatDatepickerModule, MatNativeDateModule, MatInputModule } from '@angular/material';
import { UserTypeDefinitionComponent } from '../../component/user/user-settings/user-type-definition/user-type-definition.component';
import { UserGuidModalComponent } from '../../component/user/user-settings/user-guid-modal/user-guid-modal.component';
import { UserSupervisingProvidersComponent } from '../../component/user/user-settings/user-supervising-provider-modal/user-supervising-provider.component';
import { UserMobileMessageDescriptionComponent } from '../../component/user/user-mobile-messaging-settings/user-mobile-messaging-description/user-mobile-messaging-description.component';
import { UserMobileRegistrationComponent } from '../../component/user/user-mobile-messaging-settings/user-mobile-registration/user-mobile-registration.component';
import { NewUserActivationModalPopupComponent } from '../../component/user/new-user-activation-modal-popup/new-user-activation-modal-popup.component';
import { EditSecretQuestionsModalPopupComponent } from '../../component/user/user-security-settings/edit-secret-questions-modal-popup/edit-secret-questions-modal-popup.component';



@NgModule({
    imports: [CommonModule, FormsModule, SharedComponentsModule, BrowserAnimationsModule, MatExpansionModule, MatDatepickerModule, MatNativeDateModule, MatInputModule],
    declarations: [UserComponent, NewUserActivationModalPopupComponent, UserSupervisingProvidersComponent, UserStateLicenseComponent, EditSecretQuestionsModalPopupComponent,
        AddUserStateLicenseComponent, UserInformationComponent, UserSecuritySettingsComponent, UserSettingsComponent, UserCredentialsComponent, 
        UserDeaLicenseComponent, UserEpcsSettingsComponent, UserMobileMessagingSettingsComponent, UserPreferencesComponent, AddUserDeaLicenseComponent, UserTypeDefinitionComponent,
        UserGuidModalComponent, UserMobileMessageDescriptionComponent, UserMobileRegistrationComponent],
    providers: [UserService, DeaLicenseService, MobileRegistrationService],
    exports: []
})
export class UserModule { }
