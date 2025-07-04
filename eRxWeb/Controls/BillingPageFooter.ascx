<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.Controls_BillingPageFooter" Codebehind="BillingPageFooter.ascx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<table cellpadding="0" cellspacing="0" width="100%" >
    <tr align="left" valign="top" >
        <td align="left" style="font-size: small; color: #858585; border-top: solid 1px #858585;
            width: 385%">
            <a target="_blank" href="Help/Default.aspx" style="font-size: small; color: #858585;">
                Help</a> | <a target="_blank" style="font-size: small; color: #858585;" href="/Help/Documents/ePrescribePrivacyPolicy.pdf">
                    Privacy Policy</a> | <a target="_blank" style="font-size: small; color: #858585;
                        text-decoration: underline; cursor: pointer;" onclick="throwTC()">Refund Policy</a>
            | <%=Copyright.ShortText %>
        </td>
    </tr>
    <tr>
        <td align="right" valign="top">
            <asp:Image ID="imgAuth" runat="server" ImageUrl="~/images/Authorize.GIF" ImageAlign="Top" />
            <asp:Image ID="imANV" runat="server" ImageUrl="~/images/Authorize_verified.GIF" />
        </td>
    </tr>
</table>
