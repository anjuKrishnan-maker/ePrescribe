
//testing framework
import { Observable } from 'rxjs/Observable';
import { Observer } from 'rxjs/Observer';
import {  async } from '@angular/core/testing';
import { RouterTestingModule } from "@angular/router/testing";
import { } from 'jasmine';
//import for test componet 
import { EventService, SelectPatientService, PatientService, ErrorService, PatientMedRecService, PrivacyOverrideService, ClientSortService, TableColumnsSortService, ContentLoadService, SelectMedicationService } from '../Services/service.import.def';

import {
    MessageIcon, MessageModel, ApiResponse
    
} from '../model/model.import.def';
import { PatientSearchBar } from '../component/shared/patient-search-bar/patient-search-bar.component';
import { SearchPatientComponent } from "../component/select-patient/select-patient.component";
import Searchpatientsmodel = require("../model/search-patients/search-patients.model");
import SearchPatientStartupParameters = Searchpatientsmodel.SearchPatientStartupParameters;
import {Patient} from "../model/patient.model";
import SearchPatientResponse = Searchpatientsmodel.SearchPatientResponse;
import PatientItemModel = Searchpatientsmodel.PatientItemModel;
import SearchPatientUserType = Searchpatientsmodel.SearchPatientUserType;
import Patientmodel = require("../model/patient.model");
import PatientMedRecDetailModel = Patientmodel.PatientMedRecDetailModel;
import SearchPatientDataRequest = Searchpatientsmodel.SearchPatientDataRequest;
import CheckInPatientDataResponse = Searchpatientsmodel.CheckInPatientDataResponse;
import CheckInPatientDataRequest = Searchpatientsmodel.CheckInPatientDataRequest;
import SetProviderInformationRequest = Searchpatientsmodel.SetProviderInformationRequest;
import DelegateProvider = Searchpatientsmodel.DelegateProvider;
import LoadProvidersForSupervisedPARequest = Searchpatientsmodel.LoadProvidersForSupervisedPARequest;
import SearchPatientUserCategory = Searchpatientsmodel.SearchPatientUserCategory;
import Supervisingprovidermodel = require("../model/supervising-provider.model");
import SupervisorProviderInfoResponse = Supervisingprovidermodel.SupervisorProviderInfoResponse;
import SupervisorProviderInfoRequest = Supervisingprovidermodel.SupervisorProviderInfoRequest;
//feature
describe('Select Patient Tests', () => {
    let eventService: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(eventService, null);
    let component: SearchPatientComponent;

    //Mocking SelectPatient service- SelectPatientInfo - 0 (SelectPatientInfo)
    const apiResponseSetPatientInfoSelectPatientService: ApiResponse = new ApiResponse();
    let patient = new Patient();
    patient.PatientID = "ANKIT_SINGH";
    apiResponseSetPatientInfoSelectPatientService.Payload = patient;
    apiResponseSetPatientInfoSelectPatientService.ErrorContext = null;

    //Mocking SelectPatient service- STARTUP - 1 (AuditAccessAndGetStartupParameters)
    const apiResponseSelectPatientService: ApiResponse = new ApiResponse();
    let searchPatientStartupParameters = new SearchPatientStartupParameters();
    searchPatientStartupParameters.IsRestrictedPatient = false;
    searchPatientStartupParameters.UserType = SearchPatientUserType.Provider;
    apiResponseSelectPatientService.Payload = searchPatientStartupParameters;
    apiResponseSelectPatientService.ErrorContext = null;

    //Mocking SelectPatient service - Patient 2 (SearchScheduledPatients)
    const apiResponseSelectPatientServiceSearchPatientResponse = new SearchPatientResponse();
    apiResponseSelectPatientServiceSearchPatientResponse.Patients = new Array<PatientItemModel>(10);
    apiResponseSelectPatientServiceSearchPatientResponse.Patients[0] = new PatientItemModel();
    apiResponseSelectPatientServiceSearchPatientResponse.Patients[0].PatientID = "ANKIT_SINGH";

    //Mocking MedReconciliation service - Patient 3 (SearchScheduledPatients)
    const apiResponseMedReconcileServiceSearchPatientResponse = new PatientMedRecDetailModel();
    apiResponseMedReconcileServiceSearchPatientResponse.ReconciliationMessage = "HELLO WORLD";


    let searchPatientDataRequest1 = new SearchPatientDataRequest();
    searchPatientDataRequest1.ChartID = "111";
    let searchPatientDataRequest2 = new SearchPatientDataRequest();
    searchPatientDataRequest2.ChartID = "222";

    //Mocking SelectPatient service - Patient 4 (SearchPatients) more than 50 patients
    const apiResponseSelectPatientSvcSearchPatientResponse = new SearchPatientResponse();
    apiResponseSelectPatientSvcSearchPatientResponse.Patients = new Array<PatientItemModel>(51);
    apiResponseSelectPatientSvcSearchPatientResponse.Patients[0] = new PatientItemModel();
    apiResponseSelectPatientSvcSearchPatientResponse.Patients[0].PatientID = "ANKIT_SINGH1";

    //Mocking SelectPatient service - Patient 5 (SearchPatients)Get Valid Patients
    const apiResponseSelectPatientSvcSearchPatientResponse2 = new SearchPatientResponse();
    apiResponseSelectPatientSvcSearchPatientResponse2.Patients = new Array<PatientItemModel>(10);
    apiResponseSelectPatientSvcSearchPatientResponse2.Patients[0] = new PatientItemModel();
    apiResponseSelectPatientSvcSearchPatientResponse2.Patients[0].PatientID = "ANKIT_SINGH2";

    //Mocking SelectPatient Service - CheckinClicked
    let apiResponseSelectPatientSvcCheckInPatientDataResponse = new CheckInPatientDataResponse();
    apiResponseSelectPatientSvcCheckInPatientDataResponse.CheckedInMessage = "Mock Message";
    apiResponseSelectPatientSvcCheckInPatientDataResponse.CheckedInMessageIcon = "Information";
    apiResponseSelectPatientSvcCheckInPatientDataResponse.CheckedInMessageVisibility = true;


    //Mocking SetProviderInformation - only boolean so no need to create a response object
    let apiResponseLoadProvidersForSupervisedPAResponse = new Array<DelegateProvider>(5);
    apiResponseLoadProvidersForSupervisedPAResponse[0] = new DelegateProvider();
    apiResponseLoadProvidersForSupervisedPAResponse[0].ProviderId = "ProviderId1";
    apiResponseLoadProvidersForSupervisedPAResponse[0].ProviderName = "ProviderName1";
    apiResponseLoadProvidersForSupervisedPAResponse[1] = new DelegateProvider();
    apiResponseLoadProvidersForSupervisedPAResponse[1].ProviderId = "ProviderId2";
    apiResponseLoadProvidersForSupervisedPAResponse[1].ProviderName = "ProviderName2";
    apiResponseLoadProvidersForSupervisedPAResponse[2] = new DelegateProvider();
    apiResponseLoadProvidersForSupervisedPAResponse[2].ProviderId = "ProviderId3";
    apiResponseLoadProvidersForSupervisedPAResponse[2].ProviderName = "ProviderName3";
    apiResponseLoadProvidersForSupervisedPAResponse[3] = new DelegateProvider();
    apiResponseLoadProvidersForSupervisedPAResponse[3].ProviderId = "ProviderId4";
    apiResponseLoadProvidersForSupervisedPAResponse[3].ProviderName = "ProviderName4";
    apiResponseLoadProvidersForSupervisedPAResponse[4] = new DelegateProvider();
    apiResponseLoadProvidersForSupervisedPAResponse[4].ProviderId = "ProviderId5";
    apiResponseLoadProvidersForSupervisedPAResponse[4].ProviderName = "ProviderName5";


    class MockSelectPatientService extends SelectPatientService {
        constructor() {
            super(null, errorsvc);
        }
        public SetPatientInfo(patientId: string) {
            return Observable.create((observer: Observer<any>) => {
                observer.next(apiResponseSetPatientInfoSelectPatientService);
            });
        }
        public AuditAccessAndGetStartupParameters() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(apiResponseSelectPatientService);
            });
        }
        public SearchScheduledPatients() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(apiResponseSelectPatientServiceSearchPatientResponse);
            });

        }
        public SearchPatients(request: SearchPatientDataRequest) {
            return Observable.create((observer: Observer<any>) => {
                if (request.ChartID === "111") {
                    observer.next(apiResponseSelectPatientSvcSearchPatientResponse);
                }
                else if (request.ChartID === "222") {
                    observer.next(apiResponseSelectPatientSvcSearchPatientResponse2);
                }
            });
        }
        public CheckInPatient(request: CheckInPatientDataRequest) {
            return Observable.create((observer: Observer<any>) => {
                observer.next(apiResponseSelectPatientSvcCheckInPatientDataResponse);
            });
        }
        public SetProviderInformation(request: SetProviderInformationRequest) {
            return Observable.create((observer: Observer<any>) => {
                if (request.ProviderId === "ValidProvider") {
                    observer.next(true);
                } else if (request.ProviderId === "InvalidProvider") {
                    observer.next(false);

                } else {
                    observer.next(true);
                }

            });

        }
        public LoadProvidersForSupervisedPA(request: LoadProvidersForSupervisedPARequest) {
            return Observable.create((observer: Observer<any>) => {
                if (request.ProviderId === "ValidSupervisedPa") {
                    observer.next(apiResponseLoadProvidersForSupervisedPAResponse);
                } else if (request.ProviderId === "InvalidSupervisedPa") {
                    observer.next(new Array<DelegateProvider>());

                }
            });
        }

    }
    class MockPatientService extends PatientService {
    }

    class MockPatientMedRecService extends PatientMedRecService {
        public GetEligibilityAndMedHistoryStatus() {
            return Observable.create((observer: Observer<any>) => {
                observer.next(apiResponseMedReconcileServiceSearchPatientResponse);
            });
        }
    }

    class MockPrivacyOverrideService extends PrivacyOverrideService {

    }

    class MockClientSortService extends ClientSortService {

    }

    class MockTableColumnsSortService extends TableColumnsSortService {

    }
    class MockErrorService extends ErrorService {


    }
    class MockSelectMedicationService extends SelectMedicationService {

    }
    class MockContentLoadService extends ContentLoadService {

    }

    class MockRouter extends RouterTestingModule {
        getCurrentNavigation() {

        }
    }

    //private

    //Runs once per test feature
    beforeAll(() => {
        component = new SearchPatientComponent(
            new MockSelectPatientService(),
            new MockPatientService(null, errorsvc, eventService),
            eventService,
            new MockPatientMedRecService(null, errorsvc),
            new MockPrivacyOverrideService(null, errorsvc),
            new MockSelectMedicationService(null, errorsvc),
            new MockClientSortService(), new MockRouter(), new MockContentLoadService(null, errorsvc));
    });
    // runs before each scenario 
    beforeEach(
        async(() => {

        }));


    it('ResetSelectPatientComponent - Should reset all startup parameters as expected', async(() => {
        //arrange
        component.ResetSelectPatientComponent();

        //Act

        expect(component.isMainDatasetLoading).toEqual(true);
        expect(component.isNavigatingAway).toEqual(false);
        expect(component.isMedReconciliationInfoLoaded).toEqual(false);
    }));


    xit('PostReconcileMessageToMessageBanner - Should add med reconcile message if messages list is empty but not null', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();

        //Act
        component.PostReconcileMessageToMessageBanner();

        //Assert
        expect(component.Messages.length).toEqual(1);
        expect(component.Messages[0].Tag).toEqual("medrec");
        expect(component.Messages[0].Icon).toEqual(MessageIcon.Information);
        expect(component.Messages[0].Message).toEqual(undefined);
    }));

    xit('PostReconcileMessageToMessageBanner - Should replace Medication Reconcile Message if message list is not empty and contains tag medrec', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        component.Messages[0] = new MessageModel("", MessageIcon.Information, "medrec", true);

        //Act
        component.PostReconcileMessageToMessageBanner();

        //Assert
        expect(component.Messages.length).toEqual(1);
        expect(component.Messages[0].Tag).toEqual("medrec");
        expect(component.Messages[0].Icon).toEqual(MessageIcon.Information);
        expect(component.Messages[0].Message).toEqual(undefined);
    }));

    it('PostReconcileMessageToMessageBanner - Should append med reconcile message to banner if messages list is empty but not null and if tag is not medrec', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>(1);
        component.Messages[0] = new MessageModel("", MessageIcon.Information, "", true);
        component.PatientMedRecInfo = new PatientMedRecDetailModel();
        component.PatientMedRecInfo.ReconciliationMessage = "HELLO WORLD";

        //Act
        component.PostReconcileMessageToMessageBanner();

        //Assert
        expect(component.Messages.length).toEqual(2);
        expect(component.Messages[0].Tag).toEqual("");
        expect(component.Messages[0].Message).toEqual("");
        expect(component.Messages[1].Tag).toEqual("medrec");
        expect(component.Messages[1].Message).toEqual("HELLO WORLD");
    }));

    it('GetPatientMedRecInfo - Should append med reconcile message to banner if messages list is empty but not null and if tag is not medrec', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();

        //Act
        component.GetPatientMedRecInfo("ANKIT_SINGH");

        //Assert
        expect(component.isMedReconciliationInfoLoaded).toEqual(true);
        expect(component.Messages.length).toEqual(1);
        expect(component.Messages[0].Tag).toEqual("medrec");
        expect(component.Messages[0].Message).toEqual("HELLO WORLD");
    }));

    it('GetPatientsFromService - Should show a warning message if more than 50 patient records', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        let searchPdr1 = new SearchPatientDataRequest();
        searchPdr1.ChartID = "111";

        //Act
        component.GetPatientsFromService(searchPdr1);

        //Assert
        expect(component.Messages[0].Message).toEqual("Your search returned more than 50 results. Please consider refining your search.");
    }));

    it('GetPatientsFromService - Should contain patients returned by service when Search Button Clicked', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        let searchPdr2 = new SearchPatientDataRequest();
        searchPdr2.ChartID = "222";

        //Act
        component.GetPatientsFromService(searchPdr2);

        //Assert
        expect(component.startupParameters.SearchPatientResponse.Patients[0].PatientID).toEqual("ANKIT_SINGH2");
    }));

    it('CheckInClicked - Should contain patients returned by service when Search Button Clicked', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        //Act
        component.CheckInClicked("ANKIT_SINGH");

        //Assert
        expect(component.Messages.length).toEqual(1);
        expect(component.Messages[0].Message).toEqual("Mock Message");
        //expect(component.Messages[1].Message.endsWith(" has been checked in.")).toEqual(true);
    }));

    xit('setProviderInformation - Should goto NavigateToSelectMed for valid provider', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        //Act
        let setProviderInformationRequest: SetProviderInformationRequest = new SetProviderInformationRequest();
        setProviderInformationRequest.ProviderId = "ValidProvider";
        component.setProviderInformation(setProviderInformationRequest, SearchPatientUserCategory.GENERAL_USER);
        spyOn(component, 'NavigateToSelectMed');

        //Assert
        expect(component.NavigateToSelectMed).toHaveBeenCalled();
        expect(component.Messages.length).toEqual(0);

    }));
    xit('setProviderInformation - Should return false when Provider with invalid NPI supplied', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        //Act
        let setProviderInformationRequest: SetProviderInformationRequest = new SetProviderInformationRequest();
        setProviderInformationRequest.ProviderId = "InvalidProvider";
        component.setProviderInformation(setProviderInformationRequest, SearchPatientUserCategory.GENERAL_USER);

        //Assert
        expect(component.Messages.length).toEqual(1);
        expect(component.Messages[0].Message).toEqual("Please select a provider with a valid NPI.");
    }));

    xit('LoadProvidersForSupervisedPA - Should not launch the supervisingProvider Overlay for invalid supervised PA', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        let setProviderInformationRequest: SetProviderInformationRequest = new SetProviderInformationRequest();
        setProviderInformationRequest.ProviderId = "InvalidSupervisedPa";

        //Act
        component.setProviderInformation(setProviderInformationRequest, SearchPatientUserCategory.PHYSICIAN_ASSISTANT_SUPERVISED);

        //Assert
        expect(component.Messages.length).toEqual(1);
        expect(component.Messages[0].Message).toEqual("Could not load providers.");
    }));

    xit('LoadProvidersForSupervisedPA - Should  launch the supervisingProvider Overlay for valid supervised PA', async(() => {
        //arrange
        component.Messages = new Array<MessageModel>();
        let setProviderInformationRequest: SetProviderInformationRequest = new SetProviderInformationRequest();
        setProviderInformationRequest.ProviderId = "ValidSupervisedPa";

        //Act
        component.setProviderInformation(setProviderInformationRequest, SearchPatientUserCategory.PHYSICIAN_ASSISTANT_SUPERVISED);
        spyOn(component, 'showSupervisingProviderPopup');

        //Assert
        expect(component.showSupervisingProviderPopup).toHaveBeenCalled();
    }));

    it('InitializeGridNoPatientSelected - reset the patient search bar and make no selection', async(() => {
        //arrange
        //////-----------------
        //act
        component.initializeGridNoPatientSelected();
        //assert
        expect(component.resetSearchBar).toEqual(false);       
    }));       
});


