import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators'
import { ErrorService } from './service.import.def';
import { HttpClient, HttpHeaders, HttpResponse, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { of } from 'rxjs';
import { ErrorContextModel, ErrorTypeEnum } from '../model/model.import.def';
@Injectable()
export class BaseService {
    constructor(protected http: HttpClient, private ersvc: ErrorService = null) { }

    protected Get<T>(url: string, requestedParams?: any, requestedResponseType?: any): Observable<T> {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        let httpServ = this.http
            .get(url, { headers: headers, params: requestedParams, observe: 'response', responseType: requestedResponseType })
            .pipe(
                map(
                    (resp: any) => {
                        return this.extractDataG<T>(resp);
                    }
                ),
                catchError(this.handleError())
            );
        return httpServ;
    }

    protected InvokeApiMethod<T>(url: string, data: any): Observable<T> {
        let postData = data;
        if (data != undefined)
            postData = JSON.stringify(data);
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.post(url, postData, { headers: headers, observe: 'response' })
            .pipe(
                map((resp: HttpResponse<T>) => {
                    return this.extractDataG<T>(resp);
                }),
                catchError(this.handleError())
            );
    }

    private extractDataG<T>(res: HttpResponse<T>) {
        if (res.status < 200 || res.status >= 300) {
            throw new Error('Bad response status: ' + res.status);
        }
        let body = res.body as any;
        if (body === null) {
            return;
        }
        else if (body.ErrorContext != null && body.ErrorContext != undefined) {
            if (this.ersvc != null)
                this.ersvc.InvokeErroEvent(body.ErrorContext);
        }
        else if (body.Payload != undefined) {
            return body.Payload;
        }
        else if (body != undefined) {
            return body
        }

        return [];
    }

   
    private handleError() {
        return (error: HttpErrorResponse): Observable<any> => {
            this.ersvc.InvokeErroEvent(<ErrorContextModel>{ Message: "Forbidden", Error: ErrorTypeEnum.ServerError });
            let eror = new ErrorContextModel();
            eror.Message = "Forbidden";
            eror.Error = ErrorTypeEnum.ServerError;
            // Let the app keep running by returning an empty result.
            return of(eror);
        };
    }
}