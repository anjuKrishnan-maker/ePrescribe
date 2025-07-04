import { NgModule, APP_INITIALIZER } from '@angular/core';
import { CommonModule, LocationStrategy } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from '../component/app.component';
import { EPCSService } from '../services/epcs/epcs.service';
import { ComponentCommonService } from '../services/common/component-common.service';
import {
    MedHelpSearchService, EventService, AppInfoService, PharmacyService, ReviewHistoryService, FeatureComparisonService, CancelRxService, UserInfoService, ReportService,
    PatientService, ErrorService, UserPreferenceService, GoogleAnalyticsService, PatientMedHistoryService, FormularyOverrideService, EligAndMedHxStatusService,
    SelectPatientService, WelcomeTourService, IdmeTeaserAdService, SettingsService, UserSessionService, EcouponToPharmUncheckedService, ContentLoadService, SpinnerService, DeluxeFeatureSelectionService, PatientUploadService, ChartExtractService
} from '../services/service.import.def';
import { ContentPageComponent } from '../component/common/content-page/content-page.component';
import { FeatureComparisonComponent } from '../component/feature-comparison/feature-comparison.component';
import { PdmpEnrollmentComponent } from '../component/pdmp-enrollment/pdmp-enrollment.component';
import { pdmpEpcsTeaserComponent } from '../component/pdmp-epcs-teaser/pdmp-epcs-teaser.component';
import { MedSearchComponent } from '../component/common/med-search/med-search.component';
import { ModalPopupComponent } from '../component/common/modal-popup/modal-popup.component';
import { MenuComponent } from '../component/common/menu/menu.component';
import { SearchPatientComponent } from '../component/select-patient/select-patient.component';
import { SearchPatientModule } from './select-patient-module/select-patient.module';
import { RightPanelModule } from './right-panel-module/right-panel.module';
import { SelectMedicationModule } from './select-medication-module/select-medication.module';
import { FooterComponent } from '../component/common/footer/footer.component';
import { ReviewHistoryComponent } from '../component/review-history/review-history.component';
import { PatMedAndMedAllergyReconciliationComponent } from '../component/common/shared/pat-med-and-med-allergy-reconciliation/pat-med-med-allergy-reconciliation.component';
import { RxFillDetailsComponent } from '../component/rx-fill-details/rxfill-details.component';
import { PatientMedRecService } from "../services/patient-med-rec/patient-med-rec.service";
import { PipeModule } from './pipe.module';
import { MessageInfoComponent } from '../component/shared/message-info/message-info.component';
import { HtmlThroughOverlayComponent } from '../component/shared/html-through-overlay/html-through-overlay.component';
import { PatientProgramUcheckedOverlayComponent } from '../component/ecoupon-to-pharm-unchecked-warning/ecoupon-to-pharm-unchecked-warning.component';
import { ExternalHtmlThroughOverlayComponent } from '../component/shared/external-html-through-overlay/external-html-through-overlay.component';
import { PrivacyOverrideComponent } from '../component/common/shared/privacy-override/privacy-override.component';
import { WelcomeTourComponent } from '../component/shared/welcome-tour/welcome-tour.component';
import { DeluxeTeaserAdComponent } from '../component/shared/deluxe-teaser-ad/deluxe-teaser-ad.component';
import { SupervisingProviderComponent } from '../component/common/shared/supervising-provider-overlay/supervising-provider.component';
import { TableColumnsSortService } from '../services/sortable-column/table.columns.sort.service';
import { ClientSortService } from '../services/sortable-column/client.sort.service';
import { SortableTableDirective } from '../services/sortable-column/sortable-table';
import { SortableColumnComponent } from '../component/common/shared/sortable-column/sortable-column.component';
import { SettingsComponent } from '../component/settings/settings.component';
import { ChartExtractComponent } from '../component/chart-extract-download/chart-extract.component';
import { PatientUploadComponent } from '../component/patient-upload/patient-upload.component';
import { PdfViewerModalComponent } from '../component/shared/pdf-viewer-modal-popup/pdf-viewer-modal.component';
import { PdfViewerControl } from '../component/shared/controls/pdf-viewer/pdf-viewer.control';
import { SupervisingProviderPrompt } from '../component/common/shared/supervising-provider-prompt/supervising-provider-prompt.component';
import { CompletedRxCancelRxComponent } from '../component/common/shared/completed-rx-cancel-rx/completed-rx-cancel-rx.component';
import { CancelRxComponent } from '../component/common/shared/cancel-rx/cancel-rx.component';
import { LogRxComponent } from '../component/common/log-rx/log-rx.component';
import { DeluxeFeatureSelectionComponent } from '../component/deluxe-feature-selection/deluxe-feature-selection.component'
import { ReportModule } from './reports-module/report.module';
import { AppRoutingModule } from './app-routing.module';
import { ErxRequestInterceptor } from './erx-http-interceptor';
import { PAGE_NAME } from '../tools/constants';
import { RouteReuseStrategy } from '@angular/router';
import { ErxRouteReuseStrategy } from './custom-route-reuse-strategy';
import { ErxLocationStrategy } from './custom-location-strategy';
import { UserModule } from './user-module/user.module';
import { SharedComponentsModule } from './shared-components-module/shared-components.module';
import { HeaderModule } from './header-module/header.module';
import { HomeAddressModule } from './home-address-module/home-address.module';

