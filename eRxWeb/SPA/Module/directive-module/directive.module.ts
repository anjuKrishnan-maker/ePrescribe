import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { VisibleDirective, RequiredValidatorDirective, RegxValidatorDirective, PhoneValidatorDirective, EqualValidator, LengthValidatorDirective } from '../../tools/directive/directive.def';
import { ConverToDatePipe } from '../../tools/date-conver.pipe';
@NgModule({
    declarations: [RequiredValidatorDirective, RegxValidatorDirective, PhoneValidatorDirective, EqualValidator, VisibleDirective, LengthValidatorDirective, ConverToDatePipe],
    imports: [CommonModule],
    exports: [RequiredValidatorDirective, RegxValidatorDirective, EqualValidator, PhoneValidatorDirective, VisibleDirective]
})

export class DirectiveModule { }