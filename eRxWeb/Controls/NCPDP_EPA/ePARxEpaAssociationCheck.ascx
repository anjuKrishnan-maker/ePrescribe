<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ePARxEpaAssociationCheck.ascx.cs"
    Inherits="eRxWeb.Controls_ePARxEpaAssociationCheck" %>
<asp:Panel Style="display: none" ID="panelRxEpaAssociation" runat="server" class="overlaymain">
    <div align="center">
        <br />
        This action cannot be performed as one or more of the selected prescription(s) are
        associated with active prior authorization task.
        <br />
        <br />
    </div>
    <div align="center">
        <asp:Button ID="btnOk" runat="server" CausesValidation="true" CssClass="btnstyle"
            Text="OK" />
    </div>
</asp:Panel>
<asp:Button ID="hiddenRxEpaAssociation" runat="server" Style="display: none;" />
<ajaxToolkit:ModalPopupExtender ID="mpeRxEpaAssociation" runat="server" BehaviorID="mpeRxEpaAssociation"
    DropShadow="false" BackgroundCssClass="modalBackground" OkControlID="btnOk" CancelControlID="btnOk"
    TargetControlID="hiddenRxEpaAssociation" PopupControlID="panelRxEpaAssociation">
</ajaxToolkit:ModalPopupExtender>
