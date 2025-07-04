import { Component, Input, SimpleChange, ChangeDetectorRef, ViewChild, OnInit } from '@angular/core';
import { NavigationModel, Patient, PharmacyModel, RefreshHeaderDTO, PatientContextDTO } from '../../../../model/model.import.def';
import { PatientService, ComponentCommonService, EventService, PharmacyService, SpinnerService } from '../../../../services/service.import.def';
import { NAVIGATION_EVENT_ID, EVENT_COMPONENT_ID, current_context, PAGE_NAME, MODAL_DISMISS_EVENT, ROUTE_NAME } from '../../../../tools/constants';
import { PopoverControl } from '../../../shared/controls/popover/popover.control';
import { Subscription } from 'rxjs';
import { Router, RoutesRecognized, RouterEvent, NavigationEnd } from '@angular/router';

@Component(
    {
        selector: 'erx-patient-info',
        templateUrl: './patient-info.template.html',
        styleUrls: ['./patient-info.style.css']
    }
)

export class PatientInfoComponent implements OnInit {
    _objHasMrn: boolean;
    _patientHeader: Patient;
    isLoadingData: boolean = false;
    isEditable: boolean = false;
    hideAllergy: boolean = true;
    hideProblems: boolean = true;
    isPharmaciesEditable: boolean = false;
    isRestrictedUser: boolean = false;
    showRealName: boolean = false;
    pharmaModel: PharmacyModel = new PharmacyModel();
    pharmaModalTitle: string = '';
    visible: boolean = true;
    IsVipPatient: boolean = false;
    RealName: string = '';
    allowPatientEdit: boolean = false;
    private getPatientHeaderSubscription: Subscription;
    @Input() NavigationModel: NavigationModel;
    @ViewChild('activeAlergies') activeAlergies: PopoverControl;
    @ViewChild('activeProblems') activeProblems: PopoverControl;
    @ViewChild('activeMeds') activeMeds: PopoverControl;
    @Input()

    set patientHeader(patientHeader: Patient) {
        this._patientHeader = this.ptSvc.SelectedPatient;// patientHeader || new Patient();
        this.patientHasMrn = this.ptSvc.SelectedPatient.MRN !== undefined;
        this.isRestrictedUser = this.ptSvc.SelectedPatient.IsRestrictedUser;
    }

    ngOnInit() {
        this.displayPatientInfoForContext();
        this.isEditable = false;
        this.patientHeader = new Patient();
    }

    private displayPatientInfoForContext(): void {
        this.router.events.subscribe((pageEvent: RouterEvent) => {
            if (pageEvent instanceof NavigationEnd) {
                let routeParts = pageEvent.urlAfterRedirects.split('/');
                let previousPage = this.getPreviousPage(routeParts);
                let currentPage: string = routeParts.pop() || routeParts.pop();  // handle potential trailing slash
                this.displayPatientInfoForPage(currentPage, previousPage);
            }
        });
    }

    get patientHeader() {
        return this.ptSvc.SelectedPatient;
    }

    set patientHasMrn(value: boolean) {
        this._objHasMrn = value;
    }

    get patientHasMrn() {
        return this._objHasMrn;
    }

    get isException(): boolean {
        return current_context.isException;
    }

