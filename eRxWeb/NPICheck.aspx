<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.NPICheck" Title="NPI Check"
    CodeBehind="NPICheck.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <title></title>
    <script type="text/javascript" language="javascript" src="js/formUtil.js"></script>
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
                            <span class="indnt Phead" style="padding-left: 15px;">NPI Verification</span>
                        </td>
                    </tr>
                    <tr class="h2title">
                        <td class="Phead indnt">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <table width="100%" style="border: solid 1px #666666" cellpadding="5" cellspacing="0">
                                    <tr style="background-color: #E0E0E0;">
                                        <td colspan="2">
                                            <b>Please provide your details for NPI verification.</b>
                                        </td>
                                        <td align="right">
                                            <span style="color: Red">*</span> = Required
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> First Name
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtfirstname" MaxLength="35" runat="server"
                                                Width="300px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvFName" runat="server" ControlToValidate="txtfirstname"
                                                CssClass="requiredfiled" Display="Dynamic" Text="* First Name is a required field."></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> Last Name
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtlastname" MaxLength="35" runat="server"
                                                Width="300px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvLName" CssClass="requiredfiled" runat="server"
                                                Text="* Last Name is a required field." ControlToValidate="txtlastname" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td runat="server" align="right" id="Td1" style="width: 170px">
                                            <span style="color: Red">*</span> NPI
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtNPI" runat="server" Width="300px"
                                                MaxLength="10"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="revNPI" runat="server" ControlToValidate="txtNPI"
                                                CssClass="requiredfiled" Display="Dynamic" ValidationExpression="\d{10}">* NPI must be ten (10) digits.</asp:RegularExpressionValidator>
                                            <asp:CustomValidator ID="cvNPI" runat="server" ValidateEmptyText="true" Font-Bold="False" ErrorMessage="* Please enter a valid NPI."
                                                OnServerValidate="cvNPI_ServerValidate"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnSubmit" runat="server" CssClass="btnstyle" Text="SUBMIT" ToolTip="Click to verify NPI"
                                                OnClick="btnSubmit_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" CausesValidation="false"
                                                Text="CANCEL" ToolTip="Cancel NPI verification" OnClick="btnCancel_Click" />
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
