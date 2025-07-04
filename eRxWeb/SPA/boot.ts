
import {platformBrowserDynamic} from '@angular/platform-browser-dynamic';
import {enableProdMode} from '@angular/core';
import {AppModule} from './module/app.module';

//const platform = platformBrowserDynamic();
enableProdMode();
platformBrowserDynamic().bootstrapModule(AppModule);         

