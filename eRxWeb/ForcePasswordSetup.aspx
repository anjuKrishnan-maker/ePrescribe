<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true"
    Inherits="eRxWeb.ForcePasswordSetup" Title="Password setup" CodeBehind="ForcePasswordSetup.aspx.cs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
      <asp:Panel ID="pnlChangePassword" runat="server" DefaultButton="btnSetupPassword">
    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="height: 500px">
        <tr>
            <td>
                    <table width="100%" border="0" cellpadding="2" cellspacing="0" class="indnt">
                        <tr class="h1title">
                            <td colspan="2">
                                <span class="Phead indnt" runat="server" id="spanTitle">Password Setup</span>
                            </td>
                        </tr>
                        <tr class="h4title">
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <ePrescribe:Message ID="ucErrorMessage" runat="server" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table width="100%" style="background: #fffacd; border: solid 1px DarkGray">
                                    <tr valign="middle">
                                        <td style="width: 25px">
                                            <asp:Image ID="imgStatus" runat="server" ImageUrl="../images/information.png" />
                                        </td>
                                        <td valign="middle">
                                            <div style="color: Maroon;" runat="server" id="divPasswordSetup">
                                                <b>Why am I seeing this?</b>
                                                <p style="margin: 0 0 0 0; color: Black;">
                                                    Your current practice settings allow you to setup a password that can be used for
                                                certain features such as:
                                                </p>
                                                <ul>
                                                    <li>Setting up users for the electronic prescribing of controlled substances</li>
                                                    <li>Prescribing controlled substances electronically</li>
                                                    <li>Accessing the iPhone and Mobile versions of ePrescribe</li>
                                                </ul>
                                                <p style="margin: 0 0 0 0;">
                                                    Note that these features are dependant on your current practice configuration. Please
                                                contact your system administrator with questions.
                                                </p>
                                            </div>
                                            <div style="color: Maroon;" runat="server" id="divPasswordExpired" visible="false">
                                                <b>Why am I seeing this?</b>
                                                <p style="margin: 0 0 0 0; color: Black;">
                                                    Your Password has expired.
                                                </p>
                                                <br />
                                                <p style="margin: 0 0 0 0; color: Black;">
                                                    If you select “<strong>Skip change Password For Now</strong>” you will not be able to send CS scripts electronically and will encounter authentication error.
                                                </p>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td align="left" colspan="2">
                                            <asp:ValidationSummary ID="vsChangePassword" runat="server" Font-Bold="true" ValidationGroup="main" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trUsername">
                                        <td style="width: 294px; text-align: right">
                                            <b>Username:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblUserName" runat="server"></asp:Label>
                                            <asp:ImageButton ID="imgBtnHelpUserName" runat="server" ImageUrl="~/images/question.png" />
                                            <telerik:RadToolTip runat="server" ID="radToolTipHelpUserName" ShowEvent="OnClick" ShowDelay="0" Overlay="true" Position="BottomRight"
                                                HideEvent="ManualClose" Width="450px" TargetControlID="imgBtnHelpUserName" Modal="true" RelativeTo="Element" EnableShadow="false">
                                                <div style="font-size: medium; font-weight: bold;">What is this?</div>
                                                <br />
                                                <div>
                                                    <p style="padding-bottom: 5px; color: Black">This is your ePrescribe username. It may or may not be the same username that you used to sign into your other system. Currently you cannot change this username.</p>
                                                </div>
                                            </telerik:RadToolTip>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="trCurrentPassword" visible="false">
                                        <td style="width: 294px; text-align:right">
                                            <span style="color: Red">*</span> <b>Current Password:</b> 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtOldPassword" runat="server" ValidateRequestMode="Disabled" MaxLength="20" TextMode="Password" Width="110px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" ControlToValidate="txtOldPassword" ErrorMessage="Current password is required." ValidationGroup="main" Display="Dynamic">*</asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 294px; text-align: right">
                                            <span style="color: Red">*</span> <b><label runat="server" id="lblPassword"> Password:</label></b>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtPassword" runat="server" ValidateRequestMode="Disabled" MaxLength="20" TextMode="Password" Width="110px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvPwd" runat="server" ControlToValidate="txtPassword" ErrorMessage="Please enter a password." ValidationGroup="main">*</asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password does not meet the minimum requirements."
                                                OnServerValidate="cvPassword_ServerValidate">*</asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 294px; text-align: right">
                                            <span style="color: Red">*</span> <b>Confirm Password:</b>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtConfirmPwd" runat="server" ValidateRequestMode="Disabled" MaxLength="20" TextMode="Password" Width="110px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvConfirmPwd" runat="server" ControlToValidate="txtConfirmPwd"
                                                ErrorMessage="Please enter a confirm password." ValidationGroup="main">*</asp:RequiredFieldValidator>
                                            <asp:CompareValidator ID="cvConfirmPwd" runat="server" ControlToCompare="txtPassword"
                                                ControlToValidate="txtConfirmPwd" ErrorMessage="Confirm Password entry must match Password." ValidationGroup="main">*</asp:CompareValidator>
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                  <tr>
                                    <td style="width: 45%; text-align: right; height: 10px; padding-top: 5px;">
                                       <asp:Button ID="btnSetupPassword" runat="server" CssClass="btnstyle" Text="Setup Password"
                                         OnClick="btnSetupPassword_Click" ToolTip="Click here to set your password." ValidationGroup="main" />
                                       <asp:Button ID="btnSkipSetupPassword" runat="server" CssClass="btnstyle" Text="Skip Setup For Now"
                                         OnClick="btnSkipSetupPassword_Click" ToolTip="Click here to skip the password setup." />
                                    </td>
                                  </tr>
                               </table>
                            </td>
                            <td>
                                <b>Password requirements:</b>
                                <br />
                                <ul>
                                    <li>At least 8 characters and Maximum 25 characters</li>
                                    <li>At least 3 of the 4:</li>
                                    <ul>
                                        <li style="list-style: square inside">1 upper case letter</li>
                                        <li style="list-style: square inside">1 lower case letter</li>
                                        <li style="list-style: square inside">1 numeric character</li>
                                        <li style="list-style: square inside">1 punctuation character</li>
                                        <div runat="server" id="passwordInstructionDiv" Visible="false">
                                        <li style="list-style: square inside">Cannot reuse any of your 10 most recent passwords</li>
                                        <li style="list-style: square inside">Password must not have been changed within the last 24 hours</li>
                                        <li style="list-style: square inside">User must not have been created within the last 24 hours</li>
                                        </div>
                                    </ul>
                                </ul>
                            </td>
                        </tr>
                   </table>
            </td>
        </tr>
    </table>
</asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>