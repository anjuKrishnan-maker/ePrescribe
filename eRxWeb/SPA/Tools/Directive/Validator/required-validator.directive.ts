import { Directive, forwardRef } from '@angular/core';
import { NG_VALIDATORS } from '@angular/forms';
import { Validator, AbstractControl } from '@angular/forms';
@Directive({
     selector: '[form-required][formControlName], [form-required][formControl],[form-required][ngModel]',
    providers: [
        { provide: NG_VALIDATORS, useExisting:  RequiredValidatorDirective, multi: true }
    ]
})
export class RequiredValidatorDirective implements Validator {
    private _vis: boolean; 
   
    constructor() {
    }

    validate(c: AbstractControl) {
        let val = c.value == undefined || c.value == null|| (c.value != undefined && c.value.length == 0)  ? {
            formRequired: {
                valid: false
            }
        } : null

        return val;;


    }

}