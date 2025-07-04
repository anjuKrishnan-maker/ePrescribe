import { } from 'jasmine';
import { UserStateLicenseComponent } from '../component/user/user-state-license/user-state-license.component';
import { UserService } from '../services/user.service';
import { AddUserStateLicenseComponent } from '../component/user/user-state-license/add-user-state-license/add-user-state-license.component';
import { StateLicense, UserStateLicensesModel } from '../model/model.import.def';
import { UserCategory } from '../model/user/user.model';

fdescribe('user component specs', () => {
    let http: any;

    //mocking the service class
    class MockUserService extends UserService {        
    }

    let addUserStateLicense = new AddUserStateLicenseComponent(new MockUserService(http));
    addUserStateLicense.userStateLicensesModel = {} as UserStateLicensesModel;
    addUserStateLicense.userStateLicensesModel.UserStateLicenses = [];

    beforeEach(() => {
        
    });

    it('user state license component should contain child component to add user', () => {        
        let userStateLicense = new UserStateLicenseComponent(new MockUserService(http));
        userStateLicense.addUserStateLicense = new AddUserStateLicenseComponent(new MockUserService(http));        
        userStateLicense.addLicense();        
        expect(userStateLicense.addUserStateLicense.showForm).toBeTruthy();
    });

    it('should validate add user state license with a correct license details as true ', () => {        
        addUserStateLicense.userStateLicense = {} as StateLicense;
        addUserStateLicense.userStateLicense.State = 'IL';
        addUserStateLicense.userStateLicense.LicenseNo = 'IL7876565';
        addUserStateLicense.userStateLicense.ExpiringDate = new Date("12/12/2030").toLocaleDateString();
        addUserStateLicense.userStateLicense.LicenseType = 'State License Number';
        addUserStateLicense.userStateLicensesModel.UserStateLicenses = [];
        expect(addUserStateLicense.validateLicense()).toBeTruthy();
    });

    it('should validate add user state license for license number with special character as false', () => {
        
        addUserStateLicense.userStateLicense = {} as StateLicense;        
        addUserStateLicense.userStateLicense.State = 'IL';
        addUserStateLicense.userStateLicense.LicenseNo = 'IL78&6565';
        addUserStateLicense.userStateLicense.ExpiringDate = new Date().toLocaleDateString();
        addUserStateLicense.userStateLicense.LicenseType = 'State License Number';
        addUserStateLicense.userStateLicensesModel.UserStateLicenses = [];
        
        expect(addUserStateLicense.validateLicense()).toBeFalsy();
    });

    it('should validate user state license for OH state license to have CTP code', () => {
        //arrange
        addUserStateLicense.userStateLicense = {} as StateLicense;
        addUserStateLicense.userStateLicense.State = "OH";
        addUserStateLicense.userStateLicense.LicenseNo = "IL7806565";
        addUserStateLicense.userStateLicense.ExpiringDate = new Date().toLocaleDateString();
        addUserStateLicense.userStateLicense.LicenseType = 'State License Number';
        addUserStateLicense.userStateLicensesModel.UserStateLicenses = [];        
        addUserStateLicense.userStateLicensesModel.UserType = UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
        addUserStateLicense.MessageStripArray = [];
        //act
        addUserStateLicense.validateLicense();
        //assert
        expect(addUserStateLicense.MessageStripArray[0].Message.includes("In the state of OH")).toBeTruthy();
    });

});