import {DomSanitizer} from '@angular/platform-browser';
import {PipeTransform, Pipe} from "@angular/core";

@Pipe({ name: 'safeScript'})
export class SafeScriptPipe implements PipeTransform  {
  constructor(private sanitized: DomSanitizer) {}
  transform(value) {
      return this.sanitized.bypassSecurityTrustScript(value);
  }
}