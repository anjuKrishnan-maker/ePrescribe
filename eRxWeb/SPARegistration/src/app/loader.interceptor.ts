import { Injectable } from "@angular/core";
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { finalize, map } from "rxjs/operators";
import { LoaderService } from "./service/loader.service";

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
    private apiCallsInProgress: string[] = [];

    constructor(public loaderService: LoaderService) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (req.url && req.url.length > 0)
            this.apiCallsInProgress.push(req.url.toLowerCase());

        if (this.apiCallsInProgress.length > 0)
            this.loaderService.show();

        return next.handle(req)
            .pipe(finalize(() => {
                this.apiCallsInProgress = this.apiCallsInProgress.filter(api => api !== req.url.toLowerCase());
                if (this.apiCallsInProgress.length <= 0 && !this.loaderService.overrideLoading)
                    this.loaderService.hide();

            }));
    }
}