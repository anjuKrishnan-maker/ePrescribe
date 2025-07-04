<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="eCoupon.ascx.cs" Inherits="eRxWeb.Controls_eCoupon" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<%@ Import Namespace="Allscripts.ePrescribe.Objects" %>
<script type="text/javascript">    
            function ToggleSendToPharmacy(ddldest, clientIdCheckSendOfferToPharmNotes, clientIdCheckSendToPharm) {
                var chkboxClientIdCheckSendOfferToPharmNotes = document.getElementById(clientIdCheckSendOfferToPharmNotes);
                if (chkboxClientIdCheckSendOfferToPharmNotes != null) {
                    if (ddldest.value !== '<%=Constants.ScriptPadDestinations.PHARM.ToString() %>' && ddldest.value !== '<%=Constants.ScriptPadDestinations.DOCPHARM.ToString()%>')
                    {
                        chkboxClientIdCheckSendOfferToPharmNotes.disabled = true;
                    }
                    else
                    {
                        chkboxClientIdCheckSendOfferToPharmNotes.removeAttribute('disabled');
                        chkboxClientIdCheckSendOfferToPharmNotes.parentNode.removeAttribute('disabled');
                    }
                    chkboxClientIdCheckSendOfferToPharmNotes.checked = false;
                }

                var chkboxClientIdCheckSendToPharm = document.getElementById(clientIdCheckSendToPharm);
                if (chkboxClientIdCheckSendToPharm != null) {
                    if (ddldest.value !== '<%=Constants.ScriptPadDestinations.PHARM.ToString() %>' &&
                        ddldest.value !== '<%=Constants.ScriptPadDestinations.DOCPHARM.ToString()%>') {
                        chkboxClientIdCheckSendToPharm.checked = false;
                        chkboxClientIdCheckSendToPharm.disabled = true;
                    }
                    else
                    {
                        chkboxClientIdCheckSendToPharm.checked = true;
                        chkboxClientIdCheckSendToPharm.removeAttribute('disabled');
                        chkboxClientIdCheckSendToPharm.parentNode.removeAttribute('disabled');
                    }
                }
    }
    function CheckECoupon_Clicked(chkboxGoodRx, chkboxECoupon) {
        var chkboxECouponControl = document.getElementById(chkboxECoupon);
        var chkboxGoodRxControl = document.getElementById(chkboxGoodRx);
        if (chkboxECouponControl != null && chkboxGoodRxControl != null) {
            if (chkboxECouponControl.checked === true) {
                chkboxGoodRxControl.checked = false;
            }
        }
        if ('<%=EcouponWarningOverlay%>' === '<%=Visibility.Visible%>'
            && chkboxECouponControl != null && !chkboxECouponControl.checked) {
            chkboxECouponControl.checked = true;
            window.parent.ShowEcouponUncheckedWarning(chkboxECoupon);
        }
    }
</script>
<div id="divApplyCoupons" runat="server" visible="false">
    <table style="width: 100%; margin-left: -12px;">
        <tr>
            <td class="couponCell1" style="border: none !important"></td>
            <td class="couponCell2" style="border: none !important"></td>
            <td class="couponCell3">
                <div>
                    <p>
                        <asp:LinkButton ID="lbViewOffer" runat="server" Text="eCoupon - " CssClass="couponLink"
                            ToolTip="Click to view offer."></asp:LinkButton>
                        <span style="margin: 0 5px 0 5px;">| </span>
                        <asp:LinkButton ID="lbPrescribingInfo" runat="server" Text="Prescribing Information"
                            CssClass="couponLink" ToolTip="Click to view prescribing information." OnClick="lbPrescribingInfo_Click"></asp:LinkButton>
                    </p>
                </div>
            </td>
            <td class="couponCell4">
                <asp:CheckBox ID="chkPrintOffers" Checked="true" runat="server" Text="Print coupon" CssClass="offerCB"  />
                <br />
                <asp:CheckBox ID="chkOffersPharmacyNotes" Checked="true" runat="server" Text="Send coupon to pharmacy" CssClass="offerCB"/>
                &nbsp;</td>
            <td style="border-bottom: none !important"></td>
            <td style="border: none !important"></td>
            <td style="border: none !important"></td>
        </tr>
    </table>
</div>

