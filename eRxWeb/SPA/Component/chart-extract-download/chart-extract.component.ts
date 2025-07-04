import { Component, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { ChartExtractService } from '../../services/service.import.def';
import { MessageModel, MessageIcon } from '../../model/model.import.def';
import { ExtractRequest, ExtractType } from '../../model/model.import.def';
import { Router } from '@angular/router';
import { ROUTE_NAME } from '../../tools/constants';
import { HttpResponse } from '@angular/common/http';
@Component({
    selector: 'erx-chart-extract',
    templateUrl: './chart-extract.template.html',
    styleUrls: ['./chart-extract.style.css']
})

export class ChartExtractComponent {
    private messages: Array<MessageModel>;
    public StaticMessageStrip: Array<MessageModel> = [];
    isPageLoading: boolean = true;
    ErrorMessage: string;
    public demographicsRequests: Array<ExtractRequest>;
    public pharmacyRequests: Array<ExtractRequest>;
    public extractTypes: Array<ExtractType> = [ExtractType.PATIENT_DEMOGRAPHICS, ExtractType.PHARMACY];

    constructor(private ceSvc: ChartExtractService, private router: Router) { }

    ngOnInit() {
        this.GetAllRequests();
        this.initializeChartExtract();
    }

    //TODO change button clicks to send request object to api which will then return the download file.

    GetAllRequests() {
        /*TODO
        * Call api to get all requests 
        * set up values for ngFor to populate buttons for demo and pharm
        */
        this.GetRequests(ExtractType.PATIENT_DEMOGRAPHICS,"patient demographics");
        this.GetRequests(ExtractType.PHARMACY, "pharmacy reports");
    }

    GetRequests(type: number, typeName: string) {
        var requestArray: Array<ExtractRequest> = new Array<ExtractRequest>();
        this.ceSvc.GetRequests(type).subscribe(
            (res) => {
                if (res.status == 204)//NoContent = blank file is returned
                {

                    this.displayErrorMsg("No chart information found for " + typeName, false);
                }
                else {
                    //Set demo and pharmacy variables
                    for (var i in res.body) {
                        var req = res.body[i];
                        requestArray.push(new ExtractRequest(req["Id"], req["License"], req["StartUtc"], req["CreatedUtc"], req["ExtractType"]));
                    }
                    switch (type) {
                        case ExtractType.PATIENT_DEMOGRAPHICS:
                            this.demographicsRequests = requestArray;
                            break;
                        case ExtractType.PHARMACY:
                            this.pharmacyRequests = requestArray;
                            break;
                    }
                }
            },
            (err) => {
                this.displayErrorMsg(err, false);
            }
        );
    }

    initializeChartExtract() {
        this.StaticMessageStrip = [];
        let messageModel: MessageModel;
        this.messages = new Array<MessageModel>();
        if (messageModel != undefined && messageModel.Message !== '') {
            this.messages.push(messageModel);
        }
        this.isPageLoading = false;
    }

    navigateToSettings() {
        this.router.navigate([ROUTE_NAME.Settings]);
    }

    public HandleClick(request: ExtractRequest) {
        this.ceSvc.GenerateDownloadFile(request.id, request.license, request.GetStartTimeInTicks(), request.GetEndTimeInTicks(), request.type).subscribe(
            (res) => {
                if (res.status == 204)//NoContent = blank file is returned
                {
                    this.displayErrorMsg("No relevant chart information found.", false);
                }
                else {
                    var downloadFileName: string = this.createFileName(request);
                    this.createFileDownload(res, downloadFileName + ".zip");
                }
            },
            (err) => {
                this.displayErrorMsg(err, false);
            }
        );
    }


    createFileName(request: ExtractRequest) {
        var typeName = "";
        if (request.type == ExtractType.PATIENT_DEMOGRAPHICS) {
            typeName = "Patient_Demographics";
        } else if (request.type == ExtractType.PHARMACY) {
            typeName = "Prescription_Info";
        }

        var startMonth:string = this.getMonth(request.startTime.getMonth());
        var endMonth:string = this.getMonth(request.endTime.getMonth());

        var dateRange: string = startMonth + "_" + request.startTime.getDate() + "_" + request.startTime.getFullYear() + '-' + endMonth + "_" + request.endTime.getDate() + "_" + request.endTime.getFullYear();

        return typeName + "_" + dateRange;
    }

    getMonth(month: number) {
        var arr = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        return arr[month];
    }


    createFileDownload(data, fileName) {
        let blob = new Blob([data.body], { type: 'octet/stream' });
        if (window.navigator.msSaveOrOpenBlob) //IE & Edge
        {
            window.navigator.msSaveBlob(blob, fileName);
        }
        else //Chrome & FF
        {
            const url = window.URL.createObjectURL(blob);
            const anchor = document.createElement("a");
            anchor.href = url;
            anchor.download = fileName;
            document.body.appendChild(anchor);
            anchor.click();
            document.body.removeChild(anchor);
        }
    }

    displayErrorMsg(errorMsg, allowClickoff) {
        this.StaticMessageStrip.push(new MessageModel(
            errorMsg,
            MessageIcon.Error,
            "chartExtractError",
            allowClickoff));
    }
}