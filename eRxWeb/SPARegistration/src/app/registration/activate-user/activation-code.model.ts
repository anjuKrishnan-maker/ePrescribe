export class ActivationCodeModel {
    ActivationCode: string;
    CaptchaText: string;
}

export class ValidateActivationCodeModel {
    IsValid: boolean;
    IsValidCaptcha: boolean;
}