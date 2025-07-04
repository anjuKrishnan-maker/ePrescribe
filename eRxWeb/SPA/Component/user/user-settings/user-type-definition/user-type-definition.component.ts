import { Component, OnInit } from "@angular/core";

@Component({
    selector: 'erx-user-type-definition',
    templateUrl: './user-type-definition.template.html',
    styleUrls: ['./user-type-definition.style.css']
})
export class UserTypeDefinitionComponent implements OnInit {
    public showModalPopup: boolean;
    constructor() {

    }
    ngOnInit() {

    }

    toggleModalPopup() {
        this.showModalPopup = !this.showModalPopup
    }

}