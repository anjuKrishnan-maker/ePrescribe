import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core'
import { EventService, SelectMedicationService, ScriptPadservice, PatientService } from '../../../services/service.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, MessageControlType, PAGE_NAME, ROUTE_NAME } from '../../../tools/constants';
import {
    SelectMedicationMedModel, SelectMedicationRequestType, MessageModel, FormularyOverideArgs, FormularyOverideAction,
    MedCompletionHistoryArgs, MedCompletionHistoryAction, CancelRxDialogArgs, CancelRxActions, CancelRxItemModel
} from '../../../model/model.import.def';
import { ComponentNavigationEventArgs } from "../../../model/common/component-navigation.model";
import Constants = require("../../../tools/constants");
import { SelectMedicationFormularyOverrideComponent } from '../select-medication-formulary-override/select-medication-formulary-override.component';
import { UserActionDTO } from '../../../model/dto/user-action.dto';
import { Router } from '@angular/router';

@Component({
    selector: 'erx-select-medication-action',
    templateUrl: './select-medication-action-bar.template.html',
    styleUrls: ['./select-medication-action-bar.style.css']
})
export class SelectMedicationActionBarComponent {

    @Input() medFilter: SelectMedicationRequestType;
    @Input() hasDiagnosis: boolean;
    @Input() isMedSelectedFromGrid: boolean;
    @Output() isMedSelectedFromGridChange = new EventEmitter<boolean>();
    @Input() selectedMeds: SelectMedicationMedModel[];
    @Output() isDiagnosisRemoved = new EventEmitter();
    @Output() message = new EventEmitter<MessageModel>();
    hasScriptPadMed: boolean = false;
    @Output() OnActionEventStarted = new EventEmitter();
    @Output() selectSigPressed = new EventEmitter();
    @ViewChild(SelectMedicationFormularyOverrideComponent) formularyOverrideComponent: SelectMedicationFormularyOverrideComponent;

    constructor(private evntSvc: EventService, private medicationService: SelectMedicationService, private scriptSvc: ScriptPadservice, private ptSvc: PatientService,
        private router: Router) {
        this.evntSvc.subscribeEvent(EVENT_COMPONENT_ID.ToggleReviewScriptPadDisplay, (display: boolean) => {
            this.hasScriptPadMed = display;
        });
    }

    public NavigateToUrl(route: string, page: string) {
        // clear current data
        this.message.emit(null);        
        this.router.navigateByUrl(route, { state: { navigateTo: page} });
    }

    public NavigateToScriptPad() {
        let scriptpadurl = PAGE_NAME.ScriptPad + '?from=' + PAGE_NAME.RedirectToAngular + '?componentName=' + PAGE_NAME.SelectMedication;
        this.NavigateToUrl(ROUTE_NAME.Scriptpad, scriptpadurl.toLowerCase());
    }

    public NavigateToSelectPatient() {
       
        this.router.navigateByUrl(ROUTE_NAME.SelectPatient, {
            state: {
                patientInfo: null
            }
        });
    }

    public NavigateToSelectDiagnosis() {
        this.NavigateToUrl(ROUTE_NAME.SelectDiagnosis, PAGE_NAME.SelectDiagnosis.toLowerCase());
    }
       

    clearDiagnosis() {
        this.hasDiagnosis = false;
        this.medicationService.ClearSelectedDx().subscribe();
        this.isDiagnosisRemoved.emit(true);
    }
    actionEventStarted() {
        this.OnActionEventStarted.emit();
    }
    AddToScriptPad() {
       
        this.medicationService.AddToScriptPad(this.selectedMeds).subscribe(data => {
            if (data.MessageModel !== undefined && data.MessageModel !== null && data.MessageModel.Message != undefined) {
                this.message.emit(data.MessageModel);
            }
            else if (data.ReturnAction == Constants.SelectMedicationReturnAction.ShowFormulary) {
                let foData = {
                    callback: (foa: FormularyOverideArgs) => { this.AddToScriptPad_FormularyOverRideCallback(foa) }
                }
                this.formularyOverrideComponent.loadFormularyOverride(foData);
            }
            else {
                this.AddToScriptPad_RefreshUI();
            }       
        });

    }

