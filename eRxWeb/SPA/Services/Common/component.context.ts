import { Injectable } from '@angular/core';


export class ComponentContext {
    components: KeyValuePair[] = new Array();
    componentsWithDefaultOpenPanel: Map<string, Array<string>> = new Map<string, Array<string>>();
    register(component: ComponentItems, key: string) {
        this.components.push(new KeyValuePair(key, component))
    }

    registercomponentsWithDefaultOpenPanel(panelID: string, ComponentAndPanelArray: Array<string>) {
        this.componentsWithDefaultOpenPanel.set(panelID, ComponentAndPanelArray);
        
    }

    getComponentByKey(panelId: string) {
        let componentArray: Array<string> = []; 
        let filteredComponentArray = this.componentsWithDefaultOpenPanel.get(panelId);
        if (filteredComponentArray != undefined || filteredComponentArray != null) {
            componentArray = filteredComponentArray;
        }
        return componentArray;
    }

    Hide(component: string) {
        for (var i = 0; i < this.components.length; i++) {
            if (this.components[i].key == component) {
                this.components[i].Value.visible = false;
            }
        }
    }

    Show(component: string) {
        for (var i = 0; i < this.components.length; i++) {
            if (this.components[i].key == component) {
                this.components[i].Value.visible = true;
            }
        }
    }

    get(key: string): ComponentItems {
        for (var i = 0; i < this.components.length; i++) {
            if (this.components[i].key == key) {
                return this.components[i].Value;
            }
        }
        return null;
    }
    getById(id: string): ComponentItems {
        for (let comp of this.components) {
            if (comp.Value.selector == id) {
                return comp.Value;
            }
        }
        return null;
    }
}

export class KeyValuePair {
    constructor(key: string, Value: ComponentItems) {
        this.key = key;
        this.Value = Value;
    }
    key: string;
    Value: ComponentItems;
}

export class ComponentItems {
    selector: string;
    Pages: string[];
    visible: boolean;
    constructor(selector: string, pages: string[], visible: boolean = true) {
        this.Pages = pages;
        this.selector = selector;
        this.visible = visible;
    }
}