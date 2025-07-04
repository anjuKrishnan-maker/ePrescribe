function IsNumeric(sText) {
    sText = sText.replace(/^\s+|\s+$/g, '');
    var ValidChars = "0123456789.";
    var IsNumber = true;
    var Char;

    if (sText == "") {
        IsNumber = false;
    }
    else {
        for (i = 0; i < sText.length && IsNumber == true; i++) {
            Char = sText.charAt(i);
            if (ValidChars.indexOf(Char) == -1) {
                IsNumber = false;
            }
        }
    }
    return IsNumber;

}

function currentQuestionAnswered(currentQuestionID) {
    var answered = false;
    var i = 0;
    for (i = 0; i < document.forms[0].elements.length; i = i + 1) {
        var control = document.forms[0].elements[i];
        if (control != null && control.getAttribute("ePAAnswerChoiceID") != null && control.getAttribute("ePAParentQuestionID") == currentQuestionID) {
            if (control.type == 'checkbox' || control.type == 'radio') {
                if (control.checked) {
                    answered = true;
                }
            }
            else if (control.type == 'textarea' || control.type == 'text') {
                if (control.value.replace(/^\s+|\s+$/g, '') != '') {
                    answered = true;
                }
            }

            if (answered) {
                break;
            }
        }
    }

    return answered;
}

function pullAnswerXML(skipQuestion) {
    var addlTextBox = null;
    var xmlString = '<root>';
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
                    || control.type == 'radio'
                    || control.type == 'text') {
                addlTextBox = document.getElementById(control.getAttribute("ePAAdditionalTextID"));
                if (addlTextBox != null) {
                    addlTextBox.value = '';
                }
            }
            break;
        }
        else if (control != null && control.getAttribute("ePAAnswerChoiceID") != null && control.getAttribute("ePAParentQuestionID") != skipQuestion) {
            xmlString += '<answer>';
            xmlString += '<answerChoiceID>' + control.getAttribute("ePAAnswerChoiceID") + '</answerChoiceID>';
            xmlString += '<parentQuestionID>' + control.getAttribute("ePAParentQuestionID") + '</parentQuestionID>';
            if (control.type == 'checkbox'
                    || control.type == 'radio') {
                xmlString += '<value>' + control.checked + '</value>';
            }
            else if (control.type == 'textarea'
                      || control.type == 'text') {
                xmlString += '<value>' + replaceEscapeChar(control.value) + '</value>';
            }

            if (control.type == 'checkbox'
                    || control.type == 'radio'
                    || control.type == 'text') {

                addlTextBox = document.getElementById(control.getAttribute("ePAAdditionalTextID"));
                if (addlTextBox != null) {
                    xmlString += '<addText>' + replaceEscapeChar(addlTextBox.value) + '</addText>';
                }
            }

            xmlString += '</answer>';
        }
        else if (control != null && control.getAttribute("ePAQuestionIDAddlTextHolder") != null && control.getAttribute("ePAQuestionIDAddlTextHolder") != skipQuestion) {
            xmlString += '<question>';
            xmlString += '<questionID>' + control.getAttribute("ePAQuestionIDAddlTextHolder") + '</questionID>';
            if (control.type == 'textarea'
                      || control.type == 'text') {
                xmlString += '<comments>' + replaceEscapeChar(control.value) + '</comments>';
            }

            xmlString += '</question>';
        }
        else if (control != null && control.getAttribute("ePAQuestionIDAddlTextHolder") != null && control.getAttribute("ePAQuestionIDAddlTextHolder") == skipQuestion) {
            if (control.type == 'textarea'
                      || control.type == 'text') {
                control.value = '';
            }
        }
    }
    //cycle through all divs to find date controls
    var divs = document.getElementsByTagName('div')
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control != null && control.getAttribute("ePADateQuestionAnswerHolderID") != null) {
            var dateContainer = $find(control.getAttribute("ePADateQuestionAnswerHolderID"))
            if (dateContainer != null) {
                if (dateContainer.get_selectedDate() != null && control.getAttribute("ePAParentQuestionID") != skipQuestion) {
                    xmlString += '<answer>';
                    xmlString += '<answerChoiceID>' + control.getAttribute("ePAAnswerChoiceID") + '</answerChoiceID>';
                    xmlString += '<parentQuestionID>' + control.getAttribute("ePAParentQuestionID") + '</parentQuestionID>';
                    xmlString += '<value>' + dateContainer.get_selectedDate().format("MM/dd/yyyy") + '</value>';
                    addlTextBox = document.getElementById(control.getAttribute("ePAAdditionalTextID"));
                    if (addlTextBox != null) {
                        xmlString += '<addText>' + replaceEscapeChar(addlTextBox.value) + '</addText>';
                    }

                    xmlString += '</answer>';
                }
                else if (dateContainer.get_selectedDate() != null && control.getAttribute("ePAParentQuestionID") == skipQuestion) {
                    addlTextBox = document.getElementById(control.getAttribute("ePAAdditionalTextID"));
                    if (addlTextBox != null) {
                        addlTextBox.value = '';
                    }
                }
            }
        }
    }
    xmlString = xmlString + '</root>';

    return xmlString;
}

