<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_StateLicenseDetails" Codebehind="StateLicenseDetails.ascx.cs" %>
<%@ Import Namespace="eRxWeb.AppCode" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<table id="Table1" cellspacing="2" cellpadding="1" width="100%" border="1" rules="none" style="BORDER-COLLAPSE: collapse">
    <tr>
        <td style="text-align:right">State:</td>
        <td><telerik:RadComboBox ID="ddlState" runat="server" DataSourceID="odsStates" SelectedValue='<%# CheckState(DataBinder.Eval( Container, "DataItem.State")) %>' DataValueField="State" DataTextField="State" Width="75px" Height="150px"></telerik:RadComboBox></td>    
        </tr>
    <tr>
        <td style="text-align:right">License Number:</td>
        <td><telerik:RadTextBox ID="txtLicNumber" runat="server" Text='<%# DataBinder.Eval( Container, "DataItem.LicenseNumber") %>'></telerik:RadTextBox></td>
    </tr>   
    <tr>
        <td style="text-align:right">License Type:</td>
        <td><telerik:RadComboBox ID="ddLicenseTyp" runat="server" DataSourceID="odsProviderLicenseTypes" SelectedValue='<%# CheckLicenseType(DataBinder.Eval( Container, "DataItem.TypeDescription")) %>' DataValueField="TypeDescription" DataTextField="TypeDescription" Width="160px" Height="150px"></telerik:RadComboBox></td>    
    </tr>
    <tr>
        <td style="text-align:right">Expiration Date:</td>
        <td>
            <telerik:RadDatePicker ID="dateTimeExpDate" runat="server" SelectedDate='<%# DateTimeHelper.GetValidFutureDateOrToday(DataBinder.Eval( Container, "DataItem.ExpirationDate")) %>'>
            </telerik:RadDatePicker>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="right">
            <asp:Button ID="btnUpdate" Text="Update" CssClass="btnstyle" runat="server" CommandName="Update" Visible='<%# !(DataItem is Telerik.Web.UI.GridInsertionObject) %>'></asp:Button>
            <asp:Button ID="btnInsert" Text="Insert" CssClass="btnstyle" runat="server" CommandName="PerformInsert" Visible='<%# DataItem is Telerik.Web.UI.GridInsertionObject %>'></asp:Button>
            &nbsp;
            <asp:Button ID="btnCancel" Text="Cancel" CssClass="btnstyle" runat="server" CausesValidation="False" CommandName="Cancel"></asp:Button>
        </td>
    </tr>
    <asp:ObjectDataSource runat="server" ID="odsStates" SelectMethod="ChGetState" TypeName="Allscripts.Impact.RxUser">
        <SelectParameters>
            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource runat="server" ID="odsProviderLicenseTypes" SelectMethod="GetProviderLicenseTypes" TypeName="Allscripts.Impact.RxUser">
        <SelectParameters>
            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</table>