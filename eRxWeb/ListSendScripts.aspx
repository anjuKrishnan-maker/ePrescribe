<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.ListSendScripts" Title="Task List" Codebehind="ListSendScripts.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        var prevRow;
        var savedClass;

        function onRowClick(row) {
            //alert((row.firstChild.childNodes.length ));
            var ViewDetails = document.getElementById("ctl00_ContentPlaceHolder1_btnViewScript");
            if (ViewDetails != null) {
                ViewDetails.disabled = false;
                ViewDetails.src = "images/ViewScripts.gif";

            }

            var patientID = row.cells[0].childNodes[1].value.split('|')[0];
            SelectPatient(patientID);

            if (prevRow != null) {
                prevRow.className = savedClass;
            }
            savedClass = row.className;
            row.className = 'SelectedRow';
            prevRow = row;
            cleanWhitespace(row)
            row.firstChild.childNodes[0].checked = true;
        }
        //const notWhitespace = /\S/

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
    <table border="0" cellspacing="0" cellpadding="0" width="100%">      
        <asp:Panel ID="Tasksh1Panel" runat="server">
            <tr class="h1title">
                <td>
                    <table width="100%" cellpadding="3px">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rbtnMyTask1" runat="server" AutoPostBack="True" CssClass="adminlink1"
                                    GroupName="Task2" Text="My Tasks" OnCheckedChanged="rbtnMyTask_CheckedChanged"
                                    Visible="false" />
                                <asp:RadioButton ID="rbtnSiteTask1" runat="server" AutoPostBack="True" GroupName="Task2"
                                    Text="Site Tasks" OnCheckedChanged="rbtnSiteTask_CheckedChanged" CssClass="adminlink1"
                                    Visible="false" />
                                <asp:RadioButton ID="rbtnAdminTask1" runat="server" AutoPostBack="True" GroupName="Task2"
                                    Visible="false" Text="Assistant's Tasks" OnCheckedChanged="rbtnAdminTask_CheckedChanged"
                                    Checked="false" CssClass="adminlink1" />
                                <asp:RadioButton ID="rbEPATasks1" runat="server" AutoPostBack="True" GroupName="Task2"
                                    Text="ePA Tasks" OnCheckedChanged="rbEPATasks_CheckedChanged" CssClass="adminlink1" />
                                <asp:RadioButton ID="rbPharmRefills1" runat="server" GroupName="Task2" Text="Pharmacy Tasks"
                                    Visible="false" CssClass="adminlink1" AutoPostBack="true" OnCheckedChanged="rbPharmRefills_Changed" />
                                <asp:RadioButton ID="rbSpecialtyMed1" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="Patient Access Services" OnCheckedChanged="rbSpecialtyMedTasks_CheckedChanged" CssClass="adminlink1" />
                            </td>
                            <td align="right">
                                <span class="adminlink1">Show tasks from:</span>&nbsp
                                <asp:DropDownList ID="ddlTaskSource1" runat="server" Width="170px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlTaskSource1_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:Button ID="btnPharmRefillReport1" runat="server" CssClass="btnstyle" OnClick="btnPharmRefillReport_Click"
                                    Text="Pharmacy Refill Report" Width="149px" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="Tasksh2Panel" runat="server">
            <tr class="h2title">
                <td>
                    <table width="100%" cellpadding="3px">
                        <tr>
                            <td>
                                <asp:RadioButton ID="rbtnMyTask" runat="server" AutoPostBack="True" CssClass="adminlink1"
                                    GroupName="Task2" Text="My Tasks" OnCheckedChanged="rbtnMyTask_CheckedChanged"
                                    Visible="false" />
                                <asp:RadioButton ID="rbtnSiteTask" runat="server" AutoPostBack="True" GroupName="Task2"
                                    Text="Site Tasks" OnCheckedChanged="rbtnSiteTask_CheckedChanged" CssClass="adminlink1"
                                    Visible="false" />
                                <asp:RadioButton ID="rbtnAdminTask" runat="server" AutoPostBack="True" GroupName="Task2"
                                    Visible="false" Text="Assistant's Tasks" OnCheckedChanged="rbtnAdminTask_CheckedChanged"
                                    Checked="false" CssClass="adminlink1" />
                                <asp:RadioButton ID="rbEPATasks" runat="server" AutoPostBack="True" GroupName="Task2"
                                    Text="ePA Tasks" OnCheckedChanged="rbEPATasks_CheckedChanged" CssClass="adminlink1" />
                                <asp:RadioButton ID="rbPharmRefills" runat="server" GroupName="Task2" Text="Pharmacy Tasks"
                                    Visible="false" CssClass="adminlink1" AutoPostBack="true" OnCheckedChanged="rbPharmRefills_Changed" />
                                <asp:RadioButton ID="rbSpecialtyMed" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="Patient Access Services" OnCheckedChanged="rbSpecialtyMedTasks_CheckedChanged" CssClass="adminlink1" />
                            </td>
                            <td align="right">
                                <span class="adminlink1">Show tasks from:</span>&nbsp
                                <asp:DropDownList ID="ddlTaskSource" runat="server" Width="170px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlTaskSource_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:Button ID="btnPharmRefillReport" runat="server" CssClass="btnstyle" OnClick="btnPharmRefillReport_Click"
                                    Text="Pharmacy Refill Report" Width="149px" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="Messagesh2Panel" runat="server">
            <tr class="h2title">
                <td>
                    <ePrescribe:Message ID="ucSupervisingProvider" runat="server" Visible="false" />
                    <ePrescribe:Message ID="ucTaskAlert" runat="server" Visible="false" />
                    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td>
                <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td style="width: 100%;">
                            <table border="0" cellspacing="0" cellpadding="0" width="100%">
                                <tr class="h4title">
                                    <td colspan="5">
                                        <asp:Button ID="btnSelectPat" runat="server" CssClass="btnstyle" OnClick="btnSelectPat_Click"
                                            Text="Back" Visible="False" />
                                        &nbsp;&nbsp;<asp:Button ID="btnViewScript" runat="server" CssClass="btnstyle" Enabled="False"
                                            OnClick="btnViewScript_Click" Text="View Script" ToolTip="Click here to see the details." />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td width="100%">
                            <asp:GridView ID="grdViewTasks" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" Width="104%" CaptionAlign="Left" GridLines="None"
                                EmptyDataText=" No Tasks Found" DataSourceID="TaskObjDataSource" PageSize="50"
                                OnRowDataBound="grdViewTasks_RowDataBound" DataKeyNames="PatientGUID,ProviderID,LicenseID"
                                OnRowCreated="grdViewTasks_RowCreated" onsorting="grdViewTasks_Sorting">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <input name="rdSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientGUID")) %>|<%# ObjectExtension.ToEvalEncode(Eval("ProviderID")) %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="30px" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="PatientName" HeaderText="Patient Name" SortExpression="PatientName"
                                        HeaderStyle-HorizontalAlign="Left">
                                        <ItemStyle Width="260px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ProviderName" HeaderText="Provider Name" SortExpression="ProviderName"
                                        HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="ScriptCount" HeaderText="Script Count" SortExpression="ScriptCount"
                                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                </Columns>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="TaskObjDataSource" runat="server" TypeName="Allscripts.Impact.TaskManager"
                                OldValuesParameterFormatString="original_{0}" SelectMethod="GetTaskListForNurse">
                                <SelectParameters>
                                    <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                    <asp:SessionParameter Name="TaskType" SessionField="TaskTypeForList" Type="Int32"
                                        DefaultValue="4" />
                                    <asp:Parameter Name="ProviderID" DefaultValue="" Type="String" />
                                    <asp:Parameter Name="POBID" DefaultValue="" Type="String" />
                                    <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                                    <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int32" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
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
    <asp:Button ID="hiddenSetSupervisingProvider" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeSetSupervisingProvider" runat="server" BehaviorID="mpeSetSupervisingProvider"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenSetSupervisingProvider"
        PopupControlID="panelSetSupervisingProvider" PopupDragHandleControlID="panelSetSupervisingProvider" />
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2"> 
</asp:Content>
