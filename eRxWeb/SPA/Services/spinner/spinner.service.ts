import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class SpinnerService {

    private _isApiLoading = new Subject<boolean>();
    private _isPageLoading = new Subject<boolean>();

    public get isApiLoading(): Subject<boolean> {
        return this._isApiLoading;
    }

    public get isPageLoading(): Subject<boolean> {
        return this._isPageLoading;
    }

    public showApiSpinner() {
        this._isApiLoading.next(true);
    }

    public hideApiSpinner() {
        this._isApiLoading.next(false);
    }

    public showPageSpinner() {
        this._isPageLoading.next(true);
    }

    public hidePagespinner() {
        this._isPageLoading.next(false);
    }
}