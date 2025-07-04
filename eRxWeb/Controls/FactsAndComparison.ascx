<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.FactsAndComparison" Codebehind="FactsAndComparison.ascx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script src="~/jquery/js/jquery-1.4.4.min.js" type="text/javascript"></script>
<script type="text/javascript">
    function connectToiFC(url)
    {
        var linksHit = document.getElementById("<%=linksHit.ClientID %>");
        if (linksHit != null)
        {
            if (linksHit.value == null || linksHit.value == "")
            {
                linksHit.value = url;
            }
            else
            {
                linksHit.value = linksHit.value + "|" + url;
            }            
        }        
        
        window.parent.OpenUrlInModal(url, "");
    }
</script>
<asp:HiddenField ID="linksHit" runat="server"></asp:HiddenField>