    constructor(private ptSvc: PatientService, private comonSvc: ComponentCommonService, private spinnerService: SpinnerService,
        private evtSvc: EventService, private pharmaService: PharmacyService, private cdRef: ChangeDetectorRef, private router: Router) {

        this.patientHeader = new Patient();

        // load patient header angular subscriber
        this.evtSvc.subscribeEvent(EVENT_COMPONENT_ID.RefreshPatientHeader, (data: RefreshHeaderDTO) => {
            if (data && data.isToClearHeader) {
                this.LoadPatientHeader(null);
                return;
            }
            if (this.ptSvc.SelectedPatient.PatientID === null || this.ptSvc.SelectedPatient.PatientID === undefined) {
                this.ptSvc.GetPatientFromSession().subscribe(response => {
                    this.SetPatientData(response);
                });
            } else {
                this.LoadPatientHeader(this.ptSvc.SelectedPatient.PatientID);
            }
        });

        this.evtSvc.subscribeEvent(EVENT_COMPONENT_ID.OnPatientSelected, (patientContext: PatientContextDTO) => {
            if (patientContext && patientContext.containsPatientContext && patientContext.isPrivacyPatient != true) {
                this.SetPatientHeaderEditStatus(this.ptSvc.SelectedPatient);
            }
        });
        this.evtSvc.subscribeEvent(MODAL_DISMISS_EVENT.AddDiagnosisModal, () => {
            this.ptSvc.GetPatientDiagnosis().subscribe(diagInfo => {
                this.ptSvc.SelectedPatient.Dx = diagInfo.Dx;
                this.ptSvc.SelectedPatient.MoreActiveProblem = diagInfo.MoreActiveProblem;
                this.ptSvc.SelectedPatient.ActiveDignosises = diagInfo.ActiveDiagnosis;
                this.cdRef.detectChanges();
            }
            );
        });

        this.evtSvc.subscribeEvent(MODAL_DISMISS_EVENT.AddAllergyModal, () => {
            this.ptSvc.GetPatientAllergies().subscribe(allergies => {
                this.ptSvc.SelectedPatient.Allergy = allergies.Allergy;
                this.ptSvc.SelectedPatient.MoreActiveAllergy = allergies.MoreActiveAllergy;
                this.ptSvc.SelectedPatient.ActiveAllergies = allergies.ActiveAllergies;
                this.cdRef.detectChanges();
            }
            );
        });

        this.evtSvc.subscribeEvent(EVENT_COMPONENT_ID.OnPatientMedicationsUpdated, () => {
            this.ptSvc.GetPatientActiveMeds().subscribe(response => {
                this.ptSvc.SelectedPatient.ActiveMed = response.ActiveMed;
                this.ptSvc.SelectedPatient.ActiveMeds = response.ActiveMeds;
                this.ptSvc.SelectedPatient.MoreActiveMedVisible = response.MoreActiveMedVisible;
                this.cdRef.detectChanges();
            }
            );
        });


        this.evtSvc.subscribeEvent(EVENT_COMPONENT_ID.ClosePopover, () => {
            this.showRealName = false;
        });

       

        // load patient header component from .net, called from a few weird places in the spa bridge
        comonSvc.AddWindowFunction('LoadPatientHeader', (patientid) => {
            this.LoadPatientHeader(patientid);
        });

        
        // this doesn't seem to be called from spa bridge or any where else
        //
        comonSvc.AddWindowFunction('PatientLinkSelected', (patientid) => {
            this.OpenPrivacyOverridePopup(patientid);
        });

        // called from ApproveScriptMessagePatient when reconciling restricted patient during refill workflow
        comonSvc.AddWindowFunction('PatientSelected', (patientid) => {
            this.OpenPrivacyPopupOnReconcile(patientid);
        });

        // clear patient header component from .net
        comonSvc.AddWindowFunction('CleanPatientHeader', () => {
            this.CleanPatientHeader();
        });

        // select and load patient when patient selected from TaskSummary/ListSendScripts (refills/approvals) or SpecialtyMedTasks grid
        comonSvc.AddWindowFunction('SelectPatientFromGrid', (patientid) => {
            this.SelectPatient(patientid);
        });

        comonSvc.AddWindowFunction('UpdatePatientPharmacy', () => {
            this.UpdatePatientPharmacy();
        });
    }

    private getPreviousPage(routeParts: string[]): string {
        let previousPage = "";
        if (routeParts.length > 1) {
            previousPage = routeParts[routeParts.length - 2]
        }
        return previousPage;
    }

    private getPageName(currentPage: string, previousPage: string): string {
        let pageName = currentPage;
        if (previousPage.toLowerCase() == PAGE_NAME.Settings.toLowerCase()) {
            pageName = PAGE_NAME.Settings
        }
        else if (previousPage.toLowerCase() == PAGE_NAME.Reports.toLowerCase()) {
            pageName = PAGE_NAME.Reports
        }
        return pageName;
    }

