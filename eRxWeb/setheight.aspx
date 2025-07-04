<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.setheight" Codebehind="setheight.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Please wait</title>
    <style type="text/css">
        body, html
        {
            margin-top: 000px;
            height: 100%;
        }
    </style>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body onload="detect();">
    <form id="form1" runat="server" action="setheight.aspx" method="post">
    <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0" bgcolor="white">
        <tr>
            <td bgcolor="white">
                <table border="0" width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <font style="font-family: Arial; font-size: 9px;">Loading...</font>
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <input type="hidden" name="ht" id="ht" value="" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hiddenScreenWidth" runat="server" />
    <asp:HiddenField ID="hiddenScreenHeight" runat="server" />
    <asp:HiddenField ID="hiddenBrowserWidth" runat="server" />
    <asp:HiddenField ID="hiddenBrowserHeight" runat="server" />
    </form>
 <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
<script language="javascript" type="text/javascript">
    function detect() {
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

        document.forms[0].submit();
    }
</script>
</html>
