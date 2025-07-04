//testing framework
import { async } from '@angular/core/testing';
// import rxjs 
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import { EventEmitter } from '@angular/core';
//import for test componet 
import { EventService, PatientService, ErrorService} from '../Services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID, MessageControlType } from '../tools/constants';
import { PatientSearchBarModel } from '../model/model.import.def';
import { PatientSearchBar } from '../component/shared/patient-search-bar/patient-search-bar.component';
import { DateValidator } from '../tools/utils/date-validator';
//feature
describe('PatientSearchBar Coverage Tests', () => {
    let eventService: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(eventService, null);
    let component: PatientSearchBar;
    //Runs once per test feature
    beforeAll(() => {

    });

    // runs before each scenario 
    beforeEach(async(() => {
        component = new PatientSearchBar(eventService);
    }));

    it('ReInitializePatientSearchBar - Should Reinitialize FirtName, LastName, DOB, PatientID, Error Message', async(() => {
        //arrange

        //Act
        component.ReInitializePatientSearchBar();

        //assert 
        expect(component.patientSearchBarModel.FirstName).toBe("");
        expect(component.patientSearchBarModel.LastName).toBe("");
        expect(component.patientSearchBarModel.DateOfBirth).toBe("");
        expect(component.patientSearchBarModel.PatientId).toBe("");
    }));

    it('IsValidTextLength - Should return true if text is not empty and has a length > 2', async(() => {
        //arrange
        let text: string = "";
        //Act
        let isValid: boolean = component.IsValidTextLength(text);

        //assert 
        expect(isValid).toEqual(false);

        //arrange
        text = "A";
        //Act
        isValid = component.IsValidTextLength(text);

        //assert 
        expect(isValid).toEqual(false);

        //arrange
        text = "AA";
        //Act
        isValid = component.IsValidTextLength(text);

        //assert 
        expect(isValid).toEqual(true);

    }));

    it('IsValidPatientSearchCriteria - Should return false if firstname, lastname, dob and patientId are empty', async(() => {
        //arrange
        let textFirstName: string = "";
        let textLastName: string = "";
        let textDob: string = "";
        let textPatientId: string = "";

        component.ReInitializePatientSearchBar();
        component.patientSearchBarModel.FirstName = textFirstName;
        component.patientSearchBarModel.LastName = textLastName;
        component.patientSearchBarModel.DateOfBirth = textDob;
        component.patientSearchBarModel.PatientId = textPatientId;
        let dummyElement: HTMLInputElement = document.createElement('input');

        //Act
        let isValidSearchCriteria: boolean = component.IsValidPatientSearchCriteria();

        //assert 
        expect(isValidSearchCriteria).toEqual(false);


    }));
    it('IsValidPatientSearchCriteria - Should return false if FirstName contains less than 2 characters', async(() => {
        //arrange
        let textFirstName: string = "F";
        let textLastName: string = "";
        let textDob: string = "";
        let textPatientId: string = "";
        let dummyElementSpan: HTMLSpanElement = document.createElement('span');
        let dummyElement: HTMLInputElement = document.createElement('input');

        component.ReInitializePatientSearchBar();
        component.patientSearchBarModel.FirstName = textFirstName;
        component.patientSearchBarModel.LastName = textLastName;
        component.patientSearchBarModel.DateOfBirth = textDob;
        component.patientSearchBarModel.PatientId = textPatientId;

        //Act
        let isValidSearchCriteria: boolean = component.IsValidPatientSearchCriteria();

        //assert 
        expect(isValidSearchCriteria).toEqual(false);


    }));
    it('IsValidPatientSearchCriteria - Should return false if FirstName contains less than 2 characters', async(() => {
        //arrange
        let textFirstName: string = "F";
        let textLastName: string = "";
        let textDob: string = "";
        let textPatientId: string = "";
        let dummyElementSpan: HTMLSpanElement = document.createElement('span');
        let dummyElement: HTMLInputElement = document.createElement('input');

        component.ReInitializePatientSearchBar();
        component.patientSearchBarModel.FirstName = textFirstName;
        component.patientSearchBarModel.LastName = textLastName;
        component.patientSearchBarModel.DateOfBirth = textDob;
        component.patientSearchBarModel.PatientId = textPatientId;

        //Act
        let isValidSearchCriteria: boolean = component.IsValidPatientSearchCriteria();

        //assert 
        expect(isValidSearchCriteria).toEqual(false);


    }));

    it('IsValidDateEntered - Should return false if DOB contains invalid characters', async(() => {
        //arrange
        let textFirstName: string = "F";
        let textLastName: string = "";
        let textDob: string = "11AAA";
        let textPatientId: string = "";
        let dummyElementSpan: HTMLSpanElement = document.createElement('span');
        let dummyElement: HTMLInputElement = document.createElement('input');

        component.ReInitializePatientSearchBar();
        component.patientSearchBarModel.FirstName = textFirstName;
        component.patientSearchBarModel.LastName = textLastName;
        component.patientSearchBarModel.DateOfBirth = textDob;
        component.patientSearchBarModel.PatientId = textPatientId;

        //Act
        let isValidSearchCriteria: boolean = component.IsValidPatientSearchCriteria();

        //assert 
        expect(isValidSearchCriteria).toEqual(false);


    }));

    it('ValidateDate - Should return false if Date is not in valid format', async(() => {
        //arrange
        let date: string = "08/13//2019";

        //Act
        let isValidDate: boolean = DateValidator(date);

        //assert 
        expect(isValidDate).toEqual(false);

    }));
    
    it('ValidateDate - Should return true if Date is in valid format', async(() => {
        //arrange
        let date: string = "08/13/2019";

        //Act
        let isValidDate: boolean = DateValidator(date);

        //assert 
        expect(isValidDate).toEqual(true);

    }));

});


