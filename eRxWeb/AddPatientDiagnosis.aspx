<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.AddPatientDiagnosis" Title="Patient Problems" Codebehind="AddPatientDiagnosis.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="UrgentMessages" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <script language="javascript" type="text/javascript">
        window.onload = function () {
            var hiddenFiledIsPlatinumOrDeluxe = document.getElementById("ctl00_ContentPlaceHolder1_hiddenFiledIsPlatinumOrDeluxe").value;
            var divInactiveDate = document.getElementById("divInactiveDate");

            if (hiddenFiledIsPlatinumOrDeluxe == "true") {
                var divRadioBtnActive = document.getElementById("divRadioBtnActive");
                divRadioBtnActive.style.display = 'block';

                var divCheckBoxForActive = document.getElementById("divCheckBoxForActive");
                divCheckBoxForActive.style.display = 'none';
              
            }
            var rdBtnInActive = document.getElementById("ctl00_ContentPlaceHolder1_rdBtnInActive");
            if (rdBtnInActive.checked) {
                divInactiveDate.style.display = 'block';
            }
            else {
                divInactiveDate.style.display = 'none';
            }

        }
        function RadioButtonChange() {
            var rdBtnActive = document.getElementById("ctl00_ContentPlaceHolder1_rdBtnActive");
            var rdBtnInActive = document.getElementById("ctl00_ContentPlaceHolder1_rdBtnInActive");
            var divInactiveDate = document.getElementById("divInactiveDate");


            if (rdBtnInActive.checked) {
                divInactiveDate.style.display = 'block';

            }
            else {
                divInactiveDate.style.display = 'none';

            }
        }

var prevRow;
var savedClass;

