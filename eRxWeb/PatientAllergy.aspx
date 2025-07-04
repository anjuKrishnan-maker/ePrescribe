<%@ Page Language="C#" Title="Patient Allergy" AutoEventWireup="true"
    MasterPageFile="~/PhysicianMasterPage.master" Inherits="eRxWeb.PatientAllergy" Codebehind="PatientAllergy.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="h1title">
                <span class="Phead indnt">Patient Allergy</span></td>
        </tr>
        <tr class="h2title">
			<td>
				<asp:RadioButton ID="rbActive" runat="server" AutoPostBack="True" Checked="True"
					GroupName="FilterAllergy" Text="Active" OnCheckedChanged="btFilterAllergy_CheckedChanged"
					CssClass="adminlink1" />
				<asp:RadioButton ID="rbInactive" runat="server" AutoPostBack="True" GroupName="FilterAllergy"
					Text="Inactive" OnCheckedChanged="btFilterAllergy_CheckedChanged" CssClass="adminlink1" />
				<asp:RadioButton ID="rbAll" runat="server" AutoPostBack="True" GroupName="FilterAllergy"
					Text="All" OnCheckedChanged="btFilterAllergy_CheckedChanged" CssClass="adminlink1" />
			</td>
		</tr>
		<tr class="h4title">
			<td colspan="6">
				<asp:Button ID="btnBack" runat="server" CssClass="btnstyle" CausesValidation="false"
					OnClick="btnBack_Click" Text="Back" />&nbsp;&nbsp;
				<asp:Button ID="btnAddAllergy" runat="server" CssClass="btnstyle" OnClick="btnAddAllergy_Click"
					Text="Add Allergy" />&nbsp;&nbsp;
				<asp:Button ID="btnNKA" runat="server" CssClass="btnstyle" OnClick="btnNKA_Click"
					Text="No Known Allergies" />
				
			</td>
		</tr>
        <tr height="<%=((PhysicianMasterPage)Master).getTableHeight() %>">
            <td>
                <asp:GridView ID="grdViewPatAllergy" CssClass="indnt" Height="108px" runat="server"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CaptionAlign="Left"
                    GridLines="None" PageSize="50" Width="100%"
                    OnRowCommand="grdViewPatAllergy_RowCommand" OnRowDeleting="grdViewPatAllergy_RowDeleting"
                    OnRowUpdating="grdViewPatAllergy_RowUpdating" OnRowDataBound="grdViewPatAllergy_RowDataBound"
                    OnRowCreated="grdViewPatAllergy_RowCreated" OnPageIndexChanging="grdViewPatAllergy_PageIndexChanging" 
                    OnDataBinding="grdViewPatAllergy_OnDataBinding" DataKeyNames= "AllergyType, ClassActiveStatus">
                    <Columns>
                        <asp:BoundField DataField="AllergyName" HeaderText="Class/Medication">
                            <ItemStyle Width="200px" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Reaction">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblReaction" Text='<%# getAllergyReaction(Eval("PatientAllergyID").ToString() ) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="AllergyCategory" HeaderText="Type" />
                        <asp:BoundField DataField="Active" HeaderText="Active">
                            <ItemStyle Width="30px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Updated" HeaderText="Updated Date">
							<ItemStyle Wrap="False" Width="100px" />
                        </asp:BoundField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkbtnEdit" runat="server" CommandArgument='<%# ObjectExtension.ToEvalEncode(Eval("PatientAllergyID")) %>'
                                    CausesValidation="False" CommandName="Edit" Text="Edit"></asp:LinkButton>
                                <asp:LinkButton ID="lnkbtnEIE" runat="server" CausesValidation="False" CommandName="EIE"
                                    CommandArgument='<%# ObjectExtension.ToEvalEncode(Eval("PatientAllergyID")) %>' Text="EIE"></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                    </Columns>
                    
                    <RowStyle Height="22px" />
                </asp:GridView>
                <asp:ObjectDataSource ID="PatAllergyObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PatientAllergy" TypeName="Allscripts.Impact.Patient">
                    <SelectParameters>
                        <asp:SessionParameter Name="patientID" SessionField="PATIENTID" Type="String" />
                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
       <% if (Helper.IsAngularMode == true)
          { %>
         RegisterBackButton("<%= btnBack.ClientID %>");
        <% }%>
    </script>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
  <%--  <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
