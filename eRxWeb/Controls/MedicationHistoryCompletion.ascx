<%@ Control Language="C#" AutoEventWireup="True" Inherits="eRxWeb.Controls_MedicationHistoryCompletion" Codebehind="MedicationHistoryCompletion.ascx.cs" %>
<%@ Register Src="Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>

<script language="javascript" type="text/javascript">
    function closeBackDrop() {
        window.parent.CloseOverlay();

    }
    function SelectAllChecked() {
        var chkAll = document.getElementById("<%=chkSelectAll.ClientID %>");
        var chkblHistory = document.getElementById("<%= cblHistory.ClientID %>");
        var lengthCBHistory = chkblHistory.rows.length;
        if (chkAll.checked) {
            for (var i = 0; i < lengthCBHistory; i++) {
                document.getElementById(chkblHistory.id + "_" + i).checked = true;
            }
        }
        else {
            for (var i = 0; i < lengthCBHistory; i++) {
                document.getElementById(chkblHistory.id + "_" + i).checked = false;
            }
        }
    }

    //Change the text of the pressed button to "Loading..." and disable the two non-cancel buttons
    var hasAButtonBeenClicked = false;
    function changeText(btn)
    {
        if(!hasAButtonBeenClicked){
            hasAButtonBeenClicked = true;
            btn.value = "Loading...";
            disableButtons();            
            return true;
        }
        return false;
    }

    //Disable the two non-cancel buttons
    function disableButtons()
    {
        document.getElementById("<%=btnContinue.ClientID %>").disabled = true;
        document.getElementById("<%=btnCompleteAndContinue.ClientID %>").disabled = true;
    }


</script>
   
<asp:Panel style="display: none;top:10px" id="panelHistory" runat="server">
    <div id="divHistory" runat="server" class="overlaymain" style="padding: 0">
        <div class="overlayTitle" >
            Duplicate Therapy Notification
        </div>
        <div class="overlaysub1">
            <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            <p><b>The following prescriptions are currently active in this patient’s history.</b></p>
            <p style="margin-top: 5px">To complete a prescription, select the medication(s) below and click the <i>Mark as Complete and Continue</i> button.</p>
            <p style="margin-top: 5px">The new prescriptions you create will become the active medications for this patient. 
               If active medications are not completed you may receive a Duplicate Therapy warning displayed on the DUR Review page.</p>
            <br />
            <div class="overlayscrollable">
                &nbsp;<input type="checkbox" runat="server" id="chkSelectAll" visible="true"  value="Select All" onclick="this.blur(); this.focus();"  
                onchange="SelectAllChecked()" style="font-weight:bold"/>
                <span><b><u>Select All</u></b></span>
                <br />           
                <asp:CheckBoxList ID="cblHistory" runat="server">               
                </asp:CheckBoxList>
            </div>
            <br />
        </div>
        <div style="text-align: center" class="overlayFooter">
            <asp:Button ID="btnContinue" runat="server" CssClass="btnstyle" CausesValidation="false" Text="Continue" OnClientClick="changeText(this);" OnClick="btnContinue_Click" UseSubmitBehavior="false"/>&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCompleteAndContinue" runat="server" CausesValidation="true" CssClass="btnstyle" Text="Mark As Complete And Continue" OnClientClick="changeText(this);" OnClick="btnCompleteAndContinue_Click" UseSubmitBehavior="false" />&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnHxCancel" runat="server" CssClass="btnstyle" CausesValidation="false" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="closeBackDrop()" />
        </div>
    </div>
</asp:Panel>
<asp:Button ID="hiddenHistory" runat="server" style="display:none;" />
<ajaxToolkit:ModalPopupExtender id="mpeHistory" Y="10"   runat="server" BehaviorID="mpeHistory" DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenHistory"  PopupControlID="panelHistory"></ajaxToolkit:ModalPopupExtender>