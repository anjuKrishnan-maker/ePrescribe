<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.InterstitialAd" Codebehind="InterstitialAd.aspx.cs" %>

<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex" />
    <title>Veradigm ePrescribe</title>
    <link id="lnkDefaultStyleSheet" href="Style/Default.css" runat="server" rel="stylesheet" type="text/css" />
    <link id="lnkStyleSheet" runat="server" rel="stylesheet" type="text/css" />
    <link id="PageIcon" rel="SHORTCUT ICON" runat="server" href="images/Allscripts/favicon.ico" />
    <link rel='stylesheet' type='text/css' href='style/pushup.css' />
    <style type="text/css">
        body
        {
            margin-top: 000px;
        }
    </style>
    <script language="javascript" type="text/javascript" src="<%=ResolveUrl("~/js/PageUtil.js?version="+ SessionAppVersion) %>">
    </script>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body onload="backButtonOverride()" class="loginBody">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="adScriptManager" runat="server">
    </asp:ScriptManager>
    <table width="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td class="loginLeftTopBorder">
            </td>
            <td style="width: 80%" class="redborder" colspan="3">
            </td>
        </tr>
        <tr>
            <td class="loginLeftLogoBkgrnd">
                <img id="loginLogo" src="images/shim.gif" alt="" width="155" height="105" />
            </td>
            <td style="background-color: #ffffff;" align="right" colspan="3">
                &nbsp;
                <table width="100%" border="0">
                    <tr>
                        <td align="right">
                        </td>
                        <td style="width: 10">
                            &nbsp;&nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="4" class="loginbanner" align="right">
                &nbsp;
            </td>
        </tr>
    </table>
    <center>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <ePrescribe:AdControl ID="adControl" runat="server" Show="true" SkipTime="36" DisplayMode="FULL_PAGE">
                    </ePrescribe:AdControl>
                </td>
            </tr>
        </table>
    </center>
    </form>
 <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
