<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.UpdateAllergy" Title="Update Allergy" Codebehind="UpdateAllergy.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<script language="javascript" type="text/javascript">
// <!CDATA[

function TABLE1_onclick() {

}

function btnCancel_onclick() {
window.location.href='PatientAllergy.aspx';
}

// ]]>
</script>

    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        
        <tr>
            <td>
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="height: 240px">
                            <table align="right" border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr class="h1title">
                                                <td>
                                                    <div id="divMedication" runat="server" class="Phead">
                                                    </div>
                                                </td>
                                            </tr>
											<tr>
												<td>
													<table width="100%" border="0" cellpadding="0" cellspacing="0">
														<tr class="h4title">
															<td>
																&nbsp;<asp:Button ID="btSaveAllergy" runat="server" CssClass="btnstyle" OnClick="btSaveAllergy_Click"
																	Text="Save" />
																<input id="btnCancel" class="btnstyle" type="button" value="Cancel" onclick="return btnCancel_onclick()" /></td>
														</tr>
													</table>
												</td>
											</tr>
                                            <tr>
                                                <td style="height: 383px">
                                                    <table border="0" cellpadding="0" cellspacing="0" id="TABLE1" onclick="return TABLE1_onclick()">
                                                        <tr class="h2title">
                                                            <td style="width: 59px; height: 20px;">
                                                                <span class="head">&nbsp;</span></td>
                                                            <td style="width: 726px; height: 20px;">
                                                                &nbsp;
                                                                <asp:RadioButton ID="rbtActive" runat="server" Font-Bold="True" GroupName="Active"
                                                                    Text="Active"/>
                                                                <asp:RadioButton ID="rbtInActive" runat="server" Font-Bold="True" GroupName="Active"
                                                                    Text="Inactive" /></td>
                                                        </tr>
                                                        <tr class="h3title"> 
                                                         <td class="Pheadblack indnt" style="width: 59px;">
                                                                Reaction
                                                            </td>
                                                            <td class="Normal" style="width: 726px">
                                                             
                                                            </td>
                                                        </tr>
                                                        
                                                        <tr>
                                                            <td bgcolor="#ffffff" style="width: 59px; height: 197px;">
                                                                &nbsp;</td>
                                                            <td style="height: 197px; width: 726px;">
                                                                <asp:ListBox ID="lstReactions" runat="server" Height="197px" SelectionMode="Multiple"
                                                                    Width="430px"></asp:ListBox></td>
                                                        </tr>
                                                        <tr>
                                                            <td style="height: 42px; width: 59px;">
                                                                <span class="head">&nbsp;</span></td>
                                                            <td style="height: 42px; width: 726px;" class="head">
                                                                <%--<asp:CheckBox ID="chkOther" class="head" runat="server" Height="14px" Width="614px"
                                                                 Text=" Please  click this checkbox to edit/add other reactions." OnCheckedChanged="chkOther_CheckedChanged" />
                                                                 --%>
                                                                   If Reaction not listed above(enter description below)  <br />
                                                        <asp:TextBox ID="txtOther" runat="server" MaxLength="255" Width="545px"></asp:TextBox>           
                                                            </td>      
                                                        </tr>
                                                        <tr>
                                                            <td class="head" style="width: 59px; height: 27px">
                                                            </td>
                                                            <td style="width: 726px; height: 27px">
                                                                <asp:RadioButton ID="rdAllergy" runat="server" Text="Allergy" ForeColor="Black" GroupName="Category"/>
                                                                <asp:RadioButton ID="rdIntolerance" runat="server" Text="Intolerance" ForeColor="Black" GroupName="Category"/></td>
                                                        </tr>
                                                        
                                                        <tr>
                                                            <td class="head" style="width: 59px;">
                                                            </td>
                                                            <td style="width: 726px">
                                                                <asp:ValidationSummary ID="ValSummary" runat="server" Height="1px" Width="426px" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="5">
                                                               
                                                            </td>
                                                        </tr>
                                                      
                                                    </table>
                                                    
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <br />
                <br />
                <br />
                <br />
                <br />
                <p style="margin-left: 10px;"><asp:Label ID="lblMultipleAllergyMsg" runat="server" 
                                                    Text="<br/>If you would like to enter more than one reaction for a Class or Medication, select (highlight) the first reaction from the list. Then hold down the Control Key (Ctrl) and continue to select other reactions. When done, click the Save button. All selected reactions will be associated to the Class or Medication selected."></asp:Label>
                </p>
            </td>
        </tr>
        
    </table>
</asp:Content>
