<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.MessageQueueTx"
    MasterPageFile="~/PhysicianMasterPageBlank.master" Title="Message Queue: Prescriptions" Codebehind="MessageQueueTx.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Src="Controls/PatientSearchWithResults.ascx" TagName="PatientSearchWithResults"
    TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
        <script language="javascript" type="text/javascript">
            function showStartPopup() {
                $find("<%= startDate.ClientID %>").showPopup();
            }

            function showEndPopup() {
                $find("<%= endDate.ClientID %>").showPopup();
            }

            function btnClear_OnClientClick() {
                var item;

                var ddView = $find("<%= ddlView.ClientID %>");
                if (ddView != null) {
                    item = ddView.findItemByValue("0");
                    item.select();
                }

                var ddAuth = $find("<%= ddlAuthorizer.ClientID %>");
                if (ddAuth != null) {
                    item = ddAuth.findItemByText("All");
                    item.select();
                }

                var ddOrig = $find("<%= ddlOriginator.ClientID %>");
                if (ddOrig != null) {
                    item = ddOrig.findItemByText("All");
                    item.select();
                }

                var lblPatient = document.getElementById("<%=lblPatient.ClientID %>");
                if (lblPatient != null) {
                    lblPatient.innerText = "";
                }

                var lnkSelectPatient = document.getElementById("<%= lnkSelectPatient.ClientID %>");
                if (lnkSelectPatient != null) {
                    lnkSelectPatient.innerText = "Search Patient";
                }

                var hdnPatientID = document.getElementById("<%= hdnPatientID.ClientID %>");
                if (hdnPatientID != null) {
                    hdnPatientID.value = "";
                }
            }

            function grdMessageQueue_OnRowClick(source, eventArgs) {                
                GridSingleSelect(eventArgs.getDataKeyValue("ScriptMessageID").toString(), "ScriptMessageID", "<%= grdMessageQueue.MasterTableView.ClientID %>", false)
                getSelectedMessage(eventArgs.getDataKeyValue("ScriptMessageID").toString(), eventArgs.getDataKeyValue("ErrorConfirmedByName") == null ? '' : eventArgs.getDataKeyValue("ErrorConfirmedByName").toString(),
                    eventArgs.getDataKeyValue("ErrorConfirmedDateTime") == null ? '' : eventArgs.getDataKeyValue("ErrorConfirmedDateTime").toString(),
                    eventArgs.getDataKeyValue("DisplayType") == null ? '' : eventArgs.getDataKeyValue("DisplayType").toString(),
                    eventArgs.getDataKeyValue("DisplayStatus") == null ? '' : eventArgs.getDataKeyValue("DisplayStatus").toString());
            }

            function rbSelect_OnClick(scriptMessageID, ErrorConfirmedByName, ErrorConfirmedDateTime, DisplayType, DisplayStatus) {               
                document.getElementById('<%=pnlMessage.ClientID%>').style.visibility = 'hidden'; 
                GridSingleSelect(scriptMessageID, "ScriptMessageID", "<%= grdMessageQueue.MasterTableView.ClientID %>", true)
                getSelectedMessage(scriptMessageID, ErrorConfirmedByName,ErrorConfirmedDateTime, DisplayType, DisplayStatus);
            }

            function getSelectedMessage(scriptMessageID, ErrorConfirmedByName,ErrorConfirmedDateTime, DisplayType, DisplayStatus) {
                document.getElementById("<%=HiddenFieldscriptMessageID.ClientID %>").value = scriptMessageID;
                document.getElementById("<%=HiddenFieldErrorConfirmedByName.ClientID %>").value = ErrorConfirmedByName;
                document.getElementById("<%=HiddenFieldErrorConfirmedDateTime.ClientID %>").value = ErrorConfirmedDateTime;
                document.getElementById("<%=HiddenFieldDisplayType.ClientID %>").value = DisplayType;
                document.getElementById("<%=HiddenFieldDisplayStatus.ClientID %>").value = DisplayStatus;
                var hiddenSelect = document.getElementById("<%=hiddenSelect.ClientID %>");               

                msgQueScriptLoad(scriptMessageID);

                if (hiddenSelect != null) {                  
                    hiddenSelect.click();
                }               
            }

            function clearPatientInfo()
            {               
                document.getElementById("<%=btnConfirm.ClientID %>").disabled = true;
                msgQueScriptLoad(null);
            }
        </script>
    </telerik:RadCodeBlock>
    <div>
    <asp:HiddenField ID="HiddenFieldscriptMessageID" runat="server" />
    <asp:HiddenField ID="HiddenFieldErrorConfirmedByName" runat="server" />
    <asp:HiddenField ID="HiddenFieldErrorConfirmedDateTime" runat="server" />
    <asp:HiddenField ID="HiddenFieldDisplayType" runat="server" />
    <asp:HiddenField ID="HiddenFieldDisplayStatus" runat="server" />
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td>
                    <table border="0" width="100%" cellspacing="0" style="table-layout: fixed;">
                        <tr>
                            <td colspan="2" class="h1title indnt Phead" align="left" style="width: 100%">
                                <asp:Label ID="lblHeader" CssClass="PheadSmall" Text="Message Queue for " runat="server"></asp:Label>                             
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Panel ID="pnlMessage" runat="server">
                                    <ePrescribe:Message runat="server" ID="ucMessage" Visible="false" />
                                </asp:Panel>
                               <%-- <asp:ValidationSummary ID="ValidationSummary1" runat="server" />--%>
                            </td>
                        </tr>
                        <tr class="h3title">
                            <td colspan="2">
                                <asp:Panel ID="pnlMsgQueueSearch" DefaultButton="btnSearch" runat="server">
                                    <table>
                                        <tr>
                                            <td class="h4titletext" style="padding-left: 5px; vertical-align: middle; width: 75px"
                                                align="right">
                                                <b>Start Date:</b>
                                            </td>
                                            <td>
                                                <telerik:RadDatePicker ID="startDate" Style="vertical-align: middle;" runat="server"
                                                    MaxDate="2030-12-31" TabIndex="1" dateformat="MM/dd/yyyy" displaydateformat="MM/dd/yyyy"
                                                    ToolTip="Please enter a valid Start Date in (mm/dd/yyyy) format.">
                                                    <Calendar runat="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False"
                                                        ViewSelectorText="x">
                                                    </Calendar>
                                                    <DateInput ID="DateInput1" onclick="showStartPopup()" runat="server" TabIndex="1">
                                                    </DateInput>
                                                    <DatePopupButton ImageUrl="" HoverImageUrl="" TabIndex="2"></DatePopupButton>
                                                </telerik:RadDatePicker>
                                                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="startDate" ValidationGroup="messageQRpt"
                                                    ErrorMessage="Please enter a valid start Date." Width="1px">*</asp:RequiredFieldValidator>
                                                        <asp:CompareValidator ID="CompareValstartDate" runat="server" ControlToCompare="endDate" ValidationGroup="messageQRpt"
                                                    ControlToValidate="startDate" Display="Dynamic"
                                                    Operator="LessThanEqual"  Type="Date">Start date should be lesser than or equal to end date</asp:CompareValidator>
                                                    <asp:CompareValidator ID="compareValidatorStartDate" runat="server" ControlToValidate="startDate" ValidationGroup="messageQRpt"
                                                        Display="Dynamic" Operator="LessThanEqual" Type="Date">Start date should be lesser than or equal to today</asp:CompareValidator>

                                            </td>
                                            <td class="h4titletext" style="padding-left: 5px; vertical-align: middle; width: 75px"
                                                align="right">
                                                <b>View:</b>
                                            </td>
                                            <td>
                                                <telerik:RadComboBox ID="ddlView" runat="server" AllowCustomText="false" AutoPostBack="false"
                                                    Width="200px" TabIndex="4">
                                                    <Items>
                                                        <telerik:RadComboBoxItem Text="All" Value="0" />
                                                        <telerik:RadComboBoxItem Text="Pending" Value="1" />
                                                        <telerik:RadComboBoxItem Text="Sent" Value="2" />
                                                        <telerik:RadComboBoxItem Text="Confirmed" Value="3" />
                                                        <telerik:RadComboBoxItem Text="Error" Value="4" />
                                                    </Items>
                                                </telerik:RadComboBox>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h4titletext" style="padding-left: 5px; vertical-align: middle; width: 75px"
                                                align="right">
                                                <b>End Date:</b>
                                            </td>
                                            <td>
                                                <telerik:RadDatePicker ID="endDate" Style="vertical-align: middle;" runat="server"
                                                    MaxDate="2030-12-31" TabIndex="2" dateformat="MM/dd/yyyy" displaydateformat="MM/dd/yyyy"
                                                    ToolTip="Please enter a valid End Date in (mm/dd/yyyy) format.">
                                                    <Calendar runat="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False"
                                                        ViewSelectorText="x">
                                                    </Calendar>
                                                    <DateInput ID="DateInput2" onclick="showEndPopup()" runat="server" TabIndex="2">
                                                    </DateInput>
                                                    <DatePopupButton ImageUrl="" HoverImageUrl="" TabIndex="2"></DatePopupButton>
                                                </telerik:RadDatePicker>
                                                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="endDate" ValidationGroup="messageQRpt"
                                                    ErrorMessage="Please enter a valid end date." Width="1px">*</asp:RequiredFieldValidator>
                                                <asp:CompareValidator ID="comvEndDate" runat="server" ControlToCompare="startDate" ValidationGroup="messageQRpt"
                                                    ControlToValidate="endDate" ErrorMessage="" Display="Dynamic"
                                                    Operator="GreaterThanEqual" Type="Date">End date should be greater than or equal to start date</asp:CompareValidator>
                                                <asp:CompareValidator ID="compareValidatorEndDate" runat="server" ControlToValidate="endDate" ValidationGroup="messageQRpt"
                                                        Display="Dynamic" Operator="LessThanEqual" Type="Date">End date should be lesser than or equal to today</asp:CompareValidator>

                                            </td>
                                            <td class="h4titletext" style="padding-left: 15px; vertical-align: middle" align="right">
                                                <b>Authorizer:</b>
                                            </td>
                                            <td>
                                                <telerik:RadComboBox ID="ddlAuthorizer" runat="server" Width="200px" DataTextField="DataTextField"
                                                    DataValueField="DataValueField" AutoPostBack="false" MarkFirstMatch="true" TabIndex="5"
                                                    AllowCustomText="true" Filter="Contains">
                                                </telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h4titletext" style="padding-left: 15px; vertical-align: middle" align="right">
                                                <b>Patient:</b>
                                            </td>
                                            <td>
                                                <table width="100%" border="0" cellspacing="0" cellpadding="5">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblPatient" Text="" runat="server"></asp:Label>
                                                            <asp:LinkButton ID="lnkSelectPatient" runat="server" Text="Search Patient" OnClick="lnkSelectPatient_Click"
                                                                TabIndex="3" />
                                                            <asp:HiddenField runat="server" ID="hdnPatientID" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td class="h4titletext" style="padding-left: 15px; vertical-align: middle" align="right">
                                                <b>Originator:</b>
                                            </td>
                                            <td>
                                                <telerik:RadComboBox ID="ddlOriginator" runat="server" Width="200px" DataTextField="DataTextField"
                                                    DataValueField="DataValueField" AutoPostBack="false" MarkFirstMatch="true" TabIndex="6"
                                                    AllowCustomText="true" Filter="Contains">
                                                </telerik:RadComboBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btnstyle" OnClientClick="clearPatientInfo()" OnClick="btnSearch_Click" ValidationGroup="messageQRpt" CausesValidation="true"
                                                    TabIndex="7" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btnstyle" OnClick="btnClear_Click"
                                                    TabIndex="8" OnClientClick="btnClear_OnClientClick" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr class="h4title">
                            <td colspan="2">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnBack" Text="Back" CssClass="btnstyle" runat="server" OnClick="btnBack_Click"
                                                TabIndex="9" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnConfirm" Text="Confirm" CssClass="btnstyle" runat="server" OnClick="btnConfirm_Click"
                                                Enabled="false" TabIndex="10" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button ID="hiddenSelect" CssClass="btnstyle" runat="server" Text="" Style="display: none"
                                    OnClick="hiddenSelect_Click" UseSubmitBehavior="false" />
                                <telerik:RadAjaxManagerProxy ID="RadAjaxManager1" runat="server">
                                    <AjaxSettings>
                                        <telerik:AjaxSetting AjaxControlID="btnSearch">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="pnlMessage" />
                                                <telerik:AjaxUpdatedControl ControlID="grdMessageQueue" />
                                                <telerik:AjaxUpdatedControl ControlID="btnConfirm" />
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                        <telerik:AjaxSetting AjaxControlID="btnConfirm">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="grdMessageQueue" />
                                                <telerik:AjaxUpdatedControl ControlID="pnlMessage" />
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                        <telerik:AjaxSetting AjaxControlID="hiddenSelect">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="btnConfirm" />
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                        <telerik:AjaxSetting AjaxControlID="btnClear">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="pnlMsgQueueSearch" />
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                    </AjaxSettings>
                                </telerik:RadAjaxManagerProxy>
                                <telerik:RadGrid ID="grdMessageQueue" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                    GridLines="None" DataSourceID="msgQueueObjDS" ShowStatusBar="false" AllowAutomaticUpdates="false"
                                    AllowMultiRowSelection="false" EnableViewState="true" AllowPaging="True" AllowSorting="False"
                                    Style="width: 100%; vertical-align: top" PageSize="50" AutoGenerateColumns="False"
                                    OnItemDataBound="grdMessageQueue_ItemDataBound" TabIndex="11">
                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                    <MasterTableView GridLines="None" NoMasterRecordsText="No records found for the search criteria"
                                        Style="width: 100%; vertical-align: top" CommandItemDisplay="None" ClientDataKeyNames="ScriptMessageID,DisplayType,ErrorConfirmedByName,ErrorConfirmedDateTime,DisplayStatus"
                                        DataKeyNames="ScriptMessageID,DisplayType,ErrorConfirmedByName,ErrorConfirmedDateTime,RxID,DisplayStatus,IsVIPPatient,IsRestrictedPatient">
                                        <CommandItemTemplate>
                                        </CommandItemTemplate>
                                        <HeaderStyle Font-Bold="true" />
                                        <Columns>
                                            <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn">
                                                <ItemStyle Width="30px" HorizontalAlign="Center"></ItemStyle>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemTemplate>
                                                    <input id="rbSelect" runat="server" type="radio" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn SortExpression="ErrorConfirmedByName" HeaderImageUrl="~/images/warning-16x16.png">
                                                <ItemTemplate>
                                                    <asp:Image ID="imgAlert" runat="server" ImageUrl="images/shim.gif" AlternateText="Attention needed. Unconfirmed Rx error."
                                                        Visible='<%# string.IsNullOrEmpty((Eval("ErrorConfirmedByName").ToString())) %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="12px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="Created" HeaderText="Created Date">
                                                <ItemStyle Width="150px" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Updated" HeaderText="Last Updated Date">
                                                <ItemStyle Width="150px" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn HeaderText="Type" SortExpression="DisplayType">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDisplayType" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("DisplayType")) %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="75px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Status" SortExpression="DisplayStatus">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDisplayStatus" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("DisplayStatus")) %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="75px" />
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="PatientName" UniqueName="ImgColumn" HeaderText="Patient">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn HeaderText="Message" SortExpression="ErrorText">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatusMessage" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("ErrorText")) %>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="AuthorizerName" HeaderText="Authorizer">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="OriginatorName" HeaderText="Originator">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="ScriptMessageID" UniqueName="ScriptMessageID"
                                                Visible="false">
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                    <ClientSettings>
                                        <ClientEvents OnRowClick="grdMessageQueue_OnRowClick" />
                                        <Selecting AllowRowSelect="True"></Selecting>
                                    </ClientSettings>
                                </telerik:RadGrid>
                                <asp:ObjectDataSource ID="msgQueueObjDS" runat="server" SelectMethod="LoadMessageQueueTx"
                                    TypeName="eRxWeb.EPSBroker" DataObjectTypeName="eRxWeb.ePrescribeSvc.MessageQueue" SortParameterName="sortExpression"
                                    EnableCaching="true" CacheDuration="30" CacheKeyDependency="msgQueueCacheKey"
                                    OnSelected="msgQueueObjDS_Selected" OnSelecting="msgQueueObjDS_Selecting">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="licenseID" SessionField="LicenseID" />
                                        <asp:SessionParameter Name="siteID" SessionField="SiteID" />
                                        <asp:ControlParameter Name="statusID" ControlID="ddlView" PropertyName="SelectedValue" />
                                        <asp:ControlParameter Name="startDate" ControlID="startDate" PropertyName="SelectedDate" />
                                        <asp:ControlParameter Name="endDate" ControlID="endDate" PropertyName="SelectedDate" />
                                        <asp:ControlParameter Name="authorizerID" ControlID="ddlAuthorizer" PropertyName="SelectedValue" />
                                        <asp:ControlParameter Name="originatorID" ControlID="ddlOriginator" PropertyName="SelectedValue" />
                                        <asp:Parameter Name="patientID" DefaultValue="" />
                                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                        <asp:SessionParameter Name="userID" SessionField="UserID" Type="String" />
                                        <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                      </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <ePrescribe:PatientSearchWithResults ID="ucPatientSearchWithResults" runat="server"
                                    OnPatientSelectEvent="patientSelectEvent" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>

