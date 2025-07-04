<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogRxDisplay.aspx.cs" Inherits="eRxWeb.LogRxDisplay" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title> 
    <link href="Style/AdvertisementTag.css" rel="stylesheet" />
</head>
    <%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    
    <form id="form1" runat="server">
     <div id="x">

                <%if (PlacementResponse != null)
                  { %>
                <div id="divRightHandAd" >
                    
                    <div style="margin-left: 5px; margin-top: 2px;">
                         <% if (PlacementResponse.Content["right_hand"].ToString() != string.Empty)
                       {%>
                        <div id="divAdvertisementHeader" class="advertisement-tag_block gray-font" >Advertisement</div>
                         <%  }%>
                        <%= PlacementResponse.Content["right_hand"]%>
                    </div>
                    <% if (PlacementResponse.Content["right_hand"].ToString() != string.Empty)
                       {%>
                     
                    <div id="divADPrivacyPolicy" runat="server" style="margin-left: 5px; margin-bottom: 2px;">
                       
                        <span class="smalltext">
                            <asp:HyperLink ID="lnkADPrivacyPolicy" runat="server" NavigateUrl="~/AdPrivacyPolicy.aspx"
                                Target="_blank">Ad Privacy Policy
                            </asp:HyperLink>
                        </span>
                    </div>
                    <%  }%>
                </div>
                <%} %>
                    </div>
    </form>
</body>
</html>
