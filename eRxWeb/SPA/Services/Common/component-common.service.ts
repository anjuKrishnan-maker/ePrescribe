import { Injectable }     from '@angular/core';
import {ComponentContext, ComponentItems} from './component.context';
import { IBaseComponent } from '../../component/common/base.component';
import { PAGE_NAME } from '../../tools/constants';

@Injectable()
export class ComponentCommonService {
    RightPanelSubComponents: ComponentContext;

    constructor() {
        this.initialize();
    }
    private initialize() {
        this.RightPanelSubComponents = new ComponentContext();
        this.RightPanelSubComponents.register(new ComponentItems('robust-link', [PAGE_NAME.SelectMedication], true), "RobustLinks");
        this.RightPanelSubComponents.register(new ComponentItems('formulary-alternatives', [PAGE_NAME.SelectMedication, 'ApproveRefillTask.aspx', 'PharmRefillDetails.aspx'], true), "FormularyAlternatives");
        this.RightPanelSubComponents.register(new ComponentItems('urgent-message', [PAGE_NAME.SelectPatient, 'DocRefillMenu.aspx', 'ListSendScripts.aspx', 'TaskSummary.aspx'], true), "UrgentMessage");
        this.RightPanelSubComponents.register(new ComponentItems('epcs-notice', [PAGE_NAME.SelectPatient, PAGE_NAME.ReviewHistory], true), "EpcsReportNotification");
        this.RightPanelSubComponents.register(new ComponentItems('generic-alternatives', [PAGE_NAME.SelectMedication, 'ApproveRefillTask.aspx', 'PharmRefillDetails.aspx'], true), "GenericAlternatives");
        this.RightPanelSubComponents.register(new ComponentItems('script-pad', [PAGE_NAME.SelectMedication, 'NurseSig.aspx', 'RxDURReviewMultiSelect.aspx','Sig.aspx'], true), "ScriptPad");
        this.RightPanelSubComponents.register(new ComponentItems('imp-info', [PAGE_NAME.SelectPatient, PAGE_NAME.ReviewHistory], true), "ImportantInformation");
        this.RightPanelSubComponents.register(new ComponentItems('e-auth-msg', [PAGE_NAME.SelectPatient, 'DocRefillMenu.aspx', 'ListSendScripts.aspx', 'NurseSig.aspx', 'TaskSummary.aspx'], true), "EAuthMessage");
        this.RightPanelSubComponents.register(new ComponentItems('right-box', [PAGE_NAME.SelectPatient,  PAGE_NAME.SelectMedication], true), "RightBox");
        this.RightPanelSubComponents.register(new ComponentItems('copay-coverage', ['ApproveRefillTask.aspx', PAGE_NAME.SelectMedication, 'RxDURReviewMultiSelect.aspx'], true), "CopayCoverage");

        this.RightPanelSubComponents.register(new ComponentItems('pharmacy-info', ['Pharmacy.aspx'], true), "Pharmacy");
        this.RightPanelSubComponents.register(new ComponentItems('ppt-plus-summary', [PAGE_NAME.SelectMedication], true), "PptPlusSummary");
        this.RightPanelSubComponents.register(new ComponentItems('new-pharmacy', ['Pharmacy.aspx'], true), "NewPharmacy");
        this.RightPanelSubComponents.register(new ComponentItems('sig-copay', ['Sig.aspx', 'NurseSig.aspx'], true), "SigCopayCov");
        this.RightPanelSubComponents.register(new ComponentItems('message-que', ['MessageQueueTx.aspx'], true), "MessageQue");
        this.RightPanelSubComponents.register(new ComponentItems('help-content', [PAGE_NAME.SelectMedication, 'MyProfile.aspx', PAGE_NAME.Settings, 'NursePharmacy.aspx','ScriptPad.aspx'
            , 'reports.aspx', 'Pharmacy.aspx', 'TaskScriptList.aspx', 'MultipleView.aspx', 'MultipleViewCSS.aspx', 'ChangePatientInsurance.aspx'
            , 'ChangeShieldUserPassword.aspx', PAGE_NAME.EditUser, 'NurseFreeFormDrug.aspx', 'FreeFormDrug.aspx', 'PatientMedHistory.aspx'
            , 'PharmRefillSummary.aspx', 'MyEPCSReports.aspx', 'PreBuiltMedSelectSig.aspx', 'RefillHistory.aspx', 'RxDURReview.aspx', 'SelectDx.aspx', 'SiteManagement.aspx', 'ApproveScriptMessagePatient.aspx', 
            , 'PatientDiagnosis.aspx', 'DocRefillMenu.aspx', 'PatientAllergy.aspx', PAGE_NAME.ReviewHistory, 'MessageQueueTx.aspx', 'NurseSig.aspx'
            , 'Sig.aspx', 'ListSendScripts.aspx', 'ApproveRefillTask.aspx', PAGE_NAME.SelectPatient, PAGE_NAME.Reports, PAGE_NAME.PatientUpload], true), "help-content");
        this.RightPanelSubComponents.register(new ComponentItems('epcs-send-to-pharmacy', ['ScriptPad.aspx'], true), "EpcsSendToPharmacy");
        this.RightPanelSubComponents.register(new ComponentItems('program-alert', [PAGE_NAME.SelectMedication], true), "ProgramAlert");
        this.RightPanelSubComponents.register(new ComponentItems('spec-med-info', ['ScriptPad.aspx'], true), "SpecMedInfo");
        this.RightPanelSubComponents.register(new ComponentItems('privacy-override-reason', ['ViewAuditLog.aspx'], true), "privacy-override-reason");
        this.RightPanelSubComponents.register(new ComponentItems('audit-log-info', ['ViewAuditLog.aspx'], true), "audit-log-info");
        this.RightPanelSubComponents.register(new ComponentItems('pdmp-summary', ['ScriptPad.aspx', PAGE_NAME.SelectPatient, PAGE_NAME.ApproveRefillTaskPage], true), "pdmp-summary");
        this.RightPanelSubComponents.register(new ComponentItems('getEpcs', [PAGE_NAME.ScriptPad], true), "getEpcs");

        //registering default open collapsible panel
        this.RightPanelSubComponents.registercomponentsWithDefaultOpenPanel('help-content', [PAGE_NAME.PatientUpload]);
    }

    IsVisible(key: string, page: string, targetComponent: IBaseComponent) {
        let comp = this.RightPanelSubComponents.get(key);
        let visible: boolean = false;
        if (comp != undefined) {
            let vComp = comp.Pages.filter((p) => { return p.toLocaleUpperCase() == page.toLocaleUpperCase() });
            visible = vComp != undefined && vComp.length > 0;
        }
        if (visible)
            targetComponent.Show();
        else
            targetComponent.Hide();

        return visible;
    }
    CheckVisibility(id: string,page: string): boolean {
        if (page == undefined || page.length == 0)
            return false;
        let comp = this.RightPanelSubComponents.getById(id);
        let visible: boolean = false;
        if (comp != undefined) {
            let vComp = comp.Pages.filter((p) => { return p.toLocaleUpperCase() == page.toLocaleUpperCase() });
            visible = vComp != undefined && vComp.length > 0;
        }
        return visible;
    }
    CheckDefaultOpenForPage(id: string, page: string): boolean {
        if (page == undefined || page.length == 0)
            return false;
        let components: Array<string> = this.RightPanelSubComponents.getComponentByKey(id);
        let defaultopen: boolean = components.includes(page);
        return defaultopen
    }

    public AddWindowFunction(key, func) {
        (<any>window)[key] = func;
    }
}

