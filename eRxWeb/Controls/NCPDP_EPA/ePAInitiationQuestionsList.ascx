<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_ePAInitiationQuestionsList" Codebehind="ePAInitiationQuestionsList.ascx.cs" %>
<script type="text/javascript">
    
    function OnInitQSListSave() {
        var btnNext = document.getElementById("<%=btnNext.ClientID %>");
        var btnSaveFinishLater = document.getElementById("<%=btnSaveFinishLater.ClientID %>");
        if (btnNext != null && btnSaveFinishLater != null) {
            currentQuestionID = btnNext.getAttribute("ePACurrentQuestionID").replace(/^\s+|\s+$/g, '');
            currentQuestionType = btnNext.getAttribute("ePACurrentQuestionType").replace(/^\s+|\s+$/g, '');

            if (currentQuestionType == "CO" || currentQuestionType == "CA") {
                if (!validChoiceWithFreeTextAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }
            else if (currentQuestionType == "FT") {
                if (!validFreeTextAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }
            else if (currentQuestionType == "NU") {
                if (!validNumberAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }
            else if (currentQuestionType == "DT") {
                if (!validDateAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }

            btnSaveFinishLater.value = "Processing...";
            btnSaveFinishLater.disabled = "true";
            btnSaveFinishLater.style.cursor = "wait";
            pullInitQSListAnswers('S', '');
        }
    }

    function OnInitQSListCancel() {
        var btnCancel = document.getElementById("<%=btnCancelDisp.ClientID %>");
        var btnNext = document.getElementById("<%=btnNext.ClientID %>");
        if (btnCancel != null && btnNext != null) {
            var skipQuestion = btnNext.getAttribute("ePACurrentQuestionID");
            btnCancel.value = "Processing...";
            btnCancel.disabled = "true";
            btnCancel.style.cursor = "wait";
            pullInitQSListAnswers('C', skipQuestion);
        }
    }

    function OnInitQSListStartOver() {
        var btnStartOverDisp = document.getElementById("<%=btnStartOverDisp.ClientID %>");
        var btnStartOver = document.getElementById("<%=btnStartOver.ClientID %>");
        if (btnStartOverDisp != null && btnStartOver != null && confirm("Are you sure you want to start over from the beginning of the question set?\n\nPlease note that all entered answers and attachments will be removed if you start  over.")) {
            btnStartOverDisp.value = "Processing...";
            btnStartOverDisp.disabled = "true";
            btnStartOverDisp.style.cursor = "wait";

            btnStartOver.click();
        }
    }

    function OnInitQSListNext() {
        var btnNext = document.getElementById("<%=btnNext.ClientID %>");
        var btnSaveFinishLater = document.getElementById("<%=btnSaveFinishLater.ClientID %>");
        var btnStartOver = document.getElementById("<%=btnStartOverDisp.ClientID %>");
        if (btnNext != null && btnSaveFinishLater != null && btnStartOver != null) {
            nextQuestionID = btnNext.getAttribute("ePANextQuestionID").replace(/^\s+|\s+$/g, '');
            currentQuestionID = btnNext.getAttribute("ePACurrentQuestionID").replace(/^\s+|\s+$/g, '');
            currentQuestionType = btnNext.getAttribute("ePACurrentQuestionType").replace(/^\s+|\s+$/g, '');

            if (currentQuestionType == "CO" || currentQuestionType == "CA") {
                if (!validChoiceWithFreeTextAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }
            else if (currentQuestionType == "FT") {
                if (!validFreeTextAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }
            else if (currentQuestionType == "NU") {
                if (!validNumberAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }
            else if (currentQuestionType == "DT") {
                if (!validDateAns(currentQuestionID)) {
                    alert("Invalid entry, please correct before continuing.");
                    return;
                }
            }

            btnSaveFinishLater.removeAttribute("ePAQuestionAnswered");
            btnSaveFinishLater.setAttribute("ePAQuestionAnswered", "YES");

            if (nextQuestionID.replace(/^\s+|\s+$/g, '').toUpperCase() == "END") {
                btnNext.value = "Processing...";
                btnNext.disabled = "true";
                btnNext.style.cursor = "wait";
                pullInitQSListAnswers('R', '');
                return;
            }

            btnStartOver.disabled = false;
            changeQuestionControls(btnNext, currentQuestionID, nextQuestionID);
        }
    }

    function checkOptionAnswers(optionQuestionId) {
        var nextQuestionID = "";
        var questionType = "";
        var oneChecked = false;

        for (i = 0; i < document.forms[0].elements.length; i = i + 1) {
            var control = document.forms[0].elements[i];
            if (control != null && control.getAttribute("ePAAnswerChoiceID") != null
                && control.getAttribute("ePAParentQuestionID") != null && control.getAttribute("ePAParentQuestionID") == optionQuestionId
                && (control.type == 'checkbox' || control.type == 'radio')) {

                var freeTextClientID = control.getAttribute("ePAAdditionalTextControlClientID");
                var freeTextControl = document.getElementById(freeTextClientID);
                var freeTextContainerControlClientID = control.getAttribute("ePAAdditionalTextDivControlClientID");
                var freeTextContainerControl = document.getElementById(freeTextContainerControlClientID);
               

                if (control.checked) {
                    if (control.getAttribute("ePACheckAdditionalText") != 'NA') {
                        freeTextContainerControl.style.display = "inline";
                    }
                    else {
                        clearOptionsAdditionalTextControl(freeTextControl, freeTextContainerControl);
                    }
                    oneChecked = true;
                    nextQuestionID = control.getAttribute("ePANextQuestionID");
                }
                else {
                    clearOptionsAdditionalTextControl(freeTextControl, freeTextContainerControl);
                }
            }
        }
     
        if (oneChecked) {
            enableInitQSListNextBtn(nextQuestionID);
        }
        else {
            disableInitQSListNextBtn(true);
        }
    }

    function clearOptionsAdditionalTextControl(freeTextControl, freeTextContainerControl){
        var freeAddlTextCountControlClientID = freeTextContainerControl.getAttribute("ePAAddlTextCountControlClientID");
        var freeAddlTextCountControl = document.getElementById(freeAddlTextCountControlClientID);
        freeAddlTextCountControl.innerHTML = "[0 of 2000]";
        freeTextControl.value = '';
        freeTextContainerControl.style.display = "none";
    }

    function enableInitQSListNextBtn(nextQuestionID) {
        var btnNext = document.getElementById("<%=btnNext.ClientID %>");
        var btnSaveFinishLater = document.getElementById("<%=btnSaveFinishLater.ClientID %>");
        if (btnNext != null && btnSaveFinishLater != null) {
            btnNext.disabled = false;
            if (nextQuestionID != null) {
                btnNext.removeAttribute("ePANextQuestionID");
                btnNext.setAttribute("ePANextQuestionID", nextQuestionID);
            }

            if (btnSaveFinishLater.getAttribute("ePAQuestionAnswered") != "YES") {
                btnSaveFinishLater.disabled = false;
            }
        }
    }
    
    function disableInitQSListNextBtn(isRemoveAttribute) {
        var btnNext = document.getElementById("<%=btnNext.ClientID %>");
        var btnSaveFinishLater = document.getElementById("<%=btnSaveFinishLater.ClientID %>");
        if (btnNext != null && btnSaveFinishLater != null) {
            btnNext.disabled = true;

            if (btnSaveFinishLater.getAttribute("ePAQuestionAnswered") != "YES") {
                btnSaveFinishLater.disabled = true;
            }

            if (isRemoveAttribute != null && isRemoveAttribute) {
                btnNext.removeAttribute("ePANextQuestionID");
            }
        }
    }

    function pullInitQSListAnswers(nextButton, skipQuestion) {
        var xmlString = pullInitQSAnswerJSON(skipQuestion);
        var hiddenFormAnswerList = document.getElementById("<%=hiddenFormAnswerList.ClientID %>");
        if (nextButton == 'R') {
            var btnReview = document.getElementById("<%=btnReview.ClientID %>");
            if (btnReview != null && hiddenFormAnswerList != null) {
                hiddenFormAnswerList.value = xmlString;
                btnReview.click();
            }
        }
        else if (nextButton == 'S') {
            var btnSave = document.getElementById("<%=btnSave.ClientID %>");
            if (btnSave != null && hiddenFormAnswerList != null) {
                hiddenFormAnswerList.value = xmlString;
                btnSave.click();
            }
        }
        else if (nextButton == 'C') {
            var btnCancel = document.getElementById("<%=btnCancel.ClientID %>");
            if (btnCancel != null && hiddenFormAnswerList != null) {
                hiddenFormAnswerList.value = xmlString;
                btnCancel.click();
            }
        }
    }
    
   
</script>
<div class="ePAQuestionList" id="questionListHolder" runat="server">
    <table width="100%" class="ePAQuestionList">
        <tr>
            <td>
                <div id="questionList" runat="server" class="question-list">
                
                
                </div>
            </td>
            <td valign="bottom" width="200px" style="vertical-align:bottom;">
                <input id="btnNext" runat="server" type="button" value="Next" onclick="OnInitQSListNext()" class="ePAbtnStyle btnstyle" />
                <asp:Button ID="btnReview" runat="server" Text="Review" CssClass="ePAbtnStyle" onclick="btnReview_Click" style="display:none;" CausesValidation="false"/>
                <asp:HiddenField ID="hiddenFormAnswerList" runat="server" /><br />
                <input id="btnCancelDisp" runat="server" type="button" value="Cancel" onclick="OnInitQSListCancel()" class="ePAbtnStyle btnstyle" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ePAbtnStyle" onclick="btnCancel_Click" CausesValidation="false" style="display:none;"/><br />
                <input id="btnStartOverDisp" runat="server" type="button" value="Start Over" onclick="OnInitQSListStartOver()" class="ePAbtnStyle btnstyle" />
                <asp:Button ID="btnStartOver" runat="server" Text="Start Over" CssClass="ePAbtnStyle btnstyle" onclick="btnStartOver_Click" style="display:none;" CausesValidation="false"/><br />
                <input id="btnSaveFinishLater" runat="server" type="button" value="Save & Finish Later" onclick="OnInitQSListSave()" class="btnstyle" />
                <asp:Button ID="btnSave" runat="server" Text="Review" CssClass="ePAbtnStyle" onclick="btnSaveFinishLater_Click" style="display:none;" CausesValidation="false"/>
            </td>
        </tr>
    </table>
</div>
