// JScript File
var origQuantity = 0;
var origDaysSupply = 0;
var origSIG = '';
var curSIG = '';
var IE = document.all ? true : false;

SetCurrentSIG();

//MU3 Required: This function will examine the last entry and check for numbers.
//if the last entry is a number then the method will parse to a number from a string
//this will create the following:
//10 = 10, 1.00 = 1.0, 1.001 = 1.001, 003.0 = 3.0, .4 = 0.4
function replaceLeadingAndTrailingZeros(e, txtFreeTextSig) {
    if (e.keyCode === 32) {
        var freeText = txtFreeTextSig.value;
        var newText = freeText;
        var lastIndex = freeText.lastIndexOf(" ");


        if (lastIndex === -1) {//only one word in free text

            if (isDate(freeText)) return txtFreeTextSig.value;

            var indexOfSlash = freeText.indexOf('/');
            if (indexOfSlash > -1) //user writing fraction
            {
                newText = ReturnConvertedFraction(freeText, indexOfSlash);
            }
            else {
                var result = parseFloat(freeText);
                if (!isNaN(result)) {
                    newText = result;
                }
            }
        }
        else {//multiple words, just grab the last.

            var laststring = freeText.substring(lastIndex + 1);
            if (isDate(laststring)) return txtFreeTextSig.value;

            var beginning = freeText.substring(0, lastIndex + 1);

            var indexOfSlash2 = laststring.indexOf('/');
            if (indexOfSlash2 > -1) //user writing fraction
            {
                newText = beginning + ReturnConvertedFraction(laststring, indexOfSlash2);
            }
            else {
                var result1 = parseFloat(laststring);
                if (!isNaN(result1)) {
                    newText = beginning + result1;
                }
            }
        }

        var selectionStart = txtFreeTextSig.selectionStart;
        var selectionEnd = txtFreeTextSig.selectionEnd;

        txtFreeTextSig.value = newText;

        txtFreeTextSig.setSelectionRange(selectionStart, selectionEnd);
    }
}

function isDate(wordTyped) {
    if (wordTyped.split('/').length === 3 || wordTyped.split('-').length === 3 || wordTyped.split('.').length === 3) return true;
}

function isValidSigAndMDD(txtMDD, txtFreeTextSig, sigLengthError) {
    var mddLength = 0;
    var txtMDD = document.getElementById(txtMDD.id).value;
    var txtSig = document.getElementById(txtFreeTextSig.id).value;
    if (txtMDD.length > 0) {
        mddLength = " MDD:  Per day".length + txtMDD.length;
    }
    var sigError = document.getElementById(sigLengthError.id);
    if (txtSig.length + mddLength > 1000) {
        if (sigError != null) {
            sigError.style.display = 'inherit';
        }
        return Page_ClientValidate("") && false;
    }
    else if (sigError != null) {
        sigError.style.display = 'none';
    }
    return Page_ClientValidate("");
}

function AllowMDDChange(maxLen, eventObj, txtMDD, txtFreeTextSig, lblMDDError) {

    enteredKeystroke = window.event ? eventObj.keyCode : eventObj.which;
    isNonPrintingKeystroke = ((enteredKeystroke < 32)                               // Non printing
        || (enteredKeystroke >= 33 && enteredKeystroke <= 40)              // Page Up, Down, Home, End, Arrow
        || (enteredKeystroke == 46))                                       // Delete

    // Decide whether the keystroke is allowed to proceed
    if (!isNonPrintingKeystroke) {
        var rtrn = false;
        var txtMDD = document.getElementById(txtMDD.id).value;
        var txtSig = document.getElementById(txtFreeTextSig.id).value;
        if (txtSig != null && txtMDD != null) {
            rtrn = txtSig.length + " MDD:  Per day".length + txtMDD.length + 1 <= 1000;
        }
        var mddError = document.getElementById(lblMDDError.id);
        if (!rtrn) {
            mddError.style.display = 'inherit';
        } else {
            mddError.style.display = 'none';
        }
        return rtrn;
    }
    document.getElementById(lblMDDError.id).style.display = 'none';
    return true;
}

function ReturnConvertedFraction(wordTyped, indexOfSlash) {
    var topOfFraction = wordTyped.substring(0, indexOfSlash);
    var bottomOfFraction = wordTyped.substring(indexOfSlash + 1);
    var topConverted = parseFloat(topOfFraction);
    var bottomConverted = parseFloat(bottomOfFraction);

    if (isNaN(topConverted) || isNaN(bottomConverted)) {
        //Either top or bottom is not a valid number so just ignore it
        return topOfFraction + "/" + bottomOfFraction;
    }
    else {
        //both have converted correctly, concat back together.
        return topConverted + "/" + bottomConverted;
    }
}

