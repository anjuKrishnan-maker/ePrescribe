import { Directive, forwardRef, ElementRef, Input } from '@angular/core';
import { NG_VALIDATORS } from '@angular/forms';
import { Validator, AbstractControl } from '@angular/forms';
@Directive({
    selector: '[form-length][formControlName], [form-length][formControl],[form-length][ngModel]',
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => LengthValidatorDirective), multi: true }
    ]
})
export class LengthValidatorDirective implements Validator {
    private maxLength: number;
    @Input('form-length') set MaxLength(value: number) {
        this.maxLength = value;
    }
    constructor(private el: ElementRef) {
    }

    validate(control: AbstractControl) {
        if (control != null && control.value != null) {
            let val = control.value.length <= this.maxLength  ? null : {
                formLength: {
                    valid: false
                }
            };


            return val;
        }
        return null;
    }
}