<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb._Default" CodeBehind="Login.aspx.cs" %>

<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title>Welcome to </title>

    <script type="text/javascript" src="jquery/js/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/pageUtil.js"></script>
    <link href="Style/AdvertisementTag.css" rel="stylesheet" />
    <link href="Style/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link id="lnkDefaultStyleSheet" href='Style/Default.css' rel="stylesheet" type="text/css" />
    <link href="<%=ResolveUrl("Style/LoginStyle.css?version="+ SessionAppVersion) %>" rel="stylesheet" type="text/css" />
    <link id="lnkStyleSheet" runat="server" rel="stylesheet" type="text/css" />

    <link id="PageIcon" rel="SHORTCUT ICON" runat="server" href="images/Allscripts/favicon.ico" />

    <style type="text/css">
        .modalBackground1 {
            FILTER: alpha(opacity=80);
            BACKGROUND-COLOR: gray;
            opacity: 0.8;
        }
    </style>
    <script type="text/javascript" src="js/jquery-1.4.2.js"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/js/PageUtil.js?version="+ SessionAppVersion) %>"></script>
    <script type="text/javascript">
        function setBrowserDimensions() {


            document.getElementById("LoginError").innerText = '';

            if (typeof (self.innerHeight) == 'undefined') //IE
            {
                document.getElementById("ht").value = document.forms[0].offsetHeight;
                document.getElementById("hiddenBrowserWidth").value = document.forms[0].offsetWidth;
            }
            else //Firefox
            {
                document.getElementById("ht").value = self.innerHeight;
                document.getElementById("hiddenBrowserWidth").value = self.innerWidth;
            }

            document.getElementById("hiddenScreenWidth").value = screen.width;
            document.getElementById("hiddenScreenHeight").value = screen.height;
            document.getElementById("hiddenBrowserHeight").value = document.getElementById("ht").value;
        }
        function disableContinueLoginRegistrationClick(){
            document.getElementById("btnContinueLogin").disabled = true;
            document.getElementById("btnContinueLogin").value = "Processing...";
        }

        var doc = document.documentElement;
        doc.setAttribute('data-useragent', navigator.userAgent);
    </script>
</head>

<%if (PlacementResponse != null)
    { %>
<%=PlacementResponse.Header%>
<%} %>
<body onload="backButtonOverride()">
    <form id="form2" runat="server" action="Login.aspx" autocomplete="off" style="height: 100%">
        <asp:ScriptManager ID="loginScriptManager" runat="server" EnablePartialRendering="true"
            ScriptMode="Release" OnAsyncPostBackError="loginScriptManager_AsyncPostBackError">
        </asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ValidateRequestMode="Enabled" UpdateMode="Conditional" RenderMode="inline">
            <ContentTemplate>

                <div class="container-fluid fill">
                    <div class="row fill">
                        <div class="col-lg-5 col-sm-12 col-xs-12 map LoginLeftPanel">
                            <input type="hidden" name="ht" id="ht" value="" />
                            <div class="col-lg-8 col-sm-6 col-xs-6 col-lg-push-2 col-sm-push-3 col-xs-push-3 LoginDiv">
                                <div class="loginform">

                                    <div>
                                        <div style="background-image: url(images/logo.png); background-repeat: no-repeat; background-position: left top; padding-top: 10em; margin-bottom: 2em;">
                                        </div>
                                    </div>

                                    <div class="form-group" style="margin-top: 10px">
                                        <asp:TextBox autocomplete="off" runat="server" ForeColor="#000" ID="txtUserName" class="form-control loginPageTextBox" placeholder="Username" ValidationGroup="Login"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtUSN" runat="server" ControlToValidate="txtUserName"
                                            Display="None" ErrorMessage="User ID cannot be blank." ValidationGroup="Login">*</asp:RequiredFieldValidator>
                                    </div>

                                    <div class="form-group">
                                        <asp:TextBox ID="txtPassword" ForeColor="#000" class="form-control loginPageTextBox" placeholder="Password" ValidateRequestMode="Disabled" runat="server" TextMode="Password" ValidationGroup="Login"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatortxtPWD" runat="server" ControlToValidate="txtPassword"
                                            Display="None" ErrorMessage="Password cannot be blank." ValidationGroup="Login">*</asp:RequiredFieldValidator>
                                    </div>

                                    <div class="checkbox">
                                        <label>
                                            <asp:CheckBox ID="cbRemeberLogin" runat="server" />
                                            <label style="color: white; padding: 0.2em 0em;">Remember me</label>
                                        </label>
                                    </div>

                                    <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-default col-lg-12" Text="Log In" OnClick="btnLogin_Click"
                                        OnClientClick="setBrowserDimensions()" ValidationGroup="Login" Style="margin-bottom: 0.5em; width: 100%" />

                                    <asp:Label ID="LoginError" runat="server" CssClass="LoginFailed"></asp:Label>
                                    <asp:UpdateProgress runat="server" ID="updProgress">
                                        <ProgressTemplate>
                                            <div style="width: 15px; height: 15px">
                                                <img src="images/Loading3.gif" alt="Loading..." />
                                            </div>
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>

                                </div>

                                <div class="col-lg-12" style="margin-top: 1.5em">
                                    <div class="row">
                                        <div class="col-lg-6 col-sm-6 col-xs-6">
                                            <asp:LinkButton CssClass="loginlinks" Style="float: right" ID="lnkForgotPassword" runat="server" OnClick="btnForgotShieldPassword_Click">Forgot Password? </asp:LinkButton>
                                        </div>
                                        <div class="col-lg-6 col-sm-6 col-xs-6" id="divShowRegisterLinkOnLogin" runat="server">
                                            <asp:HyperLink ID="lnkRegister" CssClass="loginlinks" runat="server">Register for New Account.</asp:HyperLink>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-12 col-sm-12 col-xs-12" runat="server" style="display: table">
                                            <asp:Label ID="lblVersion" runat="server" Style="display: table-cell; color: #808080; vertical-align: middle; text-align: center; padding-top: 20px"></asp:Label>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                        <!--End of Left Panel -->
                        <div class="col-lg-7 col-sm-12 col-xs-12 map1 LoginRightPanel">
                            <div class="row VTop">
                                <div class="col-lg-12 col-sm-12 col-xs-12">
                                    <asp:Panel ID="pnlReleaseNote" runat="server" Visible="true">
                                        <div style="width: 100%">
                                            <table id="tblMessages" runat="server" cellspacing="0" cellpadding="5" style="width: 100%">
                                                <tr>
                                                    <td class="loginMessageHeader">
                                                        <span class="subheadred" style="color: #FFF; font-size: 14px; font-weight: 100;">Veradigm News</span>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div style="padding: 0.5em;">
                                                            <asp:Label ID="lblMessages" Style="color: #FFF" runat="server"></asp:Label>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="row VTop" style="padding-left: 8%;">
                                <div class="col-lg-6 col-sm-6 col-xs-6 col-lg-push-3 col-sm-push-3 col-xs-push-3">
                                    <div>
                                        <%if (PlacementResponse != null)
                                            { %>
                                        <% if (PlacementResponse.Content["right_hand"].ToString() != string.Empty)
                                            {%>
                                        <div id="divAdvertisementHeader" class="advertisement-tag_block white-font">Advertisement</div>
                                        <%  }%>
                                        <%= PlacementResponse.Content["right_hand"] %>

                                        <%} %>
                                    </div>
                                </div>
                            </div>
                            <div class="row VTop PLeft">
                                <div class="col-lg-12 col-sm-12 col-xs-12">
                                    <div runat="server">
                                        <%if (PlacementResponse != null)
                                            { %>

                                        <% if (PlacementResponse.Content["bottom"].ToString() != string.Empty)
                                            {%>
                                        <div id="divAdvertisementFooter" class="advertisement-tag_block white-font">Advertisement</div>
                                        <%  }%>
                                        <%= PlacementResponse.Content["bottom"] %>
                                        <% if (PlacementResponse.Content["bottom"].ToString() != string.Empty)
                                            {%>
                                        <div id="divADPrivacyPolicy1" runat="server" visible="true">
                                            <span class="smalltext">
                                                <asp:HyperLink ID="lnkADPrivacyPolicy1" runat="server" ForeColor="#FFFFFF" NavigateUrl="~/AdPrivacyPolicy.aspx"
                                                    Target="_blank">Ad Privacy Policy
                                                </asp:HyperLink>
                                            </span>
                                        </div>
                                        <%  }%>
                                        <%  }%>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!--End of Right Panel -->
                    </div>
                    <!--End of Row1-->
                    <div class="row" style="padding-left: 0px; padding-right: 0px">
                        <div class="navbar-fixed-bottom">

                            <div class="col-lg-5 col-sm-12 col-xs-12" style="height: 4.5em; background-color: #FFF; padding-left: 0px; padding-right: 0px">
                                <p class="text-justify" style="color: #808080; text-align: center; font-size: 11px; padding: 0.5em">
                                    <%=Copyright.LongText%>
                                </p>

                            </div>

                            <div class="col-lg-7 col-sm-12 col-xs-12" style="height: 4.5em; background-color: #000; display: table">

                                <asp:HyperLink ID="lnkePrescribeSupport" Style="color: #FFFFFF; font-size: small; display: table-cell; vertical-align: middle; text-align: center" runat="server" NavigateUrl="help/ContactUs.aspx"
                                    Target="regtar">Contact Veradigm Support.</asp:HyperLink>

                            </div>

                        </div>
                    </div>
                    <!--End of Row2-->
                </div>
                <!--End of Container-->

                <asp:Button ID="hiddenPasswordExpiredTrigger" runat="server" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="mpePasswordExpired" runat="server" TargetControlID="hiddenPasswordExpiredTrigger"
                    PopupControlID="pnlPasswordExpired" BackgroundCssClass="modalBackground" DropShadow="false"
                    OkControlID="btnCancelPasswordExpiredOverlay" />
                <asp:Panel Style="display: none" ID="pnlPasswordExpired" runat="server">
                    <div class="overlaymainwide">
                        <div class="overlayTitle" style="height: 2em">
                            Password Expired
                        </div>
                        <table class="overlayContent">
                            <tr>
                                <td style="width: 60%; padding: 2em">
                                    <table style="padding: 5px">
                                        <tr>
                                            <td>
                                                <p style="padding-bottom: 10px">
                                                    Your password is expired and needs to be changed.
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPasswordExpiredStatus" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:ValidationSummary ID="vsChangePassword" runat="server" ValidationGroup="ChangePassword" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Current Password: <span style="color: Red">*</span>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtOldPassword" runat="server" MaxLength="25" ValidateRequestMode="Disabled" TextMode="Password"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvOldPassword" runat="server" ControlToValidate="txtOldPassword"
                                                                ErrorMessage="Current password is required." ValidationGroup="ChangePassword">*</asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>New Password: <span style="color: Red">*</span>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtNewPassword" runat="server" MaxLength="25" ValidateRequestMode="Disabled" TextMode="Password" Style="margin-top: 5px"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvNewPwd" runat="server" ControlToValidate="txtNewPassword"
                                                                ErrorMessage="New password is required." Display="dynamic" ValidationGroup="ChangePassword">*</asp:RequiredFieldValidator>
                                                            <asp:CompareValidator ID="cvOldPassword" runat="server" ControlToCompare="txtOldPassword"
                                                                ControlToValidate="txtNewPassword" ErrorMessage="New password should be different from current password."
                                                                Operator="NotEqual" Display="dynamic" ValidationGroup="ChangePassword">*</asp:CompareValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Confirm Password: <span style="color: Red">*</span>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtConfirmNewPwd" runat="server" MaxLength="25" ValidateRequestMode="Disabled" TextMode="Password" Style="margin-top: 5px"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvConfirmPwd" runat="server" ControlToValidate="txtConfirmNewPwd"
                                                                ErrorMessage="Confirm password is required." ValidationGroup="ChangePassword">*</asp:RequiredFieldValidator>
                                                            <asp:CompareValidator ID="cvConfirmNewPwd" runat="server" ControlToCompare="txtNewPassword"
                                                                ControlToValidate="txtConfirmNewPwd" ErrorMessage="Passwords do not match. Verify passwords and try again."
                                                                ValidationGroup="ChangePassword">*</asp:CompareValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 40%">
                                    <table style="padding: 5px">
                                        <tr>
                                            <td style="background-color: #fffacd; border: solid 1px DarkGray">
                                                <asp:Label ID="lblPasswordHelpText" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <div class="overlayFooter" style="height: 3.5em">
                            <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" OnClick="btnChangePassword_Click"
                                OnClientClick="setBrowserDimensions()" CssClass="btnstyle btnStyleAction" ValidationGroup="ChangePassword" />
                            <asp:Button ID="btnCancelPasswordExpiredOverlay" runat="server" Text="Cancel" CssClass="btnstyle btnStyleAction"
                                CausesValidation="false" />
                        </div>
                    </div>
                </asp:Panel>


                <asp:Button ID="btnHiddenForgotPasswordTrigger" runat="server" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="mpeForgotPasswordSelector" runat="server" TargetControlID="btnHiddenForgotPasswordTrigger"
                    PopupControlID="pnlForgotPasswordSelector" BackgroundCssClass="modalBackground"
                    DropShadow="false" OkControlID="btnForgotPasswordCancel" />
                <asp:Panel Style="display: none" ID="pnlForgotPasswordSelector" runat="server">
                    <table class="overlaymainwide">
                        <tr>
                            <td>
                                <h3>Forgot Password</h3>
                            </td>
                        </tr>
                        <tr>
                            <td>Has your ePrescribe User Account been migrated to <span style="font-weight: bold; color: #5B8F22">Veradigm Shield</span>? If you are not sure or have not yet
                            heard of <span style="font-weight: bold; color: #5B8F22">Veradigm Shield</span>
                                most likely you have NOT been migrated.
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table width="100%" cellpadding="10px" cellspacing="10px">
                                    <tr>
                                        <td style="text-align: center; width: 50%; border: 1px solid #cccccc">
                                            <div style="height: 50px">
                                                <b>If you have been migrated to Veradigm Shield click below.</b>
                                            </div>
                                            <asp:Button ID="btnForgotShieldPassword" runat="server" Text="YES, I have a Shield Account."
                                                OnClick="btnForgotShieldPassword_Click" CssClass="btnstyle" />
                                        </td>
                                        <td style="text-align: center; width: 50%; border: 1px solid #cccccc">
                                            <div style="height: 50px">
                                                <b>If you are not sure or you have not been migrated to Veradigm Shield click below.</b>
                                            </div>
                                            <asp:Button ID="btnForgotEprescribePassword" runat="server" Text="NO, I do NOT have a Shield Account."
                                                PostBackUrl="ForgotPassword.aspx" CssClass="btnstyle" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" style="padding-top: 20px">
                                <asp:Button ID="btnForgotPasswordCancel" runat="server" Text="Cancel" CssClass="btnstyle" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>


                <asp:CheckBox ID="chkRememberPassword" runat="server" Visible="false" />
                <asp:HiddenField ID="hiddenScreenWidth" runat="server" />
                <asp:HiddenField ID="hiddenScreenHeight" runat="server" />
                <asp:HiddenField ID="hiddenBrowserWidth" runat="server" />
                <asp:HiddenField ID="hiddenBrowserHeight" runat="server" />


                <asp:Button ID="hiddenServiceAlert" runat="server" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="mpeServiceAlert" runat="server" BehaviorID="mpeServiceAlert"
                    DropShadow="false" BackgroundCssClass="modalBackground1" TargetControlID="hiddenServiceAlert"
                    PopupControlID="panelServiceAlert">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelServiceAlert" Style="display: none" runat="server">
                    <div id="div1" class="overlaymain" style="min-width: 300px; max-width: 400px;">
                        <div class="overlayTitle" style="min-width: 300px; max-width: 400px; height: auto">
                            <asp:Label ID="lblServiceAlertTitle" Style="font-size: 1.5em !important; color: #e06020 !important" CssClass="serviceAlertTitleText"
                                runat="server">Service Alert- Update</asp:Label>
                        </div>
                        <div class="overlayContent" style="min-height: 150px; max-height: 400px; min-width: 300px; max-width: 400px; overflow-y: auto;">

                            <asp:Label ID="lblServiceAlertBody" runat="server">•	One announcement per enterprise can be shown in the given date range.
                                    •	This announcement should show right after login and before site selection screen or EULA.
                   
                            </asp:Label>

                        </div>
                        <div class="overlayFooter" style="padding-bottom: 30px">
                            <asp:Button ID="btnContinue" runat="server" Text="Continue" CssClass="btnstyle btnStyleAction" OnClick="btnContinue_Click" />
                        </div>

                    </div>
                </asp:Panel>

                <!--- For Existing Shield user having Registration in progress/-->
                <asp:Button ID="hiddenRegistrationWorkFlow" runat="server" Style="display: none;" />
                <ajaxToolkit:ModalPopupExtender ID="mpeRegistrationWorkFlow" runat="server" BehaviorID="mpRegistrationWorkFlow"
                    DropShadow="false" BackgroundCssClass="modalBackground1" TargetControlID="hiddenRegistrationWorkFlow"
                    PopupControlID="panelChooseRegistrationWorkFlow">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel ID="panelChooseRegistrationWorkFlow" Style="display: none" runat="server">
                    <div id="divRegistrationWorkFlow" class="overlaymain" style="min-width: 300px; max-width: 400px;">
                        <div class="overlayContent" style="min-height: 150px; max-height: 400px; min-width: 300px; max-width: 400px; overflow-y: auto;">
                            <label>It looks like you started an additional registration for this User Account. Would you like to complete this registration now?</label>
                            <asp:RadioButtonList ID="rdoRegistration" runat="server" RepeatDirection="Vertical">
                                <asp:ListItem Text="Yes - Complete this pending registration" Value="True" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="No - Login into ePrescribe" Value="False"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div class="overlayFooter" style="padding-bottom: 30px">
                            <asp:Button ID="btnContinueLogin" runat="server" Text="Continue" ClientIDMode="Static" OnClientClick="disableContinueLoginRegistrationClick();" UseSubmitBehavior="false" CssClass="btnstyle btnStyleAction" OnClick="btnContinueLoginOrRegistrationClick" />
                        </div>

                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>

    <%if (new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
        { %>
    <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
    <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
    <script type="text/javascript"> ga('send', 'pageview');</script>
    <%} %>
</body>
</html>
