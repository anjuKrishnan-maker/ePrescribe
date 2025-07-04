<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="eRxWeb.Controls_ObsoletePARClassMappingChange" Codebehind="ObsoletePARClassMappingChange.ascx.cs" %>
<%@ Register Src="Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Panel style="display: none" ID="panelPARClasses" runat="server">
    <div id="divPARClasses" runat="server" class="overlaymain">
        <div class="overlaysub1">
            <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            <b>The patient you selected has the following Par Class Allergies that have been made
                inactive. </b>
            <br />
            <br />
            <telerik:RadListBox runat="server" ID="rlbObsoletedPatientParClass" Width="580px" Height="200px" CssClass="Phead">
            </telerik:RadListBox>
            <asp:ObjectDataSource ID="odsObsoletedPatientParClass" runat="server" SelectMethod="PatientAllergy"
                TypeName="Allscripts.Impact.Patient">
                <SelectParameters>
                    <asp:SessionParameter Name="patientID" SessionField="PATIENTID" />
                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
            <br />
            <p>
                Please inactivate the listed Par Classes from the patient’s allergy list and identify
                a more specific or other available Par Classes that better identify the allergy.
                You will not be allowed to prescribe medications for this patient until the Allergy
                selection has been corrected.</p>
            <br />
        </div>
        <div class="overlayFooter">
            <asp:Button ID="btnPARContinue" runat="server" CausesValidation="true" CssClass="btnstyle btnStyleAction"
                Text="Continue" OnClick="btnPARContinue_Click" />
            </div>
    </div>
</asp:Panel>
<asp:Button ID="hiddenPARClasses" runat="server" Style="display: none;" />
<ajaxToolkit:ModalPopupExtender ID="mpePARClasses" runat="server" BehaviorID="mpePARClasses"
    DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenPARClasses"
    PopupControlID="panelPARClasses">
</ajaxToolkit:ModalPopupExtender>
