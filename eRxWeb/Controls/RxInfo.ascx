<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RxInfo.ascx.cs" Inherits="eRxWeb.Controls_RxInfo" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<div id="divRxInfo" runat="server">
    <table style="width: 100%; margin-left: -12px;">
        <tr>
            <td class="couponCell1" style="border: none !important"></td>
            <td class="couponCell2" style="border: none !important"></td>
            <td class="couponCell3">
                <div>
                    <p>
                        <asp:LinkButton ID="lbViewRxInfo" runat="server" Text="RxInfo 1"
                            ToolTip="Click to view RxInfo." OnClick="lbViewRxInfo_Click" CssClass="couponLink"></asp:LinkButton>
                        <input id="hiddenRxInfoDetailsID" runat="server" type="text" style="display: none;" />
                    </p>
                </div>
            </td>
            <td class="couponCell4">
                <asp:CheckBox ID="chkRxInfoPrint" runat="server" Text="Print" Checked="true" CssClass="offerCB" />
            </td>
            <td style="border-bottom: none !important"></td>
            <td style="border: none !important"></td>
            <td style="border: none !important"></td>
        </tr>
    </table>
</div>


