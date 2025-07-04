<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.RxDURReviewMultiSelect" Title="DUR Result" Codebehind="RxDURReviewMultiSelect.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/EPCSDigitalSigning.ascx" TagName="EPCSDigitalSigning"
    TagPrefix="ePrescribe" %>
<%@ Register Src="~/Controls/CSMedRefillRequestNotAllowed.ascx" TagName="CSMedRefillRequestNotAllowed"
    TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table border="0" width="100%" cellpadding="0" cellspacing="0">
        <tr class="h2title">
            <td class="Phead indnt">
                <asp:Label ID="lblTitle" runat="server" Text="DUR Check" Font-Bold="true" ForeColor="White"></asp:Label>
            </td>
        </tr>
        <tr class="h3title" style="padding: 3px">
            <td>
                &nbsp<asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnBack_Click"
                    ToolTip="Click here to go back to script selection." />&nbsp&nbsp
                <asp:Button ID="btnSubmit" CssClass="btnstyle" runat="server" Text="Continue" OnClick="btnSubmit_Click"
                    ToolTip="Click here to save DUR warnings and proceed." />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="headerInfo" runat="server" Style="vertical-align: middle; ">
                    <table cellpadding="1" cellspacing="5" style="background-color: White; vertical-align: middle"
                        width="100%">
                        <tr style="width: 100%">
                            <td style="width: 100%; padding: 0px 0px 0px 1px">
                                <ePrescribe:Message ID="ucMainMessage" runat="server" Visible="true" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="warningError" runat="server" Style="vertical-align: middle; ">
                    <table cellpadding="1" cellspacing="5" style="background-color: White; vertical-align: middle"
                        width="100%">
                        <tr style="width: 100%">
                            <td style="width: 100%; padding: 0px 0px 0px 1px">
                                <ePrescribe:Message ID="ucErrorMessage" runat="server" Visible="true" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>      
        <tr>
            <td>
                <asp:Panel ID="warningsPanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="Label5" runat="server" Text="DUR Errors" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="warningsPanelDetail" runat="server" Visible="false" Width="100%">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdWarningsList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdWarningsList_OnNeedDataSource" Style="width: 99%"
                                    AutoGenerateColumns="False" OnDataBound="grdWarningsList_DataBound">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top"
                                        NoMasterRecordsText="No warnings.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridBoundColumn UniqueName="Warnings" HeaderText="Warnings"
                                                DataField="Warnings">
                                                <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpeWarnings" runat="server" TargetControlID="warningsPanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="warningsPanelHeader" CollapseControlID="warningsPanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="custPanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="lblCust" runat="server" Text="Custom Medications" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="custPanelDetail" runat="server" Visible="false" Width="100%">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdCustList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdCustList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdCustList_OnNeedDataSource"
                                    Style="width: 99%" AutoGenerateColumns="False" OnDataBound="grdCustList_DataBound" OnItemCreated="grdCustList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridBoundColumn UniqueName="MedicationName" HeaderText="Medication Name"
                                                DataField="MedicationName">
                                                <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Warnings" HeaderText="Warnings" 
                                                DataField="WarningText">
                                                <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason" HeaderText="">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpeCust" runat="server" TargetControlID="custPanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="custPanelHeader" CollapseControlID="custPanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="parPanelHeader" runat="server" BackColor="LightGray" Width="100%"
                    BorderColor="black" BorderWidth="1px" Visible="false">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="lblPar" runat="server" Text="Prior Adverse Reactions" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="parPanelDetail" runat="server" Width="100%" Visible="false">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdParList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdParList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdParList_OnNeedDataSource" Style="width: 99%"
                                    AutoGenerateColumns="False" OnDataBound="grdParList_DataBound"  OnItemCreated="grdParList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top" DataKeyNames="LineNumber, FullWarningText, DurIndex"
                                        NoMasterRecordsText="No Prior Adverse Reactions.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridTemplateColumn UniqueName="GetInfo">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Image ID="info" runat="server" ImageUrl="~/images/information.png" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn UniqueName="WarningText" HeaderText="Warning"
                                                DataField="WarningText">
                                                <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true"  DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpePAR" runat="server" TargetControlID="parPanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="parPanelHeader" CollapseControlID="parPanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="intPanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="Label4" runat="server" Text="Drug Interactions" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="intPanelDetail" runat="server" Visible="false" Width="100%">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdIntList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdIntList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdIntList_OnNeedDataSource" Style="width: 99%"
                                    AutoGenerateColumns="False" OnDataBound="grdIntList_DataBound" OnItemCreated="grdIntList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top" 
                                        DataKeyNames="ExternalId, LineNumber, FullWarningText, DurIndex"
                                        NoMasterRecordsText="No Drug to Drug Interactions.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridTemplateColumn UniqueName="GetInfo">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Image ID="info" runat="server" ImageUrl="~/images/information.png" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn UniqueName="WarningText" HeaderText="Warning" DataField="WarningText">
                                                <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpeINT" runat="server" TargetControlID="intPanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="intPanelHeader" CollapseControlID="intPanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="dupPanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="lblDup" runat="server" Text="Duplicate Therapy" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="dupPanelDetail" runat="server" Visible="false" Width="100%">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdDupList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdDupList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdDupList_OnNeedDataSource" Style="width: 99%"
                                    AutoGenerateColumns="False" OnDataBound="grdDupList_DataBound" OnItemCreated="grdDupList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top" 
                                        DataKeyNames="ExternalId, LineNumber, FullWarningText, DurIndex"
                                        NoMasterRecordsText="No Duplicate Therapies.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridTemplateColumn UniqueName="GetInfo">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Image ID="info" runat="server" ImageUrl="~/images/information.png" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn UniqueName="WarningText" HeaderText="Warning"
                                                DataField="WarningText">
                                                <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpeDup" runat="server" TargetControlID="dupPanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="dupPanelHeader" CollapseControlID="dupPanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="dosePanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="Label1" runat="server" Text="Dose Check" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="dosePanelDetail" runat="server" Visible="false">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdDoseList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdDoseList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdDoseList_OnNeedDataSource"
                                    AutoGenerateColumns="False" Style="width: 99%" OnDataBound="grdDoseList_DataBound" OnItemCreated="grdDoseList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top"
                                        HierarchyLoadMode="ServerBind" HierarchyDefaultExpanded="true" ExpandCollapseColumn-Display="false"
                                        DataKeyNames="ExternalId, LineNumber, MedicationName, DurIndex"
                                        NoMasterRecordsText="No Dose Check Warnings.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridBoundColumn UniqueName="MedicationName" HeaderText="Medication Name" DataField="MedicationName">
                                                 <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="WarningsForUI">
                                                <ItemTemplate >
                                                    <asp:Literal runat="server" Mode="PassThrough" Text='<%# Eval("WarningsForUI") %>'></asp:Literal>
                                                </ItemTemplate>
                                                 <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpeDose" runat="server" TargetControlID="dosePanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="dosePanelHeader" CollapseControlID="dosePanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="alcPanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="Label2" runat="server" Text="Alcohol Interaction" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="alcPanelDetail" runat="server" Visible="false" Width="100%">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdAlcList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdAlcList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdAlcList_OnNeedDataSource" Style="width: 99%"
                                    AutoGenerateColumns="False" OnDataBound="grdAlcList_DataBound" OnItemCreated="grdAlcList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top" DataKeyNames="ExternalId, LineNumber, FullWarningText, DurIndex"
                                        NoMasterRecordsText="No Alcohol Interactions.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridTemplateColumn UniqueName="GetInfo">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Image ID="info" runat="server" ImageUrl="~/images/information.png" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                             <telerik:GridBoundColumn UniqueName="Warning" HeaderText="Warning" DataField="WarningText">
                                                  <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Onset" HeaderText="Onset" DataField="Onset">
                                                 <ItemStyle Width="50px"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Severity" HeaderText="Severity" DataField="Severity">
                                                 <ItemStyle Width="50px"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Documentation" HeaderText="Documentation" DataField="Documentation">
                                                 <ItemStyle Width="50px"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="cpeALC" runat="server" TargetControlID="alcPanelDetail"
                    Collapsed="false" CollapsedSize="0" ExpandControlID="alcPanelHeader" CollapseControlID="alcPanelHeader"
                    ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td style="padding: 0px 0px 5px 0px">
                <asp:Panel ID="foodPanelHeader" runat="server" Visible="false" BackColor="LightGray"
                    Width="100%" BorderColor="black" BorderWidth="1px">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                &nbsp&nbsp<asp:Label ID="Label3" runat="server" Text="Food Interaction" ForeColor="Black"
                                    Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="foodPanelDetail" runat="server" Visible="false" Width="100%">
                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 15px">
                            </td>
                            <td>
                                <telerik:RadGrid ID="grdFoodList" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    OnItemDataBound="grdFoodList_ItemDataBound" GridLines="None" AllowSorting="False"
                                    AllowAutomaticUpdates="True" OnNeedDataSource="grdFoodList_OnNeedDataSource"
                                    Style="width: 99%" AutoGenerateColumns="False" OnDataBound="grdFoodList_DataBound" OnItemCreated="grdFoodList_ItemCreated">
                                    <MasterTableView GridLines="None" Style="width: 100%" CommandItemDisplay="Top" DataKeyNames="ExternalId, LineNumber, FullWarningText, DurIndex"
                                        NoMasterRecordsText="No Food Interactions.">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridTemplateColumn UniqueName="GetInfo">
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15px" />
                                                <ItemTemplate>
                                                    <asp:Image ID="info" runat="server" ImageUrl="~/images/information.png" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn UniqueName="Warning" HeaderText="Warning"
                                                DataField="WarningText">
                                                 <ItemStyle CssClass="DurText"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Onset" HeaderText="Onset" DataField="Onset">
                                                 <ItemStyle Width="50px"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Severity" HeaderText="Severity" DataField="Severity">
                                                 <ItemStyle Width="50px"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn UniqueName="Documentation" HeaderText="Documentation" DataField="Documentation">
                                                <ItemStyle Width="50px"></ItemStyle>
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn UniqueName="Reason">
                                                <ItemStyle Width="215px" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true"  DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                                <HeaderStyle Width="215px" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblReq" runat="server" Text="*" ForeColor="Red"></asp:Label>
                                                    <telerik:RadComboBox ID="ddlHeaderReasons" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        Width="200px" Visible="true" DataTextField="ReasonDescription"
                                                        DataValueField="ReasonID" EmptyMessage="Choose Ignore Reason" MarkFirstMatch="false">
                                                        <CollapseAnimation Duration="200" Type="OutQuint" />
                                                    </telerik:RadComboBox>
                                                </HeaderTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender2" runat="server"
                    TargetControlID="foodPanelDetail" Collapsed="false" CollapsedSize="0" ExpandControlID="foodPanelHeader"
                    CollapseControlID="foodPanelHeader" ExpandDirection="Vertical" SuppressPostBack="true">
                </ajaxToolkit:CollapsiblePanelExtender>
            </td>
        </tr>
        <tr>
            <td align="left" style="padding: 5px">
                <br />
                <asp:Label ID="lblCopyright" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
                    <asp:Image ID="loadingBar" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif"
                        Style="border: 0; vertical-align: middle; text-align: center" />
                </telerik:RadAjaxLoadingPanel>
                <telerik:RadAjaxManagerProxy ID="radAjaxManager1" runat="server">
                    <AjaxSettings>
                        <telerik:AjaxSetting AjaxControlID="btnSubmit">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="warningError" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdParList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdParList" LoadingPanelID="LoadingPanel1" />
                                <telerik:AjaxUpdatedControl ControlID="txtDetails" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdIntList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdIntList" LoadingPanelID="LoadingPanel1" />
                                <telerik:AjaxUpdatedControl ControlID="txtDetails" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdFoodList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdFoodList" LoadingPanelID="LoadingPanel1" />
                                <telerik:AjaxUpdatedControl ControlID="txtDetails" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdAlcList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdAlcList" LoadingPanelID="LoadingPanel1" />
                                <telerik:AjaxUpdatedControl ControlID="txtDetails" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdCustList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdCustList" LoadingPanelID="LoadingPanel1" />
                                <telerik:AjaxUpdatedControl ControlID="txtDetails" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdDupList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdDupList" LoadingPanelID="LoadingPanel1" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdDoseList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdDoseList" LoadingPanelID="LoadingPanel1" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="grdWarningsList">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdWarningsList" LoadingPanelID="LoadingPanel1" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                    </AjaxSettings>
                </telerik:RadAjaxManagerProxy>  
                <asp:ObjectDataSource ID="WarningTextDummy" runat="server" SelectMethod="GetWarningText"
                    TypeName="Allscripts.Impact.DUR">
                    <SelectParameters>
                        <asp:Parameter Name="WarningText" DefaultValue="" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:Panel ID="durDetailViewPanel" runat="server" Style="display: none;">
        <div id="divHistory" runat="server" class="overlaymainwide dur-hx-width" >
            <div class="overlayTitle">
                DUR Warning Details
            </div>
            <div class="overlayscrollable dur-hx-content-height" style="">
                    <asp:Literal ID="litDetails" runat="server" Mode="PassThrough"></asp:Literal>
            </div>
            <br />
            <div class="overlayFooter">
                <asp:Button ID="btnCloseInfo" runat="server" Text="Close" CssClass="btnstyle btnStyleAction" Style="vertical-align: bottom" />
            </div>    
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenDurDetail" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeDurDetail" runat="server" BehaviorID="mpeDurDetail"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenDurDetail"
        PopupControlID="durDetailViewPanel" CancelControlID="btnCloseInfo"  >
    </ajaxToolkit:ModalPopupExtender>
    <ePrescribe:EPCSDigitalSigning ID="ucEPCSDigitalSigning" runat="server" IsScriptForNewRx="false" />
    <ePrescribe:CSMedRefillRequestNotAllowed ID="ucCSMedRefillRequestNotAllowed" runat="server" />
    <asp:Button ID="btnHndReload" runat="server" Text="" OnClick="btnHndReload_Click" Style="display:none" />
    <script type="text/javascript">
        function scriptPadRefresh() {
            var btn = $('#<%= btnHndReload.ClientID%>');
            if (btn != undefined && btn != null) {
                btn.click();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
  
</asp:Content>
