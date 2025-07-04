<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.DeluxAdContainer" Title="Library" Codebehind="DeluxAdContainer.aspx.cs" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   
  <script type="text/javascript" src="<%=ResolveUrl("~/js/PageUtil.js?version="+ SessionAppVersion) %>"></script>
<table border="0" cellspacing="0" cellpadding="0" width ="100%" >
    <tr class="h1title">            
         <td  align="right"> 
         </td>
    </tr>    
    <tr>
        <td>
            <center>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <ePrescribe:AdControl id="adControl" runat="server" Show="false" SkipTime="-1" DisplayMode="MODAL"></ePrescribe:AdControl>                                               
                        </td>
                    </tr>
                </table>
            </center>         
        </td>
    </tr>
</table>
</asp:Content>
