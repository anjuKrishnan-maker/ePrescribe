/// <reference path="../tools/repalce-strong.pipe.ts" />

import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { SafeHtmlPipe } from "../tools/safe-html.pipe";
import { SafeScriptPipe } from '../tools/safe-script.pipe';
import { SafeUrlPipe } from '../tools/safe-url.pipe';
import { ReplaceStrongPipe } from '../tools/repalce-strong.pipe';

@NgModule({
    declarations: [SafeHtmlPipe, SafeScriptPipe, SafeUrlPipe, ReplaceStrongPipe],
    imports: [CommonModule],
    exports: [SafeHtmlPipe, SafeScriptPipe, SafeUrlPipe, ReplaceStrongPipe]
})

export class PipeModule { }