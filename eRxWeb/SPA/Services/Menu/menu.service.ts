import { Injectable ,Inject,forwardRef}     from '@angular/core';
import {BaseService} from '../base.service';
import { ErrorService } from '../service.import.def';
import { MenuItemModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class MenuService extends  BaseService{
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetMenu(){
        return this.InvokeApiMethod<MenuItemModel[]>('/api/MenuApi/GetMenu', null);
    }
}