function replaceEscapeChar(freetextstring) {
    var ret = "";
    ret = freetextstring;
    ret = ret.replace(/&/gi, "&amp;");
    ret = ret.replace(/"/gi, "&quot;");
    ret = ret.replace(/'/gi, "&apos;");
    ret = ret.replace(/</gi, "&lt;");
    ret = ret.replace(/>/gi, "&gt;");
    return ret;
}

function validNumber(currentQuestionID) {
    var ret = true;
    //cycle through all divs to find date controls
    var divs = document.getElementsByTagName('div')
    for (i = 0; i < divs.length; i = i + 1) {
        var control = divs[i];
        if (control != null && control.getAttribute("ePANumberQuestion") != null && control.getAttribute("ePANumberQuestion") == currentQuestionID) {
            var answerID = control.getAttribute("ePANumberAnswerID");
            ret = checkNumber(answerID, currentQuestionID);
            break;
        }
    }

    return ret;
}

function checkNumber(answerID, curQuestionID) {
    var txtNumAnswer = document.getElementById(answerID);
    while (!(txtNumAnswer.value.indexOf('.', 0) < 0 || ((txtNumAnswer.value.length - txtNumAnswer.value.indexOf('.', 0)) <= 5))) {
        txtNumAnswer.value = txtNumAnswer.value.substring(0, txtNumAnswer.value.length - 1)
    }
    if (txtNumAnswer != null) {
        var textBoxParsed = "0";
        var num = 0;
        try {
            if (IsNumeric(txtNumAnswer.value)) {
                num = +(txtNumAnswer.value);
                textBoxParsed = "1";
            }
            else {
                textBoxParsed = "0";
            }
        } catch (err) {
            textBoxParsed = "0";
        }
        if (textBoxParsed == "1") {
            var checkOneComp = "";
            var checkOneVal = 0;
            var checkTwoComp = "";
            var checkTwoVal = 0;
            var checkLog = "";
            var nextQuestionID = "";
            var answerAddlTextIND = "";
            var questionAddlTextIND = "";
            var addTextID = "";
            var numOK = "";
            for (i = 0; i < document.forms[0].elements.length; i = i + 1) {
                var control = document.forms[0].elements[i];

                if (control != null && control.getAttribute("ePANumberCheckQuestionID") != null && control.getAttribute("ePANumberCheckQuestionID") == curQuestionID) {
                    checkOneComp = control.getAttribute("ePANumberCheckOneComp");
                    checkOneVal = +(control.getAttribute("ePANumberCheckOneVal"));
                    checkTwoComp = control.getAttribute("ePANumberCheckTwoComp");
                    checkTwoVal = +(control.getAttribute("ePANumberCheckTwoVal"));
                    checkLog = control.getAttribute("ePANumberCheckLogic");
                    nextQuestionID = control.getAttribute("ePANumberCheckNextQuestion");
                    answerAddlTextIND = control.getAttribute("ePANumberAnswerAddtionalTextInd");
                    questionAddlTextIND = control.getAttribute("ePANumberQuestionAddtionalTextInd");
                    addTextID = control.getAttribute("ePANumberAddtionalTextID");
                    numOK = executeNumberLogic(checkOneComp, checkOneVal, checkTwoComp, checkTwoVal, checkLog, num);
                    if (numOK == "1") {
                            break;
                    }
                }
            }

            if (numOK == "1") {
                if (curQuestionID != null && nextQuestionID != null) {
                    ret = true;
                    enableEPA_Next(curQuestionID, nextQuestionID, answerAddlTextIND, questionAddlTextIND, addTextID);
                }
            }
            else {
                ret = false;
            }
        }
        else {
            ret = false;
        }
    }
    else {
        ret = false;
    }

    return ret;
}

function executeNumberLogic(checkOneComp, checkOneVal, checkTwoComp, checkTwoVal, checkLog, num) {
    var failedOne = "0";
    var failedTwo = "0";
    var numOK = "0";
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

        if (checkLog == "AND" && failedOne == "0" && failedTwo == "0") {
            numOK = "1";
        }
        else if (checkLog == "OR" && (failedOne == "0" || failedTwo == "0")) {
            numOK = "1";
        }
        else if (failedOne == "0" && failedTwo == "0") {
            numOK = "1";
        }
    }
    else if (failedOne == "0" && failedTwo == "0") {
        numOK = "1";
    }


    return numOK;
}

function checkEPA_FreeText(freeTextIndicator, freeTextControlID, checkDefault) {
    var trimmed = "";
    var throwError = "0"
    if (freeTextIndicator == "M") {
        var freeTextBox = document.getElementById(freeTextControlID);
        if (freeTextBox != null) {
            if (freeTextBox.value != null) {
                trimmed = freeTextBox.value.replace(/^\s+|\s+$/g, '');
                if (trimmed == "") {
                    throwError = "1"
                }
            }
            else {
                throwError = "1";
            }
        }
        else {
            throwError = "1";
        }
        
        if (throwError == "1" && checkDefault) {
            var divList = document.getElementsByTagName('div');
            for (i = 0; i < divList.length; i = i + 1) {
                var control = divList[i];
                if (control.getAttribute("ePAQuestionID") != null) {
                    var questionID = control.getAttribute("ePAQuestionID").replace(/^\s+|\s+$/g, '');
                    if (questionID == currentQuestionID) {
                        freeTextInd = control.getAttribute("ePACheckFreeTextQ");
                        freeTextID = control.getAttribute("ePAFreeTextIDQ");
                        freeTextBox = document.getElementById(freeTextID);
                        if (freeTextBox != null && freeTextInd == "M") {
                            throwError = "0";
                            if (freeTextBox.value != null) {
                                trimmed = freeTextBox.value.replace(/^\s+|\s+$/g, '');
                                if (trimmed == "") {
                                    throwError = "1"
                                }
                            }
                            else {
                                throwError = "1";
                            }
                        }
                        else {
                            throwError = "1";
                        }
                    }
                }
            }
        }
    }

    if (throwError == "1")
    {
        return false;
    }

    return true;
}

function changeQuestion(btnNext, currentQuestionID, nextQuestionID) {
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
                    var freeTextIndicator = control.getAttribute("ePACheckFreeTextQ");
                    var freeTextID = control.getAttribute("ePAFreeTextIDQ");
                    var trimmedN = defaultNextQuestion.replace(/^\s+|\s+$/g, '');
                    if (trimmedN != "") {
                        btnNext.removeAttribute("ePANextQuestionID");
                        btnNext.removeAttribute("ePACheckFreeTextQ");
                        btnNext.removeAttribute("ePAFreeTextIDQ");

                        btnNext.setAttribute("ePANextQuestionID", defaultNextQuestion);
                        btnNext.setAttribute("ePACheckFreeTextQ", freeTextIndicator);
                        btnNext.setAttribute("ePAFreeTextIDQ", freeTextID);

                        btnNext.disabled = false;
                    }
                    else {
                        btnNext.disabled = true;
                    }
                }
                else {
                    btnNext.disabled = true;
                }
                btnNext.removeAttribute("ePACurrentQuestionID");
                btnNext.setAttribute("ePACurrentQuestionID", nextQuestionID);
                showDiv = control.getAttribute("id");
            }
        }
    }

    transition(hideDiv, showDiv, 0, 2);
}

