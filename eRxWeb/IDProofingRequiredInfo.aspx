<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.IDProofingRequiredInfo" Title="ID Proofing"
    CodeBehind="IDProofingRequiredInfo.aspx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
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
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
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
                            <span class="indnt Phead" style="padding-left: 15px;">ID Proofing Verification</span>
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
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblErrorMessage" Text="* Please enter your valid physical home address." ForeColor="Red" runat="server"/>
                                        </td>
                                    </tr>
                                    <tr style="background-color: #E0E0E0;">
                                        <td>
                                            <b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Your name and address</b>
                                        </td>
                                        <td align="right">
                                            <span style="color: Red">*</span> = Required
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                             First Name
                                        </td>
                                        <td>
                                            <asp:TextBox Enabled="false" ID="txtfirstname" MaxLength="35" runat="server"
                                                Width="300px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                             Last Name
                                        </td>
                                        <td>
                                            <asp:TextBox Enabled="false" ID="txtlastname" MaxLength="35" runat="server"
                                                Width="300px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> Home Address
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtaddress" MaxLength="35" runat="server"
                                                Width="300px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvAddress" CssClass="requiredfiled" runat="server"
                                                Text="* Address is a required field." ControlToValidate="txtaddress" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> City
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtcity" MaxLength="35" runat="server"
                                                Width="300px" onblur="capitilizeInitial(this)"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvCity" CssClass="requiredfiled" runat="server"
                                                Text="* City is a required field." ControlToValidate="txtcity" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> State
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlstate" runat="server">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvState" CssClass="requiredfiled" runat="server"
                                                Text="* State is a required field." ControlToValidate="ddlstate" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> ZIP Code
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtzip" MaxLength="5" runat="server"
                                                onkeypress="return numericKeyPressOnly(this, event);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvZip" CssClass="requiredfiled" runat="server" Text="* ZIP Code is a required field."
                                                ControlToValidate="txtzip" Display="Dynamic"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revZipCode" runat="server" ControlToValidate="txtzip"
                                                CssClass="requiredfiled" Display="Dynamic" ValidationExpression="\d{5}">* ZIP Code must be five(5) digits.</asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> Year of Birth
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtdobYear" runat="server" MaxLength="4"
                                                onkeypress="return numericKeyPressOnly(this, event);"></asp:TextBox><span class="smallgraytext">
                                                    (YYYY)</span>
                                            <asp:RequiredFieldValidator ID="rfvYOB" CssClass="requiredfiled" runat="server" Text="* Year of Birth is a required field."
                                                ControlToValidate="txtdobYear" Display="Dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                                    ID="revYOB" runat="server" ControlToValidate="txtdobYear" CssClass="requiredfiled"
                                                    Display="Dynamic" ErrorMessage="Year of Birth must be four (4) digits." ValidationExpression="\d{4}">*</asp:RegularExpressionValidator>
                                            <asp:RangeValidator ID="rvYOB" runat="server" CssClass="requiredfiled" Text="* Year of Birth is incorrect."
                                                ControlToValidate="txtdobYear" Display="dynamic" MinimumValue="1900" MaximumValue="2000"></asp:RangeValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                             Email
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEmail" Enabled="false" MaxLength="100" Width="300px"
                                                runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            <span style="color: Red">*</span> Last 4 of SSN
                                        </td>
                                        <td>
                                            <asp:TextBox AutoCompleteType="Disabled" ID="txtssnLast4" runat="server" MaxLength="4"
                                                onkeypress="return numericKeyPressOnly(this, event);"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="revSSN" runat="server" ControlToValidate="txtssnLast4"
                                                CssClass="requiredfiled" Display="Dynamic" ValidationExpression="\d{4}">* Last 4 of the SSN must be four (4) digits.</asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="rfvSSN" CssClass="requiredfiled" runat="server" Text="* Last four (4) digits of your SSN are required."
                                                ControlToValidate="txtssnLast4" Display="Dynamic"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            DEA Number
                                        </td>
                                        <td>
                                            <asp:TextBox Enabled="false" ID="txtDEA" runat="server" MaxLength="12"
                                                Width="300px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 170px">
                                            DEA Schedule
                                        </td>
                                        <td>
                                            <asp:CheckBoxList ID="chkDEAList" Enabled="false" runat="server" RepeatDirection="Horizontal" Width="151px"
                                                AutoPostBack="False">
                                                <asp:ListItem Value="two">II</asp:ListItem>
                                                <asp:ListItem Value="three">III</asp:ListItem>
                                                <asp:ListItem Value="four">IV</asp:ListItem>
                                                <asp:ListItem Value="five">V</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td runat="server" align="Right" id="Td1" style="width: 170px">
                                             NPI
                                        </td>
                                        <td>
                                            <asp:TextBox Enabled="false" ID="txtUPIN" runat="server" Width="300px"
                                                MaxLength="10"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnAccept" runat="server" CssClass="btnstyle" Text="SUBMIT" ToolTip="I agree and accept the entered values "
                                                OnClick="btnAccept_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btnDecline" runat="server" CssClass="btnstyle" CausesValidation="false"
                                                Text="CANCEL" ToolTip="I declined the ID verification" OnClick="btnDecline_Click" />
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
          
         <asp:Panel Style="display: none" ID="pnlWarning" runat="server">
        <div id="div6" runat="server" style="border: solid 1px black; background-color: White;
            padding: 10px; width: 600px;">
            <div style="padding-bottom: 10px">
                <span class="Phead">Please Read</span>
            </div>
            <div style="padding-bottom: 10px; padding-left: 30px">
                <p>
                    You will now be asked questions about your identity. Be aware that you will only
                    have a few minutes to answer the following questions.
                </p>
                <p style="padding-top: 10px">
                    If you do not answer the questions within the allocated time, you will not be able
                    to complete electronic registration.
                </p>
                <p style="padding-top: 10px">
                    Please answer these questions as quickly as possible to avoid delays in your registration
                    process.
                </p>
              
            </div>
            <div style="text-align: right">
                <asp:LinkButton ID="lnkCancel" runat="server" Text="Cancel"></asp:LinkButton>
                <asp:Button ID="btnProceed" runat="server" Text="Go to Next Step" Width="150px" CssClass="btnstyle"
                    OnClick="btnProceed_Click" />
            </div>
        </div>
    </asp:Panel>
<asp:Button ID="hiddenWarning" runat="server" Style="display: none;" /> 
        <ajaxToolkit:ModalPopupExtender ID="mpeWarning" runat="server" BehaviorID="mpeWarning" DropShadow="false" BackgroundCssClass="modalBackground"
             TargetControlID="hiddenWarning" PopupControlID="pnlWarning" CancelControlID="lnkCancel"></ajaxToolkit:ModalPopupExtender>
            
    </form>
 <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