<%--    <asp:Panel ID="panelMsgDetailHeader" CssClass="accordionHeader" runat="server">
        <table cellspacing="0" cellpadding="0" width="95%" border="0">
            <tr>
                <td class="accordionHeaderText">
                    Message Detail
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="panelMsgDetail" CssClass="accordionContent" runat="server" Visible="False">
        <table width="95%">
            <tr id="trPharmacyNotes" runat="server" Visible="False">
                <td>
                    <b>Pharmacy Notes:</b>
                </td>
                <td>
                    <asp:Label runat="server" ID="lblPharmacyNotes"></asp:Label>
                </td>
            </tr>
            <tr id="trPharmacyNotesHr" runat="server" Visible="False">
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Patient:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Address:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtAddress" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>City:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtCity" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>State:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtState" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>ZIP:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtZIP" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Phone:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtPhone" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Gender:</b>
                </td>
                <td>
                    <asp:Label ID="lblPtGender" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Rx Date:</b>
                </td>
                <td>
                    <asp:Label ID="lblRxDate" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Rx:</b>
                </td>
                <td>
                    <asp:Label ID="lblRxDesc" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Sig:</b>
                </td>
                <td>
                    <asp:Label ID="lblSig" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Quantity:</b>
                </td>
                <td>
                    <asp:Label ID="lblQty" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Refills:</b>
                </td>
                <td>
                    <asp:Label ID="lblRefills" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>DAW:</b>
                </td>
                <td>
                    <asp:Label ID="lblDAW" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Notes:</b>
                </td>
                <td>
                    <asp:Label ID="lblNotes" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    <b>Pharmacy:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmName" runat="server"></asp:Label>
                </td>
            </tr>
            <asp:Panel ID="panelPharmicistName" runat="server" Visible="false">
                <tr>
                    <td>
                        <b>Pharmacist:</b>
                    </td>
                    <td>
                        <asp:Label ID="lblPharmacist" runat="server"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td>
                    <b>NABP:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmNABP" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Address:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmAddress" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>City:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmCity" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>State:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmState" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>ZIP:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmZIP" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Phone:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmPhone" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <b>Fax:</b>
                </td>
                <td>
                    <asp:Label ID="lblPharmFax" runat="server"></asp:Label>
                </td>
            </tr>
            <asp:Panel ID="panelConfirmedBy" runat="server" Visible="false">
                <tr>
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Confirmed by:</b>
                    </td>
                    <td>
                        <asp:Label ID="lblConfirmedBy" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <b>Confirmed on:</b>
                    </td>
                    <td>
                        <asp:Label ID="lblConfirmedDateTime" runat="server"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </asp:Panel>
    <br />--%>
     <div id="divHideTools_Help" runat="server">  
</div>

    </asp:Content>

