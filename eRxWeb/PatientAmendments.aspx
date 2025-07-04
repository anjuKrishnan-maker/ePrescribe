<%@ Page Title="" Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" CodeBehind="PatientAmendments.aspx.cs" Inherits="eRxWeb.PatientAmendments" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
   <script type="text/javascript" src="js/sigUtil.js?version=<%=SessionAppVersion%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="h1title">
        &nbsp;<span id="heading" runat="server" class="highlight">Patient Amendments</span>
    </div>
    <div class="h4title" style="height: 25px;">
        &nbsp;<asp:Button ID="btnBack" Text="Back" OnClick="btnBack_OnClick" CssClass="btnstyle btnHeaderAlign" runat="server"></asp:Button>
        <asp:Button ID="btnAddAmendment" Text="Add Amendment" CssClass="btnstyle" runat="server"/>
    </div>
    <div>
        <telerik:RadGrid ID="grdAmendments" OnNeedDataSource="grdAmendments_OnNeedDataSource" EnableEmbeddedSkins="False" Skin="Allscripts" runat="server" AutoGenerateColumns="False" CellSpacing="0" GridLines="None">
            <MasterTableView>
                <Columns>
                    <telerik:GridBoundColumn FilterControlAltText="Filter colDate column" DataField="Date" HeaderText="Date" UniqueName="colDate">
                        <ItemStyle Width="100px"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn FilterControlAltText="Filter colUser column" DataField="User" HeaderText="User" UniqueName="colUser">
                        <ItemStyle Width="200px"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn FilterControlAltText="Filter colDescription column" DataField="Description" HeaderText="Description" UniqueName="colDescription">
                        <ItemStyle Width="900px"></ItemStyle>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn FilterControlAltText="Filter colResolution column" DataField="Resolution" HeaderText="Resolution" UniqueName="colResolution">
                        <ItemStyle Width="100px"></ItemStyle>
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <FilterMenu EnableImageSprites="False" EnableEmbeddedSkins="False"></FilterMenu>
        </telerik:RadGrid>
    </div>
     <asp:Panel ID="pnlAddAmendment" runat="server">
        <div class="overlaymain" style="padding-top: 0; width: 800px;">
            <div class="overlayTitle">
                Add Amendment
            </div>
            <div>
                <ePrescribe:Message ID="ucMessage" Icon="ERROR" runat="server" Visible="false" />
            </div>
            <div class="overlayContent">
                <div>
                </div>
               <div style="margin-top: -2px; margin-bottom: 30px;">
                   <div class="floatleft" style="width: 33%">Patient: <asp:Label runat="server" ID="lblPatientName"></asp:Label></div>
                   <div class="floatleft" style="width: 33%">Date: <asp:Label runat="server" ID="lblTodaysDate"></asp:Label></div>
                   <div class="floatleft valgn" style="width: 33%; margin-top: -3px;">
                       <asp:RadioButtonList RepeatDirection="Horizontal" ValidationGroup="errors" CausesValidation="True" ID="rdbResolution" runat="server">
                           <asp:ListItem>Accepted</asp:ListItem>
                           <asp:ListItem>Denied</asp:ListItem>
                       </asp:RadioButtonList>
                   </div>
               </div>
                <div style="margin-bottom: 20px;">
                    Enter description of the requested amendment. Indicate if amendment was accepted or denied. When finished click Save.
                </div>
                <div>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtDescription" ValidationGroup="errors"
                        ErrorMessage="Invalid description. Please enter valid description." ValidationExpression=""
                        Display="None"><br/>
                    </asp:RegularExpressionValidator>
                    <asp:TextBox ID="txtDescription" runat="server" 
                        onkeydown="return LimitInput(this, 1000, event);" 
                        onkeyup="return LimitInput(this, 1000, event);" 
                        onpaste="return LimitPaste(this, 1000, event);" 
                        onchange="return LimitChange(this, 1000, event);"
                        TextMode="MultiLine" Width="100%" Height="200px" MaxLength="1000"></asp:TextBox>
                </div>
                 <div id="divMaxCharacters" runat="server" class="normaltext">
                    (Maximum 1000 Characters / <span id="charsRemaining" runat="server" name="charsRemaining">
                        1000</span> characters remaining)
                </div>
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnSave" OnClick="btnSave_OnClick" Text="Save" CssClass="btnstyle btnStyleAction" runat="server" />
                <asp:Button ID="btnCancel" Text="Cancel" CssClass="btnstyle" runat="server" />
            </div>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeAddAmendment" runat="server" CancelControlID="btnCancel" TargetControlID="btnAddAmendment"
        DropShadow="false" PopupControlID="pnlAddAmendment" BackgroundCssClass="modalBackground">
    </ajaxToolkit:ModalPopupExtender>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
