import { Component, Input, Inject} from '@angular/core';
@Component({
    selector: 'max-retry-failure-popup',
    templateUrl: './max-try-failure.component.html',
    styleUrls: ['./max-try-failure.component.css']
})
export class MaxTryFailureComponent{
    @Input() shieldUserName: string;
    constructor(@Inject('window') private window: any) {
      
    }
    hide() {
        this.window.open(this.window?.appcontext?.mediator, "_self");
    }
   
}




