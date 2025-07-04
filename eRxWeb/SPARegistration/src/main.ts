import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';

//if (environment.production) {//Until we figure out the file replacement in webpack config.
enableProdMode();


platformBrowserDynamic().bootstrapModule(AppModule)
    .catch(err => console.error(err));
