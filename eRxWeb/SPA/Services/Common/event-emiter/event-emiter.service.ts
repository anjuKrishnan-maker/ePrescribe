import { Injectable, EventEmitter } from '@angular/core';

@Injectable()
class EventService {
    private _emitters: { [ID: string]: EventEmitter<any> } = {};
    
    get(ID: string): EventEmitter<any> {
        if (!this._emitters[ID])
            this._emitters[ID] = new EventEmitter();
        return this._emitters[ID];
    }

    invokeEvent<T>(ID: string, data: T|any) {
        if (this._emitters[ID]) {            
            this._emitters[ID].emit(data);
            return this._emitters[ID];
        }
        return null;
    }
    subscribeEvent<T>(ID: string, callBack: (data: T) => void, listSubscription: Array<any> = null) {

        if (!this._emitters[ID])
            this._emitters[ID] = new EventEmitter<T>();
        let subscription = this._emitters[ID].subscribe(callBack)
        if (listSubscription !== undefined && listSubscription !== null)
            listSubscription.push(subscription);
        return this._emitters[ID];
    }
}

export { EventService };