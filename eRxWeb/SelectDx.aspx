<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.SelectDx"
    MasterPageFile="~/PhysicianMasterPage.master" Title="Choose Diagnosis" Codebehind="SelectDx.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        var prevRow;
        var savedClass;

        function onRowClick(row) {

            cleanWhitespace(row);
            row.firstChild.childNodes[0].checked = true;

            if (prevRow != null) {
                prevRow.className = savedClass;
            }
            savedClass = row.className;
            row.className = 'SelectedRow';
            prevRow = row;

            var dx;
            dx = row.childNodes[2].innerHTML;
            if (dx != null) {
                var googleSearch = document.getElementById("searchbox_007346535748368836231:sbx1cfoc2pg");

                if (googleSearch != null) {
                    setSearch(dx);
                }
            }
        }
        function cleanWhitespace(node) {
            for (var x = 0; x < node.childNodes.length; x++) {
                var childNode = node.childNodes[x]
                if ((childNode.nodeType == 3) && (!/\S/.test(childNode.nodeValue))) {
                    // that is, if it's a whitespace text node
                    node.removeChild(node.childNodes[x])
                    x--
                }
                if (childNode.nodeType == 1) {
                    // elements can have text child nodes of their own
                    cleanWhitespace(childNode)
                }
            }
        }
        function closeBackDrop() {
            window.parent.CloseOverlay();
        }
        RefreshPatientHeader('<%= Session["PATIENTID"] %>');
    </script>
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td colspan="2">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr class="h1title">
                        <td>
                            <asp:Panel ID="pnlSearchDiagnosis" runat="server">
								<img src="images/searchBoxImage.png" class="searchControlImage" />
                                <asp:TextBox CssClass="searchControlTextBox" MaxLength="30" ID="txtSearchDx" ToolTip="Enter Diagnosis or ICD9"
                                    runat="server"
                                     onblur="if (this.value === '') {this.value = this.getAttribute('defaultValue');this.style.color = 'rgb(102, 102, 102)';}" 
                                    onfocus ="if (this.value === this.getAttribute('defaultValue')) {this.value = '';this.style.color = 'black';}"
                                    defaultValue="Search Diagnosis"
                                    Text="Search Diagnosis"
                                    ></asp:TextBox>
                                <asp:Button ID="btnGo" CssClass="searchControlButton" runat="server" Width="40" Text="GO" OnClick="btnGo_Click"
                                    ToolTip="Shows all Diagnosis matching the text entered in the above text-box. Enter partial or full Diagnosis name or  ICD code in the Search field." />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr class="h2title">
                        <td colspan="5">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--<tr class="h3title">
            <td colspan="2">
                &nbsp;<span class="Phead">Choose Diagnosis </span>
            </td>
        </tr>--%>
        <tr>
            <td colspan="2">
                <table width="100%" border="1" cellpadding="0" cellspacing="0" bordercolor="#b5c4c4">
                    <tr>
                        <td>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                <tr class="h4title">
                                    <td>
                                    </td>
                                    <td colspan="3">
                                        &nbsp;&nbsp;<input id="btnCancel" class="btnstyle" type="button" runat="server" value="Back"
                                            title="Click to go back to the 'Choose patient' page where you can choose another patient."
                                            onserverclick="btnCancel_ServerClick" />
                                        <asp:Button ID="btnSelectMed" runat="server" CssClass="btnstyle" OnClick="btnSelectMed_Click"
                                            Text="Select Med" ToolTip="Click to select a medication." />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr height="<%=((PhysicianMasterPage)Master).getTableHeight() %>">
                        <td>
                            <asp:GridView ID="grdViewDx" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" Width="100%" CaptionAlign="Left" GridLines="None"
                                EmptyDataText="No diagnosis found" DataSourceID="DxObjDataSource" OnRowDataBound="grdViewDx_RowDataBound"
                                PageSize="50" OnRowCreated="grdViewDx_RowCreated">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <input name="rdSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("ICD10Code")) %>' />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle Width="30px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ICD10Code" HeaderText="ICD10" SortExpression="ICD10Code">
                                        <ItemStyle Width="100px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description">
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                            <asp:ObjectDataSource ID="DxObjDataSource" runat="server" SelectMethod="GetDxList"
                                TypeName="Allscripts.Impact.CHDiagnosis" OldValuesParameterFormatString="original_{0}"
                                OnSelecting="DxObjDataSource_Selecting">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtSearchDx" ConvertEmptyStringToNull="False" Name="Phrase"
                                        PropertyName="Text" Type="String" />
                                    <asp:SessionParameter Name="providerID" SessionField="USERID" />
                                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel Style="display: none" ID="pnlSelectMed" runat="server">
        <table style="border-collapse: collapse; border: solid 1px black; background-color: White"
            cellpadding="5" cellspacing="20" width="450">
            <tr>
                <td>
                    You have not selected a diagnosis
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnSelectDiagnosis" CssClass="btnstyle" runat="server" Text="Select a Diagnosis" />&nbsp<asp:Button
                        ID="btnContinueWithoutDX" CssClass="btnstyle" runat="server" Text="Continue without a Diagnosis"
                        OnClick="btnContinueWithoutDX_Click" OnClientClick="closeBackDrop()"/>
                </td>
            </tr>
        </table>
        <asp:Button ID="btnHiddenTrigger" runat="server" Style="display: none;" />
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="modalSelectMedPopup" runat="server" TargetControlID="btnHiddenTrigger"
        PopupControlID="pnlSelectMed" BackgroundCssClass="modalBackground" DropShadow="false" />
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
        <div id="divHideTools_Help" runat="server">
            <ajaxToolkit:Accordion ID="sideAccordion" SelectedIndex="0" runat="server" ContentCssClass="accordionContent"
                HeaderCssClass="accordionHeader">
                <Panes>
                    <ajaxToolkit:AccordionPane ID="paneHelp" runat="server">
                        <Header>
                            Help With This Screen</Header>
                        <Content>
                            <asp:Panel ID="HelpPanel" runat="server">
                            </asp:Panel>
                        </Content>
                    </ajaxToolkit:AccordionPane>
                </Panes>
            </ajaxToolkit:Accordion>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
