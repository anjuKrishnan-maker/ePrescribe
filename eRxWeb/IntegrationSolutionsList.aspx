<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.IntegrationSolutionsList"
    Title="Integration Solutions" Codebehind="IntegrationSolutionsList.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
    <style type="text/css">
        .AlterRowItem {
            background-color: inherit;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <tr class="h1title">
            <td align="right">
                <telerik:RadAjaxManagerProxy ID="RadAjaxManager1" runat="server">
                    <AjaxSettings>
                        <telerik:AjaxSetting AjaxControlID="grdIntegration">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="grdIntegration" LoadingPanelID="LoadingPanel1" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                    </AjaxSettings>
                </telerik:RadAjaxManagerProxy>
            </td>
        </tr>
       
    </table>
    <table border="0" cellspacing="0" cellpadding="0" width="100%">
        <tr>
            <td align="center">
                <asp:Panel ID="toolShowcase" runat="server" Style="width: 100%">
                    <table>
                        <tr>
                            <td align="center">
                                <asp:Image ID="imShowCase" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Panel ID="showCaseLiteral" runat="server">
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td align="center">
                <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
                    <asp:Image ID="loadingBar" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif"
                        Style="border: 0; vertical-align: middle; text-align: center; font-family:Arial Narrow;" />
                </telerik:RadAjaxLoadingPanel>
                <telerik:RadGrid ID="grdIntegration" runat="server" EnableEmbeddedSkins="false"
                    OnItemDataBound="grdIntegration_ItemDataBound" HeaderStyle-Height="0px" GridLines="Horizontal"
                    DataSourceID="AllModules" ShowHeader="true" AllowSorting="false" Style="width: 80%; padding-left: 100px; text-align:left;"
                    AutoGenerateColumns="False">
                    <AlternatingItemStyle CssClass="AlterRowItem"></AlternatingItemStyle>
                    <MasterTableView GridLines="None" NoMasterRecordsText="No integration solutions."
                        NoDetailRecordsText="No integration tools." Style="width: 100%" CommandItemDisplay="None"
                        DataKeyNames="ModuleID,IconPath,SmallAdPath,Enabled,LargeAdPath,ControlPath,FullDetail">
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="Briefing">
                                <ItemStyle Height="100px" />
                                <ItemTemplate>
                                    <asp:Panel ID="panelEnroll" runat="server" HorizontalAlign="Right">
                                       <asp:Image runat="server" ID="imModuleImage" ImageUrl='<%# ObjectExtension.ToEvalEncode(Eval("LargeAdPath")) %>'
                                        Style="border: none; text-align:right; vertical-align:top; display: inline; width: 200px; height:66px"></asp:Image>
                                    </asp:Panel>
                            </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="Briefing">
                                <ItemStyle Height="100px" Width="90%"/>
                                <ItemTemplate>
                                    <asp:Label ID="lblHeaderText" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("ModuleHeaderText")) %>' CssClass="primaryForeColor" Font-Bold="true" Visible="true" Font-Size="xx-large"></asp:Label><br />
                                    <asp:Label ID="lblSubscriptionType" runat="server" Font-Bold="true" CssClass="primaryForeColor" Font-Size="xx-large"></asp:Label>
                                    <asp:Image runat="server" ID="imEnrolledImage" Style="border: none;" background-color="#C1CDC1" > </asp:Image>
                                    <br />
                                    <br />
                                    <span style="font-size: larger"><asp:Literal ID="literalBrief" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("BriefDetailHTML")) %>'></asp:Literal></span>
                                  
                                    <b><a class="primaryForeColor" style="vertical-align:bottom; " href='<%# ObjectExtension.ToEvalEncode(Eval("ActionLink")) %>'><%# ObjectExtension.ToEvalEncode(Eval("ActionText")) %></a></b>
                                    <br />
                                    <br />                                   
                                    <asp:Button ID="btnEnroll" runat="server" Width="130" Height="30" OnClick="btnGo_Click" Text="Enroll" CssClass="btnStyleAction" visible="false" />
                                    <br />  <br />  
                                    <asp:Button ID="btnAddFeature" runat="server" Width="130" Height="30" OnClick="btnAddFeature_Click"  Text="Add New Features"  CssClass="btnStyleAction" visible="false" />                                    
                                        <br /><br /><br /><br />
                            </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>                       
                    </MasterTableView>
                </telerik:RadGrid>
                <asp:ObjectDataSource ID="AllModules" runat="server" SelectMethod="GetModules" TypeName="Allscripts.Impact.Module"
                    DataObjectTypeName="Module">
                    <SelectParameters>
                        <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" />
                        <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                        <asp:Parameter Name="Status" Type="Int32" DefaultValue="0" />
                        <asp:Parameter Name="AdvertiseDeluxe" Type="Boolean" />
                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">

</asp:Content>
