<%@ Page Title="" Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="True" Inherits="eRxWeb.SelectAccountAndSite" Codebehind="SelectAccountAndSite.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        setTimeout('expireSession()', 1200000);
    </script>
<div style="padding:5px 5px 5px 15px">
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <b>Please select the account from which you are practicing today:</b>
            </td>
        </tr>
        <tr>
        <td>
        <div style="padding:5px 5px 25px 15px">
        <table>
        
        <tr>
        <td>
        <asp:HiddenField id="HiddenField1" runat="server" />
         <asp:Button ID="hiddenSelect" CssClass="btnstyle" runat="server" Text="" style="display:none" OnClick="hiddenSelect_Click" UseSubmitBehavior="false"/>
                            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" UpdatePanelsRenderMode="Inline">
                                 <AjaxSettings>
                                     <telerik:AjaxSetting AjaxControlID="hiddenSelect">
                                         <UpdatedControls>
                                             <telerik:AjaxUpdatedControl ControlID="radPanelBarAccounts" />
                                         </UpdatedControls>
                                    </telerik:AjaxSetting>
                                 </AjaxSettings>
                            </telerik:RadAjaxManager>

                    <telerik:RadPanelBar ID="radPanelBarAccounts" runat="server"  EnableEmbeddedSkins="false"
                   Style="float: left" AllowCollapseAllItems="True" CssClass="RadPanelBar"
                   Height="200" ExpandMode="FullExpandedItem" 
                oninit="radPanelBarAccounts_Init">
                    <CollapseAnimation Type="None"></CollapseAnimation>
                    
            </telerik:RadPanelBar>
        </td>
        </tr>
        </table>

        </div>
        </td>
        </tr>

        <tr>
        <td>
            <asp:CheckBox runat="server" ID="chkCookie" Text="Keep me logged into the selected site until the end of the day." />
            <ajaxToolkit:MutuallyExclusiveCheckBoxExtender runat="server" ID="mexCookie1" TargetControlID="chkCookie" Key="CookieSetting" ></ajaxToolkit:MutuallyExclusiveCheckBoxExtender>
        </td>
        </tr>
        <tr>
        <td>
            <asp:CheckBox runat="server" ID="chkDefault" Text="Make this my default site (do not ask me again)" />
            <ajaxToolkit:MutuallyExclusiveCheckBoxExtender runat="server" ID="mexCookie2" TargetControlID="chkDefault" Key="CookieSetting" ></ajaxToolkit:MutuallyExclusiveCheckBoxExtender>
        </td>
        </tr>
         <tr>
        <td>&nbsp;
        </td>
        </tr>
        <tr>
        <td>
        <asp:Button ID="btnSelect" runat="server" CssClass="btnstyle" Text="Select" 
                onclick="btnSelect_Click"/>
        </td>
        </tr>
        </table>
</div>
        <telerik:RadCodeBlock ID="gaCode" runat="server" >
          <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>("IsGaEnabled") == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings("GaAccountId") %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
        </telerik:RadCodeBlock>
</asp:Content>

