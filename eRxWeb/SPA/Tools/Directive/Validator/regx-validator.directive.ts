import { Directive, forwardRef, ElementRef, Input } from '@angular/core';
import { NG_VALIDATORS } from '@angular/forms';
import { Validator, AbstractControl } from '@angular/forms';
@Directive({
    selector: '[form-regx][formControlName], [form-regx][formControl],[form-regx][ngModel]',
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => RegxValidatorDirective), multi: true }
    ]
})
export class RegxValidatorDirective implements Validator {
    private pattern: string;
    @Input('form-regx') set Pattern(value: string) {
        this.pattern = value;
    }
    constructor(private el: ElementRef) {
    }

    validate(control: AbstractControl) {
        if (control != null && control.value != null) {
            let val = control.value.match(this.pattern) ? null : {
                formRegx: {
                    valid: false
                }
            };


            return val;
        }
        return null;
    }
}