function onRowClick(row)
{
    cleanWhitespace(row);
    row.firstChild.childNodes[0].checked=true;
    var btnSave = document.getElementById("ctl00_ContentPlaceHolder1_btnSave");
		if(btnSave != null)
		{
			btnSave.disabled = false;
	        
		}
    if(prevRow!=null)
    {
        prevRow.className=savedClass;
    }
    savedClass=row.className;
    row.className='SelectedRow';
    prevRow=row;
   
}
function cleanWhitespace(node)
{
    for (var x = 0; x < node.childNodes.length; x++) 
    {
        var childNode = node.childNodes[x]
        if ((childNode.nodeType == 3)&&(!/\S/.test(childNode.nodeValue))) 
        {
            // that is, if it's a whitespace text node
            node.removeChild(node.childNodes[x])
            x--
        }
        if (childNode.nodeType == 1) 
        {
            // elements can have text child nodes of their own
            cleanWhitespace(childNode)
        }
    }
}

    </script>
    <asp:Panel ID= "pnlChooseDiagnosis" runat="server"  DefaultButton="btnGo">
    <asp:HiddenField ID="hiddenFiledIsPlatinumOrDeluxe" runat="server" /> 
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td colspan="2">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr class="h1title">
                        <td colspan="2">
							<img src="images/searchBoxImage.png" class="searchControlImage" />
                            <asp:TextBox autocomplete="off" MaxLength="50" ID="txtSearchDx" CssClass="searchControlTextBox"  
								 Width="185px" ToolTip="Enter Partial or Full Diagnosis" runat="server"
                                onblur="if (this.value === '') {this.value = this.getAttribute('defaultValue');this.style.color = 'rgb(102, 102, 102)';}" 
                                onfocus ="if (this.value === this.getAttribute('defaultValue')) {this.value = '';this.style.color = 'black';}"
                                defaultValue="Search Diagnosis"
                                value="Search Diagnosis"
                                ></asp:TextBox>
                                                        
						    <ajaxToolkit:AutoCompleteExtender ID="aceMed" runat="server" TargetControlID="txtSearchDx" CompletionSetCount="20" Enabled="false"
							    ServiceMethod="queryDiagnosis" ServicePath="erxNowDiagnosis.asmx"  CompletionInterval="1000" 
							    MinimumPrefixLength="3" EnableCaching="true">
						    </ajaxToolkit:AutoCompleteExtender>

                            <asp:Button ID="btnGo" CssClass="searchControlButton" causesvalidation="false" runat="server" Text="GO" OnClick="btnGo_Click" ToolTip="Shows all Diagnosis matching the text entered in the above text-box. Enter partial or full Diagnosis name or  ICD code in the Search field."/>
                       </td>
                    </tr>
                    <tr class="h2title">
                    <td><div style="display:none;" id="divUrgentMessage" runat="server"><ePrescribe:UrgentMessages ID="ucMessage" runat="server" /></div>
                    </td>
                    <td align="right" nowrap="nowrap"><asp:Label ID="lblMsg" runat="server" style="display:block;" CssClass="errormsg" Text=""></asp:Label></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
					<tr height="<%=getRowHeight() %>"  style="background-color:#E7e7e7";>
                        <td>
                            <table  width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr class="h4title">
                                    <td style="height: 16px">
                                    </td>
                                    <td colspan="3" style="height: 16px" width="15%">
                                        <input id="btnCancel" class="btnstyle" type="button" runat=server
                                            value="Back" title="Click to go back to the 'Choose patient' page where you can choose another patient." onserverclick="btnCancel_ServerClick" causesvalidation="false"/>
                                            <asp:Button ID="btnSave" runat="server" CssClass="btnstyle" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="DateValidateGroup"
                                            Text="Save" Enabled="false"  ToolTip="Click to save the diagnosis." Height="22px" Width="44px"/></td>
                                            <td>
                                            <table width="100%">
                                            <tr>
                                            <td valign="bottom" width="10%">
                                                <div id="divCheckBoxForActive">
                                                    <asp:CheckBox ID="chkActive" runat="server" Checked="True" Text="Active" />
                                                </div>
                                                <div id="divRadioBtnActive" style="display:none;margin-top:5px;">
                                                <asp:RadioButton ID="rdBtnActive" runat="server" GroupName="Active" onclick="RadioButtonChange()" Checked="true" Text="Active" />
                                                <br />
                                                <asp:RadioButton ID="rdBtnInActive" runat="server" GroupName="Active" onclick="RadioButtonChange()" Text="Resolved" />
                                                  
                                                </div>
                                            </td>
                                            <td valign="bottom">
                                                <table>
                                                <tr>
                                                <td>
                                                    <label>Date when diagnosis first noticed/reported:</label>
                                                    <telerik:RadDatePicker ID="radDatePickerStartDate"  ShowPopupOnFocus="true"  Style="vertical-align: middle;" runat="server">     
                                                    </telerik:RadDatePicker>  <span style="vertical-align:middle">(mm/dd/yyyy)</span>
                                                       <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="Static" runat="server" ControlToValidate="radDatePickerStartDate" ValidationGroup="DateValidateGroup"
                                                    ErrorMessage="Enter a valid diagnosis noticed date"></asp:RequiredFieldValidator>
                                                    <asp:RangeValidator ID="rvStartDate" ControlToValidate="radDatePickerStartDate" Type="Date" ValidationGroup="DateValidateGroup" 
                                                    runat="server"  ErrorMessage="Diagnosis start date must be between patient's dob ({0}) and today ({1})"
                                                    Display="Static"></asp:RangeValidator>
                                                 
                                                    </td>
                                                    </tr>
                                                    <tr>
                                                    <td>
                                                    <div id="divInactiveDate" style="display:none;">
                                                    <label>Date when diagnosis was resolved:</label> 
                                                    <telerik:RadDatePicker ID="radDatePickerResolvedDate"  ShowPopupOnFocus="true"  Style="vertical-align: middle;margin-left:40px;" runat="server" ValidationGroup="DateValidateGroup">     
                                                    </telerik:RadDatePicker>  <span style="vertical-align:middle">(mm/dd/yyyy)</span>
                                                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="radDatePickerResolvedDate" ValidationGroup="DateValidateGroup"
                                                    ErrorMessage="Enter a valid resolved date"></asp:RequiredFieldValidator>
                                                    <asp:CompareValidator ID="compareValResolveDate" runat="server" ErrorMessage="Resolved date must be greater than diagnosis start date." ValidationGroup="DateValidateGroup" ControlToValidate="radDatePickerResolvedDate" Type="Date" ControlToCompare="radDatePickerStartDate" Operator="GreaterThanEqual">
                                                    </asp:CompareValidator>
                                                   
                                                     <asp:RangeValidator ID="RangeValResolvedDate" ControlToValidate="radDatePickerResolvedDate" Type="Date" ValidationGroup="DateValidateGroup" 
                                                    runat="server"  ErrorMessage="Resolved date must be between patient's dob ({0}) and today ({1})"
                                                    Display="Static"></asp:RangeValidator>
                                                    </div>
                                                    </td>
                                                </tr>
                                                </table>
                                             
                                                </td>
                                            </tr>
                                            </table>
                                            
                                            
                                            </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr height="<%=((PhysicianMasterPage)Master).getTableHeight() %>">
                        <td>
                            <asp:GridView ID="grdViewDx" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" Width="100%" CaptionAlign="Left" GridLines="None"
                                EmptyDataText="No diagnosis found"  OnRowDataBound="grdViewDx_RowDataBound"  DataSourceID="ObjDataSourceGetDxList"
                                PageSize="14" OnRowCreated="grdViewDx_RowCreated">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <input name="rdSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("ICD10Code")) + ";" + ObjectExtension.ToEvalEncode(Eval("TERMUID"))%>' />
                                             <asp:HiddenField ID="hdnTermId" runat="server" Value= '<%# ObjectExtension.ToEvalEncode(Eval("TERMUID")) %>'/>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="30px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Description" HeaderText="Diagnosis" SortExpression="Description">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle Width="300px" HorizontalAlign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ICD10Code" HeaderText="ICD-10 Code" SortExpression="ICD10Code">
                                          <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="100px"  HorizontalAlign="Left"/>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="SnomedCode" HeaderText="SNOMED Code" SortExpression="SnomedCode">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="100px" HorizontalAlign="Left"/>
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="ObjDataSourceGetATPDxList" runat="server" SelectMethod="GetATPDxList" OnSelected="ObjDataSourceGetATPDxList_Selected"
                                TypeName="Allscripts.Impact.CHDiagnosis" OldValuesParameterFormatString="original_{0}" OnSelecting="ObjDataSourceGetATPDxList_Selecting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtSearchDx" ConvertEmptyStringToNull="False" Name="Phrase"
                                        PropertyName="Text" Type="String" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="ObjDataSourceGetDxList" runat="server" SelectMethod="GetDxList" OnSelected="ObjDataSourceGetDxList_Selected"
                                TypeName="Allscripts.Impact.CHDiagnosis" OldValuesParameterFormatString="original_{0}" OnSelecting="ObjDataSourceGetDxList_Selecting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtSearchDx" ConvertEmptyStringToNull="False" Name="Phrase"
                                        PropertyName="Text" Type="String" />
                                    <asp:SessionParameter Name="providerID" SessionField="USERID" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </asp:Panel>
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

