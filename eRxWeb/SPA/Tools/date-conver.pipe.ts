import { Pipe, PipeTransform } from '@angular/core';
/*
 * Raise the value exponentially
 * Takes an exponent argument that defaults to 1.
 * Usage:
 *   value | exponentialStrength:exponent
 * Example:
 *   {{ 2 |  exponentialStrength:10}}
 *   formats to: 1024
*/
@Pipe({ name: 'ConvertToDate' })
export class ConverToDatePipe implements PipeTransform {
    transform(value: string): Date {
        let x = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
        return x;
    }
}