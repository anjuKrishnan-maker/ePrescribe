<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.Controls_PatientSearchWithResults" Codebehind="PatientSearchWithResults.ascx.cs" %>
<%@ Register Src="~/Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/PatientSearch.ascx" TagName="PatientSearch" TagPrefix="ePrescribe" %>
<telerik:RadAjaxManager ID="ramPatientSearchWithResults" runat="server">
</telerik:RadAjaxManager>
<asp:Panel ID="pnlSearchPatientResult" runat="server" CssClass="overlaymainwide"
    Style="display:none">
    <div class="overlayTitle">
        Patient Search
    </div>
    <asp:Panel ID="pnlMainSearchPatient" runat="server">
        <table width="100%">
            <tr>
                <td>
                    <ePrescribe:Message runat="server" ID="ucMessage" Visible="false" />
                </td>
            </tr>
            <tr class="h1title">
                <td>
                    <asp:Panel ID="pnlSearchPatient" runat="server">
                        <ePrescribe:PatientSearch ID="ucPatientSearch" runat="server" SearchLabel="Choose Patient"
                            ShowAddPatient="false" OnPatientSearchEvent="patientSearchEvent" TabIndex="1" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div style="width: inherit;">
        <div style="width: 99.2%; margin-left:2px;">
            <telerik:RadAjaxManagerProxy ID="RadAjaxManager1" runat="server">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="grdViewPatients">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="grdViewPatients"  />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                    <telerik:AjaxSetting AjaxControlID="btnConfirm">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="grdMessageQueue"  />
                            <telerik:AjaxUpdatedControl ControlID="panelMsgDetail"  />
                            <telerik:AjaxUpdatedControl ControlID="pnlMessage" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManagerProxy>
            <telerik:RadGrid ID="grdViewPatients" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                GridLines="None" AllowAutomaticUpdates="False" AllowMultiRowSelection="false"
                EnableViewState="true" AllowPaging="true" AllowSorting="True" Style="width: 100%"
                AutoGenerateColumns="False" OnItemCommand="grdViewPatients_ItemCommand" OnItemDataBound="grdViewPatients_RowDataBound"
                PageSize="5" OnPageIndexChanged="grdViewPatients_PageIndexChanged" TabIndex="2">
                <PagerStyle Mode="NextPrevAndNumeric" />
                <MasterTableView GridLines="None" NoMasterRecordsText="No records found for the search criteria"
                    Style="width: 100%; vertical-align: top" CommandItemDisplay="None" ClientDataKeyNames="PatientID, StatusID,Name,IsVIPPatient, IsRestrictedPatient"
                    DataKeyNames="PatientID, StatusID,IsVIPPatient, IsRestrictedPatient">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <HeaderStyle Font-Bold="true" />
                    <Columns>
                        <telerik:GridButtonColumn Text="Select" CommandName="SelectPatient">
                        </telerik:GridButtonColumn>
                        <telerik:GridBoundColumn DataField="MRN" SortExpression="MRN" HeaderText="Patient ID">
                            <ItemStyle Width="65px"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Name" UniqueName="ImgColumn" SortExpression="Name" HeaderText="Patient Name">
                            <ItemStyle Width="150px"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DOB" HeaderText="DOB">
                            <ItemStyle Width="90px"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone Number">
                            <ItemStyle Width="95px"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Address" HeaderText="Street Address">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <asp:ObjectDataSource ID="PatObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                TypeName="Allscripts.Impact.CHPatient" SelectMethod="SearchPatient" OnSelected="PatObjDataSource_Selected">
                <SelectParameters>
                    <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                    <asp:Parameter Name="LastName" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                    <asp:Parameter Name="FirstName" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                    <asp:Parameter Name="DateOfBirth" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                    <asp:Parameter Name="ChartID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                    <asp:Parameter Name="WildCard" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                    <asp:Parameter Name="HasVIPPatients" Type="Boolean" DefaultValue="false" />
                    <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                    <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="true" />
                    <asp:Parameter Name="includeInactive" Type="boolean" DefaultValue="false" />
                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </div>
    </div>
    <div class="overlayFooter">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btnstyle" Width="100px"
                        OnClick="btnCancel_Click" TabIndex="3" />
         <asp:Button ID="btnCancelAndClear" runat="server" Text="Cancel and Clear Filter"
                        CssClass="btnstyle" OnClick="btnCancelAndClear_OnClick" TabIndex="4" />
    </div>
</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="mpePatientSearch" runat="server" TargetControlID="btnPopUpTrigger"
    DropShadow="false" PopupControlID="pnlSearchPatientResult" BackgroundCssClass="modalBackground"
    CancelControlID="btnCancel">
</ajaxToolkit:ModalPopupExtender>
<asp:Button ID="btnPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />
