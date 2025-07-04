<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_DeluxeEPCSLogoffControl" Codebehind="DeluxeEPCSLogoffControl.ascx.cs" %>
<asp:Panel ID="pnlLogOff" runat="server" Width="500px"  Style="text-align: center; display: none;">
    <table style="width: 100%; background-color: White;" cellpadding="10px">
        <tr>
            <td>
                <span style="font-size: 14pt; text-align: center;">To use ePrescribe with your new 
                    <% 
                        if ((Session["PurchaseType"] != null) && (Session["PurchaseType"].ToString() == "EPCS"))
                            Response.Write("EPCS");
                        else if ((Session["DeluxePricingStructure"] != null) && (String.Compare(Session["DeluxePricingStructure"].ToString(), "compulsorybasic", true) == 0))
                            Response.Write("Basic");
                        else
                            Response.Write("Deluxe");
                    %>
                    privileges,
                    <br />
                    you will need to log off and log back on.</span>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="btnLogOff" runat="server" Text="OK" CssClass="btnstyle" />
            </td>
        </tr>
    </table>
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="mpeLogOff" runat="server" BehaviorID="mpeLogOff"
    DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="btnContinueLogOff"
    PopupControlID="pnlLogOff" Drag="false" PopupDragHandleControlID="pnlLogOff" />


