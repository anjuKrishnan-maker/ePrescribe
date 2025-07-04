import { } from 'jasmine';
import { UserDeaLicenseComponent } from '../component/user/user-dea-license/user-dea-license.component';
import { DeaLicenseService, ErrorService, EventService } from '../services/service.import.def';
import { AddUserDeaLicenseComponent } from '../component/user/user-dea-license/add-user-dea-license/add-user-dea-license.component';
import { DEALicense, DeaLicenseType, UserDeaLicensesModel } from '../model/model.import.def';


describe('user dea license component specs', () => {
    let http: any;
    let eventService: EventService = new EventService();
    let errorSvc: ErrorService = new ErrorService(eventService, null);
    //mocking the service class
    class MockDeaLicenseService extends DeaLicenseService {
    }


    let addUserDeaLicense = new AddUserDeaLicenseComponent(new MockDeaLicenseService(http, errorSvc));
    addUserDeaLicense.userDeaLicensesModel = {} as UserDeaLicensesModel;
    addUserDeaLicense.userDeaLicensesModel.DEALicenses = [];

    beforeEach(() => {

    });

    it('user dea license component should contain child component to add user', () => {
        let userDeaLicense = new UserDeaLicenseComponent(new MockDeaLicenseService(http, errorSvc));
        userDeaLicense.addUserDeaLicenseComponent = new AddUserDeaLicenseComponent(new MockDeaLicenseService(http, errorSvc));
        userDeaLicense.addDeaLicense();
        expect(userDeaLicense.addUserDeaLicenseComponent.showForm).toBeTruthy();
    });

    it('should validate add user dea license with a correct license details as true ', () => {
        addUserDeaLicense.DeaLicense = {} as DEALicense;
        addUserDeaLicense.DeaLicense.DEAExpirationDate = (new Date('12/20/2999')).toString();
        addUserDeaLicense.DeaLicense.DEAIAllowed = false;
        addUserDeaLicense.DeaLicense.DEAIIAllowed = true;
        addUserDeaLicense.DeaLicense.DEAIIIAllowed = true;
        addUserDeaLicense.DeaLicense.DEAIVAllowed = true;
        addUserDeaLicense.DeaLicense.DEAVAllowed = true;
        addUserDeaLicense.DeaLicense.DEALicenseTypeId = DeaLicenseType.Primary;
        addUserDeaLicense.DeaLicense.DEANumber = 'BT2361234';

        expect(addUserDeaLicense.validateDEALicense()).toBeTruthy();
    });
}