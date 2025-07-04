import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { SendCancelRxRequestModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CancelRxService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    SendCancelRx(cancelRxItemList: SendCancelRxRequestModel[]) {
        return this.InvokeApiMethod('/API/CancelRxApi/SendCancelRx', cancelRxItemList);
    }

    SendCompletedRxCancelRx(cancelRxItemList: SendCancelRxRequestModel[]) {
        return this.InvokeApiMethod('/API/CancelRxApi/SendCancelRx', cancelRxItemList);
    }
}