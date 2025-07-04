import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import ColumnSortedEventmodel = require("../../model/columnsortedevent.model");
import ColumnSortedEvent = ColumnSortedEventmodel.ColumnSortedEvent;

@Injectable()
export class TableColumnsSortService {

    constructor() { }
    private columnSortedSource = new Subject<ColumnSortedEvent>();

    columnSorted$ = this.columnSortedSource.asObservable();

    columnSorted(event: ColumnSortedEvent) {
        this.columnSortedSource.next(event);
    }

}