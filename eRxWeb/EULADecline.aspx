<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.EULADecline"
    Title="EULA Decline" Codebehind="EULADecline.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex" />
    <title>EULA Decline</title>
    <link id="lnkDefaultStyleSheet" href="Style/Default.css" runat="server" rel="stylesheet" type="text/css" />
    <link id="lnkStyleSheet" runat="server" rel="stylesheet" type="text/css" />
    <link id="PageIcon" rel="SHORTCUT ICON" runat="server" href="images/newco/favicon.ico" />
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
<body class="loginBody">
    <form id="form1" runat="server">
    <table border="0" cellpadding="0" cellspacing="0" height="100%" width="100%">
        <tr>
            <td width="100%">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr height="6">
                        <td class="loginLeftTopBorder">
                            <input type="hidden" name="ht" id="ht" value="" />
                        </td>
                        <td width="80%" class="redborder">
                        </td>
                    </tr>
                    <tr height="105" valign="middle">
                        <td class="loginLeftLogoBkgrnd">
                            <img id="loginLogo" src="images/shim.gif" alt="" width="155" height="105" />
                        </td>
                        <td bgcolor="white" align="right">
                            &nbsp;
                            <table width="100%" border="0">
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="loginstats" runat="server" Text="" Visible="false"></asp:Label>
                                    </td>
                                    <td width="10">
                                        &nbsp;&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr height="36">
                        <td colspan="2" class="loginbanner">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Label ID="LoginError" runat="server" CssClass="LoginFailed"></asp:Label><br />
                            <br />
                            <font class="subheadred">You cannot access this part of the system without accepting
                                the End User License Aggreement.<br />
                                <br />
                            </font>
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <br />
                                        <br />
                                        <asp:Button ID="btnClose" runat="server" CssClass="btnstyle" Text="Close" OnClientClick="javascript:window.close()"
                                            Width="150px" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr align="center">
            <td>
            </td>
        </tr>
    </table>
    </form>
   <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
