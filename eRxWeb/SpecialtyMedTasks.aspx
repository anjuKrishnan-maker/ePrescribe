<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="SpecialtyMedTasks.aspx.cs" Inherits="eRxWeb.SpecialtyMedTasks" MasterPageFile="~/PhysicianMasterPage.master" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register TagPrefix="eprescribe" TagName="message" Src="~/Controls/Message.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script src="js/specialtyMedTask.js"></script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var warningTimeout;
            
            function showSessionWarning() {
                var btnSessionWarningTrigger = document.getElementById("<%=btnSessionWarningTrigger.ClientID%>");
                btnSessionWarningTrigger.click();
                $("#diviFrameTimerContainer").show();
                var count = 119, timer = setInterval(function () {
                    try{
                        var newCount = count--;
                        if (newCount < 10) {
                            $("#divTimer").html('0' + newCount);
                            $("#divTimeriFrame").html('0' + newCount);
                        }
                        else {
                            $("#divTimer").html(newCount);
                            $("#divTimeriFrame").html(newCount);
                        }
                    }
                    catch (e){
                    }
                    if (count < 0) clearInterval(timer);
                }, 1000);
            }

            function specTaskSelected(source, eventArgs)  {                     
                var hiddenSelect = document.getElementById("<%=hiddenSelect.ClientID %>");
                var hdnFieldbtnProcessTask = document.getElementById("<%=hdnFieldbtnProcessTask.ClientID %>");
                if (hiddenSelect != null)
                {
                    GridSingleSelect(eventArgs.getDataKeyValue("RxTaskID").toString(), "RxTaskID", "<%= grdSpecialtyMedTasks.MasterTableView.ClientID %>", true);
                    hdnFieldbtnProcessTask.value = eventArgs.get_itemIndexHierarchical();
                    hiddenSelect.click();
                }
            }

            function FilterPharmacy(textbox) {
                var grid = window.$find("<%=grdSpecPharm.MasterTableView.ClientID %>");
                for (var i = 0; i < "<%=grdSpecPharm.Items.Count%>"; i++) {
                    var pharmName = grid.getCellByColumnUniqueName(grid.get_dataItems()[i], "colName").innerHTML;
                    if (textbox.value === '') {
                        grid.showItem(i);
                        continue;
                    }
                    if (!pharmName.toLowerCase().startsWith(textbox.value.toLowerCase())) {
                        grid.hideItem(i);
                    }
                    else {
                        grid.showItem(i);
                    }
                }
            }

            function connectToiFC(url, activityId, rxTaskId, patientId) {
                var fcFrame = document.getElementById("<%=fcFrame.ClientID %>");
                if(fcFrame != null){
                    fcFrame.src = "AssistRxLaunchPage.aspx?TargetUrl=" + url;
                };

                    var hdnActivityId = document.getElementById("<%= hdnActivityId.ClientID %>");
                    var hdnPatientId = document.getElementById("<%= hiddenPatientID.ClientID %>");
                    if (activityId != '' && hdnActivityId != null && rxTaskId != '' && rxTaskId != null && hdnPatientId != null)
                    {
                        hdnActivityId.value = activityId;
                        hdnPatientId.value = patientId;
                    }

                    var linksHit = document.getElementById("<%=linksHit.ClientID %>");
                    if (linksHit != null) {
                        if (linksHit.value == null || linksHit.value == "") {
                            linksHit.value = url;
                        }
                        else {
                            linksHit.value = linksHit.value + "|" + url;
                        }
                    }

                    sizeFrame();

                    clearTimeout(warningTimeout);
                    warningTimeout = setTimeout("showSessionWarning()", <%= PageExpirationTimerMs - 120000 %>);
                    
                    var hiddenIFC = document.getElementById("<%=hiddenIFC.ClientID %>");
                    if (hiddenIFC != null) {
                        hiddenIFC.click();
                    }
                }
           
            

            function taskSelectedRadio(rxTaskId, patientId) {
                GridSingleSelect(rxTaskId, "RxTaskID", "<%= grdSpecialtyMedTasks.MasterTableView.ClientID %>", true);
                getPatientInfo(patientId);
            }

            function getPatientInfo(patientId) {
                var loadingImage = document.getElementById("ctl00_loading");

                if (loadingImage != null) {
                    loadingImage.style.display = "inline";
                }

                var hiddenPatientID = document.getElementById("<%=hiddenPatientID.ClientID %>");
                if (hiddenPatientID != null) {
                    hiddenPatientID.value = patientId;
                }

                var hiddenSelect = document.getElementById("<%=hiddenSelectTask.ClientID %>");
                if (hiddenSelect != null) {
                    __doPostBack(hiddenSelect.name, '');
                }

                SelectPatient(patientId);
            }
        </script>
    </telerik:RadCodeBlock>
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    <table width="100%">
        <asp:Panel ID="Tasksh1Panel" runat="server" Width="100%">
            <tr class="h1title">
                <td>
                    <table width="100%" cellpadding="3px">
                        <td>
                            <asp:RadioButton ID="rbtnMyTask" runat="server" AutoPostBack="True" Checked="False"
                                CssClass="adminlink1" GroupName="Task2" Text="My Tasks" OnCheckedChanged="rbtnMyTask_CheckedChanged" />
                            <asp:RadioButton ID="rbtnSiteTask" runat="server" AutoPostBack="True" GroupName="Task2"
                                Text="Site Tasks" OnCheckedChanged="rbtnSiteTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbtnAdminTask" runat="server" AutoPostBack="True" GroupName="Task2"
                                Text="Assistant's Tasks" OnCheckedChanged="rbtnAdminTask_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbPharmRefills" runat="server" GroupName="Task" Text="Pharmacy Tasks"
                                CssClass="adminlink1" AutoPostBack="true" OnCheckedChanged="rbPharmRefills_Changed" />
                            <asp:RadioButton ID="rbEPATasks" runat="server" AutoPostBack="true" GroupName="Task2"
                                Text="ePA Tasks" OnCheckedChanged="rbtnePA_CheckedChanged" CssClass="adminlink1" />
                            <asp:RadioButton ID="rbSpecialtyMed" runat="server" AutoPostBack="true" GroupName="Task2" Checked="True"
                                Text="Patient Access Services" CssClass="adminlink1" />
                        </td>
                        <td align="right">
                            <div id="divProvider" runat="server" visible="True">
                                <span class="adminlink1">Show tasks for:</span>&nbsp
                                <asp:DropDownList ID="ddlProvider" runat="server" Width="170px" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                        </td>
                    </table>
                </td>
            </tr>
        </asp:Panel>




    </table>

    <table>
        <tr>
            <td colspan="1">
               <eprescribe:message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
                
        <tr>
            <td>
                <asp:RadioButton ID="rbtnOpen" runat="server" Text="Open" AutoPostBack="true" GroupName="rbgFilterSpecMedTask" Checked="true" OnCheckedChanged="rbgFilterSpecMed_CheckedChanged"/>
                <asp:RadioButton ID="rbtnResolved" runat="server" Text="Resolved" AutoPostBack="true" GroupName="rbgFilterSpecMedTask" OnCheckedChanged="rbgFilterSpecMed_CheckedChanged"/>
                <asp:RadioButton ID="rbtnAll" runat="server" Text="All" AutoPostBack="true" GroupName="rbgFilterSpecMedTask" OnCheckedChanged="rbgFilterSpecMed_CheckedChanged"/>
            </td>
        </tr>
    </table>


    <table>
        <tr>
            <td>
                <asp:Button ID="btnProcessTask" runat="server" CssClass="btnstyle"
                    OnClick="btnProcessTask_Click" Text="Process Task" UseSubmitBehavior="true" ToolTip="" />
                <asp:HiddenField ID="hdnFieldbtnProcessTask" runat="server" />
            </td>
        </tr>
    </table> 
    <asp:Button ID="hiddenSelectTask" CssClass="btnstyle" runat="server" Text="" Style="display: none" UseSubmitBehavior="true" OnClick="hiddenSelectTask_Click" />
    <input id="hiddenPatientID" runat="server" type="text" style="display: none;" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="hiddenSelectTask">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ucMessage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadGrid ID="grdSpecialtyMedTasks" runat="server" Skin="Allscripts" EnableEmbeddedSkins="False"
        GridLines="None" OnItemDataBound="grdSpecialtyMedTasks_ItemDataBound" EnableViewState="True"
        AllowPaging="True" AllowSorting="True" onrowcommand="grdSpecialtyMedTask_RowCommand"
        AutoGenerateColumns="False" Width="100%" PageSize="20" OnItemCommand="grdSpecialtyMedTasks_ItemCommand"
        OnDataBound="grdSpecialtyMedTasks_DataBound" OnRowDataBound="grdSpecialtyMedTasks_RowDataBound" DataSourceID="SpecialtyMedTaskObjDataSource" CellSpacing="0" CssClass="auto-style1">
        <MasterTableView GridLines="None" NoMasterRecordsText="No Tasks" Style="width: 100%; vertical-align: top"
            CommandItemDisplay="None"
            ClientDataKeyNames="RxTaskID, PatientGUID"
            DataKeyNames="IsLimitedDistributionMedication, DDI, SpecialtyEnrollmentStatus, PriorAuthorizationStatus, SpecEnrollStatus, SpecPAStatus, URL, RxID, RefillQuantity, DaysSupply, RX_TASK_status, RX_DETAIL_status, RxTaskID, LastPharmacyID, mobPharmID, ActivityID, PatientGUID, ProviderID"
            DataSourceID="SpecialtyMedTaskObjDataSource">
            <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column"></RowIndicatorColumn>
            <Columns>
 				<telerik:GridTemplateColumn UniqueName="RadioButtonTemplateColumn">
                    <ItemStyle Width="30px" HorizontalAlign="Left"></ItemStyle>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <ItemTemplate>
                        <input id="rbSelectedTask" runat="server" type="radio" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="PatientName" FilterControlAltText="Filter Patient column" HeaderText="Patient" SortExpression="PatientName" UniqueName="Patient">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn UniqueName="ProviderName" DataField="ProviderName" HeaderText="Prescriber"
                    SortExpression="ProviderName">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn UniqueName="MedicationSig" DataField="MedicationSig" HeaderText="Medication & Sig"
                    SortExpression="MedicationSig">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn UniqueName="RxDate" DataField="RxDate" HeaderText="Rx Date"
                    SortExpression="RxDate" DataFormatString="{0:MM/dd/yyyy}">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn SortExpression="SpecEnrollStatus" HeaderText="Enrollment Services Status" FilterControlAltText="Filter SpecEnrollStatus column" UniqueName="SpecEnrollStatus">
                    <ItemTemplate>
                        <asp:Literal ID="litSpecEnrollStatus" Mode="PassThrough" runat="server"></asp:Literal>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn SortExpression="SpecPAStatus" HeaderText="ePA Status" FilterControlAltText="Filter SpecPAStatus column" UniqueName="SpecPAStatus">
                    <ItemTemplate>
                        <asp:Literal ID="litSpecPAStatus" Mode="PassThrough" runat="server"></asp:Literal>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn>
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
                            <asp:Button ID="hdnDestinationSelected" runat="server" Style="display: none" OnClick="hdnDestinationSelected_OnClick" />
                            <asp:DropDownList ID="ddlDest" Width="150px" runat="server">
                            </asp:DropDownList>
                        </div>
                    </ItemTemplate>
                    <HeaderStyle Width="20%" />
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn>
                    <ItemTemplate>
                         <div class="SpecialtyMedAttachmentsDropdown">
                            <asp:LinkButton ID="linkButtonImgDoc" Visible="false" runat="server">
                                <img id="documentIcon" src="images/shim.gif" Visible="false" style="vertical-align:middle; border:0px"/>
                            </asp:LinkButton>
                            <div class="SpecialtyMedAttachmentsDropdown-content">
                                <asp:LinkButton ID="LinkButtonSpecialtyEnrollmentForm" Visible="false" Text="Enrollment Form" runat="server" OnClick="linkButtonSpecialtyEnrollmentDoc_Click" AutoGenerateRows="false" GridLines="none" BorderStyle="none" Width="100%"/>
                                <asp:LinkButton ID="LinkButtonPriorAuthorizationForm" Visible="false" Text="Prior Authorization Form" runat="server" OnClick="linkButtonPriorAuthorizationDoc_Click" AutoGenerateRows="false" GridLines="none" BorderStyle="none" Width="100%"/>
                            </div>
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="PatientGUID" Display="False" FilterControlAltText="Filter PatientGUID column" Groupable="False" UniqueName="PatientGUID"
                    HeaderText="PatientGUID" SortExpression="PatientGUID">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="RxTaskID" Display="False" FilterControlAltText="Filter column RxTaskID" Groupable="False" UniqueName="RxTaskID">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn Display="False" FilterControlAltText="Filter colLimitedPharmacySelected column" UniqueName="colLimitedPharmacySelected" Visible="False">
                    <ItemTemplate>
                        <asp:HiddenField runat="server" ID="hdnLimitedPharmacySelected" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                                    <HeaderStyle Font-Bold="true" />
                                </MasterTableView>
        <ClientSettings>
            <ClientEvents OnRowClick="taskSelected"></ClientEvents>
            <Selecting AllowRowSelect="true"></Selecting>
        </ClientSettings>
        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

        <FilterMenu EnableImageSprites="False" EnableEmbeddedSkins="False"></FilterMenu>

        <HeaderContextMenu EnableEmbeddedSkins="False"></HeaderContextMenu>
    </telerik:RadGrid>
    <asp:ObjectDataSource ID="SpecialtyMedTaskObjDataSource" runat="server" SelectMethod="LoadSpecialtyMedTaskList"
        TypeName="Allscripts.Impact.SpecialtyMedData">
        <SelectParameters>
            <asp:SessionParameter Name="licenseId" SessionField="LICENSEID" Type="String" />
            <asp:Parameter Name="physicianID" DefaultValue="" Type="String" />
            <asp:Parameter Name="pobID" DefaultValue="" Type="String" />
            <asp:SessionParameter Name="patientID" DefaultValue="" SessionField="PATIENTID" Type="String" />
            <asp:SessionParameter Name="SSOMode" DefaultValue="" SessionField="SSOMode" Type="String" />
            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Object" />
            <asp:Parameter Name="taskType" DefaultValue="1" Type="Int32" />
            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Panel Style="display: none" ID="pnlSpecialtyRxPharmacy" runat="server">
        <div class="overlaymain" style="width: 700px;
