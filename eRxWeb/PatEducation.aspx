<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.PatEducation"
    Title="Patient Education" Codebehind="PatEducation.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Patient Education</title>
    <link href="Style/Style.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        // <!CDATA[

        function IMG1_onclick() {
            if (window.print) window.print(); return false;
        }

        // ]]>
    </script>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellspacing="0" cellpadding="0" style="text-align: center" width="100%" border="0">
            <tbody>
                <tr>
                    <td align="left">
                        <img src="images/Allscripts/Allscripts_Logo_sm.jpg" alt="Allscripts" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <!--<TABLE bordercolor="#000000" cellspacing="0" cellpadding="0" width="775" align="right" border="1">-->
                        <!--old width="775" align="right" new width="650" style="text-align:left"-->
                        <table bordercolor="#b5c4c4" cellspacing="0" cellpadding="0" width="700px" style="text-align: left"
                            border="1">
                            <tbody>
                                <tr>
                                    <td class="h1title indnt">
                                        <span class="Phead">Patient Education </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="h2title">
                                    </td>
                                </tr>
                                <tr class="h4title">
                                    <td>
                                        <input id="IMG1" class="btnstyle" type="button" value="Print" onclick="return IMG1_onclick()" />&nbsp;&nbsp;
                                        <input id="IMG2" class="btnstyle" type="button" value="Close" onclick='window.close()' />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPatEdPrint" runat="server" Text="*Patient education will be printed automatically when prescription is processed."
                                            ForeColor="Red" Visible="false"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="font-family: Arial;">
                                        <!--new  style="text-align:justify" has been added in div tags -->
                                        <div id="divGenericName" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <br />
                                        <div id="divCommon" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <br />
                                        <div id="divHow" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <div id="divCautions" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <div id="divSideEffect" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <div id="divBefore" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <div id="divOverDose" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <br />
                                        <div id="divAddition" style="text-align: justify; margin-left: 10px; margin-right: 10px;"
                                            runat="server">
                                        </div>
                                        <br />
                                        <br />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
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
