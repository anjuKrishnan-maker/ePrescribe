<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_DEALicenseDetails" Codebehind="DEALicenseDetails.ascx.cs" %>
<%@ Import Namespace="eRxWeb.AppCode" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadScriptBlock ID="RadScriptBlock1" runat="server"> 
<script type="text/javascript">

    function updateDeaSchedulesLicense(radioButtonNadea) {
        var chkDeaSchedule2 = document.getElementById('<%=chkDEAIIAllowed.ClientID%>');
        var chkDeaSchedule3 = document.getElementById('<%=chkDEAIIIAllowed.ClientID%>');
        var chkDeaSchedule4 = document.getElementById('<%=chkDEAIVAllowed.ClientID%>');
        var chkDeaSchedule5 = document.getElementById('<%=chkDEAVAllowed.ClientID%>');

        chkDeaSchedule2.checked = radioButtonNadea.checked;
        chkDeaSchedule3.checked = radioButtonNadea.checked;
        chkDeaSchedule4.checked = radioButtonNadea.checked;
        chkDeaSchedule5.checked = radioButtonNadea.checked;
    }
</script>
</telerik:RadScriptBlock> 
<style type="text/css">
    .auto-style2 {
        width: 372px;
    }
    .auto-style3 {
        width: 175px;
    }
    .auto-style4 {
        width: 124px;
    }
</style>
<table id="Table1" cellspacing="2" cellpadding="1" width="100%" border="1" rules="none" style="border-collapse: collapse">
    <tr>
        <td style="text-align: right" class="auto-style2">
            DEA #:
        </td>
        <td class="auto-style3">
            <telerik:RadTextBox ID="txtDEANo" runat="server" MaxLength="30" Width="200px" Text='<%# DataBinder.Eval( Container, "DataItem.DEANumber") %>' ToolTip="DEA format is 2 characters + 7 digits, and if necessary, followed by a dash and an alphanumeric suffix up to 20 characters. For example: AA1234567-001" ></telerik:RadTextBox>
            <asp:RegularExpressionValidator ID="revDEANo" runat="server" ControlToValidate="txtDEANo" ErrorMessage="Invalid Format" Display="Dynamic" ValidationExpression="^[a-zA-Z]{2}\d{7}(-{1}[a-zA-Z\d]{0,20})?$"></asp:RegularExpressionValidator>
            <!--^[a-zA-Z]{2}\d{7}-?[a-zA-Z\d]{0,20}$-->
            <!--^[a-zA-Z]{2}\d{7}(-{1}[a-zA-Z\d]{0,20})?$-->
            <!--^[a-zA-Z]{2}\d{7}(?:-{1}[a-zA-Z\d]{0,20})?$-->
        </td>
    </tr>
    <tr>
        <td style="text-align: right" class="auto-style2">
            DEA Expiration Date:
        </td>
        <td class="auto-style3">
       <telerik:RadDatePicker ID="deaExpDate" Style="vertical-align: middle;" runat="server" SelectedDate='<%#  DateTimeHelper.GetValidFutureDateOrToday(DataBinder.Eval( Container, "DataItem.DEAExpirationDate")) %>'>        
        </telerik:RadDatePicker>      
        </td>
    </tr>   
    <tr>      
        <td style="text-align: right" class="auto-style2">
            DEA Type:
        </td>
        <td class="auto-style2">
            <asp:RadioButton ID="RbDEAPrimary" runat="server" GroupName="DeaType"
                             Text="Primary" AutoPostBack="true" Checked ='<%# DataItem!=null && IsDeaSelected(DataBinder.Eval( Container, "DataItem.DEALicenseTypeID"), "DefaultDea") %>'/>
            <asp:RadioButton ID="RbNadean" runat="server" GroupName="DeaType"
                             Text="NADEAN"  onClick="updateDeaSchedulesLicense(this);" Checked ='<%# DataItem!=null && IsDeaSelected(DataBinder.Eval( Container, "DataItem.DEALicenseTypeID"), "NaDean") %>'/>
            <asp:RadioButton ID="RbDEAAdditional" runat="server" GroupName="DeaType"
                             Text="Additional" AutoPostBack="true"  Checked ='<%# DataItem!=null && IsDeaSelected(DataBinder.Eval( Container, "DataItem.DEALicenseTypeID"), "Dea") %>'/>
        </td>

    </tr>   
    <tr>  
     <td style="text-align: right" class="auto-style2">
           DEA Schedule:
        </td>      
        <td class="auto-style3"> 
           <asp:CheckBox ID="chkDEAIIAllowed"  runat="server" CssClass="indnt" Text="II" Checked ='<%# CheckAndCast(DataBinder.Eval( Container, "DataItem.DEAIIAllowed")) %>'/>                     
           <asp:CheckBox ID="chkDEAIIIAllowed" runat="server" CssClass="indnt" Text="III" Checked ='<%# CheckAndCast(DataBinder.Eval( Container, "DataItem.DEAIIIAllowed")) %>'/>                     
           <asp:CheckBox ID="chkDEAIVAllowed" runat="server" CssClass="indnt" Text="IV" Checked ='<%# CheckAndCast(DataBinder.Eval( Container, "DataItem.DEAIVAllowed")) %>'/>                     
           <asp:CheckBox ID="chkDEAVAllowed"  runat="server" CssClass="indnt" Text="V" Checked ='<%# CheckAndCast(DataBinder.Eval( Container, "DataItem.DEAVAllowed")) %>'/>                      
        </td>
    </tr>     
    <tr>
        <td colspan="4"
            align="right">
            <asp:Button ID="btnUpdate" Text="Update" CssClass="btnstyle" runat="server" CommandName="Update"
                Visible='<%# !(DataItem is Telerik.Web.UI.GridInsertionObject) %>' >
            </asp:Button>
            <asp:Button ID="btnInsert" Text="Insert" CssClass="btnstyle" runat="server" CommandName="PerformInsert"
                Visible='<%# DataItem is Telerik.Web.UI.GridInsertionObject %>'></asp:Button>
            &nbsp;
            <asp:Button ID="btnCancel" Text="Cancel" CssClass="btnstyle" runat="server" CausesValidation="False"
                CommandName="Cancel"></asp:Button>
        </td>
    </tr>
</table>