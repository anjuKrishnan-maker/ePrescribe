<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PdmpDetails.aspx.cs" Inherits="eRxWeb.PdmpDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body onload="postToCommonUi()">   
    <form id="dtlFrmToBePosted" runat="Server" method="post" style="display: block">
        <input type="hidden" runat="server" id="CompressedSamlPayload"/>
    </form>
    <center><img src="images/loading.gif" style="position: absolute;top: 50%;left: 50%;margin-left: -50px;margin-top: -50px;" alt="" id="loading" /></center>

    <script type="text/javascript">
        function postToCommonUi() {
            document.getElementById('dtlFrmToBePosted').submit();
        }
    </script>
</body>
</html>