">
            <div class="overlayTitle">
                <div class="overlayTitleText" style="float: left">Select Limited Distribution Pharmacy</div>
                <div style="float: right">
                    <img src="../images/searchBoxImage.png" class="searchControlImage" />
                    <asp:TextBox autocomplete="off" MaxLength="50" ID="txtPharmName" CssClass="searchControlTextBoxOnly" Style="margin-right: 20px;"
                        Width="185px" ToolTip="Pharm Name" runat="server"
                        onblur="if (this.value === '') {this.value = this.getAttribute('defaultValue');this.style.color = 'rgb(102, 102, 102)';}"
                        onfocus="if (this.value === this.getAttribute('defaultValue')) {this.value = '';this.style.color = 'black';}"
                        defaultValue="Filter by name"
                        onkeyup="FilterPharmacy(this)"
                        value="Filter by name"></asp:TextBox>
                </div>
            </div>
            <div class="overlayContent">
                <asp:Label ID="lblPharmacyErrorMessage" Visible="False" Text="**Please select a pharmacy" CssClass="errormsg" runat="server"></asp:Label>
                <telerik:RadGrid ID="grdSpecPharm" runat="server" Skin="Allscripts" EnableEmbeddedSkins="False"
                    GridLines="None" AllowSorting="True" Style="width: 100%;" AutoGenerateColumns="False" CellSpacing="0">
                    <ClientSettings>
                        <Selecting AllowRowSelect="True"></Selecting>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="Id, Name, Address">
                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                        <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                            <HeaderStyle Width="20px"></HeaderStyle>
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridClientSelectColumn FilterControlAltText="Filter column1 column" UniqueName="column1">
                            </telerik:GridClientSelectColumn>
                            <telerik:GridBoundColumn DataField="Name" DataType="System.String" FilterControlAltText="Filter colName column"
                                HeaderText="Pharmacy Name" SortExpression="Name" UniqueName="colName">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn FilterControlAltText="Filter colAddress column" HeaderText="Address" UniqueName="colAddress" SortExpression="Address">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAddress" Text='<%# ObjectExtension.ToEvalEncode(Eval("Address")) %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn CancelImageUrl="Cancel.gif" FilterControlAltText="Filter EditCommandColumn column" InsertImageUrl="Update.gif" UpdateImageUrl="Update.gif">
                            </EditColumn>
                        </EditFormSettings>
                        <PagerStyle PageSizeControlType="RadComboBox" />
                    </MasterTableView>
                    <PagerStyle PageSizeControlType="RadComboBox" />
                    <FilterMenu EnableImageSprites="False" EnableEmbeddedSkins="False"></FilterMenu>
                    <HeaderContextMenu EnableEmbeddedSkins="False"></HeaderContextMenu>
                </telerik:RadGrid>
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnCancel" Text="Cancel" CssClass="btnstyle" OnClick="btnCancel_OnClick" runat="server" />
                <asp:Button ID="btnSpecialtyRxPharmacyOk" OnClick="btnSpecialtyRxPharmacyOk_OnClick" Text="OK" CssClass="btnstyle btnStyleAction" runat="server" />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeSpecialtyRxPharmacy" runat="server" TargetControlID="btnPopUpTrigger"
        DropShadow="false" PopupControlID="pnlSpecialtyRxPharmacy" BackgroundCssClass="modalBackground">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
        Style="display: none;" />
    <asp:Panel ID="panelIFC" Style="display: none" runat="server">
        <div style="display: none">
            <asp:HiddenField ID="linksHit" runat="server" />

        </div>
        <div id="ifcDiv" runat="server" class="iFrameModal">
            <div style="overflow: hidden">
                <div id="diviFrameTimerContainer"  style="float: left; margin-top: 8px; color: red; display: none">Remaining Time: &nbsp;&nbsp;<div id="divTimeriFrame" style="float: right; color: red; margin-right: 13px;">120</div></div>
                <asp:Button ID="btnFCReturn" runat="server" CssClass="btnstyle" Style="float: right;" Text="Close" />
            </div>
            <br />
            <div style="width: 100%; height: 100%">
                <center>
                <iframe name="fcFrame" id="fcFrame" align="top" width="99%" runat="server" frameborder="0" title="Library" style="height: 100%">
                </iframe>
            </center>
            </div>
        </div>

    </asp:Panel>
    <asp:HiddenField ID="hdnActivityId" runat="server"/>
    <asp:Button ID="hiddenSelect" runat="server" Text="" style="display:none" OnClick="hiddenSelect_Click" UseSubmitBehavior="false"/>
    <asp:HiddenField ID="hdnReBindGrid" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="selectedGridIndex" runat="server"></asp:HiddenField>
    <asp:Button ID="hiddenIFC" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender ID="mpeIFC" runat="server" BehaviorID="mpeIFC" DropShadow="false" BackgroundCssClass="modalBackgroundNotTransparent" TargetControlID="hiddenIFC" PopupControlID="panelIFC"></ajaxToolkit:ModalPopupExtender>
    <asp:Panel Style="display: none" ID="pnlConfirmation" runat="server">
        <div class="overlaymain">
            <div class="overlayTitle">
                Please Confirm
            </div>
            <div class="overlayContent aligncentr">
                <br/>
                Are you sure you want to process the selected row?
                <br/>
                <br/>
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnCancelConfirm" Text="Cancel" CssClass="btnstyle" OnClick="btnCancel_OnClick" runat="server" />
                <asp:Button ID="btnOk" OnClick="btnOk_OnClick" Text="OK" CssClass="btnstyle btnStyleAction" runat="server" />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeConfirmProcess" runat="server" CancelControlID="btnCancelConfirm" TargetControlID="btnHiddenConfirmPopup"
        DropShadow="false" PopupControlID="pnlConfirmation" BackgroundCssClass="modalBackground">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnHiddenConfirmPopup" runat="server" CausesValidation="false" Text="Button"
        Style="display: none;" />
    
    <asp:Panel ID="pnlSessionWarning" style="display: none;" runat="server">
        <div class="overlaymain" style="padding-top: 0">
            <div class="overlayTitle">
                <div class="overlayTitleText" style="float: left">Attention</div><div  style="float: right; margin-top: 8px;">Remaining Time: &nbsp;&nbsp;<div id="divTimer" style="float: right; margin-right: 13px;">120</div></div>
            </div>
            <div class="overlayContent aligncentr">
                <br/>
                You will be logged off soon.  Please save your iAssist changes!  If you are logged off any unsaved changes will be lost.
                <br/>
                <br/>
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnOkSessionWarning" Text="OK" CssClass="btnstyle btnStyleAction" runat="server" />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeSessionWarning" runat="server" CancelControlID="btnOkSessionWarning" TargetControlID="btnSessionWarningTrigger"
        DropShadow="false" PopupControlID="pnlSessionWarning" BackgroundCssClass="modalBackground">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnSessionWarningTrigger" runat="server" CausesValidation="false" Text="Button"
        Style="display: none;" />
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
</asp:Content>
