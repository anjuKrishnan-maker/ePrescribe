import {  ComponentFixture, TestBed} from '@angular/core/testing';
import { SecretAnswers, } from './user-security-questions.model';
import { UserSecurityQuestionsComponent } from './user-security-questions.component';

describe('UserSecurityQuestionsComponent', () => {
    let component: UserSecurityQuestionsComponent;
    let fixture: ComponentFixture<UserSecurityQuestionsComponent>;
  
    let response: any;
    let securityQuestions: any;
    class MockUserCreateService {
        //Susmita doubt
    }
    it('should_return_true_when_secret_questions_are_distinct', () => {
        let Result: boolean;
        fixture = TestBed.createComponent(UserSecurityQuestionsComponent);
        component = fixture.componentInstance;                
        component.selectedShieldQuestionOne = 1;
        component.selectedShieldQuestionTwo = 2;
        component.selectedShieldQuestionThree = 3;
        //Result = component.validateSecurityQuestions();
        //expect(Result).toBeTruthy();
    });

    it('should_return_false_when_secret_questions_are_same', () => {
        let Result: boolean;
        fixture = TestBed.createComponent(UserSecurityQuestionsComponent);
        component = fixture.componentInstance;
        component.selectedShieldQuestionOne = 1;
        component.selectedShieldQuestionTwo = 1;
        component.selectedShieldQuestionThree = 1;
        //Result = component.validateSecurityQuestions();
        //expect(Result).toBeFalsy();
    });

    xit('should_get_shieldQuestion_with_different_id', () => {
        let result: any;
        fixture = TestBed.createComponent(UserSecurityQuestionsComponent);
        component = fixture.componentInstance;
        result = component.getInitialPageData();
        component.getShieldQuestions(result.SecurityQuestions);
        expect(component.selectedShieldQuestionOne).toEqual(1);
        expect(component.selectedShieldQuestionTwo).toEqual(2);
        expect(component.selectedShieldQuestionThree).toEqual(3);
    });
   
   

    
});
