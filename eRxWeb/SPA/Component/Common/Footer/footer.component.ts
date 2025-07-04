import { Component, OnInit } from '@angular/core';
import { EventService, ContentLoadService } from '../../../services/service.import.def';
import { current_context } from '../../../tools/constants';
import { AppInfoModel, InitalContentPayload } from '../../../model/model.import.def';
import Constants = require("../../../tools/constants");

@Component({
    selector: 'erx-footer',
    templateUrl: './footer.template.html'
})
export class FooterComponent implements OnInit {
    appModel: AppInfoModel = new AppInfoModel();
    constructor(private svE: EventService, private contentLoadService: ContentLoadService) {
        
    }

    ngOnInit() {
        if (this.contentLoadService.initalContentPayload.FooterPayload !== undefined) {
            this.appModel = this.contentLoadService.initalContentPayload.FooterPayload;
        }
    }
}
