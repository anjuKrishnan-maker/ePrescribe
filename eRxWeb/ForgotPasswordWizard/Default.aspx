<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eRxWeb.ForgotPasswordWizard.Default" %>
<%@ Register TagPrefix="ePrescribe" TagName="Message" Src="~/Controls/Message.ascx" %>

<!DOCTYPE html>
<script type="text/javascript">
    function validateConfirmPassword(txtConfirmPassword)
    {
        var btnSubmit = document.getElementById('<%=btnSubmit.ClientID%>');
        var txtNewPassword = document.getElementById('<%=txtNewPassword.ClientID%>');
        var divConfrimPasswordError = document.getElementById('divConfrimPasswordError');

        if (txtConfirmPassword != null && btnSubmit != null && txtNewPassword != null)
        {
            if(txtConfirmPassword.value !== txtNewPassword.value)
            {
                txtConfirmPassword.className = 'errorBorder';
                btnSubmit.disabled = true;
                divConfrimPasswordError.style.display = 'block';
            }
            else
            {
                txtConfirmPassword.className = '';
                btnSubmit.disabled = false;
                divConfrimPasswordError.style.display = 'none';
            }
        }
    }

    function validatePassword()
        {
            var div = document.getElementById('divPasswordExplination');
            var txtPassword = document.getElementById('<%=txtNewPassword.ClientID%>');
            var txtPasswordValue = txtPassword.value;
            var btnSubmit = document.getElementById('<%=btnSubmit.ClientID%>');

            if (txtPasswordValue === '')
            {
                btnSubmit.disabled = true;
                return;
            }

            if (txtPasswordValue.length < 8 || txtPasswordValue.length > 25)
            {
                div.style.display = 'block';
                txtPassword.className = 'errorBorder';
                btnSubmit.disabled = true;
                return;
            }

            var options = 0;
            if (/[a-z]/.test(txtPasswordValue))
                options++;
            if (/[A-Z]/.test(txtPasswordValue))
                options++;
            if (/[!@#\$%\^\&*\)\(+=._-]/.test(txtPasswordValue))
                options++;
            if (/[0-9]/.test(txtPasswordValue))
                options++;

            if (options < 3)
            {
                div.style.display = 'block';
                txtPassword.className = 'errorBorder';
                btnSubmit.disabled = true;
                return;
            }

            div.style.display = 'none';
            txtPassword.className = '';
            btnSubmit.disabled = false;
        }
</script>
<link href="../Style/forgotPassword.css" rel="stylesheet" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="masterScriptManager" runat="server" EnablePartialRendering="true"
        ScriptMode="Release">
    </asp:ScriptManager>
        <div id="divLogo">
                <img id="loginLogo" src="../images/shim.gif" alt="" width="155" height="105" />
        </div>
        <div id="divHeader">
            Forgot Password
        </div>
        <ePrescribe:Message ID="ucMessage" Icon="ERROR" runat="server" Visible="false" />
        <asp:Panel ID="pnlContainer" CssClass="steps" runat="server">
            <div class="floatleft divLeft">
                &nbsp;&nbsp;&nbsp;<span class="stepText">Step 1.</span>
                <br/>
                <img src="../images/required.png" />&nbsp;Login ID
                <asp:Panel ID="pnlStep2Text" Visible="False" runat="server" >
                    <br/>
                    &nbsp;&nbsp;&nbsp;<span class="stepText">Step 2.</span>
                </asp:Panel>
            </div>
            <div class="floatleft">
                Please enter your Login ID and captcha to restore your password.
                <br/>
                <asp:TextBox ID="txtLoginId" runat="server" Width="390"></asp:TextBox>
                <asp:Panel Visible="False" ID="pnlStep2" runat="server">
                    <br/>
                    Please answer the questions to restore your password.
                </asp:Panel>
            </div>
            <asp:Panel ID="pnlQuestions" Visible="False" CssClass="clearLeft" runat="server">
                <br/>
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblQuestionOne" runat="server">What was your childhood nickname?</asp:Label>
                <br/>
                <img class="imgRequired" src="../images/required.png" />&nbsp;
                <asp:TextBox ID="txtQuestionOne" question="one" Width="575px" runat="server"></asp:TextBox>

                <br/>
                <br/>
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblQuestionTwo" runat="server">In what city did you meet your spouse/significant other?</asp:Label>
                <br/>
                <img class="imgRequired" src="../images/required.png" />&nbsp;
                <asp:TextBox ID="txtQuestionTwo" Width="575px"  question="two" runat="server"></asp:TextBox>
                
                <br/>
                <br/>
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblQuestionThree" runat="server">What is the name of your favorite childhood pet?</asp:Label>
                <br/>
                <img class="imgRequired" src="../images/required.png" />&nbsp;
                <asp:TextBox ID="txtQuestionThree" Width="575px" question="three"  runat="server"></asp:TextBox>
                <br/>
                <br/>
                <br/>
                <img class="imgRequired" src="../images/required.png" />&nbsp;&nbsp;New Password
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="txtNewPassword" onkeyup="validatePassword()" onblur="validatePassword()" TextMode="Password" Width="433px" runat="server"></asp:TextBox>
                <div id="divPasswordExplination">
                    <div style="color: white;">
                        Password must have: <br />
                        - Between 8 (min) and 25 (max) characters <br />
                        <br />
                        And three of the following: <br />
                        - One (1) upper case character <br />
                        - One (1) lower case character <br />
                        - One (1) special character <br />
                        - One (1) number
                    </div>
                </div>
                <br/>
                <br/>
                <img class="imgRequired" src="../images/required.png" />&nbsp;&nbsp;Confirm Password
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtConfirmPassword" TextMode="Password" onkeyup="validateConfirmPassword(this, '<%# btnSubmit.ClientID %>')" Width="433px" runat="server"></asp:TextBox>
                <div id="divConfrimPasswordError">
                    <div style="color: white;">
                        New and confirmed passwords do not match!
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlCaptcha" runat="server">
                <br/>
                <div id="divCapchaContent">
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Image ID="imgCaptcha" src="../CreateCaptcha.aspx" runat="server"></asp:Image>
                    <br/>
                    &nbsp;Type the code here:
                    <asp:TextBox runat="server" ID="txtCaptcha"></asp:TextBox>
                </div>
            </asp:Panel>
            <div class="buttons clearLeft">
                <asp:Button ID="btnSubmit" OnClick="btnSubmit_OnClick" CssClass="btnstyle" Text="Submit" runat="server"/>
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" CssClass="btnstyle" OnClick="btnCancel_OnClick" Text="Cancel" runat="server"/>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlConfirmationOverlay" style="display: none;" runat="server">
        <div class="overlaymain" style="padding-top: 0">
            <div class="overlayTitle">
                Attention
            </div>
            <div class="overlayContent">
                <br/>
                <asp:Label ID="lblOverlayMessage" runat="server"></asp:Label>
                <br/>
                <br/>
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnOk" Text="OK" OnClick="btnOk_OnClick" CssClass="btnstyle" runat="server" />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeConfirmation" runat="server" TargetControlID="btnOverlayTarget"
        DropShadow="false" PopupControlID="pnlConfirmationOverlay" BackgroundCssClass="modalBackground">
    </ajaxToolkit:ModalPopupExtender>
    <asp:Button ID="btnOverlayTarget" runat="server" CausesValidation="false" Text="Button"
    Style="display: none;" />
    </form>
</body>
</html>
