<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.ViewAuditLog" MasterPageFile="~/physicianMasterPageBlank.master" Title="Audit Log" Codebehind="ViewAuditLog.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
 <asp:Panel ID= "Panel1" runat="server" Wrap="false" DefaultButton="btnSearch">
    <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
    <script language="javascript" type="text/javascript">
        function rowSelected(source, eventArgs)
        {
            togglePrivacyOverride('');
            
            var auditLogId = eventArgs.getDataKeyValue("AuditLogID").toString();
            var auditActionTitle = eventArgs.getDataKeyValue("AuditActionTitle").toString();

            if (auditLogId != null && auditActionTitle.indexOf('Report') > -1) {
                window.parent.AuditLogRowSelected({ auditLogId: auditLogId });
            }
            else {
                window.parent.AuditLogRowSelected({ auditLogId: null });
            }
        }
        function showStartPopup()
        {
            $find("<%= startDate.ClientID %>").showPopup();
        }
        function showEndPopup()
        {
            $find("<%= endDate.ClientID %>").showPopup();
        }

        function OnClientItemsRequesting(sender, eventArgs) 
        {
            if (eventArgs.get_text().length == 0)
                eventArgs.set_cancel(true)
            else
                eventArgs.set_cancel(false);
        }

        function requestStart(sender, eventArgs) 
        {
             
            if (eventArgs.get_eventTarget().indexOf("comboExport") != -1) {
                  document.getElementById("divLoading").style.display = "none";
                eventArgs.set_enableAjax(false);
            }
        }
        
        var column = null;
        function MenuShowing(sender, args) {
        
            if (column == null)
                return;
            var menu = sender;
            var items = menu.get_items();
            if (column.get_dataType() == "System.String") {
                var i = 0;
                while (i < items.get_count()) {
                    if (!(items.getItem(i).get_value() in { 'NoFilter': '', 'Contains': '', 'EqualTo': '' })) {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(false);
                    }
                    else {
                        var item = items.getItem(i);
                        if (item != null)
                            item.set_visible(true);
                    }
                    i++;
                }
            }
            if (column.get_dataType() == "System.DateTime") {
                var j = 0;
                
                while (j < items.get_count()) {
                    if (!(items.getItem(j).get_value() in { 'NoFilter': '', 'GreaterThan': '', 'LessThan': '', 'EqualTo': '' })) {
                        var item = items.getItem(j);
                        if (item != null)
                            item.set_visible(false);
                    }
                    else {
                        var item = items.getItem(j);
                        if (item != null)
                            item.set_visible(true);
                    }
                    j++;
                }
            }
            column = null;
        }

        function filterMenuShowing(sender, eventArgs) {
            column = eventArgs.get_column();
        }

        function clearPatientFilter() 
        {
            var combo = $find("<%= ddlPatient.ClientID %>");

            if (combo != null)
            {
                combo.clearSelection();
            }
        }

        function clearUserFilter() 
        {
            var combo = $find("<%= ddlUser.ClientID %>");

            if (combo != null) {
                combo.clearSelection();
            }
        }
    </script>
    </telerik:RadCodeBlock>
   
    <div>
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td>
                    <table border="0" width="100%" cellspacing="0">
                        <tr>
                            <td class="h1title indnt Phead">
                                Audit Log
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:ValidationSummary ID="validationSummary" runat="server" />
                            </td>
                        </tr>
                        <tr class="h3title">
                            <td>
                                <table>
                                    <tr>
                                        <td class="h4titletext" style="padding-left:5px; vertical-align:middle; width: 75px" align="right">
                                            <b>Start Date:</b>
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="startDate" Style="vertical-align: middle;" runat="server" MinDate="2011-01-01" MaxDate="2020-12-31">
                                                <Calendar ShowRowHeaders="false"></Calendar>
                                                <DateInput ID="DateInput1" onclick="showStartPopup()" runat="server"></DateInput>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td class="h4titletext" style="padding-left:25px; vertical-align:middle" align="right">
                                            <b>End Date:</b>
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="endDate" Style="vertical-align: middle;" runat="server" ShowRowHeaders="false" MinDate="2011-01-01" MaxDate="2020-12-31">
                                                <Calendar ShowRowHeaders="false"></Calendar>
                                                <DateInput ID="DateInput2" onclick="showEndPopup()" runat="server"></DateInput>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td></td>
                                    </tr>                                
                                    <tr>
                                        <td class="h4titletext" style="padding-left:5px; vertical-align:middle; width: 75px" align="right">
                                            <b>User:</b>
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlUser" runat="server" AllowCustomText="false" MarkFirstMatch="true" AutoPostBack="false" DropDownWidth="300px" Width="200px" EnableItemCaching="true" 
												DataTextField="Name" DataValueField="UserGUID" EmptyMessage="Select a User" EnableLoadOnDemand="true" OnItemsRequested="ddlUser_ItemsRequested"></telerik:RadComboBox>
                                            &nbsp<img src="images/Delete.gif" onclick="javascript:clearUserFilter()" alt="Click to clear user filter" style="cursor:pointer" />
                                        </td>
                                        <td class="h4titletext" style="padding-left:15px; vertical-align:middle" align="right"><b>Patient Last Name:</b></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlPatient" runat="server" AllowCustomText="false" MarkFirstMatch="false" AutoPostBack="false" DropDownWidth="400px" Width="400px" OnClientItemsRequesting="OnClientItemsRequesting" 
												DataTextField="DisplayValue" DataValueField="PatientID" EmptyMessage="Enter Patient Last Name" EnableLoadOnDemand="true" OnItemsRequested="ddlPatient_ItemsRequested" ShowToggleImage="false"></telerik:RadComboBox>
                                            &nbsp<img src="images/Delete.gif" onclick="javascript:clearPatientFilter()" alt="Click to clear patient filter" style="cursor:pointer" />
                                            
                                        </td>
                                       
                                    </tr>
                                    <tr>
                                        <td class="h4titletext" style="padding-left:5px; vertical-align:middle; width: 75px; white-space:nowrap" align="right"">
                                            <b>Show Events:</b>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="chkPatientFilter" runat="server" Text="Patient" Checked="true" ToolTip="Check to show audit events related to patients. Ex: Patient Added" />
                                            <asp:CheckBox ID="chkAccountFilter" runat="server" Text="Account" ToolTip="Check to show audit events related to this account. Ex: DUR Settings Modified" />
                                            <asp:CheckBox ID="chkUserFilter" runat="server" Text="User" ToolTip="Check to show audit events related to users. Ex: User Added" />
                                        </td>
                                        <td></td>
                                         <td>
											<asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btnstyle" OnClick="btnSearch_Click" Width="150px" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <ePrescribe:Message runat="server" ID="ucMessage" Visible="false" />
                            </td>
                        </tr>
                        <tr class="h4title">
                            <td>
                                <table width="100%">
                                    <tr>
                                        <td><asp:Button ID="btnBack" Text="Back" CssClass="btnstyle" runat="server" OnClick="btnBack_Click"/></td>
                                        <td align="right">
                                            <telerik:RadComboBox ID="comboExport" runat="server" AllowCustomText="false" 
                                                MarkFirstMatch="true" NoWrap="true" AutoPostBack="true" Visible="false"
                                                onselectedindexchanged="comboExport_SelectedIndexChanged">
                                                <Items>
                                                    <telerik:RadComboBoxItem Text="Export" Value="Default"/>
                                                    <telerik:RadComboBoxItem ImageUrl="images/Excel-icon.png" Text="Export to Excel" Value="Excel" />
                                                    <telerik:RadComboBoxItem ImageUrl="images/pdf.jpg" Text="Export to PDF" Value="PDF" />
                                                </Items>
                                            </telerik:RadComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="height:400px">
                            <td>
                               <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" ClientEvents-OnRequestStart="requestStart">                                   
                                    <ClientEvents OnRequestStart="requestStart" />
                                    <AjaxSettings>
                                        <telerik:AjaxSetting AjaxControlID="grdAuditLog">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="grdAuditLog" />
                                                <telerik:AjaxUpdatedControl ControlID="lblGridResultsStatus"/>
                                              
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                        <telerik:AjaxSetting AjaxControlID="btnSearch">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="grdAuditLog" />
                                                <telerik:AjaxUpdatedControl ControlID="lblGridResultsStatus"/>
                                                <telerik:AjaxUpdatedControl ControlID="ucMessage"/>
                                                <telerik:AjaxUpdatedControl ControlID="validationSummary"/>
                                                <telerik:AjaxUpdatedControl ControlID="comboExport" />
                                              
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                        <telerik:AjaxSetting AjaxControlID="comboExport">
                                            <UpdatedControls>
                                                <telerik:AjaxUpdatedControl ControlID="grdAuditLog" />
                                            </UpdatedControls>
                                        </telerik:AjaxSetting>
                                    </AjaxSettings>                        
                                </telerik:RadAjaxManager>
                                <telerik:RadGrid ID="grdAuditLog" runat="server"
                                            GridLines="None" 
                                            ExportSettings-FileName="AuditLog" ExportSettings-OpenInNewWindow="true"
                                            AllowPaging="True" AllowSorting="True" style="width:100%" PageSize="100" OnItemCommand="grdAuditLog_RowCommand"
                                            AutoGenerateColumns="False" AllowFilteringByColumn="True"  OnItemDataBound="grdAuditLog_RowDataBound"
                                    CellSpacing="0">
                                    <GroupingSettings CaseSensitive="false"></GroupingSettings>
                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                    <ExportSettings FileName="AuditLog" OpenInNewWindow="True">
                                    </ExportSettings>
                                    <ClientSettings>
                                        <DataBinding EnableCaching="true"></DataBinding>
                                        <ClientEvents 
                                            OnFilterMenuShowing="filterMenuShowing"
                                            OnRowClick="rowSelected" />
                                    </ClientSettings>
                                    <FilterMenu OnClientShowing="MenuShowing" />
                                    <MasterTableView GridLines="None" NoMasterRecordsText="No records found for the search criteria" style="width:100%" CommandItemDisplay="None" 
                                        DataKeyNames="PatientId, UserID, CreatedUTC,IsVIPPatient"
                                        ClientDataKeyNames="AuditLogID, AuditActionTitle,IsVIPPatient,IsRestrictedPatient">
                                        <EditFormSettings>
                                            <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                            </EditColumn>
                                        </EditFormSettings>
                                        <PagerStyle PageSizeControlType="RadComboBox" />
                                        <HeaderStyle Font-Bold="true" Font-Size="13px" />
                                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" 
                                            Visible="True">
                                        </RowIndicatorColumn>
                                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" 
                                            Visible="True">
                                        </ExpandCollapseColumn>
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="UserFirstAndLastName" 
                                                DataType="System.String" HeaderText="User" 
                                                SortExpression="UserFirstAndLastName">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn DataField="AuditActionTitle" DataType="System.String" 
                                                HeaderText="Action" SortExpression="AuditActionTitle">
                                                <ItemTemplate>
                                                <asp:LinkButton ID="lnkBtnAuditAction" runat="server" Text='<%# Bind("AuditActionTitle") %>' Visible="false" ></asp:LinkButton>
                                                <asp:Label ID="lblAuditAction" runat="server" Text='<%# Bind("AuditActionTitle") %>'></asp:Label>
                                                <asp:HiddenField ID="hdnFldAuditCreatedUtc" runat="server" Value='<%# ObjectExtension.ToEvalEncode(Eval("CreatedUTC","{0:yyyy-MM-dd HH:mm:ss.fff}")) %>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn DataField="PatientName" HeaderText="Patient Action Was Taken On" SortExpression="PatientName">
                                                <ItemTemplate>
                                                    <asp:Image Visible="false" ID="shieldImage" ImageUrl="~\images\PrivacyImages\sensitivehealth-global-16-x-16.png" runat="server" />
                                                    <telerik:RadToolTip Position="MiddleLeft" runat="server" ID="realNameToolTip" ShowEvent="OnClick" TargetControlID="shieldImageButton" IsClientID="true" 
                                                        HideEvent="LeaveTargetAndToolTip" Animation="Resize" ShowDelay="0" RelativeTo="Mouse"></telerik:RadToolTip>
                                                    <asp:ImageButton Visible="false" ID="shieldImageButton" ImageUrl="~\images\PrivacyImages\sensitivehealth-global-16-x-16.png" OnClick="shieldImageButton_Click" runat="server" />
                                                    <asp:Label ID="lblPatientName" runat="server" Text='<%# Bind("PatientName") %>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridBoundColumn DataField="ActionedUserFirstAndLastName" 
                                                DataType="System.String" HeaderText="User Action Was Taken On" 
                                                SortExpression="ActionedUserFirstAndLastName">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="ApplicationName" DataType="System.String" 
                                                HeaderText="Application" SortExpression="ApplicationName">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="DateTimeLocal" DataType="System.DateTime" 
                                                HeaderText="Access Date Time" SortExpression="DateTimeLocal">
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                                <asp:ObjectDataSource ID="auditLogObjDS" runat="server" SelectMethod="AuditLogGet" TypeName="Allscripts.Impact.Audit" 
                                    EnableCaching="true" CacheDuration="30" CacheKeyDependency="auditCacheKey">
                                    <SelectParameters>
                                        <asp:SessionParameter Name="licenseID" SessionField="LicenseID" />
                                        <asp:SessionParameter Name="siteID" SessionField="SiteID" />
                                        <asp:ControlParameter Name="startDateLocal" ControlID="startDate" PropertyName="SelectedDate" />
                                        <asp:ControlParameter Name="endDateLocal" ControlID="endDate" PropertyName="SelectedDate" />
                                        <asp:ControlParameter Name="userID" ControlID="ddlUser" PropertyName="SelectedValue" />
                                        <asp:ControlParameter Name="patientID" ControlID="ddlPatient" />
                                        <asp:ControlParameter Name="getPatientEvents" ControlID="chkPatientFilter" PropertyName="Checked" />
                                        <asp:ControlParameter Name="getUserEvents" ControlID="chkUserFilter"  PropertyName="Checked" />
                                        <asp:ControlParameter Name="getLicenseEvents" ControlID="chkAccountFilter"  PropertyName="Checked" />
                                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Int16" />
                                        <asp:SessionParameter Name="SessionUserID" SessionField="UserID" Type="String" />
                                        <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
      
    </div>
    
    <asp:CustomValidator ID="cvFilterOptions" runat="server" 
        ErrorMessage="Please select at least one filter event. (e.g. Patient Events, Account Events, or User Events)" 
        onservervalidate="cvFilterOptions_ServerValidate"></asp:CustomValidator>
    <asp:CustomValidator ID="cvDateSearchOptions" runat="server" 
        ErrorMessage="Please enter valid date filters." 
        onservervalidate="cvDateSearchOptions_ServerValidate"></asp:CustomValidator>
    </asp:Panel>    
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder2">
 <%--   
<asp:Panel ID="panelPrivacyOverrideReason" class="accordionContent" runat="server" Visible="false" Width="92%">
    <asp:Label ID="lblHeader" runat="server" Text="Privacy override reason"  Width="100%" CssClass="accordionHeaderText"></asp:Label>
<asp:Label ID="lblPrivacyOverrideReason" runat="server"></asp:Label>
</asp:Panel>--%>
</asp:Content>