export function initialContentLoading(contentLoadService: ContentLoadService) {
    return () => {
        return contentLoadService
            .RetrieveInitialPayload(PAGE_NAME.SelectPatient)
    };
}
@NgModule({
    imports: [AppRoutingModule, CommonModule, BrowserModule, FormsModule, HeaderModule, PipeModule, RightPanelModule, SelectMedicationModule, HttpClientModule, HttpModule, ReactiveFormsModule, HttpClientModule, SearchPatientModule, SharedComponentsModule, ReportModule, UserModule, HomeAddressModule],
    declarations: [AppComponent, ContentPageComponent, MenuComponent, SearchPatientComponent, ModalPopupComponent, MedSearchComponent, FooterComponent, ReviewHistoryComponent,
        PatMedAndMedAllergyReconciliationComponent, FeatureComparisonComponent, PdmpEnrollmentComponent, pdmpEpcsTeaserComponent, MessageInfoComponent, RxFillDetailsComponent,
         HtmlThroughOverlayComponent, ExternalHtmlThroughOverlayComponent, PdfViewerModalComponent, PdfViewerControl, SupervisingProviderPrompt,
        CompletedRxCancelRxComponent, CancelRxComponent, PrivacyOverrideComponent, WelcomeTourComponent, DeluxeTeaserAdComponent,
        SettingsComponent, SupervisingProviderComponent, SortableColumnComponent, SortableTableDirective, PatientProgramUcheckedOverlayComponent, LogRxComponent, DeluxeFeatureSelectionComponent, PatientUploadComponent, ChartExtractComponent],

    providers: [{
        provide: APP_INITIALIZER,
        useFactory: initialContentLoading,
        multi: true,
        deps: [ContentLoadService]
    }, {
        provide: RouteReuseStrategy,
        useClass: ErxRouteReuseStrategy
    }, {
        provide: HTTP_INTERCEPTORS,
        useClass: ErxRequestInterceptor,
        multi: true
        },
        {
            provide: LocationStrategy,
            useClass: ErxLocationStrategy
        },       
        PatientService, UserInfoService, EPCSService, ComponentCommonService, EventService,
        MedHelpSearchService, AppInfoService, PharmacyService, ReviewHistoryService, FeatureComparisonService,
        CancelRxService, PatientMedRecService, ErrorService, UserPreferenceService, GoogleAnalyticsService,
        PatientMedHistoryService, FormularyOverrideService, EligAndMedHxStatusService, SelectPatientService,
        WelcomeTourService, SettingsService, TableColumnsSortService, ClientSortService, UserSessionService, EcouponToPharmUncheckedService,
        IdmeTeaserAdService, ReportService, ContentLoadService, SpinnerService, DeluxeFeatureSelectionService, PatientUploadService, ChartExtractService],
    bootstrap: [AppComponent]
})
export class AppModule { }
