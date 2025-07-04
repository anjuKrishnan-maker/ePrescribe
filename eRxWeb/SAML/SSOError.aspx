<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.SSOError" Codebehind="SSOError.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ePrescribe SSO Error</title>
    <link id="lnkDefaultStyleSheet" href="../Style/Defaultv12142016.css" runat="server" rel="stylesheet" type="text/css" />
    <link id="lnkStyleSheet" runat="server" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding:10px">
        <img src="../images/Allscripts/Allscripts_Logo.jpg" alt="Allscripts" />
        <br />
        <br />
        <h2>An error occurred on your SSO post.</h2>
        <br /><br />
        <asp:label id="lblSSOErrorMessage" runat="server"></asp:label>
    </div>
    </form>
</body>
</html>