    private displayPatientInfoForPage(currentPage: string, previousPage: string) {        
        this.visible = true;
        let pageName = this.getPageName(currentPage, previousPage);
        switch (pageName.toLowerCase()) {

            case PAGE_NAME.SelectPatient.toLowerCase():
                this.isEditable = true;
                this.isPharmaciesEditable = true;
                this.hideAllergy = false;
                this.hideProblems = false;
                break;

            case PAGE_NAME.SelectMedication.toLowerCase():
                if (this.ptSvc.SelectedPatient.PatientID == null) {
                    this.ptSvc.GetPatientFromSession().subscribe(response => {
                        this.SetPatientData(response);
                    });
                }
                this.isPharmaciesEditable = false;
                this.isEditable = false;
                break;

            
            case PAGE_NAME.ListSendScripts.toLowerCase():
            case PAGE_NAME.PharmRefillSummaryPage.toLowerCase():                
                this.LoadPatientHeader(null);
                break;

            
            case PAGE_NAME.SelfReportedMed.replace(".aspx", "").toLowerCase():
            case PAGE_NAME.SpecialtyMedTasks.toLowerCase():                     
                this.LoadPatientHeader(this.ptSvc.SelectedPatient.PatientID);
                break;

            case PAGE_NAME.ReviewHistory.toLowerCase():                
                this.isPharmaciesEditable = true;
                this.isEditable = false;
                break;

            case PAGE_NAME.NurseSig.replace(".aspx", "").toLowerCase():
            case PAGE_NAME.MultiViewReport.toLowerCase():   
                break;

            case PAGE_NAME.ScriptPad.replace(".aspx","").toLowerCase():
                this.isEditable = true;
                this.hideAllergy = true;
                this.hideProblems = true;
                break;

            case PAGE_NAME.ApproveScriptMessagePatient.replace(".aspx", "").toLowerCase():
            case PAGE_NAME.DeluxeFeatureSelection.toLowerCase():
            case PAGE_NAME.Settings.toLowerCase():
            case PAGE_NAME.Reports.toLowerCase():
            case PAGE_NAME.ManageAccount.toLowerCase():
            case PAGE_NAME.Library.replace(".aspx", "").toLowerCase():
            case PAGE_NAME.MyProfile.replace(".aspx", "").toLowerCase():
            case PAGE_NAME.MessageQueueTx.replace(".aspx", "").toLowerCase():
            case PAGE_NAME.HomeAddress.toLowerCase():
                this.visible = false;
                break;

            default:                
                this.isEditable = false;
                this.isPharmaciesEditable = false;
                break;
        }
        this.isLoadingData = false;
    }

    UpdatePatientPharmacy() {
        this.ptSvc.GetPatientPharmacy().subscribe(pharmInfo => {
            this.ptSvc.SelectedPatient.LastPharmacyName = pharmInfo.LastPharmacyName;
            this.ptSvc.SelectedPatient.IsMoEpcs = pharmInfo.IsMoEpcs;
            this.ptSvc.SelectedPatient.MoreMailOrderPharmVisible = pharmInfo.MoreMailOrderPharmVisible;
            this.ptSvc.SelectedPatient.MoreRetailPharm = pharmInfo.MoreRetailPharm;
            this.ptSvc.SelectedPatient.IsRetailEpcs = pharmInfo.IsRetailEpcs;
            this.ptSvc.SelectedPatient.PrefMOP = pharmInfo.PrefMOP;
            this.ptSvc.SelectedPatient.RemMOPharmVisible = pharmInfo.RemMOPharmVisible;
            this.ptSvc.SelectedPatient.RemPharmacyVisible = pharmInfo.RemPharmacyVisible;
        }
        );
    }

