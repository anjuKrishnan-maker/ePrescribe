<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.Help_Mobile" Title="Untitled Page" Codebehind="Mobile.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <span style="font-size:medium; font-weight:bold;">How do I access ePrescribe from my iPhone?</span>
    <br /><br />
    You must be a Deluxe Customer to prescribe directly from your iPhone. <a id="purchaseLink" runat="server" onserverclick="PurchaseLink_ServerClick" href="#">Click here</a> to subscribe to Deluxe.<br /> Then visit the app store to access the free iPhone application and then configure it for use.<br />    
    <br />
    Please note, the app can only be accessed by the provider subscribing to the ePrescribe Deluxe service.  Although many people within your practice may have access to other Veradigm products, only providers can access the app, as prescriptions will be sent from here, and not all users have the authority to write prescriptions.<br />
    <br />
    The ePrescribe Remote application is strictly for prescribing on the go!<br />
    <p id="pIPhone" runat="server">
        <a href="https://eprescribe.allscripts.com/media/files/iPhone User Guide.pdf" target="_blank">Click here</a> to view the iPhone user guide.<br />
        <br />
    </p>
</asp:Content>