<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.FullScriptEdit"
    MasterPageFile="~/PhysicianMasterPageBlank.master" Title="Edit Favorites" ViewStateEncryptionMode="Never"
    EnableViewStateMac="false" Codebehind="FullScriptEdit.aspx.cs" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">    
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr class="h1title">
            <td colspan="2">
                <table border="0" cellpadding="0" width="100%">
                    <tr>
                        <td>
                            <span class="indnt Phead">Delete Favorites </span>
                        </td>
                        <td colspan="2">                 
                            <table border=0 cellpadding=0 cellspacing=0>         
                            <tr><td>                   
                            <table border=0 style="border-collapse:collapse;" bordercolor=#E7E7E7 cellpadding=2 cellspacing=0>
                            <tr><td>
                            <asp:TextBox autocomplete="off" MaxLength="50" ID="txtSearchMed" CssClass="searchTextBox"
                                Width="185px" ToolTip="Enter Partial or Full Medication" runat="server"></asp:TextBox>
                            <ajaxToolkit:AutoCompleteExtender ID="aceMed" runat="server" TargetControlID="txtSearchMed"
                                ServiceMethod="queryMeds" ServicePath="erxnowmed.asmx" CompletionInterval="1000"
                                MinimumPrefixLength="3" EnableCaching="true">
                            </ajaxToolkit:AutoCompleteExtender>
                            </td></tr></table>
                            </td><td width=5>&nbsp;&nbsp;</td><td>
                            <table border=1 style="border-collapse:collapse;" bordercolor=#E7E7E7 cellpadding=2 cellspacing=0>
                            <tr><td>
                            <asp:Button ID="btnGo" ToolTip="Shows all medications matching the text entered in the Search field. Enter partial or full medication name in the text-box."
                                runat="server" Text="Search" OnClick="btnGo_Click" CssClass="btnstyle"
                                Width="80px" />                              
                                </td></tr></table>
                                </td><td width="5">&nbsp;&nbsp;</td>
                                <td>
                                </td></tr>
                                </table>                                       
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="message">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />            
            </td>
        </tr>
        <tr class="h2title">
            <td align="left" nowrap="nowrap" style="height: 20px; width: 50%">
               &nbsp;
            </td>
            <td align="right" nowrap="nowrap" style="height: 20px; width: 50%">
                <asp:Label ID="lblMsg" CssClass="errormsg" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table border="1" width="100%" align="center" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr class="h4title">
                                    <td colspan="7" style="height: 19px">                                    
                                        <asp:Button ID="btnCancel" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnCancel_Click" />
                                        <asp:Button ID="btnDelete" runat="server" CssClass="btnstyle" Text="Delete Selected" OnClick="btnDelete_Click" /></td>
                                        
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>">
                        <td>                            
                            <asp:GridView ID="grdViewMed" runat="server" AllowPaging="True" AllowSorting="True" EnableViewState ="true"
                                PageSize="50" AutoGenerateColumns="False" Width="100%" CaptionAlign="Left" GridLines="None"
                                EmptyDataText="No favorite meds exist"
                                OnRowDataBound="grdViewMed_RowDataBound" DataKeyNames="RxUsageID,DDI,SIGID,IsGeneric,SIGText,Medication"
                                DataSourceID="MedObjDataSource" OnRowCreated="grdViewMed_RowCreated" OnSorting="grdViewMed_OnSorting">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <input name="rdSelectRow" type="checkbox" value='<%# ObjectExtension.ToEvalEncode(Eval("RxUsageID")) %> %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="30px" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>                                  
                                    <asp:TemplateField HeaderText="Medication And Sig" SortExpression="Medication,SIGText" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <div>
                                                <asp:Literal ID="litMedicationAndSig" runat="server"></asp:Literal>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQuantity" CssClass="txtboxcolor" Enabled="false" Width="45px" runat="server"
                                                Text='<%# Bind("Quantity")%>' MaxLength="8"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="DAW">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkDAW" Enabled="false" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="30px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Refills">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtRefill" Width="30px" Enabled="false" runat="server" Text='<%# Bind("RefillQuantity") %>'
                                                MaxLength="2"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Days">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtDays" Width="30px" Enabled="false" runat="server" Text='<%# Bind("Dayssupply") %>'
                                                MaxLength="3"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HiddenField ID="RxUsageID" runat="server" Value='<%#Bind("RxUsageID")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                                
                                     <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HiddenField ID="SIGID" runat="server" Value='<%#Bind("SIGID")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <!--</DIV>-->
                            <asp:ObjectDataSource ID="MedObjDataSource" runat="server" SelectMethod="GetProviderFavorites"
                                TypeName="Allscripts.Impact.Medication" OldValuesParameterFormatString="original_{0}"
                                OnSelected="MedObjDataSource_Selected" OnSelecting="MedObjDataSource_Selecting">
                                <SelectParameters>
                                    <asp:SessionParameter Name="ProviderId" SessionField="USERID" Type="String" />
                                    <asp:ControlParameter ControlID="txtSearchMed" DefaultValue="''"
                                        Name="MedName" PropertyName="Text" Type="String" />                                    
                                     <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>  
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">    
          <%--   <!--start help panel -->
            <asp:Panel ID="panelHelpHeader" class="accordionHeader" runat="server" Width="95%">
                <table cellspacing="0" cellpadding="0" width="95%" border="0">
                    <tbody>
                        <tr>
                            <td align="left" width="160">
                                <div id="Header" class="accordionHeaderText">
                                    Help With This Screen</div>
                            </td>
                            <td align="right" width="14">
                                <asp:Image ID="hlpclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;</td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="panelHelp" class="accordionContent" runat="server" Width="92%">
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="cpeHelp" runat="server" TargetControlID="panelHelp"
                Collapsed="true" CollapsedSize="0" ExpandControlID="panelHelpHeader" CollapseControlID="panelHelpHeader"
                ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png" ExpandedImage="images/chevronup-nor-light-16-x-16.png"
                ImageControlID="hlpclpsimg" SuppressPostBack="true">
            </ajaxToolkit:CollapsiblePanelExtender>
            <!--end help panel -->
    *Brand drugs are in <b>BOLD</b><br />--%>
</asp:Content>
