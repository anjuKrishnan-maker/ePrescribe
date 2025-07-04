function isNumericValue(sText) {
    sText = sText.replace(/^\s+|\s+$/g, '');
    var ValidChars = "0123456789.";
    var Char;

    if (sText == "") {
        return false;
    }
    else {
        for (i = 0; i < sText.length; i++) {
            Char = sText.charAt(i);
            if (ValidChars.indexOf(Char) == -1) {
                return false;
            }
        }

        var index = sText.indexOf('.');

        if (index >= 0) {
            // check for more than one decimal point
            if (sText.indexOf('.', (index + 1)) >= 0) {
                return false;
            }

            // Check for four digits after decimal point
            var CharAfterdot = sText.length - index;
            if (CharAfterdot > 5) {
                return false;
            }

            // restrict to answer length to 10 char before decimal
            if ((sText.substring(0, index).length) > 10) {
                return false;
            }
        }
        else {
            // restrict to answer length to 15 char.
            if (sText.length > 10) {
                return false;
            }
        }
    }
    return true;
}

function pullInitQSAnswerJSON(skipQuestion) {
    var addlTextBox = null;
    var answerSet = '{"Answers":[';
    var i = 0;
    for (i = 0; i < document.forms[0].elements.length; i = i + 1) {
        var control = document.forms[0].elements[i];
        if (control != null && control.getAttribute("ePAAnswerChoiceID") != null && control.getAttribute("ePAParentQuestionID") == skipQuestion) {
            if (control.type == 'checkbox'
                || control.type == 'radio') {
                control.checked = false;
            }
            else if (control.type == 'textarea'
                || control.type == 'text') {
                control.value = '';
            }

            if (control.type == 'checkbox'
                || control.type == 'radio') {
                addlTextBox = document.getElementById(control.getAttribute("ePAAdditionalTextControlClientID"));
                if (addlTextBox != null) {
                    addlTextBox.value = '';
                }
            }
            break;
        }
        else if (control != null && control.getAttribute("ePAAnswerChoiceID") != null && control.getAttribute("ePAParentQuestionID") != skipQuestion) {
            answerSet += '{';
            answerSet += '"answerChoiceID":"' + control.getAttribute("ePAAnswerChoiceID").toString() + '"';
            answerSet += ',' + '"parentQuestionID":"' + control.getAttribute("ePAParentQuestionID").toString() + '"';
            if (control.type == 'checkbox'
                || control.type == 'radio') {
                answerSet += ',' + '"value":"' + control.checked.toString() + '"';
            }
            else if (control.type == 'textarea'
                || control.type == 'text') {
                answerSet += ',' + '"value":"' + replaceEscapeCharInString(control.value).toString() + '"';
            }

            if (control.type == 'checkbox'
                || control.type == 'radio') {
                addlTextBox = document.getElementById(control.getAttribute("ePAAdditionalTextControlClientID"));
                if (addlTextBox != null) {
                    answerSet += ',' + '"addText":"' + replaceEscapeCharInString(addlTextBox.value).toString() + '"';
                }
            }
            answerSet += '},';
        }
    }
    //cycle through all divs to find date controls
    var divs = document.getElementsByTagName('div')
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control != null && control.getAttribute("ePADateTimeAnswerControlClientID") != null) {
            var dateControl = $find(control.getAttribute("ePADateTimeAnswerControlClientID"))
            if (dateControl != null) {
                if (dateControl.get_selectedDate() != null && control.getAttribute("ePAParentQuestionID") != skipQuestion) {
                    answerSet += '{';
                    answerSet += '"answerChoiceID":"' + control.getAttribute("ePAAnswerChoiceID").toString() + '"';
                    answerSet += ',' + '"parentQuestionID":"' + control.getAttribute("ePAParentQuestionID").toString() + '"';
                    answerSet += ',' + '"value":"' + dateControl.get_selectedDate().format("MM/dd/yyyy HH:mm").toString() + '"';
                    answerSet += '},';
                }
                else if (dateControl.get_selectedDate() != null && control.getAttribute("ePAParentQuestionID") == skipQuestion) {
                    dateControl.clear();
                }
            }
        }
    }
    var slicedAnswerset = (answerSet[answerSet.length - 1] == ",") ? answerSet.slice(0, -1) : answerSet;
    answerSet = slicedAnswerset + ']}';

    return answerSet;
}

