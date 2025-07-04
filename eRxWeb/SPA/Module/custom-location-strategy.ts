import { LocationStrategy } from "@angular/common";
import { Injectable } from "@angular/core";



@Injectable()
export class ErxLocationStrategy extends LocationStrategy {

    path(includeHash?: boolean): string { return ''; }
    prepareExternalUrl(internal: string): string { return null; }         
    forward(): void { }
    back(): void { }
    onPopState(fn: import("@angular/common").LocationChangeListener): void { }      
    getBaseHref(): string { return '';}   
    pushState(state: any, title: string, url: string, queryParams: string): void {
        //Below method is used push the state to the history and show the url in browser address bar
        history.pushState(null, null, "");
    }
    replaceState(state: any, title: string, url: string, queryParams: string): void {
        history.replaceState(null, null, "");
    }    
}
