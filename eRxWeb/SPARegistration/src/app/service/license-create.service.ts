import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { LicenseInfo } from '../registration/license-creation/license-creation.model';
@Injectable()
export class LicenseCreateService {
    constructor(private dataService: DataService) { }
    updateRegistrantPracticeDetails(licenseData: LicenseInfo) {        
        return this.dataService.post("api/license-creation/update-practice-details", licenseData);
    }
}