function replaceEscapeCharInString(freetextstring) {
    var ret = "";
    ret = freetextstring;
    ret = ret.replace(/&/gi, "&amp;");
    ret = ret.replace(/"/gi, "&quot;");
    ret = ret.replace(/'/gi, "&apos;");
    ret = ret.replace(/</gi, "&lt;");
    ret = ret.replace(/>/gi, "&gt;");
    return ret;
}

function validFreeTextAns(currentQuestionID) {
    var ret = false;
    //cycle through all divs to find date controls
    var divs = document.getElementsByTagName('div')
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control != null && control.getAttribute("ePAQuestionID") == currentQuestionID) {
            var answerControlClientID = control.getAttribute("ePAFreeTextAnswerControlClientID");
            var textControl = document.getElementById(answerControlClientID);
            if (textControl != null) {
                var trimmed = textControl.value.replace(/^\s+|\s+$/g, '');
                ret = (trimmed != "");
            }
            break;
        }
    }

    return ret;
}

function validDateAns(currentQuestionID) {
    var ret = false;
    //cycle through all divs to find date controls
    var divs = document.getElementsByTagName('div')
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control != null && control.getAttribute("ePAQuestionID") == currentQuestionID) {
            var answerControlClientID = control.getAttribute("ePADateTimeAnswerControlClientID");
            var dateControl = $find(answerControlClientID)
            if (dateControl != null) {
                ret = (dateControl.get_selectedDate() != null);
            }
            break;
        }
    }

    return ret;
}

function validNumberAns(currentQuestionID) {
    var ret = false;
    //cycle through all divs to find date controls
    var divs = document.getElementsByTagName('div')
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control != null && control.getAttribute("ePAQuestionID") == currentQuestionID) {
            var answerControlClientID = control.getAttribute("ePANumberAnswerControlClientID");
            var txtNumAnswer = document.getElementById(answerControlClientID);
            ret = isNumericValue(txtNumAnswer.value);
            break;
        }
    }

    return ret;
}

function checkNumberValue(answerControlClientID, curQuestionID) {
    var ret = false;
    var txtNumAnswer = document.getElementById(answerControlClientID);

    if (txtNumAnswer != null) {
        var num = 0;

        if (isNumericValue(txtNumAnswer.value)) {
            num = +(txtNumAnswer.value);
            ret = true;
        }

        if (ret) {
            var checkOneComp = "";
            var checkOneVal = 0;
            var checkTwoComp = "";
            var checkTwoVal = 0;
            var checkLog = "";
            var nextQuestionID = "";
            var numOK = false;
            nextQuestionID = txtNumAnswer.getAttribute("ePADefaultNextQuestionID");
            for (i = 0; i < document.forms[0].elements.length; i = i + 1) {
                var control = document.forms[0].elements[i];
                if (control != null && control.getAttribute("ePANumberCheckQuestionID") != null && control.getAttribute("ePANumberCheckQuestionID") == curQuestionID) {
                    checkOneComp = control.getAttribute("ePANumberCheckOneComp");
                    checkOneVal = +(control.getAttribute("ePANumberCheckOneVal"));
                    checkTwoComp = control.getAttribute("ePANumberCheckTwoComp");
                    checkTwoVal = +(control.getAttribute("ePANumberCheckTwoVal"));
                    checkLog = control.getAttribute("ePANumberCheckLogic");
                    numOK = executeNumberComparisionLogic(checkOneComp, checkOneVal, checkTwoComp, checkTwoVal, checkLog, num);

                    if (numOK) {
                        nextQuestionID = control.getAttribute("ePANextQuestionID");
                        break;
                    }
                }
            }
            if (curQuestionID != null && nextQuestionID != null) {
                enableInitQSListNextBtn(nextQuestionID);
            }
        }
    }

    if (!ret) {
        disableInitQSListNextBtn(true);
    }
    return ret;
}

