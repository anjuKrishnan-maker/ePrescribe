import { Component, Input, ContentChild, AfterContentInit, ChangeDetectorRef } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { ComponentCommonService, EventService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { ComponentLoadEventArgs, ComponentLoadStatus } from '../../../../model/model.import.def';
@Component({
    selector: 'erx-collapsible-panel',
    templateUrl: './collapsible-panel.template.html',
    styleUrls: [ './collapsible-panel.style.css' ]
})
export class CollapsiblePanelComponent implements AfterContentInit {
    private _page: string;
    private _isDefault: boolean=true;
    private _checkComponentVisibility: boolean = true;
    Loading: boolean;
    @Input() IsVisibleOnFly: boolean = true;
    public Visible: boolean;
    
    @Input() id: string;
    @Input() HeaderText: string;
    @Input() HeaderBgColor: string;
    @Input() InfoImageUrl: string = '';
    @Input() SuperScript: string = '';
    @Input() set IsDefaultOpen(value: boolean) {
        this._isDefault = value;
        this.isCollapsed = !value;
    };
    get IsDefaultOpen(): boolean{
        if(!this._isDefault) return this._componentCommonService.CheckDefaultOpenForPage(this.id, this.page);
        return this._isDefault;
    };
    @Input() set CheckComponentVisibility(value: boolean) {
        this._checkComponentVisibility = value;
    };
    get CheckComponentVisibility(): boolean {
        return this._checkComponentVisibility;
    };

    @Input() isCollapsed: boolean;
    @Input() public set page(value: string) {
        this._page = value;         
        if (this._checkComponentVisibility == true) {                   
            this.Visible = this._componentCommonService.CheckVisibility(this.id, this.page) && this.IsVisibleOnFly;
        }
    }
    public get page(): string {
        return this._page;
    }
    //: string; 
    private _componentCommonService: ComponentCommonService;
    
    constructor(private evSvc: EventService, private collapsableCd: ChangeDetectorRef) {
        this.HeaderBgColor = '#999999';
        this._componentCommonService = new ComponentCommonService();
        this.evSvc.subscribeEvent(EVENT_COMPONENT_ID.ColapsableVisible, (val: any) => {
            if (val.id == undefined) return;
            if (this.id === val.id) {
                this.Visible = val.visible;
                this.collapsableCd.detectChanges();
            }
        });
        this.evSvc.subscribeEvent(EVENT_COMPONENT_ID.ColapsableCollapse, (id: any) => {
            if (id == undefined) return;
            if (this.id === id) {
                $('#colapsable-panel-' + this.id).collapse('hide');
                this.isCollapsed = true;
                this.collapsableCd.detectChanges();
            }
        });

    }
    ngAfterContentInit() {
        this.evSvc.subscribeEvent(EVENT_COMPONENT_ID.RightControlLoad,(val: ComponentLoadEventArgs) => {
            if (val.id == this.id) {
                if (val.loadStatus == ComponentLoadStatus.Started)
                    this.Loading = true;
                else if (val.loadStatus == ComponentLoadStatus.Stopped)
                    this.Loading = false;
            }
        });
    }
    toggleCollapse() {
        this.isCollapsed = !this.isCollapsed;
    }
}


