<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PhysicianMasterPageBlank.master" Title="Tutorials" Inherits="eRxWeb.Tutorials" Codebehind="Tutorials.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>

<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script language="javascript" type="text/javascript" >
    function openTutorial(URL)
    {
        window.open(URL);
    }
</script>
<table width="100%" border="0" cellspacing="0" cellpadding="5">
    <tr>
        <td>
            <table>
                <tr><td>
                    <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
                    <h3>You have tutorials available.</h3>
                    The following list of tutorials is designed to help you utilize the core features of Veradigm ePrescribe™. The tutorials will give you a good understanding of the ePrescribe application. If you do not have time right now, you can view these tutorials whenever you wish by visiting the Help section and going to the Tutorials tab.</td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table width="75%" height="<%=((PhysicianMasterPageBlank)Master).getTableHeight() %>">
                <tr>
                    <td>
                        <asp:GridView ID="tutorialsGridView" runat="server" DataSourceID="tutorialsObjectDataSource" AutoGenerateColumns="False" DataKeyNames="TutorialID" CellPadding="4" ForeColor="#333333" GridLines="None" OnRowCommand="tutorialsGridView_RowCommand" OnRowDataBound="tutorialsGridView_RowDataBound">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                    <asp:Label ID="lblTutorialText" runat="server" Text='<%# Bind("TutorialText") %>'></asp:Label></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="75px" ItemStyle-HorizontalAlign="center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkBtnTutorial" runat="server" Text="View" CommandArgument='<%# Bind("TutorialID") %>' CommandName="View"></asp:LinkButton>
                                        </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="125px">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkDontShowAgain" runat="server" Text="Don't show again" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                        <br />
                        <asp:Button ID="btnProceed" runat="server" Text="Skip >>" OnClick="btnProceed_Click" CssClass="btnstyle" />
                        <asp:ObjectDataSource ID="tutorialsObjectDataSource" runat="server" SelectMethod="GetTutorials"
                            TypeName="Allscripts.Impact.RxUser" OldValuesParameterFormatString="original_{0}">
                            <SelectParameters>
                                <asp:SessionParameter Name="userID" SessionField="UserID" Type="String" />
                                <asp:SessionParameter Name="IsProvider" SessionField="IsProvider" Type="String" />
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
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <contenttemplate>        
         <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex=0 runat="server" ContentCssClass="accordionContent" HeaderCssClass="accordionHeader">
                        <Panes>
                            <ajaxToolkit:AccordionPane ID=paneHelp runat="server">
                            <Header>Help With This Screen</Header>
                            <Content>
                                <asp:Panel ID=HelpPanel runat=server></asp:Panel>
                               </Content>
                            </ajaxToolkit:AccordionPane>                            
                        </Panes>
                    </ajaxToolkit:Accordion>
        </contenttemplate>
    </asp:UpdatePanel>
</asp:Content>
