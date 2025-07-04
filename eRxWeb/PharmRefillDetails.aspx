<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="True" Inherits="eRxWeb.PharmRefillDetails" Title="Pharmacy Refill Details" Codebehind="PharmRefillDetails.aspx.cs" %>

<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/MedicationHistoryCompletion.ascx" TagName="MedicationHistoryCompletion" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/CSMedRefillRequestNotAllowed.ascx" TagName="CSMedRefillRequestNotAllowed" TagPrefix="ePrescribe" %>
<%@ Register Src="Controls/SecureProcessing.ascx" TagName="SecureProcessing"  TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
    <script type="text/javascript" src="js/approveRefillTaskUtil.js"></script>
	<script type="text/javascript">
	    function OpenNewWindow(url) {
	        window.open(url);
	    }
	    function setControlStateForRow(denialControlID, denyControlId, notesLabelControlID, notesControlID, maxCharControlID, rdbApproved,
            ddi, formularyStatus, taskScriptMessageId, taskType) {
	        var approvedChecked = false;
	        var denialChecked = false;

            if(rdbApproved != null){
	            approvedChecked = rdbApproved.checked;
	        }
            
            if (denyControlId != null) {
                    denialChecked = denyControlId.checked;
            }           

	        var denialControl = document.getElementById(denialControlID);
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

	        if (notesLabelControl != null && notesControl != null) {
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

	        if (formularyStatus > -1) {
	            SelectMedicine(ddi, formularyStatus, '', taskScriptMessageId);
	        }
	    }
	</script>
        </telerik:RadCodeBlock>

	<table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td colspan="2" class="h1title">&nbsp
			</td>
		</tr>
		<tr class="h2title">
			<td colspan="2">
			    <ePrescribe:Message ID="ucMessage2" runat="server" Visible="false" />
			    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                <ePrescribe:Message ID="ucSupervisingProvider" runat="server" Visible="false" />
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<table align="center" class="normal" cellpadding="0" cellspacing="0" width="100%">
					<tr>
						<td>
							<table border="0" cellpadding="2" cellspacing="0" width="100%">
								<tr class="h4title">
									<td colspan="6">
									  <asp:Panel ID="pnlButtons" runat="server">
                                        <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" OnClick="btnCancel_Click" Text="Back" />&nbsp;&nbsp;
									    <asp:Button ID="btnRefillRequest" runat="server" CssClass="btnstyle" OnClick="btnProcess_Click"
											    Text="Process Tasks" ToolTip="Click here to process the renewal request."/>
                                            <asp:Button ID="btnCheckRegistry" runat="server" CausesValidation="false" CssClass="btnstyle" Text="Check Registry" Width="145px" ToolTip="Click here to connect to your state’s Prescription Monitoring Program" />                                                
                                          <asp:CheckBox ID="chkRegistryChecked" runat="server" 
                                              Text="State Registry Checked" />
                                       </asp:Panel>
                                    </td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td style="margin-left: 10px">
							<telerik:RadGrid ID="grdPharmRefillDetails" runat="server" AllowPaging="True" AllowSorting="False"
								AutoGenerateColumns="False" CaptionAlign="Left" 
								DataKeyNames="RxTaskID, scriptMessageID, TaskType, patientID, patientLastName, patientFirstName, providerID, drugDescription, sigText, strength, DDI, quantity, refills, daysSupply, DAW, pharmacyName, MessageData"
								AllowAutomaticUpdates="False" AllowMultiRowSelection="false" EnableViewState="true"
                                EmptyDataText="Pharmacy refill message is not found in system." EnableEmbeddedSkins="false"
								GridLines="None" Width="100%" PageSize="50" OnItemCommand="grdPharmRefillDetails_ItemCommand"
                                OnItemDataBound="grdPharmRefillDetails_ItemDataBound" OnItemCreated="grdPharmRefillDetails_ItemCreated" OnNeedDataSource="grdPharmRefillDetails_OnNeedDataSource">
                                <PagerStyle Mode="NextPrevAndNumeric" />
                                <MasterTableView GridLines="None" NoMasterRecordsText="Pharmacy refill message is not found in the system."
                                    Style="width: 100%" CommandItemDisplay="None" 
                                    ClientDataKeyNames="RxTaskID, scriptMessageID, TaskType, patientID, patientLastName, patientFirstName, providerID, drugDescription, sigText, strength, DDI, quantity, refills, daysSupply, DAW, pharmacyName, MessageData"
                                    DataKeyNames="RxTaskID, scriptMessageID, TaskType, patientID, patientLastName, patientFirstName, providerID, drugDescription, sigText, strength, DDI, quantity, refills, daysSupply, DAW, pharmacyName, MessageData">
                                    <HeaderStyle Font-Bold="true" BackColor="#E1E2E6" BorderColor="#c4c0b5" BorderWidth="1px"/>
							        <Columns>
								        <telerik:GridTemplateColumn>
									        <ItemTemplate>
										        <asp:RadioButton ID="rdbYes" runat="server" GroupName="select" Text="Approve"  /><br />
                                                <asp:RadioButton ID="rdbNo" runat="server" GroupName="select" Text="Deny" />
									        </ItemTemplate>
									        <ItemStyle HorizontalAlign="Left" Width="75px" />
									        <HeaderStyle HorizontalAlign="Center" />
								        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Type / Date / Formulary" UniqueName="ColTypeDateForm">
                                            <ItemTemplate>
                                                <asp:Label ID="lblType" Text="Pharmacy Renewal" runat="server"></asp:Label>
                                                <br /><br />
                                                <asp:Label runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("RxDateDT", "{0:MM/dd/yyyy}")) %>'></asp:Label>
                                                <br /><br />
                                                <asp:Image ID="Image1" runat="server" onClick="ScriptSelected" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
								        <telerik:GridTemplateColumn HeaderText="Rx Details" HeaderStyle-HorizontalAlign="Left">
									        <ItemTemplate>
										        <asp:Label ID="lblRxDetails" runat="Server"></asp:Label>
                                                <br /><br /><asp:Label ID="lblQuantityError" Visible="False" runat="server" ForeColor="Red">Refill quantity is not present.  Please select CHANGE REQUEST and enter desired quantity</asp:Label>
										        <asp:Label ID="lblDispensedRxDetails" runat="server"></asp:Label>
									        </ItemTemplate>
								        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="" UniqueName="NotesDenialCol">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="DenialCodes" runat="server" Width="250">
                                                </asp:DropDownList>
                                                <br />
                                                <br />
                                                <asp:Label ID="notesLabel" runat="server" Text="Comments"></asp:Label><br />
                                                <asp:TextBox ID="notesText" runat="server" TextMode="MultiLine" Width="250" Height="50"
                                                    MaxLength="70" Enabled="true"></asp:TextBox>
                                                <asp:Label runat="server" ID="lblPAApproveCode">Prior Auth Approval Code:</asp:Label><br/>
                                                <asp:TextBox runat="server" MaxLength="70" ID="txtPAAprroveCode" Height="20" Width="200"></asp:TextBox><br />
                                                <div id="divMaxCharacters" runat="server">
                                                    (Maximum 70 Characters / <span id="charsRemaining" runat="server">70</span> characters
                                                    remaining)
                                                </div>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridTemplateColumn HeaderText="Actions" UniqueName="ActionColumn">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lbRegisterPatient" runat="server" Text="Reconcile Patient" CommandName="RegisterPatient" Font-Bold="true" ForeColor="Red"></asp:LinkButton>
                                                <asp:LinkButton ID="lbPrintCsRx" runat="server" Text="Print CS Rx" CommandName="PrintCSRx" Font-Bold="true" ForeColor="Red"></asp:LinkButton>
                                                <asp:LinkButton ID="lbReconcileRx" runat="server" Text="Reconcile Rx" CommandName="ReconcileRx" Font-Bold="true" ForeColor="Red"></asp:LinkButton>
                                                <asp:LinkButton ID="lbChangeRequest" runat="server" Text="Change Request" CommandName="ChangeRequest" ForeColor="Gray"></asp:LinkButton>                                                
                                                <asp:LinkButton ID="lbChangePatient" runat="server" Text="Change Patient" CommandName="ChangePatient" ForeColor="Gray"></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="100px" />
                                        </telerik:GridTemplateColumn>
								        <telerik:GridTemplateColumn HeaderText="Send Notification" UniqueName="colSendToADM">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSendToADM" runat="server" Width="100px"/>
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
                  
							</telerik:RadGrid>
						</td>
					</tr>
                    <tr><td colspan="2">
                         <asp:Label ID="lblCSMed" runat="server" ForeColor="red" Visible="false">CS - Federal and state specific controlled substances cannot be sent electronically.</asp:Label>
                        </td></tr>
				</table>
			</td>
		</tr>
	</table>
    <ePrescribe:MedicationHistoryCompletion ID="ucMedicationHistoryCompletion" runat="server" CurrentPage="PharmRefillDetails.aspx" />
    <ePrescribe:CSMedRefillRequestNotAllowed ID="ucCSMedRefillRequestNotAllowed" runat="server" />
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <ePrescribe:SecureProcessing ID="secureProcessingControl" runat="server" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel2" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="radAjaxManager" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnRefillRequest">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ucMedicationHistoryCompletion" />
                    <telerik:AjaxUpdatedControl ControlID="pnlButtons" LoadingPanelID="LoadingPanel2" />
                    <telerik:AjaxUpdatedControl ControlID="grdPharmRefillDetails" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ucMessage" />
                    <telerik:AjaxUpdatedControl ControlID="ucMessage2" />
                    <telerik:AjaxUpdatedControl ControlID="ucSupervisingProvider" />
                </UpdatedControls>
            </telerik:AjaxSetting>
             <telerik:AjaxSetting AjaxControlID="grdPharmRefillDetails">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ucMedicationHistoryCompletion" />
                    <telerik:AjaxUpdatedControl ControlID="ucMessage2" />
                    <telerik:AjaxUpdatedControl ControlID="ucMessage" />
                    <telerik:AjaxUpdatedControl ControlID="ucSupervisingProvider" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">



    
</asp:Content>
