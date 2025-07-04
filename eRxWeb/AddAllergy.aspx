<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.AddAllergy"  Title="Add Allergy" Codebehind="AddAllergy.aspx.cs" %>
<%@ Import Namespace="eRxWeb" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">


<script language="javascript" type="text/javascript">
    
    var prevRow;
    var savedClass;
    function init()
    {
        var imgAddAllerg = document.getElementById("ctl00_ContentPlaceHolder1_btnAddAllergy");
        imgAddAllerg.disabled = true;
    }
    function onRowClick(row)
    {       
        var imgAddAllergy = document.getElementById("ctl00_ContentPlaceHolder1_btnAddAllergy");
        imgAddAllergy.disabled = false;
        
        if(prevRow!=null)
        {
            prevRow.className=savedClass;
        }
        savedClass=row.className;
        row.className='SelectedRow';
        prevRow=row;
        cleanWhitespace(row);
        row.firstChild.childNodes[0].checked=true;   
   
    }
    
    
    function cleanWhitespace(node)
    {
        for (var x = 0; x < node.childNodes.length; x++) 
        {
            var childNode = node.childNodes[x]
            if ((childNode.nodeType == 3)&&(!/\S/.test(childNode.nodeValue))) 
            {   
                // that is, if it's a whitespace text node
                node.removeChild(node.childNodes[x])
                x--
            }
            if (childNode.nodeType == 1) 
            {
                // elements can have text child nodes of their own
                cleanWhitespace(childNode)
            } 
        }
    }

    function btnBack_onclick() {
        window.location.href='PatientAllergy.aspx';
    }

    function performMedSearchOnKeyPress(e) {
	    var key = window.event ? e.keyCode : e.which;
	
	    if (key == 13)
	    {
		//First cancel the event bubbling, so the form will not catch the enter key event.
	    	if (window.event)
    			window.event.cancelBubble = true;
		    else
			    e.stopPropagation();
		    performMedSearch();
	    	return false;
    	}
	    else
    		return true;
    }
    function performMedSearch() {
	    var searchButton = document.getElementById("<%=btnSearch.ClientID %>");
	    var searchInput = document.getElementById("<%=txtSearchText.ClientID %>");
	
	    if (searchInput.value.length > 0)
	    {
		    searchButton.click();
	    }
    }
        function setContextKey(element) {
        // clear search criteria text box
        var searchTextBox = $get('<%=txtSearchText.ClientID %>');
//        if (typeof(searchTextBox) !== 'undefined' && searchTextBox != null)
//            searchTextBox.value = '';
        
        // get autoCompleteExtender control
        var autoCompleteExtender = $find('aceBehavior');
        if (!autoCompleteExtender)
            return;
        
        // clear any cached results
        autoCompleteExtender._cache = null;
        
        // set context key to class or meds
        autoCompleteExtender.set_contextKey(element.value);
           
        return;
    }

    </script>

     <div class="h1title Phead" style="position: relative; height: 24px;">
        Choose Allergen
        <asp:RadioButton ID="rbClass" Visible="true" runat="server" GroupName="Search" Text="Class"
            Width="65px" Checked="True" CssClass="adminlink1" Style="vertical-align: baseline;" />
        <asp:RadioButton ID="rbMedication" Visible="true" runat="server" GroupName="Search"
            Text="Medication" Width="95px" CssClass="adminlink1" Style="vertical-align: baseline;" />
        <img src="images/searchBoxImage.png" class="searchControlImage" />
        <asp:TextBox MaxLength="30" onkeypress="return performMedSearchOnKeyPress(event);"
            Style="width: 270px;" ID="txtSearchText" runat="server" meta:resourcekey="txtSearchMedResource1"
            onblur="if (this.value === '') {this.value = this.getAttribute('defaultValue');this.style.color = 'rgb(102, 102, 102)';}" 
            onfocus ="if (this.value === this.getAttribute('defaultValue')) {this.value = '';this.style.color = 'black';}"
            CssClass="searchControlTextBox"
            defaultValue="Search Class/Meds"
            value="Search Class/Meds"
            ToolTip="Enter medication name or class and click the Search button.">
        </asp:TextBox>
        <ajaxToolkit:AutoCompleteExtender ID="aceMed" runat="server" BehaviorID="aceBehavior"
            TargetControlID="txtSearchText" ServiceMethod="queryClassMeds" ServicePath="erxnowmed.asmx"
            CompletionInterval="1000" MinimumPrefixLength="3" EnableCaching="true" UseContextKey="true"
            CompletionListCssClass="completionList">
        </ajaxToolkit:AutoCompleteExtender>
        <asp:Button ID="btnSearch" CssClass="searchControlButton" runat="server" Text="GO" Width="40px"
            Style="vertical-align: baseline;" meta:resourcekey="btnGoResource1" ToolTip="Click to get list of medications matching the name entered in the Search field. If class is entered then all medications matching the class are displayed in the table."
            UseSubmitBehavior="False" OnClick="btnSearch_Click" />
    </div>
    <div class="h2title"></div>
    <div>
        <div class="h4title" style="height: 25px;">
            <span style="display: table-cell; vertical-align: middle; height: inherit;">
                <input id="btnBack" class="btnstyle" type="button" value="Back" onclick="return btnBack_onclick()" style="margin-left: 5px;" />
                <asp:Button ID="btnAddAllergy" runat="server" CssClass="btnstyle" OnClick="btnAddAllergy_Click"
                    Style="margin-left: 5px;" Text="Save" Enabled="False" ToolTip="Click here to add the selected medication to the list of medications to which this patient is allergic." />                
            </span>
        </div>
        <div>
            <table border="1" cellspacing="0" style="width: 100%; border-color: #b5c4c4">
                <tr height="<%=((PhysicianMasterPage)Master).getTableHeight() %>">
                    <td>
                        <table cellspacing="0" cellpadding="0" width="100%">
                            <tr height="<%=getGridHeight() %>">
                                <td>
                                    <asp:GridView ID="grdViewMed" runat="server" AllowPaging="True" AllowSorting="True"
                                        PageSize="50" Width="100%" AutoGenerateColumns="False" CaptionAlign="Left" DataKeyNames="DDI"
                                        EmptyDataText="No results found for the Search. Please enter new search criteria."
                                        OnRowDataBound="grdViewMed_RowDataBound" OnRowCreated="grdViewMed_RowCreated"
                                        meta:resourcekey="grdViewMedResource1">
                                        <Columns>
                                            <asp:TemplateField meta:resourcekey="TemplateFieldResource1">
                                                <ItemTemplate>
                                                    <input name="rbSelectRow" type="radio" value='<%# ObjectExtension.ToEvalEncode(Eval("DDI")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Name" HeaderText="Drug Name" SortExpression="Name" meta:resourcekey="BoundFieldResource1" />
                                            <asp:BoundField DataField="DosageForm" HeaderText="Dosage Form" meta:resourcekey="BoundFieldResource2" />
                                            <asp:BoundField DataField="RouteofAdmin" HeaderText="Route" meta:resourcekey="BoundFieldResource3" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:ObjectDataSource ID="MedObjDataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetMedicationsAndClasses" TypeName="Allscripts.Impact.Medication"
                                        OnSelecting="MedObjDataSource_Selecting">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="txtSearchText" Name="MedName" PropertyName="Text" />
                                            <asp:ControlParameter ControlID="rbMedication" DefaultValue="" Name="MedSearch" PropertyName="Checked" />
                                            <asp:ControlParameter ControlID="rbClass" Name="ClassSearch" PropertyName="Checked" />
                                            <asp:SessionParameter Name="PatientID" SessionField="PatientID" />
                                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%;">
                                    <table cellspacing="0" style="width: 100%">
                                        <tr>
                                            <td colspan="2" bgcolor="#FFFFFF" class="head" style="display: none">
                                                Active</td>
                                            <td height="30" style="display: none">
                                                <span class="head" visible="false">&nbsp;<asp:CheckBox ID="chkActive" runat="server"
                                                    Checked="True" meta:resourcekey="chkActiveResource1" />
                                                </span>
                                            </td>
                                            <td style="display: none">
                                            </td>
                                            <td style="display: none">
                                            </td>
                                        </tr>
                                        <tr class="valgn">
                                            <td colspan="2" bgcolor="#FFFFFF" class="adminlink indnt valgn">
                                                Date when allergy first noticed/reported:<asp:TextBox ID="txtStartDate" runat="server"
                                                    MaxLength="10" Width="100px" meta:resourcekey="txtStartDateResource1" ToolTip="Enter date when allergy was first noticed or reported."></asp:TextBox>
                                                (mm/dd/yyyy)
                                                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate"
                                                    ErrorMessage="Start Date is required" Text="Please enter a valid Start Date" />
                                                <asp:RadioButton ID="rbAllergy" runat="server" Text="Allergy" Width="67px" Checked="True"
                                                    GroupName="Category" />
                                                <asp:RadioButton ID="rbIntolerance" runat="server" Text="Intolerance" Width="93px"
                                                    GroupName="Category" />
                                                <asp:RegularExpressionValidator ID="revStartDate" runat="server" ControlToValidate="txtStartDate"
                                                    Text="Please enter a valid start date. " ValidationExpression="^(?:(?:(?:0?[13578]|1[02])(\/|-|\.)31)\1|(?:(?:0?[13-9]|1[0-2])(\/|-|\.)(?:29|30)\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:0?2(\/|-|\.)29\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:1[0-2]))(\/|-|\.)(?:0?[1-9]|1\d|2[0-8])\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$"
                                                    meta:resourcekey="revStartDateResource1" Display="Dynamic"></asp:RegularExpressionValidator>
                                                <asp:RangeValidator ID="rvStartDate" ControlToValidate="txtStartDate" Type="Date"
                                                    runat="server" ErrorMessage="Allergy start date must be between patient's dob ({0}) and today ({1})"
                                                    Display="Dynamic"></asp:RangeValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" bgcolor="#FFFFFF" class="head" style="display: none">
                                                End Date</td>
                                            <td width="500" height="30" style="display: none">
                                                <asp:TextBox ID="txtEndDate" runat="server" MaxLength="10" meta:resourcekey="txtEndDateResource1"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="revEndDate" runat="server" ControlToValidate="txtEndDate"
                                                    ErrorMessage="Please Enter a Valid End Date  (MM/DD/YYYY)" ValidationExpression="(0*[1-9]|1[012])[- /.](0*[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d"
                                                    Width="1px" meta:resourcekey="revEndDateResource1">*</asp:RegularExpressionValidator>(MM/DD/YYYY)</td>
                                            <td width="107" style="display: none">
                                                &nbsp;</td>
                                            <td width="77" style="display: none">
                                                &nbsp;</td>
                                        </tr>
                                        <tr bgcolor="#B5B4B4">
                                            <td colspan="5" class="head indnt">
                                                Reaction</td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <asp:ListBox ID="lstReactions" runat="server" SelectionMode="Multiple" Width="430px"
                                                    Height="136px" meta:resourcekey="lstReactionsResource1"></asp:ListBox>
                                            </td>
                                            <td >
                                                <asp:Label ID="lblMultipleAllergyMsg" runat="server" 
                                                    Text="<br/>If you would like to enter more than one reaction for a Class or Medication, select (highlight) the first reaction from the list. Then hold down the Control Key (Ctrl) and continue to select other reactions. When done, click the Save button. All selected reactions will be associated to the Class or Medication selected."></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5" bgcolor="#FFFFFF" class="head" style="height: 68px">
                                                <table>
                                                    <tr>
                                                        <td class="adminlink">
                                                            &nbsp;
                                                            <%--  <asp:CheckBox ID="chkOther" runat="server" Width="564px" CssClass="head" Text="Please click this checkbox, if Reaction not listed above(enter description below)"
                                                                                                    meta:resourcekey="chkOtherResource1" ToolTip="Click this checkbox if you wish to enter a reaction in the text-box given below; in case the reaction is not listed in the list  above." />&nbsp;
                                                                                             --%>
                                                            If Reaction not listed above,enter description below.
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="head indnt">
                                                            <asp:TextBox ID="txtOther" runat="server" MaxLength="255" Width="420px" meta:resourcekey="txtOtherResource1"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                                <%--<asp:ValidationSummary ID="ValSummary" runat="server" Width="426px" meta:resourcekey="ValSummaryResource1" />--%>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
        </asp:Content>

<asp:Content ID="Content3" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
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

