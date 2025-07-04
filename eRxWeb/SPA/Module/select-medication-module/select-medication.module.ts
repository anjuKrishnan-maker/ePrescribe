import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { GridPagerComponent } from '../../component/shared/controls/pager/grid-pager.component';
import { PipeModule } from '../pipe.module';
import { PatientMedHistoryComponent } from '../../component/common/shared/patient-medication-history/patient-medication-history.component';
import { SelectMedicationService, SelectMedicationSearchService } from '../../services/service.import.def';
import { SelectMedicationComponent } from '../../component/select-medication/select-medication.component';
import { SelectMedicationSearchComponent } from '../../component/select-medication/select-medication-search/select-medication-search.component';
import { SelectMedicationGridComponent } from '../../component/select-medication/select-medication-grid/select-medication-grid.component';
import { SelectMedicationActionBarComponent } from '../../component/select-medication/select-medication-action-bar/select-medication-action-bar.component';
import { SelectMedicationFormularyOverrideComponent } from '../../component/select-medication/select-medication-formulary-override/select-medication-formulary-override.component';
import { DirectiveModule } from '../directive-module/directive.module';
import { SharedComponentsModule } from '../shared-components-module/shared-components.module';

@NgModule({
    imports: [CommonModule, BrowserModule, PipeModule, FormsModule, SharedComponentsModule, DirectiveModule],
    declarations: [SelectMedicationFormularyOverrideComponent, PatientMedHistoryComponent, SelectMedicationComponent, SelectMedicationGridComponent, SelectMedicationSearchComponent, GridPagerComponent, SelectMedicationActionBarComponent],
    providers: [SelectMedicationService, SelectMedicationSearchService],
    exports: [SelectMedicationComponent, SelectMedicationSearchComponent]
   
})
class SelectMedicationModule { }

export { SelectMedicationModule };