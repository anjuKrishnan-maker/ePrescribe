export class ApiResponse {
    ErrorContext: ErrorContextModel;
    Payload: any;
}

export class ErrorContextModel {
    Message: string;
    Error: ErrorTypeEnum;
}

export enum ErrorTypeEnum {
    UserMessage,
    ServerError
}

export interface EPSResponse {
    successField: boolean;
    messagesField: string[];
    exceptionIDField: string;
}