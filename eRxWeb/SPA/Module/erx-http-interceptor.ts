import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from "@angular/common/http";
import { Observable } from "rxjs";
import { finalize } from "rxjs/operators";
import { ROUTES_WITH_SPINNER } from "../tools/constants";
import { SpinnerService } from "../services/service.import.def";

@Injectable()
export class ErxRequestInterceptor implements HttpInterceptor {
    private apiCallsInProgress: string[] = [];

    constructor(public spinnerService: SpinnerService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        
        
        if (this.requestRequiresSpinner(req.url)) {            
            this.apiCallsInProgress.push(req.url.toLowerCase());            
        }
        if (this.apiCallsInProgress.length > 0)
            this.spinnerService.showApiSpinner();
        
        return next.handle(req)
            .pipe(finalize(() => {
                this.apiCallsInProgress = this.apiCallsInProgress.filter(api => api !== req.url.toLowerCase());
                if (this.apiCallsInProgress.length <= 0)
                    this.spinnerService.hideApiSpinner();
            }));

    }

    public requestRequiresSpinner(url: string): boolean {
        return ROUTES_WITH_SPINNER.includes(url.toLowerCase());       
    }
}