function InitializeTrackingVariables() {
    var txtDaysSupply = document.getElementById("ctl00_ContentPlaceHolder1_txtDaysSupply");
    var txtQuantity = document.getElementById("ctl00_ContentPlaceHolder1_txtQuantity");
    var ddlCustomPack = document.getElementById("ctl00_ContentPlaceHolder1_ddlCustomPack");

    if (!IE) {
        if (txtQuantity != null) {
            txtQuantity.setAttribute('onkeyup', 'CalculateDays()');
            txtQuantity.setAttribute('onblur', 'markChecked()');
        }
        if (txtDaysSupply != null) {
            txtDaysSupply.setAttribute('onkeyup', 'CalculateQty()');
            txtDaysSupply.setAttribute('onblur', 'markChecked()');
        }

        if (ddlCustomPack != null) {
            ddlCustomPack.setAttribute('onChange', 'CalculateQty()');
        }
    }
    else {
        if (txtQuantity != null) {
            txtQuantity.attachEvent('onkeyup', CalculateDays);
            txtQuantity.attachEvent('onblur', markChecked);
        }
        if (txtDaysSupply != null) {
            txtDaysSupply.attachEvent('onkeyup', CalculateQty);
            txtDaysSupply.attachEvent('onblur', markChecked);
        }
        if (ddlCustomPack != null) {
            ddlCustomPack.attachEvent('onChange', CalculateQty);
        }
    }

    origQuantity = getMetricQuantity();

    var txtDaysSupply = document.getElementById("ctl00_ContentPlaceHolder1_txtDaysSupply");
    if (txtDaysSupply != null) {
        origDaysSupply = txtDaysSupply.value;
    }

    var txtFreeTextSig
    var txtFreeTextSig = document.getElementById("ctl00_ContentPlaceHolder1_txtFreeTextSig");
    if (txtFreeTextSig != null) {

        //
        // TODO: change to OnChange/OnBlur?
        //
        if (!IE) {
            txtFreeTextSig.setAttribute('onblur', 'UpdateFreeTextSIG()');
        }
        else {
            txtFreeTextSig.attachEvent('onblur', UpdateFreeTextSIG);
        }
    }

    var txtMDD = document.getElementById("ctl00_ContentPlaceHolder1_txtMDD");
    if (txtMDD != null) {
        if (!IE) {
            txtMDD.setAttribute('onblur', 'UpdateFreeTextSIG()');
        }
        else {
            txtMDD.attachEvent('onblur', UpdateFreeTextSIG);
        }
    }

    SetCurrentSIG();
    origSIG = curSIG;
    SetPharmacyResponseType();
}

function SetCurrentSIG() {
    var dd = document.getElementById("ctl00_ContentPlaceHolder1_pnlPreferedSig");
    if (dd != null && dd.style.display == "inline") {
        selectObj = document.getElementById("ctl00_ContentPlaceHolder1_LstPreferedSig");

        if (selectObj.options && selectObj.options.length > 0 && selectObj.selectedIndex != -1) {
            curSIG = selectObj.options[selectObj.selectedIndex].text;
        }
    }
    else {
        dd = document.getElementById("ctl00_ContentPlaceHolder1_pnlAllSig");
        if (dd != null && dd.style.display == "inline") {
            selectObj = document.getElementById("ctl00_ContentPlaceHolder1_LstSig");

            if (selectObj.options && selectObj.options.length > 0 && selectObj.selectedIndex != -1) {
                curSIG = selectObj.options[selectObj.selectedIndex].text;
            }
        }
        else {
            dd = document.getElementById("ctl00_ContentPlaceHolder1_pnlFreeTextSig");
            if (dd != null && dd.style.display == "inline") {
                UpdateFreeTextSIG();
            }
        }
    }
}

function UpdateFreeTextSIG() {
    var txtFreeTextSig = document.getElementById("ctl00_ContentPlaceHolder1_txtFreeTextSig");
    if (txtFreeTextSig != null) {
        curSIG = txtFreeTextSig.value;
    }
    var txtMDD = document.getElementById("ctl00_ContentPlaceHolder1_txtMDD");
    if (txtMDD != null && txtMDD.value.length > 0) {
        SetFreeTextSigCharsRemaining(txtFreeTextSig.value.length + txtMDD.value.length + " MDD:  Per Day".length)
    }
    else {
        SetFreeTextSigCharsRemaining(txtFreeTextSig.value.length)
    }

    SetPharmacyResponseType()
}

function SetFreeTextSigCharsRemaining(freeTextSigLength) {
    var maxLen = 1000;
    var charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining");
    if (charsRemaining != null)
        setInnerText(charsRemaining, (maxLen - freeTextSigLength).toString());
}