function transition(nameObjFrom, nameObjTo, curStep, speed) {
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

function resetCommentsBox(currentQuestionID, freeTextIndicator)
{
    var divList = document.getElementsByTagName('div');
    for (i = 0; i < divList.length; i = i + 1) {
        var control = divList[i];
        if (control.getAttribute("ePAAnswerIDAddlTextHolder") != null) {
            var questionID = control.getAttribute("ePAAnswerIDAddlTextHolder").replace(/^\s+|\s+$/g, '');
            if (questionID == currentQuestionID) {
                if (freeTextIndicator == "NA") {
                    control.style.display = "none";
                }
                else {
                    control.style.display = "inline";
                }                        
            }
        }
    }
}

function imposeMaxLength(comments, counter, event)
{    
    var enteredKeyCode;
    var isPermittedKeystroke;
    var txtArea = document.getElementById(comments);
    var count = document.getElementById(counter);
    enteredKeyCode = window.event ? event.keyCode : event.which;
    
    // Allow non-printing, arrow and delete keys    
    isPermittedKeystroke = ((enteredKeyCode < 32) // Non printing
                     || (enteredKeyCode >= 33 && enteredKeyCode <= 40)    // Page Up, Down, Home, End, Arrow
                     || (enteredKeyCode == 46))                            // Delete

    if (txtArea.value.length <= 250) {
        count.value = txtArea.value.length;
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

function imposeMaxLengthPaste(comments, counter, event)
{
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

    if (resultantLength > 250) {
        alert("Total number of characters exceeding 250 char limit.");
        return false;
    }
    else {
        count.value = resultantLength;
        return true;
    }
}
    