function executeNumberComparisionLogic(checkOneComp, checkOneVal, checkTwoComp, checkTwoVal, checkLog, num) {
    var failedOne = "0";
    var failedTwo = "0";
    var numOK = false;
    if (checkOneComp != null && checkOneVal != null) {
        var checkOneValNum = +(checkOneVal);
        if (checkOneComp == "EQ" && num != checkOneValNum) {
            failedOne = "1";
        }
        else if (checkOneComp == "GT" && num <= checkOneValNum) {
            failedOne = "1";
        }
        else if (checkOneComp == "LT" && num >= checkOneValNum) {
            failedOne = "1";
        }
        else if (checkOneComp == "GE" && num < checkOneValNum) {
            failedOne = "1";
        }
        else if (checkOneComp == "LE" && num > checkOneValNum) {
            failedOne = "1";
        }
        else if (checkOneComp == "NE" && num == checkOneValNum) {
            failedOne = "1";
        }
    }

    if (checkTwoComp != null && checkTwoVal != null) {
        var checkTwoValNum = +(checkTwoVal);
        if (checkTwoComp == "EQ" && num != checkTwoValNum) {
            failedTwo = "1";
        }
        else if (checkTwoComp == "GT" && num <= checkTwoValNum) {
            failedTwo = "1";
        }
        else if (checkTwoComp == "LT" && num >= checkTwoValNum) {
            failedTwo = "1";
        }
        else if (checkTwoComp == "GE" && num < checkTwoValNum) {
            failedTwo = "1";
        }
        else if (checkTwoComp == "LE" && num > checkTwoValNum) {
            failedTwo = "1";
        }
        else if (checkTwoComp == "NE" && num == checkTwoValNum) {
            failedTwo = "1";
        }
    }

    if (checkLog != null) {
        if (checkLog == "S" && failedOne == "0") {
            numOK = true;
        }
        else if (checkLog == "R" && (failedOne == "0" && failedTwo == "0")) {
            numOK = true;
        }
    }

    return numOK;
}

function validChoiceWithFreeTextAns(currentQuestionID) {
    var oneChecked = false;
    var additionalFreeTextCheck = false;
    var trimmed = "";
    for (i = 0; i < document.forms[0].elements.length; i = i + 1) {
        var control = document.forms[0].elements[i];
        if (control != null && control.getAttribute("ePAParentQuestionID") != null) {
            var questionID = control.getAttribute("ePAParentQuestionID").replace(/^\s+|\s+$/g, '');
            if (questionID == currentQuestionID
                && control.getAttribute("ePAAnswerChoiceID") != null
                && (control.type == 'checkbox' || control.type == 'radio')
                && control.checked) {
                oneChecked = true;
                if (control.getAttribute("ePACheckAdditionalText") == 'M') {
                    var answerControlClientID = control.getAttribute("ePAAdditionalTextControlClientID");
                    var textControl = document.getElementById(answerControlClientID);
                    if (textControl.value != null) {
                        var trimmed = textControl.value.replace(/^\s+|\s+$/g, '');
                        if (trimmed == "") {
                            additionalFreeTextCheck = false;
                            break;
                        }
                        else {
                            additionalFreeTextCheck = true;
                        }
                    }
                }
                else {
                    additionalFreeTextCheck = true;
                }
            }
        }
    }
    return oneChecked & additionalFreeTextCheck;
}

