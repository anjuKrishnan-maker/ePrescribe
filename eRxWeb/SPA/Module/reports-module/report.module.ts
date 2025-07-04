// import the new component
import { NgModule, ErrorHandler } from '@angular/core';
import { ReportListComponent } from '../../component/reports/report-list.component';
import { DeluxeReportComponent } from '../../component/deluxe-report-teaser/deluxe-report-teaser.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DeluxeModalPopupControl } from './../../component/shared/controls/deluxe-modal-popup/deluxe-modal-popup.control';

@NgModule({
    imports: [CommonModule, FormsModule],
    declarations: [ReportListComponent, DeluxeReportComponent, DeluxeModalPopupControl],    
    exports: [ReportListComponent, DeluxeModalPopupControl]
})
export class ReportModule { }

