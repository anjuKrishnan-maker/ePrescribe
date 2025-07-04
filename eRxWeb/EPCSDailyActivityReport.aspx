<%@ Page Title="" Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master"
    AutoEventWireup="true" CodeBehind="EPCSDailyActivityReport.aspx.cs" Inherits="eRxWeb.EPCSDailyActivityReport" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="h1title indnt">
        <asp:Label ID="lblReportTitle" runat="server" Text="EPCS Daily Activity Report" CssClass="Phead"></asp:Label>
    </div>
    <div style="height: 220px;">
        <div style="float: left; margin-left: 20px;">
            <h3>
                Select a report from the last 7 days:
            </h3>
            <asp:Repeater ID="rptLast7Days" runat="server">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li style="margin-top: 5px">
                        <asp:LinkButton ID="lbReportDate" runat="server" Text='<%# Container.DataItem %>'
                            CommandArgument='<%# Container.DataItem %>' OnCommand="lbReportDate_Command"></asp:LinkButton>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="float: left; margin-left: 40px; margin-right: 40px;">
            <%--<div class="vertical-line" style="height: 30%">
                </div>--%>
            <div style="line-height: 220px; font-weight: bold">
                OR
            </div>
            <%--<div class="vertical-line" style="height: 30%">
                </div>--%>
        </div>
        <div style="float: left">
            <h3>
                Select a day from the calendar:
            </h3>
            <asp:Calendar ID="calReportDate" runat="server" 
                onselectionchanged="calReportDate_SelectionChanged" 
                ondayrender="calReportDate_DayRender">
            </asp:Calendar>
        </div>
        <div style="clear: both">
        </div>
    </div>
    <div class="h3title" style="height: auto;">
        <asp:Button ID="btnBack" CssClass="btnstyle" runat="server" Text="Back" OnClick="btnBack_Click"
            CausesValidation="false" TabIndex="5" />
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>
