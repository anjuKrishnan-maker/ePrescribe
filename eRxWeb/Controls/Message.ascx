<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_Message" Codebehind="Message.ascx.cs" %>
<table width="100%" id="tbMessage" runat="server" class="success-dialog">
    <tr valign="middle">
        <td style="width: 25px">
            <asp:Image ID="imgStatus" runat="server" ImageUrl="../images/info-global-16-x-16.png" />
        </td>
        <td valign="middle"><div id="divMessage" runat="server"></div></td>
    </tr>
</table>
