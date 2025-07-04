<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProtectedStoreEpcsReports.aspx.cs" Inherits="eRxWeb.ProtectedStoreEpcsReports" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>        
    </script>
    <script language="javascript" type="text/javascript">
        if (window.parent.ContentFrameLoaded != undefined
            && typeof (window.parent.ContentFrameLoaded) == 'function' && window.frameElement) {
            window.parent.ContentFrameLoaded({
                PageName: "ProtectedStoreEpcsReports.aspx", Source: window.frameElement.id
            });
        }
        
    </script>
</head>
  <body onload="postToProtectedStore()">   

    <form id="formToBePosted" runat="Server" method="post" style="display:none">
        <input type="hidden" runat="server" id="SSOTokenInBase64String" name="SSOTokenInBase64String" />
        <input type="hidden" runat="server" id="ReportType" name="ReportType" />
        <input type="hidden" runat="server" id="Month" name="Month" />
        <input type="hidden" runat="server" id="Year" name="Year" />
        <input type="hidden" runat="server" id="TimeZone" name="TimeZone" />
    </form>
    <center><img src="images/loading.gif" style="position: absolute;top: 50%;left: 50%;margin-left: -50px;margin-top: -50px;" alt="" id="loading" /></center>
  
    <script type="text/javascript">
        function postToProtectedStore() {
            document.getElementById('formToBePosted').submit();
        }
    </script>
</body>
</html>
