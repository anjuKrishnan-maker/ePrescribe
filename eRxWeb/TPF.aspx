<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.TPF" Title="Untitled Page" Codebehind="TPF.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br /><br /><br />State:
    <telerik:RadComboBox ID="cmbState" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
        Style="vertical-align: middle" AllowCustomText="false" ShowToggleImage="True"
        MarkFirstMatch="True" Width="50px" Height="300px">
        <CollapseAnimation Duration="200" Type="OutQuint" />
    </telerik:RadComboBox>
    <br />Script Count:
    <telerik:RadComboBox ID="cmbCount" runat="server" TabIndex="1" Skin="Allscripts"
        EnableEmbeddedSkins="False"  Style="vertical-align: middle" Sort="Descending"
        Width="139px">
        <Items>
            <telerik:RadComboBoxItem Text="1" Value="1" Selected="True" />
            <telerik:RadComboBoxItem Text="2" Value="2" />
            <telerik:RadComboBoxItem Text="3" Value="3" />
            <telerik:RadComboBoxItem Text="4" Value="4" />
            <telerik:RadComboBoxItem Text="5" Value="5" />
            <telerik:RadComboBoxItem Text="6" Value="6" />
            <telerik:RadComboBoxItem Text="7" Value="7" />
            <telerik:RadComboBoxItem Text="8" Value="8" />
            <telerik:RadComboBoxItem Text="9" Value="9" />
            <telerik:RadComboBoxItem Text="10" Value="10" />
        </Items>
        <CollapseAnimation Duration="100" Type="OutQuint" />
    </telerik:RadComboBox>    
    <br />Style:
    <asp:RadioButtonList ID="rdoPrintingOption" runat="server" RepeatDirection="Horizontal">
        <asp:ListItem Text="1Up" Value="1Up" Selected="True"></asp:ListItem>
        <asp:ListItem Text="4Up" Value="4Up"></asp:ListItem>
        <asp:ListItem Text="4Row" Value="4Row"></asp:ListItem>
    </asp:RadioButtonList>
    <br />System:
    <asp:RadioButtonList ID="rdoSystem" runat="server" RepeatDirection="Horizontal">
        <asp:ListItem Text="Default" Value="D" Selected="True"></asp:ListItem>
        <asp:ListItem Text="Print Server" Value="P"></asp:ListItem>
    </asp:RadioButtonList>
    <br />Page:
    <asp:RadioButtonList ID="rdoPage" runat="server" RepeatDirection="Horizontal">
        <asp:ListItem Text="PrintScript" Value="P" Selected="True"></asp:ListItem>
        <asp:ListItem Text="MultiView" Value="M"></asp:ListItem>
    </asp:RadioButtonList>
    <br />    
    <asp:Button ID="btnGo" runat="server" Text="Go" OnClick="btnGo_Click" CssClass="btnStyle"/>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

