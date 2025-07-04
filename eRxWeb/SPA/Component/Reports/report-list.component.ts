import { Component, OnInit } from '@angular/core';
import { EventService } from '../../services/service.import.def';
import { ReportService } from '../../services/report/report.service';
import { PAGE_NAME, ROUTE_NAME } from '../../tools/constants';
import { ReportMenuModel } from '../../model/Reports/report-menu.model';
import { Router } from '@angular/router';

@Component({
    selector: 'erx-report-list',
    templateUrl: './report-list.template.html',
    styleUrls: ['./report-list.style.css']
})

export class ReportListComponent implements OnInit {

    public reportMenu: ReportMenuModel;

    constructor(private eventService: EventService,
        private reportSvc: ReportService, private router: Router) {

    }

    ngOnInit(): void {
        this.getReportList();
    }

    public showDeluxeAlert: boolean = false;

    private getReportList() {
        if (!this.reportMenu) {
            this.reportSvc.GetReportsList()
                .subscribe((reportMenu: ReportMenuModel) => {
                    this.reportMenu = reportMenu;
                });
        }
    }

    public onCloseDeluxeReportTeaser() {
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures);
    }

    public navigateToReport(reportPage: string, showDeluxeAd: boolean) {
        this.showDeluxeAlert = false;
        if (showDeluxeAd)
            setTimeout(() => { this.showDeluxeAlert = true; }, 0);
        else
            this.router.navigateByUrl(`${ROUTE_NAME.Reports}/${reportPage.replace(".aspx", '').toLowerCase()}`,
                { state: { navigateTo: reportPage } });

    }
}