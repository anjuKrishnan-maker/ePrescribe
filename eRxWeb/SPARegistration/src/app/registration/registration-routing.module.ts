import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SubscriptionComponent } from './subscription/subscription.component';
import { UserCreationComponent } from './user-creation/user-creation.component';
import { WelcomePageComponent } from './welcome-page/welcome-page.component';
import { CanActivateRegistrationStep } from './can-load-registration.guard';
import { LicenseCreationComponent } from './license-creation/license-creation.component';
import { ErrorRegistrationComponent } from './error-registration/error-registration.component';
import { UserActivationComponent } from './activate-user/user-activation/user-activation.component';
import { AccountCreationComponent } from './account-creation/account-creation.component';
import { UserCspUpdaterComponent } from './user-csp-updater/user-csp-updater.component';
//import { UserCspUpdaterComponent } from './user-csp-updater/user-csp-updater.component';

export const RegistrationRoutes = {
    default: "register/",
    subscriptions: "register/subscriptions",
    createuser: "register/createuser",
    welcome: "register/welcome",
    createlicense: "register/createlicense",
    error: "register/error",
    activateuser: "register/activateuser",
    createAccount: "register/createAccount",
    usercspupdater: "register/usercspupdater"
}


export const registrationRoutes: Routes = [
    { path: 'Register.aspx', redirectTo: RegistrationRoutes.subscriptions },
    { path: 'register', redirectTo: RegistrationRoutes.subscriptions },
    { path: RegistrationRoutes.subscriptions, component: SubscriptionComponent },
    { path: RegistrationRoutes.createuser, component: UserCreationComponent, canActivate: [CanActivateRegistrationStep] },
    { path: RegistrationRoutes.welcome, component: WelcomePageComponent, canActivate: [CanActivateRegistrationStep] },
    { path: RegistrationRoutes.createlicense, component: LicenseCreationComponent, canActivate: [CanActivateRegistrationStep] },
    { path: RegistrationRoutes.error, component: ErrorRegistrationComponent },
    { path: RegistrationRoutes.activateuser, component: UserActivationComponent },
    { path: RegistrationRoutes.createAccount, component: AccountCreationComponent },
    { path: RegistrationRoutes.usercspupdater, component: UserCspUpdaterComponent }
];


@NgModule({
    imports: [RouterModule.forRoot(registrationRoutes, { enableTracing: false })],
    providers: [CanActivateRegistrationStep],
    exports: [RouterModule]
})
export class RegistrationRoutingModule { }


