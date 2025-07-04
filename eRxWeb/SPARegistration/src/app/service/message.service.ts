import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
@Injectable()
export class MessageService {

    public message: string = "";

    constructor(private router: Router) {
    }

    public notify(message: string) {
        this.message = message;
        this.router.navigate(["register/error"]);
    }
}