import { ErrorHandler, Injectable } from '@angular/core';
import {ErrorService } from '../../services/service.import.def';
@Injectable()
export class MyErrorHandler implements ErrorHandler {
    constructor(private errorSvc: ErrorService) {

    }
  handleError(error) {
      this.errorSvc.DisplayError(error);
  }
}