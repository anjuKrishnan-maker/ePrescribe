import { Component, OnInit } from '@angular/core';
import { MedHelpSearchModel, InitalContentPayload } from '../../../model/model.import.def';
import { NAVIGATION_EVENT_ID, EVENT_COMPONENT_ID, ROUTE_NAME, PAGE_NAME } from '../../../tools/constants';
import { Router, NavigationEnd } from '@angular/router';
import { EventService, ContentLoadService } from '../../../services/service.import.def';
@Component(
    {
        selector: 'erx-med-help-search',
        templateUrl: './med-search.template.html'
        , styles: [`
        .search-med-input{
            width:68%; 
            padding-left: 20px ;
           
        }
        .back_search_med{
            background: #FFFFFF url(images/searchboxWK.gif) left no-repeat;
        }
        .back_search_med:focus{
             background: #FFFFFF ;
        }
        `]
    }
)
class MedSearchComponent implements OnInit {
    searchText: string = '';
    isFocused: boolean = false;
    visible: Boolean = true;
    medSearchModel: MedHelpSearchModel = new MedHelpSearchModel();
    constructor(private evtSvc: EventService, private router: Router, private contentLoadService: ContentLoadService) {
       
    }

    ngOnInit() {   

        this.medSearchModel = this.contentLoadService.initalContentPayload.HelpSearchPayload;

        this.router.events.subscribe((data) => {
            if (data instanceof NavigationEnd) {
                this.visible = (
                    data.urlAfterRedirects.includes(ROUTE_NAME.SelectPatient) ||
                    data.urlAfterRedirects.includes(ROUTE_NAME.Tasks) ||
                    data.urlAfterRedirects.includes(ROUTE_NAME.MultipleViewReport)
                );                
            }
        });       
    }   

    OpenSearch() {
        let textSearched = this.searchText;
        this.searchText = "";
        this.router.navigateByUrl(ROUTE_NAME.Library, { state: { navigateTo: this.medSearchModel.Url.toLowerCase() + textSearched.toLowerCase() } });
    }

    CheckEnter(event: KeyboardEvent) {
        if (event.keyCode == 13) {
            this.OpenSearch();
            return false;
        }

    }

}

export { MedSearchComponent };
