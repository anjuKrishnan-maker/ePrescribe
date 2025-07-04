//testing framework
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
// import rxjs 
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { EventEmitter } from '@angular/core';
//import for test componet 
import { EventService, SelectMedicationService, CancelRxService, PatientService, ErrorService, ComponentCommonService, PatientMedHistoryService, ContentLoadService } from '../Services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID, MessageControlType } from '../tools/constants';
import {
    MessageIcon, MessageModel, PatientCoverageHeader as CoverageModel, SelectMedicationDataRequest, SelectMedicationRequestType, SelectMedicationMedModel, SelectMedicationSelections,
    SelectMedicationSearchModel, SelectMedicationSearchArgs, PatientRecordStatus, SelectMedicationGridPopulationCompletionArgs,
    PptPlusSummaryRequest, UpdatePatientSelectedCoverageRequest, CancelRxItemModel, CancelRxDialogArgs, CancelRxActions, ApiResponse,
    SendCancelRxRequestModel, SelectMedicationEventContext, SelectMedicationGridEvent, IPptPlusSummaryRequestEventArgs, GridRowSelectionStatus, SelectMedicationStartUpModel, SelectMedicationGridModel, SelectedMedicationRows
} from '../model/model.import.def';
import { SelectMedicationComponent } from '../component/select-medication/select-medication.component';
import { SelectMedicationGridComponent } from '../component/select-medication/select-medication-grid/select-medication-grid.component';
import { PatientMedHistoryComponent } from '../component/common/shared/patient-medication-history/patient-medication-history.component';
import { RouterTestingModule } from "@angular/router/testing";
//feature
describe('SelectMedication Coverage Tests', () => {
    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc, null);
    let component: SelectMedicationComponent;
    let GridComponent: SelectMedicationGridComponent;
    let selectMedicationGrid: SelectMedicationGridModel = new SelectMedicationGridModel();
    selectMedicationGrid.IsAllMedSearch = false;
    selectMedicationGrid.IsAllMedSearch = false;
    let meds: SelectMedicationMedModel[] = [
        {
        DDI: "001925",
        DrugName : "Aspirin",
        DrugExt : " 325 MG Tablet, TAKE 1 TABLET TWICE DAILY.",
        Quantity : 2,
        DAW : false,
        Refill : 2,
        DayOfSupply : 2,
        BenefitImageUrl : null,
        LevelOfPreferedness : null,
        FormularyImageUrl : 0,
        FormularyStatus : 3,
        Selected : false,
        Strength : "325",
        StrengthUOM : "MG",
        DosageForm : "Tablet",
        RouteofAdmin : "Oral",
        IsOTC : true,
        SIGID : "26577712 - d11e - 4204 - 86c0 - c9d22431a4d0",
        PackageSize : "1",
        PackageQuantity : "1",
        PackageUOM : "EA",
        PackageDescription : "EA",
        MedicationName : "Aspirin",
        RouteofAdminCode : "OR",
        DosageFormCode : "TABS",
        SIGText : "TAKE 1 TABLET TWICE DAILY.",
        SIGTypeID : 1,
        PriorAuth : false,
        NDC : "00363015712",
        IsCouponAvailable : false,
        IsSpecialtyMed : false,
        GPI : "64100010000315",
        PreDURDrugDrug : "N",
        PreDURDUP : "N",
        PreDURPAR : "N"
        }
    ];
    
    selectMedicationGrid.Meds = meds;

    selectMedicationGrid.Messages = null;
    selectMedicationGrid.PatientRecordStatus = null;
    selectMedicationGrid.RequestFor = null;

    //Mocking service
    class MockSelectMedicationService extends SelectMedicationService {        
        ClearSelectedDx() {
            return Observable.create((observer: Observer<void>) => {
                observer.next();
            });
        }
            
        GetSelectMedicationData() {            
            return Observable.create((observer: Observer<number>) => {
                observer.next(selectMedicationGrid);
            });
        }       
    }
    class MockCancelRxService extends CancelRxService {

    }
    class MockPatientService extends PatientService {

    }

    class MockContentLoadService extends ContentLoadService{

    }
    class MockPatientMedHistoryService extends PatientMedHistoryService {
        GetPatientMedHistory() {
            return Observable.create((observer: Observer<void>) => {
                observer.next();
            });
        }
    }
    class MockComponentCommonService extends ComponentCommonService {
        AddWindowFunction(key,fun) {

        }
    }

    class MockRouter extends RouterTestingModule {
        getCurrentNavigation() {
        }

    }
    //Runs once per test feature
    beforeAll(() => {

    });

    // runs before each scenario 
    beforeEach(async(() => {
        component = new SelectMedicationComponent(new MockSelectMedicationService(null, errorsvc), evnntSvc, new MockComponentCommonService(), new MockContentLoadService(null, errorsvc), new MockRouter());
        GridComponent = new SelectMedicationGridComponent(evnntSvc, new MockSelectMedicationService(null, errorsvc), errorsvc, new ComponentCommonService);
    }));

    xit('ResetSelectMedicationUI - Should Reset Coverage, DiagnosisInfo and MessageStrip', async(() => {
        //arrange

        //Act
        component.ResetSelectMedicationUI();

        //assert 
        expect(component.CoverageList).toBe(null);
        expect(component.diagnosisName).toBe("");
        expect(component.hasDiagnosis).toEqual(false);
        expect(component.StaticMessageStrip).toEqual([]);
    })); 

    it('SetDiagnosisInfo - Should Set Diagnosis Info', async(() => {
        //arrange
        let diagnosis = "Fever";
        //Act
        component.SetDiagnosisInfo(diagnosis);

        //assert 
        expect(component.diagnosisName).toEqual("Fever");
        expect(component.hasDiagnosis).toEqual(true);
    })); 

    it('SetDiagnosisInfo - Should Set hasDiagnosis To False When diagnosis empty', async(() => {
        //arrange
        let diagnosis = "";
        //Act
        component.SetDiagnosisInfo(diagnosis);

        //assert 
        expect(component.diagnosisName).toEqual("");
        expect(component.hasDiagnosis).toEqual(false);
    })); 

    it('RemoveDiagnosis - Should Remove Diagnosis', async(() => {
        //arrange
        let isDiagnosisRemoved = true;
        //Act
        component.RemoveDiagnosis(isDiagnosisRemoved);

        //assert 
        expect(component.hasDiagnosis).toEqual(false);
    })); 

    it('ShowPatientMedHistoryPopup - Should Return False', async(() => {
        //arrange
        component.patientMedHistoryComponent = new PatientMedHistoryComponent(evnntSvc, new MockPatientMedHistoryService(null, errorsvc))
      
        //Act
        let result = component.showPatientMedHistoryPopup();

        //assert 
        expect(result).toEqual(false);
    }));

    it('CreateRightPanelMedicationDTO - Should Create The MedicationDTO object', async(() => {
        //arrange
        let selectedMeds: Array<SelectMedicationMedModel> = [];
        let fssData: SelectMedicationSelections = {
            SelectedMeds: selectedMeds,
            EventContext: {
                CurrentMed: new SelectMedicationMedModel(),
                CurrentMedIndex: 0,
                GridEvent: SelectMedicationGridEvent.MedSelected
            }
        };
        fssData.EventContext.CurrentMed.DDI = '033382';
        fssData.EventContext.CurrentMed.IsSpecialtyMed = true;
        fssData.EventContext.CurrentMed.FormularyStatus = 2;
        fssData.EventContext.CurrentMed.DrugName = 'Aspirin';
        
        let selectedMedicationRows = <SelectedMedicationRows>{
            selectedMedicationRowIndex: [0]
        };

        let pptPlusSummaryRequest: PptPlusSummaryRequest[] = [{
            DDI: "033382",
            DaysSupply: "30",
            GPPC: "",
            Index: 1,
            IsDaw: "false",
            MedExtension: "10 MG Tablet",
            MedSearchIndex: "1",
            Medname: "Aspirin",
            PackQuantity: "1",
            PackSize: "1",
            PackUom: "EA",
            Quantity: "60",
            Refills: "1",
            RowSelectionStatus: 1,
            Strength: "10",
            RemovedIndexes: [""]
        }];

        //Act
        let result = component.createMedicineSelectedInfo(fssData, selectedMedicationRows, pptPlusSummaryRequest, false);

        //assert 
        expect(result.DDI).toEqual('033382');
        expect(result.formularyStatus).toEqual(2);
        expect(result.medName).toEqual('Aspirin');
        expect(result.taskScriptMessageId).toEqual('');
        expect(result.isSpecialtyMedication).toEqual(true);
    }));

    it('CreateRightPanelMedicationDTO - Should Create The MedicationDTO object - When Formulary Status is null and drugname is undefined', async(() => {
        //arrange

        let selectedMeds: Array<SelectMedicationMedModel> = [];
        let fssData: SelectMedicationSelections = {
            SelectedMeds: selectedMeds,
            EventContext: {
                CurrentMed: new SelectMedicationMedModel(),
                CurrentMedIndex: 0,
                GridEvent: SelectMedicationGridEvent.MedSelected
            }
        };
        fssData.EventContext.CurrentMed.DDI = '033382';
        fssData.EventContext.CurrentMed.IsSpecialtyMed = null;
        fssData.EventContext.CurrentMed.FormularyStatus = NaN;
        fssData.EventContext.CurrentMed.DrugName = undefined;

        let selectedMedicationRows = <SelectedMedicationRows>{
            selectedMedicationRowIndex: [0]
        };

        let pptPlusSummaryRequest: PptPlusSummaryRequest[] = [{
            DDI: "033382",
            DaysSupply: "30",
            GPPC: "",
            Index: 1,
            IsDaw: "false",
            MedExtension: "10 MG Tablet",
            MedSearchIndex: "1",
            Medname: "Aspirin",
            PackQuantity: "1",
            PackSize: "1",
            PackUom: "EA",
            Quantity: "60",
            Refills: "1",
            RowSelectionStatus: 1,
            Strength: "10",
            RemovedIndexes: [""]
        }];

        //Act
        let result = component.createMedicineSelectedInfo(fssData, selectedMedicationRows, pptPlusSummaryRequest, false);

        //assert 
        expect(result.DDI).toEqual('033382');
        expect(result.formularyStatus).toEqual(0);
        expect(result.medName).toEqual('');
        expect(result.taskScriptMessageId).toEqual('');
        expect(result.isSpecialtyMedication).toEqual(false);
    }));
    
    it('should have Medication Grid, when the meds are not null', async(() => { 

        //arrange
        let selectMedicationDateRequest: SelectMedicationDataRequest = new SelectMedicationDataRequest();
        selectMedicationDateRequest.PatientRecordStatus = 1;
        selectMedicationDateRequest.RequestFor = 1;

        //act
        GridComponent.SelectMedDataRequest = selectMedicationDateRequest; 

        //assert
        expect(GridComponent.gridData.Meds.length).toEqual(1);
        expect(GridComponent.gridData.Meds[0].DDI).toEqual(meds[0].DDI);
        expect(GridComponent.gridData.Meds[0].DrugName).toEqual(meds[0].DrugName);
        expect(GridComponent.gridData.Meds[0].DrugExt).toEqual(meds[0].DrugExt);
        expect(GridComponent.gridData.Meds[0].Quantity).toEqual(meds[0].Quantity);
        expect(GridComponent.gridData.Meds[0].DAW).toEqual(meds[0].DAW);
        expect(GridComponent.gridData.Meds[0].Refill).toEqual(meds[0].Refill);
        expect(GridComponent.gridData.Meds[0].DayOfSupply).toEqual(meds[0].DayOfSupply);
        expect(GridComponent.gridData.Meds[0].BenefitImageUrl).toEqual(meds[0].BenefitImageUrl);
        expect(GridComponent.gridData.Meds[0].LevelOfPreferedness).toEqual(meds[0].LevelOfPreferedness);
        expect(GridComponent.gridData.Meds[0].FormularyImageUrl).toEqual(meds[0].FormularyImageUrl);
        expect(GridComponent.gridData.Meds[0].FormularyStatus).toEqual(meds[0].FormularyStatus);
        expect(GridComponent.gridData.Meds[0].Selected).toEqual(meds[0].Selected);
        expect(GridComponent.gridData.Meds[0].Strength).toEqual(meds[0].Strength);
        expect(GridComponent.gridData.Meds[0].StrengthUOM).toEqual(meds[0].StrengthUOM);
        expect(GridComponent.gridData.Meds[0].DosageForm).toEqual(meds[0].DosageForm);
        expect(GridComponent.gridData.Meds[0].RouteofAdmin).toEqual(meds[0].RouteofAdmin);
        expect(GridComponent.gridData.Meds[0].IsOTC).toEqual(meds[0].IsOTC);
        expect(GridComponent.gridData.Meds[0].SIGID).toEqual(meds[0].SIGID);
        expect(GridComponent.gridData.Meds[0].PackageSize).toEqual(meds[0].PackageSize);
        expect(GridComponent.gridData.Meds[0].PackageQuantity).toEqual(meds[0].PackageQuantity);
        expect(GridComponent.gridData.Meds[0].PackageUOM).toEqual(meds[0].PackageUOM);
        expect(GridComponent.gridData.Meds[0].PackageDescription).toEqual(meds[0].PackageDescription);
        expect(GridComponent.gridData.Meds[0].MedicationName).toEqual(meds[0].MedicationName);
        expect(GridComponent.gridData.Meds[0].RouteofAdminCode).toEqual(meds[0].RouteofAdminCode);
        expect(GridComponent.gridData.Meds[0].SIGText).toEqual(meds[0].SIGText);
        expect(GridComponent.gridData.Meds[0].SIGTypeID).toEqual(meds[0].SIGTypeID);
        expect(GridComponent.gridData.Meds[0].PriorAuth).toEqual(meds[0].PriorAuth);
        expect(GridComponent.gridData.Meds[0].NDC).toEqual(meds[0].NDC);
        expect(GridComponent.gridData.Meds[0].IsSpecialtyMed).toEqual(meds[0].IsSpecialtyMed);
        expect(GridComponent.gridData.Meds[0].GPI).toEqual(meds[0].GPI);
        expect(GridComponent.gridData.Meds[0].PreDURDrugDrug).toEqual(meds[0].PreDURDrugDrug);
        expect(GridComponent.gridData.Meds[0].PreDURDUP).toEqual(meds[0].PreDURDUP);
        expect(GridComponent.gridData.Meds[0].PreDURPAR).toEqual(meds[0].PreDURPAR);                                              
    }));

    it('SelectMedicationMessageStrip should not contain nonCSTaskToCSMed and CSTaskSearchDisabled Message', async(() => {

        component.SelectMedicationMessageStrip.push(new MessageModel(
            " You cannot respond to a Non-Controlled Substance Change Request with a Controlled Substance Medication",
            MessageIcon.Error,
            "NonCSTaskToCSMed",
            false
        ));

        component.SelectMedicationMessageStrip.push(new MessageModel(
            "Please select medication from the search results below.  If a change is needed, please deny the task and write a new prescription.",
            MessageIcon.Information,
            "CSTaskSearchDisabledMessage",
            false
        ));

        //act
        component.selectSigPressedAction();

        //assert
        expect(component.SelectMedicationMessageStrip.some(e => e.Tag == "NonCSTaskToCSMed" || e.Tag == "CSTaskSearchDisabledMessage")).toEqual(false);
      
    }));
    
});


