import { Component, ViewChild, Input, OnInit, ChangeDetectorRef, NgZone } from '@angular/core'
import { DeluxeFeatureSelectionService, EventService, FeatureComparisonService, SpinnerService } from '../../services/service.import.def';
import {
    DeluxeFeatureSelection, Products, EnumDeluxeFeature, ForceCompulsoryRestrictions, DeluxeTeaserAdModel, DeluxeFeaturePricing, MessageModel, MessageIcon,
    DeluxeFeatureStaticTexts, DeluxeFeatureMessageModelTag
} from '../../model/model.import.def';
import { PAGE_NAME, NAVIGATION_EVENT_ID, EVENT_COMPONENT_ID, ROUTE_NAME } from '../../tools/constants';
import { FeatureComparisonComponent } from '../feature-comparison/feature-comparison.component';
import { Router, RouterEvent, NavigationEnd, RoutesRecognized } from '@angular/router';


@Component({
    selector: 'erx-deluxe-feature-selection',
    templateUrl: './deluxe-feature-selection.template.html',
    styleUrls: ['./deluxe-feature-selection.style.css']
})

export class DeluxeFeatureSelectionComponent implements OnInit  {    
    @ViewChild('FeatureComparisionComponent') FeatureComparisionComponent: FeatureComparisonComponent;
     
    public isGetEPCSSelcted: boolean = false;   
    public LicenseDeluxeStatus: boolean;
    public BackBtnUrl: string = PAGE_NAME.IntegrationSolutionsList;
    public DeluxeTeaserAd: DeluxeTeaserAdModel;
    public headerTitle: string;
    public featureComparisionUrl: string;
    public deluxeFeatureSelection: DeluxeFeatureSelection = new DeluxeFeatureSelection();
    public forceCompulsoryRestrictions: ForceCompulsoryRestrictions = new ForceCompulsoryRestrictions();
    public products: Array<Products> = new Array<Products>();    
    public selectedDeluxeFeature: DeluxeFeaturePricing = new DeluxeFeaturePricing();
    public subscribedModel: string;
    public Price: number;    
    public currentRadiobtnSelected: string; 
    public addedFeature: DeluxeFeaturePricing = new DeluxeFeaturePricing();
    public TeaserAddSrc: string;    
    public isNoSponseredMessageVisible: boolean=false;
    public isSponseredMessageVisible: boolean=false;
    public isDiscountSponseredMessage: boolean=false;
    public isOneTimeFeeVisible: boolean=false;    
    public isCompulsaryBasicRbtnVisible: boolean = false;
    public isDeluxeRbtnVisible: boolean = false;
    public isDeluxeLogRxRbtnVisible: boolean = false;
    public isDeluxeEPCSRbtnVisible: boolean = false;
    public isDeluxeEPCSEpaLogRxRbtnVisible:boolean = false
    public isFeatureComparisionLinkVisible = true;
    public isBackButtonVisible = true;
    //ModalPopUp Variables
    public isModalpPopUpVisible: boolean = false;

    // Card Variables
    public isSubscriptionVisible: boolean;
    public isFeature1Visible: boolean = false;
    public isFeature2Visible: boolean = false;
    public isFeature3Visible: boolean = false;
    public isFeature4Visible: boolean = false;
    public isFeature5Visible: boolean = false;
    public isFeature6Visible: boolean = false;
    public isFeature7Visible: boolean = false;
    public oneTimePrice: number;
    public noOfBadges: number[] = null;
    public MessageStripArray: Array<MessageModel> = [];
   

    constructor(private evtSvc: EventService,
        private deluxeFeatureSelectionSvc: DeluxeFeatureSelectionService,
        private router: Router, private spinnerSvc: SpinnerService,
        private changeDectorRef: ChangeDetectorRef, private zone: NgZone) {
        setTimeout(() => this.zone.run(() => {
            this.changeDectorRef.detectChanges();
        }), 0);
    }

    ngOnInit() {
        this.CheckGetEpcsClicked();
        this.GetInitialData();
    }

    CheckGetEpcsClicked() {
        if (this.router.url.includes(ROUTE_NAME.GetEpcs))
            this.isGetEPCSSelcted = true;
    }
        
    GetInitialData() {        
        this.deluxeFeatureSelectionSvc.GetDeluxeFeatureSelectionInitialData().subscribe(response => {
            if (response.deluxeFeatureSelection.deluxeFeaturePricing.length > 0) {                
                this.deluxeFeatureSelection = response.deluxeFeatureSelection;
                this.subscribedModel = response.subscribedModule;                
                this.oneTimePrice = response.product[2].ProductPrice;
                this.forceCompulsoryRestrictions = response.forceCompulsoryRestrictions; 
            }
            this.setInitialData();
        })        
    }

    setInitialData() {
        if (this.deluxeFeatureSelection.EnterprisePricing) { this.HideFeatureComparisionLink(); };
        this.showOnlyEnterpriseFeatures();
        this.SetInitialUI(this.subscribedModel);
        this.setForcePaymentRestrictions();    

    }


    
    HideFeatureComparisionLink() {
        this.isFeatureComparisionLinkVisible = false;
    }

