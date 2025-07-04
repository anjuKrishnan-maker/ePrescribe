import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'replaceStrong'
})

export class ReplaceStrongPipe implements PipeTransform {
    transform(value: string, arg: string): any {
        
        if (!value || !arg)
            return value;
        let returnStr: string = "";

        while (value.toLowerCase().indexOf(arg.toLowerCase()) > -1) {
            returnStr += value.substr(0, value.toLowerCase().indexOf(arg.toLowerCase()));
            returnStr += "<strong>" + value.substr(value.toLowerCase().indexOf(arg.toLowerCase()), arg.length) + "</strong>";
            value = value.substr(value.toLowerCase().indexOf(arg.toLowerCase()) + arg.length);
        }
        returnStr += value;
        return returnStr;
    }
}