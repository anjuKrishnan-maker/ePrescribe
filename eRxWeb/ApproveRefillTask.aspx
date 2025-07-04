<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.ApproveRefillTask" Title="Tasks" CodeBehind="ApproveRefillTask.aspx.cs" %>

<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Src="~/Controls/CSMedRefillRequestNotAllowed.ascx" TagName="CSMedRefillRequestNotAllowed"
    TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/EPCSDigitalSigning.ascx" TagName="EPCSDigitalSigning"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/MedicationHistoryCompletion.ascx" TagName="MedicationHistoryCompletion"
    TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/SecureProcessing.ascx" TagName="SecureProcessing" TagPrefix="ePrescribe" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" src="js/approveRefillTaskUtil.js"></script>
        <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
        <script type="text/javascript" language="javascript">
            function OpenNewWindow(url) {
                window.open(url);
            }


            function RefreshPatientHeader(patientid) {
                window.parent.ReLoadPatientHeader(patientid);
            }
            function setControlStateForRow(denialControlID, denyControlId, notesLabelControlID, notesControlID, maxCharControlID,
                approvedControlId, scriptMessageID, formularyStatus, levelOfPreferedness, taskType, rxTaskId, ddi) {
                var denialControl = document.getElementById(denialControlID);

                var denialChecked = false;
                var approvedChecked = false;

                if (approvedControlId != null) {
                    approvedChecked = approvedControlId.checked;
                }

                if (denyControlId != null) {
                    denialChecked = denyControlId.checked;
                }

                if (denialControl != null) {
                    if (denialChecked && (taskType === "<%=Constants.PrescriptionTaskType.REFREQ.ToString()%>" || taskType === "<%=Constants.PrescriptionTaskType.RXCHG.ToString()%>")) {
                        denialControl.style.display = "inline";
                        denialControl.selectedIndex = 0;
                    }
                    else {
                        denialControl.style.display = "none";
                    }
                }

                var notesLabelControl = document.getElementById(notesLabelControlID);
                var notesControl = document.getElementById(notesControlID);
                var maxCharControl = document.getElementById(maxCharControlID);

                if ((taskType === "<%=Constants.PrescriptionTaskType.REFREQ.ToString()%>" || taskType === "<%=Constants.PrescriptionTaskType.RXCHG.ToString()%>") && notesLabelControl != null && notesControl != null && maxCharControl != null) {
                    if (denialChecked) {
                        notesLabelControl.style.display = "none";
                        notesControl.style.display = "none";
                        maxCharControl.style.display = "none";
                    }
                    else if (approvedChecked) {
                        notesLabelControl.style.display = "inline";
                        notesControl.style.display = "inline";
                        maxCharControl.style.display = "inline";
                    }
                }

                grdViewFormularyMedSingleSelect(rxTaskId, "RxTaskId", "<%= grdApproveRefillTask.ClientID %>", true)
                UpdateSelected(scriptMessageID, formularyStatus, levelOfPreferedness, taskType, ddi);
            }

            function SetGridBindRequirment() {
                var hdnReBindGrid = document.getElementById("<%=hdnReBindGrid.ClientID %>");

                if (hdnReBindGrid != null)
                    hdnReBindGrid.value = true;

                return true;
            }

            function ScriptDeselected(source, eventArgs) {
                UpdateSelected('', '', '', '', '');
            }

            function ScriptSelected(source, eventArgs) {
                var ddi = eventArgs.getDataKeyValue("DDI");
                var taskScriptMessageId = eventArgs.getDataKeyValue("ScriptMessageID");
                var formularyStatus = eventArgs.getDataKeyValue("FormularyStatus");
                var levelOfPreferedness = eventArgs.getDataKeyValue("LevelOfPreferedness");
                var taskType = eventArgs.getDataKeyValue("tasktype");

                if (taskType === "<%=(int)Constants.PrescriptionTaskType.APPROVAL_REQUEST%>") {
                    DisableNonApprovalRows(eventArgs, "<%=(int)Constants.PrescriptionTaskType.APPROVAL_REQUEST%>");
                }
                else {
                    HandleControlsForSelection(eventArgs);
                }

                UpdateSelected(taskScriptMessageId, formularyStatus, levelOfPreferedness, taskType, ddi)
            }


            function UpdateSelected(taskScriptMessageId, formularyStatus, levelOfPreferedness, taskType, ddi) {
                var hdnScriptMessageID = document.getElementById("<%=hdnScriptMessageID.ClientID %>");
                if (hdnScriptMessageID != null) {
                    if ($('input[type=radio]:checked').size() == 0) { //Disable the process task button if there are no radio buttons still selected after changing rows
                        disableProcessBtn();
                    }
                    hdnScriptMessageID.value = taskScriptMessageId;
                }

                var hdnDDI = document.getElementById("<%=hdnDDI.ClientID %>");
                if (hdnDDI != null) {
                    hdnDDI.value = ddi;
                }

                var selectedFormularyStatus = document.getElementById("<%=selectedFormularyStatus.ClientID %>");
                if (selectedFormularyStatus != null) {
                    selectedFormularyStatus.value = formularyStatus;
                }
                var selectedLOP = document.getElementById("<%=selectedLOP.ClientID %>");
                if (selectedLOP != null) {
                    selectedLOP.value = levelOfPreferedness;
                }

                if (<%=_checkAltCopay.ToString().ToLower()%> && taskType != 0) {
                    //call for angular invocation 
                    SelectMedicine(ddi, formularyStatus, '', taskScriptMessageId);
                    var hiddenSelect = document.getElementById("<%=hiddenSelect.ClientID %>");
                    var hdnReBindGrid = document.getElementById("<%=hdnReBindGrid.ClientID %>");

                    if (hiddenSelect != null && hdnReBindGrid != null) {
                        hdnReBindGrid.value = false;
                        //hiddenSelect.click();
                    }
                }
            }

            window.onload = function () {
                var browser = detectBrowser();
                if ((browser == 'MSIE,8.0') || (browser == 'MSIE,7.0')) {
                    var obj = document.getElementById('ctl00_ContentPlaceHolder1_grdApproveRefillTask');
                    var obj2 = document.getElementById('ctl00_ContentPlaceHolder1_divScriptPadGrid');
                    if (obj != null) // To fix the issue with the IE 8 browser (the grid hieght was 272px, because of which the scroll bar was coming)
                    {
                        obj.style.height = "100%";
                    }
                    if (obj2 != null) // To fix the issue with the IE 8 browser (the grid hieght was 272px, because of which the scroll bar was coming)
                    {
                        obj2.style.height = "500px";
                    }
                }
            }
            function detectBrowser() {
                var N = navigator.appName;
                var UA = navigator.userAgent;
                var temp;
                var browserVersion = UA.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
                if (browserVersion && (temp = UA.match(/version\/([\.\d]+)/i)) != null)
                    browserVersion[2] = temp[1];
                browserVersion = browserVersion ? [browserVersion[1], browserVersion[2]] : [N, navigator.appVersion, '-?'];
                return browserVersion;
            };

            function denialCodeChanged(element) {
                document.getElementById("<%=btnProcessTasks.ClientID %>").disabled = (element.selectedIndex == 0) ? true : false;
            }

            function enableProcessBtn() {
                document.getElementById("<%=btnProcessTasks.ClientID %>").disabled = false;
            }
            function disableProcessBtn() {
                document.getElementById("<%=btnProcessTasks.ClientID %>").disabled = true;
            }


        </script>
    </telerik:RadCodeBlock>

    <table border="0" cellpadding="2" cellspacing="0" width="100%">
        <tr>
            <td colspan="2" class="h1title">
                <asp:Label ID="lblMsg" CssClass="errormsg" runat="server"></asp:Label>
            </td>
        </tr>
        <tr class="h2title">
            <td colspan="2">
                <ePrescribe:Message ID="ucSupervisingProvider" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessage2" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucProfileMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr class="h2title">
            <td align="left" style="height: 20px; white-space: nowrap; padding: 2px">
                <asp:Label ID="lblCoverage" runat="server" CssClass="indnt Shead" Text="Coverage:"></asp:Label>
                <asp:Label ID="lblCoverageName" runat="server" Visible="false" CssClass="SheadText"></asp:Label>
                <asp:ObjectDataSource ID="CoverageDataSource" runat="server" SelectMethod="GetPatientCoverages"
                    TypeName="Allscripts.Impact.PatientCoverage" OldValuesParameterFormatString="original_{0}"
                    OnSelected="CoverageDataSource_Selected">
                    <SelectParameters>
                        <asp:SessionParameter Name="patientID" SessionField="PatientID" Type="String" />
                        <asp:SessionParameter Name="licenseID" SessionField="LicenseID" Type="String" />
                        <asp:SessionParameter Name="userID" SessionField="UserID" Type="String" />
                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <asp:DropDownList ID="coverageDropDown" runat="server" DataSourceID="CoverageDataSource"
                    DataTextField="DisplayField" AutoPostBack="True" OnSelectedIndexChanged="coverageDropDown_SelectedIndexChanged"
                    DataValueField="SelectedCoverageID" 
                    OnDataBound="coverageDropDown_DataBound">
                </asp:DropDownList>
                &nbsp;&nbsp;<asp:Label runat="server" ID="lblDiagnosis" CssClass="SheadText"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table align="center" cssclass="normal" border="1" bordercolor="#b4b4b4" cellpadding="0"
                    cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                <tr class="h4title">
                                    <td colspan="6">
                                        <asp:Button ID="btnChangePatient" runat="server" CssClass="btnstyle" OnClick="btnChangePatient_Click"
                                            Text="Back" />&nbsp;&nbsp;
                                        <asp:Button ID="btnProcessTasks" runat="server" CssClass="btnstyle" OnClick="btnProcessTasks_Click"
                                            Text="Process Tasks" ToolTip="Click here to process the renewal requests" disabled="true"/>
                                        <asp:Button ID="btnCheckRegistry" runat="server" CausesValidation="false" CssClass="btnstyle" Text="Check Registry" Width="145px" ToolTip="Click here to connect to your state’s Prescription Monitoring Program" />        
                                        <asp:CheckBox ID="chkRegistryChecked" runat="server" Text="State Registry Checked" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="gridrow" runat="server">
                        <td style="margin-left: 10px">
                            <div id="divScriptPadGrid" runat="server" style="overflow-y: scroll; position: relative; border: 0 #000000 solid; text-align: left; padding: 2px">
                            <telerik:RadGrid ID="grdApproveRefillTask" cssclass="approve-refill-grid-no-border" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                CaptionAlign="Left" DataKeyNames="RxID,RxTaskId,refillquantity,tasktype,ScriptMessageID,MessageData,PharmacyIsElectronicEnabled,FormularyStatus,LevelOfPreferedness,IsOTC,RegisterStatus,GPI"
                                    DataSourceID="RefillTaskObjDataSource" EmptyDataText="No Refill Request Task" GridLines="None" Width="100%" PageSize="50" OnItemCreated="grdApproveRefillTask_ItemCreated"
                                OnItemDataBound="grdApproveRefillTask_RowDataBound" OnPreRender="grdApproveRefillTask_PreRender" 
                                    EnableEmbeddedSkins="False" CellSpacing="0" ShowGroupPanel="True">
                                <PagerStyle Mode="NextPrevAndNumeric" />
                                <MasterTableView GridLines="None" NoMasterRecordsText="No patient task available."
                                    Style="width: 100%" CommandItemDisplay="None" ClientDataKeyNames="RxID,RxTaskId,refillquantity,tasktype,ScriptMessageID,MessageData,PharmacyIsElectronicEnabled,FormularyStatus,LevelOfPreferedness,IsOTC,RegisterStatus,DDI"
                                    DataKeyNames="RxID,RxTaskId,refillquantity,tasktype,ScriptMessageID,MessageData,PharmacyIsElectronicEnabled,FormularyStatus,LevelOfPreferedness,RegisterStatus,DDI,GPI">

                                        <EditFormSettings>
                                            <EditColumn InsertImageUrl="Update.gif" UpdateImageUrl="Update.gif" CancelImageUrl="Cancel.gif" FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                                        </EditFormSettings>

                                        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                                    <HeaderStyle Font-Bold="true"  BackColor="#E1E2E6" BorderColor="#c4c0b5" BorderWidth="1px"/>
                                        <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                                        <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column"></RowIndicatorColumn>

                                        <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column"></ExpandCollapseColumn>
                                    <Columns>
                                        <telerik:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:RadioButton ID="rdbYes" runat="server" GroupName="select" Text="Approve"/>
                                                <br />
                                                <asp:RadioButton ID="rdbNo" runat="server" GroupName="select" Text="Deny"/>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" Width="70px" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Type / Date / Formulary" UniqueName="ColTypeDateForm">
                                            <ItemTemplate>
                                                <asp:Label ID="lblType" runat="server"></asp:Label>
                                                <br /><br />
                                                <asp:Label runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("RxDateDT", "{0:MM/dd/yyyy}")) %>'></asp:Label>
                                                <br /><br />
                                                <asp:Image ID="Image1" runat="server" onClick="ScriptSelected" />
                                                <asp:Label ID="lblLevelOfPreferedness" runat="server" Font-Size="7px" Text='<%# (Eval("LevelOfPreferedness").ToString() == "0" ?string.Empty :Eval("LevelOfPreferedness")) %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
           <%--                             <telerik:GridBoundColumn DataField="RxDateDT" HeaderText="Date Received" DataFormatString="{0:MM/dd/yyyy hh:mm tt}">
                                            <ItemStyle Width="85px" />
                                        </telerik:GridBoundColumn>--%>
                                        <telerik:GridTemplateColumn Visible="False">
                                            <ItemTemplate>
                                                <asp:Image ID="Image11" runat="server" onClick="ScriptSelected" />
                                                <asp:Label ID="lblLevelOfPreferedness1" runat="server" Font-Size="7px" Text='<%# (Eval("LevelOfPreferedness").ToString() == "0" ?string.Empty :Eval("LevelOfPreferedness")) %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="39px" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Type" Visible="False" UniqueName="Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lblType1" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="85px" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Rx Details" UniqueName="ColRxDetails">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRxDetails" runat="server" Text='<%# Bind("MedicationName") %>'></asp:Label>
                                                <br />
                                                <asp:Label ID="lblPatientComments" runat="server" Text='<%# "<b><i>Patient Comments:</i></b> " + ObjectExtension.ToEvalEncode(Eval("PatientComments"))  %>'
                                                    Visible='<%# ((Eval("PatientComments")).ToString().Length>0?true:false) %>'></asp:Label>
                                                
                                                <br /><asp:Label ID="lblQuantityError" Visible="False" runat="server" ForeColor="Red">Refill quantity is not present.  Please select CHANGE REQUEST and enter desired quantity</asp:Label>
                                                <asp:Label ID="lblDispensedPrescription" runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="" UniqueName="NotesDenialCol">
                                            <ItemTemplate>
                                                <asp:DropDownList CssClass="hideWhenNotSelected" ID="DenialCodes" runat="server" Width="280" onchange="denialCodeChanged(this);">
                                                </asp:DropDownList>
                                                <br />
                                                <br />
                                                <asp:Label ID="notesLabel" CssClass="hideWhenNotSelected" runat="server" Text="Comments"></asp:Label><br />
                                                <asp:TextBox ID="notesText" CssClass="hideWhenNotSelected" runat="server" TextMode="MultiLine" Width="280" Height="50"
                                                    MaxLength="70" Enabled="true"></asp:TextBox>
                                                <asp:Label runat="server" CssClass="hideWhenNotSelected" ID="lblPAApproveCode">Prior Auth Approval Code:</asp:Label><br/>
                                                <asp:TextBox runat="server" CssClass="hideWhenNotSelected" MaxLength="35" ID="txtPAAprroveCode" Height="20" Width="200"></asp:TextBox><br />
                                                <div id="divMaxCharacters" class="hideWhenNotSelected" runat="server">
                                                    (Maximum 35 Characters / <span id="charsRemaining" class="resetSpan" runat="server">35</span> characters
                                                    remaining)
                                                </div>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Actions" UniqueName="ActionColumn">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="lnkDelete" Visible="false" Text="Admin Delete<br/>" ForeColor="Orange"
                                                    runat="server" NavigateUrl='<%# "~/approverefilltask.aspx?tid=" + ObjectExtension.ToEvalEncode(Eval("RxTaskID"))+ "&action=del&type=" + ObjectExtension.ToEvalEncode(Eval("tasktype"))%>'></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle Width="120px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Send Notification" UniqueName="colSendToADM">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSendToADM" runat="server" Width="100px" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn>
                                            <ItemTemplate>
                                                <asp:Image ID="imgCSMed" runat="server" ImageUrl="~/images/ControlSubstance_sm.gif"
                                                    AlternateText="Controlled substance" Visible="false" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                                <ClientSettings>
                                    <ClientEvents OnRowDeselected="ScriptDeselected" OnRowSelected="ScriptSelected" />
                                    <Selecting AllowRowSelect="true" />
                                    <Resizing  />
                                </ClientSettings>

                                    <FilterMenu EnableImageSprites="False" EnableEmbeddedSkins="False"></FilterMenu>

                                    <HeaderContextMenu EnableEmbeddedSkins="False"></HeaderContextMenu>
                            </telerik:RadGrid>
                            <asp:ObjectDataSource ID="RefillTaskObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="GetRefillTask" TypeName="Allscripts.Impact.Provider" OnSelected="RefillTaskObjDataSource_Selected">
                                <SelectParameters>
                                    <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                    <asp:QueryStringParameter Name="patientID" QueryStringField="PatientID" Type="String"
                                        ConvertEmptyStringToNull="true" />
                                    <asp:QueryStringParameter Name="physicianId" QueryStringField="PhyID" Type="String" />
                                    <asp:Parameter Name="group" Type="String" DefaultValue="N" />
                                    <asp:SessionParameter Name="coverageid" SessionField="SelectedCoverageID" Type="object" />
                                    <asp:SessionParameter Name="formularyID" SessionField="FormularyID" Type="object" />
                                    <asp:SessionParameter Name="otcCoverage" SessionField="OTCCoverage" Type="object" />
                                    <asp:SessionParameter Name="genericDrugPolicy" SessionField="GenericDrugPolicy" Type="object" />
                                    <asp:SessionParameter Name="unlistedDrugPolicy" SessionField="UnListedDrugPolicy"
                                        Type="object" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            </div>
                            <br />
                            <asp:Label ID="lblCSLegend" runat="server" ForeColor="red" Visible="false">CS - Federal and state specific controlled substances cannot be sent electronically.</asp:Label>
                            <br />
                            <asp:Label ID="lblMessage" CssClass="indnt" runat="server" Font-Bold="True" Text="Physician's comments:"
                                Visible="False"></asp:Label><br />
                            <!-- upto this-->

                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="hiddenSelect">
                <UpdatedControls>
                   <telerik:AjaxUpdatedControl ControlID="pnlReload" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnProcessTasks">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ucMedicationHistoryCompletion" />
                    <telerik:AjaxUpdatedControl ControlID="ucEPCSDigitalSigning" />
                    <telerik:AjaxUpdatedControl ControlID="hdnScriptMessageID" />
                    <telerik:AjaxUpdatedControl ControlID="hdnDDI" />
                    <telerik:AjaxUpdatedControl ControlID="selectedFormularyStatus" />
                    <telerik:AjaxUpdatedControl ControlID="selectedLOP" />
                    <telerik:AjaxUpdatedControl ControlID="hiddenSelect" />
                    <telerik:AjaxUpdatedControl ControlID="hdnReBindGrid" />
                    <telerik:AjaxUpdatedControl ControlID="ucMessage2" />
                    <telerik:AjaxUpdatedControl ControlID="grdApproveRefillTask" LoadingPanelID="LoadingPanel2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdApproveRefillTask">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ucMedicationHistoryCompletion" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ucMedicationHistoryCompletion">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ucEPCSDigitalSigning" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/telerik/Skins/Allscripts/Grid/Img/LoadingProgressBar.gif"
            Style="border: 0; vertical-align: middle; text-align: center" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel2" runat="server">
        <ePrescribe:SecureProcessing ID="secureProcessingControl" runat="server" />
    </telerik:RadAjaxLoadingPanel>
    <ePrescribe:CSMedRefillRequestNotAllowed ID="ucCSMedRefillRequestNotAllowed" runat="server" />
    <ePrescribe:EPCSDigitalSigning ID="ucEPCSDigitalSigning" runat="server" IsScriptForNewRx="false" />
    <ePrescribe:MedicationHistoryCompletion ID="ucMedicationHistoryCompletion" runat="server"
        CurrentPage="ApproveRefillTask.aspx" />
    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
           <asp:HiddenField ID="hdnReBindGrid" runat="server"></asp:HiddenField>
             <asp:HiddenField ID="hdnDDI" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="hdnScriptMessageID" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="selectedDDI" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="checkCopay" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="selectedFormularyStatus" runat="server"></asp:HiddenField>
            <asp:HiddenField ID="selectedLOP" runat="server"></asp:HiddenField>
           
            <asp:Button ID="hiddenSelect" CssClass="content2backcolor" Style="display: none;"
                runat="server" OnClick="hiddenSelect_Click" UseSubmitBehavior="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
        <asp:HiddenField ID="pnlReload" runat="server"></asp:HiddenField>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
</asp:Content>
