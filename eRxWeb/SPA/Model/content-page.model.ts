export class FunctionDetail {
    Caller: any;
    Name: string;
    Param: any[];
    public constructor(caller: any, name: string, param: any[]) {
        this.Caller = caller;
        this.Name = name;
        this.Param = param;
    }
}