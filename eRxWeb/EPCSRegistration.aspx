<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Title="EPCS Registration" Inherits="eRxWeb.EPCSRegistration" CodeBehind="EPCSRegistration.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/DueDiligence.ascx" TagName="DueDiligence" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ConfirmIdentCompromise.ascx" TagName="ConfirmIdentCompromise" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">
             function enableOrDisableButtons() {
                 var providerSelected = false;
                 var grid = window.$find("<%=gvProviders.MasterTableView.ClientID %>");
                if (grid != null) {
                     var items = grid.get_dataItems();
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].get_selected()) {
                             providerSelected = true;
                             break;
                         }
                     }
                 }

                 var ddlViewOptions = document.getElementById("<%=ddlProviderView.ClientID %>");
                 var btnApproveEpcsSigning = document.getElementById("<%=btnApproveEpcsSigning.ClientID %>");
                 var btnGrantEpcsPrivilege = document.getElementById("<%=btnGrantEpcsPrivilege.ClientID %>");
                 var btnReinstateEpcs = document.getElementById("<%=btnReinstateEpcs.ClientID %>");
                 var btnSuspendEpcs = document.getElementById("<%=btnSuspendEpcs.ClientID %>");

                if (ddlViewOptions.options[ddlViewOptions.selectedIndex].value !== "ShowAllProviders") {
                    if (providerSelected) {
                        if (btnApproveEpcsSigning != null)
                        {
                           btnApproveEpcsSigning.disabled = false;
                        }
                        if (btnGrantEpcsPrivilege != null)
                        {
                            btnGrantEpcsPrivilege.disabled = false;
                        }
                        if (btnReinstateEpcs != null)
                        {
                            btnReinstateEpcs.disabled = false;
                        }
                        if (btnSuspendEpcs != null)
                        {
                            btnSuspendEpcs.disabled = false;
                        }
                    }
                    else {
                        if (btnApproveEpcsSigning != null) {
                            btnApproveEpcsSigning.disabled = true;
                        }
                        if (btnGrantEpcsPrivilege != null) {
                            btnGrantEpcsPrivilege.disabled = true;
                        }
                        if (btnReinstateEpcs != null) {
                            btnReinstateEpcs.disabled = true;
                        }
                        if (btnSuspendEpcs != null) {
                            btnSuspendEpcs.disabled = true;
                        }
                     }
                }
             }

         </script>
    </telerik:RadCodeBlock>
    
    <div id="divTitle" class="h1title indnt Phead">
    </div>
    <div class="h2title" style="padding-left: 6px; padding-top: 6px">
        <asp:Label runat="server" ID="lblTitle" CssClass="Phead indnt" Text="Registration of Electronic Providers for EPCS"></asp:Label>
    </div>
    <div class="h4title" style="padding-bottom: 4px;">
        <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnBack_OnClick" />
    </div>
    <div>
        <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
     </div>
    <div id="divMainBody" style="padding-bottom: 20px">
        <div id="divViewDropDownList" style="margin-left: 6px; margin-right: 6px; margin-bottom: 25px; margin-top: 10px;">
            <asp:DropDownList runat="server" AutoPostBack="True" OnSelectedIndexChanged="ResetView" Width="450px" ID="ddlProviderView">
                <asp:ListItem Selected="True" Text="Show All Providers" Value="ShowAllProviders"></asp:ListItem>
                <asp:ListItem Selected="False" Text="Grant EPCS Privilege - View" Value="CanEnrollAssignmentView"></asp:ListItem>
                <asp:ListItem Selected="False" Text="Approve Provider for EPCS Signing Permission - View" Value="CanPrescribeAssignmentView"></asp:ListItem>
                <asp:ListItem Selected="False" Text="Search for Providers by DEA" Value="SearchByDEA" Enabled="False"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div id="divActionButtons" style="margin-left: 6px; margin-right: 6px; margin-bottom: 5px">
            <asp:Button runat="server" ID="btnGrantEpcsPrivilege" Visible="False" Enabled="False" Text="Grant EPCS Privilege" CssClass="btnstyle" OnClick="btnGrantEpcsPrivilege_Click"/>
            <asp:Button runat="server" ID="btnApproveEpcsSigning" Visible="False" Enabled="False" Text="Approve EPCS Signing Privilege" CssClass="btnstyle" OnClick="btnApproveEpcsSigning_Click"/>
            <asp:Button runat="server" ID="btnSuspendEpcs" Visible="False" Enabled="False" Text="Suspend EPCS Privilege" CssClass="btnstyle" OnClick="btnSuspendEpcs_Click"/>
            <asp:Button runat="server" ID="btnReinstateEpcs" Visible="False" Enabled="False" Text="Reinstate EPCS privilege" CssClass="btnstyle" OnClick="btnReinstateEpcs_Click"/>
        </div>
        <div id="divDueDiligence">
            <ePrescribe:DueDiligence ID="ucDueDiligence" runat="server"/>
        </div>
        <div id="divConfirmIdentCompromise">
            <ePrescribe:ConfirmIdentCompromise ID="ucConfirmIdentCompromise" runat="server"/>
   
            <div id="deaPanel" style="margin-left: 20%" >
                <asp:Panel ID="panelDeaSearch" runat="server" Visible="False">
                    <table>
                        <tr>
                            <td><asp:Label ID="lblDEaSeach" runat="server" Text="Search DEA#"></asp:Label>&nbsp;</td>
                            <td><asp:TextBox ID="txtDeaSearch" runat="server"  AutoPostBack="True"   Onchange="onchangeEventHandler"  ></asp:TextBox></td>
                        <tr>
                            <td></td>
                            <td><asp:Button ID="btnSearch" runat="server" CssClass="btnStyle"  OnClick="btnSearch_OnClick" Text="Search" /></td>
                        </tr>
                    </table>
                    
                    
                </asp:Panel>
            </div>       
        </div>
        <div id="divProviderGrid">
            <telerik:RadGrid ID="gvProviders" runat="server" OnPreRender="ResetView" OnNeedDataSource="GetDataSource" EnableEmbeddedSkins="False" AutoGenerateColumns="False" Skin="Allscripts" CellSpacing="0" GridLines="None" Width="100%" AllowMultiRowSelection="True" AllowSorting="True" >
                <SortingSettings EnableSkinSortStyles="False"/>
                <ClientSettings>
                    <Selecting AllowRowSelect="True"/>
                    <ClientEvents OnRowSelected="enableOrDisableButtons" OnGridCreated="enableOrDisableButtons" OnRowDeselected="enableOrDisableButtons"/>

                </ClientSettings>
                <MasterTableView AllowNaturalSort="False">
                    <HeaderStyle Font-Bold="true"/>
                    <NoRecordsTemplate>No Records Available </NoRecordsTemplate>
                    <CommandItemSettings ExportToPdfText="Export to PDF" ShowAddNewRecordButton="False" ShowRefreshButton="False"></CommandItemSettings>
                    <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridClientSelectColumn FilterControlAltText="Filter colCheckboxes column" UniqueName="colCheckboxes"></telerik:GridClientSelectColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colProviderLastName column" HeaderText="Last Name" UniqueName="colProviderLastName" SortExpression="LastName">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblProviderFirstName" Text='<%# ObjectExtension.ToEvalEncode(Eval("LastName")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colProviderFirstName column" HeaderText="First Name" UniqueName="colProviderFirstName" SortExpression="FirstName">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDeaNumber" Text='<%# ObjectExtension.ToEvalEncode(Eval("FirstName")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colDeaNumber column" HeaderText="DEA Number" UniqueName="colDeaNumber" SortExpression="DEANumber">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIdCompromised" Text='<%# ObjectExtension.ToEvalEncode(("DEANumber")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colEPCSRegStatus column" HeaderText="DEA Registrant" UniqueName="colDeaStatus" SortExpression="IsDeaRegistrant" >
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDeaStatus" Text='<%# ObjectExtension.ToEvalEncode(Eval("IsDeaRegistrant")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colIdCompromised column" HeaderText="EPCS<br/> Suspended" UniqueName="colIdCompromised" SortExpression="IsIdentityCompromised">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCanEnrollPrivilege" Text='<%# ObjectExtension.ToEvalEncode(Eval("IsIdentityCompromised")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colCanEnrollPrivilege column" HeaderText="EPCS <br/> Privilege Granted" UniqueName="colCanEnrollPrivilege" SortExpression="IsCanEnrollPrivilegeGranted">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCanEnroll" Text='<%# ObjectExtension.ToEvalEncode(Eval("IsCanEnrollPrivilegeGranted")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colEPCSRegStatus column" HeaderText="EPCS<br/> Registration Status" UniqueName="colEPCSRegStatus" SortExpression="EPCSRegistrationStatus">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblRegStatus" Text='<%# ObjectExtension.ToEvalEncode(Eval("EPCSRegistrationStatus")) %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn FilterControlAltText="Filter colEPCSSigningGranted column" HeaderText="EPCS<br/> Signing Granted" UniqueName="colEPCSSigningGranted" SortExpression="IsEPCSSigningGranted">
                             <ItemTemplate>
                                 <asp:Label runat="server" ID="lblSigningGranted" Text='<%# ObjectExtension.ToEvalEncode(Eval("IsEPCSSigningGranted")) %>'></asp:Label>
                             </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Display="False" FilterControlAltText="Filter colProviderGuid column" UniqueName="colProviderGuid" Visible="False">
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hdnProviderGuid" Value='<%# ObjectExtension.ToEvalEncode(Eval("ProviderGuid")) %>'/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Display="False" FilterControlAltText="Filter colProviderGuid column" UniqueName="colUserMaySetCanEnroll" Visible="False">
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hdnUserMaySetCanEnroll" Value='<%# ObjectExtension.ToEvalEncode(Eval("UserMaySetCanEnrollPrivilege")) %>'/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Display="False" FilterControlAltText="Filter colProviderGuid column" UniqueName="colUserMaySetCanPrescribe" Visible="False">
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hdnUserMaySetCanPrescribe" Value='<%# ObjectExtension.ToEvalEncode(Eval("UserMaySetCanPrescribePrivilege")) %>'/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn Display="False" FilterControlAltText="Filter colProviderGuid column" UniqueName="colShieldUserName" Visible="False">
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hdnShieldUserName" Value='<%# ObjectExtension.ToEvalEncode(Eval("ShieldUserName")) %>'/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                    <EditFormSettings>
                        <EditColumn FilterControlAltText="Filter EditCommandColumn column" CancelImageUrl="Cancel.gif" InsertImageUrl="Update.gif" UpdateImageUrl="Update.gif"></EditColumn>
                    </EditFormSettings>
                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                </MasterTableView>
                <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                <FilterMenu EnableImageSprites="False"></FilterMenu>
                <HeaderContextMenu EnableEmbeddedSkins="False"></HeaderContextMenu>
            </telerik:RadGrid>
            <asp:ObjectDataSource ID="gvProvidersDataSource" runat="server" SelectMethod="GetProviders" TypeName="Allscripts.Impact.Provider">
                <SelectParameters>
                    <asp:Parameter Name="LicenseID" Type="String"/>
                    <asp:Parameter Name="siteID" Type="Int32"/>
                    <asp:Parameter Name="pobID" Type="String"/>
                    <asp:Parameter Name="dbID" Type="Object"/>
                </SelectParameters>
            </asp:ObjectDataSource>
        </div></div>
</asp:Content>
