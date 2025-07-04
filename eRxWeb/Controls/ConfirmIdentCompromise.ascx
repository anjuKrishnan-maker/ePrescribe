<%@ Control AutoEventWireup="true" CodeBehind="ConfirmIdentCompromise.ascx.cs" Inherits="eRxWeb.Controls_ConfirmIdentCompromise" %>
<script runat="server">



</script>
<asp:Panel ID="panelConfirmPopup" Style="display: none; position: relative; width: 600px;" runat="server">
    <div id="divConfirmation" class="overlaymainwide">
        <div class="overlayTitle">Confirmation</div>
        <div id="compromise" class="overlayContent">
            <asp:Panel ID="panelCompromise" Visible="True" runat="server">
                <div id="confirmMsg">
                    <p>
                        Marking the user's identity as 'Suspended' will remove the user's ability to perform certain functions and 
                        operations in his or her client application, including, but not limited to, the ability to fill electronic 
                        prescriptions for controlled substances. 
                    </p>
                </div>
                <div>&nbsp;</div>
                <div id="confirmQuestion">
                    <p>
                        Are you sure you want to set this user's identity as 'Suspended'? 
                    </p>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelSecure" Visible="False" runat="server">
                <div id="confirmMsgSec">
                    <p>
                        Reinstating the user's identity will restore the user's ability to perform all functions and capabilities 
                        in his or her client application as specified by his or her defined roles and permissions, This may include, 
                        but not limited to, the ability to fill electronic prescriptions for controlled substances. 
                    </p>
                </div>
                <div>&nbsp;</div>
                <div id="confirmQuestionSec">
                    <p>
                        Are you sure you want to reinstate this user's identity?
                    </p>
                </div>
            </asp:Panel>
        </div>
        <div id="buttons" class="overlayFooter">
            <asp:Button ID="btnYes" runat="server" Text="Yes" CssClass="btnstyle btnStyleAction" OnClick="btnYes_OnClick" Width="104px" />
            <asp:Button ID="btnNo" runat="server" Text="No" CssClass="btnstyle btnStyleAction" OnClick="btnNo_OnClick" Width="104px" />
        </div>
    </div>
</asp:Panel>
<asp:Button ID="btnPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />
<ajaxToolkit:ModalPopupExtender ID="mpeConfirmIdentityCompromised" runat="server" TargetControlID="btnPopUpTrigger"
    DropShadow="false" PopupControlID="panelConfirmPopup" BackgroundCssClass="modalBackground">
</ajaxToolkit:ModalPopupExtender>
