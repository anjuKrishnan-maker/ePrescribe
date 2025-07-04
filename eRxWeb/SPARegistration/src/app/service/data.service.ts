import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map, catchError } from 'rxjs/operators'
import { HttpClient, HttpHeaders, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { of } from 'rxjs';
import { ApiResponse } from '../api-response.model';
import { MessageService } from './message.service';


@Injectable()
export class DataService {

    constructor(private http: HttpClient, private messageService: MessageService) { }

    public get<T>(url: string, skipErrorPage: (x: HttpErrorResponse) => boolean = null) {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.get(url, { headers: headers, observe: "response" })
            .pipe(
                map((resp: HttpResponse<ApiResponse<T>>) => {
                    return this.extractApiResponse<T>(resp);
                }),
                catchError(this.handleError<any>(url, null, false, skipErrorPage))
            );
    }

    public post<T, H>(url: string, data: T, customErrorMessage: string = "") {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.post(url, data, { headers: headers, observe: "response" })
            .pipe(
                map((resp: HttpResponse<ApiResponse<H>>) => {
                    return this.extractApiResponse<H>(resp, customErrorMessage);
                }),
                catchError(this.handleError<any>(url, null, customErrorMessage.length > 0))
            );
    }

    private extractApiResponse<T>(apiResponse: HttpResponse<ApiResponse<T>>, customErrorMessage: string = "") {
        if (apiResponse?.body.ErrorContext) {
            throw new HttpErrorResponse({ status: 500, statusText: `${customErrorMessage} ${apiResponse?.body.ErrorContext.Message}` });
        }
        //If anything unable to interpret from server redirect to the exception page
        return apiResponse.body.Payload != null ? apiResponse.body.Payload as T : {};
    }

    /**
    * Handle Http operation that failed.
    * Let the app continue.
    * @param operation - name of the operation that failed
    * @param result - optional value to return as the observable result
    */
    private handleError<T>(operation = 'operation', result?: T, avoidCustomText: boolean = false, skipErrorPage = null) {
        return (error: HttpErrorResponse): Observable<T> => {

            if (skipErrorPage && skipErrorPage(error))
                return of(result as T);

            // TODO: send the error to app insight infrastructure
            let errorAddition: string = avoidCustomText ? "" : "Please contact customer support!";
            //Display to customer
            this.messageService.notify(`${error.statusText}. ${errorAddition}`);

            // Let the app keep running by returning an empty result.
            return of(result as T);
        };
    }
}