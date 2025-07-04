import { Component, OnInit } from "@angular/core";

@Component({
    selector: 'erx-user-mobile-messaging-description',
    templateUrl: './user-mobile-messaging-description.template.html',
    styleUrls: ['./user-mobile-messaging-description.style.css']
})
export class UserMobileMessageDescriptionComponent implements OnInit {
    public showModalPopup: boolean;
    constructor() {

    }
    ngOnInit() {

    }

    toggleModalPopup() {
        this.showModalPopup = !this.showModalPopup
    }

}