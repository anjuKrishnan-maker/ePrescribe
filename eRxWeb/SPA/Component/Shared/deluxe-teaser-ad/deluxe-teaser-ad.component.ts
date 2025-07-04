import { Component, Input } from '@angular/core';
import { DeluxeTeaserAdModel, TeaserAdResponse } from '../../../model/model.import.def';
import { EventService, ComponentCommonService } from '../../../services/service.import.def';
import { PAGE_NAME, ROUTE_NAME } from "../../../tools/constants";
import { Router } from '@angular/router';

@Component({
    selector: 'erx-deluxe-teaser-ad',
    templateUrl: './deluxe-teaser-ad.template.html',
    styleUrls: []
})

export class DeluxeTeaserAdComponent {

    @Input() DeluxeTeaserAdModel: DeluxeTeaserAdModel;

    public adType: number; // BasicPromo or TeaserAd
    public teaserAdHtmlContent: Array<TeaserAdResponse> = [];
    public teaserAdHtmlString: string;

    constructor(private evSvc: EventService, private compSvc: ComponentCommonService,  private router: Router) {
        this.adType = 0;
        this.teaserAdHtmlString = '';    

        this.compSvc.AddWindowFunction('LoadDeluxeFeatureSelection', (componentParameters) => {
            $("#mdlDeluxeTeaserAd").modal('hide');
            this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures); 

        })
    }  

    public openModal() {
        this.getDeluxeTeaserAdData();
        $("#mdlDeluxeTeaserAd").modal('show');
    }

    private getDeluxeTeaserAdData() {
        if (this.DeluxeTeaserAdModel !== undefined) {
            if (this.DeluxeTeaserAdModel.IsCompulsaryBasic) {
                this.adType = 1;
            } else {
                this.adType = 2;
                this.teaserAdHtmlContent = this.DeluxeTeaserAdModel.TeaserAdContent;
            }
        }
    }

    //
    // Navigation/Action events
    //
    private btnUpgradeClicked() {
        this.closeModal();
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures); 
    }

    public closeModal() {
        $("#mdlDeluxeTeaserAd").modal('hide');
    }

    private btnCloseTeaserAd(idField: number) {
        if (this.teaserAdHtmlContent.length > 1)
            this.teaserAdHtmlContent = this.teaserAdHtmlContent.filter(ad => ad.idField !== idField);        
        else
            this.closeModal();
    }    
}