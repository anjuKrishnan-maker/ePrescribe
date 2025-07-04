<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_GoogleReCAPTCHA" Codebehind="GoogleReCAPTCHA.ascx.cs" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<script type="text/javascript">
    var RecaptchaOptions = {
        theme: 'custom',
        custom_theme_widget: 'recaptcha_widget'
    };
</script>
<recaptcha:RecaptchaControl
    ID="recaptcha"
    runat="server"
    Theme="white"
    PublicKey="x"
    PrivateKey="x"
    OverrideSecureMode="true" />
