
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class GoogleAnalyticsService  {
    SendPageView(page: string) {
        if ((<any>window).isGaEnabled == 1) {
            (<any>window).ga('send', {
                hitType: 'pageview',
                page: page
            });
        }
    }

    SendTiming(timingCategory: string, timingVar: string, timingValue:number) {
        if ((<any>window).isGaEnabled == 1) {
            (<any>window).ga('send', {
                hitType: 'timing',
                timingCategory: timingCategory,
                timingVar: timingVar,
                timingValue: timingValue
            });
        }
    }
}