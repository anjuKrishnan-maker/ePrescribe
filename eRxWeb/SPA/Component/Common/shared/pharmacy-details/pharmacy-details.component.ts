import { Component,Input } from '@angular/core';
import { PharmacyModel } from '../../../../model/model.import.def';
import { current_context } from '../../../../tools/constants';

@Component({
    selector: 'pharmacy-details',
    templateUrl: './pharmacy-details.template.html',
    
})

export class PharmacyDetailsComponent {
    @Input() pharmaModel: PharmacyModel = new PharmacyModel();
    
}
