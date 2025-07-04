import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { PatientSearchBar } from '../../component/shared/patient-search-bar/patient-search-bar.component';
import { CreditCardExpiringComponent } from '../../component/common/shared/credit-card-expiring-warning/creditcard-expiring.component';
import { SharedComponentsModule } from '../shared-components-module/shared-components.module';

@NgModule({
    imports: [CommonModule, BrowserModule, FormsModule, SharedComponentsModule],
    declarations: [PatientSearchBar, CreditCardExpiringComponent],
    exports: [PatientSearchBar, CreditCardExpiringComponent]

})
class SearchPatientModule { }

export { SearchPatientModule };