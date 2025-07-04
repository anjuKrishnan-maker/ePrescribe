export class PdmpUISummary {
    TransactionId: string;
    SummaryStatus: PdmpPatientResponseStatus;
    SummaryHtml: string;
}

export enum PdmpPatientResponseStatus {
    Unknown = 0,
    Available = 1,
    NotAvailable = 2,
    ConnectionIssue = 3,
}