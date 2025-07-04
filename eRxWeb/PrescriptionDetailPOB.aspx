<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Inherits="eRxWeb.PrescriptionDetailPOB" Title="Provider Report (Prescribe on Behalf of)" Codebehind="PrescriptionDetailPOB.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Panel ID= "pnlReport" runat="server" Wrap="false" DefaultButton="btnReport">
<table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr colspan="6">
            <td>
                <table border="1" width="100%" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                                <tr>
                        <td class="Phead indnt h1title" >
                            <asp:Label ID="lblReportTitle" runat="server" Text="Provider Report (Prescribe on Behalf of)" CssClass="Phead indnt h1title"></asp:Label></td>
                    </tr>
                    <tr class="h2title">
                    <td></td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellspacing="0">
                                <tr>
                                    <td style="height: 86px">
                                        <table class="report-table-width" cellspacing="0">
                                            <tr>
                                                <td colspan="1" class="head indnt">
                                                    Start Date
                                                    <br />
                                                    <span class="NormalForScript indnt">(mm/dd/yyyy)</span></td>
                                                <td>
                                                   <asp:textbox id="txtStartDate" runat="server" TabIndex="1" width="169px"></asp:textbox><asp:requiredfieldvalidator id="rfvStartDate" runat="server" controltovalidate="txtStartDate"
                                                        errormessage="Please enter start date">*</asp:requiredfieldvalidator><asp:regularexpressionvalidator id="revStartDate" runat="server" errormessage="Please enter a valid start date (mm/dd/yyyy HH:MM AM)"
                                                        validationexpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                        controltovalidate="txtStartDate" tooltip="Please enter a valid start date (mm/dd/yyyy)">*</asp:regularexpressionvalidator>
                                                    <asp:CustomValidator ID="cvVerifyStartDate" runat="server" ControlToValidate="txtStartDate"
                                                        ErrorMessage="Start Date cannot be greater than current date" OnServerValidate="cvVerifyStartDate_ServerValidate">*</asp:CustomValidator></td>
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
                                                    
                                                    <asp:textbox id="txtEndDate" runat="server" TabIndex="2" width="170px"></asp:textbox>
                                                    <asp:requiredfieldvalidator id="rfvEndDate" runat="server" controltovalidate="txtEndDate"
                                                        errormessage="Please enter end date">*</asp:requiredfieldvalidator>
                                                  
                                                    <asp:regularexpressionvalidator id="revEndDate" runat="server" errormessage="Please enter a valid end date (mm/dd/yyyy HH:MM AM)"
                                                        validationexpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                        controltovalidate="txtEndDate" tooltip="Please enter a valid end date (mm/dd/yyyy)">*</asp:regularexpressionvalidator>
                                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtStartDate" Enabled=false
                                                    ControlToValidate="txtEndDate" ErrorMessage="End date should be greater than or equal to start date" Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                                    <asp:CustomValidator ID="cvVerifyEndDate" runat="server" ControlToValidate="txtEndDate"
                                                        ErrorMessage="End date cannot be greater than current date and must be greater than the start date" OnServerValidate="cvVerifyEndDate_ServerValidate">*</asp:CustomValidator></td>
                                                
                                            </tr>
                                                  <tr>
                                                <td class="head indnt">
                                                    Provider </td>
                                                <td >
                                                    <asp:DropDownList ID="ddlProvider" runat="server"  TabIndex="3" Width="170px">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvProvider" runat="server" ControlToValidate="ddlProvider"
                                                        ErrorMessage="Please select a provider" InitialValue="none">*</asp:RequiredFieldValidator>
                                                </td>
                                                                                        
                                                
                                            </tr>
                                          
                                            <tr>
               
                                            <td class="head indnt"> POB </td>
                                            <td>
                                            <asp:DropDownList ID="ddlPOB" runat="server" TabIndex="4" Width="170px">
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="rvfPOB" runat="server" ControlToValidate="ddlPOB"
                                                        ErrorMessage="Please select a POB" InitialValue="none">*</asp:RequiredFieldValidator>
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
                                 <asp:button id="btnClose" cssclass="btnstyle" runat="server" TabIndex="6" text="Back" onclick="btnClose_Click"
                                causesvalidation="false" />
                            <asp:button id="btnReport" cssclass="btnstyle" runat="server" TabIndex="5" text="Display Report"
                                                        onclick="btnReport_Click" ToolTip="Click here to see the report ." />
                                                    
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </asp:Panel>
</asp:Content>
