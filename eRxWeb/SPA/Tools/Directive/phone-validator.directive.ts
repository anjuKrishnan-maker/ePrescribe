import { Directive, forwardRef, HostListener, ElementRef, Input } from '@angular/core';
import { NG_VALIDATORS } from '@angular/forms';
import { Validator, AbstractControl } from '@angular/forms';
@Directive({
    selector: '[phone-validate][formControlName],[phone-validate][formControl],[phone-validate][ngModel]',
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => PhoneValidatorDirective), multi: true }
    ]
})
export class PhoneValidatorDirective implements Validator {
    private numberRegex: RegExp = new RegExp(/[0-9]$/g);
    private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', '-', 'ArrowLeft', 'ArrowRight', 'Delete', '.'];
    @Input() isRequired: boolean = false;

    constructor(private el: ElementRef) {
    }
    @HostListener('keydown', ['$event'])
    onKeyDown(event: KeyboardEvent) {
        // Allow Backspace, tab, end, and home keys
        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }
        let current: string = this.el.nativeElement.value;
        if (event.key && !event.key.match(this.numberRegex)) {
                    event.preventDefault();                   
                }
    }

    validate(c: AbstractControl) {
        let val = (c.value == undefined || c.value == null || (c.value != undefined && c.value.length == 0)) && (this.isRequired == true) ?
            {
                formRequired: {
                    valid: false
                }
            } :
            null;

        return val;;


    }
}