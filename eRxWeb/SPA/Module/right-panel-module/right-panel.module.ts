import { NgModule } from '@angular/core';
import { CommonModule, } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as RightPanelDefs from '../../component/common/right-panel/right-panel.component.import.def';
import * as ServiceDef from '../../services/service.import.def';
import { ModalPopupControl } from './../../component/shared/controls/modal-popup/modal-popup.control';
import { DirectiveModule} from '../../module/directive-module/directive.module';
import { PipeModule } from '../pipe.module';
import { PDMPDetailsComponent } from '../../component/shared/pdmp-details/pdmp-details.component';
import { SharedComponentsModule } from '../shared-components-module/shared-components.module';

@NgModule({
    imports: [CommonModule, FormsModule, PipeModule, DirectiveModule, SharedComponentsModule],
    declarations: [RightPanelDefs.RightPanelComponent, RightPanelDefs.EPCSNoticeReportComponent, RightPanelDefs.UrgentMessageComponent, RightPanelDefs.HelpComponent, RightPanelDefs.FormularyAlternativesComponent, PDMPDetailsComponent,
        RightPanelDefs.RobustLinksComponent, RightPanelDefs.CollapsiblePanelComponent, RightPanelDefs.ScriptPadComponent, RightPanelDefs.ImportantInformationComponent, RightPanelDefs.EAuthMessageComponent, RightPanelDefs.RightBoxComponent,
        RightPanelDefs.CopayCoverageComponent, RightPanelDefs.GenericAlternativesComponent, RightPanelDefs.PharmacyComponent, RightPanelDefs.NewPharmacyComponent, RightPanelDefs.SigCopayCoverageComponent, 
     RightPanelDefs.MessageQueInfoComponent, RightPanelDefs.HelpComponent,
    RightPanelDefs.MedHistoryCompletionComponent, RightPanelDefs.RxDetailComponent, RightPanelDefs.EpcsSendToPharmacyComponent,
    RightPanelDefs.MessageQueInfoComponent, RightPanelDefs.HelpComponent,
    RightPanelDefs.MedHistoryCompletionComponent, RightPanelDefs.RxDetailComponent,
        RightPanelDefs.FailedRegMsgComponent, RightPanelDefs.ProgramAlertComponent, RightPanelDefs.SpecialtyMedsComponent, RightPanelDefs.PrivacyOverrideReasonComponent,
        RightPanelDefs.DeniedRefReqMessageComponent, RightPanelDefs.FailedRxMessageComponent, RightPanelDefs.AuditLogInfoComponent, RightPanelDefs.PPTPlusSummary, RightPanelDefs.PPTPlusScriptPadSummary, RightPanelDefs.PPTPlusDetails,
        RightPanelDefs.PDMPSummary,  RightPanelDefs.GetEpcs, RightPanelDefs.PharmacySigAlertComponent
    ],
    providers: [ServiceDef.ScriptPadservice, ServiceDef.UrgentMessageService, ServiceDef.RobustLinksService,
    ServiceDef.RightBoxService, ServiceDef.MessageQueRxService, ServiceDef.LogRxService,
    ServiceDef.ImportantInformationService, ServiceDef.GenericAlternativeService, ServiceDef.EAuthMessageService,
    ServiceDef.EPCSService, ServiceDef.CopayCoverageServcie, ServiceDef.FormularyAlternativeService,
    ServiceDef.ComponentCommonService, ServiceDef.RxDetailService, ServiceDef.HelpContentService, ServiceDef.EpcsSendToPharmacyService,
    ServiceDef.ProgramAlertService, ServiceDef.PrivacyOverrideService, ServiceDef.DeniedRefReqMessagesService, ServiceDef.FailedRxMessagesService,
    ServiceDef.AuditViewInfoService, ServiceDef.PptPlusService, ServiceDef.PDMPService, ServiceDef.RightPanelService],
    exports: [RightPanelDefs.RightPanelComponent, ModalPopupControl]
   
})

class RightPanelModule { }

export { RightPanelModule };