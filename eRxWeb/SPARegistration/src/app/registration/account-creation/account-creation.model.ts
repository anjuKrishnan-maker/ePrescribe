export class AccountDetails {
    firstname: string;
    lastName: string;
    middleName: string;
    personalEmail: string;
    contactPhoneNumber: string;
    userName: string;
    password: string;
    confirmPassword: string;
    title: string;
    suffix: string;
    speciality1: string;
    speciality2: string;
    isTermsAccepted: boolean;
    CapchaText: string;
    securityQuestion1: string;
    securityQuestion2: string;
    securityQuestion3: string;
    securityAnswer1: string;
    securityAnswer2: string;
    securityAnswer3: string;
    secretQuestionsField: SecretAnswers[];
    secretQuestionField: SecretQuestionsAns[];
}

export class SecretQuestionsAns {
    ExtensionDataObject: object;
    questionIDField: number;
    answerField: string;
}

export class shieldSecretQuestion {

    questionIDField: number;
    questionField: string;
}

export class SecretAnswers {

    questionIDField: number;
    answerField: string;
}

export class ValidateCreateUser {
    IsDataSaved: boolean;
    RedirectUrl: string;
    ErrorMessage: string;
    IsValidCaptcha: boolean;
}