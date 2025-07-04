export interface SecretQuestion {
    questionIDField: number;
    questionField: string;
}


export interface SecretQuestionAnswer {
    questionIDField: number;
    answerField: string;
}


export interface SecretQuestionAnswerRequest {
    secretAnswersField: SecretQuestionAnswer[];
    userNameField: string
    passwordField: string;
    iPAddressField: string;
}

export interface SecretQuestionAnswerResponse {
    successField: boolean;
    messagesField: string;
}