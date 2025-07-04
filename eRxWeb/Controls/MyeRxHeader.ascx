<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_MyeRxHeader" Codebehind="MyeRxHeader.ascx.cs" %>
<script language="javascript" type="text/javascript">    
</script>
<table width="100%" border="0" cellspacing="0" cellpadding="0" style="background-color: Transparent;">
    <tr class="h1title">
        <td>
            <asp:RadioButton ID="rbtnMyProfile" ForeColor="White" GroupName="submenu" runat="server"
                AutoPostBack="true" Text="My Profile" OnCheckedChanged="rbtnMyProfile_CheckedChanged">
            </asp:RadioButton>&nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="rbtnMyEPCSReports" ForeColor="White" runat="server" Text="My CS Reports"
                AutoPostBack="true" OnCheckedChanged="rbtnMyEPCSReports_CheckedChanged" GroupName="submenu">
            </asp:RadioButton>
        </td>
    </tr>
</table>
