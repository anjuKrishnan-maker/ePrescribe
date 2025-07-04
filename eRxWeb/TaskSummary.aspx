<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/PhysicianMasterPage.master"
    Inherits="eRxWeb.TaskSummary" Title="Task Summary" ViewStateEncryptionMode="Never"
    EnableViewStateMac="false" CodeBehind="TaskSummary.aspx.cs" %>

<%@ Import Namespace="eRxWeb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/NCPDP_EPA/ePAInitiationResponse.ascx" TagName="ePAInitiationResponse"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/ePAStatusInfo.ascx" TagName="ePAStatusInfo" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/NCPDP_EPA/ePANcpdpStatusInfo.ascx" TagName="ePANcpdpStatusInfo"
    TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">

            var prevRow;
            var savedClass;

            function toggleViewScriptButton(row) {
                var ViewDetails = $get("<%=btnViewScript.ClientID %>");

                if (ViewDetails != null) {
                    ViewDetails.disabled = false;
                    ViewDetails.src = "images/ViewScripts.gif";
                }

                $(row).find('input:radio').attr('checked', true);

                savedClass = row.className;
                row.className = 'SelectedRow ' + row.className;

                if (prevRow != null) {
                    prevRow.className = savedClass;

                    if (row.id != prevRow.id) {
                        $(prevRow).find('input:radio').attr('checked', false);
                    }
                }

                prevRow = row;
            }

            function RowCreated(sender, eventArgs) {
                var isIE = (window.navigator.userAgent.indexOf("MSIE") > 0);
                if (!isIE) {
                    if (eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].textContent == "ePA Requested") {
                        eventArgs.get_gridDataItem().get_element().cells[5].textContent = "ePA Requested";
                    }
                    else if (eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].textContent == "ePA Submitted") {
                        eventArgs.get_gridDataItem().get_element().cells[5].textContent = "ePA Form Submitted";
                    }
                    else if (eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].textContent == "ePA Ready") {
                        eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].textContent = "ePA Form Available";
                    }
            }
                else {
                    if (eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].innerText == "ePA Requested") {
                        eventArgs.get_gridDataItem().get_element().cells[5].innerText = "ePA Requested";
                    }
                    else if (eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].innerText == "ePA Submitted") {
                        eventArgs.get_gridDataItem().get_element().cells[5].innerText = "ePA Form Submitted";
                    }
                    else if (eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].innerText == "ePA Ready") {
                        eventArgs.get_gridDataItem().get_element().cells[5].childNodes[0].innerText = "ePA Form Available";
                    }
                }
            }

            function taskSelected(source, eventArgs) {
                GridSingleSelect(eventArgs.getDataKeyValue("TaskID").toString(), "TaskID", "<%= grdListePATasks.MasterTableView.ClientID %>", true);
                taskSelectRadio(eventArgs.getDataKeyValue("TaskID").toString(), eventArgs.getDataKeyValue("PatientID").toString(), eventArgs.getDataKeyValue("IsRetrospectiveEPA").toString());
            }

            function taskSelectRadio(taskID, patientID, IsRepa) {
                if (IsRepa == "True")
                {
                    document.getElementById("<%=btnViewScript.ClientID %>").value = "REMOVE TASK";
                    document.getElementById("<%=hdnFieldbtnViewScript.ClientID%>").value = "REMOVE TASK";
                }
                else
                {
                    document.getElementById("<%=btnViewScript.ClientID %>").value = "PROCESS TASK";
                    document.getElementById("<%=hdnFieldbtnViewScript.ClientID%>").value = "PROCESS TASK";

                }
                GridSingleSelect(taskID, "TaskID", "<%= grdListePATasks.MasterTableView.ClientID %>", true)
                getPatientInfo(patientID);
            }

            function getPatientInfo(patientID) {
                SelectPatient(patientID);
                var hiddenSelect = document.getElementById("<%=hiddenSelectTask.ClientID %>");
                if (hiddenSelect != null) {
                    __doPostBack(hiddenSelect.name, '')
                    document.getElementById("<%=btnViewScript.ClientID %>").disabled = false;
                }
            }

            function viewclientclick() {
                var viewButton = document.getElementById("<%=btnViewScript.ClientID %>");
                if (viewButton != null) {
                    __doPostBack(viewButton.name, '')
                }
            }

            function OnTaskMsgCancelOkay() {
                var btnTaskMsgCancelDisp = document.getElementById("<%=btnTaskMsgCancelDisp.ClientID %>");
                var btnTaskMsgCancelOkay = document.getElementById("<%=btnTaskMsgCancelOkay.ClientID %>");
                var btnTaskMsgCancelCancel = document.getElementById("<%=btnTaskMsgCancelCancel.ClientID %>");

                if (btnTaskMsgCancelDisp != null && btnTaskMsgCancelOkay != null) {
                    btnTaskMsgCancelDisp.value = "Processing...";
                    btnTaskMsgCancelDisp.disabled = "true";
                    btnTaskMsgCancelDisp.style.cursor = "wait";
                    btnTaskMsgCancelCancel.disabled = "true"

                    btnTaskMsgCancelOkay.click();
                }
            }
        </script>
    </telerik:RadCodeBlock>
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    <table cellspacing="0" cellpadding="0" width="100%" border="0">
        <tr class="h1title">
            <td>
                <table width="100%" cellpadding="3px">
                    <tr>
                        <td>
                            <asp:RadioButton ID="rbtnMyTask" runat="server" AutoPostBack="True" CssClass="adminlink1"
                                GroupName="Task" Text="My Tasks" OnCheckedChanged="rbtnMyTask_CheckedChanged" />
                            <asp:RadioButton ID="rbtnSiteTask" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="Site Tasks" OnCheckedChanged="rbtnSiteTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbtnAdminTask" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="Assistant's Tasks" OnCheckedChanged="rbtnAdminTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbPharmRefills" runat="server" GroupName="Task" Text="Pharmacy Tasks"
                                CssClass="adminlink1" AutoPostBack="true" OnCheckedChanged="rbPharmRefills_Changed" />
                            <asp:RadioButton ID="rbEPATasks" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="ePA Tasks" OnCheckedChanged="rbEPATasks_CheckedChanged" CssClass="adminlink1"
                                Checked="true" />                           
                            <asp:RadioButton ID="rbSpecialtyMed" runat="server" AutoPostBack="True" GroupName="Task"
                                Text="Patient Access Services" OnCheckedChanged="rbSpecialtyMedTasks_CheckedChanged" CssClass="adminlink1" />
                        </td>
                        <td align="right">
                            <div id="divProvider" runat="server" visible="true">
                                <span class="adminlink1">Show tasks for:</span>&nbsp
                                <asp:DropDownList ID="ddlProvider" runat="server" Width="170px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </td>
                        <td align="right" style="width: 151px">
                            <asp:Button ID="btnPharmRefillReport" runat="server" CssClass="btnstyle" OnClick="btnPharmRefillReport_Click"
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
        <tr class="h4title">
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rbtnOpenEpa" runat="server" Text="Open" AutoPostBack="true" GroupName="rbgFilterEpaTask" Checked="true" OnCheckedChanged="rbgFilterEpaTask_CheckedChanged" />
                            <asp:RadioButton ID="rbtnResolvedEpa" runat="server" Text="Resolved" AutoPostBack="true" GroupName="rbgFilterEpaTask" OnCheckedChanged="rbgFilterEpaTask_CheckedChanged" />
                            <asp:RadioButton ID="rbtnAllEpa" runat="server" Text="All" AutoPostBack="true" GroupName="rbgFilterEpaTask" OnCheckedChanged="rbgFilterEpaTask_CheckedChanged" />
                        </td>
                    </tr>
                    </table>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnBack" Text="Back" CssClass="btnstyle" runat="server" OnClick="btnBack_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnViewScript" runat="server" CssClass="btnstyle" OnClientClick="viewclientclick()"
                                OnClick="btnViewScript_Click" Text="View Script" UseSubmitBehavior="true" ToolTip="Click to see task details." />
                            <asp:HiddenField ID="hdnFieldbtnViewScript" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table cellspacing="0" cellpadding="0" align="center" style="width: 100%; border: 1px solid #b5c4c4;">
                    <tr height="<%# ((PhysicianMasterPage)Master).getTableHeight() %>">
                        <td>
                            <asp:Button ID="hiddenSelectTask" CssClass="btnstyle" runat="server" Text="" Style="display: none"
                                UseSubmitBehavior="true" OnClick="hiddenSelectTask_Click"/>
                            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
                                <AjaxSettings>
                                    <telerik:AjaxSetting AjaxControlID="hiddenSelectTask">
                                        <UpdatedControls>
                                            <telerik:AjaxUpdatedControl ControlID="pnlReload" />
                                        </UpdatedControls>
                                    </telerik:AjaxSetting>
                                </AjaxSettings>
                            </telerik:RadAjaxManager>
                            <telerik:RadGrid ID="grdRefillTask" runat="server" PageSize="50" GridLines="None"
                                Skin="Allscripts" EnableEmbeddedSkins="false" DataSourceID="RefillTaskObjDataSource"
                                AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True" OnItemDataBound="grdRefillTask_ItemDataBound"
                                OnDataBound="grdRefillTask_DataBound">
                                <MasterTableView DataKeyNames="OurPatientID, PhysicianId" GridLines="None">
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rbSelectedRow" runat="server" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridHyperLinkColumn DataNavigateUrlFields="patientid,patient,PhysicianId"
                                            DataTextField="Patient" HeaderText="Patient" DataNavigateUrlFormatString="TaskProcessor.aspx?PatientID={0}&amp;Patient={1}&amp;PhyId={2}&amp;From=TaskSummary.aspx&amp;To=TaskScriptList.aspx&amp;TaskType=ePA"
                                            DataTextFormatString="{0:c}" SortExpression="Patient" ItemStyle-Width="30%">
                                        </telerik:GridHyperLinkColumn>
                                        <telerik:GridBoundColumn DataField="Physician" HeaderText="Provider" SortExpression="Physician"
                                            ItemStyle-Width="30%">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="MedRequest" HeaderText="Task Count" HeaderStyle-HorizontalAlign="Center"
                                            SortExpression="MedRequest" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="center">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="LastCalled" HeaderText="Most Recent Task" SortExpression="LastCalled"
                                            ItemStyle-Width="20%">
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                            <telerik:RadGrid ID="grdListePATasks" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                GridLines="None" OnItemDataBound="grdListePATasks_ItemDataBound" AllowAutomaticUpdates="false"
                                AllowPaging="true" AllowSorting="true" AllowMultiRowSelection="false" EnableViewState="true"
                                AutoGenerateColumns="false" Width="100%" PageSize="50" OnItemCommand="grdListePATasks_ItemCommand"
                                OnDataBound="grdListePATasks_DataBound" DataSourceID="ePATaskObjDataSource" OnDataBinding="grdListePATasks_DataBinding" >
                                <MasterTableView GridLines="None" NoMasterRecordsText="No ePA Tasks" Style="width: 100%;
                                    vertical-align: top" CommandItemDisplay="None" DataKeyNames="TaskID,Patient,Prescriber,RxID,MedicationName,MedicationSig,RxDate,Status,PatientID,LicenseID,CreatedByID, StatusID,RefillQuantity,DaysSupply,ControlledSubstanceCode,StateControlledSubstanceCode,SenderID,IsNCPDP,IsRetrospectiveEPA,PharmacyName,PharmacyPhone,PharmacyAreaCode"
                                    ClientDataKeyNames="TaskID,PatientID,IsRetrospectiveEPA">
                                    <HeaderStyle Font-Bold="true" />
                                    <Columns>
                                        <telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn">
                                            <ItemStyle Width="30px" HorizontalAlign="Left"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemTemplate>
                                                <input id="rbSelectedePATask" runat="server" type="radio" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridBoundColumn UniqueName="Patient" HeaderText="Patient" DataField="Patient"
                                            SortExpression="Patient">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn UniqueName="Prescriber" DataField="Prescriber" HeaderText="Prescriber"
                                            SortExpression="Prescriber">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn UniqueName="MedicationSig" DataField="MedicationSig" HeaderText="Medication & Sig"
                                            SortExpression="MedicationSig">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn UniqueName="RxDate" DataField="RxDate" HeaderText="Rx Date"
                                            SortExpression="RxDate" DataFormatString="{0:MM/dd/yyyy}">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridButtonColumn UniqueName="Status" DataTextField="Status" HeaderText="Status"
                                            CommandName="ePAStatus" CommandArgument="Status" SortExpression="Status">
                                        </telerik:GridButtonColumn>
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
                                                    <telerik:RadComboBox ID="cbDestination" runat="server" Skin="Allscripts" EnableEmbeddedSkins="false"
                                                        AllowCustomText="False" ShowToggleImage="True" MarkFirstMatch="True" Width="200px">
                                                    </telerik:RadComboBox>
                                                    <asp:Label ID="lblCSMed" runat="server" ForeColor="red" Visible="false">CS</asp:Label>
                                                    <asp:Label ID="lblIsRepa" runat="server" Visible="false"></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <ClientEvents OnRowClick="taskSelected" OnRowCreated="RowCreated" />
                                    <Selecting AllowRowSelect="True"></Selecting>
                                </ClientSettings>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="RefillTaskObjDataSource" runat="server" TypeName="Allscripts.Impact.Provider"
                                SelectMethod="GetRefillTaskSummary" OldValuesParameterFormatString="original_{0}"
                                OnSelected="RefillTaskObjDataSource_Selected">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                    <asp:SessionParameter Name="physicianID" SessionField="USERID" Type="String" />
                                    <asp:Parameter Name="pobID" DefaultValue="" Type="String" />
                                    <asp:Parameter Name="group" DefaultValue="N" Type="String" />
                                    <asp:Parameter Name="patientID" DefaultValue="" Type="String" />
                                    <asp:Parameter Name="taskFilter" DefaultValue="Approvals" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Object" />
                                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="ePATaskObjDataSource" runat="server" SelectMethod="LoadePATaskList"
                                TypeName="Allscripts.Impact.ePA">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseId" SessionField="LICENSEID" Type="String" />
                                    <asp:Parameter Name="physicianID" DefaultValue="" Type="String" />
                                    <asp:Parameter Name="pobID" DefaultValue="" Type="String" />
                                    <asp:SessionParameter Name="patientID" SessionField="PATIENTID" Type="String" />
                                    <asp:SessionParameter Name="SSOMode" SessionField="SSOMode" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Object" />
                                    <asp:Parameter Name="TaskType" DefaultValue="1" Type="Int16" />
                                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
    <asp:Panel Style="display: none" ID="panelTaskStatusInfo" runat="server" class="overlaymain">
        <div align="center">
            Question Set has expired, a user must re-do the ePA process in order to get prior
            auth for the med.
            <br />
            <br />
            <br />
        </div>
        <div align="right">
            <asp:Button ID="btnTaskStatusInfo" CausesValidation="false" Width="100px" runat="server"
                Text="CLOSE" CssClass="btnstyle" />
        </div>
    </asp:Panel>
    <asp:Panel Style="display: none" ID="pnlConfirmTaskMsg" runat="server" class="overlaymain">
        <div align="center">
            <br />
            Are you sure you want to remove/process the selected row?
            <br />
            <br />
        </div>
        <div align="center">
            <asp:Button ID="bntConfirmTaskMsgOkay" CausesValidation="false" Width="75px" runat="server"
                Text="OK" CssClass="btnstyle" OnClick="bntConfirmTaskMsgOkay_Click" />
            <asp:Button ID="bntConfirmTaskMsgCancel" CausesValidation="false" Width="75px" runat="server"
                Text="Cancel" CssClass="btnstyle" />
        </div>
    </asp:Panel>
    <asp:Panel Style="display: none" ID="pnlSetSuperVisor" runat="server">
        <div style="border: solid 1px black; background-color: White; padding: 5px; width: 300px;">
            <div align="center">
                <br />
                Please select a supervising provider below.
                <br />
                <br />
                <asp:DropDownList ID="ddlSupervisingProvider" runat="server">
                </asp:DropDownList>
                <br />
                <br />
                <asp:Button ID="btnSetSuperVisor" CausesValidation="false" runat="server" Text="Set Supervising Provider"
                    CssClass="btnstyle" OnClick="bntSuperVisor_Click" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel Style="display: none" ID="pnlConfirmTaskMsgNCPDP" runat="server" class="overlaymain">
        <div align="center">
            <br />
            Are you sure you want to remove/process the selected row?
            <br />
            <br />
        </div>
        <div align="center">
            <asp:Button ID="btnConfirmTaskMsgNCPDPOK" CausesValidation="false" Width="75px" runat="server"
                Text="OK" CssClass="btnstyle" OnClick="bntConfirmTaskMsgOkay_Click" />
            <asp:Button ID="btnConfirmTaskMsgNCPDPCancel" CausesValidation="false" Width="75px"
                runat="server" Text="Cancel" CssClass="btnstyle" />
        </div>
    </asp:Panel>
    <asp:Panel Style="display: none" ID="pnlTaskMsgCancel" runat="server" class="overlaymain">
        <div align="center">
            <br />
            Are you sure you want to remove/process the selected row?
            <br />
            This will cancel the prior authorization.
            <br />
            <br />
        </div>
        <div align="center">
            <input id="btnTaskMsgCancelDisp" runat="server" type="button" value="OK" onclick="OnTaskMsgCancelOkay()"
                class="ePAbtnStyle" />
            <asp:Button ID="btnTaskMsgCancelOkay" CausesValidation="false" Width="75px" runat="server"
                Text="OK" Style="display: none;" CssClass="btnstyle" OnClick="bntConfirmTaskMsgOkay_Click" />
            <input id="btnTaskMsgCancelCancel" runat="server" type="button" value="Cancel" class="ePAbtnStyle" />
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenControl" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeSetSuperVisor" runat="server" BehaviorID="mpeSetSuperVisor"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenControl"
        PopupControlID="pnlSetSuperVisor" />
    <ajaxToolkit:ModalPopupExtender ID="mpeShowTaskStatusInfo" runat="server" BehaviorID="mpeShowTaskStatusInfo"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenControl"
        PopupControlID="panelTaskStatusInfo" CancelControlID="btnTaskStatusInfo" />
    <ajaxToolkit:ModalPopupExtender ID="mpeShowConfirmTaskMsg" runat="server" BehaviorID="mpeShowConfirmTaskMsg"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenControl"
        PopupControlID="pnlConfirmTaskMsg" CancelControlID="bntConfirmTaskMsgCancel" />
    <ajaxToolkit:ModalPopupExtender ID="mpeShowConfirmTaskMsgNCPDP" runat="server" BehaviorID="mpeShowConfirmTaskMsgNCPDP"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenControl"
        PopupControlID="pnlConfirmTaskMsgNCPDP" CancelControlID="btnConfirmTaskMsgNCPDPCancel" />
    <ajaxToolkit:ModalPopupExtender ID="mpeShowTaskMsgCancel" runat="server" BehaviorID="mpeShowTaskMsgCancel"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenControl"
        PopupControlID="pnlTaskMsgCancel" CancelControlID="btnTaskMsgCancelCancel" />
    <ePrescribe:ePAInitiationResponse ID="ucEPAInitiationResponse" runat="server" OnEPAUIEvent="ProcessEPAIntiationQSEvent">
    </ePrescribe:ePAInitiationResponse>
    <ePrescribe:ePAStatusInfo ID="ePAStatusInfomation" runat="server"></ePrescribe:ePAStatusInfo>
    <ePrescribe:ePANcpdpStatusInfo ID="ePANcpdpStatusInfomation" runat="server"></ePrescribe:ePANcpdpStatusInfo>
    <asp:HiddenField ID="pnlReload" runat="server"></asp:HiddenField>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
</asp:Content>