function SetPharmacyResponseType() {
    var curQuantity = getMetricQuantity();
    var curDaysSupply = 0;
    var txtDaysSupply = document.getElementById("ctl00_ContentPlaceHolder1_txtDaysSupply");
    if (txtDaysSupply != null) {
        curDaysSupply = txtDaysSupply.value;
    }

    var type = 'N';
    var defaultResponseType = document.getElementById("ctl00_ContentPlaceHolder1_defaultResponseType");
    if (defaultResponseType != null) {
        if (defaultResponseType.value != '') {
            type = defaultResponseType.value;
        }
    }

    SetPharmacyCommentsMaxLength(type)
}

function SetPharmacyCommentsMaxLength(type) {
    var maxlen = 70;
    if (type == 'N') {
        maxlen = 210;
    }

    // update max len hidden input control
    var maxlenContainer = document.getElementById("ctl00_ContentPlaceHolder1_maxlenContainer");
    if (maxlenContainer != null) {
        maxlenContainer.value = maxlen.toString();
    }

    // update max characters text for pharmacy comments
    var maxCharacters = document.getElementById("ctl00_ContentPlaceHolder1_maxCharacters");
    if (maxCharacters != null) {
        setInnerText(maxCharacters, maxlen.toString());
    }
    var pharmCommentsLength = 0;
    var txtPharmComments = document.getElementById("ctl00_ContentPlaceHolder1_txtPharmComments");

    if (txtPharmComments != null)
        pharmCommentsLength = txtPharmComments.value.length;

    // update characters remaining for pharmacy comments
    var charsRemaining2 = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining2");

    var hdnpharmacyNote = document.getElementById("hiddenPharmacyNote");

    if (charsRemaining2 != null) {
        if (hdnpharmacyNote != undefined && hdnpharmacyNote != null)
            setInnerText(charsRemaining2, (maxlen - pharmCommentsLength - hdnpharmacyNote.value.length).toString());
        else
            setInnerText(charsRemaining2, (maxlen - pharmCommentsLength).toString());
    }


    // add event handler to Pharmacy Comments text box
    if (txtPharmComments != null) {
        if (pharmCommentsLength > maxlen) {
            txtPharmComments.value = txtPharmComments.value.substring(0, maxlen);
        }

        if (!IE) {
            txtPharmComments.setAttribute('onkeydown', "return LimitInput(this, '" + maxlen.toString() + "', event);");
            txtPharmComments.setAttribute('onkeyup', "return LimitInput(this, '" + maxlen.toString() + "', event);");
            txtPharmComments.setAttribute('onpaste', "return LimitPaste(this, '" + maxlen.toString() + "', event);");
            txtPharmComments.setAttribute('onchange', "return LimitChange(this, '" + maxlen.toString() + "', event);");
        }
        else {
            txtPharmComments.attachEvent('onkeydown', LimitInputIE);
            txtPharmComments.attachEvent('onkeyup', LimitInputIE);
            txtPharmComments.attachEvent('onpaste', LimitPasteIE);
            txtPharmComments.attachEvent('onchange', LimitChangeIE);
        }
    }
}

function setInnerText(ctl, content) {
    var isIE = (window.navigator.userAgent.indexOf("MSIE") > 0);

    if (!isIE)
        ctl.textContent = content;
    else
        ctl.innerText = content;
}

function ToggleSIGs(view) {
    var pnlPreferedSig = document.getElementById("ctl00_ContentPlaceHolder1_pnlPreferedSig");
    var pnlAllSig = document.getElementById("ctl00_ContentPlaceHolder1_pnlAllSig");
    var pnlFreeTextSig = document.getElementById("ctl00_ContentPlaceHolder1_pnlFreeTextSig");
    var heading = document.getElementById("ctl00_ContentPlaceHolder1_heading");
    var sigType = document.getElementById("ctl00_ContentPlaceHolder1_sigType");

    if (pnlPreferedSig != null && pnlAllSig != null && pnlFreeTextSig != null) {
        pnlPreferedSig.style.display = "none";
        pnlAllSig.style.display = "none";
        pnlFreeTextSig.style.display = "none";

        if (view == 'P') {
            pnlPreferedSig.style.display = "inline";
            setInnerText(heading, "Choose SIG : Preferred ");
        }
        else if (view == 'A') {
            pnlAllSig.style.display = "inline";
            setInnerText(heading, "Choose SIG : All ");
        }
        else {
            pnlFreeTextSig.style.display = "inline";
            setInnerText(heading, "Choose SIG : Free Text ");
            var txtFreeTextSig = document.getElementById("ctl00_ContentPlaceHolder1_txtFreeTextSig");
            if (txtFreeTextSig != null) {
                txtFreeTextSig.focus();
            }
        }
    }

    if (sigType != null) {
        sigType.value = view;
    }

    SetCurrentSIG();
}

