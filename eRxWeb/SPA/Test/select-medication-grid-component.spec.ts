//testing framework
import { async } from '@angular/core/testing';
// import rxjs 
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { EventEmitter } from '@angular/core';
//import for test componet 
import { EventService, ErrorService, SelectMedicationService, ComponentCommonService } from '../Services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../tools/constants';
import { GridRowSelectionStatus, SelectMedicationMedModel, SelectMedicationGridModel, SelectMedicationSelections, SelectMedicationGridEvent } from '../model/model.import.def';
import { SelectMedicationGridComponent } from '../component/select-medication/select-medication-grid/select-medication-grid.component';
import { ModalPopupControl } from '../component/shared/controls/modal-popup/modal-popup.control';
//feature
describe('select medication grid OnGridItemSelected testes', () => {
    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc, null);
    let compSvc: ComponentCommonService = new ComponentCommonService();
    let component: SelectMedicationGridComponent;
    //Mocking service
    class MockSelectMedicationService extends SelectMedicationService {


    }

    //Runs once per test feature
    beforeAll(() => {

    });

    // runs before each scenario 
    beforeEach(async(() => {
        component = new SelectMedicationGridComponent(evnntSvc, new MockSelectMedicationService(null, errorsvc), errorsvc, compSvc);
    }));

    it('should return value 0 if GridReset is called', async(() => {

        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();   
        selectedItem.DAW = false;
        selectedItem.DDI = "020813";
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem.DrugName = "Aspirin Test";
        selectedItem.Selected = false;
        selectedItem.Strength = "100";
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);

        // Act
        component.GridReset();
        
        //assert
        expect(component.gridData.Meds.length).toEqual(0);

    }));

    it('rowStatus should be notSelected after reset selection', async(() => {

        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.DAW = false;
        selectedItem.DDI = "020813";
        selectedItem.index = 0;
        selectedItem.DrugName = "Aspirin Test";
        selectedItem.Strength = "100";
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);

        // Act
        component.ResetSelection();

        //assert
        expect(component.gridData.Meds[0].Selected).toBeFalsy;
        expect(component.gridData.Meds[0].RowStatus).toEqual(GridRowSelectionStatus.NotSelected);

    }));
   
    //test scenario
    it('should emit OnMedSelected output event and GridEvent should be MedSelected when OnGridItemSelected called', async(() => {
        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem.DrugName = "Aspirin Test";
        selectedItem.Selected = false;
        selectedItem.Strength = "100";
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = new Event("click");
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridItemSelected(selectedItem, 0, event)

        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(1);
        expect(component.gridData.Meds[0].RowStatus).toEqual(GridRowSelectionStatus.SelectActive);
        expect(selectionCount).toEqual(1);
        expect(gridEvent).toEqual(SelectMedicationGridEvent.MedSelected);

    }));

    it('should set the row status as GridRowSelectionStatus.SelectActive and GridEvent should be MedSelected when OnGridItemSelected called and the row already selected', async(() => {
        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.Selected;
        selectedItem.Selected = true;
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = new Event("click");
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridItemSelected(selectedItem, 0, event)

        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(1);
        expect(component.gridData.Meds[0].RowStatus).toEqual(GridRowSelectionStatus.SelectActive);
        expect(selectionCount).toEqual(1);
        expect(gridEvent).toEqual(SelectMedicationGridEvent.MedSelected);

    }));
    it('should deselect row  and GridEvent should be MedSelected when OnGridItemDeSelected called and the row already selected and even targen not null and not checked', async(() => {
        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.Selected;
        selectedItem.Selected = true;
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);
        component.gridData.IsAllMedSearch = false;
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = {
            target: {
                checked: false
            }
        };
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridItemSelected(selectedItem, 0, event)

        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(1);
        expect(component.gridData.Meds[0].RowStatus).toEqual(GridRowSelectionStatus.NotSelected);
        expect(selectionCount).toEqual(0);
        expect(gridEvent).toEqual(SelectMedicationGridEvent.MedDeselected);

    }));
    //test scenario
    it('should select multiple record true when OnGridItemSelected called and multiselect set to true', async(() => {
        //arrange
        let selectedItem0: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem0.index = 0;
        selectedItem0.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem0.DrugName = "Aspirin Test";
        selectedItem0.Selected = false;
        selectedItem0.Strength = "100";

        let selectedItem1: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem1.index = 1;
        selectedItem1.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem1.DrugName = "Calpol";
        selectedItem1.Selected = false;
        selectedItem1.Strength = "100";

        
        component.gridData = new SelectMedicationGridModel();
        component.gridData.IsAllMedSearch = false;
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem0, selectedItem1);
        //Act
        let selectionCount: number = 0;

        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
        });
        let event = new Event("click");
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridItemSelected(selectedItem0, 0, event)
        component.OnGridItemSelected(selectedItem1, 1, event)
        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(2);

        let selectedGridData = component.gridData.Meds.filter(o => o.Selected == true);
        expect(selectedGridData.length).toEqual(2);

        expect(selectionCount).toEqual(2);

    }));
    it('should select single record true when OnGridItemSelected called and multiselect set to false', async(() => {
        //arrange
        let selectedItem0: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem0.index = 0;
        selectedItem0.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem0.DrugName = "Aspirin Test";
        selectedItem0.Selected = false;
        selectedItem0.Strength = "100";

        let selectedItem1: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem1.index = 1;
        selectedItem1.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem1.DrugName = "Calpol";
        selectedItem1.Selected = false;
        selectedItem1.Strength = "100";


        component.gridData = new SelectMedicationGridModel();
        component.gridData.IsAllMedSearch = true;// setting multiple selection false
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem0, selectedItem1);
        //Act
        let selectionCount: number = 0;

        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
        });
        let event = new Event("click");
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridItemSelected(selectedItem0, 0, event)
        component.OnGridItemSelected(selectedItem1, 1, event)
        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(2);

        let selectedGridData = component.gridData.Meds.filter(o => o.Selected == true);
        expect(selectedGridData.length).toEqual(1);

        expect(selectionCount).toEqual(1);

    }));

});

