import { Input, Output, EventEmitter, ElementRef } from '@angular/core';
import { EventService } from '../../services/service.import.def';
import { ComponentLoadEventArgs, ComponentLoadStatus, ComponentNavigationEventArgs } from '../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID } from '../../tools/constants';
export class BaseComponent implements IBaseComponent {

    @Output() onVisibilityChange = new EventEmitter();
    @Output() onDataLoadEvent = new EventEmitter();
    @Output() onLinkSelected = new EventEmitter<string>();
    @Input() visible: boolean = false;
    @Input() id: string;
    AssociateWebPage: string[];
    constructor(private eR: ElementRef = null, private evSvc?: EventService) {
        this.Hide();
    }
    Collapse() {
        if (this.evSvc != null) {
            this.evSvc.invokeEvent<string>(EVENT_COMPONENT_ID.ColapsableCollapse, this.id);
        }
    }
    Hide() {
        this.visible = false;
        if (this.eR != null)
            this.onVisibilityChange.emit({
                Visible: this.visible,
                Selector: this.eR.nativeElement.tagName
            });
        if (this.evSvc != null) {
            this.evSvc.invokeEvent<any>(EVENT_COMPONENT_ID.ColapsableVisible, { id: this.id, visible: false });
        }
    }

    Show() {
        this.visible = true;
        if (this.eR != null)
            this.onVisibilityChange.emit({
                Visible: this.visible,
                Selector: this.eR.nativeElement.tagName
            });
        if (this.evSvc != null) {
            this.evSvc.invokeEvent<any>(EVENT_COMPONENT_ID.ColapsableVisible, { id: this.id, visible: true });
        }
    }
    LinkClicked(src: string) {
        this.onLinkSelected.emit(src);
    }
    //fire start load event
    StartLoading() {
        if (this.evSvc != null)
            this.evSvc.invokeEvent<ComponentLoadEventArgs>(EVENT_COMPONENT_ID.RightControlLoad, new ComponentLoadEventArgs(this.id, ComponentLoadStatus.Started));
        return this;
    }
    //Fire end load event
    EndLoading() {
        if (this.evSvc != null)
            this.evSvc.invokeEvent<ComponentLoadEventArgs>(EVENT_COMPONENT_ID.RightControlLoad, new ComponentLoadEventArgs(this.id, ComponentLoadStatus.Stopped));
        return this;
    }
    CallChildFunction(arg: any) {

    }
    NaviGateToComponent(ComponentName: string, ComponentParameter: any) {
        let data: ComponentNavigationEventArgs = { ComponentParameter: ComponentParameter, ComponetnName: ComponentName }
        this.evSvc.invokeEvent<ComponentNavigationEventArgs>(NAVIGATION_EVENT_ID.NavigateToComponent, data);
    }
}



export interface IBaseComponent {
    CallChildFunction(arg: any);
    Show();
    Hide();
    NaviGateToComponent(ComponentName: string, ComponentParameter: any);
}