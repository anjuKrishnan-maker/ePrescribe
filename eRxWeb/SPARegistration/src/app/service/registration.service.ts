import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class RegistrationService {    
    constructor(private dataService: DataService) {
    }
    getRegistrantStatus(isLicencePage: boolean) {
        let showErrorMessage = (error: HttpErrorResponse): boolean => {
            //When response URL is login.aspx,means status check was not successful
            //? In https://github.com/angular/angular/blob/7.2.x/packages/common/http/src/xhr.ts
            // IE11 for XHR request dont values responseURL, hence in this f/w as fallback they keep request url
            if (error.url && (error.url.toLowerCase().indexOf("login.aspx") > -1 ||
                error.url.toLowerCase().indexOf("api/registrantstatus") > -1)) {
                return true;
            }
            return false;
        };
        return this.dataService.get("api/registrantstatus/" + isLicencePage, showErrorMessage);
    }
    updateUserCsp() {
        return this.dataService.post("api/update-user-csp",null);
    }
}