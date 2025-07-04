import { Component, OnInit, ViewChild } from "@angular/core";
import { UserActivationPopup } from '../user-activation-popup/user-activation-popup.component';
import { ActivatedRoute, Router } from '@angular/router';
import { ActivationCodeService } from '../../../service/activation-code.service'
import { Workflow } from '../link-account-request-model'

@Component({
    selector: 'app-user-activation',
    templateUrl: './user-activation.component.html',
    styleUrls: ['./user-activation.component.css']
})
export class UserActivationComponent implements OnInit {
    @ViewChild('userActivationModalPopup') userActivationModalPopup: UserActivationPopup;
    appType: string;
    workflow: Workflow;
    isActivationCodeVisible: boolean = true;
    isLinkActivationCodeVisible: boolean = true;
    
    acivationCode: string;
    
    constructor(private route: ActivatedRoute,
        private activationService: ActivationCodeService,
        private router: Router) {       
    }

    ngOnInit() {
        this.appType = this.route.snapshot.queryParamMap.get('App');
        this.activationService.intializeWorkFlow({ AppType : this.appType }).subscribe((workflow: Workflow) => {
            this.workflow = workflow;
            if (this.workflow == Workflow.Sso) {
                this.isActivationCodeVisible = false;
                this.isLinkActivationCodeVisible = false;
            }
        });
    }

    OpenNewUserActivationPopup() {
        if (this.workflow == Workflow.Sso) {
            this.userActivationModalPopup.ShowModal = false;
            this.userActivationModalPopup.WorkflowType = this.workflow;
            this.userActivationModalPopup.ShowLinkedAccount = false;
            this.router.navigate(["register/createAccount"]);
        }
        else {
            this.userActivationModalPopup.ShowModal = true;
            this.userActivationModalPopup.WorkflowType = this.workflow;
            this.userActivationModalPopup.ShowLinkedAccount = false;
        }
    }
    OpenLinkAccountActivationPopup() {
        if (this.workflow == Workflow.Sso) {
            this.userActivationModalPopup.ShowLinkedAccount = true;
            this.userActivationModalPopup.ShowModal = false;   
            this.userActivationModalPopup.WorkflowType = this.workflow;
            this.userActivationModalPopup.showLinkedAccountPopup();
        }
        else {           
            this.userActivationModalPopup.ShowModal = true;
            this.userActivationModalPopup.ShowLinkedAccount = true;
            this.userActivationModalPopup.WorkflowType = this.workflow;
        }
    }

}