<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.PharmRefillSummary" Title="Pharmacy Refills" Codebehind="PharmRefillSummary.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ObsoletePARClassMappingChange.ascx" TagName="ObsoletePARClassMappingChange"
    TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellspacing="0" cellpadding="0" width="100%" border="0">
        <tr class="h1title">
            <td>
                <table width="100%" cellpadding="3px">
                    <tr>
                        <td>
                            <asp:RadioButton ID="rbAdminTask" runat="server" GroupName="Task" Text="Assistant's Tasks"
                                CssClass="adminlink1" AutoPostBack="true" OnCheckedChanged="rbAdminTask_Changed" />
                            <asp:RadioButton ID="rbPharmRefills" runat="server" GroupName="Task" Text="Pharmacy Tasks"
                                CssClass="adminlink1" Checked="true" />
                            <asp:RadioButton ID="rbEPATasks" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="ePA Tasks" OnCheckedChanged="rbEPATasks_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbSpecialtyMed" runat="server" AutoPostBack="true" GroupName="Task2"
                                Text="Patient Access Services" OnCheckedChanged="rbSpecialtyMed_CheckedChanged" CssClass="adminlink1" />
                        </td>
                        <td align="right">
                            <span class="adminlink1">Show tasks for:</span>&nbsp
                            <asp:DropDownList ID="ddlProvider" runat="server" Width="170px" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:Button ID="Button1" runat="server" CssClass="btnstyle" OnClick="btnPharmRefillReport_Click"
                                Text="Pharmacy Refill Report" Width="149px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr class="h2title">
            <td>
                <ePrescribe:Message ID="ucSupervisingProvider" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td style="height: 447px">
                <table cellspacing="0" cellpadding="0" width="100%" align="center">
                    <tr>
                        <td>
                            <telerik:RadGrid ID="grdPharmRefill" runat="server" PageSize="50" GridLines="None"
                                EmptyDataText="No Pharmacy Refills" DataSourceID="PharmRefillObjDataSource" CaptionAlign="Left"
                                AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True" Width="100%"
                                OnItemDataBound="grdPharmRefill_ItemDataBound" Skin="Allscripts" EnableEmbeddedSkins="false"
                                OnItemCommand="grdPharmRefill_ItemCommand">
                                <PagerStyle Mode="NextPrevAndNumeric" />
                                <MasterTableView GridLines="None" NoMasterRecordsText="No refills found." Style="width: 100%"
                                    CommandItemDisplay="None" DataKeyNames="ScriptMessageID,PatientID,ProviderID">
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lbViewDetails" runat="server" Text="View" CommandName="ViewDetails"
                                                    CommandArgument='<%# ObjectExtension.ToEvalEncode(Eval("ProviderID")) %>'></asp:LinkButton>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="Physician" HeaderText="Physician" SortExpression="Physician"
                                            HeaderStyle-HorizontalAlign="Left">
                                            <ItemStyle Width="150px" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Patient" HeaderText="Patient" SortExpression="Patient"
                                            HeaderStyle-HorizontalAlign="Left">
                                            <ItemStyle Width="250px" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="PatientRegisterStatus" HeaderText="Status" SortExpression="PatientRegisterStatus"
                                            HeaderStyle-HorizontalAlign="Left">
                                            <ItemStyle Width="250px" HorizontalAlign="Left" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DrugDescription" HeaderText="Medication" SortExpression="DrugDescription"
                                            HeaderStyle-HorizontalAlign="Left" />
                                        <telerik:GridBoundColumn DataField="MessageDate" HeaderText="Date Received" SortExpression="MessageDate"
                                            HeaderStyle-HorizontalAlign="Left">
                                            <ItemStyle Width="150px" />
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="PharmRefillObjDataSource" runat="server" TypeName="Allscripts.Impact.Provider"
                                SelectMethod="GetPharmRefillSummary">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                    <asp:Parameter Name="providerID" Type="string" />
                                    <asp:Parameter Name="pobID" DefaultValue="" Type="string" />
                                    <asp:Parameter Name="patientID" DefaultValue="" Type="String" />
                                    <asp:Parameter Name="userID" DefaultValue="" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel Style="display: none" ID="panelSetSupervisingProvider" runat="server">
        <div class="overlaymain" style="width: 300px;">
        <table class="overlayContent">
            <tr>
                <td>
                    <b>Please select a supervising provider below.</b>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlSupervisingProvider" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
            <div class="overlayFooter">
                <asp:Button ID="btnSetSupervisingProvider" runat="server" CssClass="btnstyle btnStyleAction" Text="Set Supervising Provider"
                        OnClick="btnSetSupervisingProvider_Click" />
            </div>
            </div>
    </asp:Panel>
    <asp:Button ID="hiddenSetSupervisingProvider" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeSetSupervisingProvider" runat="server" BehaviorID="mpeSetSupervisingProvider"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenSetSupervisingProvider"
        PopupControlID="panelSetSupervisingProvider" PopupDragHandleControlID="panelSetSupervisingProvider" />
    <ePrescribe:ObsoletePARClassMappingChange ID="ucObsoletePARClassMappingChange" runat="server"
        CurrentPage="PharmRefillSummary.aspx" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
  <%--  <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
            <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex="0" runat="server" ContentCssClass="accordionContent"
                HeaderCssClass="accordionHeader">
                <Panes>
                    <ajaxToolkit:AccordionPane ID="paneHelp" runat="server">
                        <Header>
                            Help With This Screen</Header>
                        <Content>
                            <asp:Panel ID="HelpPanel" runat="server">
                            </asp:Panel>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>
            </ajaxToolkit:Accordion>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
