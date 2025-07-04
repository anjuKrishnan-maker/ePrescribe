import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SubscriptionComponent } from './subscription/subscription.component';
import { UserCreationComponent } from './user-creation/user-creation.component';
import { LicenseCreationComponent } from './license-creation/license-creation.component';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { UserCreateService } from '../service/user-create.service';
import { HttpClientModule } from '@angular/common/http';
import { WelcomePageComponent } from '../registration/welcome-page/welcome-page.component';
import { LicenseCreateService } from '../service/license-create.service';
import { WelcomeService } from '../service/welcome-service';

import { RegistrationRoutingModule } from './registration-routing.module';
import { ErrorRegistrationComponent } from './error-registration/error-registration.component';
import { MaxTryFailureComponent } from './max-try-failure-popup/max-try-failure.component';
import { UserActivationComponent } from '../registration/activate-user/user-activation/user-activation.component';
import { UserActivationPopup } from '../registration/activate-user/user-activation-popup/user-activation-popup.component';
import { ShowCaptchaComponent } from '../registration/show-captcha/show-captcha.component';
import { CaptchaService } from '../service/captcha.service';
import { UserPasswordComponent } from '../registration/user-password/user-password.component';
import { UserSecurityQuestionsComponent } from '../registration/user-security-questions/user-security-questions.component';
import { UserAccountNameComponent } from '../registration/user-account-name/user-account-name.component';
import { UserExistingAccountComponent } from '../registration/user-existing-account/user-existing-account.component';
import { ActivationCodeService } from '../service/activation-code.service';
import { LinkExistingAccount } from '../registration/activate-user/link-existing-account/link-existing-account-component'
import { AccountCreationComponent } from '../registration/account-creation/account-creation.component';
import { UserCspUpdaterComponent } from './user-csp-updater/user-csp-updater.component';
import { LinkAccountConfirmationPopupComponent } from '../registration/activate-user/link-account-confirmation-popup/link-account-confirmation-popup-component'
@NgModule({
    declarations: [SubscriptionComponent,
        UserCreationComponent,
        LicenseCreationComponent,
        WelcomePageComponent,
        ErrorRegistrationComponent,
        MaxTryFailureComponent,
        UserActivationComponent,
        UserActivationPopup,
        ShowCaptchaComponent,
        UserPasswordComponent,
        UserSecurityQuestionsComponent,
        UserAccountNameComponent,
        UserExistingAccountComponent,
        AccountCreationComponent,
        UserCspUpdaterComponent,  
        LinkExistingAccount,
        LinkAccountConfirmationPopupComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        RegistrationRoutingModule
    ],
    providers: [
        UserCreateService,
        LicenseCreateService,
        WelcomeService,
        CaptchaService,
        ActivationCodeService
    ]
})
export class RegistrationModule { }
