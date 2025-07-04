<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.PatientDiagnosis" Title="Patient Problems" Codebehind="PatientDiagnosis.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="h1title">
                <span class="Phead indnt">Patient Diagnosis</span></td>
        </tr>
        <tr class="h2title">
			<td>
			</td>
		</tr>
		<tr class="h4title">
			<td colspan="6">
				<asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" CausesValidation="false"
					OnClick="btnCancel_Click" Text="Back" />&nbsp;&nbsp;
				<asp:Button ID="btnAddDiagnosis" runat="server" CssClass="btnstyle" 
					Text="Add Diagnosis" OnClick="btnAddDiagnosis_Click" />
                &nbsp;&nbsp;
                <asp:RadioButton runat="server" ID ="rbActive" Text="Active" GroupName="Diagnosis" Checked="true"
                    AutoPostBack="true" OnCheckedChanged="rbActive_CheckedChanged"/>
                &nbsp;&nbsp;
                <asp:RadioButton runat="server" ID ="rbInActive" Text="Inactive / Resolved" GroupName="Diagnosis" Checked="false"
                    AutoPostBack="true" OnCheckedChanged="rbInActive_CheckedChanged" />
                &nbsp;&nbsp;
                <asp:RadioButton runat="server" ID ="rbAll" Text="All" GroupName="Diagnosis" Checked="false"
                    AutoPostBack="true" OnCheckedChanged="rbAll_CheckedChanged" />
			</td>				
		</tr>
        <tr height="<%=((PhysicianMasterPage)Master).getTableHeight() %>">
            <td>
                <asp:GridView ID="grdViewPatDiagnosis" CssClass="indnt" Height="108px" runat="server"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CaptionAlign="Left"
                    EmptyDataText="No Known Diagnosis" GridLines="None" PageSize="50" Width="100%"
                    OnRowCommand="grdViewPatDiagnosis_RowCommand" OnRowDeleting="grdViewPatDiagnosis_RowDeleting"
                    OnRowUpdating="grdViewPatDiagnosis_RowUpdating" OnRowDataBound="grdViewPatDiagnosis_RowDataBound"
                    OnRowCreated="grdViewPatDiagnosis_RowCreated" OnPageIndexChanging="grdViewPatDiagnosis_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="Description" HeaderText="Diagnosis" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StartDate" HeaderText="Start Date" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="50px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="UpdatedDate" HeaderText="Inactivated  &nbsp; /&nbsp; Resolved Date" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="50px" />
                        </asp:BoundField>      
                        <asp:BoundField DataField="ICD9Code" HeaderText="ICD-9 Code" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="70px" />
                        </asp:BoundField>                
                        <asp:BoundField DataField="ICD10Code" HeaderText="ICD-10 Code" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                       <asp:BoundField DataField="SnomedCode" HeaderText="SNOMED Code" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StatusDisplay" HeaderText="Status" HeaderStyle-HorizontalAlign="Left">
                            <ItemStyle Width="70px" />
                        </asp:BoundField>
                         
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkbtnInactivate" runat="server" CommandArgument='<%# ObjectExtension.ToEvalEncode(Eval("DXID")) + ";" + ObjectExtension.ToEvalEncode(Eval("CodeType")) %>'
                                    CausesValidation="False" CommandName="Inactivate" Text="Inactivate"></asp:LinkButton>
                                <asp:LinkButton ID="lnkbtnEIE" runat="server" CausesValidation="False" CommandName="EIE"
                                    CommandArgument='<%# ObjectExtension.ToEvalEncode(Eval("DXID"))  + ";" + ObjectExtension.ToEvalEncode(Eval("CodeType")) %>' Text="EIE"></asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                    </Columns>
                    
                    <RowStyle Height="22px" />
                </asp:GridView>
                <asp:ObjectDataSource ID="PatDiagnosisObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}" ConvertNullToDBNull=False
                    SelectMethod="PatientDiagnosis" TypeName="Allscripts.Impact.Patient">
                    <SelectParameters>
                        <asp:SessionParameter Name="patientID" SessionField="PATIENTID" Type="String" />
                        <asp:SessionParameter Name="userID" SessionField="USERID" />
                        <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" />
                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />                        
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
        <script type="text/javascript">
          <% if (Helper.IsAngularMode == true)
          { %>
         RegisterBackButton("<%= btnCancel.ClientID %>");
        <% }%>
        </script>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
   <%-- <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
