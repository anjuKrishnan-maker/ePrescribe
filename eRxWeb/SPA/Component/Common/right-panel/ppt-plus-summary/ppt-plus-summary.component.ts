import { Component, ElementRef, ChangeDetectorRef, OnInit} from '@angular/core';
import { BaseComponent } from '../../base.component';
import { EventService, PptPlusService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, PptDetailContext } from '../../../../tools/constants';
import { PptPlusSummaryRequest, IPptPlusSummaryRequestEventArgs, GridRowSelectionStatus, MedicationDTO, SelectedMedicationRows, MedicationSelectedPayload } from '../../../../model/model.import.def';
import { Subscription } from 'rxjs';

@Component({
    selector: 'erx-ppt-plus-summary',
    templateUrl: './ppt-plus-summary.template.html',
    styleUrls: ['./ppt-plus-summary.style.css']
})
export class PPTPlusSummary extends BaseComponent implements OnInit{
    public summaryHtml: string;
    public medName: string;
    public medExtension: string;
    public ShowEmptySummaryScreen: boolean;
    public ShowNoDetailsOverlay: boolean = false;
    public IsLoading: boolean;
    public RetrievePricingInfoTimeout: any;
    public PreviouslySelectedRows: any[];
    public CurrentMedIndex: string;
    public displaydetails: boolean = false;
    
