<%@ Page Title="Privacy Patient Search" Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" CodeBehind="PrivacyPatientSearch.aspx.cs" Inherits="eRxWeb.PrivacyPatientSearch" %>
<%@ Register Src="Controls/PatientSearch.ascx" TagName="PatientSearch" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">

            function patientSelected(source, eventArgs) {
                GridSingleSelect(eventArgs.getDataKeyValue("PatientID").toString(), "PatientID", "<%= grdViewPatients.MasterTableView.ClientID %>", false);
                patientSelectRadio(eventArgs.getDataKeyValue("PatientID").toString(), eventArgs.getDataKeyValue("Name").toString()); 
            }

            function patientSelectRadio(patientID, patientName) {
                var btnManagePrivacyRestrictions = document.getElementById("<%= btnManagePrivacyRestrictions.ClientID %>");
                GridSingleSelect(patientID, "PatientID", "<%= grdViewPatients.MasterTableView.ClientID %>", true)

                if (btnManagePrivacyRestrictions)
                    btnManagePrivacyRestrictions.disabled = false;

                var hiddenPatientID = document.getElementById("<%=hiddenPatientID.ClientID %>");
                var hiddenPatientName = document.getElementById("<%=hiddenPatientName.ClientID %>");   
                if (hiddenPatientID != null) {
                    hiddenPatientID.value = patientID;
                    hiddenPatientName.value = patientName;
                }  
                RefreshPatientHeader(patientID);
            }

            function enableAliasFields(obj)
            {               
                if (obj.checked)
                {
                    document.getElementById("<%= txtAliasFirstName.ClientID%>").removeAttribute("disabled");
                    document.getElementById("<%= txtAliasLastName.ClientID%>").removeAttribute("disabled");
                    document.getElementById("<%= rqAliasFirstName.ClientID%>").enabled = true;
                    document.getElementById("<%= rqAliasLastName.ClientID%>").enabled = true;
                }
                else
                {
                    document.getElementById("<%= txtAliasFirstName.ClientID%>").disabled = "disabled";
                    document.getElementById("<%= txtAliasLastName.ClientID%>").disabled = "disabled";
                    document.getElementById("<%= rqAliasFirstName.ClientID%>").enabled = false;
                    document.getElementById("<%= rqAliasLastName.ClientID%>").enabled = false;
                }
                
            }

        </script>
    </telerik:RadCodeBlock>   

    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr class="h1title">
            <td class="Phead indnt">
                Manage Privacy Restrictions
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
                <asp:Panel ID="pnlSearchPatient" runat="server">                
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                        <tr class="h3title">
                            <td>
                                <asp:Panel ID="searchPanel" runat="server">                                    
                                    <table border="0" cellpadding="0" cellspacing="5" style="display: inline">
                                        <tr>
                                            <td align="left" valign="middle">
                                                <asp:Panel ID="baseSearch" runat="server" Wrap="false" Direction="LeftToRight" DefaultButton="btnSearch">
                                                    <table border="0" cellpadding="0" cellspacing="5">
                                                           <tr>
                                                               <asp:Label ID="errorMessage" runat="server" style="color: red" ></asp:Label>
                                                           </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblLastName" runat="server" Text="Last Name: "></asp:Label>
                                                                <telerik:RadTextBox ID="txtLastName" runat="server" EnableEmbeddedSkins="false" Skin="Allscripts"
                                                                    Width="82px" ToolTip="Enter patient's last name" TabIndex="1">
                                                                </telerik:RadTextBox>
                                                                <asp:Label ID="lblFirstName" runat="server" Text="First Name: "></asp:Label>
                                                                <telerik:RadTextBox ID="txtFirstName" runat="server" EnableEmbeddedSkins="false"
                                                                    Skin="Allscripts" Width="82px" ToolTip="Enter patient's first name" TabIndex="2">
                                                                </telerik:RadTextBox>
                                                                <asp:Label ID="lblDOB" runat="server" Text="DOB: "></asp:Label>
                                                                <telerik:RadDateInput ID="rdiDOB" runat="server" DateFormat="MM/dd/yyyy" DisplayDateFormat="MM/dd/yyyy"
                                                                    Skin="Allscripts" Width="80px" ToolTip="Enter Patient's Date of Birth (mm/dd/yyyy)"
                                                                    EnableEmbeddedSkins="false" EmptyMessage="mm/dd/yyyy" EmptyMessageStyle-ForeColor="GrayText"
                                                                    EmptyMessageStyle-Font-Italic="true" TabIndex="3">
                                                                </telerik:RadDateInput>
                                                                <asp:Label ID="lblPatientID" runat="server" Text="Patient ID: "></asp:Label>
                                                                <telerik:RadTextBox ID="txtPatientID" runat="server" EnableEmbeddedSkins="false"
                                                                    Skin="Allscripts" Width="75px" ToolTip="Enter patient's MRN" TabIndex="4">
                                                                </telerik:RadTextBox>
                                                                &nbsp;&nbsp;
                                                                <asp:Button ID="btnSearch" runat="server" CssClass="btnstyle" OnClick="btnSearch_Click"
                                                                    Text="Search" ToolTip="Search patient" CausesValidation="false" TabIndex="5" />
                                                                 &nbsp;&nbsp;
                                                                <asp:Button ID="btnManagePrivacyRestrictions" runat="server" CssClass="btnstyle" OnClick="btnManagePrivacyRestrictions_Click"
                                                                    Text="Manage Privacy Restrictions" CausesValidation="false" Enabled="false" />                                                             
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>                               
                            </td>
                        </tr>
                    </table>                
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <ePrescribe:Message ID="ucMessage" runat="server" MessageText="" Visible="false" />
       </tr>
        <tr>
            <td>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>                            
                            <input id="hiddenPatientID" runat="server" type="text" style="display: none;" />
                            <input id="hiddenPatientName" runat="server" type="text" style="display: none;" />                               
                            <telerik:RadGrid ID="grdViewPatients" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                GridLines="None" OnItemDataBound="grdViewPatients_RowDataBound" AllowAutomaticUpdates="False"
                                AllowMultiRowSelection="false" EnableViewState="true" AllowPaging="false" AllowSorting="True"
                                Style="width: 100%; vertical-align: top;" AutoGenerateColumns="False">
                                <PagerStyle Mode="NextPrevAndNumeric" />
                                <MasterTableView GridLines="None" NoMasterRecordsText="No patients found" Style="width: 100%;
                                    vertical-align: top" CommandItemDisplay="None" ClientDataKeyNames="PatientID, StatusID, Name, IsVIPPatient,IsRestrictedPatient"
                                    DataKeyNames="PatientID, StatusID, Name, IsVIPPatient,IsRestrictedPatient">
                                    <CommandItemTemplate>
                                    </CommandItemTemplate>
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn">
                                            <ItemStyle Width="30px" HorizontalAlign="Left"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemTemplate>
                                                <input id="rbSelect" runat="server" type="radio" />                                                
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn DataField="MRN" SortExpression="MRN" HeaderText="Patient ID">
                                            <ItemStyle Width="65px"></ItemStyle>
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Name" UniqueName="ImgColumn" SortExpression="Name" HeaderText="Patient Name">
                                            <ItemStyle Width="210px"></ItemStyle>
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DOB" HeaderText="DOB">
                                            <ItemStyle Width="90px"></ItemStyle>
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Phone" HeaderText="Phone Number">
                                            <ItemStyle Width="95px"></ItemStyle>
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Address" HeaderText="Street Address">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="IsVIPPatient" HeaderText="IsVIPPatient" Visible="false">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <ClientEvents OnRowClick="patientSelected" />
                                    <Selecting AllowRowSelect="True"></Selecting>
                                </ClientSettings>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="PatObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                TypeName="Allscripts.Impact.CHPatient" SelectMethod="PrivacyPatientSearch">
                                <SelectParameters>
                                    <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                    <asp:Parameter Name="LastName" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                                    <asp:Parameter Name="FirstName" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                                    <asp:Parameter Name="DateOfBirth" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                                    <asp:Parameter Name="ChartID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                    <asp:Parameter Name="includeInactive" Type="boolean" DefaultValue="false" />
                                    <asp:Parameter Name="HasVIPPatients" Type="Boolean" DefaultValue="false" />
                                    <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="ScheduledPatObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                TypeName="Allscripts.Impact.Patient" SelectMethod="GetScheduledList">
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

