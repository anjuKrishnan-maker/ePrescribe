import { Component, Input, NgModule, OnInit } from '@angular/core';
import { Patient } from '../../../model/model.import.def';


@Component({
    selector: 'erx-header',
    templateUrl: './header.template.html'
})

export class HeaderComponent implements OnInit {
    @Input() patientHeader: Patient;
    visible: boolean = true;
    constructor() {
    }

    ngOnInit() {

    }
}