//
// should be called on KeyDown/KeyUp event
//
function LimitInputIE(eventArgs) {
    var maxlen = '210';
    var maxlenContainer = document.getElementById("ctl00_ContentPlaceHolder1_maxlenContainer");
    if (maxlenContainer != null) {
        maxlen = maxlenContainer.value;
    }

    var target = document.getElementById(eventArgs["srcElement"]["id"]);
    LimitInput(target, maxlen, eventArgs);
}

//
// should be called on KeyDown/KeyUp event
// change from KeyPress to KeyDown/KeyUp events because some browswers do not trigger KeyPress for non-character keys
//
function LimitInput(targetField, maxLen, eventObj) {
    var isPermittedKeystroke;
    var enteredKeystroke;
    var maximumFieldLength;
    var currentFieldLength;
    var inputAllowed = true;
    var selectionLength = 0;

    if (maxLen != null) {
        // Get the current and maximum field length
        currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = parseInt(maxLen);

        var hdnpharmacyNote = document.getElementById("hiddenPharmacyNote");

        if (targetField.id === "ctl00_ContentPlaceHolder1_txtPharmComments") {
            currentFieldLength = currentFieldLength + hdnpharmacyNote.value.length;
        }

        var charsRemaining = null;

        if (targetField.id == "ctl00_ContentPlaceHolder1_txtFreeTextSig") {
            charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining");
            var txtMDD = document.getElementById("ctl00_ContentPlaceHolder1_txtMDD");
            if (txtMDD != null && txtMDD.value.length > 0) {
                maximumFieldLength -= (txtMDD.value.length + " MDD:  Per day".length);
            }
        }
        else if (targetField.id == "ctl00_ContentPlaceHolder1_txtPharmComments") {
            charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining2");
        }
        // Allow non-printing, arrow and delete keys
        enteredKeystroke = window.event ? eventObj.keyCode : eventObj.which;

        isNonPrintingKeystroke = ((enteredKeystroke < 32)                               // Non printing
            || (enteredKeystroke >= 33 && enteredKeystroke <= 40)              // Page Up, Down, Home, End, Arrow
            || (enteredKeystroke == 46))                                       // Delete

        // Decide whether the keystroke is allowed to proceed
        if (!isNonPrintingKeystroke) {
            // if the addition of the KeyDown/KeyUp event is larger than max length then cancel the KeyDown/KeyUp event
            // since we are doing KeyUp event we can merely truncate input if it exceeds max length
            //if ((currentFieldLength - selectionLength) >= maximumFieldLength) {
            if (currentFieldLength >= maximumFieldLength) {
                inputAllowed = false;
            }
        }
    }



    if (charsRemaining != null) {
        charsRemaining.innerHTML = maximumFieldLength - currentFieldLength;
    }

    if (window.event) {
        window.event.returnValue = inputAllowed;
    }

    return (inputAllowed);
}

//
// Limit the text input in the specified field. Should be called on OnPaste event
//
function LimitPasteIE(eventArgs) {
    var maxlen = '210';
    var maxlenContainer = document.getElementById("ctl00_ContentPlaceHolder1_maxlenContainer");
    if (maxlenContainer != null) {
        maxlen = maxlenContainer.value;
    }

    var target = document.getElementById(eventArgs["srcElement"]["id"]);
    LimitPaste(target, maxlen, eventArgs);
}

//
// Limit the text input in the specified field. Should be called on OnPaste event
//
function LimitPaste(targetField, maxLen, eventArgs) {
    var clipboardText;
    var resultantLength;
    var maximumFieldLength;
    var currentFieldLength;
    var pasteAllowed = true;
    var selectionLength = 0;

    if (maxLen != null) {
        // Get the current and maximum field length
        currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = parseInt(maxLen);

        // get the length of any text currently selected in the target field text box
        if (window.getSelection) {  // all browsers, except IE before version 9
            if (document.activeElement &&
                (document.activeElement.tagName.toLowerCase() == "textarea" ||
                    document.activeElement.tagName.toLowerCase() == "input")) {
                var text = document.activeElement.value;
                selectionLength = document.activeElement.selectionEnd - document.activeElement.selectionStart;
            }
            else {
                var selRange = window.getSelection();
                selectionLength = selRange.toString().length;
            }
        }
        else {
            if (document.selection.createRange) { // Internet Explorer
                var range = document.selection.createRange();
                selectionLength = range.text.length;
            }
        }

        // get the length of the text to be pasted
        if (window.clipboardData && window.clipboardData.getData) { // IE
            clipboardText = window.clipboardData.getData("Text");
        } else if (eventArgs.clipboardData && eventArgs.clipboardData.getData) {
            clipboardText = eventArgs.clipboardData.getData('text/plain');
        }

        // calculate the resulting lenth of text of the paste operation
        resultantLength = currentFieldLength + clipboardText.length - selectionLength;

        var txtMDD = document.getElementById("ctl00_ContentPlaceHolder1_txtMDD");
        if (txtMDD != null && txtMDD.value.length > 0) {
            maximumFieldLength -= (txtMDD.value.length + " MDD:  Per day".length);
        }

        // if the addition of the paste is larger than max length then cancel the paste event
        if (resultantLength > maximumFieldLength) {
            pasteAllowed = false;
        }

        var charsRemaining = null;

        if (targetField.id == "ctl00_ContentPlaceHolder1_txtFreeTextSig") {
            charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining");
        }
        else if (targetField.id == "ctl00_ContentPlaceHolder1_txtPharmComments") {
            charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining2");
        }

        if (charsRemaining != null) {
            charsRemaining.innerHTML = maximumFieldLength - currentFieldLength;
        }
    }

    if (window.event) {
        window.event.returnValue = pasteAllowed;
    }

    return (pasteAllowed);
}

