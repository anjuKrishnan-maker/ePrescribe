import { Directive, ElementRef, HostListener, Input } from '@angular/core';
@Directive({
    selector: '[headerEdit]'
})
export class HeaderEditDirective {

    private _isEditable: boolean = false;

    @Input('headerEdit') set isEditable(value: boolean) {
        this._isEditable = value;

    }
    constructor(private el: ElementRef) {
    }

    @HostListener('mouseenter') onMouseEnter() {
        this.Display();

    }
    @HostListener('mouseleave') onMouseLeave() {
        this.Hide();
    }

    @HostListener('click', ['$event.target']) onClick() {
        this.Hide();
    }

    
    Display() {
        if (this._isEditable) {
            let obj = $(this.el.nativeElement.parentElement).find('.editLi');
            if (obj === undefined)
            {
                obj = $(this.el.nativeElement).find('.editLi');
            }
            obj.removeClass('hide');
        }
    }

    Hide() {
        if (this._isEditable) {
            let obj = $(this.el.nativeElement.parentElement).find('.editLi');
            if (obj === undefined) {
                obj = $(this.el.nativeElement).find('.editLi');
            }
            obj.addClass('hide');
           // $(this.el.nativeElement.parentElement).find('.editLi')
        }
    }

}