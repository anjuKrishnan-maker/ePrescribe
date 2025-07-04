import { Component, Input, EventEmitter, Output } from "@angular/core";
import { UserSupervisingProviderModel } from "../../../../model/user/user.model";

@Component({
    selector: 'erx-user-supervising-providers',
    templateUrl: './user-supervising-provider.template.html',
    styleUrls: ['./user-supervising-provider.style.css']

})

export class UserSupervisingProvidersComponent {
    @Input() userSupervisingProvidersList: UserSupervisingProviderModel[];
    @Output() togglePOBSelectProviders = new EventEmitter<boolean>();
    public showModalPopup: boolean;
    public isAllCheckboxSelected = false;
    public originalIsAllCheckboxSelected: boolean;
    public originalUserSupervisingProvidersList: UserSupervisingProviderModel[];
    public userName: string = '';    

    toggleModalPopup() { this.showModalPopup = !this.showModalPopup }    

    onSelectAllProviders() {        
        for (let i = 0; i < this.userSupervisingProvidersList.length; i++) {
            this.userSupervisingProvidersList[i].Selected = this.isAllCheckboxSelected;
        }
    }

    onProviderSelected(index: number) {
        this.userSupervisingProvidersList[index].Selected = !this.userSupervisingProvidersList[index].Selected;
    }
    
    verifyChanges() {
        if (this.isAllCheckboxSelected) {
            if (confirm("You have saved the selections with ALL providers selected.\n\n The selection for the POB will be changed to POB Associated to All Providers.\n\n If this is not your intent, press Cancel and return to the selection screen.\n\n Otherwise press OK.")) {
                this.togglePOBSelectProviders.emit(true);
            }
        }
        else {

            if (!this.isAnyProviderSelected()) {
                if (confirm("You have saved the selections with NO providers selected.\n\n The selection for the POB will be changed to POB Associated to All Providers.\n\n If this is not your intent, press Cancel and return to the selection screen.\n\n Otherwise press OK.")) {
                    this.togglePOBSelectProviders.emit(true);                 
                }
            }
        }
    }  

    isAnyProviderSelected(): boolean{
        let selectedProvider = 0;
        this.userSupervisingProvidersList.forEach((provider) => {
            if (provider.Selected == true) { selectedProvider += 1; }
        });
        return selectedProvider > 0;
    }

    onSubmit() {       
        this.toggleModalPopup();
    }

    cancel() {
        for (let i = 0; i < this.userSupervisingProvidersList.length; i++) {
            this.userSupervisingProvidersList[i].Selected = this.originalUserSupervisingProvidersList[i].Selected;
        }
        this.isAllCheckboxSelected = this.originalIsAllCheckboxSelected;
        this.toggleModalPopup();
    }

}

