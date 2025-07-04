import { Component, ElementRef } from '@angular/core';
import { FeatureComparisonService, EventService, ComponentCommonService } from '../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../tools/constants';
import { FeatureComparisonModel } from '../../model/model.import.def';

@Component({
    selector: 'erx-feature-comparison',
    templateUrl: './feature-comparison.template.html'
})

export class FeatureComparisonComponent {
    featureComparisonModel: FeatureComparisonModel = new FeatureComparisonModel();
    constructor(private fcSvc: FeatureComparisonService, comonSvc: ComponentCommonService,
        private svE: EventService
    ) {
        comonSvc.AddWindowFunction('LoadFeatureComparisonComponent', () => {
            this.GetFeatureComparisonUrl();
        });
    }

    GetFeatureComparisonUrl() {
        this.fcSvc.GetFeatureComparisonUrl().subscribe(response => {
            this.featureComparisonModel.ImageUrl = response;
                $("#mdlFeatureComparision").modal();
            }
        )
    }
}

