import { ContentRefreshPayload, UserPreferenceModel, SiteInfo, CommonUiInitialPayload, AppInfoModel, MenuItemModel, SearchPatientStartupParameters, MedHelpSearchModel } from './model.import.def';

export class InitalContentPayload {
    ContentRefreshPayload: ContentRefreshPayload;
    UserPreferencePayload: UserPreferenceModel;
    SitePayload: SiteInfo;
    CommonUiInitialPayload: CommonUiInitialPayload;
    FooterPayload: AppInfoModel;
    MenuPayload: MenuItemModel[];
    SessionTimeoutPayload: number;
    SelectPatientPayload: SearchPatientStartupParameters;
    HelpSearchPayload: MedHelpSearchModel;
}