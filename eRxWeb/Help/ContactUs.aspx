<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.ContactUs" Title="Veradigm ePrescribe Help - Contact Us" CodeBehind="ContactUs.aspx.cs" %>

<%@ Register TagPrefix="ePrescribe" TagName="Message" Src="~/Controls/Message.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
</script>
    <table>
        <tr>
            <td colspan="3">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">First Name:</td>
            <td style="width: 246">
                <label id="lblAccountName" runat="server">Account Name:</label></td>
        </tr>
        <tr>
            <td colspan="2">
                <label>
                    <input type="text" name="first_name" id="first_name" runat="server" />
                </label>
            </td>
            <td>
                <label>
                    <input type="text" name="account_name" id="account_name" runat="server" />
                </label>

            </td>
        </tr>
        <tr>
            <td colspan="2">Last Name:</td>
            <td>
                <label id="lblAccountNumber" runat="server">Account Number:</label></td>
        </tr>
        <tr>
            <td colspan="2">
                <label>
                    <input type="text" name="last_name" id="last_name" runat="server" />
                </label>
            </td>
            <td>
                <label>
                    <input type="text" name="account_number" id="account_number" runat="server" />
                </label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <label id="lblUserName" runat="server">User Name:</label></td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <label>
                    <input type="text" name="user_name" id="user_name" runat="server" />
                </label>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">Email: (Required)</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <label>
                    <input type="text" name="email" id="email" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvCardNumber" runat="server" ValidationGroup="maingroup" ControlToValidate="email"
                        ErrorMessage="Please enter a valid email address." Height="9px"><b style="color:Red">*</b>
                    </asp:RequiredFieldValidator>
                </label>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="3"><span style="color: Red">*</span>Please provide below a short description of the problem you are encountering. 
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <label>
                    <input style="width: 90%" type="text" name="shortDesc" id="ShortDesc" runat="server" maxlength="25" />
                    <%--                   <textarea name="shortDesc" id="ShortDesc" cols="45" rows="2" runat="server" ></textarea>--%>
                </label>
            </td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="3"><span style="color: Red">*</span>In addition, please provide the following details:
            <ul>
                <li><strong>If a password reset is needed, please supply the following:</strong>
                    <ul>
                        <li>DEA Number and Expiration Date</li>
                        <li>NPI Number</li>
                        <li>State License Number and Expiration Date</li>
                        <li>Email address for the temporary password</li>
                    </ul>
                </li>
                <li>If you are not the provider, please enter the provider’s full name</li>
                <li>If you need assistance with an error message, provide the error message
                    <br />
                    along with steps to recreate the error</li>
                <li>Other – please provide as much detail as possible to assist support in
                    <br />
                    resolving the issue</li>
            </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <label>
                    <textarea name="comments" id="comments" cols="60" rows="10" runat="server"></textarea>
                </label>
            </td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">Captcha 
            <asp:ImageButton runat="server" src="/images/registration/Refresh.gif" OnClick="ResetCaptcha" /></td>
        </tr>
        <tr>
            <td colspan="2">
                <img alt="" src="../CreateCaptcha.aspx" runat="server" id="imgCapcha" class="divCapchaImage" />
            </td>
            <td colspan="1">Type the code here:<span style="color: red">*</span>
                <div style="display:block">
                    <asp:TextBox runat="server" CssClass="textBoxes" ID="txtCapchaResponse"></asp:TextBox>
                </div>

            </td>

        </tr>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary ID="valsum" runat="server" ValidationGroup="maingroup" />
            </td>
        </tr>
        <tr>
            <td width="90">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btnStyle" OnClick="btnSubmit_Click" CausesValidation="true" ValidationGroup="maingroup" />
            </td>
            <td width="110">
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btnStyle" OnClick="btnReset_Click" />
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>

