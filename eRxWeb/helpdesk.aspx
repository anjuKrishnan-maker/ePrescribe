<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.helpdesk" Codebehind="helpdesk.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
Click here to continue without HelpDesk access:
<asp:Button ID="btnRegular" runat="server" Text="Continue" OnClick="btnRegular_Click" />
<br /><br />
The following sites have enabled Remote Admin Access.  Please select the site you wish to access:<br />
    <br />
    <table border="0" cellpadding="0" cellspacing="0">
		<tr valign="top">
			<td>    
				<asp:ListBox ID="lstSites" runat="server" DataSourceID="AdminODS" DataTextField="SiteNameAndAccountID" DataValueField="LicenseIDWithSiteID" Rows="25"></asp:ListBox>
			</td>
			<td>
				<asp:Button ID="btnSelect" runat="server" Text="Select" OnClick="btnSelect_Click" />
			</td>
		</tr>
    </table>
    <asp:ObjectDataSource ID="AdminODS" runat="server" SelectMethod="GetAllBackdoorSites" TypeName="eRxWeb.EPSBroker">
        <SelectParameters>
            <asp:SessionParameter Name="licenseID" SessionField="LicenseID" Type="String" />
            <asp:SessionParameter Name="userID" SessionField="UserID" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>