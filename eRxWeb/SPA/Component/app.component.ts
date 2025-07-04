import { PopoverControl } from "./shared/controls/popover/popover.control";
import { Component, NgZone, ChangeDetectorRef, ViewChild, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { SafeUrl, DomSanitizer } from '@angular/platform-browser';
import {
    PatientService, EventService, ComponentCommonService, GoogleAnalyticsService,
    UserSessionService, SettingsService, ContentLoadService, SpinnerService, SelectPatientService
} from '../services/service.import.def';
import {
    NavigationModel, PrivacyOverrideModel, SelectPatientComponentParameters, InitalContentPayload,
    ContentRefreshPayload, SelectMedicationNavigationDTO, MessageModel, MenuItemModel
} from '../model/model.import.def';
import { NAVIGATION_EVENT_ID, EVENT_COMPONENT_ID, current_context, PAGE_NAME, SamlRefreshResult, ROUTE_NAME } from '../tools/constants';
import { SearchPatientComponent } from "./select-patient/select-patient.component";
import { Router, RoutesRecognized, RouterEvent, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from "@angular/router";
import { MenuComponent } from "./common/menu/menu.component";
import RouteNameExtractor from "../tools/utils/Route-Name-Extractor";
import { Subscription } from "rxjs";

@Component({
    selector: 'erx-app',
    templateUrl: './app.template.html',
    styleUrls: ['./app.style.css']
})
export class AppComponent implements OnDestroy {
    package: string;
    pageName: string;
    overrideText: string;
    purl: SafeUrl;
    isPageLoading: boolean = false;
    isOverlayVisible: boolean;
    userIsActive: boolean;
    sessionTimeoutTimer: any = null;
    clicked: boolean = false;
    Patient: PrivacyOverrideModel = new PrivacyOverrideModel();
    applicationMinWidth: string;
    isInitialLoading: boolean = true;
    forceHideDeluxeTeaserAd: boolean;
    page = PAGE_NAME.SelectPatient;
    private routeSubscription: Subscription;
    defaultMenuItem: MenuItemModel = new MenuItemModel();

    @ViewChild(PopoverControl) popoverControl: PopoverControl;
    @ViewChild(SearchPatientComponent) SelectPatientComponentObj: SearchPatientComponent;
    @ViewChild(MenuComponent) menuComponent: MenuComponent;

    set NavigationModel(value: NavigationModel) {
        this.ptSvc.PopupNavigation = value;
        this.purl = this.sanitizer.bypassSecurityTrustResourceUrl(value.PopupSrc);
    }

    get NavigationModel(): NavigationModel {
        return this.ptSvc.PopupNavigation;
    }

    get CurrentPageName(): string {
        return current_context.PageName;
    }
    get taskPage(): string {
        return PAGE_NAME.Tasks;
    }

    constructor(private ptSvc: PatientService, private location: Location,
        private sanitizer: DomSanitizer, private cd: ChangeDetectorRef, private compSvc: ComponentCommonService, private zone: NgZone,
        private evtSvc: EventService, private gaService: GoogleAnalyticsService, private settingSvc: SettingsService, private contentSvc: ContentLoadService,
        private userSessionSvc: UserSessionService, private router: Router, public spinnerSvc: SpinnerService, private selectPatientSvc: SelectPatientService) {

        this.isInitialLoading = true;

        (<any>window).AngularComponent = {
            zone: this.zone
        };


        this.routeSubscription = this.router.events.subscribe((data: RouterEvent) => {
            this.setNavigationLoader(data); 
            if (data instanceof RoutesRecognized) {
                if (!data.url)
                    return;
                if (data.url === '/')
                    return;

                current_context.PageName = this.page = "";
                Object.keys(PAGE_NAME).map((key: string) => {
                    let urlSplit = data.url.split("/");
                    let lastSplit = urlSplit[urlSplit.length - 1] === "" ? urlSplit[urlSplit.length - 2] : urlSplit[urlSplit.length - 1]; //take last non empty string in url
                    if (lastSplit.toLowerCase().trim() === key.toLowerCase().trim()) {
                        current_context.PageName = this.page = PAGE_NAME[key];
                    }
                });
                this.userIsActive = true;
                this.trackInsights(current_context.PageName);
                if (!this.isPageRefresh())
                    this.contentSvc.RetrieveRefreshPayload(current_context.PageName)
                        .then((isSuccess: boolean) => {
                            this.contentRefreshEvents(this.contentSvc.refreshedContentPayload);
                        });
            }
        });

        this.compSvc.AddWindowFunction('ShowOverlay', (value) => {
            this.isOverlayVisible = false;
            this.cd.detectChanges();
        });

        this.compSvc.AddWindowFunction('CloseOverlay', (value) => {
            this.isOverlayVisible = false;
            this.cd.detectChanges();
        });

        this.compSvc.AddWindowFunction('CloseTeaserAd', () => {
            this.hideDeluxeTeaserAd();
        });

        compSvc.AddWindowFunction('CloseDeluxeAdPopup', () => {
            //this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.CloseDeluxeAdPopup, null);
        });        

        this.compSvc.AddWindowFunction('LoadPDMPEnrollmentForm', () => {
            this.hideDeluxeTeaserAd();
            this.settingSvc.RetrieveLinks().subscribe(response => {
                response.forEach((keys: any, val: any) => {
                    if (keys["ActionUrl"] == PAGE_NAME.PdmpEnrollmentForm) {
                        this.SelectPatientComponentObj.showPDMPEnrollment();
                    }
                    else if (keys["ActionUrl"] == PAGE_NAME.PdmpEpcsTeaser) {
                        this.SelectPatientComponentObj.showEpcsTeaser();
                    }
                })
            });
        });
        this.compSvc.AddWindowFunction('LoadDeluxeAccountManagement', () => {
            this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures, { state: { navigateTo: PAGE_NAME.DeluxeAccountManagement } });
            this.cd.detectChanges();
        });

        this.compSvc.AddWindowFunction('resetModalUrl', (value) => {
            this.ptSvc.PopupNavigation.PopupSrc = undefined;
            this.cd.detectChanges();
        });

        this.compSvc.AddWindowFunction('redirectContent', (value) => {
            this.ptSvc.PopupNavigation.ContentSrc = value;
            this.cd.detectChanges();
        });

        this.compSvc.AddWindowFunction('UpdateSelectedPatientInfo', (value) => {
            this.selectPatientSvc.SetPatientInfo(value.patientId).subscribe(response => {
                if (response !== undefined) {
                    this.ptSvc.SelectedPatient = response;
                }
                this.router.navigateByUrl(ROUTE_NAME.SelectMedication);
            })
        });

        this.compSvc.AddWindowFunction('ShowReviewHistoryPage', (componentParameters) => {
            current_context.PageName = PAGE_NAME.ReviewHistory;
            this.ptSvc.PopupNavigation.ContentSrc = '';
            this.router.navigateByUrl(ROUTE_NAME.ReviewHistory, { state: { param: componentParameters } });
            this.cd.detectChanges();
        });

        this.compSvc.AddWindowFunction('ShowSelectMedicationPage', (componentParameters: SelectMedicationNavigationDTO) => {
            this.NavigateToSelectMedicationPage(componentParameters);

        });

        this.compSvc.AddWindowFunction('ShowSelectPatientPage', (componentParameters) => {
            this.NavigateToSelectPatient(componentParameters);
        });

        this.compSvc.AddWindowFunction('showDeluxeFeatureSelectionPage', (componentParameters) => {
            this.hideDeluxeTeaserAd();
            this.NavigateToDeluxeFeatureSelection(componentParameters);
        })

        this.compSvc.AddWindowFunction('ShowEditUserPage', (componentParameters) => {
            this.NavigateToEditUser(componentParameters);
        })

        this.compSvc.AddWindowFunction('ShowHomeAddressPage', (componentParameters) => {
            this.NavigateToHomeAddress(componentParameters);
        })

        //From Report(classic aspx) back to the SPA page.
        this.compSvc.AddWindowFunction('ShowReportsPage', () => {
            this.router.navigateByUrl(ROUTE_NAME.Reports);
        });

        this.compSvc.AddWindowFunction('ShowSettingsPage', (componentParameters) => {
            this.NavigateToSettings(componentParameters);           
        });

        // load patient header component from .net, called from a few weird places in the spa bridge
        this.compSvc.AddWindowFunction('SavePatient',
            (patientid) => {
                this.saveEditedPatient(patientid);
            });


        //exception
        this.compSvc.AddWindowFunction('setException', (value: boolean) => {
            current_context.isException = value;
        });

        this.compSvc.AddWindowFunction('Logout', () => {
            this.evtSvc.invokeEvent(NAVIGATION_EVENT_ID.SessionTimeout, {});
        });

        this.evtSvc.subscribeEvent(NAVIGATION_EVENT_ID.SessionTimeout, (a: any) => {
            (<any>window).location = 'Logout.aspx?Timeout=YES';
        });

        this.evtSvc.subscribeEvent(NAVIGATION_EVENT_ID.SessionReAuthRequired, (a: any) => {
            (<any>window).location = 'Logout.aspx?reauthenticate=true';
        });

        this.ptSvc.PopupNavigation = new NavigationModel();

        this.compSvc.AddWindowFunction('LoadComponent', (payload) => {
            this.ptSvc.PopupNavigation.ContentSrc = '';
            //this.evtSvc.invokeEvent(NAVIGATION_EVENT_ID.HeaderLinkClicked, payload);
            //TODO: if bugs reported revisit, else DEMOLISH
        });

        this.compSvc.AddWindowFunction('ContentPageLoadStarted', (pageName: string) => {          
            if (current_context.PageName.indexOf('.aspx') > 0)
            this.spinnerSvc.showPageSpinner();
        });

        setInterval(() => {
            if (this.userIsActive) {
                this.CheckForExtendedSessionTime();
                this.userIsActive = false;
            }
        }, 60000);

        this.compSvc.AddWindowFunction('ContentPageLoaded', (classicalPage: string) => {
            this.spinnerSvc.hidePagespinner();
            this.menuComponent.checkMenuVisibility(classicalPage);
            current_context.PageName = classicalPage;
            this.onLoadingClassicPage(classicalPage);
        });
    }


    private isPageRefresh(): boolean {
        return !this.router.navigated;
    }

    private setNavigationLoader(event: RouterEvent): void {
        if (event instanceof NavigationStart) {           
            this.spinnerSvc.showPageSpinner();
        }
        else if (event instanceof NavigationEnd ||
            event instanceof NavigationCancel ||
            event instanceof NavigationError) {           
            this.spinnerSvc.hidePagespinner();
        }
    }

    //When the aspx pages in the frame is loaded
    private onLoadingClassicPage(pageName: string): void {
        this.trackInsights(pageName);

        this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.ContentRefreshing, null);

        this.contentSvc.RetrieveRefreshPayload(this.CurrentPageName).then((isSuccess: boolean) => {
            this.contentRefreshEvents(this.contentSvc.refreshedContentPayload);
        });

        this.userIsActive = true;
    }

    private trackInsights(pageName: string): void {
        this.gaService.SendPageView(pageName);// sending analytics

        try {
            let pageviewed = pageName && pageName.length > 0 ? pageName.toLowerCase() : pageName;
            (<any>window).appInsights?.trackPageView(pageviewed);
        } catch (error) {
            console.log(error);
        }
    }

    private hideDeluxeTeaserAd(): void {
        this.forceHideDeluxeTeaserAd = false;
        setTimeout(() => this.forceHideDeluxeTeaserAd = true, 0);
    }

    ngOnInit() {
        let redirectPage = this.setRedirectionPage();
        current_context.PageName = redirectPage && redirectPage.length > 0 ? redirectPage : PAGE_NAME.SelectPatient;
        this.page = current_context.PageName;
        var thisObj = this;
        //on close modal popup
        $('#mdlPopupMain').on('hidden.bs.modal', function (e) {
            $("#frameMdlPopup").attr('src', '');
            thisObj.NavigationModel.PopupSrc = '';
            thisObj.ptSvc.PopupNavigation.PopupSrc = '';

            if (thisObj.ptSvc.PopupNavigation.OnDismissEventName != null && thisObj.ptSvc.PopupNavigation.OnDismissEventName != '') {
                thisObj.evtSvc.invokeEvent(thisObj.ptSvc.PopupNavigation.OnDismissEventName, null);
                thisObj.ptSvc.PopupNavigation.OnDismissEventName = null;
            }
        });

        window.addEventListener("message", (e) => {
            let thisObj = this;
            this.receiveMessage(e, thisObj);
        }, false);

        if (this.contentSvc.initalContentPayload.SitePayload)
            this.applicationMinWidth = current_context.isTieEnabled ? "1162px" : "1019px";
    }

    ngAfterViewInit() {
        $('[rel="popover"]')
            .popover({
                container: 'body',
                html: true,
                content: function () {
                    var clone = $($(this).data('popover-content')).clone(true).removeClass('hide');
                    return clone;
                }
            })
            .click(function (e) {
                e.preventDefault();
            });
        this.userIsActive = true;
        this.contentInitializeEvents(this.contentSvc.initalContentPayload);
        this.contentRefreshEvents(this.contentSvc.initalContentPayload.ContentRefreshPayload);
    }

    ngOnDestroy(): void {
        this.routeSubscription.unsubscribe();
    }

    private setRedirectionPage(): string {
        let redirectPage = this.getRedirectionPage();
        if (redirectPage && redirectPage.length > 0) {
            this.router.navigateByUrl(RouteNameExtractor.ExtractRoute(redirectPage.toLowerCase()), {
                state: {
                    navigateTo: redirectPage
                }
            });
        }
        return redirectPage;
    }

    private getRedirectionPage(): string {
        //Handling  Navigation coming from SPA.aspx.cs/SSO
        let redirectPage: string = (<any>window).redirectPage;
        if (redirectPage && redirectPage.length > 0) {
            return redirectPage;
        }
        //Handling  browser refresh
        if (this.contentSvc.initalContentPayload.MenuPayload) {

            this.defaultMenuItem = this.contentSvc.initalContentPayload.MenuPayload.find((x) => x.Order = 1);
            return this.defaultMenuItem.NavigationUrl;
        }
        return "";
    }

    public saveEditedPatient(patientId: string) {
        current_context.PageName = PAGE_NAME.SelectPatient;
        this.ptSvc.PopupNavigation.ContentSrc = '';
        this.updatePatientInfo(patientId);
    }

    public updatePatientInfo(patientId: string) {
        let selectPatientComponentParameters = <SelectPatientComponentParameters>{
            PatientId: patientId
        }
        this.router.navigateByUrl(ROUTE_NAME.SelectPatient, {
            state: {
                patientInfo: selectPatientComponentParameters
            }
        });
    }

    SetSessionTimeoutTimer(sessionTimeoutMs) {
        if (sessionTimeoutMs == null) sessionTimeoutMs = 1200000; //20 minutes like current implementation

        clearTimeout(this.sessionTimeoutTimer);
        this.sessionTimeoutTimer = setTimeout(() => {
            this.evtSvc.invokeEvent(NAVIGATION_EVENT_ID.SessionTimeout, {});
        }, sessionTimeoutMs);
    }

    CheckForExtendedSessionTime() {
        this.userSessionSvc.AttemptSamlTokenRefresh().subscribe((tokenRefreshResult) => {
            if (tokenRefreshResult.Result === SamlRefreshResult.ForceLogout) {
                this.evtSvc.invokeEvent(NAVIGATION_EVENT_ID.SessionTimeout, {});
            }
            else if (tokenRefreshResult.Result === SamlRefreshResult.ReauthenticationRequired) {
                this.evtSvc.invokeEvent(NAVIGATION_EVENT_ID.SessionReAuthRequired, {});
            }
            else if (tokenRefreshResult.Result === SamlRefreshResult.SessionExtended) {
                this.SetSessionTimeoutTimer(tokenRefreshResult.NewExpirationTimeMs);
            }
        });
    }

    private NavigateToSelectMedicationPage(componentParameters: SelectMedicationNavigationDTO) {
        this.router.navigateByUrl(ROUTE_NAME.SelectMedication, {
            state: {
                searchText: componentParameters
            }
        });
    }

    private NavigateToSelectPatient(componentParameters: string) {
        let patientInfo = JSON.parse(componentParameters) as SelectPatientComponentParameters;
        this.router.navigateByUrl(ROUTE_NAME.SelectPatient, {
            state: {
                patientInfo: patientInfo
            }
        });
    }    

    private NavigateToDeluxeFeatureSelection(componentParameters: string) {                                  
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures);        
    }

    private NavigateToEditUser(componentParameters: string) {
        let urlParameters = JSON.parse(componentParameters);
        this.router.navigateByUrl(ROUTE_NAME.EditUser, {
            state:
            {
                mode: 'edit',
                userid: urlParameters.userid,
                cameFrom: urlParameters?.CameFrom,
                status: urlParameters?.status
            }
        });
    }

    private NavigateToHomeAddress(componentParameters: string) {
        this.router.navigateByUrl(ROUTE_NAME.HomeAddress);       
    }


    private NavigateToSettings(componentParameters: MessageModel) {
        this.router.navigateByUrl(ROUTE_NAME.Settings, {
            state: {
                messageModel: componentParameters
            }
        });
    }

    CleanPatientHeader() {
        //reseting header patient data
        this.ptSvc.ResetSelectedPatient();
    }



    contentInitializeEvents(initalContentPayload: InitalContentPayload) {
        current_context.UserPreference = initalContentPayload.UserPreferencePayload;
        this.SetSessionTimeoutTimer(initalContentPayload.SessionTimeoutPayload);
        this.isInitialLoading = false;
    }

    contentRefreshEvents(contentRefreshPayload: ContentRefreshPayload) {
        this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, contentRefreshPayload.RightPanelPayload);
        this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.ILearnLoaded, contentRefreshPayload.ILearnPayload);
    }

    receiveMessage(event, thisObj) {
        if (event.data !== 'invoke') {
            thisObj.evtSvc.invokeEvent(event.origin, event);
        }
    }

    IsSelectPatientVisible() {
        return this.CurrentPageName === PAGE_NAME.SelectPatient;
    }

    topDivClicked() {
        this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.ClosePopover, {});
    }

    onEPCSTeaserClose(isVisible: boolean) {
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures);
    }
}