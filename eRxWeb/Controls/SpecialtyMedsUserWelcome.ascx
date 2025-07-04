<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecialtyMedsUserWelcome.ascx.cs" Inherits="eRxWeb.SpecialtyMedsUserWelcome" %>
<asp:Panel ID="pnlSpecMedsWelcomePopUp" runat="server"  Style="position: relative;">
                   
<div id="divEncapsulatingAllElements" class="overlaymain" style="padding: 0">
<div class="overlayTitle">
Patient Access Services
</div>
<div class="overlayContent">                                          
     <br />
    <br />
            <p style="font-size: 11pt; font-weight: normal;">
               Veradigm is working with a third party, AssistRx, that provides physicians with access to programs for specialty medications (AssistRx Program). <br />   <br />
               These specialty medication programs are sponsored by pharmaceutical companies.  Your practice has signed up with Veradigm to gain access to the 
                AssistRx Program, and it is available to you if you believe it to provide benefits for your patients.  There is no obligation to use any of the 
                offered programs.   <br /><br />Please note that when you click the links to refer a patient to these sponsored pharmaceutical assistance programs for specialty 
                medications, you will be sending PHI to pharmaceutical companies participating in the AssistRx Program.  If a patient elects to participate in the 
                AssistRx Program, additional patient consent may be required.
                                <br />
                                <br />
                 <span style="font-size: 11pt; font-weight: bold;">Click "OK" to Continue.</span>. 
              </p> 
    
    </div>

<div class="overlayFooter">
     <asp:Button ID="btnOk" runat="server" CssClass="btnstyle btnStyleAction" OnClick="btnOk_Click" Text="OK" Width="70px" />
 
    <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" OnClick="btnBack_Click" Text="Back" Width="70px" />
</div>
    
</div>

</asp:Panel>
<ajaxToolkit:ModalPopupExtender ID="mpeSpecMedsWelcome" runat="server" TargetControlID="btnSpecMedTrigger" 
      DropShadow="false" PopupControlID="pnlSpecMedsWelcomePopUp" BackgroundCssClass="modalBackground">    
</ajaxToolkit:ModalPopupExtender> 
<asp:Button ID="btnSpecMedTrigger" runat="server" CausesValidation="false" Text="Button"
     Style="display: none;" />

