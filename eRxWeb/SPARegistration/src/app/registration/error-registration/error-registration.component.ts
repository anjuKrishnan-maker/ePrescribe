import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from '../../service/message.service';
@Component({
    selector: 'error-registration',
    templateUrl: './error-registration.component.html',
    styleUrls: ['./error-registration.component.css']
})

export class ErrorRegistrationComponent {
    private defaultErrorMessage = "Something went wrong while showing this page";
    constructor(public messageService: MessageService) {

    }
}