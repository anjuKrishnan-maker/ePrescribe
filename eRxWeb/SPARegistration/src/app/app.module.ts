import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RegistrationModule } from './registration/registration.module';
import { DataService } from './service/data.service';



import { LoaderService } from './service/loader.service';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { LoaderInterceptor } from './loader.interceptor';
import { GlobalErrorHandler } from './global.errorhandler';
import { MessageService } from './service/message.service';

export function getWindow() {
    return (typeof window !== "undefined") ? window : null;
}

@NgModule({
    declarations: [
        AppComponent

    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        RegistrationModule,
        HttpClientModule
    ],
    providers: [LoaderService, DataService, MessageService,
        { provide: 'window', useFactory: getWindow },
        { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
        { provide: ErrorHandler, useClass: GlobalErrorHandler }],
    bootstrap: [AppComponent]
})
export class AppModule { }
