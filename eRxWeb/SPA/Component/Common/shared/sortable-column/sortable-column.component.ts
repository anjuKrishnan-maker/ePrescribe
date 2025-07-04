import { Component, OnInit, Input, Output, EventEmitter, OnDestroy, HostListener } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { TableColumnsSortService } from '../../../../services/service.import.def';
import { ColumnSortDirection } from "../../../../tools/constants";


@Component({
    selector: '[sortable-column]',
    templateUrl: './sortable-column.template.html'
})
export class SortableColumnComponent implements OnInit, OnDestroy {

    constructor(private tableColumnsSortService: TableColumnsSortService) { }

    @Input('sortable-column') columnName: string;

    @Input('sort-direction') sortDirection: ColumnSortDirection;

    private columnSortedSubscription: Subscription;

    @HostListener('click')
    sort() {
        this.toggleSortDirection(this.sortDirection);
        this.tableColumnsSortService.columnSorted({ sortColumn: this.columnName, sortDirection: this.sortDirection });
    }

    ngOnInit() {
        // subscribe to sort changes so we can react when other columns are sorted
        this.columnSortedSubscription = this.tableColumnsSortService.columnSorted$.subscribe(event => {
            // reset this column's sort direction to hide the sort icons
            if (this.columnName != event.sortColumn) {
                this.sortDirection = ColumnSortDirection.None;
            }
        });
    }

    toggleSortDirection(sortDirection: ColumnSortDirection) {
        this.sortDirection = sortDirection === ColumnSortDirection.Up ? ColumnSortDirection.Down : ColumnSortDirection.Up;
    }

    retrieveSortImageClass(sortDirection: ColumnSortDirection) {
        if (sortDirection === ColumnSortDirection.Up ||
            ColumnSortDirection[sortDirection] === ColumnSortDirection[ColumnSortDirection.Up]) {
            return 'erxSortUp';
        }

        if (sortDirection === ColumnSortDirection.Down ||
            ColumnSortDirection[sortDirection] === ColumnSortDirection[ColumnSortDirection.Down]) {
            return 'erxSortDown';
        }

        return '';
    }

    ngOnDestroy() {
        this.columnSortedSubscription.unsubscribe();
    }
}