    private summarySubscription: Subscription;    
    constructor(private cd: ElementRef,
        private chgd: ChangeDetectorRef,

        private svE: EventService,
        private svPptPlus: PptPlusService ) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationSelected, (val: MedicationSelectedPayload) => {
            if (current_context.UserPreference.IsPPTPlusEnabled) {

                this.InitiatePricingInquiry(<IPptPlusSummaryRequestEventArgs>{ PptPlusSummaryRequests: val.PptPlusSummaryRequest });
            }
        });


        this.svE.subscribeEvent(EVENT_COMPONENT_ID.PptPlusSummaryUpdate, (val: any) => {
            if (current_context.UserPreference.IsPPTPlusEnabled) {

                for (var item in val.MedSearchIndexes) {
                    if (val.MedSearchIndexes[item] == this.CurrentMedIndex) {
                        this.InitiatePricingInquiryUsingSessionData({ MedSearchIndex: val.MedSearchIndexes[item] });
                    }
                    else {
                        this.svPptPlus.InitiatePricingInquiryUsingSessionData({ MedSearchIndex: val.MedSearchIndexes[item] }).subscribe();
                    }
                }
            }
        });
            
   
        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationDeSelected,() => {
            this.SetDefaultData();
        });

        [EVENT_COMPONENT_ID.OnMedicationSelected, EVENT_COMPONENT_ID.OnMedicationDeSelected]
            .map((eventKey: string) => {
                this.svE.subscribeEvent(eventKey, (val: (MedicationDTO & SelectedMedicationRows) | null) => {
                    this.checkPreviouslySelectedRows(val)
                });
            });


      

    }

    public checkPreviouslySelectedRows(val: (MedicationDTO & SelectedMedicationRows) | null) {
        if (val == null || val.isSelectedMedicationsModified)
            return;
        var removedIndexes = this.PreviouslySelectedRows && this.PreviouslySelectedRows.filter(item => val.selectedMedicationRowIndex.indexOf(item) < 0);
        this.PreviouslySelectedRows = val.selectedMedicationRowIndex;
        if (removedIndexes && removedIndexes.length > 0) {
            var data = {
                RemovedIndexes: removedIndexes
            };
            this.svPptPlus.RemoveUnselectedRows(data).subscribe(_ => { });
        }
    }

    ngOnInit() {
        this.SetDefaultData();        
    }

    SetDefaultData() {
        this.summaryHtml = "";
        this.ShowEmptySummaryScreen = false;
        this.ShowNoDetailsOverlay = false;
        this.CurrentMedIndex = '-1';
        clearTimeout(this.RetrievePricingInfoTimeout);
        if (this.summarySubscription !== undefined) {
            this.summarySubscription.unsubscribe();
        }
    }   

    public InitiatePricingInquiry(data: IPptPlusSummaryRequestEventArgs) {

        let pptActiveMeds = data.PptPlusSummaryRequests.filter((x) => x.RowSelectionStatus == GridRowSelectionStatus.SelectActive);// [data.PptPlusSummaryRequests.length - 1];
        let pptMed: PptPlusSummaryRequest;
        if (pptActiveMeds !== undefined && pptActiveMeds.length > 0) {
            pptMed = pptActiveMeds[0];
            if (pptMed !== undefined) {
                this.medName = pptMed.Medname;
                this.medExtension = pptMed.MedExtension;
                this.CurrentMedIndex = pptMed.MedSearchIndex;
            }
        }
        this.StartLoadingSummary();
        this.svPptPlus.InitiatePricingInquiryBulk(data.PptPlusSummaryRequests).subscribe(response => {
            var pptPlusPricingRequestDelay: number = response;
                clearTimeout(this.RetrievePricingInfoTimeout);
                if (pptMed === undefined) {
                    pptMed = new PptPlusSummaryRequest();
                    pptMed.MedSearchIndex = this.CurrentMedIndex;
                };
                this.RetrievePricingInfoTimeout = setTimeout(() => {
                    this.RetrieveSummaryUi(pptMed)
                }, pptPlusPricingRequestDelay);
            }
        );
    }

    private InitiatePricingInquiryUsingSessionData(data) {
        this.StartLoadingSummary();
        this.svPptPlus.InitiatePricingInquiryUsingSessionData(data).subscribe(response => {
            var pptPlusPricingRequestDelay: number = response;
                clearTimeout(this.RetrievePricingInfoTimeout);
                this.RetrievePricingInfoTimeout = setTimeout(() => { this.RetrieveSummaryUi(data) }, pptPlusPricingRequestDelay);
            }
        );
    }

    private RetrieveSummaryUi(data) {
        if (this.svPptPlus == undefined) return;
        if (current_context.UserPreference.IsPPTPlusEnabled) {
            if (this.summarySubscription !== undefined) {
                this.summarySubscription.unsubscribe();
            }
            this.summarySubscription = this.svPptPlus.RetrieveSummaryUi(data).subscribe(
                (response) => {
                    if (this.CurrentMedIndex == response.MedIndex) {
                        this.EndLoadingSummary();
                        if (response.SummaryHtml == undefined || jQuery.isEmptyObject(response.SummaryHtml) || response.SummaryHtml.trim() == "" ) {
                            this.ShowEmptySummaryScreen = true;
                        }
                        else {
                            this.summaryHtml = response.SummaryHtml;
                            this.ShowEmptySummaryScreen = false;
                        }
                    }
                }
            );
        }
    }

    private StartLoadingSummary() {
        if (current_context.UserPreference.IsPPTPlusEnabled) {
            this.summaryHtml = "";
            this.ShowEmptySummaryScreen = true;
            this.ShowNoDetailsOverlay = false;
            this.IsLoading = true;
        }
    }

    private EndLoadingSummary() {
        this.ShowEmptySummaryScreen = false;
        this.IsLoading = false;
    }

    private OpenDetails() {
        if (this.IsLoading === false) {            
            this.svPptPlus.ShouldShowDetail().subscribe(
                tl => {            
                    if (tl === true) {                                                                       
                        this.svE.invokeEvent(EVENT_COMPONENT_ID.PptPlusDetailsUpdate  , PptDetailContext.CandidateMed);
                    }
                    else {
                        this.ShowEmptySummaryScreen = true;
                        this.ShowNoDetailsOverlay = true;
                    }
                }
            );
        }
    }

    private CloseNoDetails() {
        this.ShowNoDetailsOverlay = false;
    }
  
}