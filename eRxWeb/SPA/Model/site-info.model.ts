import {User} from './userInfo.model';

export class SiteInfo {
    siteName: string;
    siteDetails: string;
    user: User;
    SelectSiteUrl: string;
    IsMultipleSite: boolean;
    IsTieAdsEnabled: boolean;
    IsRestrictedMenu: boolean;
}
