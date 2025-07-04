<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.ProviderToPOBReport" Title="Untitled Page" Codebehind="ProviderToPOBReport.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<asp:Panel ID= "pnlReport" runat="server" Wrap="false" DefaultButton="btnReport">
    <div class="h1title indnt">
        <asp:Label ID="lblReportTitle" runat="server" Text="Provider to POB Association Report"
            CssClass="Phead"></asp:Label>
    </div>
    <div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="21px" />
        <fieldset style="border-style: none;">
            <legend></legend>
            <div style="float: left;">
                <p style="margin: 5px;">
                    <asp:Label ID="lblPOB" runat="server" AssociatedControlID="ddlPOB" Text="POB" Style="float: left;
                        width: 70px; margin-right: 5px; text-align: right; font-weight: bold;"></asp:Label>
                    <asp:DropDownList ID="ddlPOB" runat="server" Width="170px" TabIndex="1">
                    </asp:DropDownList>
                </p>
                <p style="margin: 5px;">
                    <asp:Label ID="lblPOBType" runat="server" AssociatedControlID="ddlPOBType" Text="POB Type"
                        Style="float: left; width: 70px; margin-right: 5px; text-align: right; font-weight: bold;"></asp:Label>
                    <asp:DropDownList ID="ddlPOBType" runat="server" Width="170px" TabIndex="2">
                        <asp:ListItem Text="All POB Types" Value=""></asp:ListItem>
                        <asp:ListItem Text="POB - No Review Required" Value="1002"></asp:ListItem>
                        <asp:ListItem Text="POB - Some Review Required" Value="998"></asp:ListItem>
                        <asp:ListItem Text="POB - All Review Required" Value="999"></asp:ListItem>
                    </asp:DropDownList>
                </p>
                <p style="margin: 5px;">
                    <asp:Label ID="lblProvider" runat="server" AssociatedControlID="ddlProvider" Text="Provider"
                        Style="float: left; width: 70px; margin-right: 5px; text-align: right; font-weight: bold;"></asp:Label>
                    <asp:DropDownList ID="ddlProvider" runat="server" Width="170px" TabIndex="3">
                    </asp:DropDownList>
                </p>
            </div>
            <div style="clear: both">
            </div>
        </fieldset>
    </div>
    <div class="h3title" style="height:auto;">
        <asp:Button ID="btnClose" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnClose_Click"
            CausesValidation="false" TabIndex="5" />
        <asp:Button ID="btnReport" CssClass="btnstyle" runat="server" Text="Display Report"
            OnClick="btnReport_Click" ToolTip="Click here to see the report ." TabIndex="4" />
    </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