    headerLinkClicked(url: string) {
        if (this.isPharmaciesEditable && this.isEditable)
            this.router.navigateByUrl(ROUTE_NAME.Scriptpad, { state: { navigateTo: url } });
    }
    private SetPatientHeaderEditStatus(patient: Patient) {
        this.allowPatientEdit = patient.AllowPatientEdit;
        this.isPharmaciesEditable = patient.AllowPharmacyEdit;
        this.patientHasMrn = patient.MRN !== undefined;
        this.isRestrictedUser = patient.IsRestrictedUser;
        this.IsVipPatient = patient.IsVipPatient;

        if (current_context.PageName.toLowerCase() === PAGE_NAME.SelectPatient.toLowerCase()) {
            if (patient.PatientID === null) {
                this.isEditable = false;
            } else {
                this.isEditable = true;
            }
        }
        else if (current_context.PageName.toLowerCase() === PAGE_NAME.ScriptPad.toLowerCase()) {
            this.isEditable = true;
        }
    }
    CleanPatientHeader() {
        //reseting header patient data
        this.ptSvc.ResetSelectedPatient();
        this.isLoadingData = true;
    }

    LoadPatientHeader(patientid: string) {
        this.isLoadingData = true;
        // what is getPatientHeaderSubscription used for, can we remove and refactor?
        if (this.getPatientHeaderSubscription !== undefined) {
            this.getPatientHeaderSubscription.unsubscribe();
        }

        //
        // todo: refactor this and select patient component, and maybe app componenet to have a single source of calling db to "load Patient"
        //

        if (patientid == null) {
            this.patientHeader = new Patient();
            this.visible = true;
            this.isLoadingData = false;
        } else {
            //load current selected patient 
            this.getPatientHeaderSubscription = this.ptSvc.GetPatientHeader(patientid).subscribe(
                tl => {
                    this.SetPatientData(tl);
                }
            );
        }
    }

