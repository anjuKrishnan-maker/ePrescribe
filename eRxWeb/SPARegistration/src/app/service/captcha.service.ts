import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { Observable } from 'rxjs';

@Injectable()
export class CaptchaService {
     
    constructor(private dataService: DataService) {        
    }   
    getCaptcha() {
        return this.dataService.get("api/anonymous/user-registration/getcaptcha");
    } 


}