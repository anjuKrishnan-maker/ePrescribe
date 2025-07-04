<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_ePAInitiationResponse" Codebehind="ePAInitiationResponse.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="ePAInitiationQuestionsList.ascx" TagName="ePAInitiationQuestionsList" TagPrefix="ePrescribe" %>
<%@ Register Src="ePAInitiationQuestionsAnswerReview.ascx" TagName="ePAInitiationQuestionsAnswerReview" TagPrefix="ePrescribe" %>
<asp:Panel style="DISPLAY: none" id="panelEPA" runat="server" CssClass="epa-panel-overlay">    
    <div class="ePAQuestionResponseWindowNcpdp overlaymainwide">
        <telerik:RadAjaxManagerProxy ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="ePAInitiationQuestionList">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="ePAInitiationQuestionAnswerReview" />
                        <telerik:AjaxUpdatedControl ControlID="ePAInitiationQuestionList" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="ePAInitiationQuestionAnswerReview">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="ePAInitiationQuestionAnswerReview" />
                        <telerik:AjaxUpdatedControl ControlID="ePAInitiationQuestionList" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManagerProxy>
        <div class="ePAQuestionResponseHeader"> 
            <table width="100%">
                <tr>
                    <td>
                        <span id="ePAQuestionResponseTitle" runat="server" class="ePAQuestionResponseTitle">
                            Title
                        </span>                
                    </td>
                    <td width="200px">
                        Expiration Date:&nbsp
                        <span id="ePAExpirationDate" runat="server">
                            12/31/1999
                        </span>                    
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        Patient Name:&nbsp
                        <span id="ePAPatientName" runat="server">
                            Anand Kumar
                        </span>
                    </td>
                    <td width="200px">
                        <div id="contactSection" runat="server">
                            Contact:&nbsp
                            <span id="ePAContactNumber" runat="server">

                            </span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span id="ePAQuestionResponseDescription" runat="server">
                            Question Description
                        </span>
                    </td>
                </tr>
            </table>
        </div>
        <div ID="ePA_Message" runat="server" style="display:none" class="ePAQuestionResponseHeader">
            <br /><br />
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td width="80%" style="border:solid 1px #858585; margin:10px; padding:10px; ">
                        <div id="submittedMessage" runat="server" style="display:none">
                            Your ePA form has been sent.<br /><br />
                            Please check back to see the outcome of the submitted for before processing the prescription.<br /><br />
                            Please press "Close" to close this window.
                        </div>
                        <div id="deniedMessage" runat="server" style="display:none">
                            We're sorry, your ePA form has been denied.<br /><br />
                            Please call&nbsp<span id="deniedPhoneNumber" runat="server"></span>&nbsp for more information.
                        </div>
                        <div id="failedMessage" runat="server" style="display:none">
                            An error has occured with your ePA submission.<br /><br />
                            Please call&nbsp<span id="failedPhoneNumber" runat="server"></span>&nbsp for more information.
                        </div>
                        <div id="closedCancelledMessage" runat="server" style="display:none">
                            Thank you for submitting. Your ePA request has been closed.<br /><br />
                            Please call&nbsp<span id="closedPhoneNumber" runat="server"></span>&nbsp for more information.
                        </div>                
                    </td>
                    <td width="15%">
                
                    </td>
                    <td>
                
                    </td>
                </tr>
                <tr>
                    <td>
                
                    </td>
                    <td width="15%">
                
                    </td>
                    <td>
                        <asp:Button ID="btnClose" runat="server" Text="Close" CausesValidation="false" CssClass="btnstyle" OnClick="btnClose_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <ePrescribe:ePAInitiationQuestionsList id="ePAInitiationQuestionList" runat="server" OnEPAUIEvent="ProcessEPAEvent_IQL"></ePrescribe:ePAInitiationQuestionsList>
        <ePrescribe:ePAInitiationQuestionsAnswerReview id="ePAInitiationQuestionAnswerReview" runat="server" OnePAUIEvent="ProcessEPAEvent_IQR"></ePrescribe:ePAInitiationQuestionsAnswerReview>
    </div>
</asp:Panel>
<asp:Button ID="hiddenEPA" runat="server" style="display:none;" />
<asp:Button ID="hiddenEPACancel" runat="server" style="display:none;" />
<ajaxToolkit:ModalPopupExtender id="mpeNCPDPEPA" runat="server" BehaviorID="mpeNCPDPEPA" DropShadow="false" BackgroundCssClass="modalBackground"  TargetControlID="hiddenEPA"  PopupControlID="panelEPA" CancelControlID="hiddenEPACancel"></ajaxToolkit:ModalPopupExtender>