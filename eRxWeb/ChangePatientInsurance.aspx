<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.ChangePatientInsurance" Title="Change Patient Insurance" Codebehind="ChangePatientInsurance.aspx.cs" %>
<%@ Register Assembly="Joel.Net.Refresh" Namespace="Joel.Net" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<cc1:Refresh ID="Refresh1" runat="server" />
	<script type="text/javascript" language="javascript">
		
		function insurancePlan_onChange() {
			var insurancePlanSelect = document.getElementById("<%=lbInsurancePlan.ClientID %>");
			var updateInsuranceButton = document.getElementById("<%=btnUpdatePatientInsurance.ClientID %>");
			
			if (insurancePlanSelect.selectedIndex >= 0)
			{
				if (insurancePlanSelect.options[insurancePlanSelect.selectedIndex].value.length > 0)
				{
					updateInsuranceButton.disabled = false;
				}
			}
		}
		function insuranceName_onFocus() {
			var searchByNameRadio = document.getElementById("<%=rdSearchByName.ClientID %>");
			
			//Auto select the search by radio button, when focus is on the insurance name input.
			searchByNameRadio.checked = true;
		}
		function ValidateSearchOptions(source, arguments)
		{
			var searchTopTenRadio = document.getElementById("<%=rdTopTenPlans.ClientID %>")
			var searchByNameRadio = document.getElementById("<%=rdSearchByName.ClientID %>");
			var insuranceNameInput = document.getElementById("<%=txtInsuranceName.ClientID %>");
			
			if (searchTopTenRadio.checked)
				arguments.IsValid = true;
			else if (searchByNameRadio.checked && insuranceNameInput.value.length > 0)
				arguments.IsValid = true;
			else
				arguments.IsValid = false;
			
		}
	</script>
    <asp:HiddenField ID="isFromAngularModal" runat="server" Value="false" ClientIDMode="Static"/>
    <telerik:RadAjaxManager ID="RadAjaxManagerins" runat="server">
        
        <AjaxSettings>
           
            <telerik:AjaxSetting AjaxControlID="btnUpdatePatientInsurance">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblPayorNm" />
                    <telerik:AjaxUpdatedControl ControlID="lblPayorID" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnPayorSearch">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPayorList" LoadingPanelID="LoadingPanel1"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdPayorList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPayorList" LoadingPanelID="LoadingPanel1"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
         <ClientEvents OnResponseEnd="responseEnd"/>
    </telerik:RadAjaxManager>	
	<table width="100%" border="1" cellspacing="0" cellpadding="0" bordercolor="#b5c4c4">
		<tr class="h1title indnt">
            <td>
                <span id="heading" runat="server" class="highlight">Change Patient Insurance</span>
            </td>
        </tr>
        <tr>
			<td style="width: 759px">
				<table width="100%" border="0" cellspacing="0" cellpadding="0">
					<tr class="h4title">
						<td colspan="6" style="height: 20px">
							<asp:Button ID="btnCancelUpdate" runat="server" CssClass="btnstyle"
								Text="Back" ToolTip="Click to cancel patient insurance update." CausesValidation="false" OnClick="btnCancelUpdate_Click" />&nbsp;&nbsp;
							<asp:Button ID="btnUpdatePatientInsurance" runat="server" CssClass="btnstyle" CausesValidation="true"
								Text="Save" ToolTip="Click to update patient insurance plan." OnClick="btnUpdatePatientInsurance_Click" />
							
                            <asp:Label ID="lblPlanID" runat="server" Text="" Visible="false"></asp:Label> 									
						</td>
					</tr>
				</table>
			</td>
        </tr>
        <tr>
			<td>
				<table border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td colspan="2" class="indnt">
							Member #:
							<asp:TextBox ID="txtMemberNo" runat="server" Width="124px" MaxLength="50"></asp:TextBox>
							<asp:RegularExpressionValidator ID="revMemberNo" runat="server" ControlToValidate="txtMemberNo"
								ErrorMessage="Please enter a valid member number." ValidationExpression="^[a-zA-Z0-9]+$">*</asp:RegularExpressionValidator></td>
						<td colspan="2" class="indnt">
							Group #:
							<asp:TextBox ID="txtGroupNo" runat="server" Width="200px" MaxLength="50"></asp:TextBox>&nbsp;
							<asp:RegularExpressionValidator ID="revGroupNo" runat="server" ControlToValidate="txtGroupNo"
								ErrorMessage="Please enter a valid group number." ValidationExpression="^[a-zA-Z0-9]+$">*</asp:RegularExpressionValidator>
						</td>
					</tr>
					<tr>
						<td class="indnt" style="height: 20px">
							Relationship to Cardholder:
						</td>
						<td colspan="3" style="height: 20px">
							<asp:RadioButton ID="rdMember" runat="server" GroupName="RelationShip"
											Text="Member" />
							<asp:RadioButton ID="rdSpouse" runat="server" GroupName="RelationShip"
											Text="Spouse" />
							<asp:RadioButton ID="RdChild" runat="server" GroupName="RelationShip"
											Text="Child" />
							<asp:RadioButton ID="RdOther" runat="server" GroupName="RelationShip"
											Text="Other " />
						</td>
					</tr>
					<tr>
						<td colspan="2" class="indnt">
							Card Holder First Name:
						</td>
						<td colspan="2" class="indnt">
						    <asp:TextBox ID="txtFirstName" runat="server" Width="227px" MaxLength="35"></asp:TextBox>
						</td>
					</tr>	
					<tr>
						<td colspan="2" class="indnt">
							Card Holder Last Name:
						</td>
						<td colspan="2" class="indnt">
						    <asp:TextBox ID="txtLastName" runat="server" Width="227px" MaxLength="60"></asp:TextBox>
						</td>
					</tr>										
					<tr>
						<td style="height: 19px" class="indnt">
							Drug Insurance Plan:
						</td>
						<td colspan="3" style="height: 19px">
							<asp:Label ID="insurancePlanLabel" runat="server" Text="None entered"></asp:Label>
						</td>
					</tr>
				</table>
			</td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="insuranceSearchPanel" runat="server">
                    <tr class="h2title indnt">
			            <td style="width: 759px">
				            <span class="highlight">Insurance Search</span>
			            </td>
                    </tr>
                    <tr>
			            <td style="width: 759px">
				            <table>
					            <tr>
						            <td>
							            Search By
						            </td>
						            <td>
							            <asp:RadioButton ID="rdTopTenPlans" runat="server" Text="Top ten insurance plans" GroupName="SearchOptions" AutoPostBack="false" />
						            </td>
					            </tr>
					            <tr>
						            <td>
							            &nbsp;
						            </td>
						            <td>
							            <asp:RadioButton ID="rdSearchByName" runat="server" Text="Name" GroupName="SearchOptions" />
							            <asp:TextBox ID="txtInsuranceName" runat="server" OnInit="txtInsuranceName_Init" ></asp:TextBox>
							            <asp:Button ID="btnSearhInsurance" runat="server" Text="Search" CssClass="btnstyle" OnClick="btnSearhInsurance_Click" CausesValidation="true" ValidationGroup="insuranceSearch" />
            							
						            </td>
					            </tr>
					            <tr>
						            <td >
							            &nbsp;
						            </td>
						            <td >
							            <asp:CustomValidator ID="insuranceSearchValidation" runat="server" ValidationGroup="insuranceSearch" ClientValidationFunction="ValidateSearchOptions">*</asp:CustomValidator>
						            </td>
					            </tr>
            					
					            <tr>
						            <td colspan="2">
							            <asp:Label ID="lblStatus" runat="server" ForeColor="red"></asp:Label>
						            </td>
					            </tr>
					            <tr>
						            <td colspan="2">
						            Insurance Plan
						            </td>
					            </tr>
					            <tr>
						            <td colspan="2">
							            <asp:ListBox ID="lbInsurancePlan" runat="server" Width="540px" Height="174px" OnDataBound="lbInsurancePlan_DataBound" Rows="10">
								            <asp:ListItem>Please search first...</asp:ListItem>
							            </asp:ListBox>
						            </td>
					            </tr>
				            </table>
			            </td>
                    </tr>
                </asp:Panel>            
            </td>
        </tr>         
        <tr>
            <td>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" Width="299px" />
            </td>
        </tr>
	</table>
	<script type="text/javascript">
	    setAngulrModalStatus();
	</script>
</asp:Content>

<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <%--<asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <contenttemplate>        
         <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex=0 runat="server" ContentCssClass="accordionContent" HeaderCssClass="accordionHeader">
                        <Panes>
                            <ajaxToolkit:AccordionPane ID=paneHelp runat="server">
                            <Header>Help With This Screen</Header>
                            <Content>
                                <asp:Panel ID=HelpPanel runat=server></asp:Panel>
                               </Content>
                            </ajaxToolkit:AccordionPane>                            
                        </Panes>
                    </ajaxToolkit:Accordion>
        </contenttemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
