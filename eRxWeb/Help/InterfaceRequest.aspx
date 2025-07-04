<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPageNew.master" AutoEventWireup="true" Inherits="eRxWeb.Help_InterfaceRequest" Title="Interface Request" Codebehind="InterfaceRequest.aspx.cs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Controls/AdControl.ascx" TagName="AdControl" TagPrefix="ePrescribe" %>
<%@ Register Src="~/Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script type="text/javascript" language="javascript" src="../js/formUtil.js"></script>
    <script type="text/javascript">
        
        function throwPriceDisclaimer()
        {
            $("#disclaimerModal").modal();
        }        
   
    </script>
<div>
    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
    <span class="ImportItem" style="margin-left:50px">
        <a id="lblSystemName" runat="server">Interface Request - </a>&nbsp&nbsp<a id="lnkDisclaimer" runat="server" style="color:Black" onmouseover="throwPriceDisclaimer()">*</a>
    </span>        
</div>
<br />
<asp:RequiredFieldValidator ID="rfvLicenseID" runat="server" ControlToValidate="licenseID" ValidationGroup="maingroup"
    ErrorMessage="Please login above." Width="1px">&nbsp</asp:RequiredFieldValidator>
 <div>
    <table  cellpadding="10" cellspacing="10" width="100%" border="0" style="width: 1000px;" class="tablespacing">
        <tr>
            <td style="text-align:right; width:5%;">
                <a id="pmsLabel" runat="server">PMS Account ID:</a>
                 <b id="pmsRequired" runat="server" style="color:Red">*</b>
                &nbsp&nbsp&nbsp
            </td>           
            <td style="text-align:left" colspan="2">
                <telerik:RadTextBox ID="txtPMSAccountNumber" runat="server" MaxLength="30" onblur="capitilizeInitial(this)"
                    Style="width:150px;height:18px" Skin="Allscripts"  EnableEmbeddedSkins="false"
                    CssClass="txtboxcolor">
                </telerik:RadTextBox>
               
                <asp:RequiredFieldValidator ID="rfvAccountID" runat="server" ControlToValidate="txtPMSAccountNumber" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a practice management system account ID." Width="1px"><b style="color:Red">*</b></asp:RequiredFieldValidator>
            </td>
        </tr>     
        <tr>
            <td style="text-align:right; width:20%;">
                First Name:<span style="color: Red">*</span>
                &nbsp&nbsp&nbsp
            </td>
            <td style="text-align:left" colspan="2">
                <telerik:RadTextBox ID="txtFirstName" runat="server" MaxLength="30" onblur="capitilizeInitial(this)"
                    Style="width:150px;height:18px" Skin="Allscripts"  EnableEmbeddedSkins="false"
                    CssClass="txtboxcolor">
                </telerik:RadTextBox>                
                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a First Name." Width="1px"><b style="color:Red">*</b></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revFirstName" runat="server" ControlToValidate="txtFirstName" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a valid First Name." ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*"
                    Width="1px"><b style="color:Red">*</b></asp:RegularExpressionValidator>                                  
            </td>
        </tr>        
        <tr>
               <td style="text-align:right;width:20%;">
                Last Name:<span style="color: Red">*</span>
                   &nbsp&nbsp&nbsp
                </td>
           
                 <td style="text-align:left" colspan="2">                    
                     <telerik:RadTextBox ID="txtLastName" runat="server" MaxLength="20" onblur="capitilizeInitial(this)"
                    Style="width:150px;height: 18px" Skin="Allscripts" EnableEmbeddedSkins="false"
                    CssClass="txtboxcolor">
                </telerik:RadTextBox>                
                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtlastName" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a Last Name." Width="1px"><b style="color:Red">*</b></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revLastName" runat="server" ControlToValidate="txtlastName" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a valid Last Name." ValidationExpression="^([a-zA-Z0-9]+[\s-'.]{0,35})*"
                    Width="1px"><b style="color:Red">*</b></asp:RegularExpressionValidator>
                <asp:TextBox ID="licenseID" runat="server" style="display:none"></asp:TextBox>    
                 </td>
               </tr>           
        <tr>
            <td style="text-align:right; width:20%;">
                Company:
                &nbsp&nbsp&nbsp
            </td>
            <td style="text-align:left" colspan="2">
                <telerik:RadTextBox ID="txtCompany" runat="server" MaxLength="40" onblur="capitilizeInitial(this)"
                    Style="width: 300px; height: 18px" Skin="Allscripts" EnableEmbeddedSkins="false"
                    CssClass="txtboxcolor">
                </telerik:RadTextBox>
            </td>
        </tr>
        <tr>
            <td style="text-align:right; width:20%;">
                Email:<span style="color: Red">*</span>
                &nbsp&nbsp&nbsp
            </td>
            <td style="text-align:left" colspan="2">
                <telerik:RadTextBox ID="txtEmail" runat="server" MaxLength="40" onblur="capitilizeInitial(this)"
                    Style="width: 250px; height: 18px" Skin="Allscripts" EnableEmbeddedSkins="false"
                    CssClass="txtboxcolor">
                </telerik:RadTextBox>               
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a Email."><b style="color:Red">*</b>
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="text-align:right; width:20%;">
                Confirm Email:<span style="color: Red">*</span>
                &nbsp&nbsp&nbsp
            </td>
            <td style="text-align:left" colspan="2">
                <telerik:RadTextBox ID="txtEmailConfirm" runat="server" MaxLength="40" onblur="capitilizeInitial(this)"
                    Style="width: 250px; height: 18px" Skin="Allscripts" EnableEmbeddedSkins="false"
                    CssClass="txtboxcolor">
                </telerik:RadTextBox>               
                <asp:RequiredFieldValidator ID="rfvEmailConfirm" runat="server" ControlToValidate="txtEmailConfirm" ValidationGroup="maingroup"
                ErrorMessage="Please enter a Confirm Email."><b style="color:Red">*</b>
                </asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvConfirmEmail" runat="server" ControlToCompare="txtEmail" ValidationGroup="maingroup" ErrorMessage="Text in the confirm Email field should match the Email field."
                    ControlToValidate="txtEmailConfirm"><b style="color:Red">*</b></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td style="text-align:right; width:20%;">
                Phone:<span style="color: Red">*</span>
                &nbsp&nbsp&nbsp
            </td>
            <td style="text-align:left" colspan="2">
                <telerik:RadMaskedTextBox ID="txtPhone" runat="server" Style="width: 150px; height: 18px"
                    Skin="Allscripts" EnableEmbeddedSkins="false" EmptyMessage="(###).###.####" Mask="(###).###.####"
                    DisplayMask="(###).###.####" DisplayPromptChar=" " CssClass="txtboxcolor">
                </telerik:RadMaskedTextBox>               
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone" ValidationGroup="maingroup"
                    ErrorMessage="Please enter a Phone."><b style="color:Red">*</b>
                </asp:RequiredFieldValidator>
            </td>
        </tr>     
        <tr>
            <td colspan="3">
                <asp:ValidationSummary ID="valsum" runat="server" ValidationGroup="maingroup" />
            </td>
        </tr>                        
        <tr>
            <td style="text-align:right; width:20%;">               
            </td>
            <td style="text-align:left" colspan="2">
               <asp:Button ID="btCont" runat="server" Text="Contact Me" OnClick="btCont_Click" CssClass="btnstyle" Style="padding-bottom:3px; font-size:13px;" CausesValidation="true" ValidationGroup="maingroup"/>
            </td>
        </tr>
    </table>
