<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.Library" Title="Library" Codebehind="Library.aspx.cs" %>
<%@ Register Src="Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <script type="text/javascript" language="javascript">
            window.onload = function () {
                var obj = document.getElementById('ctl00_ContentPlaceHolder1_librarySSO');
                var obj2 = document.getElementById('iframe1');
                if (obj != null && obj2 != null) // To fix the issue with the IE 8 browser (the grid hieght was 272px, because of which the scroll bar was coming)
                {
                    obj.style.height = "600px";
                    obj2.style.height = "600px";
                }

            } 
 </script>
    <script type="text/javascript" src="<%=ResolveUrl("~/js/PageUtil.js?version="+ SessionAppVersion) %>"></script>
<table border="0" cellspacing="0" cellpadding="0" width ="100%" >
    <tr class="h1title">            
         <td  align="right"> 
         </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="librarySSO" runat="server" >
            </asp:Panel>        
        </td>
    </tr>
    <tr>
        <td>
            <center>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <ePrescribe:AdControl id="adControl" runat="server" Show="false" SkipTime="-1" DisplayMode="FULL_PAGE_TEASER"></ePrescribe:AdControl>                                               
                        </td>
                    </tr>
                </table>
            </center>         
        </td>
    </tr>
</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
</asp:Content>

