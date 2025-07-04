import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HomeAddressComponent } from '../../component/home-address/home-address.component';
import { HomeAddressService } from '../../services/service.import.def';
import { SharedComponentsModule } from '../shared-components-module/shared-components.module';
import { BrowserModule } from '@angular/platform-browser';
import { PipeModule } from '../pipe.module';
import { DirectiveModule } from '../directive-module/directive.module';


@NgModule({
    imports: [CommonModule, BrowserModule, PipeModule, FormsModule, SharedComponentsModule, DirectiveModule],
    declarations: [HomeAddressComponent],
    providers: [HomeAddressService],
    exports: []
})
export class HomeAddressModule { }