    showOnlyEnterpriseFeatures() {
        
        this.deluxeFeatureSelection.deluxeFeaturePricing.forEach((deluxeFeatureSelected) => {
            if (deluxeFeatureSelected.PricingStructureDesc == EnumDeluxeFeature.CompulsoryBasic) 
                this.isCompulsaryBasicRbtnVisible = true;            
            if (deluxeFeatureSelected.PricingStructureDesc == EnumDeluxeFeature.DeluxeEpa)
                this.isDeluxeRbtnVisible = true;
            if (deluxeFeatureSelected.PricingStructureDesc == EnumDeluxeFeature.DeluxeEpaLogRx)
                this.isDeluxeLogRxRbtnVisible = true;
            if (deluxeFeatureSelected.PricingStructureDesc == EnumDeluxeFeature.DeluxeEPCSEpa2017)
                this.isDeluxeEPCSRbtnVisible = true;
            if (deluxeFeatureSelected.PricingStructureDesc == EnumDeluxeFeature.DeluxeEPCSEpaLogRx2017)
                this.isDeluxeEPCSEpaLogRxRbtnVisible = true;
        });                
    }



    setForcePaymentRestrictions() {
        if (this.forceCompulsoryRestrictions.IsForcePaymentState) {
            this.MessageStripArray.push(new MessageModel(
                DeluxeFeatureStaticTexts.ForceFeatureSelectionErrorMsg,
                MessageIcon.Information,
                DeluxeFeatureMessageModelTag.ForcePurchase,
                false));
            this.isBackButtonVisible = false;
            if (this.forceCompulsoryRestrictions.IsEnterpriseCompulsoryBasic) {
                this.TeaserAddSrc = "images/TeaserAd/BasicNoDate.png";
                this.OpenPopUp();
            }
        } else {
            this.isBackButtonVisible = true;
        }
    }

    OpenPopUp() {
        $("#Modal").modal('show');
    }

    ClosePopUp() {
        $("#Modal").modal('hide');
    }    

    onBackBtnClicked() {
        this.currentRadiobtnSelected = null;
        this.isSubscriptionVisible = false;   
        this.router.navigateByUrl(ROUTE_NAME.ManageAccount, { state: { navigateTo: this.BackBtnUrl } }); 
    }
                           
                
    FeatureComparisionClicked() {             
        this.FeatureComparisionComponent.GetFeatureComparisonUrl();
    } 
       
    SetInitialUI(selectedFeature: string) { 
        if (this.isGetEPCSSelcted) {
            this.currentRadiobtnSelected = EnumDeluxeFeature.DeluxeEPCSEpa2017;
            this.setSubscriptionCardData(this.currentRadiobtnSelected);
            this.validateOneTimeFeeVisible(this.currentRadiobtnSelected);
            this.isGetEPCSSelcted = false;
        }
        else {
            if (selectedFeature) {
                this.isSubscriptionVisible = true;
                this.deluxeFeatureSelection.deluxeFeaturePricing.forEach((deluxeFeatureSelected) => {
                    if (deluxeFeatureSelected.PricingStructureDesc.toLowerCase() == selectedFeature.toLowerCase()) {
                        this.selectedDeluxeFeature = deluxeFeatureSelected;
                        return this.selectedDeluxeFeature;
                    }
                });
                this.currentRadiobtnSelected = this.selectedDeluxeFeature.PricingStructureDesc;
                if (this.currentRadiobtnSelected) {
                    this.isSubscriptionVisible = true;
                    this.setSubscriptionCardData(this.currentRadiobtnSelected);
                    this.validateOneTimeFeeVisible(this.currentRadiobtnSelected);
                }
                else {
                    this.isSubscriptionVisible = false;
                }
            }
        }
    }
    
    RemoveDescriptionUI() {
        this.isNoSponseredMessageVisible = false;
        this.isDiscountSponseredMessage = false;
        this.isNoSponseredMessageVisible = false;
    }

    ResetFeatures() {        
        this.isFeature1Visible = false;
        this.isFeature2Visible = false;
        this.isFeature3Visible = false;
        this.isFeature4Visible = false;
        this.isFeature5Visible = false;
        this.isFeature6Visible = false;
        this.isFeature7Visible = false;
    }

