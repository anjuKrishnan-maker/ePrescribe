import { Component, OnInit, Input } from "@angular/core";

@Component({
    selector: 'erx-user-guid-modal',
    templateUrl: './user-guid-modal.template.html',
    styleUrls: ['./user-guid-modal.style.css']
})
export class UserGuidModalComponent implements OnInit {
    public showModalPopup: boolean = false;    
    userGuid: string;
    constructor() {


    }
    ngOnInit() {

    }

    toggleModalPopup(userId: string) {
        this.userGuid = userId;
        this.showModalPopup = !this.showModalPopup
    }

}