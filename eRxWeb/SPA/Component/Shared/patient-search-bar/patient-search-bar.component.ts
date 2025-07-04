import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EventService } from '../../../services/service.import.def';
import { PatientSearchBarModel } from '../../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, PAGE_NAME } from '../../../tools/constants';
import { DateValidator } from '../../../tools/utils/date-validator';

@Component({
    selector: "erx-patient-search-bar",
    templateUrl: "./patient-search-bar.template.html"
})

export class PatientSearchBar {
    patientSearchBarModel: PatientSearchBarModel;
    spanSearchErrorMessage: string;
    menuVisible: boolean = false;
    isValidPatientSearchCriteria: boolean = true;
    @Output() selectPatient = new EventEmitter();
    
    @Input()
    set reset(reset: boolean) {
        if (reset)
            this.ReInitializePatientSearchBar();
    }

    @Output() OnPatientSearchSubmitEvent = new EventEmitter<PatientSearchBarModel>();

    constructor(public evSvc: EventService) {     
    }

    //Lifecycle
    ngOnInit() {
        this.ReInitializePatientSearchBar();
    }

    //Button Click Event Handlers
    btnSearch_Click() {
        this.isValidPatientSearchCriteria = this.IsValidPatientSearchCriteria();

        if (this.isValidPatientSearchCriteria) {
            this.patientSearchBarModel.IncludeInactive = false;
            this.OnPatientSearchSubmitEvent.emit(this.patientSearchBarModel);
        }
    }

    btnAddPatient_Click() {
        this.selectPatient.emit();
    }

    //Action Methods
    public toggleMenu() {
        this.menuVisible = !this.menuVisible;
    }

    public ReInitializePatientSearchBar() {
        this.patientSearchBarModel = new PatientSearchBarModel();
        this.patientSearchBarModel.FirstName = "";
        this.patientSearchBarModel.LastName = "";
        this.patientSearchBarModel.DateOfBirth = "";
        this.patientSearchBarModel.PatientId = "";
        this.patientSearchBarModel.IncludeInactive = false;
        this.spanSearchErrorMessage = "";
        this.isValidPatientSearchCriteria = true;
    }

    public SearchInactiveAndActive() {
        this.isValidPatientSearchCriteria = this.IsValidPatientSearchCriteria();

        if (this.isValidPatientSearchCriteria) {
            this.patientSearchBarModel.IncludeInactive = true;
            this.OnPatientSearchSubmitEvent.emit(this.patientSearchBarModel);
        }
    }

    //Data Validation
    IsEmpty(input) {
        return (input.replace(/\s/g, "").length > 0 ? false : true);
    }

    IsValidDateEntered(s) {
        var bits = s.split('/');
        var d = new Date(bits[2], bits[0] - 1, bits[1]);
        return d && (d.getMonth() + 1) == bits[0];
    }

    IsValidTextLength(text) {
        if (this.IsEmpty(text) === false && text.length >= 2) {
            return true;
        }
        return false;
    }

    IsValidPatientSearchCriteria() {
        let hasCharacters: boolean = false;
        let hasDate: boolean = false;
        let isValidTextLength: boolean = true;
        let isValidTextFormat: boolean = true;
        let isValidDate: boolean = true;
        let isValid: boolean = true;
        this.spanSearchErrorMessage = "";

        // Step 1. Check the Text property of the RadDateInput control as it will contain what the user entered
        // even if it is not a valid date
        if (this.IsEmpty(this.patientSearchBarModel.DateOfBirth) === false) {
            hasDate = true;

            // if the enterd data is NOT a valid date IsEmpty will be true, if it is valid IsEmpty is false
            isValidDate = DateValidator(this.patientSearchBarModel.DateOfBirth);
        }

        // Step 2. check if any of the search text boxes have text entered
        if (!this.IsEmpty(this.patientSearchBarModel.FirstName) || !this.IsEmpty(this.patientSearchBarModel.LastName) || !this.IsEmpty(this.patientSearchBarModel.PatientId)) {
            hasCharacters = true;

            // check if at least one of the text boxes has at least 2 chars, use regex "\s" to find and remove any whitespace
            isValidTextLength =
                (
                    this.patientSearchBarModel.FirstName.replace(/\s/g, "").length >= 2 ||
                    this.patientSearchBarModel.LastName.replace(/\s/g, "").length >= 2 ||
                    this.patientSearchBarModel.PatientId.replace(/\s/g, "").length >= 2
                );
        }

        if (!hasCharacters && !hasDate) {
            // nothing has been entered for search criteria
            this.spanSearchErrorMessage = "•  Please enter at least 2 valid characters for Last Name, First Name or Patient ID or enter a Date of Birth.";
        } else {
            if (hasCharacters && !isValidTextLength) {
                this.spanSearchErrorMessage = "•  Please enter at least 2 valid characters for Last Name, First Name or Patient ID.";
            } else if (hasCharacters && isValidTextLength && (!this.IsEmpty(this.patientSearchBarModel.FirstName) || !this.IsEmpty(this.patientSearchBarModel.LastName))) {
                //Regex regexPattern = new Regex(@"([a-zA-Z0-9]{2,})|[a-zA-Z0-9]{1}([\.|\'|\-]{1})");
                let regexPattern: RegExp = new RegExp(/^[a-zA-Z0-9.\'-\s|]+$/g);
                if (!this.IsEmpty(this.patientSearchBarModel.LastName.trim()) && !regexPattern.test(this.patientSearchBarModel.LastName.trim())) {
                    isValidTextFormat = false;
                    this.spanSearchErrorMessage = "•  Please enter a valid format for Last Name.";
                }

                regexPattern.lastIndex = 0;
                if (!this.IsEmpty(this.patientSearchBarModel.FirstName.trim()) && !regexPattern.test(this.patientSearchBarModel.FirstName.trim())) {
                    isValidTextFormat = false;
                    if (this.spanSearchErrorMessage === undefined || this.spanSearchErrorMessage === "") {
                        this.spanSearchErrorMessage = "•  Please enter a valid format for First Name.";
                    } else {
                        this.spanSearchErrorMessage += "•  Please enter a valid format for First Name.";
                    }
                }
            }

            if ((hasDate && !isValidDate)) {
                this.spanSearchErrorMessage = "•  Date of Birth must be in the format mm/dd/yyyy.";
            }
        }

        // if the user did NOT enter a DOB or FirstName/LastName/PatientID
        // or if the DOB entered is not valid
        // or if whatever was entered in FirstName/LastName is not valid
        // then display the error
        if ((!hasCharacters && !hasDate) ||
            (hasCharacters && (!isValidTextLength || !isValidTextFormat)) ||
            (hasDate && !isValidDate)) {
            isValid = false;
        }

        return isValid;
    }
}