import { Component, Input, OnInit } from '@angular/core';
import { MenuService, ComponentCommonService, EventService, ContentLoadService } from '../../../services/service.import.def';
import { PAGES_TO_HIDE_MENU, current_context } from '../../../tools/constants';
import { NavigationModel, MenuItemModel } from '../../../model/model.import.def';
import { Router, RouterEvent, NavigationEnd } from '@angular/router';
import RouteNameExtractor, { } from '../../../tools/utils/Route-Name-Extractor';
type PageRoute = { page: string, route: string };

@Component({
    selector: 'erx-menu',
    templateUrl: './menu.template.html',
    providers: [MenuService],
    styles: [`.menu-height:{min-height:20px;}`]
})
export class MenuComponent implements OnInit {
    public selectedTab = 1;
    @Input() NavigationModel: NavigationModel;
    @Input() id: string;
    visible: boolean = true;
    get isException(): boolean {
        return current_context.isException;
    }
    public MenuItems: MenuItemModel[];
    private navigationRoutes: PageRoute[] = RouteNameExtractor.navigationRoutes;

    constructor(private compSvc: ComponentCommonService,
        public evSvc: EventService, private router: Router, private contentLoadService: ContentLoadService) {

        this.compSvc.AddWindowFunction('UpdateTasksCount', (numberOfRemainingTasks) => {
            this.UpdateTasksCount(numberOfRemainingTasks);
        });

        this.compSvc.AddWindowFunction('changeAcitiveMenu', (newMenu) => {
            this.changeAcitiveMenu(newMenu);
        });

        this.id = 'patientHeader';
    }

    setSelectedTab() {
        let mn = this.MenuItems.sort((a, b) => a.Order - b.Order);
        if (mn.length > 0) {
            if (current_context.PageName.toLowerCase() === "reports") {
                this.selectedTab = mn.find(x => x.Name.toLowerCase() === "reports").Order;
            }
            else {
                this.selectedTab = mn[0].Order;
            }
        }
    }

    onSelectTab(val: MenuItemModel) {
        this.selectedTab = val.Order;
        //Angular pages navigation.
        let navigateTo = this.navigationRoutes.find(x => x.page == val.Name.toLowerCase())?.route;
        if (navigateTo) {
            this.router.navigate([navigateTo]);
            return;
        }
        //Legacy page navigation.
        navigateTo = this.navigationRoutes.find(x => x.page == val.NavigationUrl.toLowerCase())?.route;
        if (navigateTo) {
            this.router.navigateByUrl(navigateTo, { state: { navigateTo: val.NavigationUrl.toLowerCase() } });
            return;
        }
    }

    ngOnInit() {
        if (this.contentLoadService.initalContentPayload.MenuPayload !== undefined) {
            this.MenuItems = this.contentLoadService.initalContentPayload.MenuPayload;
            this.setSelectedTab();
        }

        this.router.events.subscribe((pageEvent: RouterEvent) => {
            if (pageEvent instanceof NavigationEnd) {
                let routeParts = pageEvent.urlAfterRedirects.split('/');
                let currentPage: string = routeParts.pop() || routeParts.pop();  // handle potential trailing slash
                this.selectPageMenuTab(currentPage);
            }
        });
    }

    private selectPageMenuTab(currentPage: string) {
        let page = currentPage.toLowerCase();
        this.checkMenuVisibility(page);

        if (!this.visible)
            return;

        let men;
        if (page.indexOf("Library".toLowerCase()) >= 0) {
            men = this.MenuItems.find(x => x.Name === "Library");
        }
        else if ((page.indexOf("Tasks".toLowerCase()) >= 0) || (page.indexOf("TaskSummary".toLowerCase()) >= 0)) {
            men = this.MenuItems.find(x => x.Name.startsWith("Tasks"));
        }
        else if (page.indexOf("selectpatient".toLowerCase()) >= 0) {
            men = this.MenuItems.find(x => x.Name.startsWith("Patient"));
        }
        else if (page.indexOf("Settings".toLowerCase()) >= 0) {
            men = this.MenuItems.find(x => x.Name.startsWith("Settings"));
        }
        else if (page.indexOf("MyProfile".toLowerCase()) >= 0
            || page.indexOf("MyEPCSReports".toLowerCase()) >= 0
            || page.indexOf("ProtectedStoreEpcsReports".toLowerCase()) >= 0
            || page.indexOf("EPCSDailyActivityReport".toLowerCase()) >= 0) {
            men = this.MenuItems.find(x => x.Name === "My eRx");
        }

        if (men != null)
            this.selectedTab = men.Order;
    }

    ComparePage(val: string, page: string): boolean {
        return (val.toLowerCase() === page);
    }

    isHideMenuPage(val: string): boolean {
        for (var i = 0; i < PAGES_TO_HIDE_MENU.length; i++) {            
                if (this.ComparePage(val.toLowerCase(), PAGES_TO_HIDE_MENU[i].toLowerCase())) {
                    return true;
                }
        }
        return false;
    }

    UpdateTasksCount(numberOfRemainingTasks: number) {
        for (let men of this.MenuItems) {
            if (men.Name.startsWith("Tasks")) {
                if (numberOfRemainingTasks > 0)
                    men.Name = "Tasks (" + numberOfRemainingTasks + ")";
                else
                    men.Name = "Tasks";
            }
        }
    }

    checkMenuVisibility(page: string) {
        if (!this.MenuItems || this.MenuItems.length <= 0) {
            this.visible = false;
            return;
        }
        if (this.isHideMenuPage(page)) {
            this.visible = false;
            return;
        }
        this.visible = true;
    }

    changeAcitiveMenu(newMenu: string) {
        for (let men of this.MenuItems) {
            if (men.Name == newMenu) {
                this.selectedTab = men.Order;
            }

        }
    }
}