<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.Help_faq" Title="Untitled Page" Codebehind="faq.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">
        function showMoreF() {
            var sessionUserId = document.getElementById("<%=hdnFldSessionUserId.ClientID %>").value;

            var FAQ_NOT_TOP_FIVE = document.getElementById("<%=notTopFive.ClientID %>");
            if (FAQ_NOT_TOP_FIVE != null) {
                FAQ_NOT_TOP_FIVE.style.display = "inline";
            }

            var showLessA = document.getElementById("<%=showLess.ClientID %>");
            if (showLessA != null) {
                showLessA.style.display = "inline";
            }

            var showMoreA = document.getElementById("<%=showMore.ClientID %>");
            if (showMoreA != null) {
                showMoreA.style.display = "none";
            }

            var divNoAdditionalFbk = document.getElementById("<%=divNoAdditionalFeedback.ClientID %>");
            if (sessionUserId == 0) {
                divNoAdditionalFbk.style.display = "block";

            }
            else {

                divNoAdditionalFbk.style.display = "none";
            }
        }

        function showLessF() {
            var divNoAdditionalFbk = document.getElementById("<%=divNoAdditionalFeedback.ClientID %>");
            divNoAdditionalFbk.style.display = "none";
            var FAQ_NOT_TOP_FIVE = document.getElementById("<%=notTopFive.ClientID %>");
            if (FAQ_NOT_TOP_FIVE != null) {
                FAQ_NOT_TOP_FIVE.style.display = "none";
            }

            var showLessA = document.getElementById("<%=showLess.ClientID %>");
            if (showLessA != null) {
                showLessA.style.display = "none";
            }

            var showMoreA = document.getElementById("<%=showMore.ClientID %>");
            if (showMoreA != null) {
                showMoreA.style.display = "inline";
            }
        }    
    </script>
    <div id="TopFive" runat="server">
        <asp:Repeater runat="server" ID="FAQ_TOP_FIVE" DataSourceID="DS_TOP_FIVE">
            <ItemTemplate>
                <span class="FAQ_Question">
                    <%--<%# Eval("ListOrder")%>. --%><%# Eval("Question") %>
                </span>
                <br />
                <span class="FAQ_Answer">
                    <%# Eval("Answer") %>
                </span>
                <br />
                <br />
            </ItemTemplate>
        </asp:Repeater>
        <asp:ObjectDataSource ID="DS_TOP_FIVE" runat="server" SelectMethod="GetHelpFAQ" TypeName="Allscripts.Impact.SystemConfig">
            <SelectParameters>
                <asp:Parameter Name="Top" DefaultValue="-5" Type="Int32" />
                <asp:Parameter Name="Active" DefaultValue="Y" Type="String" />
                <asp:Parameter Name="Category" DefaultValue="GENERAL" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    <br />
    <span class="FAQ_Additional">Have additional question?</span>&nbsp&nbsp<span class="FAQ_Toggle"><a
        id="showMore" runat="server" onclick="showMoreF()" style="cursor: pointer">Show
        additional FAQs</a><a id="showLess" runat="server" onclick="showLessF()" style="display: none;
            cursor: pointer">Hide additional FAQs</a></span>
    <br />
    <br />
    <div id="notTopFive" runat="server" style="display: none;">
        <asp:Repeater runat="server" ID="FAQ_NOT_TOP_FIVE" DataSourceID="DS_NOT_TOP_FIVE"
            OnItemDataBound="FAQ_NOT_TOP_FIVE_ItemDataBound">
            <ItemTemplate>
                <span class="FAQ_Question">
                    <%-- <%# Eval("ListOrder") %>.--%>
                    <%# Eval("Question") %>
                </span>
                <br />
                <span class="FAQ_Answer">
                    <%# Eval("Answer") %>
                </span>
                <br />
                <br />
            </ItemTemplate>
        </asp:Repeater>
        <asp:ObjectDataSource ID="DS_NOT_TOP_FIVE" runat="server" SelectMethod="GetHelpFAQ"
            TypeName="Allscripts.Impact.SystemConfig">
            <SelectParameters>
                <asp:Parameter Name="Top" DefaultValue="5" Type="Int32" />
                <asp:Parameter Name="Active" DefaultValue="Y" Type="String" />
                <asp:Parameter Name="Category" DefaultValue="GENERAL" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    <div id="divNoAdditionalFeedback" runat="server" style="display: none;">
        <br />
        <asp:HiddenField ID="hdnFldSessionUserId" runat="server" />
        <span class="FAQ_Additional">To see additional FAQ's, Please login above</span>
    </div>
</asp:Content>
