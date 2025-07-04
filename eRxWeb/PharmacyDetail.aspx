<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.PharmacyDetail" Title="Pharmacy Utilization Detail" Codebehind="PharmacyDetail.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr colspan="2">
            <td>
                <table border="1" width="100%" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    
                    <tr>
                        <td class="Phead indnt h1title" >
                            Pharmacy Utilization Detail </td>
                    </tr>
                    <tr class="h2title">
                    <td></td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellspacing="0">
                                <tr>
                                    <td>
                                        <table class="report-table-width" cellspacing="0">
                                            <tr>
                                                <td colspan="1" class="head indnt">
                                                    Start Date
                                                    <br />
                                                    <span class="NormalForScript indnt">(mm/dd/yyyy)</span></td>
                                                <td class="indnt">
                                                    &nbsp;<asp:textbox id="txtStartDate" runat="server" TabIndex="1" width="169px"></asp:textbox><asp:requiredfieldvalidator id="rfvStartDate" runat="server" controltovalidate="txtStartDate"
                                                        errormessage="Please enter start date">*</asp:requiredfieldvalidator><asp:regularexpressionvalidator id="revStartDate" runat="server" errormessage="Please enter a valid start date (mm/dd/yyyy)"
                                                        validationexpression="(0*[1-9]|1[012])[- /.](0*[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"
                                                        controltovalidate="txtStartDate" tooltip="Please enter a valid start date (mm/dd/yyyy)">*</asp:regularexpressionvalidator><asp:CustomValidator
                                                            ID="cvVerifyStartDate" runat="server" ControlToValidate="txtStartDate" ErrorMessage="Start Date cannot be greater than current date"
                                                            OnServerValidate="cvVerifyStartDate_ServerValidate">*</asp:CustomValidator></td>
                                                <td colspan="2" rowspan="2" style="width: 400px" > 
                                                  <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="21px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="head indnt">
                                                    End Date
                                                    <br />
                                                    <span class="NormalForScript indnt">(mm/dd/yyyy)</span></td>
                                                <td >
                                                    &nbsp; &nbsp;
                                                    
                                                    <asp:textbox id="txtEndDate" runat="server" TabIndex="2" width="170px"></asp:textbox>
                                                    <asp:requiredfieldvalidator id="rfvEndDate" runat="server" controltovalidate="txtEndDate"
                                                        errormessage="Please enter end date">*</asp:requiredfieldvalidator>
                                                  
                                                    <asp:regularexpressionvalidator id="revEndDate" runat="server" errormessage="Please enter a valid end date (mm/dd/yyyy)"
                                                        validationexpression="(0*[1-9]|1[012])[- /.](0*[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"
                                                        controltovalidate="txtEndDate" tooltip="Please enter a valid end date (mm/dd/yyyy)">*</asp:regularexpressionvalidator>
                                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtStartDate"
                                                    ControlToValidate="txtEndDate" ErrorMessage="End date should be greater than or equal to start date" Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                                    <asp:CustomValidator ID="cvVerifyEndDate" runat="server" ControlToValidate="txtEndDate"
                                                        ErrorMessage="End date cannot be greater than current date" OnServerValidate="cvVerifyEndDate_ServerValidate">*</asp:CustomValidator></td>
                                            </tr>
                                            
                                              <tr>
                                                
                                                
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                   
                    <tr class="h3title">
                        <td>
                            <!--<asp:button id="btnPrint" cssclass="btnstyle" runat="server" text="Print Report"
                                />&nbsp;&nbsp;-->
                                 <asp:button id="btnClose" cssclass="btnstyle" runat="server" TabIndex="4" text="Back" onclick="btnClose_Click"
                                causesvalidation="false" />
                                
                            <asp:button id="btnReport" cssclass="btnstyle" runat="server" TabIndex="3" text="Display Report"
                                                        onclick="btnReport_Click" ToolTip="Click here to see the report ." />
                                                    
                           
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
