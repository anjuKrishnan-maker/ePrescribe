<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" 
    Inherits="eRxWeb.UserPreferences" Title="DUR Settings" Codebehind="UserPreferences.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script language="javascript" type="text/javascript">
    function init()
    {
        var chkPerformFormulary=document.getElementById("chkPerformFormulary")
        if(chkPerformFormulary != null)
            enabledisable(chkPerformFormulary);
        
    }
    function enabledisable(chkPerformFormulary)
    {
        
        var chk=document.getElementById("radioNoAlternatives")
        if( chk!= null)
            chk.disabled = !chkPerformFormulary.checked;
        chk=document.getElementById("radioYesAlternatives")
        if( chk!= null)
            chk.disabled = !chkPerformFormulary.checked;
        chk=document.getElementById("radioRedFaceAlternatives")
        if( chk!= null)
            chk.disabled = !chkPerformFormulary.checked;
    }
    
    </script>

        <div>
            <table width="100%"  border="0"  cellspacing="0" cellpadding="0">
              <tr class="h1title">
                    <td colspan="2">
                     </td>
                </tr>
                <tr class="h2title">
                    <td colspan="2">
                    </td>
                 </tr>
				<tr class="h4title">
					<td colspan="2">
						&nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" OnClick="btnCancel_Click"
							Text="Back" ToolTip="Click here to cancel your changes." />&nbsp;
						<asp:Button ID="btnSave" runat="server" CssClass="btnstyle" OnClick="btnSave_Click"
							Text="Save" ToolTip="Click here to save your changes." />
						</td>
				</tr>
              <tr><td colspan="2">
              <table width="100%"  border="1"  cellspacing="0" cellpadding="0" bordercolor="#b5c4c4">
                <tr>
                    <td style="width: 50%;" valign="top">
                        <table width="100%"  border="0" >
                            <caption class="DURHeading primaryForeColor">
                                DUR General</caption>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkPerformPAR" runat="server" Text="Perform Prior Adverse Reaction checks?"
                                        Width="273px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkPerformDT" runat="server" Text="Perform Duplicate Therapy checks?"
                                        Width="270px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkPerformDosage" runat="server" Text="Perform Dosage checks?" 
                                        Width="267px"  /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkPerformInteraction" runat="server" Text="Perform Drug Interaction checks?"
                                        Width="260px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkRequireReason" runat="server" Text="Require DUR Ignore Reasons?"
                                        Width="260px" /></td>
                            </tr>                            
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkRequireFormularyReason" runat="server" Text="Require Formulary Ignore Reasons?"
                                        Width="260px" /></td>
                            </tr>                            
                        </table>
                    </td>
                    <td valign="top" >
                        <table width="100%"  border="0">
                            <caption class="DURHeading primaryForeColor">
                                DUR Dosage*</caption>
                            <tr style="display: none">
                                <td>
                                    <asp:CheckBox ID="chkScreenMaxConsecutive" runat="server" Text="Screen for maximum consecutive duration?"
                                        Width="300px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkScreenMaxIndividualDose" runat="server" Text="Screen for maximum INDIVIDUAL dose?"
                                        Width="300px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkScreenMinDuration" runat="server" Text="Screen for minimum duration?"
                                        Width="273px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkScreenMaxDose" runat="server" Text="Screen for maximum dosage?"
                                        Width="220px" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkScreenMinDose" runat="server" Text="Screen for minimum dosage?"
                                        Width="220px" OnCheckedChanged="chkScreenMinDose_CheckedChanged" /></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkScreenMaxDuration" runat="server" Text="Screen for maximum duration?"
                                        Width="220px" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <table width="100%"  border="0">
                            <caption class="DURHeading primaryForeColor">
                                DUR DuplicateTherapy</caption>
                            <tr>
                                <td >
                                    Screen for full range of duplicate therapies -or- only those that present an abuse
                                    or dependency potential?</td>
                                <td>
                                    <asp:DropDownList ID="ddlDTScreen" runat="server" Width="100%">
                                        <asp:ListItem Value="N">Full Range</asp:ListItem>
                                        <asp:ListItem Value="Y">Abuse/Dependency</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <!-- This TR added by JJ 14th Aug -->
                            <tr><td colspan="2"></td></tr>
                            <tr>
                                <td >
                                    Display warnings for:</td>
                                <td>
                                    <asp:DropDownList Width="260px" ID="ddlDTWarning" runat="server">
                                        <asp:ListItem Value="N">All Duplications</asp:ListItem>
                                        <asp:ListItem Width="100%" Value="Y">Medi-Span Duplication Allowance Exceeded</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                        </table>
                        </td>
                    <td valign="top">
                        <table width="100%"  border="0">
                            <caption class="DURHeading primaryForeColor">
                                DUR Interaction</caption>
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox ID="chkScreenFood" runat="server" Text="Screen for Food interactions?*"
                                        Width="278px" /></td>
                            </tr>
                            <tr> 
                                <td colspan="2" >
                                    <asp:CheckBox ID="chkScreenAlcohol" runat="server" Text="Screen for Alcohol interactions?*"/></td>
                            </tr>
                          
                            <tr>
                                <td>
                                    Minimum Onset Level:</td>
                                <td style="width: 165px">
                                    <asp:DropDownList ID="ddlMinOnSet" runat="server" Width="100%">
                                        <asp:ListItem Value="1">Delayed</asp:ListItem>
                                        <asp:ListItem Value="2">Rapid</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>
                                    Minimum Severity Level:</td>
                                <td style="width: 165px">
                                    <asp:DropDownList ID="ddlMinSeverity" runat="server" Width="100%">
                                        <asp:ListItem Value="1">Minor</asp:ListItem>
                                        <asp:ListItem Value="2">Moderate</asp:ListItem>
                                        <asp:ListItem Value="3">Major</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>
                                    Minimum Documentation Level:</td>
                                <td>
                                    <asp:DropDownList ID="ddlMinDocumentation" runat="server" Width="100%">
                                        <asp:ListItem Value="1">Doubtful/Unknown</asp:ListItem>
                                        <asp:ListItem Value="2">Possible</asp:ListItem>
                                        <asp:ListItem Value="3">Suspected</asp:ListItem>
                                        <asp:ListItem Value="4">Probable</asp:ListItem>
                                        <asp:ListItem Value="5">Established</asp:ListItem>
                                    </asp:DropDownList></td>
                            </tr>
                        </table>
                        * does not apply to pre-emptive DUR</td>
                </tr>
             </table>
            </td>
            </tr>
            </table>
        </div>
       
    </asp:Content>
 
<%-- <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server" >
  <DIV id="quickSearchContent">   
    <table bgcolor="e7e7e7" width="195px">
        <tr>
            <td colspan="2" align="center" class="h2title highlight"><strong>Did you know?</strong>
               </td>
        </tr>
        <tr width="195px" background-color="#ffffff">
            <td width="19px">&nbsp;</td>
            <td width="176px"></td>
        </tr>
        <tr>
            <td colspan="2" background-color="#868585" height="2px">&nbsp;
            </td> 
        </tr>
      </table>
   
  </DIV>
</asp:Content>--%>