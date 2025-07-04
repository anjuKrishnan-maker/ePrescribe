import { ROUTE_NAME, PAGE_NAME } from '../../tools/constants';
type PageRoute = { page: string; route: string };
export default class RouteNameExtractor {

    static navigationRoutes: PageRoute[] = [
        { page: PAGE_NAME.Settings.toLowerCase(), route: ROUTE_NAME.Settings },
        { page: PAGE_NAME.Reports.toLowerCase(), route: ROUTE_NAME.Reports },
        { page: PAGE_NAME.Patient.toLowerCase(), route: ROUTE_NAME.SelectPatient },
        { page: PAGE_NAME.IntegrationSolutionsList.toLowerCase(), route: ROUTE_NAME.ManageAccount },
        { page: PAGE_NAME.Library.toLowerCase(), route: ROUTE_NAME.Library },
        { page: PAGE_NAME.Tasks.toLowerCase(), route: ROUTE_NAME.Tasks },
        { page: PAGE_NAME.ListSendScripts.toLowerCase(), route: ROUTE_NAME.Tasks },
        { page: PAGE_NAME.MyProfile.toLowerCase(), route: ROUTE_NAME.MyProfile },
        { page: PAGE_NAME.DeluxeFeatureSelectionPage.toLowerCase(), route: ROUTE_NAME.DeluxeFeatures },
        { page: PAGE_NAME.DeluxeAccountManagement, route: ROUTE_NAME.ManageAccount },        
        { page: PAGE_NAME.MessageQueueTx.toLowerCase(), route: ROUTE_NAME.MessageQueueTx },
        { page: PAGE_NAME.MyEpcsReports, route: ROUTE_NAME.MyEPCSReport },
        { page: PAGE_NAME.EpcsDailyActivityReport.toLowerCase(), route: ROUTE_NAME.EPCSDailyReport },
        { page: PAGE_NAME.EditUser.toLowerCase(), route: ROUTE_NAME.EditUser },
        { page: PAGE_NAME.GetEPCS.toLowerCase(), route: ROUTE_NAME.GetEpcs },
        { page: PAGE_NAME.DeluxeBillingPage.toLowerCase(), route: ROUTE_NAME.DeluxeBilling },
        { page: PAGE_NAME.ReviewHistory.toLowerCase(), route: ROUTE_NAME.ReviewHistory },
        { page: PAGE_NAME.ForcePasswordSetup.toLowerCase(), route: ROUTE_NAME.ForcePasswordSetup },
        { page: PAGE_NAME.PDFInPage.toLowerCase(), route: ROUTE_NAME.PdfInPage },
        { page: PAGE_NAME.HomeAddress.toLowerCase(), route: ROUTE_NAME.HomeAddress },
    ];

    public static ExtractRoute(url: string) {
        let navigateTo: string = "";

        for (let i = 0; i < this.navigationRoutes.length; i++) {
            let page = this.navigationRoutes[i].page.toLowerCase();
            if (this.IsValidUrlToNavigate(page, url)) {
                navigateTo = this.navigationRoutes[i].route;
                break;
            }
        }
        return navigateTo;
    }

    static IsValidUrlToNavigate(page: string, currentUrl: string): boolean {
        currentUrl = currentUrl.charAt(0) == '/' ? currentUrl.substring(1) : currentUrl; //Ex, currentUrl =  /EditUser.aspx?page=NoLic

        switch (page) {
            case currentUrl.trim().toLowerCase():  // Ex, currentUrl =  EditUser.aspx
            case currentUrl.split('?')[0].toLowerCase(): // Ex, currentUrl =   EditUser.aspx?page=Nolic
                {
                    return true;
                }
            default: return false;
        }
    }

    public static ExtractPage(url: string) {
        const urlTrimmed = url.replace(/^\s*\/*\s*|\s*\/*\s*$/gm, '');
        let page: string = "";

        for (let navigationRouteIndex = 0; navigationRouteIndex < this.navigationRoutes.length; navigationRouteIndex++) {
            const route = this.navigationRoutes[navigationRouteIndex].route.toLowerCase();
            if (route === urlTrimmed.trim().toLowerCase()) {
                page = this.navigationRoutes[navigationRouteIndex].page;
                break;
            }
        }

        return page;
    }
}

