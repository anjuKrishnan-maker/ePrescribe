import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { ContentRefreshPayload, InitalContentPayload, MedicationSelectedPayload } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
import { API_ROUTE_NAME } from '../../tools/constants';
@Injectable()

export class ContentLoadService extends BaseService {
    private _initalContentPayload: InitalContentPayload;
    private _refreshedContentPayload: ContentRefreshPayload;

    get initalContentPayload(): InitalContentPayload {
        return this._initalContentPayload;
    }

    set initalContentPayload(newName: InitalContentPayload) {
        this._initalContentPayload = newName;
    }

    get refreshedContentPayload(): ContentRefreshPayload {
        return this._refreshedContentPayload;
    }

    set refreshedContentPayload(refreshedContent: ContentRefreshPayload) {
        this._refreshedContentPayload = refreshedContent;
    }

    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    RetrieveRefreshPayload(pageName: string): Promise<boolean> {
        this.initalContentPayload = <InitalContentPayload>{};
        return new Promise(resolve => {
            return this.InvokeApiMethod<ContentRefreshPayload>(API_ROUTE_NAME.RETRIEVE_REFRESH_PAYLOAD, pageName)
                .subscribe((initialContentPayload: ContentRefreshPayload) => {
                    this.refreshedContentPayload = initialContentPayload;                    
                    resolve(true);
                }, () => { resolve(false) });
        });
    }
    //TODO: Balu make it a pipe
    RetrieveInitialPayload(pageName: string): Promise<boolean> {
        return new Promise(resolve => {
            return this.InvokeApiMethod<InitalContentPayload>('/api/ContentLoad/RetrieveInitialPayload', pageName)
                .subscribe((initialContentPayload: InitalContentPayload) => {
                    this.initalContentPayload = initialContentPayload;
                    resolve(true);
                }, () => { resolve(false) });
        });
    }

    RetrieveMedicationLoadedPayload(medSelectReq) {
        return this.InvokeApiMethod<MedicationSelectedPayload>('/api/ContentLoad/RetrieveMedicationLoadedPayload', medSelectReq);
    }
}