    SetPatientData(patient: Patient) {
        this.ptSvc.SelectedPatient = patient;
        this.patientHasMrn = patient.MRN != null && patient.MRN !== undefined && patient.MRN.length > 2;
        this.SetPatientHeaderEditStatus(patient);
        if (current_context.PageName == PAGE_NAME.ApproveRefillTaskPage) {
            this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, null);
        }
        this.visible = true;
        this.isLoadingData = false;
    }

    SelectPatient(patientid: any) {
        this.visible = true;
        //this.isPageLoading = true;
        this.isLoadingData = false;
        //load current selected patient 
        this.ptSvc.GetPatientHeader(patientid).subscribe(patient => {
            this.isRestrictedUser = false;

            // is this patient Restricted for the logged in user?
            if (patient.IsRestrictedUser == true) {

                let data = {
                    Patient: patient,
                    isPrivacyPatient: true,
                    callback: () => this.LoadPatientHeader(null)
                }

                this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, data);

                this.isRestrictedUser = true;
            }
            else {
                this.ptSvc.SelectedPatient = patient;
                this.patientHasMrn = patient.MRN != null && patient.MRN !== undefined && patient.MRN.length > 2;
            }

            this.isLoadingData = false;
            //this.isPageLoading = false;

        }
        );
    }

    OpenPrivacyPopupOnReconcile(patientid: any) {
        this.visible = true;
        this.ptSvc.GetPatientHeader(patientid).subscribe(patient => {
            let data = {
                Patient: patient,
                isPrivacyPatient: true,
                callback: () => {
                    this.visible = false;                   
                }
            }

            // call event on privacy-override component to display the popup
            this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, data);

            this.isLoadingData = false;
        });
    }

    OpenPrivacyOverridePopup(patientid: any) {
        this.visible = true;
        //load current selected patient 
        this.ptSvc.GetPatientHeader(patientid).subscribe(patient => {
            if (patient.IsRestrictedUser == true) {
                let data = {
                    Patient: patient,
                    isPrivacyPatient: true,
                    callback: () => {
                        this.router.navigateByUrl(ROUTE_NAME.Tasks, { state: { navigateTo: PAGE_NAME.Tasks } });
                    }
                }
                this.evtSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, data);
            }
            else {
                this.ptSvc.SelectedPatient = patient;
                this.patientHasMrn = patient.MRN != null && patient.MRN !== undefined && patient.MRN.length > 2;
            }
            this.isLoadingData = false;
        }
        );
    }

    onChange(): void {
        this.cdRef.detectChanges();
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
    }

    openPopup(title, popupPage) {
        if (this.isEditable && this.patientHasMrn && !this.isRestrictedUser && this.allowPatientEdit) {
            this.ptSvc.PopupNavigation.PopupTitle = title;
            if (current_context.PageName.toLowerCase() === PAGE_NAME.ScriptPad.toLowerCase()) {
                popupPage += "?Mode=ViewOnly";
            } else {
                popupPage += "?Mode=Edit";
            }
            this.ptSvc.PopupNavigation.PopupSrc = popupPage;
            (<any>window)['isModalPopup'] = true;
        }
    }

    openAllergyPopup(title, page) {
        if (this.ptSvc.SelectedPatient.AllowAllergyEdit === true && this.isEditable) {
            this.ptSvc.PopupNavigation.PopupTitle = title;
            this.ptSvc.PopupNavigation.PopupSrc = page;
            this.ptSvc.PopupNavigation.OnDismissEventName = MODAL_DISMISS_EVENT.AddAllergyModal;
            (<any>window)['isModalPopup'] = true;
        }
    }

    openDiagnosisPopup(title, page) {
        if (this.ptSvc.SelectedPatient.AllowDiagnosisEdit === true && this.isEditable) {
            this.ptSvc.PopupNavigation.PopupTitle = title;
            this.ptSvc.PopupNavigation.PopupSrc = page;
            this.ptSvc.PopupNavigation.OnDismissEventName = MODAL_DISMISS_EVENT.AddDiagnosisModal;
            (<any>window)['isModalPopup'] = true;
        }
    }

    ToggleRealName(ev: Event) {
        this.showRealName = false;
        this.IsVipPatient = this.ptSvc.SelectedPatient.IsVipPatient;
        this.isRestrictedUser = this.ptSvc.SelectedPatient.IsRestrictedUser;

        if (this.IsVipPatient && !this.isRestrictedUser) {
            this.RealName = this.ptSvc.SelectedPatient.RealName;
            this.showRealName = true;
            ev.stopPropagation();
        }
    }

    showEditLink(obj) {
        if (this.isEditable) {
            $(obj.fromElement).find('.noEditRollover').css('display', 'inline').css('visibility', 'visible');
        }
    }

    hideEditLink(obj) {
        if (this.isEditable)
            $(obj.toElement).find('.noEditRollover').css('display', 'none').css('visibility', 'hidden');
    }

    deletePharmacy(type: number) {
        if (type == 1) //mail order
            this.pharmaService.DeleteSelectedMOPharmacy().subscribe(() => {
                this.pharmaModel = new PharmacyModel();;
                this.LoadPatientHeader(this.ptSvc.SelectedPatient.PatientID);

            });
        else if (type == 2)// retail
            this.pharmaService.DeleteSelectedRetailPharmacy().subscribe(() => {
                this.pharmaModel = new PharmacyModel();
                this.LoadPatientHeader(this.ptSvc.SelectedPatient.PatientID);
            });
    }

    openPharmaDetails(type: number) {
        if (type == 1) //mail order
            this.pharmaService.GetSelectedMOPharmacy().subscribe((response) => {
                this.pharmaModel = response;
                this.pharmaModalTitle = "Patient's Mail Order Pharmacy";
                $('#mdlMorePharma').modal();

            });
        else if (type == 2)// retail
            this.pharmaService.GetSelectedRetailPharmacy().subscribe((response) => {
                this.pharmaModel = response;
                this.pharmaModalTitle = "Patient's Retail Pharmacy";
                $('#mdlMorePharma').modal();
            });
    }

    actAlergyMoreClicked(ev: Event) {
        this.activeAlergies.OpenPopOver();
        ev.stopPropagation();
    }

    actProblemMoreClicked(ev: Event) {
        this.activeProblems.OpenPopOver();
        ev.stopPropagation();
    }

    actMedsMoreClicked(ev: Event) {
        this.activeMeds.OpenPopOver();
        ev.stopPropagation();
    }
}