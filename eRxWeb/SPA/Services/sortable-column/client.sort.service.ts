import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { ColumnSortDirection } from "../../tools/constants";
import ColumnSortedEventmodel = require("../../model/columnsortedevent.model");
import ColumnSortedEvent = ColumnSortedEventmodel.ColumnSortedEvent;

@Injectable()
export class ClientSortService {

    constructor() { }
    
    getSortedResults(criteria: ColumnSortedEvent, unsortedModelArray: any[]): any[] {
        return unsortedModelArray.sort((a, b) => {
            if (criteria.sortColumn === "DOB") {
                let dateA: Date = new Date(a[criteria.sortColumn]);
                let dateB: Date = new Date(b[criteria.sortColumn]);
                if (criteria.sortDirection === ColumnSortDirection.Down) {
                    return dateB.getTime() - dateA.getTime();
                }
                else {
                    return dateA.getTime() - dateB.getTime();
                }
            }
            if (criteria.sortDirection === ColumnSortDirection.Down) {
                if (a[criteria.sortColumn].toString().toLowerCase() < b[criteria.sortColumn].toString().toLowerCase()) {
                    return 1;
                }
                if (a[criteria.sortColumn].toString().toLowerCase() > b[criteria.sortColumn].toString().toLowerCase()) {
                    return -1;
                }
                return 0;
            }
            else {

                if (a[criteria.sortColumn].toString().toLowerCase() > b[criteria.sortColumn].toString().toLowerCase()) {
                    return 1;
                }
                if (a[criteria.sortColumn].toString().toLowerCase() < b[criteria.sortColumn].toString().toLowerCase()) {
                    return -1;
                }
                return 0;
            }
        });
    }

}
