import { Component, Input } from '@angular/core';
import { current_context } from '../../tools/constants';
import { FillHistoryTimelineModel } from '../../model/model.import.def';

@Component({
    selector: 'erx-rxfill-history-details',
    templateUrl: './rxfill-details.template.html'
})

export class RxFillDetailsComponent {

    @Input() FillHistoryTimelineItems: Array<FillHistoryTimelineModel>;

    cancel() { $("#mdlFillHistoryDetails").modal('toggle'); }
}