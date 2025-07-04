<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.DocRefillMenu" Title="Refill Request" Codebehind="DocRefillMenu.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ObsoletePARClassMappingChange.ascx" TagName="ObsoletePARClassMappingChange"
    TagPrefix="ePrescribe" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table cellspacing="0" cellpadding="0" width="100%" border="0">       
        <asp:Panel ID="Tasksh1Panel" runat="server">
            <tr class="h1title">
                <td>
                    <table width="100%" cellpadding="3px">
                        <td>
                            <asp:RadioButton ID="rbtnMyTask1" runat="server" AutoPostBack="True" Checked="True"
                                CssClass="adminlink1" GroupName="Task2" Text="My Tasks" OnCheckedChanged="rbtnMyTask_CheckedChanged" />
                            <asp:RadioButton ID="rbtnSiteTask1" runat="server" AutoPostBack="True" GroupName="Task2"
                                Text="Site Tasks" OnCheckedChanged="rbtnSiteTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbtnAdminTask1" runat="server" AutoPostBack="True" GroupName="Task2"
                                Text="Assistant's Tasks" OnCheckedChanged="rbtnAdminTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbEPATasks1" runat="server" AutoPostBack="true" GroupName="Task2"
                                Text="ePA Tasks" OnCheckedChanged="rbEPATasks_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbSpecialtyMed" runat="server" AutoPostBack="true" GroupName="Task2"
                                Text="Patient Access Services" OnCheckedChanged="rbSpecialtyMed_CheckedChanged" CssClass="adminlink1" />
                        </td>
                        <td align="right">
                            <div id="divProvider1" runat="server" visible="false">
                                <span class="adminlink1">Show tasks for:</span>&nbsp
                                <asp:DropDownList ID="ddlProvider1" runat="server" Width="170px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlProvider1_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <td align="right" style="width: 151px">
                            <asp:Button ID="btnPharmRefillReport1" runat="server" CssClass="noWrapBtnstyle" OnClick="btnPharmRefillReport_Click"
                                Text="Pharmacy Refill Report" Width="149px" />
                        </td>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="Tasksh2Panel" runat="server" Visible="false">
            <tr class="h2title">
                <td>
                    <table width="100%" cellpadding="3px">
                        <td>
                            <asp:RadioButton ID="rbtnMyTask" runat="server" AutoPostBack="True" Checked="True"
                                CssClass="adminlink1" GroupName="Task2" Text="My Tasks" OnCheckedChanged="rbtnMyTask_CheckedChanged" />
                            <asp:RadioButton ID="rbtnSiteTask" runat="server" AutoPostBack="True" GroupName="Task2"
                                Text="Site Tasks" OnCheckedChanged="rbtnSiteTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbtnAdminTask" runat="server" AutoPostBack="True" GroupName="Task2"
                                Text="Assistant's Tasks" OnCheckedChanged="rbtnAdminTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbEPATasks" runat="server" AutoPostBack="true" GroupName="Task2"
                                Text="ePA Tasks" OnCheckedChanged="rbEPATasks_CheckedChanged" CssClass="adminlink1" />
                        </td>
                        <td align="right">
                            <div id="divProvider" runat="server" visible="false">
                                <span class="adminlink1">Show tasks for:</span>&nbsp
                                <asp:DropDownList ID="ddlProvider" runat="server" Width="170px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <td align="right" style="width: 151px">
                            <asp:Button ID="btnPharmRefillReport" runat="server" CssClass="noWrapBtnstyle" OnClick="btnPharmRefillReport_Click"
                                Text="Pharmacy Refill Report" Width="149px" />
                        </td>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="Messagesh2Panel" runat="server">
            <tr class="h2title">
                <td>
                    <ePrescribe:Message ID="ucSupervisingProvider" runat="server" Visible="false" />
                    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                </td>
            </tr>
        </asp:Panel>
        <tr class="h4title">
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnBack" Text="Back" CssClass="btnstyle" runat="server" OnClick="btnBack_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="height: 447px">
                <table bordercolor="#b5c4c4" cellspacing="0" cellpadding="0" width="100%" align="center"
                    border="1">
                    <tr>
                        <td>
                            <asp:GridView ID="grdDocRefillTask" runat="server" PageSize="50" GridLines="None" EmptyDataText=" No Tasks Found"
                                DataSourceID="RefillTaskObjDataSource" CaptionAlign="Left" AutoGenerateColumns="False"
                                AllowSorting="True" AllowPaging="True" Width="100%" OnRowCreated="grdDocRefillTask_RowCreated"
                                OnDataBound="grdDocRefillTask_DataBound" 
                                DataKeyNames="patientid,patient,PhysicianId" 
                                onrowcommand="grdDocRefillTask_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="Physician" HeaderText="Provider" SortExpression="Physician" HeaderStyle-HorizontalAlign="Left">
                                        <ItemStyle Width="250px" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText ="Patient"  HeaderStyle-HorizontalAlign="Left" SortExpression="Patient">
                                        <ItemTemplate>
                                           <asp:LinkButton ID="lnkPatientName" runat="server" CausesValidation="False" 
                                           CommandName="RefillDetails" Text='<%# ObjectExtension.ToEvalEncode(Eval("Patient")) %>'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="RegisterStatus" HeaderText="Status" SortExpression="RegisterStatus"
                                        HeaderStyle-HorizontalAlign="Left">
                                        <ItemStyle Width="250px" HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="MedRequest" HeaderText="Task Count" HeaderStyle-HorizontalAlign="Center"
                                        SortExpression="MedRequest">
                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="LastCalled" HeaderText="Most Recent Task" SortExpression="LastCalled"
                                        HeaderStyle-HorizontalAlign="Left">
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                </Columns>
                                <RowStyle Height="22px" />
                            </asp:GridView>
                            <asp:ObjectDataSource ID="RefillTaskObjDataSource" runat="server" TypeName="Allscripts.Impact.Provider"
                                SelectMethod="GetRefillTaskSummary" OldValuesParameterFormatString="original_{0}">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                    <asp:SessionParameter Name="physicianID" SessionField="USERID" Type="String" />
                                    <asp:Parameter DefaultValue="N" Name="group" Type="String" />
                                    <asp:Parameter DefaultValue="" Name="patientID" Type="String" />
                                    <asp:Parameter DefaultValue="Approvals" Name="taskFilter" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Object" />
                                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
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
    <script type="text/javascript">
    </script>
    <asp:Button ID="hiddenSetSupervisingProvider" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeSetSupervisingProvider" runat="server" BehaviorID="mpeSetSupervisingProvider"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenSetSupervisingProvider"
        PopupControlID="panelSetSupervisingProvider" PopupDragHandleControlID="panelSetSupervisingProvider" />
    <ePrescribe:ObsoletePARClassMappingChange ID="ucObsoletePARClassMappingChange" runat="server"
        CurrentPage="DocRefillMenu.aspx" />
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2"> 
</asp:Content>
