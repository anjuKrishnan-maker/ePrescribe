import { Component, ElementRef, ViewChild } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { SafeHtml } from '@angular/platform-browser';
import { EventService, PatientService, PDMPService, ComponentCommonService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { PdmpPatientResponseStatus, PdmpUISummary, PatientContextDTO } from '../../../../model/model.import.def';
import { Subscription } from 'rxjs';
import ObjectUtil from '../../../../tools/Utils/object.extension';
import { PDMPDetailsComponent } from '../../../shared/pdmp-details/pdmp-details.component';


@Component({
    selector: 'erx-pdmp-summary',
    templateUrl: './pdmp-summary.template.html',
    styleUrls: ['./pdmp-summary.style.css']
})
export class PDMPSummary extends BaseComponent {
    public PdmpUISummary: PdmpUISummary;
    private summarySubscription: Subscription;
    public pdmpSummaryClicked = false;
    @ViewChild(PDMPDetailsComponent) pdmpDetailsComponent: PDMPDetailsComponent;

    constructor(private cd: ElementRef, private svE: EventService, private compSvc: ComponentCommonService, private pdmpSvc: PDMPService, private patientService: PatientService) {
        super(cd, svE);

        this.SetDefaultData();
        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnPatientSelected, (patientContext: PatientContextDTO) => {
            if (current_context.UserPreference.IsShowPDMP  &&  patientContext.isPrivacyPatient != true) {
                this.SetDefaultData();
                this.StartLoading();
                if (this.IsPatientSelected()) {
                    this.summarySubscription = this.pdmpSvc.GetPDMPSummary().subscribe(
                        (data) => {
                            this.EndLoading();
                            this.PdmpUISummary = data;
                        });
                }
                else {
                    this.EndLoading();
                }
            }
        });

        this.SubscibeEvents();
    }

    private SubscibeEvents(): void {
        [          
            EVENT_COMPONENT_ID.RightPanelLoaded,
            EVENT_COMPONENT_ID.RefreshPatientHeader        
        ]
            .map((eventKey: string) => {
                this.svE.subscribeEvent(eventKey, () => {
                    this.ResetOnNoPatientSelection();
                    this.CheckUIVisiblity();
                });
            });
    }

    private IsPatientSelected(): boolean {
        let selectedPatient = this.patientService.SelectedPatient;
        return !ObjectUtil.IsEmpty(selectedPatient) && selectedPatient.PatientID != null && selectedPatient.PatientID.length > 0;
    }

    private ResetOnNoPatientSelection(): void {
        if (!this.IsPatientSelected())
            this.SetDefaultData();
    }

    private CheckUIVisiblity() {
        let shouldPdmpVisibleForPage = this.compSvc.CheckVisibility(this.id, current_context.PageName);
        this.ChangeVisiblity(current_context.UserPreference.IsShowPDMP && shouldPdmpVisibleForPage);
    }

    private SetDefaultData() {
        this.PdmpUISummary = null;
        if (this.summarySubscription !== undefined) {
            this.summarySubscription.unsubscribe();
        }
    }

    private ChangeVisiblity(visible: boolean) {
        if (visible) {
            this.Show();
        }
        else {
            this.Hide();
        }
    }

    private OpenDetails() {
        this.pdmpDetailsComponent.loadPDMPDetails();
    }
}