function LimitChangeIE(eventArgs) {
    //
    // TODO: does this need to be more dynamically set to be either 210 (comments) or 140 (sig)
    //
    var maxlen = '210';
    var maxlenContainer = document.getElementById("ctl00_ContentPlaceHolder1_maxlenContainer");
    if (maxlenContainer != null) {
        maxlen = maxlenContainer.value;
    }

    var target = document.getElementById(eventArgs["srcElement"]["id"]);
    LimitChange(target, maxlen, eventArgs);
}

//
// Limit the text input in the specified field. Should be called on OnChange event
//
function LimitChange(targetField, maxLen, eventObj) {
    var maximumFieldLength;
    var currentFieldLength;

    if (maxLen != null) {
        var charsRemaining = null;
        currentFieldLength = parseInt(targetField.value.length);
        maximumFieldLength = maxLen;

        if (targetField.id == "ctl00_ContentPlaceHolder1_txtFreeTextSig") {
            charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining");
            var txtMDD = document.getElementById("ctl00_ContentPlaceHolder1_txtMDD");
            if (txtMDD != null && txtMDD.value.length > 0) {
                maximumFieldLength -= (txtMDD.value.length + " MDD:  Per day".length);
            }
        }
        else if (targetField.id == "ctl00_ContentPlaceHolder1_txtPharmComments") {
            charsRemaining = document.getElementById("ctl00_ContentPlaceHolder1_charsRemaining2");
        }

        var hdnpharmacyNote = document.getElementById("hiddenPharmacyNote");

        if (targetField.id === "ctl00_ContentPlaceHolder1_txtPharmComments") {
            currentFieldLength = currentFieldLength + hdnpharmacyNote.value.length;
        }

        if (currentFieldLength > maximumFieldLength) {
            targetField.value = targetField.value.substring(0, maximumFieldLength);
            currentFieldLength = maximumFieldLength;
        }

        if (charsRemaining != null) {
            charsRemaining.innerHTML = maximumFieldLength - currentFieldLength;
        }
    }
}

function ClientValidate(source, arguments) {
    var cc = arguments.Value;
    if (cc.length > 255) {
        arguments.IsValid = false;
        return;
    }
    else {
        arguments.IsValid = true;
        return;
    }
}

var alreadyChecked = false;
var calcField = "";

function markChecked() {
    alreadyChecked = true;
}

function getSIGListID() {
    var dd = document.getElementById("ctl00_ContentPlaceHolder1_pnlPreferedSig");
    if (dd != null && dd.style.display == "inline") {
        return "ctl00_ContentPlaceHolder1_LstPreferedSig";
    }
    else {
        dd = document.getElementById("ctl00_ContentPlaceHolder1_pnlAllSig");
        if (dd != null && dd.style.display == "inline") {
            return "ctl00_ContentPlaceHolder1_LstSig";
        }
    }

    return "";
}

