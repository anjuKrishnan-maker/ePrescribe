<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.TaskScriptList" Title="Script List" Codebehind="TaskScriptList.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/SecureProcessing.ascx" TagName="SecureProcessing"  TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ControlsPatMissingInfo.ascx" TagName="ControlsPatMissingInfo" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        function cleanWhitespace(node) {
            for (var x = 0; x < node.childNodes.length; x++) {
                var childNode = node.childNodes[x]
                if ((childNode.nodeType == 3) && (!/\S/.test(childNode.nodeValue))) {
                    // that is, if it's a whitespace text node
                    node.removeChild(node.childNodes[x])
                    x--
                }
                if (childNode.nodeType == 1) {
                    // elements can have text child nodes of their own
                    cleanWhitespace(childNode)
                }
            }
        }

    </script>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr class="h1title">
            <td>
            </td>
        </tr>
        <tr class="h2title" align="left" nowrap="nowrap">
            <td>
                <ePrescribe:Message ID="ucSupervisingProvider" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <table width="100%" border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0">
                    <tr>
                        <!--     <td style="width: 753px">  -->
                        <td width="100%">
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr class="h4title">
                                    <td>
                                    </td>
                                    <td colspan="3">
                                    <asp:Panel ID="pnlButtons" runat="server">
                                        <asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnCancel_Click" />
                                        <asp:Button ID="btnChangePharm" CssClass="btnstyle" runat="server" Text="Choose Pharmacy"
                                            Enabled="false" OnClick="btnChangePharm_Click" />
                                        <asp:Button ID="btnProcess" CssClass="btnstyle" runat="server" Text="Process Task List ►"
                                            OnClick="btnProcess_Click" />
                                    </asp:Panel>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadGrid ID="grdListScirpt" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" GridLines="None" Width="100%" PageSize="50" Skin="Allscripts"
                                EnableEmbeddedSkins="False" DataSourceID="RxScriptListObjDataSource" OnDataBound="grdListScirpt_DataBound"
                                OnItemDataBound="grdListScirpt_ItemDataBound">
                                <MasterTableView DataKeyNames="TaskID,ControlledSubstanceCode,SenderId,RxID,LineNumber,MessageData,StateControlledSubstanceCode,DDI,PharmacyIsElectronicEnabled,MedicationName,Strength,StrengthUOM,Quantity,RefillQuantity,DaysSupply,SIGText,DAW,PharmacyNotes, FormDescription"
                                    AllowNaturalSort="false">
                                    <NoRecordsTemplate>
                                        No Scripts
                                    </NoRecordsTemplate>
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderText="Medication And Sig">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMedicationSig" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn UniqueName="RxDate" DataField="RxDate" HeaderText="Rx Date"
                                            SortExpression="RxDate">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn UniqueName="Prescriber" DataField="Prescriber" HeaderText="Prescriber"
                                            SortExpression="Prescriber">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridTemplateColumn UniqueName="SendtoADM" HeaderText="Send Notification">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSendToADM" runat="server" Width="100px" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridHyperLinkColumn UniqueName="EPAPortalLink" Text="Sign into ePA Portal"
                                            Target="_blank">
                                        </telerik:GridHyperLinkColumn>
                                        <telerik:GridTemplateColumn HeaderText="Destination">
                                            <HeaderTemplate>
                                                <table cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td style="vertical-align: middle">
                                                            <b>Destination</b>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <div style="white-space: nowrap">
                                                    <telerik:RadComboBox ID="ddlDest" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        AllowCustomText="False" ShowToggleImage="True" MarkFirstMatch="True" Width="200px">
                                                    </telerik:RadComboBox>
                                                    <img id="Img1" runat="server" onclick="showMenu(event)" src="images/arrowdown-nor-dark-12-x-12.png"
                                                      class="combobtnstyledropdown" style="display: inline; height: 8px; width: auto;"
                                                       tabindex="6"/>
                                                    &nbsp
                                                    <asp:Label ID="lblCSMed" runat="server" ForeColor="red" Visible="false">CS</asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="RxScriptListObjDataSource" runat="server" SelectMethod="CHTaskScriptList"
                                TypeName="Allscripts.Impact.TaskManager" OnSelected="RxScriptListObjDataSource_Selected">
                                <SelectParameters>
                                    <asp:SessionParameter Name="LicenseId" SessionField="LICENSEID" Type="String" />
                                    <asp:SessionParameter Name="PatientId" SessionField="TL_PatientId" Type="String" />
                                    <asp:SessionParameter Name="ProviderID" SessionField="TL_ProviderID" Type="String" />
                                    <asp:SessionParameter Name="TaskType" SessionField="TL_TaskType" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <br />
                            <asp:Label ID="lblCSLegend" runat="server" ForeColor="red" Visible="false">CS - Federal and state specific controlled substances cannot be sent electronically.</asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <ePrescribe:SecureProcessing ID="secureProcessingControl" runat="server" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel2" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="radAjaxManager" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnProcess">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlButtons" LoadingPanelID="LoadingPanel2" />
                    <telerik:AjaxUpdatedControl ControlID="grdListScirpt" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <ePrescribe:ControlsPatMissingInfo ID="ucPatMissingInfo" runat="server"/>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
    </asp:UpdatePanel>
</asp:Content>
