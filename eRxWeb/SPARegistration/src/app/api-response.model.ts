export class ApiResponse<T> {
    ErrorContext: ErrorContextModel;
    Payload: T;
}

export class ErrorContextModel {
    Message: string;
    Error: ErrorTypeEnum;
}

export enum ErrorTypeEnum {
    UserMessage,
    ServerError
}

export interface AppContext {
    login: string;
    basicPrice: string;
    deluxePrice: string;
    epcsPrice: string;
    version: string;
    cspLogin: string;
    mediator: string;
    appName: string;
    supportMailAddress: string;
}