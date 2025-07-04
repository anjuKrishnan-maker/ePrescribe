import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class LoaderService {

    public isLoading = new Subject<boolean>();
    private _overrideLoading: boolean;
    public get overrideLoading(): boolean {
        return this._overrideLoading;
    }

    public show(forceOverrideLoader: boolean = false) {
        this._overrideLoading = forceOverrideLoader;
        this.isLoading.next(true);
    }

    public hide() {
        this.isLoading.next(false);
    }
}