import { Injectable } from '@angular/core';
import { DataService } from './data.service';

@Injectable()
export class WelcomeService {
    constructor(private dataService: DataService) { }
    logRegistrantUserNavigationToCSP() {        
        return this.dataService.post("api/welcome/geturl-for-navigation-to-csp", null);
    }
}