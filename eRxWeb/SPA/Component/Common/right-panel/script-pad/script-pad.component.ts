import { Component, ElementRef } from '@angular/core';

import { BaseComponent } from '../../base.component';

import { ScriptPadservice, EventService, ComponentCommonService, PatientService, SelectMedicationService } from '../../../../services/service.import.def';
import { ScriptPadModel, MedCompletionHistoryArgs, MedCompletionHistoryAction, CancelRxDialogArgs, CancelRxActions, CancelRxItemModel, RightPanelPayload} from '../../../../model/model.import.def';

import { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID, PAGE_NAME, ROUTE_NAME } from '../../../../tools/constants';
import { Router } from '@angular/router';
@Component({
    selector: 'erx-script-pad',
    templateUrl: './script-pad.template.html'
})
export class ScriptPadComponent extends BaseComponent {
    scriptPadItems: ScriptPadModel[]=[];
    isEditable: boolean = true;
    constructor(private scriptSvc: ScriptPadservice,
        cd: ElementRef, private svE: EventService,
        private compSvc: ComponentCommonService, private ptSvc: PatientService, private router: Router, private medicationService: SelectMedicationService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            if (compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.scriptPadItems = [];
                this.StartLoading();
            }
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (compSvc.CheckVisibility(this.id, current_context.PageName)) {

                this.processScriptPadPayload(rightPanelPayload.ScriptPadPayload);
                if (current_context.PageName.toLowerCase() === PAGE_NAME.SigPage.toLowerCase()
                    || current_context.PageName.toLowerCase() === PAGE_NAME.NurseSig.toLowerCase()) {
                    this.isEditable = false;
                }
                else {
                    this.isEditable = true;
                }
            }
            else {
                this.Hide();
            }
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ReloadScriptPadPanel,() => {
            this.LoadScriptPad();
        });
    }

    CallChildFunction(arg: any) {
        this.Hide();
        this.LoadScriptPad();

    }

    LoadScriptPad() {
        this.scriptSvc.GetCurrentScripts(current_context.PageName).subscribe(response => {
            this.processScriptPadPayload(response);
        });
    }

    processScriptPadPayload(scriptPad: ScriptPadModel[]) {
        this.scriptPadItems = scriptPad;
        if (scriptPad !== undefined && scriptPad.length > 0) {
            this.Show();
            this.svE.invokeEvent(EVENT_COMPONENT_ID.ToggleReviewScriptPadDisplay, true);
        }
        else {
            this.Hide();
            this.svE.invokeEvent(EVENT_COMPONENT_ID.ToggleReviewScriptPadDisplay, false);
        }
        this.EndLoading();
    }

    RemoveScript(rxId: string) {
        this.scriptSvc.DeleteScript(rxId).subscribe(() => {
            this.LoadScriptPad();

            if (typeof window.frames[0].scriptPadRefresh === "function") {
                window.frames[0].scriptPadRefresh();
            }
        }
        );
    }

    ReviewScriptPad() {
       
        this.scriptSvc.ReviewScriptPad().subscribe(val => {       
            if (val.showMediHistoryCompletionPopUp) {
                let mchcData = {
                    callback: (mhca: MedCompletionHistoryArgs) => { this.Review_MedHistoryCompletionCallback(mhca) },
                    MedCompletionHistory: val.MedCompletionHistory
                }
                this.svE.invokeEvent(EVENT_COMPONENT_ID.ReviewScriptOpenModal,mchcData);
            }
            else {
                this.navigateToScriptPad();
            }
        });
    }

    private navigateToScriptPad() {
        let scriptpadurl = PAGE_NAME.ScriptPad + '?from=' + PAGE_NAME.RedirectToAngular + '?componentName=' + PAGE_NAME.SelectMedication;
        this.router.navigateByUrl(ROUTE_NAME.Scriptpad, { state: { navigateTo: scriptpadurl } });
    }
    Review_MedHistoryCompletionCallback(args: MedCompletionHistoryArgs) {
        switch (args.Action) {
            case MedCompletionHistoryAction.CompleteAndContinue: {
                this.ReloadPatientHeader();
                if (args.Data.CancelRxScripts.length > 0) {
                    let crdaData = {
                        callback: (crda: CancelRxDialogArgs) => { this.Review_CompletedRxCancelRxCallback(crda) },
                        CancelRxItems: args.Data.CancelRxScripts.map(item => new CancelRxItemModel(item.RxId, item.Medication, item.OriginalNewRxTrnsCtrlNo))
                    }
                    this.svE.invokeEvent(EVENT_COMPONENT_ID.CompletedRxCancelRxOpen,crdaData);
                }
                else {
                    this.navigateToScriptPad();
                }
                break;
            }
            case MedCompletionHistoryAction.Continue: {
                this.navigateToScriptPad();
                break;
            }
            default: {
                break;
            }
        }
    }

    public Review_CompletedRxCancelRxCallback(args: CancelRxDialogArgs) {
        if (args.Action === CancelRxActions.ContinueWithoutCancelRx || args.Action === CancelRxActions.ContinueWithCancelRx) {
            this.router.navigateByUrl(ROUTE_NAME.Scriptpad, { state: { navigateTo: PAGE_NAME.ScriptPad } });
        }
    }

    EditRx(selectedMedicine: string) {      
        this.medicationService.CompleteSelectSig().subscribe(data => {      
            let sigRoute = data.toLowerCase() === 'sig.aspx' ? ROUTE_NAME.Sig : ROUTE_NAME.NurseSig;
            this.router.navigateByUrl(sigRoute, { state: { navigateTo: selectedMedicine } });
        });
    }

    public ReloadPatientHeader() {
        this.ptSvc.GetPatientHeader(this.ptSvc.SelectedPatient.PatientID).subscribe(
            tl => {
                this.ptSvc.SelectedPatient = tl;
            }
        );
    }   
}