function CalculateQty() {
    var LstSigID = document.getElementById(getSIGListID());
    var DaysID = document.getElementById("ctl00_ContentPlaceHolder1_txtDaysSupply");
    var QtyID = document.getElementById("ctl00_ContentPlaceHolder1_txtQuantity");
    var PSPQID = document.getElementById("ctl00_ContentPlaceHolder1_ddlCustomPack");
    var lblQtyDes = document.getElementById("ctl00_ContentPlaceHolder1_lblQuantity");
    var QtyUnits = document.getElementById("ctl00_ContentPlaceHolder1_txtQuantityUnits");
    QtyUnits.value = 0;
    var metricQuantity = 0;
    var metricDaysSupply = 0;

    if (DaysID != null)
        metricDaysSupply = parseInt(DaysID.value);

    if (calcField == "")
        calcField = "QTY";

    if (LstSigID != null) {
        var stringDQ = LstSigID.value;
        var msg = "";
        msg = msg + " " + stringDQ;
        var finddq = stringDQ.indexOf("[DQ");
        msg = msg + " " + finddq;
        var currOffset = parseInt(finddq);

        if (stringDQ.indexOf("[DQ") != -1) {
            var getDQ = stringDQ.substring(currOffset + 4, stringDQ.length - 1);  //Dailyquanity
            if (getDQ <= 0) {
                // To check and display the quantity ...
                setInnerText(lblQtyDes, ""); //lblQtyDes.innerText="";
                if (QtyID != null && PSPQID != null)               //Got the quantity value.
                {
                    if (QtyID.value > 0) {
                        var strQty = QtyID.value;

                        if (PSPQID != null) {
                            var PSPQvalue = PSPQID.value;
                            if (PSPQvalue.indexOf("[PZ=") != -1)     //Get the package size ...
                            {
                                var PZstIndex = PSPQvalue.indexOf("[PZ");
                                var PZendIndex = PSPQvalue.indexOf("]");
                                PackSize = parseFloat(PSPQvalue.substring(PZstIndex + 4, PZendIndex));
                                msg = msg + " " + PackSize;
                            }
                            if (PSPQvalue.indexOf("(PQ=") != -1)           //Get the package quantity
                            {
                                var PQstIndex = PSPQvalue.indexOf("(PQ");
                                var PQendIndex = PSPQvalue.indexOf(")");
                                PackQuantity = parseInt(PSPQvalue.substring(PQstIndex + 4, PQendIndex));
                                msg = msg + " " + PackQuantity;
                            }
                        }

                        var TotalQty = strQty * PackSize;
                        if (isNaN(TotalQty)) {
                            setInnerText(lblQtyDes, "Quantity =" + strQty + "(" + strQty + " x " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = strQty;
                        }
                        else {
                            setInnerText(lblQtyDes, "Quantity =" + TotalQty + "(" + strQty + " x " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = TotalQty;
                        }
                    }
                }
                // End of Display message
                return false;
            }

            if (DaysID != null) {
                var DaysSupply = parseInt(DaysID.value);    //got the dayssupply value.
                msg = msg + " " + DaysSupply;
            }
            if (QtyID != null)               //Got the quantity value.
            {
                var txtQty = QtyID.value;
                msg = msg + " " + txtQty;
            }
            var PackSize = 1;
            var PackQuantity = 1;
            metricQuantity = txtQty;

            if (PSPQID != null) {
                var PSPQvalue = PSPQID.value;
                msg = msg + " " + PSPQvalue;

                if (PSPQvalue == "")   // no value there
                {
                    PackSize = 1;
                    PackQuantity = 1;
                }
                else {
                    if (PSPQvalue.indexOf("[PZ=") != -1)     //Get the package size ...
                    {
                        var PZstIndex = PSPQvalue.indexOf("[PZ");
                        var PZendIndex = PSPQvalue.indexOf("]");
                        PackSize = parseFloat(PSPQvalue.substring(PZstIndex + 4, PZendIndex));
                        msg = msg + " " + PackSize;
                    }
                    if (PSPQvalue.indexOf("(PQ=") != -1)           //Get the package quantity
                    {
                        var PQstIndex = PSPQvalue.indexOf("(PQ");
                        var PQendIndex = PSPQvalue.indexOf(")");
                        PackQuantity = parseInt(PSPQvalue.substring(PQstIndex + 4, PQendIndex));
                        msg = msg + " " + PackQuantity;
                    }
                    metricQuantity = txtQty * PackSize * PackQuantity;
                }
            }

            if (DaysSupply > 0) {
                if (calcField == "QTY") {
                    QtyID.value = parseFloat(((getDQ * DaysSupply) / (PackSize * PackQuantity)).toFixed(4));
                }

                if (PSPQID != null) {
                    var DisplayQty = (QtyID.value * PackSize * PackQuantity)
                    if (DisplayQty != null) {
                        var CheckEAML = PSPQID.options[PSPQID.selectedIndex].text;  //getting wheter EA ,ML present
                        var reEA = /^EA/;
                        var reML = /^ML/;
                        var FoundEA = reEA.exec(CheckEAML);
                        var FoundML = reML.exec(CheckEAML);

                        if (FoundEA == null && FoundML == null) {
                            setInnerText(lblQtyDes, "Quantity =" + DisplayQty + "(" + QtyID.value + " x " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = DisplayQty;
                        }
                        else {
                            //This is for EA and ML
                            setInnerText(lblQtyDes, "Quantity =" + DisplayQty + "(" + QtyID.value + " x " + "1 " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = DisplayQty;
                        }
                    }
                }
            }
            else {
                if (!alreadyChecked) {
                    QtyID.value = "";
                    setInnerText(lblQtyDes, "");
                    QtyUnits.value = 0;
                }
            }
        }
    }

    SetPharmacyResponseType();
}

function CalculateDays() {
    var LstSigID = document.getElementById(getSIGListID());
    var DaysID = document.getElementById("ctl00_ContentPlaceHolder1_txtDaysSupply");
    var QtyID = document.getElementById("ctl00_ContentPlaceHolder1_txtQuantity");
    var PSPQID = document.getElementById("ctl00_ContentPlaceHolder1_ddlCustomPack");
    var lblQtyDes = document.getElementById("ctl00_ContentPlaceHolder1_lblQuantity");
    var QtyUnits = document.getElementById("ctl00_ContentPlaceHolder1_txtQuantityUnits");

    QtyUnits.value = 0;
    var metricQuantity = 0;
    var metricDaysSupply = 0;

    if (DaysID != null)
        metricDaysSupply = parseInt(DaysID.value);

    if (calcField == "")
        calcField = "DS";
    if (LstSigID != null) {
        var stringDQ = LstSigID.value;
        var msg = "";
        msg = msg + " " + stringDQ;
        var finddq = stringDQ.indexOf("[DQ");
        msg = msg + " " + finddq;
        var currOffset = parseInt(finddq);
        msg = msg + " " + currOffset;
        if (stringDQ.indexOf("[DQ") != -1) {
            var getDQ = stringDQ.substring(currOffset + 4, stringDQ.length - 1);  //Dailyquanity
            if (getDQ <= 0) {
                setInnerText(lblQtyDes, "");
                if (QtyID != null && PSPQID != null) {
                    if (QtyID.value > 0) {
                        var strQty = QtyID.value;
                        //..............................................................
                        if (PSPQID != null) {
                            var PSPQvalue = PSPQID.value;
                            if (PSPQvalue.indexOf("[PZ=") != -1)     //Get the package size ...
                            {
                                var PZstIndex = PSPQvalue.indexOf("[PZ");
                                var PZendIndex = PSPQvalue.indexOf("]");
                                PackSize = (PSPQvalue.substring(PZstIndex + 4, PZendIndex));

                                msg = msg + " " + PackSize;
                            }
                            if (PSPQvalue.indexOf("(PQ=") != -1)           //Get the package quantity
                            {
                                var PQstIndex = PSPQvalue.indexOf("(PQ");
                                var PQendIndex = PSPQvalue.indexOf(")");
                                PackQuantity = (PSPQvalue.substring(PQstIndex + 4, PQendIndex));

                                msg = msg + " " + PackQuantity;
                            }
                        }

                        var TotalQty = (strQty * PackSize);
                        if (TotalQty > TotalQty)
                            TotalQty = TotalQty.toFixed(2);
                        if (isNaN(TotalQty)) {
                            setInnerText(lblQtyDes, "Quantity = " + strQty + "(" + strQty + " x " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = strQty;
                        }
                        else {
                            setInnerText(lblQtyDes, "Quantity = " + TotalQty + "(" + strQty + " x " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = TotalQty;
                        }
                    }
                }
                // Check message for quantity
                return false;
            }

            if (DaysID != null && !alreadyChecked) {
                var DaysSupply = parseInt(DaysID.value);    //got the dayssupply value.
                DaysSupply = Math.round(DaysSupply);
                msg = msg + "" + DaysSupply;
            }
            if (QtyID != null)               //Got the quantity value.
            {
                var txtQty = QtyID.value;
                msg = msg + "" + txtQty;
            }
            var PackSize = 1;
            var PackQuantity = 1;

            if (PSPQID != null) {
                var PSPQvalue = PSPQID.value;
                msg = msg + "" + PSPQvalue;

                if (PSPQvalue == "")   // no value there
                {
                    PackSize = 1;
                    PackQuantity = 1;
                    metricQuantity = txtQty;
                }
                else {
                    if (PSPQvalue.indexOf("[PZ=") != -1)     //Get the package size ...
                    {
                        var PZstIndex = PSPQvalue.indexOf("[PZ");
                        var PZendIndex = PSPQvalue.indexOf("]");
                        PackSize = parseFloat(PSPQvalue.substring(PZstIndex + 4, PZendIndex));
                        msg = msg + " " + PackSize;
                    }
                    if (PSPQvalue.indexOf("(PQ=") != -1)           //Get the package quantity
                    {
                        var PQstIndex = PSPQvalue.indexOf("(PQ");
                        var PQendIndex = PSPQvalue.indexOf(")");
                        PackQuantity = parseInt(PSPQvalue.substring(PQstIndex + 4, PQendIndex));
                        msg = msg + " " + PackQuantity;
                        metricQuantity = txtQty * PackQuantity * PackSize;
                    }
                }  //End of else
            }
            else {
                metricQuantity = txtQty;
            }

            if (txtQty > 0) {
                if (calcField == "DS") {
                    // Display of the Message ..
                    DaysID.value = (txtQty * PackSize * PackQuantity) / getDQ;
                }
                if (PSPQID != null) {
                    var DisplayQty = (txtQty * PackSize * PackQuantity);
                    metricQuantity = DisplayQty;
                    //******************************************************
                    if (DisplayQty != null) {
                        var CheckEAML = PSPQID.options[PSPQID.selectedIndex].text;  //getting wheter EA ,ML present
                        var reEA = /^EA/;
                        var reML = /^ML/;
                        var FoundEA = reEA.exec(CheckEAML);
                        var FoundML = reML.exec(CheckEAML);

                        if (FoundEA == null && FoundML == null) {
                            setInnerText(lblQtyDes, "Quantity =" + DisplayQty + "(" + txtQty + " x " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = DisplayQty;
                        }
                        else {
                            setInnerText(lblQtyDes, "Quantity =" + DisplayQty + "(" + txtQty + " x " + "1 " + PSPQID.options[PSPQID.selectedIndex].text + ")");
                            QtyUnits.value = DisplayQty;
                        }
                    }
                }
                else {
                    metricQuantity = txtQty;
                }
            }
            else {
                if (!alreadyChecked) {
                    DaysID.value = "";
                    setInnerText(lblQtyDes, "");
                    QtyUnits.value = 0;
                }
            }
        }
    }

    SetPharmacyResponseType();
}

function init() {
    var dd = document.getElementById("pnlPreferedSig");
    if (dd != null)
        dd.style.display = "none";

    dd = document.getElementById("pnlAllSig");
    if (dd != null)
        dd.style.display = "none";
}

function selectSig(id) {
    switch (id) {
        case 0:
            var dd = document.getElementById("pnlPreferedSig");
            dd.style.display = "inline";
            dd = document.getElementById("pnlAllSig");
            dd.style.display = "none";
            dd = document.getElementById("pnlFreeTextSig");
            dd.style.display = "none";
            break;
        case 1:
            var dd = document.getElementById("pnlPreferedSig");
            dd.style.display = "none";
            dd = document.getElementById("pnlAllSig");
            dd.style.display = "inline";
            dd = document.getElementById("pnlFreeTextSig");
            dd.style.display = "none";
            break;
        case 2:
            break;
    }
}

function btnPatientEdu_onclick() {
    window.open('PatEducation.aspx', 'PatEdu', 'menubar=yes,Width=720,Height=400,scrollbars=yes,statusbar=no,help=no,resizable=yes');
}

function setSelectToolTip(selectObj) {
    if (selectObj) {
        if (selectObj.options && selectObj.options.length > 0 && selectObj.selectedIndex != -1)
            selectObj.title = selectObj.options[selectObj.selectedIndex].text;
    }
}

function setSelectToolTipOnSelect(selectObj) {
    if (selectObj) {
        if (selectObj.options && selectObj.options.length > 0 && selectObj.selectedIndex != -1) {
            selectObj.title = selectObj.options[selectObj.selectedIndex].text;
            curSIG = selectObj.options[selectObj.selectedIndex].text;
            SetPharmacyResponseType();
        }
    }
}

function setInnerText(ctl, content) {
    var isIE = (window.navigator.userAgent.indexOf("MSIE") > 0);

    if (!isIE)
        ctl.textContent = content;
    else
        ctl.innerText = content;
}

function getMetricQuantity() {
    var quantity = document.getElementById("ctl00_ContentPlaceHolder1_txtQuantity");
    var PSPQID = document.getElementById("ctl00_ContentPlaceHolder1_ddlCustomPack");
    var PackSize = 1;
    var PackQuantity = 1;
    var TypedQuantity = 0;
    if (quantity != null && PSPQID != null) {
        TypedQuantity = parseFloat(quantity.value)
        var PSPQvalue = PSPQID.options[PSPQID.selectedIndex].value
        if (PSPQvalue != null) {
            if (PSPQvalue.indexOf("[PZ=") != -1) {
                var PZstIndex = PSPQvalue.indexOf("[PZ");
                var PZendIndex = PSPQvalue.indexOf("]");
                PackSize = parseFloat((PSPQvalue.substring(PZstIndex + 4, PZendIndex)));
            }

            if (PSPQvalue.indexOf("(PQ=") != -1) {
                var PQstIndex = PSPQvalue.indexOf("(PQ");
                var PQendIndex = PSPQvalue.indexOf(")");
                PackQuantity = parseFloat((PSPQvalue.substring(PQstIndex + 4, PQendIndex)));
            }
        }
    }
    else if (quantity != null) {
        TypedQuantity = parseFloat(quantity.value);
    }

    if (PackSize == null) {
        PackSize = 1;
    }

    if (PackQuantity == null) {
        PackQuantity = 1;
    }

    if (TypedQuantity == null) {
        TypedQuantity = 0;
    }

    return TypedQuantity * PackSize * PackQuantity;
}
