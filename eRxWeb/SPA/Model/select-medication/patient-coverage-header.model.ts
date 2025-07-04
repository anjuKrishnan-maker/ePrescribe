export class PatientCoverageHeader {
    Name: string;
    ID: string;
}

export class PatientCoverageHeaderList {
    PatientCoverageHeaders: PatientCoverageHeader[];
    InvokeAgainDelayMilliseconds: number;
}