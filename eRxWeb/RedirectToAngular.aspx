<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RedirectToAngular.aspx.cs" Inherits="eRxWeb.RedirectToAngular" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="x-ua-compatible" content="IE=edge"/>
    <title>Veradigm ePrescribe</title>
    <script src="jquery/js/jquery.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <!--this js gets all function to comunicate between content and angular-->
    <script src="SPA/Script/spa.bridge.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
    <script type="text/javascript">
        function NavigateToAngularComponent(componentName, componentParameters) {
            window.parent.NavigateToAngularComponent(componentName, componentParameters);
        }
    </script>
</body>
</html>
