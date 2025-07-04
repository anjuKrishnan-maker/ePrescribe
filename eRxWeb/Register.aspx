<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="eRxWeb.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta http-equiv="x-ua-compatible" content="IE=edge" />
    <title>Veradigm ePrescribe - Registration</title>
    <link type="text/css" href="<%=ResolveUrl("Style/RegistrationCommonStyle.css?version="+Version)%>" rel="stylesheet" />
    <link type="text/css" runat="server" id="partnerStyle" rel="stylesheet" />
    <base href="/">
</head>
<body>
    <script type="text/javascript">
        window["appcontext"] = window["appcontext"] || {};
        window.appcontext = {
            version:"<%=Version%>",
            basicPrice:"<%=registrationPricingStructure.BasicPrice%>",
            deluxePrice:"<%=registrationPricingStructure.DeluxePrice%>",
            deluxeLogRxPrice: "<%=registrationPricingStructure.DeluxeLogRxPrice%>",
            deluxeEpcsPrice: "<%=registrationPricingStructure.DeluxeEpcsPrice%>",
            deluxeEpcsLogRxPrice: "<%=registrationPricingStructure.DeluxeEpcsLogRxPrice%>",
            epcsSetupPrice: "<%=registrationPricingStructure.EPCSSetupPrice%>",
            enterprisePricing: "<%=registrationPricingStructure.EnterprisePricing%>",
            login: "<%=Login%>",
            logout: "<%=Logout%>",
            mediator:"<%=RegistrantMediator%>",
            twoNUserMediator:"<%=TwoNUserMediator%>",
            appName: "<%=AppName%>",
            supportMailAddress: "<%=SupportMailAddress%>"
        };
    </script>

    <app-root>Loading...</app-root>

    <%Response.WriteFile("~/SPARegistration/dist/ChunkBundleReference.html");%>
    
</body>
</html>