<asp:Panel Style="display: none;" ID="RestrictedUser" runat="server">
    <div id="div1" class="overlaymain" style="height: 550px; width: 800px;">
    <div style="font-weight:bolder; font-size:larger;">
        <div class="overlayTitle">
               <asp:Label CssClass="overlayTitleText" style="font-size: medium" ID="lblSelectedPatientHeader" runat="server" ></asp:Label>
           </div>
        <ePrescribe:Message ID="ucPrivacyMessageStatus" runat="server" MessageText="" Visible="false" />
        <div style="font-size:larger;" class="overlayContent">
               <asp:CheckBox Text="" runat="server" ID="chkAlias" OnClick="enableAliasFields(this)" />  &nbsp;&nbsp;
                <br />
               <asp:Label ID="lblAliasLastName" runat="server" Text="Alias Last Name:" style="vertical-align: middle;margin-left: 20px;"></asp:Label> <asp:TextBox ID="txtAliasLastName" disabled="disabled" runat="server"></asp:TextBox> 
               <asp:RequiredFieldValidator ID="rqAliasLastName" Enabled="false" runat="server" ControlToValidate="txtAliasLastName" Text="*" ValidationGroup="reqAlias"></asp:RequiredFieldValidator>
               <asp:Label ID="lblAliasFirstName" runat="server" Text="Alias First Name:" style="vertical-align: middle;"></asp:Label> <asp:TextBox ID="txtAliasFirstName" runat="server" disabled="disabled"></asp:TextBox> 
               <asp:RequiredFieldValidator ID="rqAliasFirstName" Enabled="false" runat="server" ControlToValidate="txtAliasFirstName" Text="*" ValidationGroup="reqAlias"></asp:RequiredFieldValidator><br />
               <asp:ValidationSummary ID="vsAliasName" DisplayMode="BulletList" Enabled ="true" EnableClientScript="true"  HeaderText="*Please enter alias last name and first name." style="text-align:start" ValidationGroup="reqAlias" runat="server"/>
            <div>
                <telerik:RadAjaxManager ID="RadAjaxManagerPrivacyPatSearch" runat="server">
                    <AjaxSettings>
                        <telerik:AjaxSetting AjaxControlID="rgRestrictedUser">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="rgRestrictedUser"/>
                                <telerik:AjaxUpdatedControl ControlID="ucPrivacyMessageStatus"/>
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                        <telerik:AjaxSetting AjaxControlID="btnRestrictedUserSave">
                            <UpdatedControls>
                                <telerik:AjaxUpdatedControl ControlID="rgRestrictedUser"/>
                                <telerik:AjaxUpdatedControl ControlID="ucPrivacyMessageStatus"/>
                                <telerik:AjaxUpdatedControl ControlID="grdViewPatients" />
                            </UpdatedControls>
                        </telerik:AjaxSetting>
                    </AjaxSettings>
                </telerik:RadAjaxManager>
            
            </div>
            <div style="overflow: auto;max-height:400px;width: 100%;">
                <br />
            <asp:Label ID="lblSelectedPatient" runat="server"></asp:Label>
            <telerik:RadGrid ID="rgRestrictedUser" runat="server" EnableEmbeddedSkins="false"
                Skin="Allscripts" ShowStatusBar="false" AllowAutomaticUpdates="False" AllowMultiRowSelection="true"  OnItemDataBound="rgRestrictedUser_ItemDataBound"
                EnableViewState="true" AllowPaging="True" AllowSorting="false" Style="width: 100%;
                margin-top: 5px;" PageSize="50" AutoGenerateColumns="False" OnPageIndexChanged="rgRestrictedUser_PageIndexChanged">
                <PagerStyle Mode="NextPrevAndNumeric" />
                <ClientSettings>
                    <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                </ClientSettings>
                <MasterTableView GridLines="None" NoMasterRecordsText="No Records Found" Style="width: 100%"
                    CommandItemDisplay="None" DataKeyNames="UserGUID, UserFullName" ClientDataKeyNames="UserGUID, UserFullName">
                    <HeaderStyle Font-Bold="true" Font-Size="13px" />
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="MyClientSelectColumn">
                        </telerik:GridClientSelectColumn>
                        <telerik:GridBoundColumn DataField="LastName" HeaderText="Last Name" SortExpression="LastName">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="FirstName" HeaderText="First Name" SortExpression="FirstName">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="UserFullName" HeaderText="User" SortExpression="UserFullName">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings>
                    <Selecting AllowRowSelect="true" EnableDragToSelectRows="true" />
                    <Selecting AllowRowSelect="true" />
                    <Scrolling AllowScroll="true" />
                </ClientSettings>
            </telerik:RadGrid>
            </div>
        </div>
        <br />
        <div class="overlayFooter">
         <asp:Button ID="btnRestrictedUserSave" runat="server" Text="Save" Width="120px" ValidationGroup="reqAlias" CssClass="btnstyle btnStyleAction" ToolTip="Click to save the selected users for restriction" OnClick="btnRestrictedUserSave_Click" />
          <asp:Button ID="btnRestrictedUserSaveNClose" runat="server" Text="Save and Close" ValidationGroup="reqAlias" CssClass="btnstyle" ToolTip="Click to save the selected users for restriction and close the window" Width="120px" OnClick="btnRestrictedUserSaveNClose_Click" />
          <asp:Button ID="btnRestrictedUserCancel" runat="server" Text="Cancel" Width="120px" Visible="true" ToolTip="Close the window" CssClass="btnstyle" OnClick="btnRestrictedUserCancel_Click" />
        </div>
    </div>
    </div>
    </asp:Panel>
<asp:Button ID="hiddenFA" runat="server" Style="display: none;" />
<ajaxToolkit:ModalPopupExtender ID="mpeRestrictedUser" runat="server" BehaviorID="mpeRestrictedUser"
   DropShadow="false" BackgroundCssClass="modalBackground" 
    TargetControlID="hiddenFA" PopupControlID="RestrictedUser">
</ajaxToolkit:ModalPopupExtender>
</asp:Content>
