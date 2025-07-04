import { } from 'jasmine';
import { EditSecretQuestionsModalPopupComponent } from '../component/user/user-security-settings/edit-secret-questions-modal-popup/edit-secret-questions-modal-popup.component';
import { UserService } from '../services/user.service';

describe('edit secret questions modal popup specs', () => {
    let http = null;
    class MockUserService extends UserService { }
    beforeAll(() => {

    });


    it('should have all three valid questions/answers validation', async () => {
        //arrange
        let secretQuestionPopup = new EditSecretQuestionsModalPopupComponent(new MockUserService(http));
        secretQuestionPopup.q1 = secretQuestionPopup.q2 = secretQuestionPopup.q3 = 2;
        secretQuestionPopup.a1 = '1';
        secretQuestionPopup.a2 = '2';
        secretQuestionPopup.a3 = '3';

        let f = { valid: true };
        //act
        let result = secretQuestionPopup.validate(f);
        //assert
        expect(result).toBeFalsy();
        expect(secretQuestionPopup.errorMessage).toContain("Recovery answers must be unique");
    });

    it('should have all three answers validation', async () => {
        //arrange
        let secretQuestionPopup = new EditSecretQuestionsModalPopupComponent(new MockUserService(http));

        secretQuestionPopup.a1 = '';
        secretQuestionPopup.a2 = '2';
        secretQuestionPopup.a3 = '3';

        let f = { valid: true };
        //act
        let result = secretQuestionPopup.validate(f);
        //assert
        expect(result).toBeFalsy();
        expect(secretQuestionPopup.errorMessage).toContain("You must answer all three secret questions.");
    });

    it('should have all three valid answers ateast one character validation', async () => {
        //arrange
        let secretQuestionPopup = new EditSecretQuestionsModalPopupComponent(new MockUserService(http));
        secretQuestionPopup.q1 = 1; secretQuestionPopup.q2 = 2; secretQuestionPopup.q3 = 3;

        secretQuestionPopup.a1 = 'one';
        secretQuestionPopup.a2 = 'two';
        secretQuestionPopup.a3 = '3';

        let f = { valid: true };
        //act
        let result = secretQuestionPopup.validate(f);
        //assert
        expect(result).toBeTruthy();
    });


});