import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from '../../component/common/header/header.component';
import { DirectiveModule } from '../directive-module/directive.module';
import { PatientInfoComponent } from '../../component/common/header/patient-info/patient-info.component';
import { SiteInfoComponent } from '../../component/common/header/site-info/site-info.component';
import { RightHeaderComponent } from '../../component/common/header/header-tool/right-header-tool.component';
import { PopoverControl } from '../../component/shared/controls/popover/popover.control';
import { HeaderEditDirective } from '../../tools/directive/directive.def';
import { PharmacyDetailsComponent } from '../../component/common/shared/pharmacy-details/pharmacy-details.component';

@NgModule({

    imports: [CommonModule, DirectiveModule],
    declarations: [PatientInfoComponent, SiteInfoComponent, RightHeaderComponent, HeaderComponent, HeaderEditDirective, PharmacyDetailsComponent, PopoverControl],
    exports: [PatientInfoComponent, SiteInfoComponent, RightHeaderComponent, HeaderComponent, DirectiveModule, PopoverControl]
})
export class HeaderModule { }