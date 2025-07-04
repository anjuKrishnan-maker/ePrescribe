import { } from 'jasmine';
import { ComponentFixture, inject, TestBed } from '@angular/core/testing';
import { async } from '@angular/core/testing';
import { Observable } from 'rxjs/Observable';
import { ChangeDetectorRef } from '@angular/core';
import { Observer } from 'rxjs/Observer';
import { RightPanelComponent } from "../component/common/right-panel/right-panel.component";
import { PharmacyService, EventService, ErrorService, ComponentCommonService } from '../services/service.import.def';
import { PharmacyModel } from '../model/model.import.def';
import { PharmacyComponent } from '../component/common/right-panel/pharmacy/pharmacy.component';
import { PharmacySigAlertComponent } from '../component/common/right-panel/right-panel.component.import.def';
import { ModalPopupControl } from '../component/shared/controls/modal-popup/modal-popup.control';


describe('Pharmacy Component Tests', () => {
    let component: PharmacyComponent

    let pharmacyModel = new PharmacyModel();
    pharmacyModel.Name = "VA PHARMACY 10.6MU";
    pharmacyModel.Address1 = "7723 JEFFERSON DAVIS HIGHWAY";
    pharmacyModel.City = "ARLINGTON";
    pharmacyModel.State = "VA";
    pharmacyModel.Phone = "123456789";
    pharmacyModel.Fax = "123456789";
    pharmacyModel.EPCS = "YES";
    pharmacyModel.ZipCode = "22211";
    pharmacyModel.TransMethod = "Electronic";

    let nullpharmacyModel = new PharmacyModel();
    nullpharmacyModel.Name = "";
    nullpharmacyModel.Address1 = "";
    nullpharmacyModel.City = "";
    nullpharmacyModel.State = "";
    nullpharmacyModel.Phone = "";
    nullpharmacyModel.Fax = "";
    nullpharmacyModel.EPCS = "";
    nullpharmacyModel.ZipCode = "";
    nullpharmacyModel.TransMethod = "";



    let evnntSvc: EventService = new EventService();
    let errorsvc: ErrorService = new ErrorService(evnntSvc);

    class MockPharmacyService extends PharmacyService {
        GetPharmacy(data) {
            return Observable.create((observer: Observer<void>) => {
                if (data.PharmacyID == "123")
                    observer.next(pharmacyModel);
                else
                    observer.next(nullpharmacyModel);
            });
        }
    }

    class MockChangeDetectorRef extends ChangeDetectorRef {
        detectChanges() {
        }
    }

    beforeEach(async(() => {
        component = new PharmacyComponent(new MockPharmacyService(null, errorsvc), new MockChangeDetectorRef(), evnntSvc);
    }));
    
    it('Should have Pharmacy model with values returned by api response ', async(() => {
        component.LoadPharmacy({ PharmacyID: "123" });
        expect(component.pharmaModel.Name == pharmacyModel.Name).toBe(true);
        expect(component.pharmaModel.Address1 == pharmacyModel.Address1).toBe(true);
        expect(component.pharmaModel.City == pharmacyModel.City).toBe(true);
        expect(component.pharmaModel.State == pharmacyModel.State).toBe(true);
        expect(component.pharmaModel.Phone == pharmacyModel.Phone).toBe(true);
        expect(component.pharmaModel.Fax == pharmacyModel.Fax).toBe(true);
        expect(component.pharmaModel.EPCS == pharmacyModel.EPCS).toBe(true);
        expect(component.pharmaModel.ZipCode == pharmacyModel.ZipCode).toBe(true);
        expect(component.pharmaModel.TransMethod == pharmacyModel.TransMethod).toBe(true);
    }));

    it('should have pharmacy model value reset/default values when api return empty response', async(() => {
        component.LoadPharmacy({ PharmacyID: " " });
        expect(component.pharmaModel.Name).toEqual(nullpharmacyModel.Name);
        expect(component.pharmaModel.Address1).toEqual(nullpharmacyModel.Address1);
        expect(component.pharmaModel.City).toEqual(nullpharmacyModel.City);
        expect(component.pharmaModel.State).toEqual(nullpharmacyModel.State);
        expect(component.pharmaModel.Phone).toEqual(nullpharmacyModel.Phone);
        expect(component.pharmaModel.Fax).toEqual(nullpharmacyModel.Fax);
        expect(component.pharmaModel.EPCS).toEqual(nullpharmacyModel.EPCS);
        expect(component.pharmaModel.ZipCode).toEqual(nullpharmacyModel.ZipCode);
        expect(component.pharmaModel.TransMethod).toEqual(nullpharmacyModel.TransMethod);
    }));
});

describe('Pharmacy Sig Alert Tests', () => {
    //let pharmacySigAlertComponent: PharmacySigAlertComponent;
    let evnntSvc: EventService = new EventService();
    let compSvc: ComponentCommonService = new ComponentCommonService();


    let rightPanelComponent = new RightPanelComponent(compSvc, evnntSvc);
    rightPanelComponent.pharmacySigAlertComponent = new PharmacySigAlertComponent();
    rightPanelComponent.pharmacySigAlertComponent.modalPopup = new ModalPopupControl();
    
    it('should show popup alert when parent method triggers the method call', async(() => {
        //arrange
        spyOn(rightPanelComponent.pharmacySigAlertComponent, 'showPharmacySigAlert');
        //act
        rightPanelComponent.displayPharmacySigAlert("message");
        //assert
        expect(rightPanelComponent.pharmacySigAlertComponent.showPharmacySigAlert).toHaveBeenCalled();

    }));

});
