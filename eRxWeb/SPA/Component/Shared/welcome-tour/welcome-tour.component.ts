import { Component, OnInit } from '@angular/core';
import { SafeUrl, DomSanitizer } from '@angular/platform-browser';
import { WelcomeTourModel, ApiResponse } from '../../../model/model.import.def';
import { EventService, WelcomeTourService } from '../../../services/service.import.def';

@Component({
    selector: 'erx-welcome-tour',
    templateUrl: './welcome-tour.template.html',
    styleUrls: ['./welcome-tour.style.css']
})

export class WelcomeTourComponent implements OnInit {
    public welcomeTourModel: WelcomeTourModel;
    public frameSource: SafeUrl;
    public tourType: number = 0;

    constructor(private evSvc: EventService, private wtSvc: WelcomeTourService, private sanitizer: DomSanitizer, private evtSvc: EventService) {
    }

    ngOnInit() {

    }

    public OpenModal() {
        this.getWelcomeTourData();

        $("#mdlWelcomeTour").modal({ backdrop: 'static', keyboard: false });
        $("#mdlWelcomeTour").modal('show');
    }

    private getWelcomeTourData() {
        if (this.welcomeTourModel && this.welcomeTourModel.SourceUrl.length > 0) {
            this.frameSource = this.sanitizer.bypassSecurityTrustResourceUrl(this.welcomeTourModel.SourceUrl);
            this.tourType = this.welcomeTourModel.TourType;
        }
    }

    private welcomeTourDoNotShowAgain() {
        this.wtSvc.WelcomeTourDoNotShowAgain(this.tourType).subscribe((r) => {
            let response = r as ApiResponse;
            if (response.ErrorContext !== null && response.ErrorContext != undefined) {
                return;
            }
        });
    }

    //
    // Navigation/Action events
    //
    public btnSkipForNowClicked() {
        $("#mdlWelcomeTour").modal('hide');
    }
    public btnDoNotShowAgainClicked() {
        $("#mdlWelcomeTour").modal('hide');
        this.welcomeTourDoNotShowAgain();
    }
}