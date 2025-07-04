<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master"
    Inherits="eRxWeb.PatientMedReport" Title="Medication Report" Codebehind="PatientMedReport.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<script language="javascript" type="text/javascript">
var prevRow;
var savedClass;

function onRowClick(row)
{

var btnReport=document.getElementById("ctl00_ContentPlaceHolder1_btnReport");
if(btnReport != null)
{
    btnReport.disabled = false;
}

/*var imgSelectSig = document.getElementById("ctl00_ContentPlaceHolder1_imgSelectSig");
if(imgSelectSig != null)
{
imgSelectSig.disabled = false;
imgSelectSig.src="images/selectSIGNext.gif";
}*/
if(prevRow!=null)
{
prevRow.className=savedClass;
}
savedClass=row.className;
row.className='SelectedRow';
prevRow=row;

cleanWhitespace(row);
row.firstChild.childNodes[0].checked=true;

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
<asp:Panel ID= "pnlReport" runat="server" Wrap="false" DefaultButton="btnReport">
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    
    <tr>
        <td style="width: 100%; ">
            <table style="width: 100%; " border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td class="Phead indnt h1title">
                        Medication Report</td>
                </tr>
                <tr class="h2title"><td></td></tr>
                <tr>
                    <td>
                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 129px" >
                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                    <td class="h4title" colspan="4" style="height: 18px"></td>
                                    </tr>
                                        <tr>
                                            <td class="head indnt" style="width: 140px">
                                                Start Date<br />
                                                <span class="NormalForScript indnt">(mm/dd/yyyy)</span></td>
                                            <td style="width: 262px" >
                                                <asp:textbox id="txtStartDate" runat="server"  width="169px" TabIndex="1"></asp:textbox><asp:requiredfieldvalidator id="rfvStartDate" runat="server" controltovalidate="txtStartDate"
                                                    errormessage="Please enter start date">*</asp:requiredfieldvalidator>
                                                
                                                <asp:regularexpressionvalidator id="revStartDate" runat="server" errormessage="Please enter a valid start date (mm/dd/yyyy)"
                                                    ValidationExpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                    controltovalidate="txtStartDate" tooltip="Please enter a valid start date (mm/dd/yyyy)">*</asp:regularexpressionvalidator>
                                                <asp:CustomValidator ID="cvVerifyStartDate" runat="server" ControlToValidate="txtStartDate"
                                                    ErrorMessage="Start Date should not be greater than current Date" Height="3px" OnServerValidate="cvVerifyStartDate_ServerValidate"
                                                    Width="1px">*</asp:CustomValidator></td>
                                                 <td style="width: 400px" colspan="2" rowspan="2">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="10px" />
                                                    </td>
                                                 
                                        </tr>
                                        <tr>
                                            <td class="head indnt" style="height: 34px">
                                                End Date
                                                <br />
                                                <span class="NormalForScript indnt">(mm/dd/yyyy)</span></td>
                                            <td style="height: 34px; width: 262px;" >
                                                <asp:textbox id="txtEndDate" runat="server"  width="170px" TabIndex="2"></asp:textbox>
                                                <asp:requiredfieldvalidator id="rfvEndDate" runat="server" controltovalidate="txtEndDate"
                                                    errormessage="Please enter end date" Text="*"></asp:requiredfieldvalidator>
                                               
                                                <asp:regularexpressionvalidator id="revEndDate" runat="server" errormessage="Please enter a valid end date (mm/dd/yyyy)"
                                                    ValidationExpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                    controltovalidate="txtEndDate" tooltip="Please enter a valid end date (mm/dd/yyyy)">*</asp:regularexpressionvalidator>
                                               
                                                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtStartDate"
                                                    ControlToValidate="txtEndDate" ErrorMessage="End date should be greater than or equal to start date" Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                                <asp:CustomValidator ID="cvVerifyEndDate" runat="server" ControlToValidate="txtEndDate"
                                                    ErrorMessage="End Date should not be greater than current date" OnServerValidate="cvVerifyEndDate_ServerValidate"
                                                    Width="1px">*</asp:CustomValidator></td>
                                        </tr>
                                        <!--<tr>
   
    <td class="head" style="height: 28px">End Date / Time </td>
    <td colspan="3" style="height: 28px">
        <asp:TextBox ID="txtEndDate1" runat="server" Width="170px"></asp:TextBox></td>
  </tr>-->
                                        <tr>
                                            <td class="head indnt">
                                                Search Medication
                                            </td>
                                            <td style="width: 262px">
                                            <asp:Panel ID= "Panel1" runat="server" Wrap="false" DefaultButton="btnGo">
                                                <asp:textbox MaxLength="30" id="txtSearchMed" runat="server" width="170px" TabIndex="3"></asp:textbox>
                                                <asp:button id="btnGo" cssclass="btnstyle" Text="Search" runat="server" onclick="btnGo_Click" TabIndex="4" />
                                                </asp:Panel>
                                                <!--<asp:DropDownList ID="ddlUser" runat="server" Width="175px">
        </asp:DropDownList>-->
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
               <%-- <tr class="h4title">
                    <td colspan="2">
                        &nbsp;<span class="Phead indnt">Choose Medication </span>
                    </td>
                </tr>--%>
                <tr>
                    <td colspan="2">
                        
                            <table width="100%"  border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
								<tr>
									<td colspan="2" class="h4title">
										<asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" TabIndex="7" Text="Back" OnClick="btnCancel_Click"
											CausesValidation="false" />&nbsp;&nbsp;
										<asp:Button ID="btnReport" CssClass="btnstyle" runat="server" Text="Display Report"
											ToolTip="Click here to see the report ." OnClick="btnReport_Click" TabIndex="6" />
										
									
									</td>
								</tr>
                                <tr height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>">
                                    <td valign="top" >
                                        <asp:gridview id="grdViewDrug" runat="server" allowpaging="True" allowsorting="True"
                                            datasourceid="odsDrugList" autogeneratecolumns="False" width="100%" captionalign="Left"
                                            gridlines="horizontal" emptydatatext="No Medication Found" datakeynames="name"
                                            pagesize="50" onrowdatabound="grdViewDrug_RowDataBound">
                                               
                                                <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                <input name="rdSelectRow" type="radio"  tabindex="5" value='<%# ObjectExtension.ToEvalEncode(Eval("name")) %>'  />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Name" HeaderText="Drug Name" SortExpression="Name" />
                                                </Columns>
                                                </asp:gridview>
                                        <asp:objectdatasource id="odsDrugList" runat="server" selectmethod="GetDrugs" ConvertNullToDBNull="False" typename="Allscripts.Impact.Medication">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="txtSearchMed" Name="drugName" PropertyName="Text" Type="String" /> 
                                                <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />                                                  
                                            </SelectParameters>
                                          </asp:objectdatasource>
                                    </td>
                                </tr>
                            </table>
                       
                    </td>
                </tr>
            </table>
</table>
</asp:Panel>

 </asp:Content>