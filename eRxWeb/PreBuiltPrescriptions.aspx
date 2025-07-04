<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreBuiltPrescriptions.aspx.cs"
    MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.PreBuiltPrescriptions" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">

        </script>
    </telerik:RadCodeBlock>
    <div style="height: 650px;">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr class="h1title">
                <td class="Phead indnt">
                    Manage Pre-Built Prescriptions
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="3">
                        <tr class="h2title" align="left">
                            <td>
                                <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_OnClick" CssClass="btnstyle" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr class="h3title">
                            <td>
                                &nbsp; &nbsp;
                                <table width="30%">
                                    <tr>
                                        <td>
                                            &nbsp; &nbsp;<asp:ImageButton ID="imgBtnPlus" runat="server" ImageUrl="~/images/Insert.gif"
                                                Style="border: 0px; vertical-align: middle; cursor: pointer;" PostBackUrl="~/PreBuiltPrescriptionsAddOrEdit.aspx" />&nbsp;<a
                                                    href="PreBuiltPrescriptionsAddOrEdit.aspx">Add New Group </a>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chkboxShowAllGroupd" runat="server" AutoPostBack="true" Text="Show All Groups"
                                                OnCheckedChanged="chkboxShowAllGroupd_CheckedChanged" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div>
            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableOutsideScripts="True"
                EnableAJAX="true">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="gridGroups">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridGroups" LoadingPanelID="AjaxLoadingPanel1" />
                            <telerik:AjaxUpdatedControl ControlID="chkboxShowAllGroupd" LoadingPanelID="AjaxLoadingPanel1" />
                            <telerik:AjaxUpdatedControl ControlID="pnlModalPopUp" LoadingPanelID="AjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="chkboxShowAllGroupd">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridGroups" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="btnYes" EventName="Click">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="gridGroups" LoadingPanelID="AjaxLoadingPanel1" />
                            <telerik:AjaxUpdatedControl ControlID="pnlModalPopUp" LoadingPanelID="AjaxLoadingPanel1" />
                            <telerik:AjaxUpdatedControl ControlID="chkboxShowAllGroupd" LoadingPanelID="AjaxLoadingPanel1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
            <telerik:RadGrid ID="gridGroups" runat="server" AutoGenerateColumns="false" EnableEmbeddedSkins="false"
                Skin="Allscripts" AllowSorting="true" AllowPaging="true" PageSize="20" OnItemCommand="gridGroups_ItemCommand"
                OnNeedDataSource="gridGroups_NeedDataSource"
                OnPageIndexChanged="gridGroups_PageIndexChanged">
                <MasterTableView DataKeyNames="ID, Name,IsActive" AllowNaturalSort="true">
                    <NoRecordsTemplate>
                        <div>
                            <p>
                                No records found.
                            </p>
                        </div>
                    </NoRecordsTemplate>
                    <HeaderStyle Font-Bold="true" />
                    <Columns>
                        <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="images/Edit.gif" Text="Edit"
                            ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" UniqueName="Edit"
                            CommandName="Edit">
                        </telerik:GridButtonColumn>
                        <telerik:GridBoundColumn DataField="Name" HeaderText="Group" SortExpression="Name">
                            <ItemStyle Width="35%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ModifiedDate" HeaderText="Modified" SortExpression="ModifiedDate">
                            <ItemStyle Width="25%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="IsActive" HeaderText="Status" SortExpression="IsActive">
                            <ItemStyle Width="25%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkBtnDelete" runat="server" Text="Delete" CommandName="Delete"></asp:LinkButton>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <PagerStyle Mode="NextPrevAndNumeric" PagerTextFormat="{4}" AlwaysVisible="false" />
            </telerik:RadGrid>
        </div>
    </div>
    <asp:Panel ID="pnlModalPopUp" runat="server">
        <asp:HiddenField ID="hdnFldGroupId" runat="server" />
        <asp:Button ID="hiddenFA" runat="server" Style="display: none;" />
        <ajaxToolkit:ModalPopupExtender ID="mpeConfirmDeleteGroup" runat="server" BehaviorID="mpeConfirmDeleteGroup"
            DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenFA"
            CancelControlID="btnNo" PopupControlID="pnlConfirmDeleteGroup">
        </ajaxToolkit:ModalPopupExtender>
        <asp:Panel Style="display: none;" ID="pnlConfirmDeleteGroup" runat="server">
            <div id="div1" class="overlaymain" style="height: 75px; width: 400px; padding: 1px;">
                <center style="margin-top: 10px;">
                    <asp:Label ID="lblConfirmDeleteMessage" runat="server" Text=""></asp:Label></center>
                <br />
                <center>
                    <asp:Button ID="btnYes" runat="server" Text="Delete Group" Width="100px" OnClick="btnYes_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <input type="button" value="Cancel" id="btnNo" style="width: 75px;" />
                </center>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <contenttemplate>        
         <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex=0 runat="server" ContentCssClass="accordionContent" HeaderCssClass="accordionHeader">
                        <Panes>
                            <ajaxToolkit:AccordionPane ID="paneHelp" runat="server">
                            <Header>Help With This Screen</Header>
                            <Content>
                                <asp:Panel ID="HelpPanel" runat="server"></asp:Panel>
                               </Content>
                            </ajaxToolkit:AccordionPane>                            
                        </Panes>
                    </ajaxToolkit:Accordion>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>