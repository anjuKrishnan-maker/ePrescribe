import { Component, OnInit, Inject } from '@angular/core';
import { subscription, PricingStructurePrice, PricingStructureEnum } from './subscription.model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-subscription',
    templateUrl: './subscription.component.html',
    styleUrls: ['./subscription.component.css']
})
export class SubscriptionComponent implements OnInit {

    public model = new subscription();
    private appContext: any;
    constructor(@Inject('window') private window: any, private router: Router) {

    }

    redirectToCreateUser(pricingStructure: PricingStructureEnum) {
        this.router.navigate(["register/createuser"], { state: { SelectedPricingStructure: pricingStructure } });
    }

    redirectToLogin() {
        window.location.href = this.appContext?.login;
    }

    ngOnInit() {
        this.appContext = this.window?.appcontext;
        this.model.price = new PricingStructurePrice();
        this.model.price.basic = this.appContext?.basicPrice;
        this.model.price.deluxe = this.appContext?.deluxePrice;
        this.model.price.deluxeLogRx = this.appContext?.deluxeLogRxPrice;
        this.model.price.deluxeEpcs = this.appContext?.deluxeEpcsPrice;
        this.model.price.deluxeEpcsLogRx = this.appContext?.deluxeEpcsLogRxPrice;
        this.model.price.epcsSetup = this.appContext?.epcsSetupPrice;
        this.model.enterprisePricing = this.appContext?.enterprisePricing && this.appContext.enterprisePricing.toLowerCase() == 'true';
    }

}
