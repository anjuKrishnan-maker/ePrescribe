<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPageNew.master" AutoEventWireup="true" Inherits="eRxWeb.Help_Import" Title="Veradigm ePrescribe Help - Import Patients" Codebehind="Import.aspx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div>  
    <ul class="list-group">
      <li class="list-group-item imp-ptn-box-top"> 
          <div class="pull-left imp-ptn-box-left-icon">
             </div>
          <div id="autoPanel" runat="server" style="text-align:left;">
        <div class="ImportItem">
            Automated Patient Import Systems
        </div>
        <p> Make it easy! An automated import system works with your scheduling and billing systems to automatically send patient information into ePrescribe.</p>
        <div id="divGetStarted" runat="server">
            <a id="autoLink" runat="server" href="InterfaceList.aspx" class="link">Click here to get started.</a>
       </div>
       <div id="divRestricted" runat="server" style="display: none; font-style: italic;">
            <br />
            We've detected that you have an interface in place. Accounts with active patient demographics interfaces are prohibited from ordering another interface. If you believe you've received this message in error, please contact Customer Support.
       </div>

    </div> </li>
    </ul>
</div>
<asp:Panel style="DISPLAY: none" id="panelAd" runat="server" CssClass="popou-background">    
    <div id="div4" runat="server" class="overlaymain">
        <div id="div1" runat="server" class="overlaysub1">
            <ePrescribe:AdControl id="adControl" runat="server" Show="true" SkipTime="-1" DisplayMode="MODAL" ></ePrescribe:AdControl>
            <br />
            <asp:Button ID="btnAdCancel" runat="server" Text="OK" CssClass="btnstyle" />
        </div>
    </div>
</asp:Panel>
<asp:Button ID="hiddenAd" runat="server" style="display:none;" />
<ajaxToolkit:ModalPopupExtender id="mpeAd" runat="server" BehaviorID="mpeAd" DropShadow="false" BackgroundCssClass="modalBackground"  TargetControlID="hiddenAd"  PopupControlID="panelAd" CancelControlID="btnAdCancel"></ajaxToolkit:ModalPopupExtender>


<asp:Panel style="DISPLAY: none" id="panelAd1" runat="server" CssClass="popou-background">    
    <div id="div5" runat="server" class="overlaymain">
        <div id="div6" runat="server" class="overlaysub1">
            <ePrescribe:AdControl id="adControl1" runat="server" Show="true" SkipTime="-1" DisplayMode="MODAL" ></ePrescribe:AdControl>
            <br />
            <asp:Button ID="btnAdCancel1" runat="server" Text="OK" CssClass="btnstyle" />
        </div>
    </div>
</asp:Panel>
<asp:Button ID="hiddenAd1" runat="server" style="display:none;" />
<ajaxToolkit:ModalPopupExtender id="mpeAd1" runat="server" BehaviorID="mpeAd1" DropShadow="false" BackgroundCssClass="modalBackground"  TargetControlID="hiddenAd1"  PopupControlID="panelAd1" CancelControlID="btnAdCancel1"></ajaxToolkit:ModalPopupExtender>    
<asp:Button ID="hiddenMPL" runat="server" style="display:none;" />
</asp:Content>




