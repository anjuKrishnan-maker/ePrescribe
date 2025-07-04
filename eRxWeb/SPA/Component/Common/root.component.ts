import { OnDestroy } from '@angular/core';

export class EventSubscriptionDisposer implements OnDestroy{
    listOfSubscription: Array<any> = [];
    constructor() {
        this.listOfSubscription = [];
    }
    ngOnDestroy() {
        for (let i =0; i < this.listOfSubscription.length; i++) {
            this.listOfSubscription[i].unsubscribe();
        }
    }
}