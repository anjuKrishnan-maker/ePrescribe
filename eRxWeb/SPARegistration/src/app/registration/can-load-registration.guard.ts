import { CanActivate } from "@angular/router/src/utils/preactivation";
import { RouterStateSnapshot, UrlTree, ActivatedRouteSnapshot, Router, Route, NavigationExtras } from "@angular/router";
import { Observable } from "rxjs";
import { Injectable, Inject } from "@angular/core";
import { map, catchError } from 'rxjs/operators';

import { RegistrationService } from "../service/registration.service";
import { RegistrationRoutes } from "./registration-routing.module";
import { MessageService } from "../service/message.service";
import { HttpErrorResponse } from "@angular/common/http";
@Injectable()
export class CanActivateRegistrationStep implements CanActivate {
    path: ActivatedRouteSnapshot[];
    route: ActivatedRouteSnapshot;
    readonly defaultRoute = `/${RegistrationRoutes.default}`;

    constructor(private registrationService: RegistrationService,
        private router: Router,
        @Inject('window') private window: any,
        private messageService: MessageService) {

    }

    canActivate(route: ActivatedRouteSnapshot, routerStateSnapshot: RouterStateSnapshot): Observable<boolean | UrlTree>
        | Promise<boolean | UrlTree> | boolean | UrlTree {

        const currentState = this.router.getCurrentNavigation().extras.state;


        if (currentState?.authentication == false && routerStateSnapshot.url.indexOf(`/${RegistrationRoutes.createuser}`) > -1)
            return true;

        return this.registrationService
            .getRegistrantStatus(routerStateSnapshot.url == `/${RegistrationRoutes.createlicense}`)
            .pipe(map((registrantContext: RegistrantContext) => {
                if (!registrantContext) {
                    throw new HttpErrorResponse({ status: 401, statusText: "not authenticated" });
                }
                return this.interpretRegistantStatus(routerStateSnapshot.url, registrantContext);
            }), catchError((err, caught) => {

                let url = '/register';
                let naviagtionExtras: NavigationExtras = { queryParamsHandling: "merge" };
                

                if (err?.status == 401 && routerStateSnapshot.url.indexOf(`/${RegistrationRoutes.createuser}`) > -1) {
                    url = routerStateSnapshot.url;
                    let tempState = {
                        state: { ...currentState, ...{ authentication: false } }
                    };
                    naviagtionExtras = { ...naviagtionExtras, ...tempState };
                    return this.router.navigateByUrl(url, naviagtionExtras);
                }

                if (err?.status == 401) {
                    this.messageService.notify("Not authorized");
                }
                return Promise.resolve(false);

            }));

    }

    private registrantRouteDecoder = (registrantContext: RegistrantContext, requestedRoute: string): string => {

        if (registrantContext.IsLOA3StatusConfirmed)
            return `/${RegistrationRoutes.createlicense}`;

        if (registrantContext.IsUserCreated || registrantContext.IseRxUser)
            return `/${RegistrationRoutes.welcome}`; //redirect to csp 

        if (!registrantContext.IsUserCreated)
            return `/${RegistrationRoutes.createuser}`;//Redirect to create user page.

        return requestedRoute;
    }

    readonly handlers: Dictionary<(regitratContext: RegistrantContext, requestedRoute: string) => string> = {
        [`/${RegistrationRoutes.createuser}`]: this.registrantRouteDecoder,
        [`/${RegistrationRoutes.welcome}`]: this.registrantRouteDecoder,
        [`/${RegistrationRoutes.createlicense}`]: this.registrantRouteDecoder

    };

    public interpretRegistantStatus(command: string, regitratContext: RegistrantContext): UrlTree | boolean {
        if (!this.containsHandler(command))
            return this.router.parseUrl(this.defaultRoute);
        let interpretedRoute = this.handlers[command](regitratContext, command);

        if (interpretedRoute === command)
            return true;
        if (!this.isKnownRoute(interpretedRoute)) {
            this.window.location.href = interpretedRoute;
            return false;
        }

        return this.router.parseUrl(interpretedRoute);
    }

    private isKnownRoute(url:string): boolean {
        let routes: Route[] = this.router.config;
        //Note:url contains starting /.. hence the substring
        return routes && routes.length > 0 && routes.find(x => x.path.indexOf(url.substring(1)) > -1) != undefined;
    }

    private containsHandler(key: string) {
        return this.handlers[key] != null;
    }
}

interface Dictionary<T> {
    [Key: string]: T;
}


export interface RegistrantContext {
    IsUserCreated: boolean;//Is False- redirect to create user.
    IsLOA3StatusConfirmed: boolean;//* Is False & IsUserCreated - redirect to CSP page//Is True - Redirect to Welcome page 
    IseRxUser: boolean;// Is true - redirect to welcome page
    RedirectUrl: string;
    CspUrl: string;
}