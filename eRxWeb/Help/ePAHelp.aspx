<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.Help_ePAHelp" Title="EPA" Codebehind="ePAHelp.aspx.cs" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server"> 
    <h1 class ="ePAHeader">
        <b>LOOKING FOR A QUICK GUIDE TO HELP YOU GET STARTED?</b>
     </h1>         

    <p class="copy">
        <a href="/Help/Documents/Veradigm eAuth Quick Guide.pdf" style="color:#5B8F22" target="_blank">Click here</a> to download ePrescribe eAuth Quick Guide?</p>
        <br />
    <h1 class ="ePAHeader" >
        <b>VIDEO TUTORIALS</b>
     </h1>
    <p class="copy">
        <%--<a href="https://eprescribe.allscripts.com/media/tutorials/portal/ePA.htm"style="color:#5B8F22" >Click here</a> to view the ePA tutorial</p>--%>
        <asp:HyperLink ID="lnkILearn" runat="server" Target="_self" >Click here </asp:HyperLink> to view the ePA tutorial</p>
      <%--  <a href="/media/tutorials/FullVideo/curriculum/1.html"style="color:#5B8F22" runat="server" id="lnkLearn" >Click here</a> to view the ePA tutorial</p>--%>
        <br />
    <p class="copy">
        Need support? Contact <a href="mailto:eprescribesupport@allscripts.com" style="color:#5B8F22" >eprescribesupport@allscripts.com</a></p>
</asp:Content>
