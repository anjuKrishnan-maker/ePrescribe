<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_ePAInitiationQuestionsAnswerReview" Codebehind="ePAInitiationQuestionsAnswerReview.ascx.cs" %>
<script type="text/javascript">

    function OnInitQSReviewSubmit() {
        var btnSubmitDisp = document.getElementById("<%=btnSubmitDisp.ClientID %>");
        if (btnSubmitDisp != null) {
            btnSubmitDisp.value = "Processing...";
            btnSubmitDisp.disabled = "true";
            btnSubmitDisp.style.cursor = "wait";

            window.location = "ePAInitQAReviewRedirect.aspx?Action=SUBMIT";  
        }
    }

    function OnInitQSReviewCancel() {
        var btnCancelDisp = document.getElementById("<%=btnCancelDisp.ClientID %>");
        if (btnCancelDisp != null) {
            btnCancelDisp.value = "Processing...";
            btnCancelDisp.disabled = "true";
            btnCancelDisp.style.cursor = "wait";

            window.location = "ePAInitQAReviewRedirect.aspx?Action=CANCEL";
        }
    }

    function OnInitQSReviewStartOver() {
        var btnStartOverDisp = document.getElementById("<%=btnStartOverDisp.ClientID %>");
        if (btnStartOverDisp != null && confirm('Are you sure you want to start over from the beginning of the question set?\n\nPlease note that all entered answers and attachments will be removed if you start  over.')) {
            btnStartOverDisp.value = "Processing...";
            btnStartOverDisp.disabled = "true";
            btnStartOverDisp.style.cursor = "wait";

            window.location = "ePAInitQAReviewRedirect.aspx?Action=START_OVER";
        }
    }

</script>
<div class="ePAQuestionList" id="questionReviewHolder" runat="server">
    <table width="100%" class="ePAQuestionList">
        <tr>
            <td colspan="2">
                <div class="ePAQuestionTitleNcpdp" id="questionTitle" runat="server">
                    Review your answers:
                </div>
                <br />
                <div id="questionReview" runat="server" class="ePAReviewTableHolderNcpdp">
                                    
                </div>
                 <br />
            </td>
        </tr>
        <tr>
            <td>
                <iframe name="fileUploader" id="fileUploader" runat="server" class="epa-file-uploader">
                </iframe>
            </td>
            <td valign="bottom" width="150px" style="vertical-align:bottom; padding-left:10px">
                <input id="btnSubmitDisp" runat="server" type="button" value="Submit" onclick="OnInitQSReviewSubmit()" class="ePAbtnStyle" /><br />
                <input id="btnStartOverDisp" runat="server" type="button" value="Start Over" onclick="OnInitQSReviewStartOver()" class="ePAbtnStyle" /><br />
                <input id="btnCancelDisp" runat="server" type="button" value="Return to Task List" onclick="OnInitQSReviewCancel()" class="ePAbtnStyle" /><br />
            </td>
        </tr>
    </table>
</div>