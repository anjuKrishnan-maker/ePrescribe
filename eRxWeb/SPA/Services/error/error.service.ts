import { Injectable, Injector } from '@angular/core';
import { EventService } from '../service.import.def';
import { ErrorContextModel, ErrorTypeEnum } from '../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, PAGE_NAME, ROUTE_NAME } from '../../tools/constants';
import { Router } from '@angular/router';

export interface IErrorService {
    DisplayError(message: string):void;
    DisplayInfo(message: string): void;
    SubscribeError(callBack: (errArgs: ErrorContextModel) => void);
    InvokeErroEvent(errorContext: ErrorContextModel): void;
}

@Injectable()
export class ErrorService implements IErrorService {
    constructor(private evtSvc: EventService, private injector: Injector) {

    }

    public get router(): Router {
        return this.injector.get(Router);
    }

    InvokeErroEvent(errorContext: ErrorContextModel): void {
        if (errorContext != null && errorContext != undefined && errorContext.Error == ErrorTypeEnum.ServerError) {
            this.redirectToExceptionPage();
        }
        else {
            this.evtSvc.invokeEvent<ErrorContextModel>(EVENT_COMPONENT_ID.ErrorEvent, errorContext);
        }
    }

    DisplayError(message: string): void {
        let errorContext = new ErrorContextModel();
        errorContext.Error = ErrorTypeEnum.UserMessage;
        errorContext.Message = message;

        this.evtSvc.invokeEvent<ErrorContextModel>(EVENT_COMPONENT_ID.ErrorEvent, errorContext);
    }
    DisplayInfo(message: string): void {
        let errorContext = new ErrorContextModel();
        errorContext.Error = ErrorTypeEnum.UserMessage;
        errorContext.Message = message;
        this.evtSvc.invokeEvent<ErrorContextModel>(EVENT_COMPONENT_ID.ErrorEvent, errorContext);
    }
    SubscribeError(callBack: (errArgs: ErrorContextModel) => void):void {
        this.evtSvc.subscribeEvent<ErrorContextModel>(EVENT_COMPONENT_ID.ErrorEvent, callBack);
    }

    redirectToExceptionPage() {
        this.router.navigateByUrl(ROUTE_NAME.Exception, { state: { navigateTo: PAGE_NAME.ExceptionPage } })
    }
}