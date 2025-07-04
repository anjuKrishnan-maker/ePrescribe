import { Component, Input, Output, EventEmitter  } from '@angular/core';
import { GridPaginationModel } from '../../../../model/model.import.def';
@Component({
    selector: 'erx-grid-pager',
    templateUrl: './grid-pager.template.html',
    styleUrls: ['./grid-pager.style.css']
})

export class GridPagerComponent {
    @Output() onPageChange = new EventEmitter<number>();
    @Input() Page: GridPaginationModel;
    PageChange(pageNo: number) {
        this.onPageChange.emit(pageNo);
    }
}