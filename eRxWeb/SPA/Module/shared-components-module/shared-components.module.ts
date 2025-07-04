import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { EligAndMedHxStatusComponent } from '../../component/common/shared/elig-and-med-hx/elig-and-medhx-status.component';
import { ModalPopupControl } from './../../component/shared/controls/modal-popup/modal-popup.control';
import { SelectMedicationUrlModalPopupComponent } from '../../component/select-medication/select-medication-url-modal-popup/select-medication-url-modal-popup.component';
import { DirectiveModule } from '../directive-module/directive.module';
import { PipeModule } from '../pipe.module';
import { MessageComponent } from '../../component/common/shared/message/message.component';
import { ChangePasswordPopupComponent } from '../../component/shared/change-password-popup/change-password-popup.component';

@NgModule({
    imports: [CommonModule, FormsModule, BrowserModule, DirectiveModule, PipeModule],
    declarations: [ModalPopupControl, EligAndMedHxStatusComponent, SelectMedicationUrlModalPopupComponent, MessageComponent, ChangePasswordPopupComponent],
    exports: [ModalPopupControl, EligAndMedHxStatusComponent, SelectMedicationUrlModalPopupComponent, MessageComponent, ChangePasswordPopupComponent]

})
class SharedComponentsModule { }

export { SharedComponentsModule };