import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SearchPatientComponent } from '../component/select-patient/select-patient.component';
import { SelectMedicationComponent } from '../component/select-medication/select-medication.component';
import { ReviewHistoryComponent } from '../component/review-history/review-history.component';
import { SettingsComponent } from '../component/settings/settings.component';
import { ReportListComponent } from '../component/reports/report-list.component';
import { ContentPageComponent } from '../component/common/content-page/content-page.component';
import { ROUTE_NAME } from '../tools/constants';
import { DeluxeFeatureSelectionComponent } from '../component/deluxe-feature-selection/deluxe-feature-selection.component';
import { PatientUploadComponent } from '../component/patient-upload/patient-upload.component';
import { ChartExtractComponent } from '../component/chart-extract-download/chart-extract.component';
import { UserComponent } from '../component/user/user.component';
import { HomeAddressComponent } from '../component/home-address/home-address.component';

const mainApplicationRoutes: Routes = [

    { path: ROUTE_NAME.SelectPatient, component: SearchPatientComponent },
    { path: '', redirectTo: ROUTE_NAME.SelectPatient, pathMatch: 'full' },
    { path: 'Spa.aspx', redirectTo: ROUTE_NAME.SelectPatient, pathMatch: 'full' },
    

    //Patient - Medication Workflow
    { path: ROUTE_NAME.SelectMedication, component: SelectMedicationComponent },
    { path: ROUTE_NAME.SelectDiagnosis, component: ContentPageComponent },
    { path: ROUTE_NAME.Scriptpad, component: ContentPageComponent },
    { path: ROUTE_NAME.ReviewHistory, component: ReviewHistoryComponent },
    { path: ROUTE_NAME.AddPatient, component: ContentPageComponent },
    { path: ROUTE_NAME.Sig, component: ContentPageComponent },
    { path: ROUTE_NAME.NurseSig, component: ContentPageComponent },
    { path: ROUTE_NAME.FreeFormDrug, component: ContentPageComponent },
    { path: ROUTE_NAME.SelfReportedMed, component: ContentPageComponent },
    { path: ROUTE_NAME.PdfInPage, component:ContentPageComponent},

    //Tasking Pages
    { path: ROUTE_NAME.Tasks, component: ContentPageComponent },

    //Deluxe n Billing Pages
    { path: ROUTE_NAME.ManageAccount, component: ContentPageComponent },
    { path: ROUTE_NAME.DeluxeFeatures, component: DeluxeFeatureSelectionComponent },
    { path: ROUTE_NAME.GetEpcs, component: DeluxeFeatureSelectionComponent },
    { path: ROUTE_NAME.DeluxeBilling, component: ContentPageComponent },

    //Reports
    { path: ROUTE_NAME.Reports, component: ReportListComponent },
    { path: ROUTE_NAME.MultipleViewReport, component: ContentPageComponent },
    { path: `${ROUTE_NAME.Reports}/:reportdetail`, component: ContentPageComponent },
    { path: ROUTE_NAME.EPCSDailyReport, component: ContentPageComponent },
    { path: ROUTE_NAME.MyEPCSReport, component: ContentPageComponent },

    //Settings
    { path: ROUTE_NAME.Settings, component: SettingsComponent },
    { path: ROUTE_NAME.PatientUpload, component: PatientUploadComponent },
    { path: ROUTE_NAME.ChartExtractDownload, component: ChartExtractComponent },
    { path: `${ROUTE_NAME.Settings}/:specificsetting`, component: ContentPageComponent },
    { path: ROUTE_NAME.ForcePasswordSetup, component: ContentPageComponent },
    { path: ROUTE_NAME.EditUsers, component: ContentPageComponent },
    
    //User
    { path: ROUTE_NAME.EditUser, component: UserComponent },   
    { path: ROUTE_NAME.RxFavourites, component: ContentPageComponent },
    { path: ROUTE_NAME.HomeAddress, component: HomeAddressComponent },

    //Exception
    { path: ROUTE_NAME.Exception, component: ContentPageComponent },

    //Others
    { path: ROUTE_NAME.Library, component: ContentPageComponent },
    { path: ROUTE_NAME.MyProfile, component: ContentPageComponent },

    { path: ROUTE_NAME.MessageQueueTx, component: ContentPageComponent },
];


@NgModule({
    imports: [RouterModule.forRoot(mainApplicationRoutes, { onSameUrlNavigation: 'reload' })],
    providers: [],
    exports: [RouterModule]
})
export class AppRoutingModule {
}


