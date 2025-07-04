import { ErrorHandler } from "@angular/core";

export class GlobalErrorHandler implements ErrorHandler {
    handleError(error) {
        console.error(error);
        //TODO: Send to app-insight framework
    }
}