</div>
<!-- Disclaimer Modal -->
<div id="disclaimerModal" class="modal fade" role="dialog">
    <div class="modal-dialog" style="width: 90%; max-width: 670px;">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-body">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <div id="div1" style="padding:10px; margin:0px;">
                    <p>*Listed price is for normal installation. Normal installation requires that we have access to the database computer with 
                    patient demographics, any necessary logins/passwords, and ODBC database drivers. Unforeseen barriers or unusual environments 
                    may require professional services at additional cost.  These would include but are not limited to:</p>
                    <ul style="padding:0px 10px 10px 20px;">
                        <li>
                            Database server off-site and maintained by IT service provider who do not allow access to system.
                        </li>
                        <li>
                            Server which is underpowered including issues such as insufficient memory or CPU resources to run the HyperBridge interface software.
                        </li>
                        <li>
                            Unreliable, underpowered or "locked-down" Internet connection.
                        </li>
                        <li>
                            Non-standard or obsolete database structure.
                        </li>
                        <li>
                            Special requirements such as using a non-standard patient ID or needs to filter out certain patient records based on specialized requirements.
                        </li>
                        <li>
                            Erroneous demographic data content, such as multiple patients with the same ID.
                        </li>                                                            
                        <li>
                            Any non-HyperBridge method of transport (i.e. HL7 file drop, CSV file drop, TCP/IP (MLLP) HL7 message).
                        </li>  
                        <li>
                            Any requests for changes and/or modifications to the default poll times.
                        </li>                          
                    </ul>
                    <asp:Button ID="Button1" runat="server" Text="Close" CssClass="btnstyle" />
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>



