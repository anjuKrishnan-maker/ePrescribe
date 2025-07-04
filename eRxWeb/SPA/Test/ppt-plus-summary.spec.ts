//testing framework
import { async } from '@angular/core/testing';
// import rxjs 
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';

//import for test componet 
import { EventService, PptPlusService, PatientService, ErrorService } from '../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../tools/constants';
import { PptPlusSummaryRequest, IPptPlusSummaryRequestEventArgs, GridRowSelectionStatus, MedicationDTO, SelectedMedicationRows } from '../model/model.import.def';
import { PPTPlusSummary } from '../component/common/right-panel/ppt-plus-summary/ppt-plus-summary.component';
import { request } from 'http';

//feature
describe('PPT Plus Summary Unit Tests', () => {
    let component: PPTPlusSummary;
    let evnntSvc: EventService = new EventService();
    let pptRequest: PptPlusSummaryRequest;
    let medName: string;
    let strength: string;
    let InitiatePricingInquiryResponse: any;
    let response = {
        SummaryHtml: undefined
    }

    //Mocking service
    class MockPptPlusService extends PptPlusService {
        constructor() {
            super(null);
        }
        InitiatePricingInquiryBulk (data) {
            return Observable.create((observer: Observer<number>) => {
                observer.next(InitiatePricingInquiryResponse);
            });
        }
        RetrieveSummaryUi(data) {
            return Observable.create((observer: Observer<any>) => {
                observer.next(JSON.stringify( response));
            });
        }
        RemoveUnselectedRows(data) {
            return Observable.create((observer: Observer<any>) => {
                observer.next();
            });
        }
    }

    const pptSummary = new MockPptPlusService();

    //Runs once per test feature
    beforeAll(() => {
        component = new PPTPlusSummary(null, null, evnntSvc, pptSummary);
    });

    // runs before each scenario 
    beforeEach(async(() => {
       ///todo
    }));

    //test scenario
    it('should update summary panel when initial pricing inquiry is called', async(() => {
        //arrange
        let pptRequest = new PptPlusSummaryRequest();
        pptRequest.Medname = "Amoxicillin";
        pptRequest.MedSearchIndex = "3";
        pptRequest.MedExtension = "125 MG Tablet Chewable";
        pptRequest.RowSelectionStatus = GridRowSelectionStatus.SelectActive;
        component.RetrievePricingInfoTimeout = null;
        //act
        component.InitiatePricingInquiry(<IPptPlusSummaryRequestEventArgs>{ PptPlusSummaryRequests: [pptRequest] });

        //assert 
        expect(component.medName).toEqual(pptRequest.Medname);
        expect(component.medExtension).toEqual(pptRequest.MedExtension);
        expect(component.CurrentMedIndex).toEqual(pptRequest.MedSearchIndex);

    }));

    it('should not call removeUnselectedRows when value is null', async(() => {

        //arrange
        spyOn(pptSummary, 'RemoveUnselectedRows').and.returnValue({ subscribe: () => { } });

        //act
        component.checkPreviouslySelectedRows(null);

        //assert
        expect(pptSummary.RemoveUnselectedRows).toHaveBeenCalledTimes(0);

    }));

    it('previouslySelectedRows should not be null after selecting medicine', async(() => {

        //arrange
        spyOn(pptSummary, 'RemoveUnselectedRows').and.returnValue({ subscribe: () => { } });

        let val = <MedicationDTO & SelectedMedicationRows>{
            DDI: "024712",
            formularyStatus: 1,
            isSelectedMedicationsModified: false,
            isSpecialtyMedication: false,
            medName: "Amoxicillin",
            selectedMedicationRowIndex: [1]
        }

        //act
        component.checkPreviouslySelectedRows(val);

        //assert
        expect(pptSummary.RemoveUnselectedRows).toHaveBeenCalledTimes(0);
        expect(component.PreviouslySelectedRows.length).toEqual(1);
    }));

    it('should call removeunselectedRows after MedicationDeSelected ', async(() => {

        //arrange
        
        spyOn(pptSummary, 'RemoveUnselectedRows').and.returnValue({ subscribe: () => { } });

        let val = <MedicationDTO & SelectedMedicationRows>{
            selectedMedicationRowIndex: []
        }

        component.PreviouslySelectedRows = [1]

        //act
        component.checkPreviouslySelectedRows(val);

        //assert
        expect(pptSummary.RemoveUnselectedRows).toHaveBeenCalledTimes(1);
    }));
});


