export class ExtractRequest {
    id: string;
    license: string;
    startTime: Date;
    endTime: Date;
    type: ExtractType;



    public constructor(id: string, license: string, cSharpStartTime: string, cSharpEndTime: string, type: ExtractType) {
        this.id = id;
        this.license = license;
        this.startTime = new Date(Date.parse(cSharpStartTime));
        this.endTime = new Date(Date.parse(cSharpEndTime));
        this.type = type;
    }

    public GetStartTimeInTicks() {
        return ((this.startTime.getTime() * 10000) + 621355968000000000); //Converts from javascript date into # of ticks to send to C#
    }

    public GetEndTimeInTicks() {
        return ((this.endTime.getTime() * 10000) + 621355968000000000); //Converts from javascript date into # of ticks to send to C#
    }

}
export enum ExtractType {
    PATIENT_DEMOGRAPHICS = 1,
    PHARMACY = 2
}