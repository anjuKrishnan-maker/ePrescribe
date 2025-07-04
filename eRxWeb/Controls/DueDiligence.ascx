<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DueDiligence.ascx.cs" Inherits="eRxWeb.Controls_DueDiligence" %>
<%@ Register Src="/Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<style type="text/css">
    .auto-style1 {
        width: 979px;
    }
</style>
<asp:Panel ID="pnlDueDiligencePopUp" runat="server" CssClass="overlaymainwide" Style="display: none; position: relative;">
    <script language="javascript" type="text/javascript">
        function activateDeactivateButton() {
            var chk1 = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_chkGovID').checked;
            var chk2 = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_chkStateAuthorization').checked;
            var chk3 = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_chkCurrentDEA').checked;
            var chk4 = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_chkHealthCareWorkers').checked;
            var btnAccept = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_btnAccept');

            var showOTPForms = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_panelOTP');
            var txtPassword = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_txtPassword');
            var txtOtp = document.getElementById('ctl00_ContentPlaceHolder1_ucDueDiligence_txtOTP');

            if (showOTPForms == null) {
                btnAccept.disabled = !(chk1 && chk2 && chk3 && chk4);
            } else {
                btnAccept.disabled = !(chk1 && chk2 && chk3 && chk4 && (txtPassword.value.length > 0) && (txtOtp.value.length > 0));
            }

        }

    </script>
    <asp:Panel runat="server" ID="panelDueDiligence">
        <div class="overlayTitle">
            EPCS Permission Due Diligence Dialog
        </div>
        <div class="overlayContent" style="margin: 0 auto">
            <div id="message">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </div>
            <p>
                When you assign EPCS permissions to others, you must confirm a number of items are true.
            </p>
            <br />
            <p>
                I certify due diligence to ensure that the selected practitioners are eligible for EPCS as follows:               
            </p>
            <br />


            <div id="divCheckboxes">

                <table>
                    <tr>
                        <td>
                            <asp:CheckBox runat="server" ID="chkGovID" Text="" onclick="activateDeactivateButton()" />
                            <br />
                        </td>
                        <td>
                            <p>Either State or Federal government identification was used to verify their identity.</p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkStateAuthorization" Text="" runat="server" onclick="activateDeactivateButton()" />
                            <br />
                        </td>
                        <td>
                            <p>State authorizations to practice and prescribe controlled substances are current and in good standing.</p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkCurrentDEA" Text="" runat="server" onclick="activateDeactivateButton()" />
                            <br />
                        </td>
                        <td>
                            <p>Either DEA registrations are current, or exception has been granted from the requirement of registration under &sect; 1301.22.</p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkHealthCareWorkers" Text="" onclick="activateDeactivateButton()" runat="server" />
                        </td>
                        <td>
                            <p>If the practitioner is working at healthcare facilities operated by the Department of Veterans Affairs as an employee or at a healthcare facility operated by the Department of Veterans Affairs on a contractual basis, pursuant to 38 U.S.C. 8153, the practitioner has been validated for the eligibility to do so under 38 U.S.C. 7401-7408.
                            </p>
                        </td>
                    </tr>

                </table>
            </div>

            <br />
            <div>
                <asp:Panel ID="panelOTP" runat="server" Visible="True" Height="134px">
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblAuthenticationStatus" runat="server" ForeColor="Red" Font-Bold="true"
                                    Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label Text="User Name" runat="server"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label Text="Password" runat="server"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="txtPassword" TextMode="Password" onchange="activateDeactivateButton()" onkeyup="activateDeactivateButton()" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label Text="Token Device" runat="server"></asp:Label></td>
                            <td>
                                <asp:DropDownList ID="tokenList" runat="server" OnSelectedIndexChanged="tokenList_OnSelectedIndexChanged" AutoPostback=true></asp:DropDownList></td>
                            <td>
                                <asp:Button ID="btnRequestOTP" Text="Request OTP" OnClick="btnRequestOTP_OnClick" CssClass="btnstyle" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label Text="One Time Password(OTP)" runat="server"></asp:Label></td>
                            <td>
                                <asp:TextBox ID="txtOTP" TextMode="Password" onchange="activateDeactivateButton()" onkeyup="activateDeactivateButton()" runat="server"></asp:TextBox></td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
            <div class="overlayFooter">
                <asp:Button ID="btnAccept" CssClass="btnstyle btnStyleAction" runat="server" Text="Accept" Enabled="False" OnClick="btnAccept_OnClick" />
                <asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" Text="Cancel" OnClick="btnCancel_OnClick" />
            </div>
    </asp:Panel>
</asp:Panel>
<asp:Button ID="btnPopUpTrigger" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />
<ajaxToolkit:ModalPopupExtender ID="mpeDueDiligence" runat="server" TargetControlID="btnPopUpTrigger"
    DropShadow="false" PopupControlID="pnlDueDiligencePopUp" BackgroundCssClass="modalBackground">
</ajaxToolkit:ModalPopupExtender>