    AddToScriptPad_RefreshUI() {
        this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.ReloadScriptPadPanel, null);
        this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.OnScriptPadMedicationAdded, null);
        this.isMedSelectedFromGrid = false;
        this.isMedSelectedFromGridChange.emit(this.isMedSelectedFromGrid);
    }

    AddToScriptPad_FormularyOverRideCallback(data: FormularyOverideArgs) {
        if (data.Action == FormularyOverideAction.Process) {
            
            this.medicationService.AddMedToScript().subscribe(data => {
                this.AddToScriptPad_RefreshUI();
            
            });
        }
    }

    AddAndReview() {
        
        this.medicationService.AddAndReview(this.selectedMeds).subscribe(data => {        
            if (data.MessageModel !== undefined && data.MessageModel !== null && data.MessageModel.Message != undefined) {
                this.message.emit(data.MessageModel);
            }
            else if (data.ReturnAction == Constants.SelectMedicationReturnAction.ShowFormulary) {
                let foData = {
                    callback: (foa: FormularyOverideArgs) => { this.AddAndReview_FormularyOverRideCallback(foa) }
                }
                this.formularyOverrideComponent.loadFormularyOverride(foData);
            }
            else if (data.ScriptPadModel !== undefined && data.ScriptPadModel.showMediHistoryCompletionPopUp) {
                let mchData = {
                    callback: (mcha: MedCompletionHistoryArgs) => { this.AddAndReview_MedHistoryCompletionCallback(mcha) },
                    MedCompletionHistory: data.ScriptPadModel.MedCompletionHistory
                }
                this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.ReviewScriptOpenModal, mchData);
            }
            else {
                this.NavigateToScriptPad();
            }
        });
    }

    AddAndReview_FormularyOverRideCallback(data: FormularyOverideArgs) {
        if (data.Action == FormularyOverideAction.Process) {
           
            this.medicationService.GetScriptPadMedicationHistory().subscribe(data => {
                if (data.showMediHistoryCompletionPopUp) {
                    let mchData = {
                        callback: (mhca: MedCompletionHistoryArgs) => { this.AddAndReview_MedHistoryCompletionCallback(mhca) },
                        MedCompletionHistory: data.MedCompletionHistory
                    }
                    this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.ReviewScriptOpenModal, mchData);
           
                }
                else {
                    this.medicationService.AddMedToScript().subscribe(data => {           
                        this.NavigateToScriptPad();
                    });
                }

            });
        }
    }

    AddAndReview_MedHistoryCompletionCallback(args: MedCompletionHistoryArgs) {
       
        switch (args.Action) {
            case MedCompletionHistoryAction.CompleteAndContinue: {
                this.ReloadPatientMedications();
                if (args.Data.CancelRxScripts.length > 0) {
                    let crdaData = {
                        callback: (crda: CancelRxDialogArgs) => { this.AddAndReview_CompletedRxCancelRxCallback(crda) },
                        CancelRxItems: args.Data.CancelRxScripts.map(item => new CancelRxItemModel(item.RxId, item.Medication, item.OriginalNewRxTrnsCtrlNo))
                    }
                    this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.CompletedRxCancelRxOpen, crdaData);       
                }
                else {
                    this.medicationService.AddMedToScript().subscribe(data => {
                        this.NavigateToScriptPad();
                    });
                }
                break;
            }
            case MedCompletionHistoryAction.Continue: {
                 this.medicationService.AddMedToScript().subscribe(data => {
                    this.NavigateToScriptPad();
                });
                break;
            }
            default: {               
                break;
            }
        }
    }
    public AddAndReview_CompletedRxCancelRxCallback(args: CancelRxDialogArgs) {
        
        if (args.Action === CancelRxActions.ContinueWithoutCancelRx || args.Action === CancelRxActions.ContinueWithCancelRx) {
            this.medicationService.AddMedToScript().subscribe(data => {
                this.NavigateToScriptPad();
            });
        }
        else {
        
        }
    }

    ReviewScriptPad() {
        
        this.scriptSvc.ReviewScriptPad().subscribe(val => {
            if (val.showMediHistoryCompletionPopUp) {
        
                let mchcData = {
                    callback: (mhca: MedCompletionHistoryArgs) => { this.Review_MedHistoryCompletionCallback(mhca) },
                    MedCompletionHistory: val.MedCompletionHistory
                }
                this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.ReviewScriptOpenModal, mchcData);
            }
            else {
                this.NavigateToScriptPad();
            }
        });
    }

    Review_MedHistoryCompletionCallback(args: MedCompletionHistoryArgs) {
        switch (args.Action) {
            case MedCompletionHistoryAction.CompleteAndContinue: {
                this.ReloadPatientMedications();
                if (args.Data.CancelRxScripts.length > 0) {
                    let crdaData = {
                        callback: (crda: CancelRxDialogArgs) => { this.Review_CompletedRxCancelRxCallback(crda) },
                        CancelRxItems: args.Data.CancelRxScripts.map(item => new CancelRxItemModel(item.RxId, item.Medication, item.OriginalNewRxTrnsCtrlNo))
                    }
                    this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.CompletedRxCancelRxOpen, crdaData);
                }
                else {
                    this.NavigateToScriptPad();
                }
                break;
            }
            case MedCompletionHistoryAction.Continue: {
                this.NavigateToScriptPad();
                break;
            }
            default: {
                break;
            }
        }
    }

    public Review_CompletedRxCancelRxCallback(args: CancelRxDialogArgs) {
        if (args.Action === CancelRxActions.ContinueWithoutCancelRx || args.Action === CancelRxActions.ContinueWithCancelRx) {
            this.NavigateToScriptPad();
        }
    }

    SelectSIG() {
        this.selectSigPressed.emit();
        
        this.medicationService.SelectSig(this.selectedMeds[0]).subscribe(data => {
            if (data.MessageModel !== undefined && data.MessageModel !== null && data.MessageModel.Message != undefined) {
        
                this.message.emit(data.MessageModel);
            } else if (data.ReturnAction == Constants.SelectMedicationReturnAction.ShowFormulary) {
        
                let foData = {
                    callback: (foa: FormularyOverideArgs) => { this.SelectSig_FormularyOverRideCallback(foa) }
                }
                this.formularyOverrideComponent.loadFormularyOverride(foData);
            } else {
                this.CompleteSelectSig();
            }
        });
    }

    SelectSig_FormularyOverRideCallback(data: FormularyOverideArgs) {
        if (data.Action == FormularyOverideAction.Process) {
            this.CompleteSelectSig();
        }
    }

    CompleteSelectSig() {
        
        this.medicationService.CompleteSelectSig().subscribe(data => {
        
            let sigRoute = data.toLowerCase() === 'sig.aspx' ? ROUTE_NAME.Sig : ROUTE_NAME.NurseSig;
            this.NavigateToUrl(sigRoute, data.toLowerCase());
        });
    }

    public ReloadPatientMedications() {
        this.evntSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientMedicationsUpdated, null);
    }
}