describe('selectmedication grid OnGridItemModified testes', () => {
    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc);
    let compSvc: ComponentCommonService = new ComponentCommonService();
    let component: SelectMedicationGridComponent;
    //Mocking service
    class MockSelectMedicationService extends SelectMedicationService {

    }

    //Runs once per test feature
    beforeAll(() => {

    });

    // runs before each scenario 
    beforeEach(async(() => {
        component = new SelectMedicationGridComponent(evnntSvc, new MockSelectMedicationService(null, errorsvc), errorsvc, compSvc);
    }));


    //test scenario
    it('should emit OnMedSelected output event and GridEvent should be MedModified when OnGridItemModified called when the row is selected', async(() => {
        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem.DrugName = "Aspirin Test";
        selectedItem.Selected = true;
        selectedItem.Strength = "100";
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = new Event("change");
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridItemModified(selectedItem, 0, event)

        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(1);
        expect(selectionCount).toEqual(1);
        expect(gridEvent).toEqual(SelectMedicationGridEvent.MedModified);

    }));

    it('should not emit OnMedSelected output event and GridEvent should be MedModified when OnGridItemModified called when the row is not selected', async(() => {
        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem.DrugName = "Aspirin Test";
        selectedItem.Selected = false;
        selectedItem.Strength = "100";
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = new Event("change");
      
        component.OnGridItemModified(selectedItem, 0, event)

        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(1);
        expect(selectionCount).toEqual(0);
        expect(gridEvent).toBe(undefined);


    }));

});


describe('selectmedication grid OnGridSelectAllChecked testes', () => {
    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc);
    let compSvc: ComponentCommonService = new ComponentCommonService();
    let component: SelectMedicationGridComponent;
    //Mocking service
    class MockSelectMedicationService extends SelectMedicationService {

    }

    //Runs once per test feature
    beforeAll(() => {

    });

    // runs before each scenario 
    beforeEach(async(() => {
        component = new SelectMedicationGridComponent(evnntSvc, new MockSelectMedicationService(null, errorsvc), errorsvc, compSvc);
    }));


    //test scenario
    it('should emit OnMedSelected output event and GridEvent should be AllMedSelected and select set all record to selected true and last record to selectedActive row status when OnGridItemModified called when sellect all is false', async(() => {
        let selectedItem0: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem0.index = 0;
        selectedItem0.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem0.Selected = false;

        let selectedItem1: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem1.index = 1;
        selectedItem1.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem1.Selected = false;


        component.gridData = new SelectMedicationGridModel();
        component.gridData.IsAllMedSearch = false;// setting multiple selection false
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem0, selectedItem1);
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = new Event("click");
        //(<any>event).target = {
        //    checked: true
        //}
        component.OnGridSelectAllChecked(event)
   
        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(2);

        let lastGridData = component.gridData.Meds[component.gridData.Meds.length -1];
        expect(lastGridData.RowStatus).toEqual(GridRowSelectionStatus.SelectActive);

        expect(selectionCount).toEqual(2);

        let selectedGridData = component.gridData.Meds.filter(o => o.Selected == true);
        expect(gridEvent).toEqual(SelectMedicationGridEvent.AllMedSelected);
        expect(selectedGridData.length).toEqual(2);

    }));

    it('should emit OnMedSelected output event and GridEvent should be AllMedSelected and select set all record to selected true and last record to selectedActive row status when OnGridItemModified called when sellect all is true', async(() => {
        //arrange
        let selectedItem: SelectMedicationMedModel = new SelectMedicationMedModel();
        selectedItem.index = 0;
        selectedItem.RowStatus = GridRowSelectionStatus.NotSelected;
        selectedItem.DrugName = "Aspirin Test";
        selectedItem.Selected = true;
        selectedItem.Strength = "100";
        component.gridData = new SelectMedicationGridModel();
        component.gridData.Meds = [];
        component.gridData.Meds.push(selectedItem);
        //Act
        let selectionCount: number = 0;
        let gridEvent: SelectMedicationGridEvent;
        component.SelectAll = true;
        component.OnMedSelected.subscribe((args: SelectMedicationSelections) => {
            selectionCount = args.SelectedMeds.length;
            gridEvent = args.EventContext.GridEvent;
        });
        let event = new Event("change");

        component.OnGridSelectAllChecked(event)

        //assert 
        expect(component.gridData).not.toBe(null);
        expect(component.gridData.Meds).not.toBe(null);
        expect(component.gridData.Meds.length).toEqual(1);
        expect(selectionCount).toEqual(0);
        let selectedGridData = component.gridData.Meds.filter(o => o.Selected == true);
        expect(selectedGridData.length).toEqual(0);
        expect(gridEvent).toEqual(SelectMedicationGridEvent.AllMedDeSelected);

    }));

    it('should not show the deluxe ad popup after closeModalPopup method is called in the SelectMedicationGridComponent', async(() => {
        //arrange        
        component.modalPopup = new ModalPopupControl();
        //act
        component.closeModalPopup();
        //assert
        expect(component.modalPopup.ShowCloseButton).not.toBeTruthy();
    }));
});

