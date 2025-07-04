<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.Controls_CSMedRefillRequestNotAllowed" Codebehind="CSMedRefillRequestNotAllowed.ascx.cs" %>

<script language = "javascript" type = "text/javascript">
    function CallCancel() {
        alert("button click called");
        document.getElementById('<%=btnCancelCS.ClientID%>').click()
        }

</script>
<asp:Panel Style="display: none" ID="pnlCSWarning" runat="server">
    <div class="overlaymain">
        <div class="overlayTitle">
            Notice
        </div>
        <div class="overlayContent">
             You are attempting to respond to a controlled substance request. By law, this is
                not permitted.
                <br />
                <br />
                <asp:Literal ID="ltrYes" runat="server" Text = "Click <b>“Yes”</b> below to print the prescription for faxing back to the pharmacy.If you choose this selection, a prescription will be printed with the requesting pharmacy name and fax number. You must sign the prescription and manually fax it to the pharmacy requesting the renewal. A “Deny with new prescription to follow”will be electronically sent to the pharmacy."></asp:Literal>
                <br />
                <br />
                <asp:Literal ID="ltrContactMe" runat="server" Text = " Click <b>“Contact Me”</b> below to deny the renewal request and inform the pharmacy to contact your office by an alternate method (phone call or fax) to obtain approval. A “Deny” will be sent to the pharmacy and in the comments field will contain the following “Contact provider by alternate methods regarding controlled medications”."></asp:Literal>
                <br />
                <br />
        </div>
        <div class="overlayFooter" align="center">
            <asp:Button ID="btnPrint" runat="server" Text="Yes" OnClick="btnPrint_Click" Width="125px"
                    CssClass="btnstyle" />
                <asp:Button ID="btnContactMe" runat="server" Text="Contact Me" OnClick="btnContactMe_Click"
                    Width="125px" CssClass="btnstyle" />
                <asp:Button ID="btnCancelCS" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                    Width="125px" CssClass="btnstyle"  />
        </div>
    </div>
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="modalCSWarningPopup" runat="server" TargetControlID="btnHiddenTrigger"
    PopupControlID="pnlCSWarning" BackgroundCssClass="modalBackground" DropShadow="false">
</ajaxToolkit:ModalPopupExtender>
<asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />