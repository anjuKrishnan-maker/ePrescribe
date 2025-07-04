import { Directive, ElementRef, Input } from '@angular/core';
@Directive({
    selector: '[comp-visible]'
})
export class VisibleDirective {
    private _vis: boolean;
    @Input('comp-visible') set isVisible(value: boolean) {
        this._vis = value;
        this.el.nativeElement.style.display = value ? '' : 'none';
    }
    constructor(private el: ElementRef) {
        el.nativeElement.style.display =this.isVisible? '':'none';
    }
   
}