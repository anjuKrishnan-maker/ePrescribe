import { } from 'jasmine';
import { UserSettingsComponent } from '../component/user/user-settings/user-settings.component';
import { UserCategory } from '../model/user/user.model';

describe('user settings specs', () => {

    beforeAll(() => {

    });

    beforeEach(() => {
        
    });

    it('should deny a provider switching to a staff user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.PROVIDER;
        let toUserType = UserCategory.GENERAL_USER;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeTruthy();
    });

    it('should allow a physician assistant with supervision switching to a physician assistant without supervision user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
        let toUserType = UserCategory.PHYSICIAN_ASSISTANT;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeFalsy();
    });

    it('should deny a physician assistant with supervision switching to a pob super user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
        let toUserType = UserCategory.POB_SUPER;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeTruthy();
    });

    it('should deny a physician assistant without supervision switching to a pob limited user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.PHYSICIAN_ASSISTANT;
        let toUserType = UserCategory.POB_LIMITED;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeTruthy();
    });

    it('should allow a physician assistant with supervision switching to a provider user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
        let toUserType = UserCategory.PROVIDER;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeFalsy();
    });

    it('should allow a physician assistant with supervision switching to a Provider and then to a physician assistant without supervision user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        userSettings.initialUserType = UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;
        let fromUserType = UserCategory.PROVIDER;
        let toUserType = UserCategory.PHYSICIAN_ASSISTANT;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeFalsy();
    });

    it('should allow a pob limited switching to a pob regular user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.POB_LIMITED;
        let toUserType = UserCategory.POB_REGULAR;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeFalsy();
    });

    it('should deny a pob super switching to a physician assistant without supervision user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.POB_SUPER;
        let toUserType = UserCategory.PHYSICIAN_ASSISTANT;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeTruthy();
    });

    it('should deny a staff user switching to a physician assistant without supervision user type', () => {
        //arrange;
        let userSettings = new UserSettingsComponent();
        let fromUserType = userSettings.initialUserType = UserCategory.GENERAL_USER;
        let toUserType = UserCategory.PHYSICIAN_ASSISTANT;

        //act
        let result = userSettings.IsSwitchingDenied(fromUserType, toUserType);

        //assert
        expect(result).toBeTruthy();
    });
});