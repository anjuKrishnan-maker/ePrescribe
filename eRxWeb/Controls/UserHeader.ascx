<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_UserHeader" Codebehind="UserHeader.ascx.cs" %>
<div  id="userPanel" runat="server" style="display:inline">
    <div id="loggedIn" runat="server">
        <span >
            <b><a class="navbar-link" id="userName" runat="server"></a></b>
            &nbsp&nbsp<asp:HyperLink ID="lnkLogout1" runat="server" NavigateUrl="~/logout.aspx"><img src="../images/shim.gif" id="logoutIcon" style="vertical-align:middle; border:0px"/></asp:HyperLink>
        </span>
    </div>
    <div id="loggedOut" runat="server">
        <span class="copyHeader">
            <b><a id="labelName" runat="server">User ID:</a></b>&nbsp<input id="txtUserName" runat="server" type="text" style="width:120px"/>
            &nbsp&nbsp<b><a id="labelPassword" runat="server">Password:</a></b>&nbsp<input id="txtPassword" runat="server" type="password" style="width:120px" />
            &nbsp&nbsp<asp:Button ID="lnkLogin" runat="server" Text="Log In" OnClick="lnkLogin_Click" CssClass="btnstyle" ></asp:Button>
        </span>
    </div>
</div>
