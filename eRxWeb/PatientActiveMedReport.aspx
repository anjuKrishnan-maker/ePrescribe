<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.PatientActiveMedReport" Codebehind="PatientActiveMedReport.aspx.cs" %>

<html>
<head>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body onload="callFunction();">
    <form action="patientActiveMedReport.aspx" method="post">
    <script language="javascript" type="text/javascript">
        function callFunction() {
            window.frames.iframe1.fnCheck();
            return false;
        }


    </script>
    <iframe name='iframe1' id='iframe1' src='PrintReport.aspx?ReportID=PatientCurrentMeds'
        align='top' width='837' height='507' frameborder='0' title='Print Report' scrolling='no'>
        If you can see this, your browser does not support iframes! </iframe>
    </form>
 <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
