<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.ProviderErxActivityReport" MasterPageFile="~/PhysicianMasterPageBlank.master"
    Title="Provider eRx Activity Report" Codebehind="ProviderErxActivityReport.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlReport" runat="server" Wrap="false" DefaultButton="btnReport">
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr colspan="6">
                <td>
                    <table border="1" width="100%" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                        <tr>
                            <td class="Phead indnt h1title">
                                Provider eRx Activity Report
                            </td>
                        </tr>
                        <tr class="h2title">
                            <td>
                            </td>
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
                                                        <span class="NormalForScript indnt">(mm/dd/yyyy)</span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtStartDate" runat="server" TabIndex="1" Width="169px"></asp:TextBox><asp:RequiredFieldValidator
                                                            ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate" ErrorMessage="Please enter start date">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                                                ID="revStartDate" runat="server" ErrorMessage="Please enter a valid start date (mm/dd/yyyy)"
                                                                ValidationExpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                                ControlToValidate="txtStartDate" ToolTip="Please enter a valid start date (mm/dd/yyyy)">*</asp:RegularExpressionValidator>
                                                        <asp:CustomValidator ID="cvVerifyStartDate" runat="server" ControlToValidate="txtStartDate"
                                                            ErrorMessage="Start Date cannot be greater than current date" OnServerValidate="cvVerifyStartDate_ServerValidate">*</asp:CustomValidator>
                                                    </td>
                                                    <td colspan="2" rowspan="2" style="width: 400px">
                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" Height="21px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="head indnt">
                                                        End Date
                                                        <br />
                                                        <span class="NormalForScript indnt">(mm/dd/yyyy)</span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtEndDate" runat="server" TabIndex="2" Width="170px"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate"
                                                            ErrorMessage="Please enter end date">*</asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="revEndDate" runat="server" ErrorMessage="Please enter a valid end date (mm/dd/yyyy)"
                                                            ValidationExpression="^((((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00))))$"
                                                            ControlToValidate="txtEndDate" ToolTip="Please enter a valid end date (mm/dd/yyyy)">*</asp:RegularExpressionValidator>
                                                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtStartDate"
                                                            ControlToValidate="txtEndDate" ErrorMessage="End date should be greater than or equal to start date"
                                                            Operator="GreaterThanEqual" Type="Date">*</asp:CompareValidator>
                                                        <asp:CustomValidator ID="cvVerifyEndDate" runat="server" ControlToValidate="txtEndDate"
                                                            ErrorMessage="End date cannot be greater than current date" OnServerValidate="cvVerifyEndDate_ServerValidate">*</asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="head indnt" style="height: 24px;">
                                                        Provider
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlProvider" runat="server" TabIndex="3" Width="170px">
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator ID="rfvProvider" runat="server" ControlToValidate="ddlProvider"
                                                            ErrorMessage="Please select a provider" InitialValue="none">*</asp:RequiredFieldValidator>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="head indnt" style="height: 24px;">
                                                        Include
                                                    </td>
                                                    <td style="height: 24px">
                                                        <asp:DropDownList ID="ddlInclude" runat="server" TabIndex="3" Width="170px">
                                                            <asp:ListItem Text="All" Value="A" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Text="Non Controlled Substance" Value="NCS"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator ID="rfvInclude" runat="server" ControlToValidate="ddlInclude"
                                                            ErrorMessage="Please select a include attribute" InitialValue="none">*</asp:RequiredFieldValidator>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class="h3title">
                            <td>
                                <asp:Button ID="btnClose" CssClass="btnstyle" runat="server" TabIndex="5" Text="Back"
                                    OnClick="btnClose_Click" CausesValidation="false" />
                                <asp:Button ID="btnReport" CssClass="btnstyle" runat="server" TabIndex="4" Text="Display Report"
                                    OnClick="btnReport_Click" ToolTip="Click here to see the report ." />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
