<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.DeluxeTermsConditions" Codebehind="DeluxeTermsConditions.aspx.cs" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Terms And Conditions</title>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<script  language ="javascript" type ="text/javascript">


</script>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="eulaContent" runat="server"></div>      
    </div>
    </form>
      <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
