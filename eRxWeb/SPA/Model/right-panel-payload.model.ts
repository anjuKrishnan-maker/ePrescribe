import { UrgentMessageModel, EPCSNotice, ImportantInfoModel, EAuthMessageModel, RightBoxModel, ScriptPadModel, FormularyAlternative, EpcsSendToPharmacyModel } from './model.import.def';

export class RightPanelPayload {
    UrgentMessagePayload: UrgentMessageModel;
    EPCSNoticePayload: EPCSNotice;
    ImportantInfoPayload: ImportantInfoModel[];
    EAuthMessagePayload: EAuthMessageModel;
    RightBoxPayload: RightBoxModel;
    HelpContentPayload: string;
    ScriptPadPayload: ScriptPadModel[];
    SigCopayPayload: FormularyAlternative;
    EpcsSendToPharmPayload: EpcsSendToPharmacyModel;
    SpecialtyMedInfoPayload: boolean;
    GetEpcsPayload: boolean;
}