function changeQuestionControls(btnNext, currentQuestionID, nextQuestionID) {
    //cycle through all controls and current question to hide and next to show
    var divs = document.getElementsByTagName('div');
    var hideDiv = null;
    var showDiv = null;
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control.getAttribute("ePAQuestionID") != null) {
            var questionID = control.getAttribute("ePAQuestionID");
            if (questionID == currentQuestionID) {
                //control.style.display = "none";
                hideDiv = control.getAttribute("id");
            }
            else if (questionID == nextQuestionID) {
                if (control.getAttribute("ePADefaultNextQuestionID") != null) {
                    var defaultNextQuestion = control.getAttribute("ePADefaultNextQuestionID");
                    var questionType = control.getAttribute("ePACurrentQuestionType");
                    var trimmedN = defaultNextQuestion.replace(/^\s+|\s+$/g, '');
                    if (trimmedN != "") {
                        btnNext.removeAttribute("ePANextQuestionID");
                        btnNext.setAttribute("ePANextQuestionID", defaultNextQuestion);
                    }
                }

                btnNext.disabled = true;
                btnNext.removeAttribute("ePACurrentQuestionID");
                btnNext.removeAttribute("ePACurrentQuestionID");

                btnNext.setAttribute("ePACurrentQuestionID", nextQuestionID);
                btnNext.setAttribute("ePACurrentQuestionType", questionType);
                showDiv = control.getAttribute("id");
            }
        }
    }

    transitionControls(hideDiv, showDiv, 0, 2);
}

function transitionControls(nameObjFrom, nameObjTo, curStep, speed) {
    var fromObj = document.getElementById(nameObjFrom);
    var toObj = document.getElementById(nameObjTo);

    if (curStep == 100) {
        fromObj.style.display = "none";
        toObj.style.display = "inline";
        toObj.style.opacity = 0;
        toObj.style.filter = 'alpha(opacity=' + 0 + ')';
    }
    else if (curStep > 100) {
        toObj.style.opacity = (curStep - 100) / 100;
        toObj.style.filter = 'alpha(opacity=' + (curStep - 100) + ')';
    }
    else if (curStep < 100) {
        toObj.style.opacity = (100 - curStep) / 100;
        toObj.style.filter = 'alpha(opacity=' + (100 - curStep) + ')';
    }

    curStep = curStep + speed;
    if (curStep <= 200) {
        setTimeout("transition('" + nameObjFrom + "','" + nameObjTo + "'," + curStep.toString() + "," + speed.toString() + ")", speed)
    }
}

function limitFreeTextQSMaxLength(comments, counter, event) {
    var ret = limitMaxLength(comments, counter, event);
    var txtArea = document.getElementById(comments);
    if (txtArea.value.length > 0) {
        enableInitQSListNextBtn(null);
    }
    else {
        disableInitQSListNextBtn(false);
    }
    return ret;
}

function limitMaxLength(comments, counter, event) {
    var enteredKeyCode;
    var isPermittedKeystroke;
    var txtArea = document.getElementById(comments);
    var count = document.getElementById(counter);
    enteredKeyCode = window.event ? event.keyCode : event.which;

    // Allow non-printing, arrow and delete keys
    isPermittedKeystroke = ((enteredKeyCode < 32) // Non printing
        || (enteredKeyCode >= 33 && enteredKeyCode <= 40)    // Page Up, Down, Home, End, Arrow
        || (enteredKeyCode == 46))                            // Delete

    if (txtArea.value.length <= 2000) {
        count.innerHTML = "[" + txtArea.value.length + " of 2000]";
        return true;
    }
    else {
        if (isPermittedKeystroke) {
            return true;
        }
        else {
            return false;
        }
    }
}

function limitFreeTextMaxLengthPaste(comments, counter, event) {
    var ret = limitMaxLengthPaste(comments, counter, event);
    var txtArea = document.getElementById(comments);
    if (txtArea.value.length > 0) {
        enableInitQSListNextBtn(null);
    }
    else {
        disableInitQSListNextBtn(false);
    }
    return ret;
}

function limitMaxLengthPaste(comments, counter, event) {
    var txtArea = document.getElementById(comments);
    var count = document.getElementById(counter);
    var selectionLength = 0;
    var clipboardText;
    var resultantLength;
    var currentFieldLength;

    currentFieldLength = txtArea.value.length;

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
    } else if (event.clipboardData && event.clipboardData.getData) {
        clipboardText = event.clipboardData.getData('text/plain');
    }

    // calculate the resulting lenth of text of the paste operation
    resultantLength = currentFieldLength + clipboardText.length - selectionLength;

    if (resultantLength > 2000) {
        alert("Total number of characters exceeding 2000 char limit.");
        return false;
    }
    else {
        count.innerHTML = "[" + resultantLength + " of 2000]";
        return true;
    }
}

