<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master"
    Title="Patient Medication History" Inherits="eRxWeb.PatientMedHistory" Codebehind="PatientMedHistory.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        var prevRow;
        var savedClass;

        function onRowClick(row, isRestricted) {

            var btnReport = document.getElementById("ctl00_ContentPlaceHolder1_btnReport");
            if (btnReport != null && isRestricted!= 1) {
                btnReport.disabled = false;
            }
            else {
                btnReport.disabled = true;
            }

            /*var imgSelectSig = document.getElementById("ctl00_ContentPlaceHolder1_imgSelectSig");
            if(imgSelectSig != null)
            {
            imgSelectSig.disabled = false;
            imgSelectSig.src="images/selectSIGNext.gif";
            }*/
            if (prevRow != null) {
                prevRow.className = savedClass;
            }
            savedClass = row.className;
            row.className = 'SelectedRow';
            prevRow = row;

            cleanWhitespace(row);
            row.firstChild.childNodes[0].checked = true;

        }
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
    <asp:Panel ID="pnlReport" runat="server" Wrap="false" DefaultButton="btnReport">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td style="width: 100%;">
                    <table style="width: 100%;" border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td class="Phead indnt h2title">
                                Patient Medication Report
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td class="h4title" colspan="4">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="head indnt">
                                                        Start Date:<br />
                                                        <span class="NormalForScript indnt">(mm/dd/yyyy)</span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtStartDate" runat="server" TabIndex="1"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate" ValidationGroup="abc"
                                                            ErrorMessage="Please enter start date">*</asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revStartDate" runat="server" ErrorMessage="Please enter a valid start date (mm/dd/yyyy)" ValidationGroup="abc"
                                                            ValidationExpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                            ControlToValidate="txtStartDate" ToolTip="Please enter a valid start date (mm/dd/yyyy)">*</asp:RegularExpressionValidator>
                                                        <asp:CustomValidator ID="cvVerifyStartDate" runat="server" ControlToValidate="txtStartDate" OnServerValidate="cvVerifyStartDate_ServerValidate"
                                                            ErrorMessage="Start Date should not be greater than current date">*</asp:CustomValidator>
                                                    </td>
                                                    <td colspan="2" rowspan="2" style="width: 400px">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="21px" ValidationGroup="abc" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="head indnt" style="height: 34px">
                                                        End Date:
                                                        <br />
                                                        <span class="NormalForScript indnt">(mm/dd/yyyy)</span>
                                                    </td>
                                                    <td style="height: 34px">
                                                        <asp:TextBox ID="txtEndDate" runat="server" TabIndex="2"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate" ValidationGroup="abc"
                                                            ErrorMessage="Please enter end date">*</asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revEndDate" runat="server" ErrorMessage="Please enter a valid end date (mm/dd/yyyy)" ValidationGroup="abc"
                                                            ValidationExpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                            ControlToValidate="txtEndDate" ToolTip="Please enter a valid end date (mm/dd/yyyy)">*</asp:RegularExpressionValidator>
                                                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtStartDate" ValidationGroup="abc"
                                                            ControlToValidate="txtEndDate" ErrorMessage="End date should be greater than or equal to start date"
                                                            Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                                        <asp:CustomValidator ID="cvVerifyEndDate" runat="server" ControlToValidate="txtEndDate"
                                                            ErrorMessage="End date should not be greater than current date" OnServerValidate="cvVerifyEndDate_ServerValidate"
                                                            Width="5px">*</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <!--<tr>
   
    <td class="head" style="height: 28px">End Date / Time </td>
    <td colspan="3" style="height: 28px">
        <asp:TextBox ID="txtEndDate1" runat="server" Width="170px"></asp:TextBox></td>
  </tr>-->
                                                
                                                    <tr>
                                                        <td class="head indnt">
                                                            Last name:
                                                        </td>
                                                        <td colspan="2">
                                                        <asp:Panel ID="Panel1" runat="server" Wrap="false" DefaultButton="btnGo">
                                                            <asp:TextBox ID="txtLastNameSearch" runat="server" TabIndex="3"></asp:TextBox>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="head indnt">
                                                            First name:
                                                        </td>
                                                        <td colspan="2">
                                                        <asp:Panel ID="Panel2" runat="server" Wrap="false" DefaultButton="btnGo">
                                                            <asp:TextBox ID="txtFirstNameSearch" runat="server" TabIndex="4"></asp:TextBox>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="head indnt" style="height: 26px">
                                                            Patient ID:
                                                        </td>
                                                        <td colspan="2" style="height: 26px">
                                                        <asp:Panel ID="Panel3" runat="server" Wrap="false" DefaultButton="btnGo">
                                                            <asp:TextBox ID="txtPatientIDSearch" runat="server" TabIndex="5"></asp:TextBox>
                                                            
                                                            <asp:CheckBox ID="chkIncludeInactive" runat="server" TabIndex="6" Text="Include inactive patients in search" />
                                                            </asp:Panel>
                                                            <asp:Button ID="btnGo" CssClass="btnstyle" TabIndex="7" Text="Search" runat="server" CausesValidation="true" ValidationGroup="abc"
                                                                OnClick="btnGo_Click" />
                                                        </td>
                                                    </tr>
                                                </asp:Panel>
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
                                <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#000000">
                                    <tr>
                                        <td colspan="2" class="h4title">
                                            <asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" TabIndex="10" Text="Back"
                                                OnClick="btnCancel_Click" CausesValidation="false" />&nbsp;&nbsp;
                                            <asp:Button ID="btnReport" CssClass="btnstyle" runat="server" TabIndex="9" Text="Display Report"
                                                ToolTip="Click here to see the report ." OnClick="btnReport_Click" />
                                        </td>
                                    </tr>
                                    <tr height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>">
                                        <td valign="top">
                                            <asp:GridView ID="grdViewPatients" runat="server" AllowPaging="True" AllowSorting="True"
                                                Width="100%" DataKeyNames="PatientID, StatusID" AutoGenerateColumns="False" CaptionAlign="Left"
                                                OnRowDataBound="grdViewPatients_RowDataBound" GridLines="None" EmptyDataText=" No Patients Found"
                                                OnRowCreated="grdViewPatients_RowCreated" PageSize="50" TabIndex="8">
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <input name="rdSelectRow" type="radio" tabindex="8" value='<%# ObjectExtension.ToEvalEncode(Eval("PatientID")) %>+<%# ObjectExtension.ToEvalEncode(Eval("Name")) %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="MRN" HeaderText="Patient ID" SortExpression="MRN">
                                                        <ItemStyle Width="70px"></ItemStyle>
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Patient Name" SortExpression="Name">
                                                        <ItemTemplate>
                                                            <asp:Image ID="confidentialIcon" ImageUrl="~\images\PrivacyImages\sensitivehealth-global-16-x-16.png" runat="server" Visible="false" />
                                                            <asp:Label ID="Name" runat="server" Text='<%# ObjectExtension.ToEvalEncode(Eval("Name")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="215px" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="DOB" HeaderText="DOB" ItemStyle-Width="90px" />
                                                    <asp:ButtonField CommandName="Select" />
                                                    <asp:BoundField DataField="Phone" HeaderText="Phone Number">
                                                        <ItemStyle Width="100px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Address" HeaderText="Address">
                                                        <ItemStyle Width="270px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="IsVIPPatient">
                                                        <ItemStyle CssClass="boundfield-hidden" />
                                                        <HeaderStyle CssClass="boundfield-hidden" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="IsRestrictedPatient">
                                                        <ItemStyle CssClass="boundfield-hidden" />
                                                        <HeaderStyle CssClass="boundfield-hidden" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:ObjectDataSource ID="ObjectDSPatient" runat="server" SelectMethod="SearchPatient"
                                                TypeName="Allscripts.Impact.CHPatient" OldValuesParameterFormatString="original_{0}">
                                                <SelectParameters>
                                                    <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                                    <asp:ControlParameter Name="LastName" Type="String" ControlID="txtLastNameSearch"
                                                        DefaultValue="" ConvertEmptyStringToNull="False" />
                                                    <asp:ControlParameter Name="FirstName" Type="String" ControlID="txtFirstNameSearch"
                                                        DefaultValue="" ConvertEmptyStringToNull="False" />
                                                    <asp:ControlParameter Name="ChartID" Type="String" ControlID="txtPatientIDSearch"
                                                        DefaultValue="" ConvertEmptyStringToNull="False" />
                                                    <asp:Parameter Name="WildCard" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                                                    <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                                                    <asp:Parameter Name="HasVIPPatients" Type="Boolean" DefaultValue="false" />
                                                    <asp:SessionParameter Name="UserType" SessionField="UserType" Type="Int16" />
                                                    <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="true" />
                                                    <asp:ControlParameter ControlID="chkIncludeInactive" Name="includeInactive" PropertyName="checked"
                                                        Type="boolean" />
                                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                                </SelectParameters>
                                            </asp:ObjectDataSource>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
   <%-- <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
            <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex="0" runat="server" ContentCssClass="accordionContent"
                HeaderCssClass="accordionHeader">
                <Panes>
                    <ajaxToolkit:AccordionPane ID="paneHelp" runat="server">
                        <Header>
                            Help With This Screen</Header>
                        <Content>
                            <asp:Panel ID="HelpPanel" runat="server">
                            </asp:Panel>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>
            </ajaxToolkit:Accordion>
        </ContentTemplate>
    </asp:UpdatePanel>    --%>
</asp:Content>
