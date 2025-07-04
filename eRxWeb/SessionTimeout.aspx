<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.SessionTimeOut" Title="Idology - ID Proofing"
    CodeBehind="SessionTimeout.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link id="lnkDefaultStyleSheet" href="Style/Default.css" runat="server" rel="stylesheet" type="text/css" />
    <%if (Session["Theme"] != null)
      { %>
    <link href="<%=Session["Theme"]%>" rel="stylesheet" type="text/css" />
    <%}%>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    <form id="form1" runat="server">
    <table id="Header" width="100%" cellspacing="0" cellpadding="0" style="height: 126px">
        <tr>
            <td class="appStripe" rowspan="6" style="height: 8%; vertical-align: top; width: 100px">
                <img id="mainLogo" src="images/shim.gif" width="120" height="65" />
                <img src="images/powered_by.gif" id="poweredByImage" class="sponsored" />
                <center>
                    <img src="images/spinner.gif" alt="" id="loading" style="display: none" /></center>
            </td>
            <td colspan="7" class="appStripe" style="height: 5px">
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="head valgn nowrap" style="text-align: right; padding-right: 10px">
                <asp:Label ID="lblSiteName" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="head valgn" style="text-align: right; padding-right: 10px">
                <asp:Label ID="lblUser" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="head valgn" style="text-align: right; padding-right: 10px">
                <%  if (base.SessionLicense.EnterpriseClient.ShowLogoutIcon)
                    { %><a href="logout.aspx">Logout</a> &nbsp;&nbsp;<% } %>
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="valgn" style="text-align: right; padding-right: 10px">
                &nbsp;
            </td>
        </tr>
        <tr style="height: 20px">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td class="h1title">
                            <span class="indnt Phead">Manual Registration</span>
                        </td>
                    </tr>
                    <tr class="h2title">
                        <td class="Phead indnt">
                        </td>
                    </tr>
                    <tr class="h3title">
                        <td>
                            <%--                            <asp:Button ID="btnAccept" runat="server" CssClass="btnstyle" Text="I Agree" ToolTip="I agree and accept the IDology Questions"
                                OnClick="btnAccept_Click" />
                            <asp:Button ID="btnDecline" runat="server" CssClass="btnstyle" Text="I Decline" ToolTip="I declined the ID Proofing"
                                OnClick="btnDecline_Click" />--%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <table width="100%" style="height: 400px" border="0" cellspacing="0" cellpadding="5px">
                                    <tr>
                                        <td>
                                            <br />
                                            For security purposes, your session has ended due to inactivity.
                                            <br />
                                            <asp:Label runat="server" ID="lblTryAgain" Visible="false"><br />Please <a href="IDProofingRequiredInfo.aspx">click here</a> to begin again.</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
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
