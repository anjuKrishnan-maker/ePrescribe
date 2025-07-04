export class RobustLinkModel {
    SeqNumber: number;
    DescriptionText: string;
    URL: string;
    ClickLink: string;
}

export class RobustLinkMesssagel {
    MessageId: number;
    MessageLines: RobustLinkModel[]= new Array();
    constructor() {
        this.MessageLines.push(new RobustLinkModel());
    }
}