    // Setting Subscription Card Features
    setCompulsaryBasicSubscriptionCard() {        
        this.headerTitle = "Basic";
        this.isFeature1Visible = true;
        this.isFeature7Visible = true;
        this.isFeature2Visible = true;
        this.noOfBadges = [1];
    }
    setDeluxeEpaSubscriptionCard() {
        this.isNoSponseredMessageVisible = true;
        this.headerTitle = "Deluxe"
        this.isFeature1Visible = true;
        this.isFeature7Visible = true;
        this.isFeature2Visible = true;
        this.isFeature3Visible = true;
        this.isFeature4Visible = true;
        this.noOfBadges = [1, 2];
    }
    setDeluxeEpaLogRxSubscriptionCard() {
        this.isNoSponseredMessageVisible = true;
        this.headerTitle = "Deluxe with Sponsored Messages"
        this.isFeature1Visible = true;
        this.isFeature2Visible = true;
        this.isFeature3Visible = true;
        this.isFeature7Visible = true;
        this.noOfBadges = [1, 2];
    }
    setDeluxeEPCSEpa2017SubscriptionCard() {
        this.isDiscountSponseredMessage = true;
        this.headerTitle = "Deluxe with EPCS"
        this.isFeature1Visible = true;
        this.isFeature7Visible = true;
        this.isFeature2Visible = true;
        this.isFeature3Visible = true;
        this.isFeature4Visible = true;
        this.isFeature5Visible = true;
        this.isFeature6Visible = true;
        this.noOfBadges = [1, 2, 3];
    }
    setDeluxeEPCSEpaLogRx2017SubscriptionCard() {
        this.isDiscountSponseredMessage = true;
        this.headerTitle = "Deluxe with EPCS With sponsored messages."
        this.isFeature1Visible = true;
        this.isFeature7Visible = true;
        this.isFeature2Visible = true;
        this.isFeature3Visible = true;
        this.isFeature5Visible = true;
        this.isFeature6Visible = true;
        this.noOfBadges = [1, 2, 3];
    }

    setSubscriptionCardData(selectedFeature: string) {        
        this.ResetFeatures();
        this.isSubscriptionVisible = true;
        this.getDeluxeFeaturePriceBasedOnSelection(selectedFeature);
        if (selectedFeature == EnumDeluxeFeature.CompulsoryBasic) {            
            this.setCompulsaryBasicSubscriptionCard();
        }
        else if (selectedFeature == EnumDeluxeFeature.DeluxeEpa) {
            this.setDeluxeEpaSubscriptionCard();
        }
        else if (selectedFeature == EnumDeluxeFeature.DeluxeEpaLogRx) {
            this.setDeluxeEpaLogRxSubscriptionCard();
        }
        else if (selectedFeature == EnumDeluxeFeature.DeluxeEPCSEpa2017) {
           this.setDeluxeEPCSEpa2017SubscriptionCard()
        }
        else if (selectedFeature == EnumDeluxeFeature.DeluxeEPCSEpaLogRx2017) {
            this.setDeluxeEPCSEpaLogRx2017SubscriptionCard();
        }         
    }
    
    getDeluxeFeaturePriceBasedOnSelection(selectedFeature: string) {        
        this.deluxeFeatureSelection.deluxeFeaturePricing.forEach((deluxeFeatureSelected) => {            
            if (deluxeFeatureSelected.PricingStructureDesc.toLowerCase() == selectedFeature.toLowerCase()) {
                this.Price = deluxeFeatureSelected.Price;                
            }            
        });
    } 

    getSelectedFeature() {
        this.addedFeature = this.deluxeFeatureSelection.deluxeFeaturePricing.find(deluxeFeature => deluxeFeature.PricingStructureDesc === this.currentRadiobtnSelected); 
        this.setSubscriptionCardData(this.addedFeature.PricingStructureDesc);
        this.validateOneTimeFeeVisible(this.addedFeature.PricingStructureDesc);
    }

    onItemChange() {
        this.MessageStripArray = this.MessageStripArray.filter(x => x.Tag !== DeluxeFeatureMessageModelTag.ValidationError);
        this.RemoveDescriptionUI();
        this.getSelectedFeature();               
    } 

    validateOneTimeFeeVisible(deluxeFeature) {                
        if (deluxeFeature == EnumDeluxeFeature.DeluxeEPCSEpa2017 || deluxeFeature == EnumDeluxeFeature.DeluxeEPCSEpaLogRx2017) {
            this.isOneTimeFeeVisible = true;
        }
        else {
            this.isOneTimeFeeVisible = false;
        }   
    }   

    public onContinueClicked() {       
        if (this.currentRadiobtnSelected) { 
            setTimeout(() => this.zone.run(() => {
                this.deluxeFeatureSelectionSvc.SetSelectedDeluxeFeature(this.currentRadiobtnSelected).subscribe(response => {
                    if (response) {                    
                        this.router.navigateByUrl(ROUTE_NAME.DeluxeBilling, { state: { navigateTo: PAGE_NAME.DeluxeBillingPage + '?from=' + "DeluxeFeatureSelectionComponent" } });
                    }
                });
            }), 0);                                               
        }
        else {
            this.MessageStripArray = this.MessageStripArray.filter(x => x.Tag !== DeluxeFeatureMessageModelTag.ValidationError);
            this.MessageStripArray.push(new MessageModel(
                DeluxeFeatureStaticTexts.NoFeatureSelectionErrorMsg,
                MessageIcon.Error,
                DeluxeFeatureMessageModelTag.ValidationError,
                true));
        }
    }    
}