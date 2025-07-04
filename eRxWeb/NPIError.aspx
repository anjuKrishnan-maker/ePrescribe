<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.NPIError" CodeBehind="NPIError.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex" />
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
    <form id="form2" runat="server">
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
                            <asp:Label ID="lblErrorTitle" runat="server" CssClass="indnt Phead" Text="Error-01"
                                Style="padding-left: 15px;"></asp:Label>
                        </td>
                    </tr>
                    <tr class="h2title">
                        <td class="Phead indnt">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <asp:Panel ID="pnlRegisterManually" runat="server">
                                    <p style="padding-top: 10px; padding-left: 15px;">
                                        <div id="divNPIRetry" runat="server">
                                            Your NPI verification failed. You can either attempt to retry or you can complete
                                            the manual NPI verification form and fax it to us.
                                        </div>
                                        <div id="divNPIFailed" visible="false" runat="server">
                                            Your NPI verification failed. You can complete the manual NPI verification form
                                            and fax it to us.
                                        </div>
                                        <br />
                                        <br />
                                        <div style="padding-left: 300px">
                                            <asp:Button ID="btnRetry" runat="server" OnClick="btnRetry_Click" Text="Retry" />
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnRegisterManually" runat="server" OnClick="btnRegisterManually_Click"
                                                Text="View Manual NPI Check Form" />
                                        </div>
                                        <br />
                                        <br />
                                    </p>
                                </asp:Panel>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height: 19px">
                            <br />
                            <br />
                            <br />
                            <br />
                            <hr class="hrstyle" style="width: 100%" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left" style="padding-bottom: 10px; padding-left: 10px; padding-right: 10px">
                            Contact us:&nbsp;
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://eprescribe.allscripts.com/help/default.aspx"
                                Target="_blank">eprescribe.allscripts.com/help</asp:HyperLink><asp:Label ID="Label1"
                                    runat="server" Text=" | "></asp:Label>
                            <img src="images/email_icon.gif" alt="" />
                            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="mailto:eprescribesupport@allscripts.com">eprescribesupport@allscripts.com</asp:HyperLink>
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