function limitKeyForDecimalValue(obj, event) {
    var charCode = window.event ? event.keyCode : event.which;

    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) {
        return false;
    }
    else {
        var selectionStart = -1;
        var selectionEnd = -1;
        if (window.getSelection) {  // all browsers, except IE before version 9
            if (document.activeElement &&
                document.activeElement.tagName.toLowerCase() == "input") {
                var text = document.activeElement.value;
                selectionStart = document.activeElement.selectionStart;
                selectionEnd = document.activeElement.selectionEnd;
            }
            else {
                var selRange = window.getSelection();
                selectionStart = selRange.startOffset;
                selectionEnd = selRange.endOffset;
            }
        }
        else {
            if (document.selection.createRange) { // Internet Explorer
                var range = document.selection.createRange();
                var selLen = range.text.length;
                range.moveStart('character', -obj.value.length);

                selectionStart = range.text.length - selLen;
                selectionEnd = selectionStart + selLen;
            }
        }

        if ((charCode >= 48 && charCode <= 57) || (charCode == 46)) {
            var resultantString = obj.value.substring(0, selectionStart) + String.fromCharCode(charCode) + obj.value.substring(selectionEnd);

            if (!isNumericValue(resultantString)) {
                return false;
            }
        }
    }
}

function validateDecimalValuePaste(obj, event) {
    var selectionLength = 0;
    var clipboardText;
    var resultantLength;
    var currentFieldLength = -1;
    var selectionStart = -1;
    var selectionEnd = -1;

    currentFieldLength = obj.value.length;

    // get the length of any text currently selected in the target field text box
    if (window.getSelection) {  // all browsers, except IE before version 9
        if (document.activeElement &&
            document.activeElement.tagName.toLowerCase() == "input") {
            var text = document.activeElement.value;
            selectionLength = document.activeElement.selectionEnd - document.activeElement.selectionStart;
            selectionStart = document.activeElement.selectionStart;
            selectionEnd = document.activeElement.selectionEnd;
        }
        else {
            var selRange = window.getSelection();
            selectionLength = selRange.toString().length;
            selectionStart = selRange.startOffset;
            selectionEnd = selRange.endOffset;
        }
    }
    else {
        if (document.selection.createRange) { // Internet Explorer
            var range = document.selection.createRange();
            selectionLength = range.text.length;
            range.moveStart('character', -currentFieldLength);

            selectionStart = range.text.length - selectionLength;
            selectionEnd = selectionStart + selectionLength;
        }
    }

    // get the length of the text to be pasted
    if (window.clipboardData && window.clipboardData.getData) { // IE
        clipboardText = window.clipboardData.getData("Text");
    } else if (event.clipboardData && event.clipboardData.getData) {
        clipboardText = event.clipboardData.getData('text/plain');
    }

    // resulting text
    resultantLength = currentFieldLength + clipboardText.length - selectionLength;
    var resultantString = obj.value.substring(0, selectionStart) + clipboardText + obj.value.substring(selectionEnd);

    if (!isNumericValue(resultantString)) {
        alert("Invalid Paste");
        return false;
    }
    else {
        return true;
    }
}

function minimizeTextAreaControl(comments) {
    var txtArea = document.getElementById(comments);
    txtArea.rows = 2;
}

function maximizeTextAreaControl(comments) {
    var txtArea = document.getElementById(comments);
    txtArea.rows = 6;
}

function OnDateValuePickedFromPopUp(sender, e) {
    if (sender.get_selectedDate() != null) {
        enableInitQSListNextBtn(null);
    }
    else {
        disableInitQSListNextBtn(false);
    }
}

function OnDateValueChanged(dateControlCliendId) {
    var datepicker = $find(dateControlCliendId);
    var textbox = datepicker.get_textBox();
    if (textbox.value.length > 0) {
        enableInitQSListNextBtn(null);
    }
    else {
        disableInitQSListNextBtn(false);
    }
}