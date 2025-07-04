
<%@ Page Language="C#" AutoEventWireup="true"  Inherits="eRxWeb.RxDURReview" MasterPageFile="~/PhysicianMasterPage.master" Title="DUR Result" Codebehind="RxDURReview.aspx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="Controls/EPCSDigitalSigning.ascx" TagName="EPCSDigitalSigning" TagPrefix="ePrescribe" %>
<%@ Register Src="~/Controls/CSMedRefillRequestNotAllowed.ascx" TagName="CSMedRefillRequestNotAllowed" TagPrefix="ePrescribe" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderPlaceHolder" runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <ePrescribe:EPCSDigitalSigning ID="ucEPCSDigitalSigning" runat="server" IsScriptForNewRx="false"/>
      <ePrescribe:CSMedRefillRequestNotAllowed ID="ucCSMedRefillRequestNotAllowed" runat="server" HideContactMe = "true" HideContactMeText= "true"
                            YesMessage = "Click <b>YES</b> below to print the prescription." />

        <table style="width: 100%" border="0" cellpadding="0" cellspacing="0" >
            
            <tr class="h1title">
                <td colspan="2">
                    <span class="Phead indnt">DUR Check</span>
                </td>
            </tr>
            <tr class="h2title">
                <td colspan="2" class="adminlink1 indnt">
                    The following medication created warnings during the DUR process. Please review the warnings and take appropriate action or deny this medication from the prescription:
                </td>
            </tr>
            <tr>
                <td colspan="2"  >
                    <asp:GridView ID="gridMedication" runat="server" Width="100%" AutoGenerateColumns="False" AllowPaging="True"  PageSize="5"  OnSelectedIndexChanged="gridMedication_SelectedIndexChanged" OnRowDataBound="gridMedication_RowDataBound"  >
                    <SelectedRowStyle CssClass="SelectedRow" />
                    <PagerStyle CssClass="PagerStyle" ForeColor="White" HorizontalAlign="Center" />
                    <%--  <AlternatingRowStyle CssClass="AlternateItem" /> --%>
                        <Columns>
                        <asp:BoundField HeaderText="Line" DataField="LineNumber" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundField HeaderText="Medication" DataField="MedicationName" HeaderStyle-HorizontalAlign="Left"  />
                            <asp:BoundField HeaderText="Strength" DataField="Strength" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundField HeaderText="Form" DataField="Form" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundField HeaderText="Route" DataField="RouteOfAdmin" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundField HeaderText="Quantity" DataField="Quantity" HeaderStyle-HorizontalAlign="Left" />
                            <asp:BoundField HeaderText="Refill" DataField="RefillQuantity" HeaderStyle-HorizontalAlign="Left" />
                        </Columns>
                       
                        <HeaderStyle  CssClass="GridHeader" HorizontalAlign="Left" />
                       
                    </asp:GridView>
                    
                </td>
            </tr>
            <tr  >
            
                <td colspan="2" >
                    <img src="images/warning.gif" />
                    <span >Warnings/Details:</span></td>
            </tr>
            <tr style="height:328px">
                <td  style="width: 150px;">
                    <asp:ListBox ID="lstWarning" runat="server"  Height="328px" Width="150px" OnSelectedIndexChanged="lstWarning_SelectedIndexChanged" AutoPostBack="True"></asp:ListBox></td>
                <td style="width: 670px;" >
                    
                    <asp:TextBox ID="txtDetails" runat="server" TextMode="MultiLine" CssClass="DURWarning" Height="320px" Width="670px" ReadOnly="true"></asp:TextBox>
                </td>
            </tr>
            <tr class="h4title" align="center">
                <td colspan="2">
                    <asp:Button ID="btnBack" runat="server" CssClass="btnstyle" OnClick="btnBack_Click" Text="Back"  ToolTip="Click here if you want to make changes to the script as suggested by the DUR warnings."/>&nbsp;&nbsp;
                    <asp:Button ID="btnContinue" runat="server" CssClass="btnstyle" OnClick="btnContinue_Click" Text="Ignore/Submit" ToolTip="Click here if you want to ignore the DUR warnings." /></td>
            </tr>
   </table></asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
  
</asp:Content>
