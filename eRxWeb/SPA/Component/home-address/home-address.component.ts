import { Component, OnInit, ViewChild, Inject } from "@angular/core";
import { HomeAddressService } from "../../services/service.import.def";
import { Router } from "@angular/router";
import 'rxjs/add/operator/filter';
import { HomeAddressStartUpModel, HomeAddressDetail, HomeAddressResponse } from '../../model/home-address.model';
import { NgForm } from '@angular/forms';
import { PAGE_NAME } from '../../tools/constants';

@Component({
    selector: 'erx-home-address',
    templateUrl: './home-address.template.html',
    styleUrls: ['./home-address.style.css']
})
export class HomeAddressComponent implements OnInit {
    homeAddressDetail: HomeAddressDetail = new HomeAddressDetail();
    homeAddressStartUpModel: HomeAddressStartUpModel;
    zipCodePattern = "^(\\d{5})(?:\\d{4})?$";
    cityPattern = "^([a-zA-Z]+[\\s-'.]{0,20})*";

    @ViewChild('homeAddressForm') public homeAddressForm: NgForm;

    constructor(private homeAddressSvc: HomeAddressService, private router: Router) {
    }

    ngOnInit() {
        this.homeAddressSvc.getStartUpdata()
            .subscribe((homeAddrStartUpModel: HomeAddressStartUpModel) => {
            this.homeAddressStartUpModel = homeAddrStartUpModel
        });
    }

    markAllControlAsTouched() {
        Object.keys(this.homeAddressForm.controls).forEach(field => {
            const control = this.homeAddressForm.controls[field];
            control.markAsTouched({ onlySelf: true });
        })
    }

    onSubmit() {
        this.markAllControlAsTouched();
        if (this.homeAddressForm.valid) {
            this.homeAddressSvc.save(this.homeAddressDetail)
                .subscribe((homeAddressResponse: HomeAddressResponse) => {
                    if (homeAddressResponse.IsSaveSuccessful) {
                        window.open(homeAddressResponse.RedirectUrl, "_self");
                    }
                });
        }
    }


    OnCancel() {
        window.open(PAGE_NAME.LogOut, "_self");
    }
}