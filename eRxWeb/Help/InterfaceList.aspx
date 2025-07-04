<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPageNew.master" AutoEventWireup="true" Inherits="eRxWeb.Help_InterfaceList" Title="Interface Selection" Codebehind="InterfaceList.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" language="javascript" src="../js/formUtil.js"></script>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">         
        
        function interfaceSelected(source, eventArgs)
        {  
            GridSingleSelect(eventArgs.getDataKeyValue("InterfaceID").toString(), "InterfaceID", "<%= grdInterfaces.MasterTableView.ClientID %>", false)
        }

    </script>
</telerik:RadCodeBlock>    
<div>
    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" Icon="ERROR" />
    <center>
        <span class="strongText" style="padding-bottom:3px;">Select Your Practice Management System</span><br />
     </center>
</div>
<div>
    <telerik:RadAjaxManagerProxy ID="RadAjaxManager1" runat="server">
         <AjaxSettings>
             <telerik:AjaxSetting AjaxControlID="grdInterfaces">
                 <UpdatedControls>
                     <telerik:AjaxUpdatedControl ControlID="grdInterfaces" LoadingPanelID="LoadingPanel1"/>
                 </UpdatedControls>
            </telerik:AjaxSetting>
         </AjaxSettings>
    </telerik:RadAjaxManagerProxy>                         
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <asp:Image ID="loadingBar" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif" style="border: 0; vertical-align:middle; text-align:center" />
    </telerik:RadAjaxLoadingPanel>
    <asp:ObjectDataSource ID="InterfaceObjDataSource" runat="server" SelectMethod="GetInterfaceSystems"
        TypeName="Allscripts.Impact.SystemConfig">
        <SelectParameters>
            <asp:Parameter DefaultValue="Y" Name="Active" Type="String" />
            <asp:Parameter DefaultValue="all" Name="free" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
</div>
<table width="95%" cellpadding="0" cellspacing="0">
    <tr>
        <td width="5%">
        </td>
        <td width="90%">
            <telerik:RadGrid ID="grdInterfaces" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                            GridLines="None" DataSourceID="InterfaceObjDataSource" OnItemDataBound="grdInterfaces_ItemDataBound"
                            AllowAutomaticUpdates="False" AllowMultiRowSelection="false" EnableViewState="true" 
                            AllowPaging="False" AllowSorting="True" style="width:98%; height:395px; vertical-align:top;" HorizontalAlign="Center"
                            AutoGenerateColumns="False">
                <MasterTableView GridLines="None" NoMasterRecordsText="No Interfaces Available" style="width:100%; vertical-align:top" CommandItemDisplay="None"
                                 DataKeyNames="InterfaceID, Name, TierOneBase, TierOneMonth, TierTwoBase, TierTwoMonth" ClientDataKeyNames="InterfaceID">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <Columns>
                        <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn">
                            <ItemStyle Width="20px" />
                            <ItemTemplate>
                                <input id="rbSelect" runat="server" type="radio" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Practice Management System" UniqueName="Name">
                            <ItemTemplate>
                                <div id="nameDiv" runat="server"></div>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings>
                    <ClientEvents OnRowClick="interfaceSelected"/>  
                    <Selecting AllowRowSelect="True"></Selecting>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="True" ScrollHeight="395px" ></Scrolling>
                </ClientSettings>
            </telerik:RadGrid>            
        </td>
        <td width="5%">
        </td>            
    </tr>
</table>
<div>
    <center>
        <br />
        <asp:Button ID="btnContinueBottom" runat="server" Text="Continue" CssClass="btnstyle" style="padding-bottom:3px; font-size:13px;" OnClick="btnContinueBottom_Click" />&nbsp&nbsp
        <asp:Button ID="btnNotInList" runat="server" Text="Not in List" CssClass="btnstyle" style="padding-bottom:3px; font-size:13px;" OnClick="btnNotInList_Click" />
    </center>
</div>
</asp:Content>

