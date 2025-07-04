export class UserDetails {
    firstname: string;
    lastName: string;
    middleName: string;
    personalEmail: string;
    contactPhoneNumber: string;
    HomeAddress: string;
    HomeAddress2: string;
    city: string;
    deaScheduleII: boolean;
    deaScheduleIII: boolean ;
    deaScheduleIV: boolean ;
    deaScheduleV: boolean ;
    shieldUserName: string;
    password: string;
    confirmPassword: string;
    title: string;
    suffix: string;
    speciality1: string;
    speciality2: string;
    deaNumber: string;
    zipCode: string;
    state: string;
    stateLicenseNumber: string;
    deaExpirationDate: string;
    idMeTermsAccepted: boolean;    
    npi: string;
    stateLicenseExpirationDate: string;
    stateLicenseIssueState: string;
    txtCapchaResponse: string;
    securityQuestion1: string;
    securityQuestion2: string;
    securityQuestion3: string;
    securityAnswer1: string;
    securityAnswer2: string;
    securityAnswer3: string;
    pricingStructure: string;
}

export class shieldSecretQuestion {
    questionIDField: number;
    questionField: string;
}

export class SecretAnswers {

    questionId: number;
    answer: string;
}
export class State {
    state: string;
    description: string
}

export class Speciality {
    specialty_CD: string;
    specialty: string;
}

export class CreateRegistrantUserRequest {
    RegistrantUser: UserDetails;
    SecretAnswers: SecretAnswers[];
    Captcha: string;
    IsLinkExistingShieldUser: boolean;
}

export class ValidateRegistrantModel {
    IsValid: boolean;
    IsValidCaptcha: boolean;
    IsValidNpi: boolean;
    IsDeaCheck: boolean;
    IsValidDEA: boolean;
    IsValidExistingShieldUser: boolean;
    IsMaxRetryFinished: boolean;
}