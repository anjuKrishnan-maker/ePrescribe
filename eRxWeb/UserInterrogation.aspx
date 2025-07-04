<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.UserInterrogation" Title="Idology - ID Proofing"
    CodeBehind="UserInterrogation.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex" />
    <title></title>
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
            <td class="appStripe" rowspan="6" style="height: 8%; vertical-align: top; width: 10px">
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
                            <span class="indnt Phead">IDology Questions</span>
                        </td>
                    </tr>
                    <tr class="h2title">
                        <td class="Phead indnt">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <asp:ScriptManager ID="IDProofingScriptManager" runat="server" EnablePartialRendering="true">
                                </asp:ScriptManager>
                                <asp:Timer ID="timer" runat="server" OnTick="timer_Tick">
                                </asp:Timer>
                                <table width="100%" cellpadding="10px">
                                    <tr>
                                        <td>
                                            <span class="Phead">Register for
                                                <asp:Label ID="lblProductName" runat="server" CssClass="Phead"></asp:Label>
                                                <asp:Label ID="lblStepNo" runat="server" CssClass="Phead"> (Step 3 of 3)</asp:Label></span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <p>
                                                These questions are related to your public information our search returned.</p>
                                            <p style="padding-top: 10px; font-weight: bold">
                                                PLEASE READ: You only have a few minutes to answer these questions.</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="questionTable">
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="panQ1" runat="server">
                                                            <asp:Label ID="IDologyNumber" runat="server" Text="" Visible="false"></asp:Label>
                                                            <asp:Label ID="lblQuestion1" runat="server" Text="">
                                                            </asp:Label>
                                                            <asp:Label ID="lblQ1Type" runat="server" Text="" Visible="false"></asp:Label><br />
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvQ1" ControlToValidate="rblQuestion1"
                                                                runat="server" ErrorMessage="Question #1 was not answered." Font-Bold="true">* * * Answer Required * * *</asp:RequiredFieldValidator><br />
                                                            <asp:RadioButtonList ID="rblQuestion1" runat="server">
                                                            </asp:RadioButtonList>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <table class="questionTable">
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="panQ2" runat="server">
                                                            <asp:Label ID="lblQuestion2" runat="server" Text=""></asp:Label><asp:Label ID="lblQ2Type"
                                                                runat="server" Text="" Visible="false"></asp:Label>
                                                            <br />
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvQ2" ControlToValidate="rblQuestion2"
                                                                runat="server" ErrorMessage="Question #2 was not answered." Font-Bold="true">* * * Answer Required * * *</asp:RequiredFieldValidator><br />
                                                            <asp:RadioButtonList ID="rblQuestion2" runat="server">
                                                            </asp:RadioButtonList>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <table class="questionTable">
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="panQ3" runat="server">
                                                            <asp:Label ID="lblQuestion3" runat="server" Text=""></asp:Label><asp:Label ID="lblQ3Type"
                                                                runat="server" Text="" Visible="false"></asp:Label>
                                                            <br />
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvQ3" ControlToValidate="rblQuestion3"
                                                                runat="server" ErrorMessage="Question #3 was not answered." Font-Bold="true">* * * Answer Required * * *</asp:RequiredFieldValidator><br />
                                                            <asp:RadioButtonList ID="rblQuestion3" runat="server">
                                                            </asp:RadioButtonList>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <table class="questionTable">
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="panQ4" runat="server">
                                                            <asp:Label ID="lblQuestion4" runat="server" Text=""></asp:Label><asp:Label ID="lblQ4Type"
                                                                runat="server" Text="" Visible="false"></asp:Label>
                                                            <br />
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvQ4" ControlToValidate="rblQuestion4"
                                                                runat="server" ErrorMessage="Question #4 was not answered." Font-Bold="true">* * * Answer Required * * *</asp:RequiredFieldValidator><br />
                                                            <asp:RadioButtonList ID="rblQuestion4" runat="server">
                                                            </asp:RadioButtonList>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <table class="questionTable">
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="panQ5" runat="server">
                                                            <asp:Label ID="lblQuestion5" runat="server" Text=""></asp:Label><asp:Label ID="lblQ5Type"
                                                                runat="server" Text="" Visible="false"></asp:Label>
                                                            <br />
                                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RequiredFieldValidator ID="rfvQ5" ControlToValidate="rblQuestion5"
                                                                runat="server" ErrorMessage="Question #5 was not answered." Font-Bold="true">* * * Answer Required * * *</asp:RequiredFieldValidator><br />
                                                            <asp:RadioButtonList ID="rblQuestion5" runat="server">
                                                            </asp:RadioButtonList>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <table border="0" width="100%" cellspacing="0" cellpadding="5px">
                                                <tr align="center">
                                                    <td>
                                                        <asp:LinkButton ID="btnNext" runat="server" CssClass="btnstyle" OnClick="btnNext_Click"><span>Next ></span></asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
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
