import { Component, Input, OnInit, ViewChild, EventEmitter, Output } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { CaptchaService } from '../../service/captcha.service';
import { CaptchaModel } from '../show-captcha/show-captcha.model'
import { NgForm } from "@angular/forms";

@Component({
    selector: 'show-captcha',
    templateUrl: './show-captcha.component.html',
    styleUrls: ['./show-captcha.component.css']
})
export class ShowCaptchaComponent implements OnInit {
    captchaImgSrc: SafeUrl;
    captchaModel: CaptchaModel = new CaptchaModel();
    @Input() ShowInValidCaptchaError: boolean = false;
    @ViewChild('captchaForm') public captchaForm: NgForm;

    constructor(private captchaService: CaptchaService, private domSanitizer: DomSanitizer) {

    }
    ngOnInit() {
        this.getCaptcha();
    }
    refreshCaptcha() {
        this.getCaptcha();
    }
    getCaptcha() {
        this.captchaService.getCaptcha().subscribe(base64String => {
            this.captchaImgSrc = this.domSanitizer.bypassSecurityTrustUrl('data:image/png;base64,' + base64String);
        });
    }
    onCaptchaChanged() {
        this.ShowInValidCaptchaError = false;
    };

    GetCaptchaDetails(): CaptchaModel {
        this.captchaForm.controls.txtCaptchaResponse.markAsTouched({ onlySelf: true });
        this.captchaModel.isValid = this.captchaForm.valid
        return this.captchaModel;
    }
}

